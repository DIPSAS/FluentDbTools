using System.Collections.Generic;
using FluentDbTools.Extensions.MSDependencyInjection;
using FluentDbTools.Example.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FluentDbTools.Example.Common
{
    public static class ExampleConfigurationExtensions
    {
        public static IServiceCollection UseExampleConfiguration(this IServiceCollection serviceCollection, Dictionary<string, string> overrideConfig = null, string additionalJsonConfig = null)
        {
            return serviceCollection
                .AddScoped<IConfiguration>(serviceProvider => new ConfigurationBuilder()
                .AddDbToolsExampleConfiguration()
                .AddJsonFileIfTrue(additionalJsonConfig, () => additionalJsonConfig != null)
                .AddInMemoryIfTrue(overrideConfig, () => overrideConfig != null)
                .Build());
        }
        
        private static IConfigurationBuilder AddDbToolsExampleConfiguration(this IConfigurationBuilder configurationBuilder)
        {
            return configurationBuilder
                .AddJsonFile(BaseConfig.DefaultConfigFilename)
                .AddJsonFileIfTrue(BaseConfig.DefaultConfigDockerFilename, () => BaseConfig.InContainer)
                .AddInMemoryIfTrue(BaseConfig.ExternalServiceHostConfiguration, () => BaseConfig.UseExternalServiceHost);
        }
    }
}