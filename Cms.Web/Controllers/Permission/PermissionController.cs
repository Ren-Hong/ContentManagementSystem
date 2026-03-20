using Cms.Contract.Controllers.Api;
using Cms.Contract.Controllers.Permission.Models;
using Cms.Contract.Services.Permission.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cms.Web.Controllers.Permission
{
    [ApiController]
    [Route("api/permissions")]
    [Authorize(Roles = "Admin")]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPermissionOptions()
        {
            var rdto = await _permissionService.GetPermissionOptionsAsync();

            var res = rdto.Select(x => new GetPermissionOptionsResponseModel
            {
                PermissionName = x.PermissionName,
                PermissionId = x.PermissionId,
            }).ToList();

            return Ok(new ApiResponse<IEnumerable<GetPermissionOptionsResponseModel>>
            {
                Success = true,
                Data = res
            });
        }
    }
}
