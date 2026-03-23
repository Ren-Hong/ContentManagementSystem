using Cms.Contract.Common.Pagination;
using Cms.Contract.Repositories.PermissionAssignment.Entities;

namespace Cms.Contract.Repositories.PermissionAssignment.Interfaces
{
    /// <summary>
    /// 提供帳號權限指派摘要的資料存取操作。
    /// </summary>
    public interface IPermissionAssignmentRepository
    {
        /// <summary>
        /// 以分頁方式取得權限指派摘要資料列。
        /// </summary>
        /// <param name="pageRequest">分頁條件。</param>
        /// <returns>權限指派摘要資料列的分頁結果。</returns>
        Task<PagedResult<PermissionAssignmentRowEntity>> PagePermissionAssignmentRowsAsync(PageRequest pageRequest);
    }
}
