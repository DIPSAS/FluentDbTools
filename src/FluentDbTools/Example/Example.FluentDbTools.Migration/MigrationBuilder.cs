using System;
using System.Collections.Generic;
using System.Reflection;
using Example.FluentDbTools.Config;
using FluentDbTools.Common.Abstractions;
using Example.FluentDbTools.Migration.MigrationModels;
using FluentDbTools.Extensions.Migration;
using FluentDbTools.Extensions.MSDependencyInjection.Oracle;
using FluentDbTools.Extensions.MSDependencyInjection.Postgres;
using Microsoft.Extensions.DependencyInjection;

namespace Example.FluentDbTools.Migration
{
    public static class MigrationBuilder
    {
        public static IEnumerable<Assembly> MigrationAssemblies => new[] { typeof(AddPersonTable).Assembly };

        public static IServiceCollection BuildMigrationServiceCollection(
            SupportedDatabaseTypes databaseType,
            Dictionary<string, string> overrideConfig = null,
            Func<IServiceCollection, IServiceCollection> additionalRegistration = null)
        {
            return new ServiceCollection()
                .ConfigureWithMigrationAndScanForVersionTable(MigrationAssemblies)
                .UseExampleConfiguration(
                    databaseType,
                    overrideConfig);
        }

        public static IServiceProvider BuildMigration(
            SupportedDatabaseTypes databaseType,
            Dictionary<string, string> overrideConfig = null,
            Func<IServiceCollection, IServiceCollection> additionalRegistration = null)
        {
            var serviceCollection = BuildMigrationServiceCollection(databaseType, overrideConfig);
            additionalRegistration?.Invoke(serviceCollection);


            return serviceCollection.BuildServiceProvider();
        }
    }
}