using Cms.Contract.Common.Pagination;
using Cms.Contract.Repositories.Account.Entities;
using Cms.Contract.Repositories.Account.Interfaces;
using Cms.Contract.Repositories.Account.Persistence;
using Cms.Contract.Services.UnitOfWork.Interfaces;
using Cms.Infrastructure.Repositories.Base;
using Dapper;
using System.Data;

namespace Cms.Infrastructure.Repositories.Account
{
    public class AccountRepository : BaseRepository, IAccountRepository
    {
        public AccountRepository(
            IDbConnection db,
            IUnitOfWork unitOfWork
        ) : base(db, unitOfWork)
        {
        }

        public async Task<IEnumerable<AccountAuthEntity>> GetAccountAuthByUsernameAsync(string username)
        {
            const string sql = @"
                SELECT
                    RAWTOHEX(a.AccountId) AS AccountId,
                    a.Username,
                    a.PasswordHash,
                    r.RoleCode,
                    p.PermissionCode
                FROM Accounts a
                LEFT JOIN AccountRoleAssignments ar
                    ON a.AccountId = ar.AccountId
                LEFT JOIN Roles r
                    ON ar.RoleId = r.RoleId
                    AND r.Status = 1
                LEFT JOIN RolePermissions rp
                    ON r.RoleId = rp.RoleId
                LEFT JOIN Permissions p
                    ON rp.PermissionId = p.PermissionId
                    AND p.Status = 1
                WHERE a.Username = :Username
                    AND a.Status = 1
            ";

            return await _db.QueryAsync<AccountAuthEntity>(
                sql,
                new { Username = username }
            );
        }

        public async Task<bool> HasGlobalPermissionScopeAsync(Guid accountId, string permissionCode)
        {
            const string sql = @"
                SELECT 1
                FROM AccountRoleAssignments ara
                INNER JOIN Roles r
                    ON r.RoleId = ara.RoleId
                    AND r.Status = 1
                INNER JOIN RolePermissions rp
                    ON rp.RoleId = r.RoleId
                INNER JOIN Permissions p
                    ON p.PermissionId = rp.PermissionId
                    AND p.Status = 1
                INNER JOIN Scopes s
                    ON s.ScopeId = rp.ScopeId
                WHERE ara.AccountId = HEXTORAW(:AccountId)
                    AND p.PermissionCode = :PermissionCode
                    AND s.ScopeCode = :ScopeCode
                FETCH FIRST 1 ROWS ONLY
            ";

            var result = await _db.ExecuteScalarAsync<int?>(
                sql,
                new
                {
                    AccountId = ToRaw(accountId),
                    PermissionCode = permissionCode,
                    ScopeCode = "Global"
                },
                transaction: Tx
            );

            return result.HasValue;
        }

        public async Task<List<Guid>> GetDepartmentIdsByPermissionScopeAsync(Guid accountId, string permissionCode)
        {
            const string sql = @"
                SELECT DISTINCT RAWTOHEX(ara.DepartmentId) AS DepartmentId
                FROM AccountRoleAssignments ara
                INNER JOIN Roles r
                    ON r.RoleId = ara.RoleId
                    AND r.Status = 1
                INNER JOIN RolePermissions rp
                    ON rp.RoleId = r.RoleId
                INNER JOIN Permissions p
                    ON p.PermissionId = rp.PermissionId
                    AND p.Status = 1
                INNER JOIN Scopes s
                    ON s.ScopeId = rp.ScopeId
                WHERE ara.AccountId = HEXTORAW(:AccountId)
                    AND p.PermissionCode = :PermissionCode
                    AND s.ScopeCode = :ScopeCode
            ";

            var rows = await _db.QueryAsync<string>(
                sql,
                new
                {
                    AccountId = ToRaw(accountId),
                    PermissionCode = permissionCode,
                    ScopeCode = "Department"
                },
                transaction: Tx
            );

            return rows.Select(Guid.Parse).ToList();
        }

