using System.Collections.Generic;
using System.Reflection;
using DIPS.Extensions.FluentDbTools.DbProvider;
using DIPS.FluentDbTools.Common.Abstractions;
using DIPS.FluentDbTools.Migration.Common;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DIPS.FluentDbTools.Migration
{
    internal static class FluentMigrationConfigurationExtensions
    {
        public static IServiceCollection ConfigureFluentMigrationWithDatabaseType(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddScoped<IOptionsSnapshot<ProcessorOptions>>(sp =>
                {
                    var factory = sp.GetService<IOptionsFactory<ProcessorOptions>>();
                    var manager = new OptionsManager<ProcessorOptions>(factory);
                    var dbConfig = sp.GetService<IDbConfig>();
                    manager.Value.ConnectionString = dbConfig.GetAdminConnectionString();
                    return manager;
                })
                .AddScoped<IOptions<ProcessorOptions>>(sp =>
                {
                    var options = new OptionsWrapper<ProcessorOptions>(new ProcessorOptions
                    {
                        ConnectionString =
                            sp.GetService<IDbConfig>().GetAdminConnectionString()
                    });
                    return options;
                })
                .AddScoped<IOptionsSnapshot<SelectingProcessorAccessorOptions>>(sp =>
                {
                    var factory = sp.GetService<IOptionsFactory<SelectingProcessorAccessorOptions>>();
                    var optionsManager = new OptionsManager<SelectingProcessorAccessorOptions>(factory);
                    var databaseType = sp.GetService<IDbConfig>().DbType;
                    optionsManager.Value.ProcessorId = databaseType.GetProcessorId();
                    return optionsManager;
                })
                .AddScoped<IOptions<SelectingProcessorAccessorOptions>>(sp =>
                {
                    var options = new OptionsWrapper<SelectingProcessorAccessorOptions>(
                        new SelectingProcessorAccessorOptions()
                        {
                            ProcessorId = sp.GetService<IDbConfig>().DbType.GetProcessorId()
                        });
                    return options;
                });
        }
        
        public static IScanInBuilder AssembliesForMigrations(this IScanInBuilder scanInBuilder, IEnumerable<Assembly> assembliesWithMigrationModels)
        {
            foreach (var assemblyWithMigrationModels in assembliesWithMigrationModels)
            {
                scanInBuilder.ScanIn(assemblyWithMigrationModels).For.Migrations();
            }
            return scanInBuilder;
        }
    }
}