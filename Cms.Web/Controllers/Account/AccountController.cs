using Cms.Contract.Common.Pagination;
using Cms.Contract.Controllers.Account.Models;
using Cms.Contract.Controllers.Api;
using Cms.Contract.Controllers.Department.Models;
using Cms.Contract.Services.Account.Dtos;
using Cms.Contract.Services.Account.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cms.Web.Controllers.Account
{
    [ApiController]
    [Route("api/accounts")]
    [Authorize(Roles = "Admin")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [AllowAnonymous]
        [HttpPost("/api/sessions")]
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

            var rdto = await _accountService.LoginAsync(dto);

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
        [HttpDelete("/api/sessions/current")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> ListAccounts([FromQuery] PageRequest req)
        {
            var rdto = await _accountService.PageAccountsAsync(req);

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

        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequestModel req)
        {
            if (!ModelState.IsValid)
            {
                return ValidationErrorResponse();
            }

            var rdto = await _accountService.CreateAccountAsync(
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

        [HttpPut("{username}")]
        public async Task<IActionResult> UpdateAccount(string username, [FromBody] UpdateAccountRequestModel req)
        {
            if (!ModelState.IsValid)
            {
                return ValidationErrorResponse();
            }

            var rdto = await _accountService.UpdateAccountAsync(
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

        [HttpPut("{username}/password")]
        public async Task<IActionResult> ResetPassword(string username, [FromBody] ResetPasswordRequestModel req)
        {
            if (!ModelState.IsValid)
            {
                return ValidationErrorResponse();
            }

            var rdto = await _accountService.ResetPasswordAsync(
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

        [HttpDelete("{username}")]
        public async Task<IActionResult> DeleteAccount(string username)
        {
            var rdto = await _accountService.DeleteAccountAsync(
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

        private IActionResult ValidationErrorResponse()
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                ValidationErrors = ModelState
                    .Where(entry => entry.Value!.Errors.Any())
                    .ToDictionary(
                        entry => entry.Key,
                        entry => entry.Value!.Errors.Select(error => error.ErrorMessage).ToList()
                    )
            });
        }

        private IActionResult ErrorResponse(string errorCode)
        {
            return StatusCode(MapStatusCode(errorCode), new ApiResponse
            {
                Success = false,
                ErrorCode = errorCode
            });
        }

        private static int MapStatusCode(string errorCode)
        {
            if (errorCode.Contains("PermissionDenied", StringComparison.Ordinal))
            {
                return StatusCodes.Status403Forbidden;
            }

            if (errorCode.Contains("NotFound", StringComparison.Ordinal))
            {
                return StatusCodes.Status404NotFound;
            }

            if (
                errorCode.Contains("Required", StringComparison.Ordinal)
                || errorCode.Contains("Invalid", StringComparison.Ordinal)
            )
            {
                return StatusCodes.Status400BadRequest;
            }

            return StatusCodes.Status409Conflict;
        }
    }
}
