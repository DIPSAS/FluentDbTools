using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Common.Abstractions.PrioritizedConfig;
using FluentDbTools.Contracts;
using FluentDbTools.Extensions.DbProvider;
using FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
// ReSharper disable InconsistentNaming
#pragma warning disable CS1591

namespace FluentDbTools.Extensions.MSDependencyInjection
{
    /// <summary>
    /// DependencyInjection extension methods
    /// </summary>
    public static class ServiceCollectionConfigurationExtensions
    {
        private static readonly List<Assembly> PrioritizedConfigKeysAssemblies = new List<Assembly>();

        public static void AddPrioritizedConfigKeysAssembly(Assembly assembly)
        {
            if (PrioritizedConfigKeysAssemblies.Contains(assembly))
            {
                return;
            }
            PrioritizedConfigKeysAssemblies.Add(assembly);
        }

        public static void RemovePrioritizedConfigKeysAssembly(Assembly assembly)
        {
            if (PrioritizedConfigKeysAssemblies.Contains(assembly))
            {
                PrioritizedConfigKeysAssemblies.Remove(assembly);
            }
        }

        public static void ClearPrioritizedConfigKeysAssemblies(Assembly assembly)
        {
            PrioritizedConfigKeysAssemblies.Clear();
        }


        /// <summary>
        /// Register the DependencyInjection implementation of <see cref="IDbConfig"/>(Strong-type Ms<see cref="DbConfig"/>)<br/>
        /// Register the DependencyInjection implementation of <see cref="IConfigurationChangedHandler"/> 
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="asSingleton"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IServiceCollection AddDefaultDbConfig(this IServiceCollection serviceProvider, bool asSingleton = true,
            IEnumerable<Assembly> assemblies = null)
        {
            assemblies = assemblies ?? new [] { Assembly.GetEntryAssembly(), Assembly.GetCallingAssembly(), Assembly.GetExecutingAssembly() };
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

            serviceProvider.TryAddTransient<IDbUserGrantsConfig>(sp => sp.GetService<IDbConfig>());
            serviceProvider.AddPrioritizedConfigKeysRegistration(assemblies, asSingleton);

            return serviceProvider.AddDbConfigDatabaseTargets();
        }

        public static IServiceCollection AddDbConfig(this IServiceCollection serviceProvider, Type dbConfigType, bool asSingleton = true)
        {
            serviceProvider.RemoveAll<IDbConfig>();
            serviceProvider.RemoveAll<IDbUserGrantsConfig>();

            if (asSingleton)
            {
                serviceProvider.AddSingleton(typeof(IDbConfig), dbConfigType);
            }
            else
            {
                serviceProvider.AddScoped(typeof(IDbConfig), dbConfigType);
            }

            serviceProvider.TryAddTransient<IDbUserGrantsConfig>(sp => sp.GetService<IDbConfig>());

            serviceProvider.AddPrioritizedConfigKeysRegistration(asSingleton);

            return serviceProvider.AddDbConfigDatabaseTargets();

        }

        public static IServiceCollection AddDbConfig<TDbConfig>(this IServiceCollection serviceProvider, bool asSingleton = true) where TDbConfig : class, IDbConfig
        {
            serviceProvider.RemoveAll<IDbConfig>();
            serviceProvider.RemoveAll<IPrioritizedConfigKeys>();
            serviceProvider.RemoveAll<IDbUserGrantsConfig>();

            if (asSingleton)
            {
                serviceProvider.AddSingleton<IDbConfig, TDbConfig>();
            }
            else
            {
                serviceProvider.AddScoped<IDbConfig, TDbConfig>();
            }

            serviceProvider.TryAddTransient<IDbUserGrantsConfig>(sp => sp.GetService<IDbConfig>());
            serviceProvider.AddPrioritizedConfigKeysRegistration(asSingleton);

            return serviceProvider.AddDbConfigDatabaseTargets();

        }

