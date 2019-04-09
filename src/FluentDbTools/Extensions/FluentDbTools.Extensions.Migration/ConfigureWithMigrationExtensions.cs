using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentDbTools.Extensions.MSDependencyInjection;
using FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Extensions.Migration.DefaultConfigs;
using FluentDbTools.Migration.Abstractions;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FluentDbTools.Extensions.Migration
{
    public static class ConfigureWithMigrationExtensions
    {
        public static IServiceCollection ConfigureWithMigrationAndScanForVersionTable(
            this IServiceCollection serviceCollection, 
            IEnumerable<Assembly> assembliesWithMigrationModels)
        {
            var assembliesWithMigrationModelArray = assembliesWithMigrationModels.ToArray();

            return serviceCollection
                .ConfigureWithMigration(assembliesWithMigrationModelArray)
                .ConfigureVersionTableMetaData(assembliesWithMigrationModelArray);
        }

        public static IServiceCollection ConfigureWithMigration(
            this IServiceCollection serviceCollection, 
            IEnumerable<Assembly> assembliesWithMigrationModels)
        {
            var assembliesWithMigrationModelArray = assembliesWithMigrationModels.ToArray();
            return serviceCollection
                .Register(FluentDbTools.Migration.Oracle.ServiceRegistration.Register)
                .Register(FluentDbTools.Migration.Postgres.ServiceRegistration.Register)
                .AddDefaultDbMigrationConfig()
                .ConfigureWithMigrationAssemblies(FluentDbTools.Migration.ServiceRegistration.Register,
                    assembliesWithMigrationModelArray);
        }

        internal static IServiceCollection ConfigureVersionTableMetaData(
            this IServiceCollection serviceCollection,
            IEnumerable<Assembly> assembliesWithMigrationModels)
        {
            var serviceType = typeof(IVersionTableMetaData);
            foreach (var assembliesWithMigrationModel in assembliesWithMigrationModels)
            {
                var implementationType = assembliesWithMigrationModel
                    .GetTypes()
                    .FirstOrDefault(x => !x.IsInterface && !x.IsAbstract && serviceType.IsAssignableFrom(x));

                if (implementationType == null)
                {
                    continue;
                }

                serviceCollection.Remove<IVersionTableMetaData>();
                serviceCollection.AddScoped(serviceType, implementationType);
                return serviceCollection;
            }

            return serviceCollection;
        }

        public static IServiceCollection AddDefaultDbMigrationConfig(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddDefultDbConfig()
                .AddScopedIfNotExists<IDbMigrationConfig, MsDbMigrationConfig>();
        }


    }
}