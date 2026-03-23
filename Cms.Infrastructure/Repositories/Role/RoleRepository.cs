using Cms.Contract.Common.Pagination;
using Cms.Contract.Repositories.Role.Entities;
using Cms.Contract.Repositories.Role.Interfaces;
using Cms.Contract.Repositories.Role.Persistence;
using Cms.Contract.Services.UnitOfWork.Interfaces;
using Cms.Infrastructure.Repositories.Base;
using Dapper;
using System.Data;

namespace Cms.Infrastructure.Repositories.Role
{
    public class RoleRepository : BaseRepository, IRoleRepository
    {
        public RoleRepository(
            IDbConnection db,
            IUnitOfWork unitOfWork
        ) : base(db, unitOfWork)
        {
        }

        public async Task<bool> AllRolesExistAsync(List<Guid> roleIds)
        {
            if (roleIds.Count == 0)
            {
                return true;
            }

            var parameters = new DynamicParameters();
            var inClause = BuildRawInClause(parameters, "RoleId", roleIds);
            var sql = $@"
                SELECT COUNT(RoleId)
                FROM Roles
                WHERE RoleId IN {inClause}
            ";

            var count = await _db.ExecuteScalarAsync<int>(
                sql,
                parameters,
                transaction: Tx
            );

            return count == roleIds.Count;
        }

        public async Task<IEnumerable<RoleLookupEntity>> ListRoleLookupsAsync()
        {
            const string sql = @"
                SELECT
                    RAWTOHEX(RoleId) AS RoleId,
                    RoleName
                FROM Roles
                WHERE Status = 1
                ORDER BY RoleName
            ";

            return await _db.QueryAsync<RoleLookupEntity>(
                sql,
                transaction: Tx
            );
        }

        public async Task<PagedResult<RolePermissionScopeRowEntity>> ListRolePermissionScopeRowsAsync(PageRequest pageRequest)
        {
            var page = pageRequest.Page <= 0 ? 1 : pageRequest.Page;
            var pageSize = pageRequest.PageSize <= 0 ? 10 : pageRequest.PageSize;
            var offset = (page - 1) * pageSize;

            const string countSql = @"
                SELECT COUNT(*)
                FROM Roles
            ";

            const string dataSql = @"
                WITH PagedRoles AS (
                    SELECT RoleId
                    FROM Roles
                    ORDER BY RoleName
                    OFFSET :Offset ROWS FETCH NEXT :PageSize ROWS ONLY
                )
                SELECT
                    RAWTOHEX(r.RoleId) AS RoleId,
                    r.RoleName,
                    RAWTOHEX(p.PermissionId) AS PermissionId,
                    p.PermissionName,
                    RAWTOHEX(s.ScopeId) AS ScopeId,
                    s.ScopeCode,
                    s.ScopeName,
                    r.Status
                FROM Roles r
                JOIN PagedRoles pr ON r.RoleId = pr.RoleId
                LEFT JOIN RolePermissions rp ON r.RoleId = rp.RoleId
                LEFT JOIN Permissions p ON rp.PermissionId = p.PermissionId
                LEFT JOIN Scopes s ON rp.ScopeId = s.ScopeId
                ORDER BY r.RoleName, p.PermissionName
            ";

            var totalCount = await _db.ExecuteScalarAsync<int>(countSql);

            var items = (await _db.QueryAsync<RolePermissionScopeRowEntity>(
                dataSql,
                new
                {
                    Offset = offset,
                    PageSize = pageSize
                }
            )).ToList();

            return new PagedResult<RolePermissionScopeRowEntity>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = items
            };
        }

        public async Task<bool> RoleNameExistsAsync(string roleName)
        {
            const string sql = @"
                SELECT 1
                FROM Roles
                WHERE RoleName = :RoleName
            ";

            var result = await _db.ExecuteScalarAsync<int?>(
                sql,
                new { RoleName = roleName }
            );

            return result != null;
        }

        public async Task<Guid> CreateRoleAsync(string roleName, string roleCode)
        {
            var roleId = Guid.NewGuid();

            const string sql = @"
                INSERT INTO Roles
                (
                    RoleId,
                    RoleName,
                    RoleCode,
                    Status
                )
                VALUES
                (
                    HEXTORAW(:RoleId),
                    :RoleName,
                    :RoleCode,
                    :Status
                )
            ";

            await _db.ExecuteAsync(
                sql,
                new
                {
                    RoleId = ToRaw(roleId),
                    RoleName = roleName,
                    RoleCode = roleCode,
                    Status = 1
                },
                transaction: Tx
            );

            return roleId;
        }

        public async Task CreateRolePermissionAsync(Guid roleId, Guid permissionId)
        {
            throw new NotSupportedException(
                "RolePermissions requires ScopeId under the current Oracle schema. Use CreateRolePermissionScopeAsync instead."
            );
        }

        public async Task CreateRolePermissionScopeAsync(Guid roleId, Guid permissionId, Guid scopeId)
        {
            const string sql = @"
                INSERT INTO RolePermissions
                (
                    RoleId,
                    PermissionId,
                    ScopeId
                )
                VALUES
                (
                    HEXTORAW(:RoleId),
                    HEXTORAW(:PermissionId),
                    HEXTORAW(:ScopeId)
                )
            ";

            await _db.ExecuteAsync(
                sql,
                new
                {
                    RoleId = ToRaw(roleId),
                    PermissionId = ToRaw(permissionId),
                    ScopeId = ToRaw(scopeId)
                },
                transaction: Tx
            );
        }

        public async Task<Guid> UpdateRoleStatusAsync(string roleName, RoleStatus status)
        {
            const string getRoleIdSql = @"
                SELECT RAWTOHEX(RoleId) AS RoleId
                FROM Roles
                WHERE RoleName = :RoleName
            ";

            const string updateSql = @"
                UPDATE Roles
                SET
                    Status = :Status,
                    UpdatedAt = :UpdatedAt
                WHERE RoleName = :RoleName
            ";

            var roleIdHex = await _db.ExecuteScalarAsync<string?>(
                getRoleIdSql,
                new
                {
                    RoleName = roleName
                },
                Tx
            );

            if (string.IsNullOrWhiteSpace(roleIdHex))
            {
                throw new InvalidOperationException($"Role '{roleName}' was not found.");
            }

            await _db.ExecuteAsync(
                updateSql,
                new
                {
                    RoleName = roleName,
                    Status = status,
                    UpdatedAt = DateTime.UtcNow
                },
                Tx
            );

            return Guid.Parse(roleIdHex);
        }

        public async Task DeleteRoleAsync(string roleName)
        {
            const string sql = @"
                DELETE FROM Roles
                WHERE RoleName = :RoleName
            ";

            await _db.ExecuteAsync(
                sql,
                new
                {
                    RoleName = roleName
                }
            );
        }

        public async Task DeleteRolePermissionsAsync(Guid roleId)
        {
            const string sql = @"
                DELETE FROM RolePermissions
                WHERE RoleId = HEXTORAW(:RoleId)
            ";

            await _db.ExecuteAsync(
                sql,
                new { RoleId = ToRaw(roleId) },
                transaction: Tx
            );
        }
    }
}