        public static IServiceCollection AddDbConfig<TDbConfig>(this IServiceCollection serviceProvider, TDbConfig impl, bool asSingleton = true) where TDbConfig : class, IDbConfig
        {
            serviceProvider.RemoveAll<IDbConfig>();
            serviceProvider.RemoveAll<IDbUserGrantsConfig>();

            if (asSingleton)
            {
                serviceProvider.AddSingleton<IDbConfig>(sp => impl);
            }
            else
            {
                serviceProvider.AddScoped<IDbConfig>(sp => impl);
            }

            serviceProvider.TryAddTransient<IDbUserGrantsConfig>(sp => sp.GetService<IDbConfig>());
            serviceProvider.AddPrioritizedConfigKeysRegistration(asSingleton);

            return serviceProvider.AddDbConfigDatabaseTargets();
        }

        public static IServiceCollection AddDbConfigDatabaseTargets(this IServiceCollection serviceProvider)
        {
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
        /// <param name="required"></param>
        /// <returns></returns>
        public static IDbConfig GetDbConfig(this IServiceProvider serviceProvider, bool required = true)
        {
            return required ? serviceProvider.GetRequiredService<IDbConfig>() : serviceProvider.GetService<IDbConfig>();
        }

        public static IServiceCollection AddPrioritizedConfigKeysRegistration(
            this IServiceCollection serviceProvider, bool asSingleton = true)
        {
            var assemblies = new [] { Assembly.GetEntryAssembly(), Assembly.GetCallingAssembly(), Assembly.GetExecutingAssembly() };
            serviceProvider.AddPrioritizedConfigKeysRegistration(assemblies, asSingleton);
            return serviceProvider;
        }


        public static IServiceCollection AddPrioritizedConfigKeysRegistration(
            this IServiceCollection serviceCollection,
            IEnumerable<Assembly> assemblies,
            bool asSingleton = true)
        {
            assemblies = assemblies ?? new [] { Assembly.GetEntryAssembly(), Assembly.GetCallingAssembly(), Assembly.GetExecutingAssembly() };
            var list = assemblies.ToList();
            if (PrioritizedConfigKeysAssemblies.Any())
            {
                list.AddRange(PrioritizedConfigKeysAssemblies);
            }

            var prioritizedConfigKeysTypes = GetPrioritizedConfigKeysTypes(list);
            if (prioritizedConfigKeysTypes == null)
            {
                return serviceCollection;
            }

            foreach (var prioritizedConfigKeysType in prioritizedConfigKeysTypes.Distinct())
            {
                serviceCollection.RemoveIfExists(prioritizedConfigKeysType);

                if (asSingleton)
                {
                    serviceCollection.AddSingleton(typeof(IPrioritizedConfigKeys), prioritizedConfigKeysType);
                }
                else
                {
                    serviceCollection.AddScoped(typeof(IPrioritizedConfigKeys), prioritizedConfigKeysType);
                }                
            }
            

            return serviceCollection;
        }

        public static IEnumerable<Type> GetPrioritizedConfigKeysTypes(IEnumerable<Assembly> assemblies)
        {
            assemblies = assemblies ?? Enumerable.Empty<Assembly>();
            var searchForForInterfaceType = typeof(IPrioritizedConfigKeys);
            foreach (var assembliesWithMigrationModel in assemblies.Where(x => x != null).Distinct())
            {
                foreach (var type in assembliesWithMigrationModel.GetTypes())
                {
                    var foundType = !type.IsInterface && !type.IsAbstract && type.IsImplementingInterfaceType(searchForForInterfaceType);
                    if (!foundType)
                    {
                        continue;
                    }

                    yield return type;
                }
            }
        }

        internal static bool IsImplementingInterfaceType(this Type type, Type ofInterfaceType)
        {
            if (type == null || ofInterfaceType == null)
            {
                return false;
            }

            if (ofInterfaceType.IsAssignableFrom(type) || type.IsAssignableFrom(ofInterfaceType))
            {
                return true;
            }

            return type.GetInterface(ofInterfaceType.Name) != null;
        }


    }
}