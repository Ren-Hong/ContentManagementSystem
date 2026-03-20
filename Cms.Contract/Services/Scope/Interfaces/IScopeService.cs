using Cms.Contract.Services.Scope.Dtos;

namespace Cms.Contract.Services.Scope.Interfaces
{
    public interface IScopeService
    {
        /// <summary>
        /// 列出 scope lookup 資料
        /// </summary>
        /// <returns></returns>
        Task<List<ScopeLookupDto>> ListScopeLookupsAsync();
    }
}
