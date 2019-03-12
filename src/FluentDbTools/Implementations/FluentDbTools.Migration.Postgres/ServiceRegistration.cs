using FluentDbTools.Extensions.MSDependencyInjection;
using FluentDbTools.Migration.Abstractions;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Generators.Generic;
using FluentMigrator.Runner.Generators.Postgres;
using Microsoft.Extensions.DependencyInjection;

namespace FluentDbTools.Migration.Postgres
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
                        .AddScoped<OverridenPostgresDescriptionGenerator>()
                        .AddScoped<PostgresQuoter, OverriddenPostgresQouter>()
                        .AddScoped<PostgresTypeMap>()
                        .AddScoped<OverriddenPostgresColumn>()
                        .AddScoped<PostgresGenerator, OverriddenPostgresGenerator>()
                );

            return serviceCollection;

        }

    }
}
