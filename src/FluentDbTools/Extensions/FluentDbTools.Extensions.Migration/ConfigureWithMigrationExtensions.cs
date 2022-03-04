﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Common.Abstractions.PrioritizedConfig;
using FluentDbTools.Extensions.MSDependencyInjection;
using FluentDbTools.Extensions.Migration.DefaultConfigs;
using FluentDbTools.Migration.Abstractions;
using FluentDbTools.Migration.Oracle;
using FluentDbTools.Migration.Postgres;
using FluentMigrator;
using FluentMigrator.Runner.Generators.Oracle;
using FluentMigrator.Runner.Generators.Postgres;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
// ReSharper disable InconsistentNaming
#pragma warning disable CS1591

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
                .AddPrioritizedConfigKeysRegistration(assembliesWithMigrationModelArray)
                .AddDefaultDbMigrationConfig()
                .Register(FluentDbTools.Migration.Oracle.ServiceRegistration.Register)
                .Register(FluentDbTools.Migration.Postgres.ServiceRegistration.Register)
                .RegisterDatabaseDependedFluentMigrationTypes()
                .ConfigureWithMigrationAssemblies(FluentDbTools.Migration.ServiceRegistration.Register, assembliesWithMigrationModelArray)
                .RegisterDatabaseDependedFluentMigrationTypes();
        }


        internal static IServiceCollection ConfigureVersionTableMetaData(
            this IServiceCollection serviceCollection,
            IEnumerable<Assembly> assembliesWithMigrationModels)
        {
            var interfaceType = typeof(IVersionTableMetaData);
            foreach (var assembliesWithMigrationModel in assembliesWithMigrationModels)
            {
                var implementationType = assembliesWithMigrationModel
                    .GetTypes()
                    .FirstOrDefault(x => !x.IsInterface && !x.IsAbstract && x.IsImplementingInterfaceType(interfaceType));

                if (implementationType == null)
                {
                    continue;
                }

                serviceCollection.AddScoped(interfaceType, implementationType);
                return serviceCollection;
            }

            return serviceCollection;
        }

        public static IServiceCollection AddDefaultDbMigrationConfig(this IServiceCollection serviceCollection,
            IEnumerable<Assembly> assemblies = null)
        {
            assemblies = assemblies ?? new[] { Assembly.GetEntryAssembly(), Assembly.GetCallingAssembly(), Assembly.GetExecutingAssembly() };

            serviceCollection.AddDefaultDbConfig(assemblies: assemblies);
            serviceCollection.TryAddScoped<IDbMigrationConfig, MsDbMigrationConfig>();
            return serviceCollection;
        }

        public static IServiceCollection RegisterDatabaseDependedFluentMigrationTypes(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .Replace(ServiceDescriptor.Scoped<IMigrationProcessor>(sp =>
            {
                var isPostgres = sp.GetService<IDbMigrationConfig>()?.DbType == SupportedDatabaseTypes.Postgres;
                if (isPostgres)
                {
                    return sp.GetRequiredService<ExtendedPostgresProcessor>();
                }

                return sp.GetRequiredService<ExtendedOracleManagedProcessor>();
            }))
            .Replace(ServiceDescriptor.Scoped<IMigrationGenerator>(sp =>
            {
                var isPostgres = sp.GetService<IDbMigrationConfig>()?.DbType == SupportedDatabaseTypes.Postgres;
                if (isPostgres)
                {
                    return sp.GetRequiredService<PostgresGenerator>();
                }

                return sp.GetRequiredService<OracleGenerator>();
            }));


        }

        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public static IDbMigrationConfig GetOrCreateDbMigrationConfig(this IServiceProvider serviceProvider)
        {
            var dbMigrationConfig = serviceProvider.GetDbMigrationConfig(false);
            if (dbMigrationConfig != null)
            {
                return dbMigrationConfig;
            }

            return new MsDbMigrationConfig(
                serviceProvider.GetRequiredService<IConfiguration>(),
                serviceProvider.GetDbConfig(false),
                serviceProvider.GetService<IConfigurationChangedHandler>(),
                serviceProvider.GetService<IPrioritizedConfigValues>(),
                serviceProvider.GetServices<IPrioritizedConfigKeys>());
        }

    }
}