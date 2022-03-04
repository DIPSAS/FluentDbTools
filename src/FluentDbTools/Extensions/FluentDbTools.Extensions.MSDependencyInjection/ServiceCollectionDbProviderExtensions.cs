﻿using System;
using System.Data;
using System.Data.Common;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Extensions.DbProvider;
using FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
// ReSharper disable InconsistentNaming
// ReSharper disable ConstantNullCoalescingCondition
// ReSharper disable RedundantTypeArgumentsOfMethod
// ReSharper disable UnusedMember.Local

#pragma warning disable CS1591

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

        public static IServiceCollection AddDbProvider<TDbConfig>(
            this IServiceCollection serviceCollection,
            Action actionToRunIfDbFactoryIsNull = null,
            bool asSingleton = true, 
            bool openConnectionWhenRequest = true)
            where TDbConfig : class, IDbConfig
        {
            return serviceCollection
                .AddDbConfig<TDbConfig>(asSingleton)
                .AddDbProvider(actionToRunIfDbFactoryIsNull, asSingleton, openConnectionWhenRequest);
        }

        public static IServiceCollection AddDbProvider(this
            IServiceCollection serviceCollection,
            Type dbConfigType,
            Action actionToRunIfDbFactoryIsNull = null,
            bool asSingleton = true, 
            bool openConnectionWhenRequest = true)
        {
            return serviceCollection
                .AddDbConfig(dbConfigType, asSingleton)
                .AddDbProvider(actionToRunIfDbFactoryIsNull, asSingleton, openConnectionWhenRequest);
        }
        
        public static IServiceCollection AddDbProvider(this IServiceCollection serviceCollection,
            Action actionToRunIfDbFactoryIsNull = null, bool asSingleton = true, bool openConnectionWhenRequest = true)
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
                .AddDbConnection(actionToRunIfDbFactoryIsNull, openConnectionWhenRequest)
                .AddDbTransaction();
        }

        static IServiceCollection AddDbConnection(this IServiceCollection serviceCollection, Action actionToRunIfDbFactoryIsNull = null, bool openConnectionWhenRequest = true)
        {
            serviceCollection.TryAddScoped<IDbConnection>(sp =>
            {

                var dbProviderFactory = sp.GetService<DbProviderFactory>();
                if (dbProviderFactory == null)
                {

                    if (actionToRunIfDbFactoryIsNull != null)
                    {
                        actionToRunIfDbFactoryIsNull();
                        dbProviderFactory = sp.GetService<DbProviderFactory>();
                    }
                }

                if (dbProviderFactory == null)
                {
                    throw new NullReferenceException(
                        $"{nameof(DbProviderFactory)} from container cannot be null. Please register DbProviderFactory! \n" +
                        "ie: OracleClientFactory.Instance.Register(SupportedDatabaseTypes.Oracle); or \n    NpgsqlFactory.Instance.Register(SupportedDatabaseTypes.Postgres)");
                }


                var dbConnection = dbProviderFactory.CreateConnection() ?? sp.GetDbConfig()?.CreateDbConnection();

                if (dbConnection == null)
                {
                    throw new NullReferenceException(
                        $"{nameof(IDbConnection)} provided by the db provider factory cannot be null!");
                }

                try
                {
                    if (openConnectionWhenRequest)
                    {
                        dbConnection.SafeOpen();
                    }
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

        /// <summary>
        /// Open the <paramref name="dbConnection"/> only if it isn't already opened
        /// </summary>
        /// <param name="dbConnection"></param>
        public static void SafeOpen(this IDbConnection dbConnection)
        {
            if (dbConnection.State != ConnectionState.Open)
            {
                dbConnection.Open();
            }
        }

        /// <summary>
        /// Close the <paramref name="dbConnection"/> only if it's opened
        /// </summary>
        /// <param name="dbConnection"></param>
        public static void SafeClose(this IDbConnection dbConnection)
        {
            if (dbConnection.State == ConnectionState.Open)
            {
                dbConnection.Close();
            }
        }

    }
}