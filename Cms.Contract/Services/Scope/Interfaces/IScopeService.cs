using Cms.Contract.Services.Scope.Dtos;

namespace Cms.Contract.Services.Scope.Interfaces
{
    /// <summary>
    /// 提供作用範圍查詢相關的應用服務操作。
    /// </summary>
    public interface IScopeService
    {
        /// <summary>
        /// 取得作用範圍 lookup 資料。
        /// </summary>
        /// <returns>作用範圍 lookup 資料清單。</returns>
        Task<List<ScopeLookupDto>> ListScopeLookupsAsync();
    }
}
