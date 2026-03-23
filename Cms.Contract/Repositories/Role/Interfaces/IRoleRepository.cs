using Cms.Contract.Common.Pagination;
using Cms.Contract.Repositories.Role.Persistence;
using Cms.Contract.Repositories.Role.Entities;

namespace Cms.Contract.Repositories.Role.Interfaces
{
    public interface IRoleRepository
    {
        /// <summary>
        /// 判斷該角色是否存在
        /// </summary>
        /// <param name="roleIds">前端傳過來的角色Id</param>
        /// <returns>bool</returns>
        Task<bool> AllRolesExistAsync(List<Guid> roleIds);
        
        /// <summary>
        /// 列出角色 lookup 資料
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<RoleLookupEntity>> ListRoleLookupsAsync();

        /// <summary>
        /// 拿角色摘要
        /// </summary>
        /// <returns></returns>
        Task<PagedResult<RolePermissionScopeRowEntity>> PageRolePermissionScopeRowsAsync(PageRequest pageRequest);

        /// <summary>
        /// 判斷角色名稱是否已存在
        /// </summary>
        Task<bool> RoleNameExistsAsync(string roleName);

        /// <summary>
        /// 建立角色
        /// </summary>
        /// <returns></returns>
        Task<Guid> CreateRoleAsync(string roleName, string roleCode);

        /// <summary>
        /// 幫角色權限加作用範圍
        /// </summary>
        /// <returns>不用回傳值, 失敗就是例外</returns>
        Task CreateRolePermissionScopeAsync(Guid roleId, Guid permissionId, Guid ScopeId);

        /// <summary>
        /// 先改狀態後改中介表
        /// </summary>
        /// <returns>後面要改中介表所以回傳guid</returns>
        Task<Guid> UpdateRoleStatusAsync(string roleName, RoleStatus status);

        /// <summary>
        /// 真刪角色
        /// </summary>
        Task DeleteRoleAsync(string roleName);

        /// <summary>
        /// 刪掉帳戶所有對應角色
        /// </summary>
        Task DeleteRolePermissionsAsync(Guid roleId);
    }
}
