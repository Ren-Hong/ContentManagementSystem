using Cms.Contract.Repositories.ContentType.Entities;
using Cms.Contract.Repositories.ContentType.Interfaces;
using Cms.Contract.Services.UnitOfWork.Interfaces;
using Cms.Infrastructure.Repositories.Base;
using Dapper;
using System.Data;

namespace Cms.Infrastructure.Repositories.ContentType
{
    public class ContentTypeRepository : BaseRepository, IContentTypeRepository
    {
        public ContentTypeRepository(
            IDbConnection db,
            IUnitOfWork unitOfWork
        ) : base(db, unitOfWork)
        {
        }

        public async Task<IEnumerable<ContentTypeLookupEntity>> ListContentTypeLookupsAsync(Guid departmentId)
        {
            const string sql = @"
                SELECT
                    RAWTOHEX(TypeId) AS TypeId,
                    TypeCode,
                    TypeName
                FROM ContentTypes
                WHERE DepartmentId = HEXTORAW(:DepartmentId)
                    AND IsEnabled = 1
                ORDER BY TypeName
            ";

            return await _db.QueryAsync<ContentTypeLookupEntity>(
                sql,
                new
                {
                    DepartmentId = ToRaw(departmentId)
                },
                transaction: Tx
            );
        }

        public async Task<IEnumerable<ContentFieldEntity>> ListContentFieldsAsync(Guid typeId)
        {
            const string sql = @"
                SELECT
                    RAWTOHEX(cf.FieldId) AS FieldId,
                    cf.FieldCode,
                    cf.FieldType,
                    cf.IsRequired
                FROM ContentTypeFields ctf
                INNER JOIN ContentFields cf
                    ON cf.FieldId = ctf.FieldId
                WHERE ctf.TypeId = HEXTORAW(:TypeId)
                ORDER BY ctf.SortOrder, cf.FieldCode
            ";

            return await _db.QueryAsync<ContentFieldEntity>(
                sql,
                new
                {
                    TypeId = ToRaw(typeId)
                },
                transaction: Tx
            );
        }

        public async Task<bool> TypeNameExistsAsync(Guid departmentId, string typeName)
        {
            const string sql = @"
                SELECT 1
                FROM ContentTypes
                WHERE DepartmentId = HEXTORAW(:DepartmentId)
                    AND TypeName = :TypeName
            ";

            var result = await _db.ExecuteScalarAsync<int?>(
                sql,
                new
                {
                    DepartmentId = ToRaw(departmentId),
                    TypeName = typeName
                },
                transaction: Tx
            );

            return result.HasValue;
        }

        public async Task<Guid?> GetTypeIdByDepartmentIdAndTypeNameAsync(Guid departmentId, string typeName)
        {
            const string sql = @"
                SELECT RAWTOHEX(TypeId) AS TypeId
                FROM ContentTypes
                WHERE DepartmentId = HEXTORAW(:DepartmentId)
                    AND TypeName = :TypeName
            ";

            var typeIdHex = await _db.ExecuteScalarAsync<string?>(
                sql,
                new
                {
                    DepartmentId = ToRaw(departmentId),
                    TypeName = typeName
                },
                transaction: Tx
            );

            return string.IsNullOrWhiteSpace(typeIdHex)
                ? null
                : Guid.Parse(typeIdHex);
        }

        public async Task<bool> TypeExistsAsync(Guid typeId)
        {
            const string sql = @"
                SELECT 1
                FROM ContentTypes
                WHERE TypeId = HEXTORAW(:TypeId)
            ";

            var result = await _db.ExecuteScalarAsync<int?>(
                sql,
                new { TypeId = ToRaw(typeId) },
                transaction: Tx
            );

            return result.HasValue;
        }

        public async Task<Guid?> GetDepartmentIdByTypeIdAsync(Guid typeId)
        {
            const string sql = @"
                SELECT RAWTOHEX(DepartmentId) AS DepartmentId
                FROM ContentTypes
                WHERE TypeId = HEXTORAW(:TypeId)
            ";

            var departmentIdHex = await _db.ExecuteScalarAsync<string?>(
                sql,
                new { TypeId = ToRaw(typeId) },
                transaction: Tx
            );

            return string.IsNullOrWhiteSpace(departmentIdHex)
                ? null
                : Guid.Parse(departmentIdHex);
        }

        public async Task<Guid> CreateContentTypeAsync(Guid departmentId, string typeName, string typeCode)
        {
            var typeId = Guid.NewGuid();

            const string sql = @"
                INSERT INTO ContentTypes
                (
                    TypeId,
                    DepartmentId,
                    TypeCode,
                    TypeName
                )
                VALUES
                (
                    HEXTORAW(:TypeId),
                    HEXTORAW(:DepartmentId),
                    :TypeCode,
                    :TypeName
                )
            ";

            await _db.ExecuteAsync(
                sql,
                new
                {
                    TypeId = ToRaw(typeId),
                    DepartmentId = ToRaw(departmentId),
                    TypeName = typeName,
                    TypeCode = typeCode
                },
                transaction: Tx
            );

            return typeId;
        }

