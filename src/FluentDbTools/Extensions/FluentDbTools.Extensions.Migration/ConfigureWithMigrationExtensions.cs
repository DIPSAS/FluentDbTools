using System.Collections.Generic;
using System.Reflection;
using FluentDbTools.Extensions.MSDependencyInjection;
using FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs;
using FluentDbTools.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FluentDbTools.Extensions.Migration
{
    public static class ConfigureWithMigrationExtensions
    {
        public static IServiceCollection ConfigureWithMigration(this IServiceCollection serviceCollection, IEnumerable<Assembly> assembliesWithMigrationModels)
        {
            serviceCollection
                .Register(FluentDbTools.Migration.Oracle.ServiceRegistration.Register)
                .Register(FluentDbTools.Migration.Postgres.ServiceRegistration.Register)
                .ConfigureWithMigrationAssemblies(FluentDbTools.Migration.ServiceRegistration.Register,
                    assembliesWithMigrationModels)
                .TryAddScoped<IDbConfig, DefaultDbConfig>();
            return serviceCollection;
        }
    }
}