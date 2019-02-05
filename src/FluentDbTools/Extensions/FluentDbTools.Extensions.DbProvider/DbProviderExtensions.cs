using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.DbProvider.Oracle;
using FluentDbTools.DbProvider.Postgres;
using Npgsql;
using Oracle.ManagedDataAccess.Client;

namespace FluentDbTools.Extensions.DbProvider
{
    public static class DbProviderExtensions
    {
        private const string ErrorMsg = "Database type {0} is not implemented. " +
                                        "Please register a database provider implementing the '{1}' interface, " +
                                        "and register with 'Register'.";
       
        public static readonly Dictionary<SupportedDatabaseTypes, IDbConnectionProvider> DbConnectionProvidersField =
            new Dictionary<SupportedDatabaseTypes, IDbConnectionProvider>
            {
                {SupportedDatabaseTypes.Oracle, new OracleProvider()},
                {SupportedDatabaseTypes.Postgres, new PostgresProvider()}
            };
        
        public static readonly Dictionary<SupportedDatabaseTypes, DbProviderFactory> DbProviderFactoriesField =
            new Dictionary<SupportedDatabaseTypes, DbProviderFactory>
            {
                {SupportedDatabaseTypes.Oracle, OracleClientFactory.Instance},
                {SupportedDatabaseTypes.Postgres, NpgsqlFactory.Instance}
            };

        public static IReadOnlyDictionary<SupportedDatabaseTypes, IDbConnectionProvider>
            DbConnectionProviders => DbConnectionProvidersField;

        public static string GetConnectionString(this IDbConfig dbConfig)
        {
            var dbType = dbConfig.DbType;
            AssertDbConnectionImplemented(dbType);
            return DbConnectionProviders[dbType].GetConnectionString(dbConfig);
        }
        
        public static string GetAdminConnectionString(this IDbConfig dbConfig)
        {
            var dbType = dbConfig.DbType;
            AssertDbConnectionImplemented(dbType);
            return DbConnectionProviders[dbType].GetAdminConnectionString(dbConfig);
        }
        
        public static DbProviderFactory GetDbProviderFactory(this IDbConfig dbConfig, bool withAdminPrivileges = false)
        {
            var dbType = dbConfig.DbType;
            AssertDbProviderFactoryImplemented(dbType);
            var connectionString =
                withAdminPrivileges ? dbConfig.GetAdminConnectionString() : dbConfig.GetConnectionString();
            var dbProviderFactory = new FluentDbProviderFactory(DbProviderFactoriesField[dbType], connectionString);
            return dbProviderFactory;
        }

        public static IDbConnection CreateDbConnection(this IDbConfig dbConfig, bool withAdminPrivileges = false)
        {
            var dbType = dbConfig.DbType;
            AssertDbConnectionImplemented(dbType);
            return DbConnectionProviders[dbType].CreateDbConnection(dbConfig, withAdminPrivileges); 
        }

        public static IDbConnectionProvider Register(this IDbConnectionProvider dbConnectionProvider, bool replaceOldInstance = true)
        {
            if (replaceOldInstance)
            {
                DbConnectionProvidersField[dbConnectionProvider.DatabaseType] = dbConnectionProvider;
            }
            else
            {
                DbConnectionProvidersField.Add(dbConnectionProvider.DatabaseType, dbConnectionProvider);
            }
            return dbConnectionProvider;
        }
        
        public static DbProviderFactory Register(this DbProviderFactory dbProviderFactory, SupportedDatabaseTypes databaseType, bool replaceOldInstance = true)
        {
            if (replaceOldInstance)
            {
                DbProviderFactoriesField[databaseType] = dbProviderFactory;
            }
            else
            {
                DbProviderFactoriesField.Add(databaseType, dbProviderFactory);
            }
            return dbProviderFactory;
        }

        private static void AssertDbConnectionImplemented(SupportedDatabaseTypes dbType)
        {
            if (!DbConnectionProviders.ContainsKey(dbType))
            {
                throw new NotImplementedException(string.Format(ErrorMsg, dbType.ToString(), nameof(IDbConnectionProvider)));
            }
        }
        
        private static void AssertDbProviderFactoryImplemented(SupportedDatabaseTypes dbType)
        {
            if (!DbProviderFactoriesField.ContainsKey(dbType))
            {
                throw new NotImplementedException(string.Format(ErrorMsg, dbType.ToString(), nameof(DbProviderFactory)));
            }
        }
    }
}