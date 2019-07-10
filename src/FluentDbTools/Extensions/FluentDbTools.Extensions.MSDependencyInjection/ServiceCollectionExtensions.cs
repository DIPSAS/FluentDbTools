using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

[assembly: InternalsVisibleTo("FluentDbTools.Migration")]
[assembly: InternalsVisibleTo("FluentDbTools.Migration.Oracle")]
[assembly: InternalsVisibleTo("FluentDbTools.Migration.Postgres")]
namespace FluentDbTools.Extensions.MSDependencyInjection
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection Register(this IServiceCollection serviceCollection,
            Func<IServiceCollection, IServiceCollection> func)
        {
            return func.Invoke(serviceCollection);
        } 
        
        public static bool Exists<T>(this IServiceCollection serviceCollection)
        {
            return GetFirstServiceDescriptor<T>(serviceCollection) != null;
        }

        public static IServiceCollection IfExistThen<T>(this IServiceCollection serviceCollection, Action thenAction)
        {
            if (serviceCollection.Exists<T>())
            {
                thenAction.Invoke();
            }

            return serviceCollection;
        }
        
        public static IServiceCollection IfNotExistThen<T>(this IServiceCollection serviceCollection, Action thenAction)
        {
            if (!serviceCollection.Exists<T>())
            {
                thenAction.Invoke();
            }

            return serviceCollection;
        }
        
        public static IServiceCollection AddScopedIfNotExists<T>(this IServiceCollection serviceCollection)
            where T : class
        {
            serviceCollection.TryAddScoped<T>();
            return serviceCollection;
        }

        public static ServiceDescriptor GetFirstServiceDescriptor<T>(this IServiceCollection serviceCollection)
        {
            var type = typeof(T);
            return serviceCollection.FirstOrDefault(x => x.ServiceType == type || x.ImplementationType == type);
        }

        public static T GetImplementation<T>(this ServiceDescriptor descriptor, IServiceScope serviceScope = null) where T:class
        {
            if (descriptor == null)
            {
                return default(T);
            }

            if (descriptor.ImplementationInstance != null && descriptor.ImplementationInstance is T instance)
            {
                return instance;
            }

            if (serviceScope == null)
            {
                return default(T);
            }

            return (descriptor.ImplementationFactory?.Invoke(serviceScope.ServiceProvider) as T) ?? default(T);
        }
    }
}