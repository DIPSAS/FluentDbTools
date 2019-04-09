using System.Collections.Generic;
using System.IO;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Extensions.DbProvider;
using FluentDbTools.Extensions.MSDependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Example.FluentDbTools.Config
{
    public static class ExampleConfigurationExtensions
    {
        public static IServiceCollection UseExampleConfiguration(this IServiceCollection serviceCollection, 
            SupportedDatabaseTypes databaseType,
            Dictionary<string, string> overrideConfig = null)
        {
            return serviceCollection
                .AddScoped<IConfiguration>(serviceProvider => new ConfigurationBuilder()
                .AddDbToolsExampleConfiguration(databaseType)
                .AddInMemoryIfTrue(overrideConfig, () => overrideConfig != null)
                .Build());
        }
        
        public static IConfigurationBuilder AddDbToolsExampleConfiguration(this IConfigurationBuilder configurationBuilder, 
            SupportedDatabaseTypes databaseType)
        {
            DbProviderExtensions.ConfigureOracleTnsAdminPath(Path.Combine("oracle", "network", "admin"));
            return configurationBuilder
                .AddJsonFile(BaseConfig.ConfigFilename(databaseType))
                .AddJsonFileIfTrue(BaseConfig.ConfigDockerFilename(databaseType), () => BaseConfig.InContainer)
                .AddInMemoryIfTrue(BaseConfig.ExternalServiceHostConfiguration, () => BaseConfig.UseExternalServiceHost);
        }
    }
}