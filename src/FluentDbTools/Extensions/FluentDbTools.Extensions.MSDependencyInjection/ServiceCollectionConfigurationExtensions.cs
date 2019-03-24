using System;
using System.Collections.Generic;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FluentDbTools.Extensions.MSDependencyInjection
{
    public static class ServiceCollectionConfigurationExtensions
    {      
        public static IConfigurationBuilder AddJsonFileIfTrue(this IConfigurationBuilder configurationBuilder,
            string path,
            Func<bool> func)
        {
            if (func.Invoke())
            {
                configurationBuilder.AddJsonFile(path);
            }
            return configurationBuilder;
        }
        
        public static IConfigurationBuilder AddInMemoryIfTrue(this IConfigurationBuilder configurationBuilder,
            Dictionary<string, string> dict,
            Func<bool> func)
        {
            if (func.Invoke())
            {
                configurationBuilder.AddInMemoryCollection(dict);
            }
            return configurationBuilder;
        }
        
        public static IConfigurationBuilder AddConfigurationIfTrue(this IConfigurationBuilder configurationBuilder,
            IConfiguration configuration,
            Func<bool> func)
        {
            if (func.Invoke())
            {
                configurationBuilder.AddConfiguration(configuration);
            }
            return configurationBuilder;
        }

        public static IServiceCollection AddDefultDbConfig(this IServiceCollection serviceProvider)
        {
            return serviceProvider
                .AddScopedIfNotExists<IDbConfig, MsDbConfig>()
                .AddDbConfigDatabaseTargets();
        }

        public static IServiceCollection AddDbConfig<TDbConfig>(this IServiceCollection serviceProvider) where TDbConfig: class, IDbConfig
        {
            return serviceProvider
                .Remove<TDbConfig>()
                .Remove<IDbConfigDatabaseTargets>()
                .AddScoped<IDbConfig, TDbConfig>()
                .AddDbConfigDatabaseTargets();

        }
        public static IServiceCollection AddDbConfig<TDbConfig>(this IServiceCollection serviceProvider, TDbConfig impl) where TDbConfig : class, IDbConfig
        {
            return serviceProvider
                .Remove<TDbConfig>()
                .Remove<IDbConfigDatabaseTargets>()
                .AddScoped<IDbConfig>(sp => impl)
                .AddDbConfigDatabaseTargets();
        }
        public static IServiceCollection AddDbConfigDatabaseTargets(this IServiceCollection serviceProvider)
        {
            return serviceProvider
                .AddTransientIfNotExists<IDbConfigDatabaseTargets>(sp => sp.GetRequiredService<IDbConfig>());
        }

        public static IDbConfig GetDbConfig(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetRequiredService<IDbConfig>();
        }

    }
}