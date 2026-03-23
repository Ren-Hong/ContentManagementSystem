using Cms.Contract.Common.Pagination;
using Cms.Contract.Repositories.Account.Entities;
using Cms.Contract.Repositories.Account.Persistence;

namespace Cms.Contract.Repositories.Account.Interfaces
{
    /// <summary>
    /// 提供帳號與帳號角色指派相關的資料存取操作。
    /// </summary>
    public interface IAccountRepository
    {
        /// <summary>
        /// 依帳號名稱取得登入驗證所需的帳號、角色與權限資料。
        /// </summary>
        /// <param name="username">帳號名稱。</param>
        /// <returns>符合帳號名稱的驗證資料集合。</returns>
        Task<IEnumerable<AccountAuthEntity>> GetAccountAuthByUsernameAsync(string username);

        /// <summary>
        /// 判斷帳號是否具有指定權限的全域範圍。
        /// </summary>
        /// <param name="accountId">帳號識別碼。</param>
        /// <param name="permissionCode">權限代碼。</param>
        /// <returns>若具有全域範圍則為 <c>true</c>；否則為 <c>false</c>。</returns>
        Task<bool> HasGlobalPermissionScopeAsync(Guid accountId, string permissionCode);

        /// <summary>
        /// 取得帳號在指定權限下可存取的部門識別碼清單。
        /// </summary>
        /// <param name="accountId">帳號識別碼。</param>
        /// <param name="permissionCode">權限代碼。</param>
        /// <returns>可存取的部門識別碼清單。</returns>
        Task<List<Guid>> GetDepartmentIdsByPermissionScopeAsync(Guid accountId, string permissionCode);

        /// <summary>
        /// 更新帳號最後登入時間。
        /// </summary>
        /// <param name="accountId">帳號識別碼。</param>
        /// <param name="loginTime">登入時間。</param>
        /// <returns>表示非同步作業的工作。</returns>
        Task UpdateLastLoginAtAsync(Guid accountId, DateTime loginTime);

        /// <summary>
        /// 以分頁方式取得帳號角色指派摘要資料。
        /// </summary>
        /// <param name="pageRequest">分頁條件。</param>
        /// <returns>帳號角色指派摘要的分頁結果。</returns>
        Task<PagedResult<AccountRoleAssignmentRowEntity>> PageAccountRoleAssignmentRowsAsync(PageRequest pageRequest);

        /// <summary>
        /// 判斷指定帳號名稱是否已存在。
        /// </summary>
        /// <param name="username">帳號名稱。</param>
        /// <returns>若帳號已存在則為 <c>true</c>；否則為 <c>false</c>。</returns>
        Task<bool> AccountExistsAsync(string username);

        /// <summary>
        /// 建立帳號並回傳新帳號識別碼。
        /// </summary>
        /// <param name="username">帳號名稱。</param>
        /// <param name="passwordHash">雜湊後的密碼。</param>
        /// <returns>新建立的帳號識別碼。</returns>
        Task<Guid> CreateAccountAsync(string username, string passwordHash);

        /// <summary>
        /// 建立單筆帳號、角色與部門的指派資料。
        /// </summary>
        /// <param name="accountId">帳號識別碼。</param>
        /// <param name="roleId">角色識別碼。</param>
        /// <param name="departmentId">部門識別碼。</param>
        /// <returns>表示非同步作業的工作。</returns>
        Task CreateAccountRoleAssignmentAsync(Guid accountId, Guid roleId, Guid departmentId);

        /// <summary>
        /// 批次建立帳號、角色與部門的指派資料。
        /// </summary>
        /// <param name="rows">要建立的指派資料列集合。</param>
        /// <returns>表示非同步作業的工作。</returns>
        Task CreateAccountRoleAssignmentsAsync(IEnumerable<object> rows);

        /// <summary>
        /// 更新帳號狀態並回傳該帳號識別碼。
        /// </summary>
        /// <param name="username">帳號名稱。</param>
        /// <param name="status">欲更新的帳號狀態。</param>
        /// <returns>被更新的帳號識別碼。</returns>
        Task<Guid> UpdateAccountStatusAsync(string username, AccountStatus status);

        /// <summary>
        /// 更新帳號密碼。
        /// </summary>
        /// <param name="username">帳號名稱。</param>
        /// <param name="password">雜湊後的密碼。</param>
        /// <returns>表示非同步作業的工作。</returns>
        Task UpdatePasswordAsync(string username, string password);

        /// <summary>
        /// 更新帳號最後修改時間。
        /// </summary>
        /// <param name="username">帳號名稱。</param>
        /// <param name="updatedTime">更新時間。</param>
        /// <returns>表示非同步作業的工作。</returns>
        Task UpdateUpdatedAtAsync(string username, DateTime updatedTime);

        /// <summary>
        /// 刪除指定帳號。
        /// </summary>
        /// <param name="username">帳號名稱。</param>
        /// <returns>表示非同步作業的工作。</returns>
        Task DeleteAccountAsync(string username);

        /// <summary>
        /// 刪除指定帳號的所有角色指派資料。
        /// </summary>
        /// <param name="accountId">帳號識別碼。</param>
        /// <returns>表示非同步作業的工作。</returns>
        Task DeleteAccountRoleAssignmentsAsync(Guid accountId);

        /// <summary>
        /// 新增一筆帳號角色指派資料。
        /// </summary>
        /// <param name="accountId">帳號識別碼。</param>
        /// <param name="roleId">角色識別碼。</param>
        /// <param name="departmentId">部門識別碼。</param>
        /// <returns>表示非同步作業的工作。</returns>
        Task AddAccountRoleAssignmentAsync(Guid accountId, Guid roleId, Guid departmentId);
    }
}
