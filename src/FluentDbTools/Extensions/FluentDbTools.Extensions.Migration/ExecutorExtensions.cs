using System;
using System.Collections.Generic;
using System.Reflection;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Migration;
using FluentDbTools.Migration.Abstractions;
using FluentMigrator.Runner;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.DependencyInjection;

namespace FluentDbTools.Extensions.Migration
{
    public static class ExecutorExtensions
    {
        public static IMigrationRunner GetMigrationRunner(this IDbMigrationConfig dbConfig,
            IEnumerable<Assembly> assembliesWithMigrationModels,
            IServiceCollection serviceCollection = null)
        {
            return GetMigrationRunner(provider => dbConfig, assembliesWithMigrationModels, serviceCollection);
        }

        public static IMigrationRunner GetMigrationRunner(this IServiceProvider provider)
        {
            return provider.GetService<IMigrationRunner>();
        }

        public static IMigrationRunner GetMigrationRunner(Func<IServiceProvider, IDbMigrationConfig> dbConfig,
            IEnumerable<Assembly> assembliesWithMigrationModels,
            IServiceCollection serviceCollection = null)
        {
            var serviceProvider = BuildServiceProviderWithMigration(dbConfig, assembliesWithMigrationModels, serviceCollection);

            var scope = serviceProvider.CreateScope();
            return scope.ServiceProvider.GetMigrationRunner();
        }

        public static void DropSchema(this IDbMigrationConfig dbConfig,
            IEnumerable<Assembly> assembliesWithMigrationModels,
            IServiceCollection serviceCollection = null)
        {
            DropSchema(provider => dbConfig, assembliesWithMigrationModels, serviceCollection);
        }


        public static void DropSchema(Func<IServiceProvider, IDbMigrationConfig> dbConfig,
            IEnumerable<Assembly> assembliesWithMigrationModels,
            IServiceCollection serviceCollection = null)
        {
            BuildServiceProviderWithMigration(dbConfig, assembliesWithMigrationModels, serviceCollection)
                .DropSchema();
        }

        public static IServiceProvider DropSchema(this IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {
                var versionTable = scope.ServiceProvider.GetVersionTableMetaData();
                scope.ServiceProvider.GetMigrationRunner().DropSchema(versionTable);
            }

            return provider;
        }

        public static IServiceProvider CreateSchema(this IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {
                var versionTable = scope.ServiceProvider.GetVersionTableMetaData();
                scope.ServiceProvider.GetMigrationRunner().CreateSchema(versionTable);
            }

            return provider;
        }


        [Obsolete("Please use MigrateUp")]
        public static IServiceProvider ExecuteMigration(this IServiceProvider provider)
        {
            return provider.MigrateUp();
        }

        public static IServiceProvider MigrateUp(this IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {
                var runner = scope.ServiceProvider.GetMigrationRunner();
                runner.MigrateUp();
            }

            return provider;
        }

        public static IServiceProvider MigrateDown(this IServiceProvider provider, long version = default(long))
        {
            using (var scope = provider.CreateScope())
            {
                var runner = scope.ServiceProvider.GetMigrationRunner();
                runner.MigrateDown(version);
            }

            return provider;
        }


        public static IServiceProvider ResetMigration(this IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {
                var runner = scope.ServiceProvider.GetMigrationRunner();
                runner.RollbackToVersion(0);
            }

            return provider;
        }


        public static IVersionTableMetaData GetVersionTableMetaData(this IServiceProvider provider)
        {
            return provider.GetService<IVersionTableMetaData>();
        }


        private static ServiceProvider BuildServiceProviderWithMigration(
            Func<IServiceProvider, IDbMigrationConfig> dbConfig,
            IEnumerable<Assembly> assembliesWithMigrationModels, IServiceCollection serviceCollection)
        {
            serviceCollection = serviceCollection ?? new ServiceCollection();
            var serviceProvider = serviceCollection
                .AddScoped(dbConfig)
                .ConfigureWithMigrationAndScanForVersionTable(assembliesWithMigrationModels)
                .BuildServiceProvider();
            return serviceProvider;
        }

    }
}