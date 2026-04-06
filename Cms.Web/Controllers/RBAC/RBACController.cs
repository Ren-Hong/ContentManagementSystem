using System.Security.Claims;
using Cms.Contract.Common.Pagination;
using Cms.Contract.Controllers.Account.Models;
using Cms.Contract.Controllers.Api;
using Cms.Contract.Controllers.Department.Models;
using Cms.Contract.Controllers.Role.Models;
using Cms.Contract.Services.RBAC.Dtos;
using Cms.Contract.Services.RBAC.Interfaces;
using Cms.Web.Controllers.Api;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cms.Web.Controllers.RBAC
{
    [ApiController]
    [Route("api")]
    public class RBACController : ApiControllerBase
    {
        private readonly IRBACService _rbacService;

        public RBACController(IRBACService rbacService)
        {
            _rbacService = rbacService;
        }

        [AllowAnonymous]
        [HttpPost("sessions")]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel req)
        {
            if (!ModelState.IsValid)
            {
                return ValidationErrorResponse();
            }

            var dto = new LoginRequestDto
            {
                Username = req.Username,
                Password = req.Password
            };

            var rdto = await _rbacService.LoginAsync(dto);

            if (rdto.Result != LoginResult.Success)
            {
                return Unauthorized(new ApiResponse
                {
                    Success = false,
                    ErrorCode = rdto.Result.ToString()
                });
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, rdto.AccountId!.Value.ToString()),
                new Claim(ClaimTypes.Name, rdto.Username!)
            };

            if (rdto.RoleCodes != null)
            {
                foreach (var roleCode in rdto.RoleCodes)
                {
                    claims.Add(new Claim(ClaimTypes.Role, roleCode));
                }
            }

            if (rdto.PermissionCodes != null)
            {
                foreach (var permissionCode in rdto.PermissionCodes)
                {
                    claims.Add(new Claim("permission", permissionCode));
                }
            }

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );

            return Ok(new ApiResponse<LoginResponseModel>
            {
                Success = true,
                Data = new LoginResponseModel
                {
                    IsAdmin = rdto.RoleCodes?.Contains("Admin") == true
                }
            });
        }

        [AllowAnonymous]
        [HttpDelete("sessions/current")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("accounts")]
        public async Task<IActionResult> ListAccounts([FromQuery] PageRequest req)
        {
            var rdto = await _rbacService.ListAccountsAsync(req);

            var res = new PagedResult<AccountListResponseModel>
            {
                Page = rdto.Page,
                PageSize = rdto.PageSize,
                TotalCount = rdto.TotalCount,
                Items = rdto.Items.Select(x => new AccountListResponseModel
                {
                    AccountId = x.AccountId,
                    Username = x.Username,
                    Status = x.Status,
                    RoleAssignments = x.RoleAssignments.Select(d => new AccountRoleAssignmentResponseModel
                    {
                        RoleId = d.RoleId,
                        RoleName = d.RoleName,
                        Departments = d.Departments.Select(dep => new DepartmentLookupResponseModel
                        {
                            DepartmentId = dep.DepartmentId,
                            DepartmentName = dep.DepartmentName
                        }).ToList()
                    }).ToList()
                }).ToList()
            };

            return Ok(new ApiResponse<PagedResult<AccountListResponseModel>>
            {
                Success = true,
                Data = res
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("accounts")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequestModel req)
        {
            if (!ModelState.IsValid)
            {
                return ValidationErrorResponse();
            }

            var rdto = await _rbacService.CreateAccountAsync(
                new CreateAccountRequestDto
                {
                    Username = req.Username,
                    Password = req.Password,
                    RoleAssignments = req.RoleAssignments.Select(x => new AccountRoleAssignmentRequestDto
                    {
                        RoleId = x.RoleId,
                        DepartmentIds = x.DepartmentIds
                    }).ToList(),
                }
            );

            if (rdto.Result != CreateAccountResult.Success)
            {
                return ErrorResponse(rdto.Result.ToString());
            }

            return Created($"/api/accounts/{Uri.EscapeDataString(req.Username)}", new ApiResponse
            {
                Success = true
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("accounts/{username}")]
        public async Task<IActionResult> UpdateAccount(string username, [FromBody] UpdateAccountRequestModel req)
        {
            if (!ModelState.IsValid)
            {
                return ValidationErrorResponse();
            }

            var rdto = await _rbacService.UpdateAccountAsync(
                new UpdateAccountRequestDto
                {
                    Username = username,
                    RoleAssignments = req.RoleAssignments.Select(x => new AccountRoleAssignmentRequestDto
                    {
                        RoleId = x.RoleId,
                        DepartmentIds = x.DepartmentIds
                    }).ToList(),
                    Status = req.Status,
                }
            );

            if (rdto.Result != UpdateAccountResult.Success)
            {
                return ErrorResponse(rdto.Result.ToString());
            }

            return Ok(new ApiResponse
            {
                Success = true
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("accounts/{username}/password")]
        public async Task<IActionResult> ResetPassword(string username, [FromBody] ResetPasswordRequestModel req)
        {
            if (!ModelState.IsValid)
            {
                return ValidationErrorResponse();
            }

            var rdto = await _rbacService.ResetPasswordAsync(
                new ResetPasswordRequestDto
                {
                    Username = username,
                    Password = req.Password,
                }
            );

            if (rdto.Result != ResetPasswordResult.Success)
            {
                return ErrorResponse(rdto.Result.ToString());
            }

            return Ok(new ApiResponse
            {
                Success = true
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("accounts/{username}")]
        public async Task<IActionResult> DeleteAccount(string username)
        {
            var rdto = await _rbacService.DeleteAccountAsync(
                new DeleteAccountRequestDto
                {
                    Username = username,
                }
            );

            if (rdto.Result != DeleteAccountResult.Success)
            {
                return ErrorResponse(rdto.Result.ToString());
            }

            return Ok(new ApiResponse
            {
                Success = true
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("roles")]
        public async Task<IActionResult> ListRoles([FromQuery] PageRequest req)
        {
            var rdto = await _rbacService.ListRolesAsync(req);

            var res = new PagedResult<RoleListResponseModel>
            {
                Page = rdto.Page,
                PageSize = rdto.PageSize,
                TotalCount = rdto.TotalCount,
                Items = rdto.Items.Select(x => new RoleListResponseModel
                {
                    RoleId = x.RoleId,
                    RoleName = x.RoleName,
                    Status = x.Status,
                    PermissionScopes = x.PermissionScopes
                        .Select(p => new RolePermissionScopeResponseModel
                        {
                            PermissionId = p.PermissionId,
                            PermissionName = p.PermissionName,
                            Scopes = p.Scopes
                                .Select(s => new RoleScopeResponseModel
                                {
                                    ScopeId = s.ScopeId,
                                    ScopeName = s.ScopeName
                                })
                                .ToList()
                        })
                        .ToList()
                }).ToList()
            };

            return Ok(new ApiResponse<PagedResult<RoleListResponseModel>>
            {
                Success = true,
                Data = res
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("roles")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequestModel req)
        {
            if (!ModelState.IsValid)
            {
                return ValidationErrorResponse();
            }

            var rdto = await _rbacService.CreateRoleAsync(
                new CreateRoleRequestDto
                {
                    RoleName = req.RoleName,
                    RoleCode = req.RoleCode,
                    PermissionScopes = req.PermissionScopes
                        .Select(x => new PermissionScopeDto
                        {
                            PermissionId = x.PermissionId,
                            ScopeId = x.ScopeId
                        })
                        .ToList()
                }
            );

            if (rdto.Result != CreateRoleResult.Success)
            {
                return ErrorResponse(rdto.Result.ToString());
            }

            return Created($"/api/roles/{Uri.EscapeDataString(req.RoleName)}", new ApiResponse
            {
                Success = true
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("roles/{roleName}")]
        public async Task<IActionResult> UpdateRole(string roleName, [FromBody] UpdateRoleRequestModel req)
        {
            if (!ModelState.IsValid)
            {
                return ValidationErrorResponse();
            }

            var rdto = await _rbacService.UpdateRoleAsync(
                new UpdateRoleRequestDto
                {
                    RoleName = roleName,
                    PermissionScopes = req.PermissionScopes
                        .Select(x => new PermissionScopeDto
                        {
                            PermissionId = x.PermissionId,
                            ScopeId = x.ScopeId
                        })
                        .ToList(),
                    Status = req.Status,
                }
            );

            if (rdto.Result != UpdateRoleResult.Success)
            {
                return ErrorResponse(rdto.Result.ToString());
            }

            return Ok(new ApiResponse
            {
                Success = true
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("roles/{roleName}")]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            var rdto = await _rbacService.DeleteRoleAsync(
                new DeleteRoleRequestDto
                {
                    RoleName = roleName,
                }
            );

            if (rdto.Result != DeleteRoleResult.Success)
            {
                return ErrorResponse(rdto.Result.ToString());
            }

            return Ok(new ApiResponse
            {
                Success = true
            });
        }
    }
}
