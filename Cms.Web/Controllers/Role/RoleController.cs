using Cms.Contract.Common.Pagination;
using Cms.Contract.Controllers.Api;
using Cms.Contract.Controllers.Role.Models;
using Cms.Contract.Services.Role.Dtos;
using Cms.Contract.Services.Role.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cms.Web.Controllers.Role
{
    [ApiController]
    [Route("api/roles")]
    [Authorize(Roles = "Admin")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("options")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetRoleOptions()
        {
            var rdto = await _roleService.GetRoleOptionsAsync();

            var res = rdto.Select(x => new GetRoleOptionsResponseModel
            {
                RoleName = x.RoleName,
                RoleId = x.RoleId,
            }).ToList();

            return Ok(new ApiResponse<IEnumerable<GetRoleOptionsResponseModel>>
            {
                Success = true,
                Data = res
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetRoleSummaries([FromQuery] PageRequest req)
        {
            var rdto = await _roleService.GetRoleSummariesAsync(req);

            var res = new PagedResult<GetRoleSummariesResponseModel>
            {
                Page = rdto.Page,
                PageSize = rdto.PageSize,
                TotalCount = rdto.TotalCount,
                Items = rdto.Items.Select(x => new GetRoleSummariesResponseModel
                {
                    RoleId = x.RoleId,
                    RoleName = x.RoleName,
                    Status = x.Status,
                    PermissionScopes = x.PermissionScopes
                        .Select(p => new PermissionScopesSummaryModel
                        {
                            PermissionId = p.PermissionId,
                            PermissionName = p.PermissionName,
                            Scopes = p.Scopes
                                .Select(s => new ScopeSummaryModel
                                {
                                    ScopeId = s.ScopeId,
                                    ScopeName = s.ScopeName
                                })
                                .ToList()
                        })
                        .ToList()
                }).ToList()
            };

            return Ok(new ApiResponse<PagedResult<GetRoleSummariesResponseModel>>
            {
                Success = true,
                Data = res
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequestModel req)
        {
            if (!ModelState.IsValid)
            {
                return ValidationErrorResponse();
            }

            var rdto = await _roleService.CreateRoleAsync(
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

        [HttpPut("{roleName}")]
        public async Task<IActionResult> UpdateRole(string roleName, [FromBody] UpdateRoleBodyModel req)
        {
            if (!ModelState.IsValid)
            {
                return ValidationErrorResponse();
            }

            var rdto = await _roleService.UpdateRoleAsync(
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

        [HttpDelete("{roleName}")]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            var rdto = await _roleService.DeleteRoleAsync(
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
