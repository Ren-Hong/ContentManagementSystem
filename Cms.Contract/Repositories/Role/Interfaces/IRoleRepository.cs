using Cms.Contract.Common.Pagination;
using Cms.Contract.Repositories.Role.Entities;
using Cms.Contract.Repositories.Role.Persistence;

namespace Cms.Contract.Repositories.Role.Interfaces
{
    /// <summary>
    /// 提供角色與角色權限範圍相關的資料存取操作。
    /// </summary>
    public interface IRoleRepository
    {
        /// <summary>
        /// 判斷指定的角色識別碼是否全部存在。
        /// </summary>
        /// <param name="roleIds">角色識別碼集合。</param>
        /// <returns>若全部存在則為 <c>true</c>；否則為 <c>false</c>。</returns>
        Task<bool> AllRolesExistAsync(List<Guid> roleIds);

        /// <summary>
        /// 取得角色 lookup 資料。
        /// </summary>
        /// <returns>角色 lookup 資料集合。</returns>
        Task<IEnumerable<RoleLookupEntity>> ListRoleLookupsAsync();

        /// <summary>
        /// 以分頁方式取得角色與權限範圍摘要資料。
        /// </summary>
        /// <param name="pageRequest">分頁條件。</param>
        /// <returns>角色與權限範圍摘要的分頁結果。</returns>
        Task<PagedResult<RolePermissionScopeRowEntity>> PageRolePermissionScopeRowsAsync(PageRequest pageRequest);

        /// <summary>
        /// 判斷指定角色名稱是否已存在。
        /// </summary>
        /// <param name="roleName">角色名稱。</param>
        /// <returns>若角色名稱已存在則為 <c>true</c>；否則為 <c>false</c>。</returns>
        Task<bool> RoleNameExistsAsync(string roleName);

        /// <summary>
        /// 建立角色。
        /// </summary>
        /// <param name="roleName">角色名稱。</param>
        /// <param name="roleCode">角色代碼。</param>
        /// <returns>新建立的角色識別碼。</returns>
        Task<Guid> CreateRoleAsync(string roleName, string roleCode);

        /// <summary>
        /// 建立角色、權限與作用範圍的對應資料。
        /// </summary>
        /// <param name="roleId">角色識別碼。</param>
        /// <param name="permissionId">權限識別碼。</param>
        /// <param name="scopeId">作用範圍識別碼。</param>
        /// <returns>表示非同步作業的工作。</returns>
        Task CreateRolePermissionScopeAsync(Guid roleId, Guid permissionId, Guid scopeId);

        /// <summary>
        /// 更新角色狀態並回傳角色識別碼。
        /// </summary>
        /// <param name="roleName">角色名稱。</param>
        /// <param name="status">角色狀態。</param>
        /// <returns>被更新的角色識別碼。</returns>
        Task<Guid> UpdateRoleStatusAsync(string roleName, RoleStatus status);

        /// <summary>
        /// 刪除指定角色。
        /// </summary>
        /// <param name="roleName">角色名稱。</param>
        /// <returns>表示非同步作業的工作。</returns>
        Task DeleteRoleAsync(string roleName);

        /// <summary>
        /// 刪除指定角色的所有權限對應資料。
        /// </summary>
        /// <param name="roleId">角色識別碼。</param>
        /// <returns>表示非同步作業的工作。</returns>
        Task DeleteRolePermissionsAsync(Guid roleId);
    }
}
