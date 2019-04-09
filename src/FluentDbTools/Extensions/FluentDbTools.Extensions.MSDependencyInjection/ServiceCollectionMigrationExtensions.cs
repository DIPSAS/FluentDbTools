using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace FluentDbTools.Extensions.MSDependencyInjection
{
    public static class ServiceCollectionMigrationExtensions
    {
        public static IServiceCollection ConfigureWithMigrationAssemblies(this IServiceCollection serviceCollection, 
            Func<IServiceCollection, IEnumerable<Assembly>, IServiceCollection> func,
            IEnumerable<Assembly> assembliesWithMigrationModels)
        {
            return func.Invoke(serviceCollection, assembliesWithMigrationModels);
        }
    }
}