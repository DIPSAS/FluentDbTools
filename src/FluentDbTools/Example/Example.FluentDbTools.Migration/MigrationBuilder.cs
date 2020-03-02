using System;
using System.Collections.Generic;
using System.Reflection;
using Example.FluentDbTools.Config;
using FluentDbTools.Common.Abstractions;
using Example.FluentDbTools.Migration.MigrationModels;
using FluentDbTools.Extensions.Migration;
using FluentDbTools.Extensions.MSDependencyInjection;
using FluentDbTools.Extensions.MSDependencyInjection.Oracle;
using FluentDbTools.Extensions.MSDependencyInjection.Postgres;
using FluentDbTools.Migration;
using Microsoft.Extensions.DependencyInjection;

namespace Example.FluentDbTools.Migration
{
    public static class MigrationBuilder
    {
        public static IEnumerable<Assembly> MigrationAssemblies => new[] { typeof(AddPersonTable).Assembly };

        public static IServiceCollection BuildMigrationServiceCollection(
            SupportedDatabaseTypes databaseType,
            Dictionary<string, string> overrideConfig = null,
            bool loadExampleConfig = true,
            IEnumerable<Assembly> assemblies = null)
        {
            assemblies =  assemblies ?? new [] { Assembly.GetEntryAssembly(), Assembly.GetCallingAssembly(), Assembly.GetExecutingAssembly() };

            return new ServiceCollection()
                .ConfigureWithMigrationAndScanForVersionTable(MigrationAssemblies)
                .AddDefaultDbConfig(assemblies:assemblies)
                .UseExampleConfiguration(
                    databaseType,
                    overrideConfig,
                    loadExampleConfig)
            
                    .AddLogging(x => x.AddFluentMigratorConsoleLogger(null)
                             .AddFluentMigratorFileLogger());
        }

        public static IServiceProvider BuildMigration(
            SupportedDatabaseTypes databaseType,
            Dictionary<string, string> overrideConfig = null,
            Func<IServiceCollection, IServiceCollection> additionalRegistration = null,
            bool loadExampleConfig = true,
            bool validateScope=true,
            IEnumerable<Assembly> assemblies = null)
        {
            assemblies =  assemblies ?? new [] { Assembly.GetEntryAssembly(), Assembly.GetCallingAssembly(), Assembly.GetExecutingAssembly() };

            var serviceCollection = BuildMigrationServiceCollection(databaseType, overrideConfig, loadExampleConfig, assemblies);
            additionalRegistration?.Invoke(serviceCollection);


            return serviceCollection.BuildServiceProvider(validateScope);
        }
    }
}