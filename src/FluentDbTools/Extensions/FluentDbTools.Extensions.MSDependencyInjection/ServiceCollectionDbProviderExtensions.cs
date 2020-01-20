using System;
using System.Data;
using System.Data.Common;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Extensions.DbProvider;
using FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FluentDbTools.Extensions.MSDependencyInjection
{
    public static class ServiceCollectionDbProviderExtensions
    {
        public static Type GetDependecyInjectionDbConfigType(this IServiceCollection _)
        {
            return GetDependecyInjectionDbConfigType();
        }

        public static Type GetDependecyInjectionDbConfigType()
        {
            return typeof(MsDbConfig);
        }

        public static IServiceCollection AddDbProvider<TDbConfig>(this IServiceCollection serviceCollection)
            where TDbConfig : class, IDbConfig
        {
            return serviceCollection
                .AddDbConfig<TDbConfig>()
                .AddDbProvider();
        }

        public static IServiceCollection AddDbProvider(this IServiceCollection serviceCollection, Type dbConfigType)
        {
            return serviceCollection
                .AddDbConfig(dbConfigType)
                .AddDbProvider();
        }

        public static IServiceCollection AddDbProvider(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddDbConfigDatabaseTargets();
            serviceCollection.TryAddScoped(sp => sp.GetRequiredService<IDbConfig>().GetDbProviderFactory());
            serviceCollection.IfExistThen<DbConnection>(() => serviceCollection.TryAddScoped<IDbConnection>(sp => sp.GetRequiredService<DbConnection>()));
            serviceCollection.TryAddScoped<IDbConnection>(sp =>
            {
                var dbConnection = sp.GetRequiredService<DbProviderFactory>().CreateConnection();

                if (dbConnection == null)
                {
                    throw new NullReferenceException(
                        $"{nameof(IDbConnection)} provided by the db provider factory cannot be null!");
                }

                try
                {
                    dbConnection.Open();
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Exception:" + exception.Message);
                    throw;
                }
                return dbConnection;
            });
            serviceCollection.TryAddScoped(sp =>
                sp.GetRequiredService<IDbConnection>().BeginTransaction());

            return serviceCollection;
        }
    }
}