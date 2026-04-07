using Cms.Contract.Common.Pagination;
using Cms.Contract.Repositories.Account.Interfaces;
using Cms.Contract.Repositories.Account.Persistence;
using Cms.Contract.Repositories.Role.Interfaces;
using Cms.Contract.Repositories.Permission.Interfaces;
using Cms.Contract.Repositories.Scope.Interfaces;
using Cms.Contract.Repositories.Department.Interfaces;
using Cms.Contract.Services.RBAC.Dtos;
using Cms.Contract.Services.RBAC.Interfaces;
using Cms.Contract.Services.UnitOfWork.Interfaces;


namespace Cms.Application.Services.RBAC
{
    public class RBACService : IRBACService
    {
        private const string ContentViewPermissionCode = "Content.View";

        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountRepository _accountRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IScopeRepository _scopeRepository;
        private readonly IDepartmentRepository _departmentRepository;

        public RBACService(
            IUnitOfWork unitOfWork,
            IAccountRepository accountRepository,
            IRoleRepository roleRepository,
            IPermissionRepository permissionRepository,
            IScopeRepository scopeRepository,
            IDepartmentRepository departmentRepository
        )
        {
            _unitOfWork = unitOfWork;
            _accountRepository = accountRepository;
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _scopeRepository = scopeRepository;
            _departmentRepository = departmentRepository;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
        {
            // 帳號沒給
            if (string.IsNullOrWhiteSpace(dto.Username))
            {
                return new LoginResponseDto
                {
                    Result = LoginResult.UsernameRequired
                };
            }

            // 密碼沒給
            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                return new LoginResponseDto
                {
                    Result = LoginResult.PasswordRequired
                };
            }

            // 取得帳號角色權限
            var rows = (await _accountRepository.GetAccountAuthByUsernameAsync(dto.Username)).ToList();

            // 找不到帳號
            if (!rows.Any())
                return new LoginResponseDto
                {
                    Result = LoginResult.AccountNotFound
                };

            // Entity 轉 Dto
            var auth = new GetAccountAuthResponseDto 
            {
                
                AccountId = rows[0].AccountId,
                Username = rows[0].Username,
                PasswordHash = rows[0].PasswordHash,

                RoleCodes = rows
                    .Select(x => x.RoleCode)                            // 只拿 RoleCodes
                    .Where(code => !string.IsNullOrWhiteSpace(code))    // 去 Null 與 空字串, IEnumerable<string?>
                    .Select(code => code!)                              // IEnumerable<string?> -> IEnumerable<string>
                    .Distinct()
                    .ToList(),

                PermissionCodes = rows
                    .Select(x => x.PermissionCode)                            
                    .Where(code => !string.IsNullOrWhiteSpace(code))    
                    .Select(code => code!)                              
                    .Distinct()
                    .ToList(),
            };

            // 驗證密碼
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, auth.PasswordHash))
                return new LoginResponseDto
                {
                    Result = LoginResult.InvalidPassword
                };

            // 更新上一次登入時間
            await _accountRepository.UpdateLastLoginAtAsync(auth.AccountId, DateTime.UtcNow);

