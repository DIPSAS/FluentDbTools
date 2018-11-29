using DIPS.Extensions.FluentDbTools.Common;
using DIPS.FluentDbTools.Migration.Abstractions;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Generators.Postgres;
using Microsoft.Extensions.DependencyInjection;

namespace DIPS.FluentDbTools.Migration.Postgres
{
    public static class ServiceRegistration
    {
        public static IServiceCollection Register(IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddScopedIfNotExists<IExtendedMigrationGenerator<ExtendedPostgresGenerator>, ExtendedPostgresGenerator>()
                .AddExtendedPostgres();
        }

        public static IServiceCollection AddExtendedPostgres(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .IfNotExistThen<PostgresQuoter>(() => serviceCollection.ConfigureRunner(builder => builder.AddPostgres()))
                .IfNotExistThen<IExtendedMigrationProcessor<ExtendedPostgresProcessor>>(() =>
                    serviceCollection
                        .AddScoped<ExtendedPostgresProcessor>()
                        .AddScoped<IExtendedMigrationProcessor<ExtendedPostgresProcessor>>(sp => sp.GetRequiredService<ExtendedPostgresProcessor>())
                        .AddScoped<IMigrationProcessor>(sp => sp.GetRequiredService<ExtendedPostgresProcessor>())
                        .AddScoped<PostgresQuoter, OverriddenPostgresQouter>()
                        .AddScoped<PostgresTypeMap>()
                        .AddScoped<OverriddenPostgresColumn>()
                        .AddScoped<PostgresGenerator, OverriddenPostgresGenerator>()
                );

            return serviceCollection;

        }

    }
}
