using System;
using System.Runtime.CompilerServices;
using DIPS.FluentDbTools.Common.Abstractions;

[assembly: InternalsVisibleTo("DIPS.FluentDbTools.Migration")]
[assembly: InternalsVisibleTo("DIPS.FluentDbTools.Migration.Oracle")]
[assembly: InternalsVisibleTo("DIPS.FluentDbTools.Migration.Postgres")]
namespace DIPS.FluentDbTools.Migration.Common
{
    internal static class ProcessorIds
    {
        public const string PostgresProcessorId = "PostgresExtended";
        public const string OracleProcessorId = "OracleManagedExtended";

        public static string GetProcessorId(this SupportedDatabaseTypes databaseType)
        {
            switch (databaseType)
            {
                case SupportedDatabaseTypes.Postgres:
                    return PostgresProcessorId;
                case SupportedDatabaseTypes.Oracle:
                    return OracleProcessorId;
                default:
                    throw new ArgumentOutOfRangeException($"Following db type is not implemented: {databaseType.ToString()}", databaseType, null);
            }
        }
    }
}