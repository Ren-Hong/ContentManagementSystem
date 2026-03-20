using Cms.Contract.Services.Department.Dtos;

namespace Cms.Contract.Services.Department.Interfaces
{
    public interface IDepartmentService
    {
        /// <summary>
        /// 列出部門 lookup 資料
        /// </summary>
        /// <returns></returns>
        Task<List<DepartmentLookupDto>> ListDepartmentLookupsAsync();

        /// <summary>
        /// 列出目前帳號可存取的部門
        /// </summary>
        /// <returns></returns>
        Task<List<AccessibleDepartmentDto>> ListAccessibleDepartmentsAsync(Guid accountId);
    }
}
