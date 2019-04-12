using System;
using FluentDbTools.Extensions.MSDependencyInjection;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Extensions.MSDependencyInjection.Oracle;
using FluentDbTools.Migration.Abstractions;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Generators.Oracle;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Processors.Oracle;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentDbTools.Migration.Oracle
{
    public static class ServiceRegistration
    {
        public static IServiceCollection Register(IServiceCollection serviceCollection)
        {
            serviceCollection
                .TryAddScoped<IExtendedMigrationGenerator<ExtendedOracleMigrationGenerator>,
                    ExtendedOracleMigrationGenerator>();
            return serviceCollection.AddExtendedOracleManaged();
        }

        private static IServiceCollection AddExtendedOracleManaged(this IServiceCollection serviceCollection)
        {
            serviceCollection
                    .IfNotExistThen<OracleQuoterBase>(() => serviceCollection.ConfigureRunner(builder => builder.AddOracleManaged()))
                    .IfNotExistThen<IExtendedMigrationProcessor<ExtendedOracleProcessorBase>>(() =>
                        serviceCollection
                        .AddScoped<IExtendedMigrationProcessor<ExtendedOracleProcessorBase>>(CreateExtendedOracleProcessorBase)
                        .AddScoped<ExtendedOracleManagedProcessor>()
                        .AddScoped<IMigrationProcessor>(sp => sp.GetRequiredService<ExtendedOracleManagedProcessor>())
                        .AddScopedIfNotExists<OracleGenerator>()
                        .AddScopedIfNotExists<OracleManagedDbFactory>()
                        .AddOracleDbProvider()
                );


            return serviceCollection;

        }

        private static ExtendedOracleProcessorBase CreateExtendedOracleProcessorBase(IServiceProvider sp)
        {
            return new ExtendedOracleProcessorBase(sp.GetRequiredService<OracleManagedDbFactory>(),
                sp.GetRequiredService<OracleGenerator>(),
                sp.GetRequiredService<ILogger<ExtendedOracleManagedProcessor>>(),
                sp.GetRequiredService<IOptionsSnapshot<ProcessorOptions>>(),
                sp.GetRequiredService<IConnectionStringAccessor>(),
                sp.GetRequiredService<IExtendedMigrationGenerator<ExtendedOracleMigrationGenerator>>());
        }
    }
}