using Cms.Contract.Repositories.Department.Entities;

namespace Cms.Contract.Repositories.Department.Interfaces
{
    public interface IDepartmentRepository
    {
        /// <summary>
        /// 判斷該部門是否存在
        /// </summary>
        /// <param name="departmentIds">前端傳過來的權限Id</param>
        /// <returns>bool</returns>
        Task<bool> AllDepartmentsExistAsync(List<Guid> departmentIds);

        /// <summary>
        /// 列出部門 lookup 資料
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<DepartmentLookupEntity>> ListDepartmentLookupsAsync();

        /// <summary>
        /// 列出所有可存取的部門
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<AccessibleDepartmentEntity>> ListAccessibleDepartmentsAsync();

        /// <summary>
        /// 依指定 DepartmentId 列出可存取的部門。
        /// </summary>
        Task<IEnumerable<AccessibleDepartmentEntity>> ListAccessibleDepartmentsAsync(IEnumerable<Guid> departmentIds);
    }
}
