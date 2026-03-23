using Cms.Contract.Repositories.Content.Entities;
using Cms.Contract.Repositories.Content.Interfaces;
using Cms.Contract.Services.UnitOfWork.Interfaces;
using Cms.Infrastructure.Repositories.Base;
using Dapper;
using System.Data;

namespace Cms.Infrastructure.Repositories.Content
{
    public class ContentRepository : BaseRepository, IContentRepository
    {
        public ContentRepository(
            IDbConnection db,
            IUnitOfWork unitOfWork
        ) : base(db, unitOfWork)
        {
        }

        public async Task<bool> ContentTypeExistsAsync(Guid typeId)
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

        public async Task<bool> ContentExistsAsync(Guid contentId)
        {
            const string sql = @"
                SELECT 1
                FROM Contents
                WHERE ContentId = HEXTORAW(:ContentId)
            ";

            var result = await _db.ExecuteScalarAsync<int?>(
                sql,
                new { ContentId = ToRaw(contentId) },
                transaction: Tx
            );

            return result.HasValue;
        }

        public async Task<bool> ContentRevisionExistsAsync(Guid revisionId)
        {
            const string sql = @"
                SELECT 1
                FROM ContentRevisions
                WHERE RevisionId = HEXTORAW(:RevisionId)
            ";

            var result = await _db.ExecuteScalarAsync<int?>(
                sql,
                new { RevisionId = ToRaw(revisionId) },
                transaction: Tx
            );

            return result.HasValue;
        }

        public async Task<bool> ContentRevisionBelongsToContentAsync(Guid contentId, Guid revisionId)
        {
            const string sql = @"
                SELECT 1
                FROM ContentRevisions
                WHERE ContentId = HEXTORAW(:ContentId)
                    AND RevisionId = HEXTORAW(:RevisionId)
            ";

            var result = await _db.ExecuteScalarAsync<int?>(
                sql,
                new
                {
                    ContentId = ToRaw(contentId),
                    RevisionId = ToRaw(revisionId)
                },
                transaction: Tx
            );

            return result.HasValue;
        }

