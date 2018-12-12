using System.Collections.Generic;
using System.Reflection;
using DIPS.Extensions.FluentDbTools.MSDependencyInjection;
using DIPS.Extensions.FluentDbTools.MSDependencyInjection.DefaultConfigs;
using DIPS.FluentDbTools.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DIPS.Extensions.FluentDbTools.Migration
{
    public static class ConfigureWithMigrationExtensions
    {
        public static IServiceCollection ConfigureWithMigration(this IServiceCollection serviceCollection, IEnumerable<Assembly> assembliesWithMigrationModels)
        {
            serviceCollection
                .Register(DIPS.FluentDbTools.Migration.Oracle.ServiceRegistration.Register)
                .Register(DIPS.FluentDbTools.Migration.Postgres.ServiceRegistration.Register)
                .ConfigureWithMigrationAssemblies(DIPS.FluentDbTools.Migration.ServiceRegistration.Register,
                    assembliesWithMigrationModels)
                .TryAddScoped<IDbConfig, DefaultDbConfig>();
            return serviceCollection;
        }
    }
}