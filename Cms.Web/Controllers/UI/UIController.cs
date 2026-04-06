using Cms.Contract.Controllers.Api;
using Cms.Contract.Controllers.Department.Models;
using Cms.Contract.Controllers.Permission.Models;
using Cms.Contract.Controllers.Role.Models;
using Cms.Contract.Controllers.Scope.Models;
using Cms.Contract.Services.RBAC.Interfaces;
using Cms.Contract.Services.UI.Interfaces;
using Cms.Web.Controllers.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cms.Web.Controllers.UI
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class UIController : ApiControllerBase
    {
        private readonly IUIService _uiService;
        private readonly IRBACService _rbacService;

        public UIController(
            IUIService uiService,
            IRBACService rbacService
        )
        {
            _uiService = uiService;
            _rbacService = rbacService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("roles/options")]
        public async Task<IActionResult> ListRoleLookups()
        {
            var rdto = await _uiService.ListRoleLookupsAsync();

            var res = rdto.Select(x => new RoleLookupResponseModel
            {
                RoleName = x.RoleName,
                RoleId = x.RoleId,
            }).ToList();

            return Ok(new ApiResponse<IEnumerable<RoleLookupResponseModel>>
            {
                Success = true,
                Data = res
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("permissions")]
        public async Task<IActionResult> ListPermissionLookups()
        {
            var rdto = await _uiService.ListPermissionLookupsAsync();

            var res = rdto.Select(x => new PermissionLookupResponseModel
            {
                PermissionName = x.PermissionName,
                PermissionId = x.PermissionId,
            }).ToList();

            return Ok(new ApiResponse<IEnumerable<PermissionLookupResponseModel>>
            {
                Success = true,
                Data = res
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("scopes")]
        public async Task<IActionResult> ListScopeLookups()
        {
            var rdto = await _uiService.ListScopeLookupsAsync();

            var res = rdto.Select(x => new ScopeLookupResponseModel
            {
                ScopeName = x.ScopeName,
                ScopeCode = x.ScopeCode,
                ScopeId = x.ScopeId,
            }).ToList();

            return Ok(new ApiResponse<IEnumerable<ScopeLookupResponseModel>>
            {
                Success = true,
                Data = res
            });
        }

        [HttpGet("departments")]
        public async Task<IActionResult> ListDepartmentLookups()
        {
            var rdto = await _uiService.ListDepartmentLookupsAsync();

            var res = rdto.Select(x => new DepartmentLookupResponseModel
            {
                DepartmentName = x.DepartmentName,
                DepartmentId = x.DepartmentId,
            }).ToList();

            return Ok(new ApiResponse<IEnumerable<DepartmentLookupResponseModel>>
            {
                Success = true,
                Data = res
            });
        }

        [Authorize(Policy = "Permission.Content.View")]
        [HttpGet("departments/sidebar")]
        public async Task<IActionResult> ListAccessibleDepartments()
        {
            var rdto = await _rbacService.ListAccessibleDepartmentsAsync(GetCurrentAccountId());

            var res = rdto.Select(x => new AccessibleDepartmentResponseModel
            {
                DepartmentId = x.DepartmentId,
                DepartmentName = x.DepartmentName,
                DepartmentCode = x.DepartmentCode,
            }).ToList();

            return Ok(new ApiResponse<IEnumerable<AccessibleDepartmentResponseModel>>
            {
                Success = true,
                Data = res
            });
        }
    }
}
