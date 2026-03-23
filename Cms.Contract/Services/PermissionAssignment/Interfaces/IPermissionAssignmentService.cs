using Cms.Contract.Common.Pagination;
using Cms.Contract.Services.PermissionAssignment.Dtos;

namespace Cms.Contract.Services.PermissionAssignment.Interfaces
{
    public interface IPermissionAssignmentService
    {
        /// <summary>
        /// 專給下拉用的Permission
        /// </summary>
        /// <returns></returns>
        Task<PagedResult<PermissionAssignmentListResponseDto>> PagePermissionAssignmentsAsync(PageRequest pageRequest);
    }
}
