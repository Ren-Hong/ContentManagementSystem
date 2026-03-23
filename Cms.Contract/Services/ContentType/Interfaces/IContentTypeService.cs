using Cms.Contract.Services.ContentType.Dtos;

namespace Cms.Contract.Services.ContentType.Interfaces
{
    /// <summary>
    /// 提供內容類型與欄位管理相關的應用服務操作。
    /// </summary>
    public interface IContentTypeService
    {
        /// <summary>
        /// 取得指定條件下可用的內容類型 lookup 資料。
        /// </summary>
        /// <param name="requestDto">內容類型查詢條件。</param>
        /// <param name="accountId">目前登入帳號識別碼。</param>
        /// <returns>內容類型 lookup 資料清單。</returns>
        Task<List<ContentTypeLookupDto>> ListContentTypeLookupsAsync(ListContentTypeLookupsRequestDto requestDto, Guid accountId);

        /// <summary>
        /// 取得指定內容類型的欄位清單。
        /// </summary>
        /// <param name="typeId">內容類型識別碼。</param>
        /// <param name="accountId">目前登入帳號識別碼。</param>
        /// <returns>內容欄位清單。</returns>
        Task<List<GetContentFieldsResponseDto>> ListContentFieldsAsync(Guid typeId, Guid accountId);

        /// <summary>
        /// 建立內容類型。
        /// </summary>
        /// <param name="req">建立內容類型請求資料。</param>
        /// <returns>建立內容類型結果。</returns>
        Task<CreateContentTypeResponseDto> CreateContentTypeAsync(CreateContentTypeRequestDto req);

        /// <summary>
        /// 更新內容類型。
        /// </summary>
        /// <param name="req">更新內容類型請求資料。</param>
        /// <returns>更新內容類型結果。</returns>
        Task<UpdateContentTypeResponseDto> UpdateContentTypeAsync(UpdateContentTypeRequestDto req);

        /// <summary>
        /// 刪除內容類型。
        /// </summary>
        /// <param name="req">刪除內容類型請求資料。</param>
        /// <returns>刪除內容類型結果。</returns>
        Task<DeleteContentTypeResponseDto> DeleteContentTypeAsync(DeleteContentTypeRequestDto req);
    }
}
