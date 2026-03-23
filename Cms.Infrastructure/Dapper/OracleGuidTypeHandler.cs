using System.Data;
using System.Globalization;
using System.Threading;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace Cms.Infrastructure.Dapper
{
    internal sealed class OracleGuidTypeHandler : SqlMapper.TypeHandler<Guid>
    {
        public override void SetValue(IDbDataParameter parameter, Guid value)
        {
            parameter.DbType = DbType.Binary;
            parameter.Value = value.ToByteArray();

            if (parameter is OracleParameter oracleParameter)
            {
                oracleParameter.OracleDbType = OracleDbType.Raw;
                oracleParameter.Size = 16;
            }
        }

        public override Guid Parse(object value)
        {
            return value switch
            {
                Guid guid => guid,
                byte[] bytes when bytes.Length == 16 => new Guid(bytes),
                OracleBinary oracleBinary when !oracleBinary.IsNull => new Guid(oracleBinary.Value),
                string text when Guid.TryParse(text, out var guid) => guid,
                _ => throw new DataException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Cannot convert value of type '{0}' to Guid.",
                        value.GetType().FullName
                    )
                )
            };
        }
    }

    internal sealed class OracleNullableGuidTypeHandler : SqlMapper.TypeHandler<Guid?>
    {
        private static readonly OracleGuidTypeHandler InnerHandler = new();

        public override void SetValue(IDbDataParameter parameter, Guid? value)
        {
            if (value.HasValue)
            {
                InnerHandler.SetValue(parameter, value.Value);
                return;
            }

            parameter.Value = DBNull.Value;

            if (parameter is OracleParameter oracleParameter)
            {
                oracleParameter.OracleDbType = OracleDbType.Raw;
                oracleParameter.Size = 16;
            }
        }

        public override Guid? Parse(object value)
        {
            if (value is null or DBNull)
            {
                return null;
            }

            return InnerHandler.Parse(value);
        }
    }

    internal sealed class OracleBooleanTypeHandler : SqlMapper.TypeHandler<bool>
    {
        public override void SetValue(IDbDataParameter parameter, bool value)
        {
            parameter.DbType = DbType.Int16;
            parameter.Value = value ? 1 : 0;
        }

        public override bool Parse(object value)
        {
            return value switch
            {
                bool boolValue => boolValue,
                byte byteValue => byteValue != 0,
                short shortValue => shortValue != 0,
                int intValue => intValue != 0,
                long longValue => longValue != 0,
                decimal decimalValue => decimalValue != 0,
                string text when bool.TryParse(text, out var boolValue) => boolValue,
                string text when int.TryParse(text, out var intValue) => intValue != 0,
                _ => throw new DataException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Cannot convert value of type '{0}' to bool.",
                        value.GetType().FullName
                    )
                )
            };
        }
    }

    internal sealed class OracleNullableBooleanTypeHandler : SqlMapper.TypeHandler<bool?>
    {
        private static readonly OracleBooleanTypeHandler InnerHandler = new();

        public override void SetValue(IDbDataParameter parameter, bool? value)
        {
            if (value.HasValue)
            {
                InnerHandler.SetValue(parameter, value.Value);
                return;
            }

            parameter.Value = DBNull.Value;
        }

        public override bool? Parse(object value)
        {
            if (value is null or DBNull)
            {
                return null;
            }

            return InnerHandler.Parse(value);
        }
    }

    public static class OracleDapperBootstrap
    {
        private static int _initialized;

        public static void Initialize()
        {
            if (Interlocked.Exchange(ref _initialized, 1) == 1)
            {
                return;
            }

            SqlMapper.AddTypeHandler(new OracleGuidTypeHandler());
            SqlMapper.AddTypeHandler(new OracleNullableGuidTypeHandler());
            SqlMapper.AddTypeHandler(new OracleBooleanTypeHandler());
            SqlMapper.AddTypeHandler(new OracleNullableBooleanTypeHandler());
        }
    }
}