        public async Task<PagedResult<AccountRoleAssignmentRowEntity>> PageAccountRoleAssignmentRowsAsync(PageRequest pageRequest)
        {
            var offset = (pageRequest.Page - 1) * pageRequest.PageSize;

            const string countSql = @"
                SELECT COUNT(DISTINCT a.AccountId)
                FROM Accounts a
            ";

            const string dataSql = @"
                WITH PagedAccounts AS (
                    SELECT AccountId
                    FROM Accounts
                    ORDER BY Username
                    OFFSET :Offset ROWS FETCH NEXT :PageSize ROWS ONLY
                )
                SELECT
                    RAWTOHEX(a.AccountId) AS AccountId,
                    a.Username,
                    RAWTOHEX(d.DepartmentId) AS DepartmentId,
                    d.DepartmentName,
                    RAWTOHEX(r.RoleId) AS RoleId,
                    r.RoleName,
                    a.Status
                FROM Accounts a
                JOIN PagedAccounts pa ON a.AccountId = pa.AccountId
                LEFT JOIN AccountRoleAssignments ar ON a.AccountId = ar.AccountId
                LEFT JOIN Departments d ON ar.DepartmentId = d.DepartmentId
                LEFT JOIN Roles r ON ar.RoleId = r.RoleId
                ORDER BY a.Username
            ";

            var totalCount = await _db.ExecuteScalarAsync<int>(countSql);

            var items = (await _db.QueryAsync<AccountRoleAssignmentRowEntity>(
                dataSql,
                new { Offset = offset, PageSize = pageRequest.PageSize }
            )).ToList();

            return new PagedResult<AccountRoleAssignmentRowEntity>
            {
                Page = pageRequest.Page,
                PageSize = pageRequest.PageSize,
                TotalCount = totalCount,
                Items = items
            };
        }

        public async Task<bool> AccountExistsAsync(string username)
        {
            const string sql = @"
                SELECT 1
                FROM Accounts
                WHERE Username = :Username
            ";

            var result = await _db.ExecuteScalarAsync<int?>(
                sql,
                new { Username = username }
            );

            return result != null;
        }

        public async Task<Guid> CreateAccountAsync(string username, string passwordHash)
        {
            var accountId = Guid.NewGuid();

            const string sql = @"
                INSERT INTO Accounts
                (
                    AccountId,
                    Username,
                    PasswordHash,
                    Status
                )
                VALUES
                (
                    HEXTORAW(:AccountId),
                    :Username,
                    :PasswordHash,
                    :Status
                )
            ";

            await _db.ExecuteAsync(
                sql,
                new
                {
                    AccountId = ToRaw(accountId),
                    Username = username,
                    PasswordHash = passwordHash,
                    Status = 1
                },
                transaction: Tx
            );

            return accountId;
        }

        public async Task CreateAccountRoleAssignmentAsync(Guid accountId, Guid roleId, Guid departmentId)
        {
            const string sql = @"
                INSERT INTO AccountRoleAssignments
                (
                    AccountId,
                    RoleId,
                    DepartmentId
                )
                VALUES
                (
                    HEXTORAW(:AccountId),
                    HEXTORAW(:RoleId),
                    HEXTORAW(:DepartmentId)
                )
            ";

            await _db.ExecuteAsync(
                sql,
                new
                {
                    AccountId = ToRaw(accountId),
                    RoleId = ToRaw(roleId),
                    DepartmentId = ToRaw(departmentId)
                },
                transaction: Tx
            );
        }

        public async Task CreateAccountRoleAssignmentsAsync(IEnumerable<object> rows)
        {
            const string sql = @"
                INSERT INTO AccountRoleAssignments
                (
                    AccountId,
                    RoleId,
                    DepartmentId
                )
                VALUES
                (
                    HEXTORAW(:AccountId),
                    HEXTORAW(:RoleId),
                    HEXTORAW(:DepartmentId)
                )
            ";

            var rawRows = rows
                .Select(row => new
                {
                    AccountId = ToRaw(ReadGuidProperty(row, "AccountId")),
                    RoleId = ToRaw(ReadGuidProperty(row, "RoleId")),
                    DepartmentId = ToRaw(ReadGuidProperty(row, "DepartmentId"))
                })
                .Distinct()
                .ToList();

            if (rawRows.Count == 0)
            {
                return;
            }

            await _db.ExecuteAsync(sql, rawRows, transaction: Tx);
        }

