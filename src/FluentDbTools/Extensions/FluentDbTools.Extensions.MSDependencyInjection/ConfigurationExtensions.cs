using System;
using FluentDbTools.Common.Abstractions;
using Microsoft.Extensions.Configuration;

namespace FluentDbTools.Extensions.MSDependencyInjection
{
    public static class ConfigurationExtensions
    {
        public static string GetConfigValue(this IConfiguration configuration, params string[] keys)
        {
            if (configuration == null)
            {
                return null;
            }

            foreach (var key in keys)
            {
                var value = configuration[key];
                if (!value.IsEmpty())
                {
                    return value;
                }
            }

            var sections = new[] { "Logging", "database:migration", "database" };

            foreach (var section in sections)
            {
                var dbSection = configuration.GetSection(section);
                if (dbSection == null)
                {
                    continue;
                }

                foreach (var key in keys)
                {
                    var value = dbSection[key];
                    if (!value.IsEmpty())
                    {
                        return value;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// MigrationLogging with Console logging can be enabled from configuration key(s):
        ///     ConsoleLog = true
        ///     OR
        ///     [Logging:Console:LogLevel | Logging:LogLevel:Console] != None
        ///     When both Logging:Console:LogLevel and Logging:LogLevel:Console is null, Console logging will be enabled.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static bool IsConsoleLogEnabled(this IConfiguration configuration)
        {
            var consoleLogEnabled = configuration.GetConfigValue("ConsoleLog")
                .WithDefault(false.ToString().ToLower())
                .Equals(true.ToString(), StringComparison.CurrentCultureIgnoreCase);

            return consoleLogEnabled || !configuration
                                            .GetConfigValue("Logging:Console:LogLevel", "Logging:LogLevel:Console")
                                            .WithDefault("Information")
                                            .Equals("None", StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// MigrationLogging with Sql logging can be enabled from configuration key(s):
        ///     [Logging:Migration:ShowSql | Logging:LogShowSql] = true
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static bool IsMigrationLogShowSqlEnabled(this IConfiguration configuration)
        {
            return configuration.GetConfigValue("Logging:Migration:ShowSql", "LogShowSql")
                .WithDefault(false.ToString().ToLower())
                .Equals(true.ToString(), StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// MigrationLogging with File logging can be enabled from configuration key(s):
        ///     [Logging:Migration:File | Logging:LogFile] 
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static string GetMigrationLogFile(this IConfiguration configuration)
        {
            return configuration.GetConfigValue("Logging:Migration:File", "LogFile");
        }

        /// <summary>
        /// MigrationLogging with File logging can be enabled from configuration key(s):
        ///     [Logging:Migration:File | Logging:LogFile] 
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static bool IsMigrationLogFileEnabled(this IConfiguration configuration)
        {
            return !configuration.GetMigrationLogFile().IsEmpty();
        }

        /// <summary>
        /// MigrationLogging with ElapsedTime can be enabled from configuration key(s):
        ///     [Logging:Migration:ShowElapsedTime | Logging:LogShowElapsedTime] = true
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static bool IsMigrationLogShowElapsedTimeEnabled(this IConfiguration configuration)
        {
            return configuration.GetConfigValue("Logging:Migration:ShowElapsedTime", "LogShowElapsedTime")
                .WithDefault(false.ToString().ToLower())
                .Equals(true.ToString(), StringComparison.CurrentCultureIgnoreCase);
        }
    }
}