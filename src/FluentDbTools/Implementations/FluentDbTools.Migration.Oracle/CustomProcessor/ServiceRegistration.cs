using FluentDbTools.Migration.Abstractions;
using FluentMigrator.Runner.Processors.Oracle;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FluentDbTools.Migration.Oracle.CustomProcessor
{
    /// <summary>
    /// Service registration for <see cref="DefaultOracleCustomMigrationProcessor"/>
    /// </summary>
    public static class ServiceRegistration
    {
        /// <summary>
        /// Register <see cref="ICustomMigrationProcessor&lt;OracleProcessor&gt;"/>  with implementation <see cref="DefaultOracleCustomMigrationProcessor"/>
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterCustomMigrationProcessor(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddScoped<ICustomMigrationProcessor<OracleProcessor>,DefaultOracleCustomMigrationProcessor>();
            return serviceCollection;
        }
        
    }
}