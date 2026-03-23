using Cms.Contract.Services.UnitOfWork.Interfaces;
using Dapper;
using System.Data;
using System.Globalization;

namespace Cms.Infrastructure.Repositories.Base
{
    public abstract class BaseRepository
    {
        protected readonly IDbConnection _db;
        protected readonly IUnitOfWork _unitOfWork;

        protected BaseRepository(
            IDbConnection db,
            IUnitOfWork unitOfWork
        )
        {
            _db = db;
            _unitOfWork = unitOfWork;
        }

        protected IDbTransaction? Tx => _unitOfWork.Transaction; // 如果Service有跑BeginTransaction 這個屬性就會被附值

        protected static string ToRaw(Guid value)
        {
            return value.ToString("N", CultureInfo.InvariantCulture).ToUpperInvariant();
        }

        protected static string? ToRaw(Guid? value)
        {
            return value.HasValue ? ToRaw(value.Value) : null;
        }

        protected static string BuildRawInClause(
            DynamicParameters parameters,
            string parameterBaseName,
            IEnumerable<Guid> values
        )
        {
            ArgumentNullException.ThrowIfNull(parameters);
            ArgumentNullException.ThrowIfNull(values);

            var placeholders = new List<string>();
            var index = 0;

            foreach (var value in values)
            {
                var parameterName = $"{parameterBaseName}{index++}";
                parameters.Add(parameterName, ToRaw(value));
                placeholders.Add($"HEXTORAW(:{parameterName})");
            }

            return placeholders.Count == 0
                ? "(SELECT CAST(NULL AS RAW(16)) FROM DUAL WHERE 1 = 0)"
                : $"({string.Join(", ", placeholders)})";
        }
    }
}
