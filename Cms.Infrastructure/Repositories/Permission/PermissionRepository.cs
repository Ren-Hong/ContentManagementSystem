using Cms.Contract.Repositories.Permission.Entities;
using Cms.Contract.Repositories.Permission.Interfaces;
using Cms.Contract.Services.UnitOfWork.Interfaces;
using Cms.Infrastructure.Repositories.Base;
using Dapper;
using System.Data;

namespace Cms.Infrastructure.Repositories.Permission
{
    public class PermissionRepository : BaseRepository, IPermissionRepository
    {
        public PermissionRepository(
            IDbConnection db,
            IUnitOfWork unitOfWork
        ) : base(db, unitOfWork)
        {
        }

        public async Task<bool> AllPermissionsExistAsync(List<Guid> permissionIds)
        {
            if (permissionIds.Count == 0)
            {
                return true;
            }

            var parameters = new DynamicParameters();
            var inClause = BuildRawInClause(parameters, "PermissionId", permissionIds);
            var sql = $@"
                SELECT COUNT(PermissionId)
                FROM Permissions
                WHERE PermissionId IN {inClause}
            ";

            var count = await _db.ExecuteScalarAsync<int>(
                sql,
                parameters,
                transaction: Tx
            );

            return count == permissionIds.Count;
        }

        public async Task<IEnumerable<PermissionLookupEntity>> ListPermissionLookupsAsync()
        {
            const string sql = @"
                SELECT
                    RAWTOHEX(PermissionId) AS PermissionId,
                    PermissionName
                FROM Permissions
                WHERE Status = 1
                ORDER BY PermissionName
            ";

            return await _db.QueryAsync<PermissionLookupEntity>(
                sql,
                transaction: Tx
            );
        }
    }
}
