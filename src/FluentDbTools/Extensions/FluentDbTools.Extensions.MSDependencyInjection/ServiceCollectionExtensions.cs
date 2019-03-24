using System;
using System.Linq;
using FluentDbTools.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FluentDbTools.Extensions.MSDependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection Register(this IServiceCollection serviceCollection,
            Func<IServiceCollection, IServiceCollection> func)
        {
            return func.Invoke(serviceCollection);
        } 
        
        public static bool Exists<T>(this IServiceCollection serviceCollection)
        {
            return serviceCollection.GetServiceRegistration<T>() != null;
        }

        public static IServiceCollection IfExistThen<T>(this IServiceCollection serviceCollection, Action thenAction)
        {
            return serviceCollection.IfTrueThen(serviceCollection.Exists<T>, thenAction);
        }

        public static IServiceCollection IfNotExistThen<T>(this IServiceCollection serviceCollection, Action thenAction)
        {
            return serviceCollection.IfFalseThen(serviceCollection.Exists<T>, thenAction);
        }

        public static IServiceCollection IfTrueThen(this IServiceCollection serviceCollection, Func<bool> testAction, Action thenAction)
        {
            if (testAction?.Invoke() ?? false)
            {
                thenAction?.Invoke();
            }

            return serviceCollection;
        }

        public static IServiceCollection IfFalseThen(this IServiceCollection serviceCollection, Func<bool> testAction, Action thenAction)
        {
            if (!(testAction?.Invoke() ?? false))
            {
                thenAction?.Invoke();
            }

            return serviceCollection;
        }

        public static IServiceCollection Remove<T>(this IServiceCollection serviceCollection)
        {
            var found = GetServiceRegistration<T>(serviceCollection);
            while (found != null)
            {
                serviceCollection.Remove(found);
                found = GetServiceRegistration<T>(serviceCollection);
            }
            return serviceCollection;
        }

        public static ServiceDescriptor GetServiceRegistration<T>(this IServiceCollection serviceCollection)
        {
            var type = typeof(T);
            return serviceCollection.FirstOrDefault(x => x.ServiceType == type || x.ImplementationType == type);
        }

        public static IServiceCollection AddSingletonIfNotExists<T>(this IServiceCollection serviceCollection)
            where T : class
        {
            serviceCollection.TryAddSingleton<T>();
            return serviceCollection;
        }


        public static IServiceCollection AddSingletonIfNotExists<T, TImpl>(this IServiceCollection serviceCollection)
            where T : class
            where TImpl : class, T
        {
            serviceCollection.TryAddSingleton<T,TImpl>();
            return serviceCollection;
        }

        public static IServiceCollection AddSingletonIfNotExists<T, TImpl>(this IServiceCollection serviceCollection, TImpl implent)
            where T : class
            where TImpl : class, T
        {
            serviceCollection.TryAddSingleton<T>(implent);
            return serviceCollection;
        }

        public static IServiceCollection AddSingletonIfNotExists<T>(this IServiceCollection serviceCollection, Func<IServiceProvider, T> implentFunc)
            where T : class
        {
            serviceCollection.TryAddSingleton(implentFunc);
            return serviceCollection;
        }

        public static IServiceCollection AddScopedIfNotExists<T>(this IServiceCollection serviceCollection)
            where T : class
        {
            serviceCollection.TryAddScoped<T>();
            return serviceCollection;
        }

        public static IServiceCollection AddScopedIfNotExists<T, TImpl>(this IServiceCollection serviceCollection)
            where T : class
            where TImpl : class, T
        {
            serviceCollection.TryAddScoped<T,TImpl>();
            return serviceCollection;
        }
        
        public static IServiceCollection AddScopedIfNotExists<T>(this IServiceCollection serviceCollection, Func<IServiceProvider, T> implentFunc)
            where T : class
        {
            serviceCollection.TryAddScoped(implentFunc);
            return serviceCollection;
        }

        public static IServiceCollection AddTransientIfNotExists<T>(this IServiceCollection serviceCollection)
            where T : class
        {
            serviceCollection.TryAddTransient<T>();
            return serviceCollection;
        }

        public static IServiceCollection AddTransientIfNotExists<T, TImpl>(this IServiceCollection serviceCollection)
            where T : class
            where TImpl : class, T
        {
            serviceCollection.TryAddTransient<T,TImpl>();
            return serviceCollection;
        }


        public static IServiceCollection AddTransientIfNotExists<T>(this IServiceCollection serviceCollection, Func<IServiceProvider, T> implentFunc)
            where T : class
        {
            serviceCollection.TryAddTransient(implentFunc);
            return serviceCollection;
        }
    }
}