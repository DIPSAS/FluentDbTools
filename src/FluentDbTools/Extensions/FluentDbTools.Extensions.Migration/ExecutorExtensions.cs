using System;
using System.Collections.Generic;
using System.Reflection;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Migration;
using FluentMigrator.Runner;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.DependencyInjection;

namespace FluentDbTools.Extensions.Migration
{
    public static class ExecutorExtensions
    {
        public static IMigrationRunner GetMigrationRunner(this IDbConfig dbConfig,
            IEnumerable<Assembly> assembliesWithMigrationModels,
            IServiceCollection serviceCollection = null)
        {
            return GetMigrationRunner(provider => dbConfig, assembliesWithMigrationModels, serviceCollection);
        }

        public static void DropData(this IDbConfig dbConfig,
            IEnumerable<Assembly> assembliesWithMigrationModels,
            IServiceCollection serviceCollection = null)
        {
            DropData(provider => dbConfig, assembliesWithMigrationModels, serviceCollection);
        }

        public static IMigrationRunner GetMigrationRunner(Func<IServiceProvider, IDbConfig> dbConfig, IEnumerable<Assembly> assembliesWithMigrationModels, 
            IServiceCollection serviceCollection = null)
        {
            serviceCollection = serviceCollection ?? new ServiceCollection();
            var serviceProvider = serviceCollection
                .AddScoped(dbConfig)
                .ConfigureWithMigration(assembliesWithMigrationModels)
                .BuildServiceProvider();

            var scope = serviceProvider.CreateScope();
            return scope.ServiceProvider.GetService<IMigrationRunner>();
        }
        
        public static void DropData(Func<IServiceProvider, IDbConfig> dbConfig, IEnumerable<Assembly> assembliesWithMigrationModels, 
            IServiceCollection serviceCollection = null)
        {
            serviceCollection = serviceCollection ?? new ServiceCollection();
            var serviceProvider = serviceCollection
                .AddScoped(dbConfig)
                .ConfigureWithMigration(assembliesWithMigrationModels)
                .BuildServiceProvider();
            
            using (var scope = serviceProvider.CreateScope())
            {
                var versionTable = scope.ServiceProvider.GetService<IVersionTableMetaData>();
                var migrationRunner = scope.ServiceProvider.GetService<IMigrationRunner>();
                migrationRunner.DropData(versionTable);
            }
        }
    }
}