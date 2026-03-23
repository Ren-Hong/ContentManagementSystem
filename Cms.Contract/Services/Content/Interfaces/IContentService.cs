using Cms.Contract.Services.Content.Dtos;

namespace Cms.Contract.Services.Content.Interfaces
{
    /// <summary>
    /// 提供內容建立、查詢、更新與刪除相關的應用服務操作。
    /// </summary>
    public interface IContentService
    {
        /// <summary>
        /// 建立內容。
        /// </summary>
        /// <param name="requestDto">建立內容請求資料。</param>
        /// <returns>建立內容結果。</returns>
        Task<CreateContentResponseDto> CreateContentAsync(CreateContentRequestDto requestDto);

        /// <summary>
        /// 取得指定內容類型底下的內容清單。
        /// </summary>
        /// <param name="typeId">內容類型識別碼。</param>
        /// <param name="accountId">目前登入帳號識別碼。</param>
        /// <returns>內容清單。</returns>
        Task<List<GetContentsResponseDto>> ListContentsAsync(Guid typeId, Guid accountId);

        /// <summary>
        /// 更新內容。
        /// </summary>
        /// <param name="requestDto">更新內容請求資料。</param>
        /// <param name="accountId">目前登入帳號識別碼。</param>
        /// <returns>更新內容結果。</returns>
        Task<UpdateContentResponseDto> UpdateContentAsync(UpdateContentRequestDto requestDto, Guid accountId);

        /// <summary>
        /// 刪除內容。
        /// </summary>
        /// <param name="requestDto">刪除內容請求資料。</param>
        /// <param name="accountId">目前登入帳號識別碼。</param>
        /// <returns>刪除內容結果。</returns>
        Task<DeleteContentResponseDto> DeleteContentAsync(DeleteContentRequestDto requestDto, Guid accountId);
    }
}
