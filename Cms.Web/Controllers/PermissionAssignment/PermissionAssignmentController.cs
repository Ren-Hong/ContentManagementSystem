using Cms.Contract.Common.Pagination;
using Cms.Contract.Controllers.Api;
using Cms.Contract.Controllers.PermissionAssignment.Models;
using Cms.Contract.Services.PermissionAssignment.Dtos;
using Cms.Contract.Services.PermissionAssignment.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cms.Web.Controllers.PermissionAssignment
{
    [ApiController]
    [Route("api/permission-assignments")]
    [Authorize(Roles = "Admin")]
    public class PermissionAssignmentController : ControllerBase
    {
        private readonly IPermissionAssignmentService _permissionAssignmentService;

        public PermissionAssignmentController(IPermissionAssignmentService permissionAssignmentService)
        {
            _permissionAssignmentService = permissionAssignmentService;
        }

        [HttpGet]
        public async Task<IActionResult> ListPermissionAssignments([FromQuery] PageRequest req)
        {
            var paged = await _permissionAssignmentService.ListPermissionAssignmentsAsync(req);

            var res = paged.Items
                .Select(x => new PermissionAssignmentListResponseModel
                {
                    AccountId = x.AccountId,
                    Username = x.Username,
                    PermissionDepartments = x.PermissionDepartments?
                        .Select(p => new PermissionDepartmentResponseModel
                        {
                            PermissionId = p.PermissionId,
                            PermissionName = p.PermissionName,
                            Departments = p.Departments?
                                .Select(d => new AssignedDepartmentResponseModel
                                {
                                    DepartmentId = d.DepartmentId,
                                    DepartmentName = d.DepartmentName,
                                    ValidFrom = d.ValidFrom,
                                    ValidTo = d.ValidTo
                                })
                                .ToList()
                        })
                        .ToList()
                })
                .ToList();

            return Ok(new ApiResponse<PagedResult<PermissionAssignmentListResponseModel>>
            {
                Success = true,
                Data = new PagedResult<PermissionAssignmentListResponseModel>
                {
                    Page = paged.Page,
                    PageSize = paged.PageSize,
                    TotalCount = paged.TotalCount,
                    Items = res
                }
            });
        }
    }
}
