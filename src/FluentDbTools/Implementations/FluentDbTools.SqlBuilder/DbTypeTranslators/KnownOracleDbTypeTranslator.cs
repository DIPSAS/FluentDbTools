using System.Collections.Generic;
using System.Data;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.SqlBuilder.Abstractions.Parameters;

namespace FluentDbTools.SqlBuilder.DbTypeTranslators
{
    internal class KnownOracleDbTypeTranslator : IDbTypeTranslator
    {
        public SupportedDatabaseTypes DatabaseType => SupportedDatabaseTypes.Oracle;

        public Dictionary<DbType, string> DbTypeMappings { get; } = new Dictionary<DbType, string>
        {
            { DbType.Boolean, "NUMBER(1,0)"},
            { DbType.Int16, "INTEGER" },
            { DbType.Int32, "INTEGER" },
            { DbType.Int64, "INTEGER" },
            { DbType.Single, "FLOAT" },
            { DbType.Double, "FLOAT" },
            { DbType.Decimal, "FLOAT" },
            { DbType.VarNumeric, "NUMBER" },
            { DbType.Currency, "NUMBER" },
            { DbType.String, "NVARCHAR2" },
            { DbType.StringFixedLength, "NCHAR" },
            { DbType.AnsiString, "VARCHAR2" },
            { DbType.AnsiStringFixedLength, "CHAR" },
            { DbType.Date, "DATE" },
            { DbType.DateTime, "DATE" },
            { DbType.DateTime2, "TIMESTAMP" },
            { DbType.DateTimeOffset, "TIMESTAMP" },
            { DbType.Time, "DATE" },
            { DbType.Binary, "LONG RAW" },
            { DbType.Guid, "RAW(16)" },
            { DbType.Byte, "UNSIGNED INTEGER" },
            { DbType.SByte, "INTEGER" },
            { DbType.Object, "LONG RAW" },
            { DbType.Xml, "RAW" },
            { DbType.UInt16, "UNSIGNED INTEGER" },
            { DbType.UInt32, "UNSIGNED INTEGER" },
            { DbType.UInt64, "UNSIGNED INTEGER" },
        };
    };
}