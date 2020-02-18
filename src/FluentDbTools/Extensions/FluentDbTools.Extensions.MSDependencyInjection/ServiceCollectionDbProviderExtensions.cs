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

        public static IServiceCollection AddDbProvider<TDbConfig>(this IServiceCollection serviceCollection, bool asSingleton = true)
            where TDbConfig : class, IDbConfig
        {
            return serviceCollection
                .AddDbConfig<TDbConfig>(asSingleton)
                .AddDbProvider(asSingleton);
        }

        public static IServiceCollection AddDbProvider(this IServiceCollection serviceCollection, Type dbConfigType, bool asSingleton = true)
        {
            return serviceCollection
                .AddDbConfig(dbConfigType, asSingleton)
                .AddDbProvider(asSingleton);
        }

        public static IServiceCollection AddDbProvider(this IServiceCollection serviceCollection, bool asSingleton = true)
        {
            serviceCollection.AddDbConfigDatabaseTargets();
            if (asSingleton)
            {
                serviceCollection.TryAddSingleton(sp => sp.GetDbConfig()?.GetDbProviderFactory());
            }
            else
            {
                serviceCollection.TryAddScoped(sp => sp.GetDbConfig()?.GetDbProviderFactory());

            }
            serviceCollection.IfExistThen<DbConnection>(() => serviceCollection.TryAddScoped<IDbConnection>(sp => sp.GetRequiredService<DbConnection>()));

            return serviceCollection
                .AddDbConnection()
                .AddDbTransaction();
        }

        static IServiceCollection AddDbConnection(this IServiceCollection serviceCollection, Action ActionToRunIfDbFactoryIsNull = null)
        {
            serviceCollection.TryAddScoped<IDbConnection>(sp =>
            {

                var dbProviderFactory = sp.GetService<DbProviderFactory>();
                if (dbProviderFactory == null)
                {
                    
                    if (ActionToRunIfDbFactoryIsNull != null)
                    {
                        ActionToRunIfDbFactoryIsNull();
                        dbProviderFactory = sp.GetService<DbProviderFactory>();
                    }
                }


                if (dbProviderFactory == null)
                {
                    throw new NullReferenceException(
                        $"{nameof(DbProviderFactory)} from container cannot be null. Please register DbProviderFactory! \n" +
                        "ie: OracleClientFactory.Instance.Register(SupportedDatabaseTypes.Oracle); or \n    NpgsqlFactory.Instance.Register(SupportedDatabaseTypes.Postgres)");
                }


                var dbConnection = dbProviderFactory.CreateConnection();

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
            return serviceCollection;

        }

        static IServiceCollection AddDbTransaction(this IServiceCollection serviceCollection, Func<IDbConnection, IDbTransaction> transactionProviderFunc)
        {
            transactionProviderFunc = transactionProviderFunc ?? (connection => connection.BeginTransaction());

            serviceCollection.TryAddScoped(sp => transactionProviderFunc(sp.GetRequiredService<IDbConnection>()));
            return serviceCollection;
        }

        static IServiceCollection AddDbTransaction(this IServiceCollection serviceCollection, Func<IServiceProvider, IDbTransaction> transactionProviderFunc = null)
        {
            transactionProviderFunc = transactionProviderFunc ??
                                      (sp => sp.GetRequiredService<IDbConnection>().BeginTransaction());

            serviceCollection.TryAddScoped(transactionProviderFunc);
            return serviceCollection;

        }

    }
}