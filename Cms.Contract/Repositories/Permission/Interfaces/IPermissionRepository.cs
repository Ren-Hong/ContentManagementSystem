using Cms.Contract.Repositories.Permission.Entities;

namespace Cms.Contract.Repositories.Permission.Interfaces
{
    /// <summary>
    /// 提供權限相關的資料存取操作。
    /// </summary>
    public interface IPermissionRepository
    {
        /// <summary>
        /// 判斷指定的權限識別碼是否全部存在。
        /// </summary>
        /// <param name="permissionIds">權限識別碼集合。</param>
        /// <returns>若全部存在則為 <c>true</c>；否則為 <c>false</c>。</returns>
        Task<bool> AllPermissionsExistAsync(List<Guid> permissionIds);

        /// <summary>
        /// 取得權限 lookup 資料。
        /// </summary>
        /// <returns>權限 lookup 資料集合。</returns>
        Task<IEnumerable<PermissionLookupEntity>> ListPermissionLookupsAsync();
    }
}
