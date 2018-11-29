using System;
using System.Collections.Generic;
using System.Data;
using DIPS.FluentDbTools.Common.Abstractions;
using DIPS.FluentDbTools.Database.Abstractions;
using DIPS.FluentDbTools.DbProvider.Oracle;
using DIPS.FluentDbTools.DbProvider.Postgres;

namespace DIPS.Extensions.FluentDbTools.DbProvider
{
    public static class DbProviderExtensions
    {
        private const string ErrorMsg = "Database type {0} is not implemented. " +
                                        "Please register a database provider implementing the 'IDbConnectionProvider' interface, " +
                                        "and register with 'Register'.";
       
        public static readonly Dictionary<SupportedDatabaseTypes, IDbConnectionProvider> DbConnectionProvidersField =
            new Dictionary<SupportedDatabaseTypes, IDbConnectionProvider>
            {
                {SupportedDatabaseTypes.Oracle, new OracleProvider()},
                {SupportedDatabaseTypes.Postgres, new PostgresProvider()}
            };

        public static IReadOnlyDictionary<SupportedDatabaseTypes, IDbConnectionProvider>
            DbConnectionProviders => DbConnectionProvidersField;

        public static string GetConnectionString(this IDbConfig dbConfig)
        {
            var dbType = dbConfig.DbType;
            AssertDbTypeImplemented(dbType);
            return DbConnectionProviders[dbType].GetConnectionString(dbConfig);
        }
        
        public static string GetAdminConnectionString(this IDbConfig dbConfig)
        {
            var dbType = dbConfig.DbType;
            AssertDbTypeImplemented(dbType);
            return DbConnectionProviders[dbType].GetAdminConnectionString(dbConfig);
        }

        public static IDbConnection CreateDbConnection(this IDbConfig dbConfig, bool withAdminPrivileges = false)
        {
            var dbType = dbConfig.DbType;
            AssertDbTypeImplemented(dbType);
            return DbConnectionProviders[dbType].CreateDbConnection(dbConfig); 
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

        private static void AssertDbTypeImplemented(SupportedDatabaseTypes dbType)
        {
            if (!DbConnectionProviders.ContainsKey(dbType))
            {
                throw new NotImplementedException(string.Format(ErrorMsg, dbType.ToString()));
            }
        }
    }
}