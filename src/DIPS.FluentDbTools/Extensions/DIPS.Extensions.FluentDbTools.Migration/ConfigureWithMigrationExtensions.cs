using System.Collections.Generic;
using System.Reflection;
using DIPS.Extensions.FluentDbTools.Common;
using DIPS.Extensions.FluentDbTools.DbProvider;
using Microsoft.Extensions.DependencyInjection;

namespace DIPS.Extensions.FluentDbTools.Migration
{
    public static class ConfigureWithMigrationExtensions
    {
        public static IServiceCollection ConfigureWithMigration(this IServiceCollection serviceCollection, IEnumerable<Assembly> assembliesWithMigrationModels)
        {
            return serviceCollection
                .Register(DIPS.FluentDbTools.Migration.Oracle.ServiceRegistration.Register)
                .Register(DIPS.FluentDbTools.Migration.Postgres.ServiceRegistration.Register)
                .ConfigureWithMigrationAssemblies(DIPS.FluentDbTools.Migration.ServiceRegistration.Register,
                    assembliesWithMigrationModels);
        }
    }
}