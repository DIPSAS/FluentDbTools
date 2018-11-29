using System.Collections.Generic;
using System.Data;
using DIPS.FluentDbTools.Common.Abstractions;
using DIPS.FluentDbTools.SqlBuilder.Abstractions.Parameters;

namespace DIPS.FluentDbTools.SqlBuilder.DbTypeTranslators
{
    internal class KnownPostgresDbTypeTranslator : IDbTypeTranslator
    {
        public SupportedDatabaseTypes DatabaseType { get; } = SupportedDatabaseTypes.Postgres;

        public Dictionary<DbType, string> DbTypeMappings { get; } = new Dictionary<DbType, string>
        {
            { DbType.Boolean, "boolean" },
            { DbType.Int16, "smallint" },
            { DbType.Int32, "integer" },
            { DbType.Int64, "bigint" },
            { DbType.Single, "real" },
            { DbType.Double, "double precision" },
            { DbType.Decimal, "numeric" },
            { DbType.VarNumeric, "numeric" },
            { DbType.Currency, "money" },
            { DbType.String, "text" },
            { DbType.StringFixedLength, "text" },
            { DbType.AnsiString, "text" },
            { DbType.AnsiStringFixedLength, "text" },
            { DbType.Date, "date" },
            { DbType.DateTime, "timestamp" },
            { DbType.DateTime2, "timestamp" },
            { DbType.DateTimeOffset, "timestamp with time zone" },
            { DbType.Time, "time" },
            { DbType.Binary, "bytea" },
            { DbType.Guid, "uuid" },
            { DbType.Byte, "smallint" },
            { DbType.SByte, "smallint" },
            { DbType.Xml, "bytea" },
            { DbType.UInt16, "smallint" },
            { DbType.UInt32, "integer" },
            { DbType.UInt64, "bigint" },
        };
    }
}