        public async Task UpdateLastLoginAtAsync(Guid accountId, DateTime loginTime)
        {
            const string sql = @"
                UPDATE Accounts
                SET LastLoginAt = :LastLoginAt
                WHERE AccountId = HEXTORAW(:AccountId)
            ";

            await _db.ExecuteAsync(
                sql,
                new
                {
                    AccountId = ToRaw(accountId),
                    LastLoginAt = loginTime
                }
            );
        }

        public async Task<Guid> UpdateAccountStatusAsync(string username, AccountStatus status)
        {
            const string getAccountIdSql = @"
                SELECT RAWTOHEX(AccountId) AS AccountId
                FROM Accounts
                WHERE Username = :Username
            ";

            const string updateSql = @"
                UPDATE Accounts
                SET
                    Status = :Status,
                    UpdatedAt = :UpdatedAt
                WHERE Username = :Username
            ";

            var accountIdHex = await _db.ExecuteScalarAsync<string?>(
                getAccountIdSql,
                new
                {
                    Username = username
                },
                Tx
            );

            if (string.IsNullOrWhiteSpace(accountIdHex))
            {
                throw new InvalidOperationException($"Account '{username}' was not found.");
            }

            await _db.ExecuteAsync(
                updateSql,
                new
                {
                    Username = username,
                    Status = status,
                    UpdatedAt = DateTime.UtcNow
                },
                Tx
            );

            return Guid.Parse(accountIdHex);
        }

        public async Task UpdatePasswordAsync(string username, string passwordHash)
        {
            const string sql = @"
                UPDATE Accounts
                SET PasswordHash = :PasswordHash
                WHERE Username = :Username
            ";

            await _db.ExecuteAsync(
                sql,
                new
                {
                    Username = username,
                    PasswordHash = passwordHash
                }
            );
        }

        public async Task UpdateUpdatedAtAsync(string username, DateTime updatedTime)
        {
            const string sql = @"
                UPDATE Accounts
                SET UpdatedAt = :UpdatedAt
                WHERE Username = :Username
            ";

            await _db.ExecuteAsync(
                sql,
                new
                {
                    Username = username,
                    UpdatedAt = updatedTime
                }
            );
        }

        public async Task DeleteAccountAsync(string username)
        {
            const string sql = @"
                DELETE FROM Accounts
                WHERE Username = :Username
            ";

            await _db.ExecuteAsync(
                sql,
                new
                {
                    Username = username
                }
            );
        }

        public async Task DeleteAccountRoleAssignmentsAsync(Guid accountId)
        {
            const string sql = @"
                DELETE FROM AccountRoleAssignments
                WHERE AccountId = HEXTORAW(:AccountId)
            ";

            await _db.ExecuteAsync(
                sql,
                new { AccountId = ToRaw(accountId) },
                transaction: Tx
            );
        }

        public async Task AddAccountRoleAssignmentAsync(Guid accountId, Guid roleId, Guid departmentId)
        {
            const string sql = @"
                INSERT INTO AccountRoleAssignments
                (
                    AccountId,
                    RoleId,
                    DepartmentId
                )
                VALUES
                (
                    HEXTORAW(:AccountId),
                    HEXTORAW(:RoleId),
                    HEXTORAW(:DepartmentId)
                )
            ";

            await _db.ExecuteAsync(
                sql,
                new
                {
                    AccountId = ToRaw(accountId),
                    RoleId = ToRaw(roleId),
                    DepartmentId = ToRaw(departmentId)
                },
                transaction: Tx
            );
        }

        private static Guid ReadGuidProperty(object row, string propertyName)
        {
            ArgumentNullException.ThrowIfNull(row);

            var property = row.GetType().GetProperty(propertyName);
            if (property is null)
            {
                throw new InvalidOperationException(
                    $"Account role assignment row is missing property '{propertyName}'."
                );
            }

            var value = property.GetValue(row);
            if (value is Guid guid)
            {
                return guid;
            }

            if (value is string text && Guid.TryParse(text, out var parsedGuid))
            {
                return parsedGuid;
            }

            throw new InvalidOperationException(
                $"Account role assignment property '{propertyName}' is not a valid Guid."
            );
        }
    }
}