        public async Task<Guid?> GetTypeIdByRevisionIdAsync(Guid revisionId)
        {
            const string sql = @"
                SELECT RAWTOHEX(c.TypeId) AS TypeId
                FROM ContentRevisions cr
                INNER JOIN Contents c
                    ON c.ContentId = cr.ContentId
                WHERE cr.RevisionId = HEXTORAW(:RevisionId)
            ";

            var typeIdHex = await _db.ExecuteScalarAsync<string?>(
                sql,
                new { RevisionId = ToRaw(revisionId) },
                transaction: Tx
            );

            return string.IsNullOrWhiteSpace(typeIdHex)
                ? null
                : Guid.Parse(typeIdHex);
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

        public async Task<Guid?> GetDepartmentIdByContentIdAsync(Guid contentId)
        {
            const string sql = @"
                SELECT RAWTOHEX(ct.DepartmentId) AS DepartmentId
                FROM Contents c
                INNER JOIN ContentTypes ct
                    ON ct.TypeId = c.TypeId
                WHERE c.ContentId = HEXTORAW(:ContentId)
            ";

            var departmentIdHex = await _db.ExecuteScalarAsync<string?>(
                sql,
                new { ContentId = ToRaw(contentId) },
                transaction: Tx
            );

            return string.IsNullOrWhiteSpace(departmentIdHex)
                ? null
                : Guid.Parse(departmentIdHex);
        }

        public async Task<List<ContentTypeFieldDefinitionEntity>> GetFieldDefinitionsByTypeIdAsync(Guid typeId)
        {
            const string sql = @"
                SELECT
                    RAWTOHEX(cf.FieldId) AS FieldId,
                    cf.FieldType,
                    cf.IsRequired
                FROM ContentTypeFields ctf
                INNER JOIN ContentFields cf
                    ON cf.FieldId = ctf.FieldId
                WHERE ctf.TypeId = HEXTORAW(:TypeId)
                ORDER BY ctf.SortOrder, cf.FieldCode
            ";

            var fields = await _db.QueryAsync<ContentTypeFieldDefinitionEntity>(
                sql,
                new { TypeId = ToRaw(typeId) },
                transaction: Tx
            );

            return fields.ToList();
        }

        public async Task<List<ContentEntity>> ListContentsAsync(Guid typeId)
        {
            const string sql = @"
                SELECT
                    RAWTOHEX(c.ContentId) AS ContentId,
                    RAWTOHEX(cr.RevisionId) AS RevisionId,
                    cr.Version,
                    RAWTOHEX(c.OwnerId) AS OwnerId,
                    a.Username AS OwnerUsername,
                    c.Status,
                    cr.CreatedAt,
                    RAWTOHEX(cf.FieldId) AS FieldId,
                    cf.FieldCode,
                    cf.FieldType,
                    cfv.FieldValue,
                    COALESCE(ctf.SortOrder, 0) AS SortOrder
                FROM Contents c
                INNER JOIN ContentRevisions cr
                    ON cr.ContentId = c.ContentId
                INNER JOIN Accounts a
                    ON a.AccountId = c.OwnerId
                INNER JOIN ContentFieldValues cfv
                    ON cfv.RevisionId = cr.RevisionId
                INNER JOIN ContentFields cf
                    ON cf.FieldId = cfv.FieldId
                LEFT JOIN ContentTypeFields ctf
                    ON ctf.TypeId = c.TypeId
                    AND ctf.FieldId = cf.FieldId
                WHERE c.TypeId = HEXTORAW(:TypeId)
                ORDER BY cr.CreatedAt DESC, COALESCE(ctf.SortOrder, 0), cf.FieldCode
            ";

            var rows = await _db.QueryAsync<ContentEntity>(
                sql,
                new { TypeId = ToRaw(typeId) },
                transaction: Tx
            );

            return rows.ToList();
        }

        public async Task<Guid> CreateContentAsync(Guid typeId, Guid ownerId, string status)
        {
            var contentId = Guid.NewGuid();

            const string sql = @"
                INSERT INTO Contents
                (
                    ContentId,
                    TypeId,
                    OwnerId,
                    Status
                )
                VALUES
                (
                    HEXTORAW(:ContentId),
                    HEXTORAW(:TypeId),
                    HEXTORAW(:OwnerId),
                    :Status
                )
            ";

            await _db.ExecuteAsync(
                sql,
                new
                {
                    ContentId = ToRaw(contentId),
                    TypeId = ToRaw(typeId),
                    OwnerId = ToRaw(ownerId),
                    Status = status
                },
                transaction: Tx
            );

            return contentId;
        }

        public async Task<Guid> CreateContentRevisionAsync(Guid contentId, int version)
        {
            var revisionId = Guid.NewGuid();

            const string sql = @"
                INSERT INTO ContentRevisions
                (
                    RevisionId,
                    ContentId,
                    Version
                )
                VALUES
                (
                    HEXTORAW(:RevisionId),
                    HEXTORAW(:ContentId),
                    :Version
                )
            ";

            await _db.ExecuteAsync(
                sql,
                new
                {
                    RevisionId = ToRaw(revisionId),
                    ContentId = ToRaw(contentId),
                    Version = version
                },
                transaction: Tx
            );

            return revisionId;
        }

        public async Task CreateContentFieldValueAsync(Guid revisionId, Guid fieldId, string? fieldValue)
        {
            const string sql = @"
                INSERT INTO ContentFieldValues
                (
                    RevisionId,
                    FieldId,
                    FieldValue
                )
                VALUES
                (
                    HEXTORAW(:RevisionId),
                    HEXTORAW(:FieldId),
                    :FieldValue
                )
            ";

            await _db.ExecuteAsync(
                sql,
                new
                {
                    RevisionId = ToRaw(revisionId),
                    FieldId = ToRaw(fieldId),
                    FieldValue = fieldValue
                },
                transaction: Tx
            );
        }

        public async Task UpdateContentFieldValueAsync(Guid revisionId, Guid fieldId, string? fieldValue)
        {
            const string sql = @"
                UPDATE ContentFieldValues
                SET FieldValue = :FieldValue
                WHERE RevisionId = HEXTORAW(:RevisionId)
                    AND FieldId = HEXTORAW(:FieldId)
            ";

            await _db.ExecuteAsync(
                sql,
                new
                {
                    RevisionId = ToRaw(revisionId),
                    FieldId = ToRaw(fieldId),
                    FieldValue = fieldValue
                },
                transaction: Tx
            );
        }

        public async Task DeleteContentFieldValuesByContentIdAsync(Guid contentId)
        {
            const string sql = @"
                DELETE FROM ContentFieldValues
                WHERE RevisionId IN
                (
                    SELECT cr.RevisionId
                    FROM ContentRevisions cr
                    WHERE cr.ContentId = HEXTORAW(:ContentId)
                )
            ";

            await _db.ExecuteAsync(
                sql,
                new { ContentId = ToRaw(contentId) },
                transaction: Tx
            );
        }

        public async Task DeleteContentRevisionsByContentIdAsync(Guid contentId)
        {
            const string sql = @"
                DELETE FROM ContentRevisions
                WHERE ContentId = HEXTORAW(:ContentId)
            ";

            await _db.ExecuteAsync(
                sql,
                new { ContentId = ToRaw(contentId) },
                transaction: Tx
            );
        }

        public async Task DeleteContentAsync(Guid contentId)
        {
            const string sql = @"
                DELETE FROM Contents
                WHERE ContentId = HEXTORAW(:ContentId)
            ";

            await _db.ExecuteAsync(
                sql,
                new { ContentId = ToRaw(contentId) },
                transaction: Tx
            );
        }
    }
}
