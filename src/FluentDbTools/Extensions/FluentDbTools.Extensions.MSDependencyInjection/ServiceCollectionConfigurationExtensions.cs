using System;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FluentDbTools.Extensions.MSDependencyInjection
{
    public static class ServiceCollectionConfigurationExtensions
    {      
        public static IServiceCollection AddDefaultDbConfig(this IServiceCollection serviceProvider)
        {
            serviceProvider.TryAddScoped<IDbConfig, MsDbConfig>();
            return serviceProvider.AddDbConfigDatabaseTargets();
        }

        public static IServiceCollection AddDbConfig<TDbConfig>(this IServiceCollection serviceProvider) where TDbConfig: class, IDbConfig
        {
            return serviceProvider
                .AddScoped<IDbConfig, TDbConfig>()
                .AddDbConfigDatabaseTargets();

        }
        public static IServiceCollection AddDbConfig<TDbConfig>(this IServiceCollection serviceProvider, TDbConfig impl) where TDbConfig : class, IDbConfig
        {
            return serviceProvider
                .AddScoped<IDbConfig>(sp => impl)
                .AddDbConfigDatabaseTargets();
        }
        
        public static IServiceCollection AddDbConfigDatabaseTargets(this IServiceCollection serviceProvider)
        {
            serviceProvider.TryAddTransient<IDbConfigDatabaseTargets>(sp => sp.GetRequiredService<IDbConfig>());
            return serviceProvider;
        }

        public static IDbConfig GetDbConfig(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetRequiredService<IDbConfig>();
        }

    }
}