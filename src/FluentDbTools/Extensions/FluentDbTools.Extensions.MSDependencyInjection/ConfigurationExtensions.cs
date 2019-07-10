using FluentDbTools.Common.Abstractions;
using Microsoft.Extensions.Configuration;

namespace FluentDbTools.Extensions.MSDependencyInjection
{
    public static class ConfigurationExtensions
    {
        public static string GetConfigValue(this IConfiguration configuration, params string[] keys)
        {
            foreach (var key in keys)
            {
                var str = configuration[key];
                if (!str.IsEmpty())
                    return str;
            }
            return null;
        }

        public static bool IsConsoleLogEnabled(this IConfiguration configuration)
        {
            return configuration.GetConfigValue("Logging:Console:LogLevel", "Logging:LogLevel:Console").WithDefault("Information") != "None";
        }

        public static bool IsMigrationLogShowSqlEnabled(this IConfiguration configuration)
        {
            return configuration.GetConfigValue("Logging:Migration:ShowSql").WithDefault("false") != "false";
        }

        public static string GetMigrationLogFile(this IConfiguration configuration)
        {
            return configuration.GetConfigValue("Logging:Migration:File");
        }

        public static bool IsMigrationLogFileEnabled(this IConfiguration configuration)
        {
            return !configuration.GetMigrationLogFile().IsEmpty();
        }

        public static bool IsMigrationLogShowElapsedTimeEnabled(this IConfiguration configuration)
        {
            return configuration.GetConfigValue("Logging:Migration:ShowElapsedTime").WithDefault("false") != "false";
        }
    }
}