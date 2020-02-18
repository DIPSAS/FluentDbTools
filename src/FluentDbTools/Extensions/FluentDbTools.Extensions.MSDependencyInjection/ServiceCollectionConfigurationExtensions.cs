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
        /// <param name="asSingleton"></param>
        /// <returns></returns>
        public static IServiceCollection AddDefaultDbConfig(this IServiceCollection serviceProvider, bool asSingleton = true)
        {
            if (asSingleton)
            {
                serviceProvider.TryAddSingleton<DefaultDbConfigValues, MsDefaultDbConfigValues>();
                serviceProvider.TryAddSingleton<DbConfigCredentials, MsDbConfigCredentials>();
                serviceProvider.TryAddSingleton<IConfigurationChangedHandler, MsConfigurationChangedHandler>();
                serviceProvider.TryAddSingleton<IDbConfig, MsDbConfig>();
            }
            else
            {
                serviceProvider.TryAddScoped<DefaultDbConfigValues, MsDefaultDbConfigValues>();
                serviceProvider.TryAddScoped<DbConfigCredentials, MsDbConfigCredentials>();
                serviceProvider.TryAddScoped<IConfigurationChangedHandler, MsConfigurationChangedHandler>();
                serviceProvider.TryAddScoped<IDbConfig, MsDbConfig>();
            }

            return serviceProvider.AddDbConfigDatabaseTargets();
        }

        public static IServiceCollection AddDbConfig(this IServiceCollection serviceProvider, Type dbConfigType, bool asSingleton = true)
        {
            serviceProvider.RemoveAll<IDbConfig>();
            
            if (asSingleton)
            {
                serviceProvider.AddSingleton(typeof(IDbConfig), dbConfigType);
            }
            else
            {
                serviceProvider.AddScoped(typeof(IDbConfig), dbConfigType);
            }

            return serviceProvider.AddDbConfigDatabaseTargets();

        }

        public static IServiceCollection AddDbConfig<TDbConfig>(this IServiceCollection serviceProvider, bool asSingleton = true) where TDbConfig : class, IDbConfig
        {
            serviceProvider.RemoveAll<IDbConfig>();
            
            if (asSingleton)
            {
                serviceProvider.AddSingleton<IDbConfig, TDbConfig>();
            }
            else
            {
                serviceProvider.AddScoped<IDbConfig, TDbConfig>();
            }
            return serviceProvider.AddDbConfigDatabaseTargets();

        }

        public static IServiceCollection AddDbConfig<TDbConfig>(this IServiceCollection serviceProvider, TDbConfig impl, bool asSingleton = true) where TDbConfig : class, IDbConfig
        {
            serviceProvider.RemoveAll<IDbConfig>();
            
            if (asSingleton)
            {
                serviceProvider.AddSingleton<IDbConfig>(sp => impl);
            }
            else
            {
                serviceProvider.AddScoped<IDbConfig>(sp => impl);
            }

            return serviceProvider.AddDbConfigDatabaseTargets();
        }

        public static IServiceCollection AddDbConfigDatabaseTargets(this IServiceCollection serviceProvider)
        {
            serviceProvider.TryAddTransient<IDbConfigDatabaseTargets>(sp => sp.GetRequiredService<IDbConfig>());
            serviceProvider.AddDbConfigSchemaTargets();
            return serviceProvider;
        }

        public static IServiceCollection AddDbConfigSchemaTargets(this IServiceCollection serviceProvider)
        {
            serviceProvider.TryAddTransient<IDbConfigSchemaTargets>(sp => sp.GetRequiredService<IDbConfig>());
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