        public async Task UpdateContentTypeAsync(Guid typeId, string typeName)
        {
            const string sql = @"
                UPDATE ContentTypes
                SET TypeName = :TypeName
                WHERE TypeId = HEXTORAW(:TypeId)
            ";

            await _db.ExecuteAsync(
                sql,
                new
                {
                    TypeId = ToRaw(typeId),
                    TypeName = typeName
                },
                transaction: Tx
            );
        }

        public async Task<Guid> CreateContentFieldAsync(string fieldCode, string fieldType, bool isRequired)
        {
            var fieldId = Guid.NewGuid();

            const string sql = @"
                INSERT INTO ContentFields
                (
                    FieldId,
                    FieldCode,
                    FieldType,
                    IsRequired
                )
                VALUES
                (
                    HEXTORAW(:FieldId),
                    :FieldCode,
                    :FieldType,
                    :IsRequired
                )
            ";

            await _db.ExecuteAsync(
                sql,
                new
                {
                    FieldId = ToRaw(fieldId),
                    FieldCode = fieldCode,
                    FieldType = fieldType,
                    IsRequired = isRequired ? 1 : 0
                },
                transaction: Tx
            );

            return fieldId;
        }

        public async Task UpdateContentFieldAsync(Guid fieldId, string fieldCode, string fieldType, bool isRequired)
        {
            const string sql = @"
                UPDATE ContentFields
                SET
                    FieldCode = :FieldCode,
                    FieldType = :FieldType,
                    IsRequired = :IsRequired
                WHERE FieldId = HEXTORAW(:FieldId)
            ";

            await _db.ExecuteAsync(
                sql,
                new
                {
                    FieldId = ToRaw(fieldId),
                    FieldCode = fieldCode,
                    FieldType = fieldType,
                    IsRequired = isRequired ? 1 : 0
                },
                transaction: Tx
            );
        }

        public async Task CreateContentTypeFieldAsync(Guid typeId, Guid fieldId, int sortOrder)
        {
            const string sql = @"
                INSERT INTO ContentTypeFields
                (
                    TypeId,
                    FieldId,
                    SortOrder
                )
                VALUES
                (
                    HEXTORAW(:TypeId),
                    HEXTORAW(:FieldId),
                    :SortOrder
                )
            ";

            await _db.ExecuteAsync(
                sql,
                new
                {
                    TypeId = ToRaw(typeId),
                    FieldId = ToRaw(fieldId),
                    SortOrder = sortOrder
                },
                transaction: Tx
            );
        }

        public async Task UpdateContentTypeFieldSortOrderAsync(Guid typeId, Guid fieldId, int sortOrder)
        {
            const string sql = @"
                UPDATE ContentTypeFields
                SET SortOrder = :SortOrder
                WHERE TypeId = HEXTORAW(:TypeId)
                    AND FieldId = HEXTORAW(:FieldId)
            ";

            await _db.ExecuteAsync(
                sql,
                new
                {
                    TypeId = ToRaw(typeId),
                    FieldId = ToRaw(fieldId),
                    SortOrder = sortOrder
                },
                transaction: Tx
            );
        }

        public async Task DeleteContentTypeFieldAsync(Guid typeId, Guid fieldId)
        {
            const string sql = @"
                DELETE FROM ContentTypeFields
                WHERE TypeId = HEXTORAW(:TypeId)
                    AND FieldId = HEXTORAW(:FieldId)
            ";

            await _db.ExecuteAsync(
                sql,
                new
                {
                    TypeId = ToRaw(typeId),
                    FieldId = ToRaw(fieldId)
                },
                transaction: Tx
            );
        }

        public async Task DeleteContentFieldAsync(Guid fieldId)
        {
            const string sql = @"
                DELETE FROM ContentFields
                WHERE FieldId = HEXTORAW(:FieldId)
            ";

            await _db.ExecuteAsync(
                sql,
                new { FieldId = ToRaw(fieldId) },
                transaction: Tx
            );
        }

        public async Task DeleteContentTypeAsync(Guid typeId)
        {
            const string sql = @"
                DELETE FROM ContentTypes
                WHERE TypeId = HEXTORAW(:TypeId)
            ";

            await _db.ExecuteAsync(
                sql,
                new { TypeId = ToRaw(typeId) },
                transaction: Tx
            );
        }

        public async Task<bool> ContentExistsByTypeIdAsync(Guid typeId)
        {
            const string sql = @"
                SELECT 1
                FROM Contents
                WHERE TypeId = HEXTORAW(:TypeId)
            ";

            var result = await _db.ExecuteScalarAsync<int?>(
                sql,
                new { TypeId = ToRaw(typeId) },
                transaction: Tx
            );

            return result.HasValue;
        }

        public async Task<bool> ContentFieldValueExistsAsync(Guid fieldId)
        {
            const string sql = @"
                SELECT 1
                FROM ContentFieldValues
                WHERE FieldId = HEXTORAW(:FieldId)
            ";

            var result = await _db.ExecuteScalarAsync<int?>(
                sql,
                new { FieldId = ToRaw(fieldId) },
                transaction: Tx
            );

            return result.HasValue;
        }
    }
}
