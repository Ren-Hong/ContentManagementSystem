using Cms.Contract.Controllers.Api;
using Cms.Contract.Controllers.Department.Models;
using Cms.Contract.Services.Department.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cms.Web.Controllers.Department
{
    [ApiController]
    [Route("api/departments")]
    [Authorize]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        public async Task<IActionResult> ListDepartmentLookups()
        {
            var rdto = await _departmentService.ListDepartmentLookupsAsync();

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

        [HttpGet("sidebar")]
        [Authorize(Policy = "Permission.Content.View")]
        public async Task<IActionResult> ListAccessibleDepartments()
        {
            var rdto = await _departmentService.ListAccessibleDepartmentsAsync(GetCurrentAccountId());

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

        private Guid GetCurrentAccountId()
        {
            var ownerClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(ownerClaim, out var accountId)
                ? accountId
                : Guid.Empty;
        }
    }
}
