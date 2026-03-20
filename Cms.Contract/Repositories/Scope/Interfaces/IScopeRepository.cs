using Cms.Contract.Repositories.Scope.Entities;

namespace Cms.Contract.Repositories.Scope.Interfaces
{
    public interface IScopeRepository
    {
        /// <summary>
        /// 列出 scope lookup 資料
        /// </summary>
        /// <param name="includeAssigned">預設不給assign</param>
        /// <returns></returns>
        Task<IEnumerable<ScopeLookupEntity>> ListScopeLookupsAsync(bool includeAssigned = false);

        /// <summary>
        /// 判斷該作用範圍是否存在
        /// </summary>
        /// <returns>bool</returns>
        Task<bool> AllScopesExistAsync(List<Guid> scopeIds);
    }
}
