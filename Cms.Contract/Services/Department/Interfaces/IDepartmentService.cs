using Cms.Contract.Services.Department.Dtos;

namespace Cms.Contract.Services.Department.Interfaces
{
    /// <summary>
    /// 提供部門查詢相關的應用服務操作。
    /// </summary>
    public interface IDepartmentService
    {
        /// <summary>
        /// 取得部門 lookup 資料。
        /// </summary>
        /// <returns>部門 lookup 資料清單。</returns>
        Task<List<DepartmentLookupDto>> ListDepartmentLookupsAsync();

        /// <summary>
        /// 取得目前帳號可存取的部門清單。
        /// </summary>
        /// <param name="accountId">目前登入帳號識別碼。</param>
        /// <returns>可存取的部門清單。</returns>
        Task<List<AccessibleDepartmentDto>> ListAccessibleDepartmentsAsync(Guid accountId);
    }
}
