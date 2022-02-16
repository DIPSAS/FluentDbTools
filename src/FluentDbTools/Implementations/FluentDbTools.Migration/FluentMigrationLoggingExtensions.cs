using System;
using System.Linq;
using FluentDbTools.Extensions.MSDependencyInjection;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentDbTools.Migration
{
    public static class FluentMigrationLoggingExtensions
    {
        private static bool MigrationConsoleLoggingDisabledReported;
        private static bool MigrationFileLoggingDisabledReported;
        public static bool UseLogFileAppendFluentMigratorLoggerProvider { get; set; }

        /// <summary>
        /// Remove all log-providers and add FluentMigratorConsoleLoggerProvider if enabled by configuration
        /// </summary>
        /// <param name="loggingBuilder"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static ILoggingBuilder AddFluentMigratorConsoleLogger(
          this ILoggingBuilder loggingBuilder,
          IConfiguration configuration = null)
        {
            configuration = GetConfiguration(loggingBuilder, configuration);
            string disabledReason = null;   
            if (configuration == null || !configuration.IsMigrationConsoleLogEnabledInternal(out disabledReason))
            {
                disabledReason = disabledReason ?? "by missing configuration";
                loggingBuilder.Services.RemoveFluentMigratorLoggerOptions();
                loggingBuilder.Services.Remove(new ServiceDescriptor(typeof(ILoggerProvider), typeof(FluentMigratorConsoleLoggerProvider)));

                if (MigrationConsoleLoggingDisabledReported == false)
                {
                    MigrationConsoleLoggingDisabledReported = true;
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine($"Migration Console logging is turned off {disabledReason}");
                    Console.ResetColor();
                }

                return loggingBuilder;
            }

            var options = new FluentMigratorLoggerOptions
            {
                ShowSql = configuration.IsMigrationConsoleLogShowSqlEnabled(),
                ShowElapsedTime = configuration.IsMigrationLogConsoleShowElapsedTimeEnabled()
            };

            return loggingBuilder.AddFluentMigratorConsoleLogger(options);
        }

        /// <summary>
        /// Add FluentMigratorConsoleLoggerProvider with setting in 'options' parameter
        /// </summary>
        /// <param name="loggingBuilder"></param>
        /// <param name="options"></param>
        /// <param name="clearLoggingProviders">If true, all log-providers is first removed</param>
        /// <returns></returns>
        public static ILoggingBuilder AddFluentMigratorConsoleLogger(
          this ILoggingBuilder loggingBuilder,
          FluentMigratorLoggerOptions options,
          bool clearLoggingProviders = false)
        {
            if (clearLoggingProviders)
            {
                loggingBuilder.ClearProviders();
            }

            if (options == null)
            {
                var configuration = GetConfiguration(loggingBuilder, null);
                options = new FluentMigratorLoggerOptions
                {
                    ShowSql = configuration?.IsMigrationConsoleLogShowSqlEnabled() ?? false,
                    ShowElapsedTime = configuration?.IsMigrationLogConsoleShowElapsedTimeEnabled() ?? false
                };
            }
            loggingBuilder.Services.RemoveFluentMigratorLoggerOptions();

            loggingBuilder.Services.TryAddSingleton<IOptions<FluentMigratorLoggerOptions>>(new OptionsWrapper<FluentMigratorLoggerOptions>(options));
            loggingBuilder.Services.TryAddSingleton<ILoggerProvider, FluentMigratorConsoleLoggerProvider>();

            return loggingBuilder;

        }

        /// <summary>
        /// Add LogFileFluentMigratorLoggerProvider if enabled by configuration
        /// </summary>
        /// <param name="loggingBuilder"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static ILoggingBuilder AddFluentMigratorFileLogger(
          this ILoggingBuilder loggingBuilder,
          IConfiguration configuration = null)
        {
            configuration = GetConfiguration(loggingBuilder, configuration);

            string disabledReason = null;   

            if (configuration == null || !configuration.IsMigrationLogFileEnabledInternal(out disabledReason))
            {
                disabledReason = disabledReason ?? "by missing configuration";
                loggingBuilder.Services.RemoveLogFileFluentMigratorLoggerOptions();
                loggingBuilder.Services.RemoveFluentMigratorLogFileProviders();
                if (MigrationFileLoggingDisabledReported == false)
                {
                    MigrationFileLoggingDisabledReported = true;
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine($"Migration File logging is turned off {disabledReason}");
                    Console.ResetColor();
                }

                return loggingBuilder;
            }

            var options = new LogFileFluentMigratorLoggerOptions
            {
                ShowSql = configuration.IsMigrationLogFileShowSqlEnabled(),
                ShowElapsedTime = configuration.IsMigrationLogFileShowElapsedTimeEnabled(),
                OutputFileName = configuration.GetMigrationLogFile()
            };

            return loggingBuilder.AddFluentMigratorFileLogger(options);
        }

        /// <summary>
        /// Add LogFileFluentMigratorLoggerProvider with setting in 'options' parameter
        /// </summary>
        /// <param name="loggingBuilder"></param>
        /// <param name="options"></param>
        /// <param name="clearLoggingProviders">If true, all log-providers is first removed</param>
        /// <returns></returns>
        public static ILoggingBuilder AddFluentMigratorFileLogger(
          this ILoggingBuilder loggingBuilder,
          LogFileFluentMigratorLoggerOptions options,
          bool clearLoggingProviders = false)
        {
            if (clearLoggingProviders)
            {
                loggingBuilder.ClearProviders();
            }

            if (options == null)
            {
                var configuration = GetConfiguration(loggingBuilder, null);
                options = new LogFileFluentMigratorLoggerOptions
                {
                    ShowSql = configuration?.IsMigrationLogFileShowSqlEnabled() ?? false,
                    ShowElapsedTime = configuration?.IsMigrationLogFileShowElapsedTimeEnabled() ?? false,
                    OutputFileName = configuration?.GetMigrationLogFile()
                };
            }

            loggingBuilder.Services.RemoveLogFileFluentMigratorLoggerOptions();
            loggingBuilder.Services.AddSingleton<IOptions<LogFileFluentMigratorLoggerOptions>>(new OptionsWrapper<LogFileFluentMigratorLoggerOptions>(options));
            if (UseLogFileAppendFluentMigratorLoggerProvider)
            {
                loggingBuilder.Services.AddLogFileAppendFluentMigratorLoggerProvider();
            }
            else
            {
                if (options?.OutputFileName != null)
                {
                    var writer = LogFileAppendFluentMigratorLoggerProvider.GetStreamWriter(null, options, out var logFile);
                    writer?.Close();

                    options.OutputFileName = logFile;
                }

                loggingBuilder.Services.AddLogFileFluentMigratorLoggerProvider();

            }
            return loggingBuilder;
        }

        /// <summary>
        /// Add LogFileAppendFluentMigratorLoggerProvider
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        internal static IServiceCollection AddLogFileAppendFluentMigratorLoggerProvider(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .RemoveFluentMigratorLogFileProviders()
                .AddSingleton<ILoggerProvider,LogFileAppendFluentMigratorLoggerProvider>();
        }

        /// <summary>
        /// Add LogFileFluentMigratorLoggerProvider
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        internal static IServiceCollection AddLogFileFluentMigratorLoggerProvider(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .RemoveFluentMigratorLogFileProviders()
                .AddSingleton<ILoggerProvider,LogFileFluentMigratorLoggerProvider>();
        }

        private static IServiceCollection RemoveFluentMigratorLogFileProviders(this IServiceCollection serviceCollection)
        {
            var toRemove = serviceCollection.FirstOrDefault(x => x.ImplementationInstance?.GetType() == typeof(LogFileFluentMigratorLoggerProvider) || x.ImplementationType == typeof(LogFileFluentMigratorLoggerProvider));
            if (toRemove != null)
            {
                serviceCollection.Remove(toRemove);
            }

            toRemove = serviceCollection.FirstOrDefault(x => x.ImplementationInstance?.GetType() == typeof(LogFileAppendFluentMigratorLoggerProvider) || x.ImplementationType == typeof(LogFileAppendFluentMigratorLoggerProvider));
            if (toRemove != null)
            {
                serviceCollection.Remove(toRemove);
            }
            return serviceCollection;
        }

        private static void RemoveFluentMigratorLoggerOptions(this IServiceCollection serviceCollection)
        {
            var toRemove = serviceCollection.FirstOrDefault(x => x.ImplementationInstance?.GetType() == typeof(OptionsWrapper<FluentMigratorLoggerOptions>) || x.ImplementationType == typeof(OptionsWrapper<FluentMigratorLoggerOptions>));
            if (toRemove != null)
            {
                serviceCollection.Remove(toRemove);
            }
        }

        private static void RemoveLogFileFluentMigratorLoggerOptions(this IServiceCollection serviceCollection)
        {
            var toRemove = serviceCollection.FirstOrDefault(x => x.ImplementationInstance?.GetType() == typeof(OptionsWrapper<LogFileFluentMigratorLoggerOptions>) || x.ImplementationType == typeof(OptionsWrapper<LogFileFluentMigratorLoggerOptions>));
            if (toRemove != null)
            {
                serviceCollection.Remove(toRemove);
            }
        }


        private static IConfiguration GetConfiguration(
          ILoggingBuilder loggingBuilder,
          IConfiguration configuration)
        {
            return configuration ??
                   loggingBuilder.Services
                       .GetFirstServiceDescriptor<IConfiguration>()
                       .GetImplementation<IConfiguration>(loggingBuilder.Services.BuildServiceProvider().CreateScope());
        }
    }
}
