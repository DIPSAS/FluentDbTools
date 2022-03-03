using FluentDbTools.Extensions.MSDependencyInjection;
using FluentDbTools.Extensions.MSDependencyInjection.Postgres;
using FluentDbTools.Migration.Abstractions;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Generators.Postgres;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
#pragma warning disable CS1591

namespace FluentDbTools.Migration.Postgres
{
    public static class ServiceRegistration
    {
        public static IServiceCollection Register(IServiceCollection serviceCollection)
        {
            serviceCollection
                .TryAddScoped<IExtendedMigrationGenerator<ExtendedPostgresGenerator>, ExtendedPostgresGenerator>();
            return serviceCollection.AddExtendedPostgres();
        }

        public static IServiceCollection AddExtendedPostgres(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .IfNotExistThen<PostgresQuoter>(() => serviceCollection.ConfigureRunner(builder => builder.AddPostgres()))
                .IfNotExistThen<IExtendedMigrationProcessor<ExtendedPostgresProcessor>>(() =>
                    serviceCollection
                        .AddScoped<ExtendedPostgresProcessor>()
                        .AddScoped<IExtendedMigrationProcessor<ExtendedPostgresProcessor>>(sp => sp.GetRequiredService<ExtendedPostgresProcessor>())
                        .AddScoped<OverridenPostgresDescriptionGenerator>()
                        .AddScoped<PostgresQuoter, OverriddenPostgresQouter>()
                        .AddScoped<PostgresTypeMap>()
                        .AddScoped<OverriddenPostgresColumn>()
                        .AddScoped<PostgresGenerator, OverriddenPostgresGenerator>()
                        .AddPostgresDbProvider()
                );

            return serviceCollection;

        }

    }
}
