using Cms.Contract.Repositories.Content.Entities;

namespace Cms.Contract.Repositories.Content.Interfaces
{
    /// <summary>
    /// 提供內容、版本與欄位值相關的資料存取操作。
    /// </summary>
    public interface IContentRepository
    {
        /// <summary>
        /// 判斷指定內容類型是否存在。
        /// </summary>
        /// <param name="typeId">內容類型識別碼。</param>
        /// <returns>若存在則為 <c>true</c>；否則為 <c>false</c>。</returns>
        Task<bool> ContentTypeExistsAsync(Guid typeId);

        /// <summary>
        /// 判斷指定內容是否存在。
        /// </summary>
        /// <param name="contentId">內容識別碼。</param>
        /// <returns>若存在則為 <c>true</c>；否則為 <c>false</c>。</returns>
        Task<bool> ContentExistsAsync(Guid contentId);

        /// <summary>
        /// 判斷指定內容版本是否存在。
        /// </summary>
        /// <param name="revisionId">版本識別碼。</param>
        /// <returns>若存在則為 <c>true</c>；否則為 <c>false</c>。</returns>
        Task<bool> ContentRevisionExistsAsync(Guid revisionId);

        /// <summary>
        /// 判斷指定版本是否屬於指定內容。
        /// </summary>
        /// <param name="contentId">內容識別碼。</param>
        /// <param name="revisionId">版本識別碼。</param>
        /// <returns>若版本屬於內容則為 <c>true</c>；否則為 <c>false</c>。</returns>
        Task<bool> ContentRevisionBelongsToContentAsync(Guid contentId, Guid revisionId);

        /// <summary>
        /// 依版本識別碼取得內容類型識別碼。
        /// </summary>
        /// <param name="revisionId">版本識別碼。</param>
        /// <returns>內容類型識別碼；若查無資料則為 <c>null</c>。</returns>
        Task<Guid?> GetTypeIdByRevisionIdAsync(Guid revisionId);

        /// <summary>
        /// 依內容類型識別碼取得所屬部門識別碼。
        /// </summary>
        /// <param name="typeId">內容類型識別碼。</param>
        /// <returns>部門識別碼；若查無資料則為 <c>null</c>。</returns>
        Task<Guid?> GetDepartmentIdByTypeIdAsync(Guid typeId);

        /// <summary>
        /// 依內容識別碼取得所屬部門識別碼。
        /// </summary>
        /// <param name="contentId">內容識別碼。</param>
        /// <returns>部門識別碼；若查無資料則為 <c>null</c>。</returns>
        Task<Guid?> GetDepartmentIdByContentIdAsync(Guid contentId);

        /// <summary>
        /// 取得指定內容類型的欄位定義。
        /// </summary>
        /// <param name="typeId">內容類型識別碼。</param>
        /// <returns>欄位定義清單。</returns>
        Task<List<ContentTypeFieldDefinitionEntity>> GetFieldDefinitionsByTypeIdAsync(Guid typeId);

        /// <summary>
        /// 取得指定內容類型底下的內容清單。
        /// </summary>
        /// <param name="typeId">內容類型識別碼。</param>
        /// <returns>內容資料清單。</returns>
        Task<List<ContentEntity>> ListContentsAsync(Guid typeId);

        /// <summary>
        /// 建立內容主檔。
        /// </summary>
        /// <param name="typeId">內容類型識別碼。</param>
        /// <param name="ownerId">建立者帳號識別碼。</param>
        /// <param name="status">內容狀態。</param>
        /// <returns>新建立的內容識別碼。</returns>
        Task<Guid> CreateContentAsync(Guid typeId, Guid ownerId, string status);

        /// <summary>
        /// 建立內容版本。
        /// </summary>
        /// <param name="contentId">內容識別碼。</param>
        /// <param name="version">版本號碼。</param>
        /// <returns>新建立的版本識別碼。</returns>
        Task<Guid> CreateContentRevisionAsync(Guid contentId, int version);

        /// <summary>
        /// 建立內容欄位值。
        /// </summary>
        /// <param name="revisionId">版本識別碼。</param>
        /// <param name="fieldId">欄位識別碼。</param>
        /// <param name="fieldValue">欄位值。</param>
        /// <returns>表示非同步作業的工作。</returns>
        Task CreateContentFieldValueAsync(Guid revisionId, Guid fieldId, string? fieldValue);

        /// <summary>
        /// 更新內容欄位值。
        /// </summary>
        /// <param name="revisionId">版本識別碼。</param>
        /// <param name="fieldId">欄位識別碼。</param>
        /// <param name="fieldValue">欄位值。</param>
        /// <returns>表示非同步作業的工作。</returns>
        Task UpdateContentFieldValueAsync(Guid revisionId, Guid fieldId, string? fieldValue);

        /// <summary>
        /// 刪除指定內容底下所有版本的欄位值。
        /// </summary>
        /// <param name="contentId">內容識別碼。</param>
        /// <returns>表示非同步作業的工作。</returns>
        Task DeleteContentFieldValuesByContentIdAsync(Guid contentId);

        /// <summary>
        /// 刪除指定內容的所有版本資料。
        /// </summary>
        /// <param name="contentId">內容識別碼。</param>
        /// <returns>表示非同步作業的工作。</returns>
        Task DeleteContentRevisionsByContentIdAsync(Guid contentId);

        /// <summary>
        /// 刪除指定內容主檔。
        /// </summary>
        /// <param name="contentId">內容識別碼。</param>
        /// <returns>表示非同步作業的工作。</returns>
        Task DeleteContentAsync(Guid contentId);
    }
}
