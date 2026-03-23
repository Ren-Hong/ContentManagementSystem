using Cms.Contract.Repositories.Department.Entities;

namespace Cms.Contract.Repositories.Department.Interfaces
{
    /// <summary>
    /// 提供部門相關的資料存取操作。
    /// </summary>
    public interface IDepartmentRepository
    {
        /// <summary>
        /// 判斷指定的部門識別碼是否全部存在。
        /// </summary>
        /// <param name="departmentIds">部門識別碼集合。</param>
        /// <returns>若全部存在則為 <c>true</c>；否則為 <c>false</c>。</returns>
        Task<bool> AllDepartmentsExistAsync(List<Guid> departmentIds);

        /// <summary>
        /// 取得部門 lookup 資料。
        /// </summary>
        /// <returns>部門 lookup 資料集合。</returns>
        Task<IEnumerable<DepartmentLookupEntity>> ListDepartmentLookupsAsync();

        /// <summary>
        /// 取得所有可存取的部門資料。
        /// </summary>
        /// <returns>可存取的部門資料集合。</returns>
        Task<IEnumerable<AccessibleDepartmentEntity>> ListAccessibleDepartmentsAsync();

        /// <summary>
        /// 依指定部門識別碼集合取得可存取的部門資料。
        /// </summary>
        /// <param name="departmentIds">部門識別碼集合。</param>
        /// <returns>可存取的部門資料集合。</returns>
        Task<IEnumerable<AccessibleDepartmentEntity>> ListAccessibleDepartmentsAsync(IEnumerable<Guid> departmentIds);
    }
}
