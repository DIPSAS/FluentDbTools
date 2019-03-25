using System.Collections.Generic;
using System.Reflection;
using FluentDbTools.Extensions.MSDependencyInjection;
using FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Extensions.Migration.DefaultConfigs;
using FluentDbTools.Migration.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FluentDbTools.Extensions.Migration
{
    public static class ConfigureWithMigrationExtensions
    {
        public static IServiceCollection ConfigureWithMigration(this IServiceCollection serviceCollection, IEnumerable<Assembly> assembliesWithMigrationModels)
        {
            return serviceCollection
                .Register(FluentDbTools.Migration.Oracle.ServiceRegistration.Register)
                .Register(FluentDbTools.Migration.Postgres.ServiceRegistration.Register)
                .AddDefaultDbMigrationConfig()
                .ConfigureWithMigrationAssemblies(FluentDbTools.Migration.ServiceRegistration.Register,
                    assembliesWithMigrationModels);
        }

        public static IServiceCollection AddDefaultDbMigrationConfig(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddDefultDbConfig()
                .AddScopedIfNotExists<IDbMigrationConfig, MsDbMigrationConfig>();
        }


    }
}