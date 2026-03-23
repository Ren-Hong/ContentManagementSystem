using Cms.Contract.Repositories.Department.Entities;
using Cms.Contract.Repositories.Department.Interfaces;
using Cms.Contract.Services.UnitOfWork.Interfaces;
using Cms.Infrastructure.Repositories.Base;
using Dapper;
using System.Data;

namespace Cms.Infrastructure.Repositories.Department
{
    public class DepartmentRepository : BaseRepository, IDepartmentRepository
    {
        public DepartmentRepository(
            IDbConnection db,
            IUnitOfWork unitOfWork
        ) : base(db, unitOfWork)
        {
        }

        public async Task<bool> AllDepartmentsExistAsync(List<Guid> departmentIds)
        {
            if (departmentIds.Count == 0)
            {
                return true;
            }

            var parameters = new DynamicParameters();
            var inClause = BuildRawInClause(parameters, "DepartmentId", departmentIds);
            var sql = $@"
                SELECT COUNT(DepartmentId)
                FROM Departments
                WHERE DepartmentId IN {inClause}
            ";

            var count = await _db.ExecuteScalarAsync<int>(
                sql,
                parameters,
                transaction: Tx
            );

            return count == departmentIds.Count;
        }

        public async Task<IEnumerable<DepartmentLookupEntity>> ListDepartmentLookupsAsync()
        {
            const string sql = @"
                SELECT
                    RAWTOHEX(DepartmentId) AS DepartmentId,
                    DepartmentName
                FROM Departments
                WHERE Status = 1
                ORDER BY DepartmentName
            ";

            return await _db.QueryAsync<DepartmentLookupEntity>(
                sql,
                transaction: Tx
            );
        }

        public async Task<IEnumerable<AccessibleDepartmentEntity>> ListAccessibleDepartmentsAsync()
        {
            const string sql = @"
                SELECT
                    RAWTOHEX(DepartmentId) AS DepartmentId,
                    DepartmentCode,
                    DepartmentName
                FROM Departments
                WHERE Status = 1
                ORDER BY DepartmentCode
            ";

            return await _db.QueryAsync<AccessibleDepartmentEntity>(
                sql,
                transaction: Tx
            );
        }

        public async Task<IEnumerable<AccessibleDepartmentEntity>> ListAccessibleDepartmentsAsync(IEnumerable<Guid> departmentIds)
        {
            var ids = departmentIds.ToList();
            if (ids.Count == 0)
            {
                return Array.Empty<AccessibleDepartmentEntity>();
            }

            var parameters = new DynamicParameters();
            var inClause = BuildRawInClause(parameters, "DepartmentId", ids);
            var sql = $@"
                SELECT
                    RAWTOHEX(DepartmentId) AS DepartmentId,
                    DepartmentCode,
                    DepartmentName
                FROM Departments
                WHERE Status = 1
                    AND DepartmentId IN {inClause}
                ORDER BY DepartmentCode
            ";

            return await _db.QueryAsync<AccessibleDepartmentEntity>(
                sql,
                parameters,
                transaction: Tx
            );
        }
    }
}
