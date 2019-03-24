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
       
        public static readonly ConcurrentDictionary<SupportedDatabaseTypes, IDbConnectionStringBuilder> DbConnectionProviders =
            new ConcurrentDictionary<SupportedDatabaseTypes, IDbConnectionStringBuilder>
            {
                [SupportedDatabaseTypes.Oracle] = new DbProviders.OracleConnectionStringBuilder(),
                [SupportedDatabaseTypes.Postgres] = new DbProviders.PostgresConnectionStringBuilder()
            };

        public static readonly ConcurrentDictionary<SupportedDatabaseTypes, DbProviderFactory> DbProviderFactories =
            new ConcurrentDictionary<SupportedDatabaseTypes, DbProviderFactory>();

        public static IDbConnectionStringBuilder GetConnectionStringProvider(this SupportedDatabaseTypes dbType)
        {
            AssertDbConnectionImplemented(dbType);
            return DbConnectionProviders[dbType];
        }

        public static string GetConnectionString(this IDbConfig dbConfig)
        {
            if (!string.IsNullOrEmpty(dbConfig.ConnectionString))
            {
                return dbConfig.ConnectionString;
            }

            var dbType = dbConfig.DbType;
            AssertDbConnectionImplemented(dbType);
            return dbType.GetConnectionStringProvider().BuildConnectionString(dbConfig);
        }


        public static string GetAdminConnectionString(this IDbConfig dbConfig)
        {
            if (!string.IsNullOrEmpty(dbConfig.AdminConnectionString))
            {
                return dbConfig.AdminConnectionString;
            }

            var dbType = dbConfig.DbType;
            AssertDbConnectionImplemented(dbType);
            return dbType.GetConnectionStringProvider().BuildAdminConnectionString(dbConfig);
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

        public static IDbConnection CreateDbConnection(this SupportedDatabaseTypes dbType, string connectionString)
        {
            AssertDbConnectionImplemented(dbType);
            AssertDbProviderFactoryImplemented(dbType);
            var dbProviderFactory = new FluentDbProviderFactory(DbProviderFactories[dbType], connectionString);
            return dbProviderFactory.CreateConnection();
        }


        public static IDbConnectionStringBuilder Register(this IDbConnectionStringBuilder dbConnectionStringBuilder, bool skipIfAlreadyRegistered = false)
        {
            if (skipIfAlreadyRegistered && DbConnectionProviders.ContainsKey(dbConnectionStringBuilder.DatabaseType))
            {
                return DbConnectionProviders[dbConnectionStringBuilder.DatabaseType];
            }
            
            DbConnectionProviders[dbConnectionStringBuilder.DatabaseType] = dbConnectionStringBuilder;
            return dbConnectionStringBuilder;
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
                throw new NotImplementedException(string.Format(ErrorMsg, dbType.ToString(), nameof(IDbConnectionStringBuilder)));
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