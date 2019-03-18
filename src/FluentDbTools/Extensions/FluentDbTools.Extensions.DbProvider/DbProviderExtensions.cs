using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using FluentDbTools.Common.Abstractions;

namespace FluentDbTools.Extensions.DbProvider
{
    public static class DbProviderExtensions
    {
        private const string ErrorMsg = "Database type {0} is not implemented. " +
                                        "Please register a database provider implementing the '{1}' interface, " +
                                        "and register with 'Register'.";
       
        public static readonly ConcurrentDictionary<SupportedDatabaseTypes, IDbConnectionProvider> DbConnectionProviders =
            new ConcurrentDictionary<SupportedDatabaseTypes, IDbConnectionProvider>
            {
                [SupportedDatabaseTypes.Oracle] = new DbProviders.OracleProvider(),
                [SupportedDatabaseTypes.Postgres] = new DbProviders.PostgresProvider()
            };

        public static readonly ConcurrentDictionary<SupportedDatabaseTypes, DbProviderFactory> DbProviderFactories =
            new ConcurrentDictionary<SupportedDatabaseTypes, DbProviderFactory>();

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
            var dbProviderFactory = new FluentDbProviderFactory(DbProviderFactories[dbType], connectionString);
            return dbProviderFactory;
        }

        public static IDbConnection CreateDbConnection(this IDbConfig dbConfig, bool withAdminPrivileges = false)
        {
            var dbType = dbConfig.DbType;
            AssertDbConnectionImplemented(dbType);
            AssertDbProviderFactoryImplemented(dbType);
            return dbConfig.GetDbProviderFactory(withAdminPrivileges).CreateConnection();
        }

        public static IDbConnectionProvider Register(this IDbConnectionProvider dbConnectionProvider, bool skipIfAlreadyRegistered = false)
        {
            if (skipIfAlreadyRegistered && DbConnectionProviders.ContainsKey(dbConnectionProvider.DatabaseType))
            {
                return DbConnectionProviders[dbConnectionProvider.DatabaseType];
            }
            
            DbConnectionProviders[dbConnectionProvider.DatabaseType] = dbConnectionProvider;
            return dbConnectionProvider;
        }
        
        public static DbProviderFactory Register(this DbProviderFactory dbProviderFactory, SupportedDatabaseTypes databaseType, bool skipIfAlreadyRegistered = false)
        {
            if (skipIfAlreadyRegistered && DbProviderFactories.ContainsKey(databaseType))
            {
                return DbProviderFactories[databaseType];
            }
            
            DbProviderFactories[databaseType] = dbProviderFactory;
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
            if (!DbProviderFactories.ContainsKey(dbType))
            {
                throw new NotImplementedException(string.Format(ErrorMsg, dbType.ToString(), nameof(DbProviderFactory)));
            }
        }
    }
}