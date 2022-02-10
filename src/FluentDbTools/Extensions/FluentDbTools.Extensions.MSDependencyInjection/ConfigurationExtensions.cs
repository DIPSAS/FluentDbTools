using System;
using System.IO;
using System.Runtime.InteropServices;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs;
using Microsoft.Extensions.Configuration;

namespace FluentDbTools.Extensions.MSDependencyInjection
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Return the first valid(non empty) configuration value
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="keys">Configuration keys to resolve</param>
        /// <returns></returns>
        public static string GetConfigValue(this IConfiguration configuration, params string[] keys)
        {
            if (configuration == null)
            {
                return null;
            }

            foreach (var key in keys)
            {
                var value = configuration[key];
                if (value.IsNotEmpty())
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

                foreach (var readonlyKey in keys)
                {
                    var key = readonlyKey;

                    var value = dbSection.GetSectionStringValue(key);
                    if (value.IsNotEmpty())
                    {
                        return value;
                    }

                    if (key.StartsWithIgnoreCase($"{section}:"))
                    {
                        key = key.Substring($"{section}:".Length);

                        value = dbSection.GetSectionStringValue(key);
                        if (value.IsNotEmpty())
                        {
                            return value;
                        }
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
            var consoleLogEnabled = configuration
                .GetConfigValue("ConsoleLog")
                .WithDefault(false)
                .IsTrue();

            return consoleLogEnabled || !configuration
                                            .GetConfigValue("Logging:Console:LogLevel", "Logging:LogLevel:Console")
                                            .WithDefault("Information")
                                            .EqualsIgnoreCase("None");
        }

        /// <summary>
        /// MigrationLogging with Console logging can be enabled from configuration key(s):
        ///     [Logging:Migration:Console | LogMigrationConsole ]= true
        ///     OR
        ///     [Logging:Migration:LogLevel |Logging:LogLevel:Migration] != None
        ///     When both Logging:Console:LogLevel and Logging:LogLevel:Console is null, Console logging will be enabled.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static bool IsMigrationConsoleLogEnabled(this IConfiguration configuration)
        {
            var consoleLogEnabled = configuration
                .GetConfigValue("Logging:Migration:Console", "LogMigrationConsole")
                .WithDefault(false)
                .IsTrue();

            return consoleLogEnabled || !configuration
                       .GetConfigValue("Logging:Migration:LogLevel", "Logging:LogLevel:Migration")
                       .WithDefault("Information")
                       .EqualsIgnoreCase("None");
        }


        /// <summary>
        /// MigrationLogging with Sql logging can be enabled from configuration key(s):
        ///     [Logging:Migration:ShowSql | Logging:LogMigrationShowSql | LogMigrationShowSql] = true
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static bool IsMigrationLogShowSqlEnabled(this IConfiguration configuration)
        {
            return configuration
                .GetConfigValue("Logging:Migration:ShowSql", "LogMigrationShowSql")
                .WithDefault(false)
                .IsTrue();
        }

        /// <summary>
        /// MigrationLogging with File location can be configured from configuration key(s):
        ///     [Logging:Migration:File | Logging:LogMigrationFile | LogMigrationFile] != null
        ///     If set, IsMigrationLogFileEnabled() will be true
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static string GetMigrationLogFile(this IConfiguration configuration)
        {
            var file = configuration.GetConfigValue("Logging:Migration:File", "LogMigrationFile");
            return file.IsEmpty() ? file : GetLogFile(configuration, file);
        }

        /// <summary>
        /// Get log file based on LogPath
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string GetLogFile(this IConfiguration configuration, string file)
        {
            if (file.IsEmpty())
            {
                return file;
            }

            var startPathToReplace = @"DIPS-Log";
            var pathToReplaceWindows = $@"C:\{startPathToReplace}\";
            var pathToReplaceRuntime = $"C:{Path.DirectorySeparatorChar}{startPathToReplace}{Path.DirectorySeparatorChar}";
            var defaultLogPath = string.Empty;
            var logPath = configuration.GetConfigValue("LogPath").WithDefault(defaultLogPath).Trim();
            var windowsLogPath = logPath.IsEmpty() ? logPath : $@"{logPath.TrimEnd('\\')}\";
            var runtimeLogPath = logPath.IsEmpty() ? logPath : $@"{logPath.TrimEnd(Path.DirectorySeparatorChar)}{Path.DirectorySeparatorChar}";
            file = file.ReplaceIgnoreCase(defaultLogPath, logPath)
                .ReplaceIgnoreCase(pathToReplaceWindows, windowsLogPath)
                .ReplaceIgnoreCase(pathToReplaceRuntime, runtimeLogPath)
                .ReplaceIgnoreCase("${LogPath}", logPath);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
                file.StartsWith(@"\\"))
            {
                return file;
            }

            if (file.StartsWith(@"\") == false)
            {
                return file;
            }

            file = file.Substring(1);

            return file.StartsWith(@"\") ? file.Substring(1) : file;
        }

        /// <summary>
        /// MigrationLogging with File logging can be enabled from configuration key(s):
        ///     [Logging:Migration:FileEnabled | Logging:LogMigrationFileEnabled | LogMigrationFileEnabled ] == true
        ///     OR
        ///     [Logging:Migration:File | Logging:LogMigrationFile | LogMigrationFile] != null
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static bool IsMigrationLogFileEnabled(this IConfiguration configuration)
        {
            var enabled = configuration
                .GetConfigValue("Logging:Migration:FileEnabled", "LogMigrationFileEnabled")
                .WithDefault(false)
                .IsTrue();

            return enabled || configuration.GetMigrationLogFile().IsNotEmpty();
        }

        /// <summary>
        /// MigrationLogging with ElapsedTime can be enabled from configuration key(s):
        ///     [Logging:Migration:ShowElapsedTime | Logging:LogMigrationShowElapsedTime | LogMigrationShowElapsedTime] == true
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static bool IsMigrationLogShowElapsedTimeEnabled(this IConfiguration configuration)
        {
            return configuration.GetConfigValue("Logging:Migration:ShowElapsedTime", "LogMigrationShowElapsedTime")
                .WithDefault(false)
                .IsTrue();
        }
    }
}