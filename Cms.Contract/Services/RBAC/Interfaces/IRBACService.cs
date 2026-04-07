using Cms.Contract.Common.Pagination;
using Cms.Contract.Services.RBAC.Dtos;

namespace Cms.Contract.Services.RBAC.Interfaces
{
    public interface IRBACService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto req);

        Task<PagedResult<AccountListResponseDto>> ListAccountsAsync(PageRequest pageRequest);

        Task<List<RoleOptionDto>> ListRoleOptionsAsync();

        Task<List<PermissionOptionDto>> ListPermissionOptionsAsync();

        Task<List<ScopeOptionDto>> ListScopeOptionsAsync();

        Task<List<DepartmentOptionDto>> ListDepartmentOptionsAsync();

        Task<List<AccessibleDepartmentDto>> ListAccessibleDepartmentsAsync(Guid accountId);

        Task<bool> HasGlobalScopeAsync(Guid accountId, string permissionCode);

        Task<List<Guid>> GetDepartmentScopeDepartmentIdsAsync(Guid accountId, string permissionCode);

        Task<bool> CanAccessDepartmentAsync(Guid accountId, string permissionCode, Guid departmentId);

        Task<CreateAccountResponseDto> CreateAccountAsync(CreateAccountRequestDto req);

        Task<UpdateAccountResponseDto> UpdateAccountAsync(UpdateAccountRequestDto req);

        Task<ResetPasswordResponseDto> ResetPasswordAsync(ResetPasswordRequestDto req);

        Task<DeleteAccountResponseDto> DeleteAccountAsync(DeleteAccountRequestDto req);

        Task<PagedResult<RoleListResponseDto>> ListRolesAsync(PageRequest pageRequest);

        Task<CreateRoleResponseDto> CreateRoleAsync(CreateRoleRequestDto req);

        Task<UpdateRoleResponseDto> UpdateRoleAsync(UpdateRoleRequestDto req);

        Task<DeleteRoleResponseDto> DeleteRoleAsync(DeleteRoleRequestDto req);
    }
}

