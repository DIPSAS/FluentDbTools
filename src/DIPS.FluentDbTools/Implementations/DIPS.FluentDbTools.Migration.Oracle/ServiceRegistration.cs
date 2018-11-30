using System;
using DIPS.Extensions.FluentDbTools.MSDependencyInjection;
using DIPS.FluentDbTools.Common.Abstractions;
using DIPS.FluentDbTools.Migration.Abstractions;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Generators.Oracle;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Processors.Oracle;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DIPS.FluentDbTools.Migration.Oracle
{
    public static class ServiceRegistration
    {
        public static IServiceCollection Register(IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddScopedIfNotExists<IExtendedMigrationGenerator<ExtendedOracleMigrationGenerator>, ExtendedOracleMigrationGenerator>()
                .AddExtendedOracleManaged();
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
                );


            return serviceCollection;

        }

        private static ExtendedOracleProcessorBase CreateExtendedOracleProcessorBase(IServiceProvider sp)
        {
            return new ExtendedOracleProcessorBase(
                sp.GetRequiredService<IDbConfig>(),
                sp.GetRequiredService<OracleManagedDbFactory>(),
                sp.GetRequiredService<OracleGenerator>(),
                sp.GetRequiredService<ILogger<ExtendedOracleManagedProcessor>>(),
                sp.GetRequiredService<IOptionsSnapshot<ProcessorOptions>>(),
                sp.GetRequiredService<IConnectionStringAccessor>(),
                sp.GetRequiredService<IExtendedMigrationGenerator<ExtendedOracleMigrationGenerator>>());
        }
    }
}