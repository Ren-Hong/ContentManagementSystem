using Cms.Contract.Repositories.Scope.Entities;
using Cms.Contract.Repositories.Scope.Interfaces;
using Cms.Contract.Services.UnitOfWork.Interfaces;
using Cms.Infrastructure.Repositories.Base;
using Dapper;
using System.Data;

namespace Cms.Infrastructure.Repositories.Scope
{
    public class ScopeRepository : BaseRepository, IScopeRepository
    {
        public ScopeRepository(
            IDbConnection db,
            IUnitOfWork unitOfWork
        ) : base(db, unitOfWork)
        {
        }

        public async Task<IEnumerable<ScopeLookupEntity>> ListScopeLookupsAsync(bool includeAssigned = false)
        {
            const string sql = @"
                SELECT
                    RAWTOHEX(ScopeId) AS ScopeId,
                    ScopeCode,
                    ScopeName
                FROM Scopes
                WHERE (:IncludeAssigned = 1 OR ScopeCode <> 'Assigned')
                ORDER BY ScopeName
            ";

            return await _db.QueryAsync<ScopeLookupEntity>(
                sql,
                new { IncludeAssigned = includeAssigned ? 1 : 0 },
                transaction: Tx
            );
        }

        public async Task<bool> AllScopesExistAsync(List<Guid> scopeIds)
        {
            if (scopeIds.Count == 0)
            {
                return true;
            }

            var parameters = new DynamicParameters();
            var inClause = BuildRawInClause(parameters, "ScopeId", scopeIds);
            var sql = $@"
                SELECT COUNT(ScopeId)
                FROM Scopes
                WHERE ScopeId IN {inClause}
            ";

            var count = await _db.ExecuteScalarAsync<int>(
                sql,
                parameters,
                transaction: Tx
            );

            return count == scopeIds.Count;
        }
    }
}