            return new LoginResponseDto
            {
                Result = LoginResult.Success,
                AccountId = auth.AccountId,
                Username = auth.Username,
                RoleCodes = auth.RoleCodes,
                PermissionCodes = auth.PermissionCodes
            };
        }

        public async Task<PagedResult<AccountListResponseDto>> ListAccountsAsync(PageRequest pageRequest)
        {
            var pagedRows = await _accountRepository.ListAccountRoleAssignmentRowsAsync(pageRequest);

            var grouped = pagedRows.Items
                .GroupBy(x => new { x.AccountId, x.Username, x.Status })
                .Select(g => new AccountListResponseDto
                {
                    AccountId = g.Key.AccountId,
                    Username  = g.Key.Username,
                    Status    = (AccountStatus)g.Key.Status,

                    RoleAssignments = g
                        .Where(r => r.RoleId.HasValue && r.RoleName != null)
                        .GroupBy(r => new { RoleId = r.RoleId!.Value, RoleName = r.RoleName! })
                        .Select(roleGroup => new AccountRoleAssignmentResponseDto
                        {
                            RoleId = roleGroup.Key.RoleId,
                            RoleName = roleGroup.Key.RoleName,

                            Departments = roleGroup
                                .Where(r => r.DepartmentId.HasValue && r.DepartmentName != null)
                                .GroupBy(r => new { DeptId = r.DepartmentId!.Value, DeptName = r.DepartmentName! })
                                .Select(dg => new AssignedDepartmentDto
                                {
                                    DepartmentId = dg.Key.DeptId,
                                    DepartmentName = dg.Key.DeptName
                                })
                                .ToList()
                        })
                        .ToList()
                })
                .ToList();

            return new PagedResult<AccountListResponseDto>
            {
                Page = pagedRows.Page,
                PageSize = pagedRows.PageSize,
                TotalCount = pagedRows.TotalCount,
                Items = grouped
            };
        }

        public async Task<List<RoleOptionDto>> ListRoleOptionsAsync()
        {
            var rows = await _roleRepository.ListRoleLookupsAsync();

            return rows
                .Select(x => new RoleOptionDto
                {
                    RoleId = x.RoleId,
                    RoleName = x.RoleName
                })
                .ToList();
        }

        public async Task<List<PermissionOptionDto>> ListPermissionOptionsAsync()
        {
            var rows = await _permissionRepository.ListPermissionLookupsAsync();

            return rows
                .Select(x => new PermissionOptionDto
                {
                    PermissionId = x.PermissionId,
                    PermissionName = x.PermissionName
                })
                .ToList();
        }

        public async Task<List<ScopeOptionDto>> ListScopeOptionsAsync()
        {
            var rows = await _scopeRepository.ListScopeLookupsAsync();

            return rows
                .Select(x => new ScopeOptionDto
                {
                    ScopeId = x.ScopeId,
                    ScopeCode = x.ScopeCode,
                    ScopeName = x.ScopeName
                })
                .ToList();
        }

        public async Task<List<DepartmentOptionDto>> ListDepartmentOptionsAsync()
        {
            var rows = await _departmentRepository.ListDepartmentLookupsAsync();

            return rows
                .Select(x => new DepartmentOptionDto
                {
                    DepartmentId = x.DepartmentId,
                    DepartmentName = x.DepartmentName
                })
                .ToList();
        }
        public async Task<List<AccessibleDepartmentDto>> ListAccessibleDepartmentsAsync(Guid accountId)
        {
            if (accountId == Guid.Empty)
            {
                return [];
            }

            var hasGlobalScope = await HasGlobalScopeAsync(
                accountId,
                ContentViewPermissionCode
            );

            var rows = hasGlobalScope
                ? await _departmentRepository.ListAccessibleDepartmentsAsync()
                : await GetDepartmentScopedRowsAsync(accountId);

            return rows
                .Select(x => new AccessibleDepartmentDto
                {
                    DepartmentId = x.DepartmentId,
                    DepartmentCode = x.DepartmentCode,
                    DepartmentName = x.DepartmentName
                })
                .ToList();
        }

        public async Task<bool> HasGlobalScopeAsync(Guid accountId, string permissionCode)
        {
            if (accountId == Guid.Empty || string.IsNullOrWhiteSpace(permissionCode))
            {
                return false;
            }

            return await _accountRepository.HasGlobalPermissionScopeAsync(accountId, permissionCode);
        }

        public async Task<List<Guid>> GetDepartmentScopeDepartmentIdsAsync(Guid accountId, string permissionCode)
        {
            if (accountId == Guid.Empty || string.IsNullOrWhiteSpace(permissionCode))
            {
                return [];
            }

            return await _accountRepository.GetDepartmentIdsByPermissionScopeAsync(accountId, permissionCode);
        }

        public async Task<bool> CanAccessDepartmentAsync(Guid accountId, string permissionCode, Guid departmentId)
        {
            if (departmentId == Guid.Empty)
            {
                return false;
            }

            if (await HasGlobalScopeAsync(accountId, permissionCode))
            {
                return true;
            }

            var departmentIds = await GetDepartmentScopeDepartmentIdsAsync(accountId, permissionCode);

            return departmentIds.Contains(departmentId);
        }
        public async Task<PagedResult<RoleListResponseDto>> ListRolesAsync(PageRequest pageRequest)
        {
            var pagedRows = await _roleRepository.ListRolePermissionScopeRowsAsync(pageRequest);

            var grouped = pagedRows.Items
                .GroupBy(r => new { r.RoleId, r.RoleName, r.Status })
                .Select(roleGroup => new RoleListResponseDto
                {
                    RoleId = roleGroup.Key.RoleId,
                    RoleName = roleGroup.Key.RoleName,
                    Status = roleGroup.Key.Status,

                    PermissionScopes = roleGroup
                        .Where(x => x.PermissionId.HasValue)
                        .GroupBy(x => new { x.PermissionId, x.PermissionName })
                        .Select(pGroup => new RolePermissionScopeResponseDto
                        {
                            PermissionId = pGroup.Key.PermissionId!.Value,
                            PermissionName = pGroup.Key.PermissionName!,

                            Scopes = pGroup
                                .Where(s => s.ScopeId.HasValue)
                                .Select(s => new RoleScopeResponseDto
                                {
                                    ScopeId = s.ScopeId!.Value,
                                    ScopeName = s.ScopeName!
                                })
                                .DistinctBy(s => s.ScopeId)
                                .ToList()
                        })
                        .ToList()
                })
                .ToList();

            return new PagedResult<RoleListResponseDto>
            {
                Page = pagedRows.Page,
                PageSize = pagedRows.PageSize,
                TotalCount = pagedRows.TotalCount,
                Items = grouped
            };
        }


        public async Task<CreateRoleResponseDto> CreateRoleAsync(CreateRoleRequestDto dto)
        {
            // 角色名稱沒給
            if (string.IsNullOrWhiteSpace(dto.RoleName))
            {
                return new CreateRoleResponseDto
                {
                    Result = CreateRoleResult.RoleNameRequired
                };
            }

            // 角色代碼沒給
            if (string.IsNullOrWhiteSpace(dto.RoleCode))
            {
                return new CreateRoleResponseDto
                {
                    Result = CreateRoleResult.RoleCodeRequired
                };
            }

            // PermissionScopes 沒給
            if (dto.PermissionScopes == null || !dto.PermissionScopes.Any())
            {
                return new CreateRoleResponseDto
                {
                    Result = CreateRoleResult.PermissionRequired
                };
            }

            var normalizedScopes = dto.PermissionScopes
                .DistinctBy(x => new { x.PermissionId, x.ScopeId })
                .ToList();

            var permissionIds = normalizedScopes
                .Select(x => x.PermissionId)
                .Distinct()
                .ToList();

            var scopeIds = normalizedScopes
                .Select(x => x.ScopeId)
                .Distinct()
                .ToList();

            // Permission 是否存在
            if (!await _permissionRepository.AllPermissionsExistAsync(permissionIds))
            {
                return new CreateRoleResponseDto
                {
                    Result = CreateRoleResult.PermissionNotFound
                };
            }

            // Scope 是否存在
            if (!await _scopeRepository.AllScopesExistAsync(scopeIds))
            {
                return new CreateRoleResponseDto
                {
                    Result = CreateRoleResult.ScopeNotFound
                };
            }

            // 角色名稱是否重複
            if (await _roleRepository.RoleNameExistsAsync(dto.RoleName))
            {
                return new CreateRoleResponseDto
                {
                    Result = CreateRoleResult.RoleNameDuplicated
                };
            }


            // 包交易
            _unitOfWork.BeginTransaction();

            try
            {
                // 建立 Role
                var roleId = await _roleRepository.CreateRoleAsync(
                    dto.RoleName,
                    dto.RoleCode
                );

                // 建立 RolePermissionScope
                foreach (var item in dto.PermissionScopes)
                {
                    await _roleRepository.CreateRolePermissionScopeAsync(
                        roleId,
                        item.PermissionId,
                        item.ScopeId
                    );
                }

                _unitOfWork.Commit();

                return new CreateRoleResponseDto
                {
                    Result = CreateRoleResult.Success,
                };
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public async Task<UpdateRoleResponseDto> UpdateRoleAsync(UpdateRoleRequestDto dto)
        {
            // 角色名稱沒給
            if (string.IsNullOrWhiteSpace(dto.RoleName))
            {
                return new UpdateRoleResponseDto
                {
                    Result = UpdateRoleResult.RoleNameRequired
                };
            }

            // PermissionScopes 沒給
            if (dto.PermissionScopes == null || !dto.PermissionScopes.Any())
            {
                return new UpdateRoleResponseDto
                {
                    Result = UpdateRoleResult.PermissionRequired
                };
            }

            var normalizedScopes = dto.PermissionScopes
                .DistinctBy(x => new { x.PermissionId, x.ScopeId })
                .ToList();

            var permissionIds = normalizedScopes
                .Select(x => x.PermissionId)
                .Distinct()
                .ToList();

            var scopeIds = normalizedScopes
                .Select(x => x.ScopeId)
                .Distinct()
                .ToList();

            // Permission 是否存在
            if (!await _permissionRepository.AllPermissionsExistAsync(permissionIds))
            {
                return new UpdateRoleResponseDto
                {
                    Result = UpdateRoleResult.PermissionNotFound
                };
            }

            // Scope 是否存在
            if (!await _scopeRepository.AllScopesExistAsync(scopeIds))
            {
                return new UpdateRoleResponseDto
                {
                    Result = UpdateRoleResult.ScopeNotFound
                };
            }

            // 角色名稱是否重複
            if (!await _roleRepository.RoleNameExistsAsync(dto.RoleName))
            {
                return new UpdateRoleResponseDto
                {
                    Result = UpdateRoleResult.RoleNameNotFound
                };
            }

            // 有沒有這種狀態
            if (!Enum.IsDefined(dto.Status))
            {
                return new UpdateRoleResponseDto
                {
                    Result = UpdateRoleResult.StatusNotFound
                };
            }

            // 包交易
            _unitOfWork.BeginTransaction();

            try
            {
                // 更新帳戶狀態
                var roleId = await _roleRepository.UpdateRoleStatusAsync(
                    dto.RoleName,
                    dto.Status
                );

                // 建立 RolePermissionScope
                foreach (var item in dto.PermissionScopes)
                {
                    await _roleRepository.CreateRolePermissionScopeAsync(
                        roleId,
                        item.PermissionId,
                        item.ScopeId
                    );
                }

                _unitOfWork.Commit();

                return new UpdateRoleResponseDto
                {
                    Result = UpdateRoleResult.Success
                };
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public async Task<DeleteRoleResponseDto> DeleteRoleAsync(DeleteRoleRequestDto dto)
        {
            // 角色名稱沒給
            if (string.IsNullOrWhiteSpace(dto.RoleName))
            {
                return new DeleteRoleResponseDto
                {
                    Result = DeleteRoleResult.RoleNameRequired
                };
            }

            // 有沒有這角色名稱
            if (!await _roleRepository.RoleNameExistsAsync(dto.RoleName))
            {
                return new DeleteRoleResponseDto
                {
                    Result = DeleteRoleResult.RoleNotFound
                };
            }

            await _roleRepository.DeleteRoleAsync(dto.RoleName);

            return new DeleteRoleResponseDto
            {
                Result = DeleteRoleResult.Success
            };
        }
        public async Task<CreateAccountResponseDto> CreateAccountAsync(CreateAccountRequestDto dto)
        {
            // 帳號沒給
            if (string.IsNullOrWhiteSpace(dto.Username))
            {
                return new CreateAccountResponseDto
                {
                    Result = CreateAccountResult.UsernameRequired
                };
            }

            // 密碼沒給
            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                return new CreateAccountResponseDto
                {
                    Result = CreateAccountResult.PasswordRequired
                };
            }

            // RoleAssignments 沒給
            if (dto.RoleAssignments == null || !dto.RoleAssignments.Any())
            {
                return new CreateAccountResponseDto
                {
                    Result = CreateAccountResult.RoleAssignmentsRequired
                };
            }

            // 檢查 Role 是否存在
            var roleIds = dto.RoleAssignments
                .Select(x => x.RoleId)
                .Distinct()
                .ToList();

            if (!await _roleRepository.AllRolesExistAsync(roleIds))
            {
                return new CreateAccountResponseDto
                {
                    Result = CreateAccountResult.RoleNotFound
                };
            }

            // 檢查 Department 是否存在
            var departmentIds = dto.RoleAssignments
                .SelectMany(x => x.DepartmentIds) // 因為一個 RoleAssignment 可能對多個 Department, 所以要 SelectMany 展平
                .Distinct()
                .ToList();

            if (!await _departmentRepository.AllDepartmentsExistAsync(departmentIds))
            {
                return new CreateAccountResponseDto
                {
                    Result = CreateAccountResult.DepartmentNotFound
                };
            }

            // 帳號是否重複
            if (await _accountRepository.AccountExistsAsync(dto.Username))
            {
                return new CreateAccountResponseDto
                {
                    Result = CreateAccountResult.UsernameDuplicated
                };
            }

            // 灑鹽加密
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);


            // 包交易
            _unitOfWork.BeginTransaction();

            try
            {
                // 先建帳戶
                var accountId = await _accountRepository.CreateAccountAsync(
                    dto.Username,
                    passwordHash
                );

                // 建多筆 AccountRoleAssignment
                var rows = dto.RoleAssignments
                    .SelectMany(r => r.DepartmentIds.Select(d => new //心法：SelectMany(RoleId) + Select(DepartmentId) 就是雙層foreach
                    {
                        AccountId = accountId,
                        RoleId = r.RoleId,
                        DepartmentId = d
                    }))
                    .ToList();

                await _accountRepository.CreateAccountRoleAssignmentsAsync(rows);

                _unitOfWork.Commit();

                return new CreateAccountResponseDto
                {
                    Result = CreateAccountResult.Success,
                };
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public async Task<UpdateAccountResponseDto> UpdateAccountAsync(UpdateAccountRequestDto dto)
        {
            // 帳號沒給
            if (string.IsNullOrWhiteSpace(dto.Username))
            {
                return new UpdateAccountResponseDto
                {
                    Result = UpdateAccountResult.UsernameRequired
                };
            }

            // RoleAssignments 沒給
            if (dto.RoleAssignments == null || !dto.RoleAssignments.Any())
            {
                return new UpdateAccountResponseDto
                {
                    Result = UpdateAccountResult.RoleAssignmentsRequired
                };
            }

            // 檢查 Role 是否存在
            var roleIds = dto.RoleAssignments
                .Select(x => x.RoleId)
                .Distinct()
                .ToList();

            if (!await _roleRepository.AllRolesExistAsync(roleIds))
            {
                return new UpdateAccountResponseDto
                {
                    Result = UpdateAccountResult.RoleNotFound
                };
            }

            // 檢查 Department 是否存在
            var departmentIds = dto.RoleAssignments
                .SelectMany(x => x.DepartmentIds)
                .Distinct()
                .ToList();

            if (!await _departmentRepository.AllDepartmentsExistAsync(departmentIds))
            {
                return new UpdateAccountResponseDto
                {
                    Result = UpdateAccountResult.DepartmentNotFound
                };
            }

            // 有沒有這帳號
            if (!await _accountRepository.AccountExistsAsync(dto.Username))
            {
                return new UpdateAccountResponseDto
                {
                    Result = UpdateAccountResult.AccountNotFound
                };
            }

            // 有沒有這種狀態
            if (!Enum.IsDefined(dto.Status))
            {
                return new UpdateAccountResponseDto
                {
                    Result = UpdateAccountResult.StatusNotFound
                };
            }

            // 包交易
            _unitOfWork.BeginTransaction();

            try
            {
                // 更新帳戶狀態
                var accountId = await _accountRepository.UpdateAccountStatusAsync(
                    dto.Username,
                    dto.Status
                );

                // 更新帳戶角色（先清掉再加，最乾淨）
                await _accountRepository.DeleteAccountRoleAssignmentsAsync(accountId);

                // 建多筆 AccountRoleAssignment
                var rows = dto.RoleAssignments
                    .SelectMany(r => r.DepartmentIds.Select(d => new
                    {
                        AccountId = accountId,
                        RoleId = r.RoleId,
                        DepartmentId = d
                    }))
                    .ToList();

                await _accountRepository.CreateAccountRoleAssignmentsAsync(rows);

                _unitOfWork.Commit();

                return new UpdateAccountResponseDto
                {
                    Result = UpdateAccountResult.Success
                };
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public async Task<ResetPasswordResponseDto> ResetPasswordAsync(ResetPasswordRequestDto dto)
        {
            // 帳號沒給
            if (string.IsNullOrWhiteSpace(dto.Username))
            {
                return new ResetPasswordResponseDto
                {
                    Result = ResetPasswordResult.UsernameRequired
                };
            }

            // 密碼沒給
            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                return new ResetPasswordResponseDto
                {
                    Result = ResetPasswordResult.PasswordRequired
                };
            }

            // 帳號不存在
            if (!await _accountRepository.AccountExistsAsync(dto.Username))
            {
                return new ResetPasswordResponseDto
                {
                    Result = ResetPasswordResult.AccountNotFound
                };
            }

            // 加鹽撒密
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // 更新密碼
            await _accountRepository.UpdatePasswordAsync(dto.Username, passwordHash);

            // 更新上次更新時間
            await _accountRepository.UpdateUpdatedAtAsync(dto.Username, DateTime.Now);

            return new ResetPasswordResponseDto
            {
                Result = ResetPasswordResult.Success
            };
        }

        public async Task<DeleteAccountResponseDto> DeleteAccountAsync(DeleteAccountRequestDto dto)
        {
            // 帳號沒給
            if (string.IsNullOrWhiteSpace(dto.Username))
            {
                return new DeleteAccountResponseDto
                {
                    Result = DeleteAccountResult.UsernameRequired
                };
            }

            // 有沒有這帳號
            if (!await _accountRepository.AccountExistsAsync(dto.Username))
            {
                return new DeleteAccountResponseDto
                {
                    Result = DeleteAccountResult.AccountNotFound
                };
            }

            await _accountRepository.DeleteAccountAsync(dto.Username);

            return new DeleteAccountResponseDto
            {
                Result = DeleteAccountResult.Success
            };
        }

        private async Task<IEnumerable<Cms.Contract.Repositories.Department.Entities.AccessibleDepartmentEntity>> GetDepartmentScopedRowsAsync(Guid accountId)
        {
            var departmentIds = await GetDepartmentScopeDepartmentIdsAsync(
                accountId,
                ContentViewPermissionCode
            );

            if (departmentIds.Count == 0)
            {
                return [];
            }

            return await _departmentRepository.ListAccessibleDepartmentsAsync(departmentIds);
        }
    }
}








