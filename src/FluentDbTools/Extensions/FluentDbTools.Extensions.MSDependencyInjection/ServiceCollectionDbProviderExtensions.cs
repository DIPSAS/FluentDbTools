using System;
using System.Data;
using System.Data.Common;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Extensions.DbProvider;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FluentDbTools.Extensions.MSDependencyInjection
{
    public static class ServiceCollectionDbProviderExtensions
    {
        public static IServiceCollection AddDbProvider<TDbConfig>(this IServiceCollection serviceCollection)
            where  TDbConfig : class, IDbConfig
        {
            serviceCollection.TryAddTransient<IDbConfig, TDbConfig>();
            return serviceCollection.AddDbProvider();
        }

        public static IServiceCollection AddDbProvider(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton(sp =>
                sp.GetRequiredService<IDbConfig>().GetDbProviderFactory());
            serviceCollection.IfExistThen<DbConnection>(() => serviceCollection.TryAddScoped<IDbConnection>(sp => sp.GetRequiredService<DbConnection>()));
            serviceCollection.TryAddScoped<IDbConnection>(sp =>
            {
                var dbConnection = sp.GetRequiredService<DbProviderFactory>().CreateConnection();

                if (dbConnection == null)
                {
                    throw new NullReferenceException(
                        $"{nameof(IDbConnection)} provided by the db provider factory cannot be null!");
                }
                
                dbConnection.Open();
                return dbConnection;
            });
            serviceCollection.TryAddScoped(sp =>
                sp.GetRequiredService<IDbConnection>().BeginTransaction());

            return serviceCollection;
        }
    }
}