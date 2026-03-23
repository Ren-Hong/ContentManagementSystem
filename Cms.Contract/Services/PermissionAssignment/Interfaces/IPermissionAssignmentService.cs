using Cms.Contract.Common.Pagination;
using Cms.Contract.Services.PermissionAssignment.Dtos;

namespace Cms.Contract.Services.PermissionAssignment.Interfaces
{
    /// <summary>
    /// 提供帳號權限指派摘要查詢相關的應用服務操作。
    /// </summary>
    public interface IPermissionAssignmentService
    {
        /// <summary>
        /// 以分頁方式取得權限指派摘要資料。
        /// </summary>
        /// <param name="pageRequest">分頁條件。</param>
        /// <returns>權限指派摘要的分頁結果。</returns>
        Task<PagedResult<PermissionAssignmentListResponseDto>> PagePermissionAssignmentsAsync(PageRequest pageRequest);
    }
}
