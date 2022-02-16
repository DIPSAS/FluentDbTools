using System.IO;
using System.Runtime.InteropServices;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs;
using Microsoft.Extensions.Configuration;

namespace FluentDbTools.Extensions.MSDependencyInjection
{
#pragma warning disable CS1591
    public static class ConfigurationExtensions
#pragma warning restore CS1591
    {
        /// <summary>
        /// MigrationLogging with Console logging can be enabled from configuration key(s): <br/>
        ///     [Logging:Migration:ConsoleEnabled | LogMigrationConsole ]= true or section Logging:Migration:Console exists<br/>
        ///     OR<br/>
        ///     [Logging:Migration:Console:LogLevel | Logging:LogLevel:Migration:Console |
        ///      Logging:Migration:LogLevel | Logging:LogLevel:Migration ] != None<br/><br/>
        ///     When all [Logging:Migration:Console:LogLevel, Logging:LogLevel:Migration:Console, 
        ///      Logging:Migration:LogLevel, Logging:LogLevel:Migration] is null, Console logging will be enabled.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static bool IsMigrationConsoleLogEnabled(this IConfiguration configuration)
        {
            return configuration.IsMigrationConsoleLogEnabledInternal(out _);
        }
        
        internal static bool IsMigrationConsoleLogEnabledInternal(this IConfiguration configuration, out string disabledReason)
        {
            var consoleLogEnabledStr = configuration.GetConfigValue("Logging:Migration:ConsoleEnabled", "LogMigrationConsole");
            if (consoleLogEnabledStr.IsNotEmpty())
            {
                disabledReason = $"by Logging:Migration:ConsoleEnabled = {consoleLogEnabledStr}";
                return consoleLogEnabledStr.IsTrue();
            }

            var logLevel = configuration
                .GetConfigValue("Logging:Migration:Console:LogLevel", "Logging:LogLevel:Migration:Console", "Logging:Migration:LogLevel", "Logging:LogLevel:Migration")
                .WithDefault("Information");
            var enabled = logLevel.EqualsIgnoreCase("None") == false;
            disabledReason = $"by LogLevel = {logLevel}";
            return enabled;
        }


        /// <summary>
        /// MigrationLogging with Sql logging can be enabled from configuration key(s):
        ///     [Logging:Migration:ShowSql | Logging:LogMigrationShowSql | LogMigrationShowSql] = true
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static bool IsMigrationConsoleLogShowSqlEnabled(this IConfiguration configuration)
        {
            return configuration
                .GetConfigValue("Logging:Migration:Console:ShowSql", "Logging:Migration:ShowSql", "LogMigrationShowSql")
                .WithDefault(false)
                .IsTrue();
        }

        /// <summary>
        /// MigrationLogging with ElapsedTime can be enabled from configuration key(s):
        ///     [Logging:Migration:ShowElapsedTime | Logging:LogMigrationShowElapsedTime | LogMigrationShowElapsedTime] == true
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static bool IsMigrationLogConsoleShowElapsedTimeEnabled(this IConfiguration configuration)
        {
            return configuration.GetConfigValue("Logging:Migration:File:ShowElapsedTime", "Logging:Migration:ShowElapsedTime", "LogMigrationShowElapsedTime")
                .WithDefault(false)
                .IsTrue();
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
            return configuration.IsMigrationLogFileEnabledInternal(out _);
        }

        internal static bool IsMigrationLogFileEnabledInternal(this IConfiguration configuration, out string disabledReason)
        {
            var consoleLogEnabledStr = configuration.GetConfigValue("Logging:Migration:FileEnabled", "LogMigrationFileEnabled");
            if (consoleLogEnabledStr.IsNotEmpty())
            {
                disabledReason = $"by Logging:Migration:FileEnabled = {consoleLogEnabledStr}";
                if (consoleLogEnabledStr.IsTrue() == false)
                {
                    return false;
                }
            }


            var file = configuration.GetSection("Logging:Migration:File") != null ? configuration.GetMigrationLogFile() : null;
            disabledReason = "by empty Logging:Migration:File configuration-value";

            return file.IsNotEmpty();
        }


        /// <summary>
        /// MigrationLogging with File location can be configured from configuration key(s):
        ///     [Logging:Migration:File | Logging:LogMigrationFile | LogMigrationFile] != null
        ///     If set, IsMigrationLogFileEnabled() will be true
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="defaultLogPath"></param>
        /// <returns></returns>
        public static string GetMigrationLogFile(this IConfiguration configuration, string defaultLogPath = null)
        {
            var file = configuration.GetConfigValue("Logging:Migration:FileName", "Logging:Migration:File:Name","Logging:Migration:File:File","Logging:Migration:File:$value","Logging:Migration:File:Value","Logging:Migration:File:", "Logging:Migration:File", "LogMigrationFile");
            return file.IsEmpty() ? file : GetLogFile(configuration, file, defaultLogPath);
        }

        /// <summary>
        /// MigrationLogging with Sql logging can be enabled from configuration key(s):
        ///     [Logging:Migration:ShowSql | Logging:LogMigrationShowSql | LogMigrationShowSql] = true
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static bool IsMigrationLogFileShowSqlEnabled(this IConfiguration configuration)
        {
            return configuration
                .GetConfigValue("Logging:Migration:File:ShowSql", "Logging:Migration:ShowSql", "LogMigrationShowSql")
                .WithDefault(true)
                .IsTrue();
        }

        /// <summary>
        /// MigrationLogging with ElapsedTime can be enabled from configuration key(s):
        ///     [Logging:Migration:ShowElapsedTime | Logging:LogMigrationShowElapsedTime | LogMigrationShowElapsedTime] == true
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static bool IsMigrationLogFileShowElapsedTimeEnabled(this IConfiguration configuration)
        {
            return configuration.GetConfigValue("Logging:Migration:File:ShowElapsedTime", "Logging:Migration:ShowElapsedTime", "LogMigrationShowElapsedTime")
                .WithDefault(false)
                .IsTrue();
        }


        /// <summary>
        /// Get log file based on LogPath
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="file"></param>
        /// <param name="defaultLogPath"></param>
        /// <returns></returns>
        public static string GetLogFile(this IConfiguration configuration, string file, string defaultLogPath = null)
        {
            if (file.IsEmpty())
            {
                return file;
            }

            var startPathToReplace = @"DIPS-Log";
            var pathToReplaceWindows = $@"C:\{startPathToReplace}\";
            var pathToReplaceRuntime = $"C:{Path.DirectorySeparatorChar}{startPathToReplace}{Path.DirectorySeparatorChar}";
            defaultLogPath = defaultLogPath ?? string.Empty;
            var logPath = configuration.GetConfigValue("LogPath").WithDefault(defaultLogPath).Trim();
            var replace = true;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && file.StartsWithIgnoreCase(pathToReplaceWindows))
            {
                var rootExists = new DirectoryInfo(pathToReplaceWindows).Root.Exists;
                replace = rootExists == false;
                if (rootExists && logPath.IsEmpty())
                {
                    logPath = pathToReplaceWindows;
                }
            }

            if (replace || logPath.IsNotEmpty())
            {
                var windowsLogPath = logPath.IsEmpty() ? logPath : $@"{logPath.TrimEnd('\\')}\";
                var runtimeLogPath = logPath.IsEmpty() ? logPath : $@"{logPath.TrimEnd(Path.DirectorySeparatorChar)}{Path.DirectorySeparatorChar}";

                file = file
                    .ReplaceIgnoreCase(defaultLogPath, logPath)
                    .ReplaceIgnoreCase(pathToReplaceWindows, windowsLogPath)
                    .ReplaceIgnoreCase(pathToReplaceRuntime, runtimeLogPath);
            }
            
            file = file.ReplaceIgnoreCase("${LogPath}", logPath);

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
        /// <inheritdoc cref="IsMigrationConsoleLogEnabled"/>
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static bool IsConsoleLogEnabled(this IConfiguration configuration)
        {
            return IsMigrationConsoleLogEnabled(configuration);
        }

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


    }
}