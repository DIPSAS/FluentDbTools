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
using FluentMigrator.Runner.VersionTableInfo;
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
                        .AddScoped(CreateExtendedOracleProcessorBase)
                        .AddScoped<IExtendedMigrationProcessor<ExtendedOracleProcessorBase>>(sp => sp.GetRequiredService<ExtendedOracleProcessorBase>())
                        .AddScoped<IExtendedMigrationProcessorOracle>(sp => sp.GetRequiredService<ExtendedOracleProcessorBase>())
                        .AddScoped<OracleManagedProcessor>(sp => sp.GetRequiredService<ExtendedOracleManagedProcessor>())
                        .AddScoped<OracleProcessorBase>(sp => sp.GetRequiredService<ExtendedOracleManagedProcessor>())
                        .AddScoped<ExtendedOracleManagedProcessor>()
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
                sp.GetService<ILogger<ExtendedOracleProcessorBase>>(),
                sp.GetRequiredService<IOptionsSnapshot<ProcessorOptions>>(),
                sp.GetRequiredService<IOptionsSnapshot<RunnerOptions>>(),
                sp.GetRequiredService<IConnectionStringAccessor>(),
                sp.GetRequiredService<IExtendedMigrationGenerator<ExtendedOracleMigrationGenerator>>(),
                sp.GetService<IDbMigrationConfig>(), 
                                    sp.GetService<IMigrationSourceItem>(),
                sp.GetService<IVersionTableMetaData>());
        }
    }
}