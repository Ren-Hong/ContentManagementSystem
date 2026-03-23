using Cms.Contract.Repositories.ContentType.Entities;

namespace Cms.Contract.Repositories.ContentType.Interfaces
{
    /// <summary>
    /// 提供內容類型與內容欄位相關的資料存取操作。
    /// </summary>
    public interface IContentTypeRepository
    {
        /// <summary>
        /// 取得指定部門可用的內容類型 lookup 資料。
        /// </summary>
        /// <param name="departmentId">部門識別碼。</param>
        /// <returns>內容類型 lookup 資料集合。</returns>
        Task<IEnumerable<ContentTypeLookupEntity>> ListContentTypeLookupsAsync(Guid departmentId);

        /// <summary>
        /// 取得指定內容類型的欄位清單。
        /// </summary>
        /// <param name="typeId">內容類型識別碼。</param>
        /// <returns>欄位資料集合。</returns>
        Task<IEnumerable<ContentFieldEntity>> ListContentFieldsAsync(Guid typeId);

        /// <summary>
        /// 判斷指定部門下的內容類型名稱是否已存在。
        /// </summary>
        /// <param name="departmentId">部門識別碼。</param>
        /// <param name="typeName">內容類型名稱。</param>
        /// <returns>若已存在則為 <c>true</c>；否則為 <c>false</c>。</returns>
        Task<bool> TypeNameExistsAsync(Guid departmentId, string typeName);

        /// <summary>
        /// 依部門與內容類型名稱取得內容類型識別碼。
        /// </summary>
        /// <param name="departmentId">部門識別碼。</param>
        /// <param name="typeName">內容類型名稱。</param>
        /// <returns>內容類型識別碼；若查無資料則為 <c>null</c>。</returns>
        Task<Guid?> GetTypeIdByDepartmentIdAndTypeNameAsync(Guid departmentId, string typeName);

        /// <summary>
        /// 判斷指定內容類型是否存在。
        /// </summary>
        /// <param name="typeId">內容類型識別碼。</param>
        /// <returns>若存在則為 <c>true</c>；否則為 <c>false</c>。</returns>
        Task<bool> TypeExistsAsync(Guid typeId);

        /// <summary>
        /// 依內容類型識別碼取得所屬部門識別碼。
        /// </summary>
        /// <param name="typeId">內容類型識別碼。</param>
        /// <returns>部門識別碼；若查無資料則為 <c>null</c>。</returns>
        Task<Guid?> GetDepartmentIdByTypeIdAsync(Guid typeId);

        /// <summary>
        /// 建立內容類型。
        /// </summary>
        /// <param name="departmentId">部門識別碼。</param>
        /// <param name="typeName">內容類型名稱。</param>
        /// <param name="typeCode">內容類型代碼。</param>
        /// <returns>新建立的內容類型識別碼。</returns>
        Task<Guid> CreateContentTypeAsync(Guid departmentId, string typeName, string typeCode);

        /// <summary>
        /// 建立內容欄位。
        /// </summary>
        /// <param name="fieldCode">欄位代碼。</param>
        /// <param name="fieldType">欄位型別。</param>
        /// <param name="isRequired">欄位是否必填。</param>
        /// <returns>新建立的欄位識別碼。</returns>
        Task<Guid> CreateContentFieldAsync(string fieldCode, string fieldType, bool isRequired);

        /// <summary>
        /// 建立內容類型與欄位的對應關係。
        /// </summary>
        /// <param name="typeId">內容類型識別碼。</param>
        /// <param name="fieldId">欄位識別碼。</param>
        /// <param name="sortOrder">排序。</param>
        /// <returns>表示非同步作業的工作。</returns>
        Task CreateContentTypeFieldAsync(Guid typeId, Guid fieldId, int sortOrder);

        /// <summary>
        /// 更新內容類型名稱。
        /// </summary>
        /// <param name="typeId">內容類型識別碼。</param>
        /// <param name="typeName">內容類型名稱。</param>
        /// <returns>表示非同步作業的工作。</returns>
        Task UpdateContentTypeAsync(Guid typeId, string typeName);

        /// <summary>
        /// 更新內容欄位設定。
        /// </summary>
        /// <param name="fieldId">欄位識別碼。</param>
        /// <param name="fieldCode">欄位代碼。</param>
        /// <param name="fieldType">欄位型別。</param>
        /// <param name="isRequired">欄位是否必填。</param>
        /// <returns>表示非同步作業的工作。</returns>
        Task UpdateContentFieldAsync(Guid fieldId, string fieldCode, string fieldType, bool isRequired);

        /// <summary>
        /// 更新內容類型欄位排序。
        /// </summary>
        /// <param name="typeId">內容類型識別碼。</param>
        /// <param name="fieldId">欄位識別碼。</param>
        /// <param name="sortOrder">排序。</param>
        /// <returns>表示非同步作業的工作。</returns>
        Task UpdateContentTypeFieldSortOrderAsync(Guid typeId, Guid fieldId, int sortOrder);

        /// <summary>
        /// 刪除內容類型與欄位的對應關係。
        /// </summary>
        /// <param name="typeId">內容類型識別碼。</param>
        /// <param name="fieldId">欄位識別碼。</param>
        /// <returns>表示非同步作業的工作。</returns>
        Task DeleteContentTypeFieldAsync(Guid typeId, Guid fieldId);

        /// <summary>
        /// 刪除內容欄位。
        /// </summary>
        /// <param name="fieldId">欄位識別碼。</param>
        /// <returns>表示非同步作業的工作。</returns>
        Task DeleteContentFieldAsync(Guid fieldId);

        /// <summary>
        /// 刪除內容類型。
        /// </summary>
        /// <param name="typeId">內容類型識別碼。</param>
        /// <returns>表示非同步作業的工作。</returns>
        Task DeleteContentTypeAsync(Guid typeId);

        /// <summary>
        /// 判斷指定內容類型下是否已有內容資料。
        /// </summary>
        /// <param name="typeId">內容類型識別碼。</param>
        /// <returns>若已有內容資料則為 <c>true</c>；否則為 <c>false</c>。</returns>
        Task<bool> ContentExistsByTypeIdAsync(Guid typeId);

        /// <summary>
        /// 判斷指定欄位是否已被內容欄位值使用。
        /// </summary>
        /// <param name="fieldId">欄位識別碼。</param>
        /// <returns>若已有欄位值資料則為 <c>true</c>；否則為 <c>false</c>。</returns>
        Task<bool> ContentFieldValueExistsAsync(Guid fieldId);
    }
}
