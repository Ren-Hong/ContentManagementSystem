using Cms.Contract.Repositories.Scope.Entities;

namespace Cms.Contract.Repositories.Scope.Interfaces
{
    /// <summary>
    /// 提供作用範圍相關的資料存取操作。
    /// </summary>
    public interface IScopeRepository
    {
        /// <summary>
        /// 取得作用範圍 lookup 資料。
        /// </summary>
        /// <param name="includeAssigned">是否包含 Assigned 範圍。</param>
        /// <returns>作用範圍 lookup 資料集合。</returns>
        Task<IEnumerable<ScopeLookupEntity>> ListScopeLookupsAsync(bool includeAssigned = false);

        /// <summary>
        /// 判斷指定的作用範圍識別碼是否全部存在。
        /// </summary>
        /// <param name="scopeIds">作用範圍識別碼集合。</param>
        /// <returns>若全部存在則為 <c>true</c>；否則為 <c>false</c>。</returns>
        Task<bool> AllScopesExistAsync(List<Guid> scopeIds);
    }
}
