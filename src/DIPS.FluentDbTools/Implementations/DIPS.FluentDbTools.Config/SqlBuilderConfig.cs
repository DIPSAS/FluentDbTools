using System;
using DIPS.FluentDbTools.Common.Abstractions;
using Microsoft.Extensions.Configuration;

namespace DIPS.FluentDbTools.Config
{
    public static class SqlBuilderConfig
    {
        public static string GetDbParameterPrefix(this IDbConfig configuration)
        {
            var databaseType = configuration.DbType;
            switch (databaseType)
            {
                case SupportedDatabaseTypes.Undefined:
                case SupportedDatabaseTypes.Postgres:
                    return "@";
                case SupportedDatabaseTypes.Oracle:
                    return ":";
                default:
                    throw new ArgumentOutOfRangeException($"Unknown prefix for {databaseType}");
            }
        }
    }
}