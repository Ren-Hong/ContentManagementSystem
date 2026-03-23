namespace Cms.Contract.Services.PermissionScope.Interfaces
{
    /// <summary>
    /// 提供帳號權限作用範圍判斷相關的應用服務操作。
    /// </summary>
    public interface IPermissionScopeService
    {
        /// <summary>
        /// 判斷帳號是否具有指定權限的全域範圍。
        /// </summary>
        /// <param name="accountId">帳號識別碼。</param>
        /// <param name="permissionCode">權限代碼。</param>
        /// <returns>若具有全域範圍則為 <c>true</c>；否則為 <c>false</c>。</returns>
        Task<bool> HasGlobalScopeAsync(Guid accountId, string permissionCode);

        /// <summary>
        /// 取得帳號在指定權限下可存取的部門識別碼清單。
        /// </summary>
        /// <param name="accountId">帳號識別碼。</param>
        /// <param name="permissionCode">權限代碼。</param>
        /// <returns>可存取的部門識別碼清單。</returns>
        Task<List<Guid>> GetDepartmentScopeDepartmentIdsAsync(Guid accountId, string permissionCode);

        /// <summary>
        /// 判斷帳號是否可存取指定部門。
        /// </summary>
        /// <param name="accountId">帳號識別碼。</param>
        /// <param name="permissionCode">權限代碼。</param>
        /// <param name="departmentId">部門識別碼。</param>
        /// <returns>若可存取則為 <c>true</c>；否則為 <c>false</c>。</returns>
        Task<bool> CanAccessDepartmentAsync(Guid accountId, string permissionCode, Guid departmentId);
    }
}
