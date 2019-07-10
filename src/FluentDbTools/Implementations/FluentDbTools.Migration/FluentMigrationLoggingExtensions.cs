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
        public static ILoggingBuilder AddFluentMigratorConsoleLogger(
          this ILoggingBuilder loggingBuilder,
          IConfiguration configuration = null)
        {
            configuration = GetConfiguration(loggingBuilder, configuration);
            if (configuration == null || !configuration.IsConsoleLogEnabled())
            {
                return loggingBuilder;
            }
            loggingBuilder.ClearProviders();

            var options = new FluentMigratorLoggerOptions
            {
                ShowSql = configuration.IsMigrationLogShowSqlEnabled(),
                ShowElapsedTime = configuration.IsMigrationLogShowElapsedTimeEnabled()
            };

            loggingBuilder.Services.AddSingleton<IOptions<FluentMigratorLoggerOptions>>(new OptionsWrapper<FluentMigratorLoggerOptions>(options));

            return loggingBuilder.AddFluentMigratorConsoleLogger(options);
        }

        public static ILoggingBuilder AddFluentMigratorConsoleLogger(
          this ILoggingBuilder loggingBuilder,
          FluentMigratorLoggerOptions options,
          bool clearProviders = false)
        {
            if (clearProviders)
            {
                loggingBuilder.ClearProviders();
            }

            return loggingBuilder.AddProvider(new FluentMigratorConsoleLoggerProvider(new OptionsWrapper<FluentMigratorLoggerOptions>(options)));
        }

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

        public static ILoggingBuilder AddFluentMigratorFileLogger(
          this ILoggingBuilder loggingBuilder,
          LogFileFluentMigratorLoggerOptions options,
          bool clearProviders = false)
        {
            if (clearProviders)
            {
                loggingBuilder.ClearProviders();
            }

            var assembly = loggingBuilder.Services
                                    .GetFirstServiceDescriptor<IMigrationSourceItem>()
                                    .GetImplementation<IMigrationSourceItem>()
                                    ?.MigrationTypeCandidates?.FirstOrDefault()?.Assembly;

            var assemblySourceItem = assembly == null
                ? new AssemblySourceItem()
                : new AssemblySourceItem(assembly);

            var optionsManager = new OptionsManager<AssemblySourceOptions>(
                new OptionsFactory<AssemblySourceOptions>(
                    Enumerable.Empty<IConfigureOptions<AssemblySourceOptions>>(),
                    Enumerable.Empty<IPostConfigureOptions<AssemblySourceOptions>>()));

            var assemblySource = new AssemblySource(optionsManager, new List<IAssemblyLoadEngine>(), new[] { assemblySourceItem });

            return loggingBuilder.AddProvider(new LogFileFluentMigratorLoggerProvider(assemblySource, new OptionsWrapper<LogFileFluentMigratorLoggerOptions>(options)));
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
