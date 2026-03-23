using Cms.Contract.Common.Pagination;
using Cms.Contract.Services.Role.Dtos;

namespace Cms.Contract.Services.Role.Interfaces
{
    /// <summary>
    /// 提供角色查詢與角色管理相關的應用服務操作。
    /// </summary>
    public interface IRoleService
    {
        /// <summary>
        /// 取得角色 lookup 資料。
        /// </summary>
        /// <returns>角色 lookup 資料清單。</returns>
        Task<List<RoleLookupDto>> ListRoleLookupsAsync();

        /// <summary>
        /// 以分頁方式取得角色摘要資料。
        /// </summary>
        /// <param name="pageRequest">分頁條件。</param>
        /// <returns>角色摘要的分頁結果。</returns>
        Task<PagedResult<RoleListResponseDto>> PageRolesAsync(PageRequest pageRequest);

        /// <summary>
        /// 建立角色。
        /// </summary>
        /// <param name="req">建立角色請求資料。</param>
        /// <returns>建立角色結果。</returns>
        Task<CreateRoleResponseDto> CreateRoleAsync(CreateRoleRequestDto req);

        /// <summary>
        /// 更新角色。
        /// </summary>
        /// <param name="req">更新角色請求資料。</param>
        /// <returns>更新角色結果。</returns>
        Task<UpdateRoleResponseDto> UpdateRoleAsync(UpdateRoleRequestDto req);

        /// <summary>
        /// 刪除角色。
        /// </summary>
        /// <param name="req">刪除角色請求資料。</param>
        /// <returns>刪除角色結果。</returns>
        Task<DeleteRoleResponseDto> DeleteRoleAsync(DeleteRoleRequestDto req);
    }
}
