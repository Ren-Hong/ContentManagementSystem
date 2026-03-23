using Cms.Contract.Services.Permission.Dtos;

namespace Cms.Contract.Services.Permission.Interfaces
{
    /// <summary>
    /// 提供權限查詢相關的應用服務操作。
    /// </summary>
    public interface IPermissionService
    {
        /// <summary>
        /// 取得權限 lookup 資料。
        /// </summary>
        /// <returns>權限 lookup 資料清單。</returns>
        Task<List<PermissionLookupDto>> ListPermissionLookupsAsync();
    }
}
