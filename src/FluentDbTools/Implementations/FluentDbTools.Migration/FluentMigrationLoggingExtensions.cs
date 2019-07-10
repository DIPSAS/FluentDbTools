
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    public static ILoggingBuilder AddFluentMigratorConsole(
      this ILoggingBuilder loggingBuilder,
      IConfiguration configuration)
    {
      configuration = GetConfiguration(loggingBuilder, configuration);
      if (configuration == null || !configuration.IsConsoleLogEnabled())
        return loggingBuilder;
      loggingBuilder.ClearProviders();
      return loggingBuilder.AddFluentMigratorConsole(new FluentMigratorLoggerOptions
      {
        ShowSql = configuration.IsMigrationLogShowSqlEnabled(),
        ShowElapsedTime = configuration.IsMigrationLogShowElapsedTimeEnabled()
      });
    }

    public static ILoggingBuilder AddFluentMigratorConsole(
      this ILoggingBuilder loggingBuilder,
      FluentMigratorLoggerOptions options,
      bool clearProviders = false)
    {
      if (clearProviders)
        loggingBuilder.ClearProviders();
      return loggingBuilder.AddProvider(new FluentMigratorConsoleLoggerProvider(new OptionsWrapper<FluentMigratorLoggerOptions>(options)));
    }

    public static ILoggingBuilder AddFluentMigratorFileLogger(
      this ILoggingBuilder loggingBuilder,
      IConfiguration configuration = null)
    {
      configuration = GetConfiguration(loggingBuilder, configuration);
      if (configuration == null || !configuration.IsMigrationLogFileEnabled())
        return loggingBuilder;
      var loggingBuilder1 = loggingBuilder;
      var options = new LogFileFluentMigratorLoggerOptions
      {
          ShowSql = configuration.IsMigrationLogShowSqlEnabled(),
          ShowElapsedTime = configuration.IsMigrationLogShowElapsedTimeEnabled(),
          OutputFileName = configuration.GetMigrationLogFile()
      };
      return loggingBuilder1.AddFluentMigratorFileLogger(options);
    }

    public static ILoggingBuilder AddFluentMigratorFileLogger(
      this ILoggingBuilder loggingBuilder,
      LogFileFluentMigratorLoggerOptions options,
      bool clearProviders = false)
    {
      if (clearProviders)
        loggingBuilder.ClearProviders();
      var implementation = loggingBuilder.Services.GetFirstServiceDescriptor<IMigrationSourceItem>().GetImplementation<IMigrationSourceItem>((IServiceScope) null);
      Type type1;
      if (implementation == null)
      {
        type1 = null;
      }
      else
      {
        var migrationTypeCandidates = implementation.MigrationTypeCandidates;
        type1 = migrationTypeCandidates != null ? migrationTypeCandidates.FirstOrDefault() : null;
      }
      var type2 = type1;
      AssemblySourceItem assemblySourceItem;
      if (!(type2 == null))
        assemblySourceItem = new AssemblySourceItem(type2.Assembly);
      else
        assemblySourceItem = new AssemblySourceItem(Array.Empty<Assembly>());
      var assemblySource = new AssemblySource(new OptionsManager<AssemblySourceOptions>(new OptionsFactory<AssemblySourceOptions>(Enumerable.Empty<IConfigureOptions<AssemblySourceOptions>>(), Enumerable.Empty<IPostConfigureOptions<AssemblySourceOptions>>())), new List<IAssemblyLoadEngine>(), new AssemblySourceItem[1]
      {
          assemblySourceItem
      });
      return loggingBuilder.AddProvider(new LogFileFluentMigratorLoggerProvider(assemblySource, new OptionsWrapper<LogFileFluentMigratorLoggerOptions>(options)));
    }

    private static IConfiguration GetConfiguration(
      ILoggingBuilder loggingBuilder,
      IConfiguration configuration)
    {
      return configuration ?? loggingBuilder.Services.GetFirstServiceDescriptor<IConfiguration>().GetImplementation<IConfiguration>(loggingBuilder.Services.BuildServiceProvider().CreateScope());
    }
  }
}
