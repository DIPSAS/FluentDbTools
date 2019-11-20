using System;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Contracts;
using FluentDbTools.Extensions.DbProvider;
using FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FluentDbTools.Extensions.MSDependencyInjection
{
    /// <summary>
    /// DependencyInjection extension methods
    /// </summary>
    public static class ServiceCollectionConfigurationExtensions
    {      
        /// <summary>
        /// Register the DependencyInjection implementation of <see cref="IDbConfig"/>(Strong-type Ms<see cref="DbConfig"/>)<br/>
        /// Register the DependencyInjection implementation of <see cref="IConfigurationChangedHandler"/> 
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static IServiceCollection AddDefaultDbConfig(this IServiceCollection serviceProvider)
        {
            serviceProvider.TryAddScoped<DefaultDbConfigValues, MsDefaultDbConfigValues>();
            serviceProvider.TryAddScoped<DbConfigCredentials, MsDbConfigCredentials>();
            serviceProvider.TryAddScoped<IConfigurationChangedHandler, MsConfigurationChangedHandler>();
            serviceProvider.TryAddScoped<IDbConfig, MsDbConfig>();
            return serviceProvider.AddDbConfigDatabaseTargets();
        }

        public static IServiceCollection AddDbConfig(this IServiceCollection serviceProvider, Type dbConfigType) 
        {
            return serviceProvider
                .AddScoped(typeof(IDbConfig), dbConfigType)
                .AddDbConfigDatabaseTargets();

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

        /// <summary>
        /// Resolve the registered implementation of <see cref="IDbConfig"/> from the DependencyInjection container <see cref="IServiceProvider"/>
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static IDbConfig GetDbConfig(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetRequiredService<IDbConfig>();
        }
    }
}