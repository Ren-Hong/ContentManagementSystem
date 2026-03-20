using Cms.Contract.Services.Permission.Dtos;

namespace Cms.Contract.Services.Permission.Interfaces
{
    public interface IPermissionService
    {
        /// <summary>
        /// 列出權限 lookup 資料
        /// </summary>
        /// <returns></returns>
        Task<List<PermissionLookupDto>> ListPermissionLookupsAsync();
    }
}
