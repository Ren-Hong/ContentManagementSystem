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
        public async Task<IActionResult> GetPermissionAssignmentSummaries([FromQuery] PageRequest req)
        {
            var paged = await _permissionAssignmentService.GetPermissionAssignmentSummariesAsync(req);

            var res = paged.Items
                .Select(x => new GetPermissionAssignmentSummariesResponseModel
                {
                    AccountId = x.AccountId,
                    Username = x.Username,
                    PermissionDepartments = x.PermissionDepartments?
                        .Select(p => new PermissionDepartmentsSummaryResponseModel
                        {
                            PermissionId = p.PermissionId,
                            PermissionName = p.PermissionName,
                            Departments = p.Departments?
                                .Select(d => new DepartmentSummaryResponseModel
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

            return Ok(new ApiResponse<PagedResult<GetPermissionAssignmentSummariesResponseModel>>
            {
                Success = true,
                Data = new PagedResult<GetPermissionAssignmentSummariesResponseModel>
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
