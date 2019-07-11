using System.Collections.Generic;
using System.Linq;
using FluentDbTools.Extensions.MSDependencyInjection;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentDbTools.Migration
{
    public static class FluentMigrationLoggingExtensions
    {
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
            if (configuration == null || !configuration.IsMigrationConsoleLogEnabled())
            {
                return loggingBuilder;
            }

            var options = new FluentMigratorLoggerOptions
            {
                ShowSql = configuration.IsMigrationLogShowSqlEnabled(),
                ShowElapsedTime = configuration.IsMigrationLogShowElapsedTimeEnabled()
            };

            return loggingBuilder.AddFluentMigratorConsoleLogger(options, true);
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

            loggingBuilder.Services.AddSingleton<IOptions<FluentMigratorLoggerOptions>>(new OptionsWrapper<FluentMigratorLoggerOptions>(options));
            loggingBuilder.Services.AddSingleton<ILoggerProvider, FluentMigratorConsoleLoggerProvider>();

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

            if (configuration == null || !configuration.IsMigrationLogFileEnabled())
            {
                return loggingBuilder;
            }

            var options = new LogFileFluentMigratorLoggerOptions
            {
                ShowSql = configuration.IsMigrationLogShowSqlEnabled(),
                ShowElapsedTime = configuration.IsMigrationLogShowElapsedTimeEnabled(),
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

            loggingBuilder.Services.AddSingleton<IOptions<LogFileFluentMigratorLoggerOptions>>(new OptionsWrapper<LogFileFluentMigratorLoggerOptions>(options));
            loggingBuilder.Services.AddSingleton<ILoggerProvider, LogFileFluentMigratorLoggerProvider>();
            return loggingBuilder;
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
