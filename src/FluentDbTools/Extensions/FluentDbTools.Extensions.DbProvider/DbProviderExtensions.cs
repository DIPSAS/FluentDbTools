using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.IO;
using FluentDbTools.Common.Abstractions;
// ReSharper disable UnusedMember.Global

namespace FluentDbTools.Extensions.DbProvider
{
    /// <summary>
    /// DbProviderExtensions functions
    /// </summary>
    public static class DbProviderExtensions
    {
        private const string ErrorMsg = "Database type {0} is not implemented. " +
                                        "Please register a database provider implementing the '{1}' interface, " +
                                        "and register with 'Register'.";
       
        /// <summary>
        /// All registered IDbConnectionStringBuilders
        /// </summary>
        public static readonly ConcurrentDictionary<SupportedDatabaseTypes, IDbConnectionStringBuilder> DbConnectionProviders =
            new ConcurrentDictionary<SupportedDatabaseTypes, IDbConnectionStringBuilder>
            {
                [SupportedDatabaseTypes.Oracle] = new DbProviders.OracleConnectionStringBuilder(),
                [SupportedDatabaseTypes.Postgres] = new DbProviders.PostgresConnectionStringBuilder()
            };

        /// <summary>
        /// All registered DbProviderFactories 
        /// </summary>
        public static readonly ConcurrentDictionary<SupportedDatabaseTypes, DbProviderFactory> DbProviderFactories =
            new ConcurrentDictionary<SupportedDatabaseTypes, DbProviderFactory>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static IDbConnectionStringBuilder GetConnectionStringProvider(this SupportedDatabaseTypes dbType)
        {
            AssertDbConnectionImplemented(dbType);
            return DbConnectionProviders[dbType];
        }

        /// <summary>
        /// Return dbConfig.ConnectionString if set, elsewhere ConnectionString is build from dbConfig settings 
        /// </summary>
        /// <param name="dbConfig"></param>
        /// <returns></returns>
        public static string GetConnectionString(this IDbConfig dbConfig)
        {
            if (dbConfig.ConnectionString.IsNotEmpty())
            {
                return dbConfig.ConnectionString;
            }

            var dbType = dbConfig.DbType;
            AssertDbConnectionImplemented(dbType);
            return dbType.GetConnectionStringProvider().BuildConnectionString(dbConfig);
        }


        /// <summary>
        /// Return dbConfig.AdminConnectionString if set, elsewhere AdminConnectionString is build from dbConfig settings 
        /// </summary>
        /// <param name="dbConfig"></param>
        /// <returns></returns>
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
        
        /// <summary>
        /// Return a new instance of DbProviderFactory created from dbConfig settings 
        /// </summary>
        /// <param name="dbConfig"></param>
        /// <param name="withAdminPrivileges">If true, a DbProviderFactory with AdminConnectionString is build</param>
        /// <returns></returns>
        public static DbProviderFactory GetDbProviderFactory(this IDbConfig dbConfig, bool withAdminPrivileges = false)
        {
            var dbType = dbConfig.DbType;
            AssertDbProviderFactoryImplemented(dbType);
            var connectionString =
                withAdminPrivileges ? dbConfig.GetAdminConnectionString() : dbConfig.GetConnectionString();
            var dbProviderFactory = new FluentDbProviderFactory(DbProviderFactories[dbType], connectionString);
            return dbProviderFactory;
        }

        /// <summary>
        /// Return a new instance if IDbConnection created from dbConfig settings 
        /// </summary>
        /// <param name="dbConfig"></param>
        /// <param name="withAdminPrivileges">If true, a IDbConnection with AdminConnectionString is created</param>
        /// <returns></returns>
        public static IDbConnection CreateDbConnection(this IDbConfig dbConfig, bool withAdminPrivileges = false)
        {
            var dbType = dbConfig.DbType;
            AssertDbConnectionImplemented(dbType);
            AssertDbProviderFactoryImplemented(dbType);
            return dbConfig.GetDbProviderFactory(withAdminPrivileges).CreateConnection();
        }

        /// <summary>
        /// Return a new instance if IDbConnection with ConnectionString configured from 'connectionString' parameter
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static IDbConnection CreateDbConnection(this SupportedDatabaseTypes dbType, string connectionString)
        {
            AssertDbConnectionImplemented(dbType);
            AssertDbProviderFactoryImplemented(dbType);
            var dbProviderFactory = new FluentDbProviderFactory(DbProviderFactories[dbType], connectionString);
            return dbProviderFactory.CreateConnection();
        }


        /// <summary>
        /// Add/Register dbConnectionStringBuilder to DbConnectionProviders
        /// </summary>
        /// <param name="dbConnectionStringBuilder"></param>
        /// <param name="skipIfAlreadyRegistered"></param>
        /// <returns></returns>
        public static IDbConnectionStringBuilder Register(this IDbConnectionStringBuilder dbConnectionStringBuilder, bool skipIfAlreadyRegistered = false)
        {
            if (skipIfAlreadyRegistered && DbConnectionProviders.ContainsKey(dbConnectionStringBuilder.DatabaseType))
            {
                return DbConnectionProviders[dbConnectionStringBuilder.DatabaseType];
            }
            
            DbConnectionProviders[dbConnectionStringBuilder.DatabaseType] = dbConnectionStringBuilder;
            return dbConnectionStringBuilder;
        }
        
        /// <summary>
        /// Add/Register dbProviderFactory to DbProviderFactories
        /// </summary>
        /// <param name="dbProviderFactory"></param>
        /// <param name="databaseType"></param>
        /// <param name="skipIfAlreadyRegistered"></param>
        /// <returns></returns>
        public static DbProviderFactory Register(this DbProviderFactory dbProviderFactory, SupportedDatabaseTypes databaseType, bool skipIfAlreadyRegistered = false)
        {
            if (skipIfAlreadyRegistered && DbProviderFactories.ContainsKey(databaseType))
            {
                return DbProviderFactories[databaseType];
            }
            
            DbProviderFactories[databaseType] = dbProviderFactory;
            return dbProviderFactory;
        }

        /// <summary>
        /// To support TnsName lookup you can use this function to configure then path containing tnsnames.ora and sqlnet.ora
        /// If path is null or empty, the function wil try to resolve the path containing tnsnames.ora from Environment::Path 
        /// </summary>
        /// <param name="dbConfig"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IDbConfig ConfigureOracleTnsAdminPath(this IDbConfig dbConfig, string path)
        {
            ConfigureOracleTnsAdminPath(path);
            return dbConfig;
        }

        /// <summary>
        /// To support TnsName lookup you can use this function to configure then path containing tnsnames.ora and sqlnet.ora
        /// If path is null or empty, the function wil try to resolve the path containing tnsnames.ora from environmentPath or Environment::Path 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="environmentPath"></param>
        /// <returns></returns>
        public static string ConfigureOracleTnsAdminPath(string path, string environmentPath = null)
        {
            var resolvedPath = string.Empty;
            if (string.IsNullOrEmpty(path))
            {
                
                var environmentPaths = (environmentPath ?? Environment.GetEnvironmentVariable("PATH") ?? string.Empty).Split(Path.PathSeparator);
                foreach (var pathToCheck in environmentPaths)
                {
                    resolvedPath = ResolveTnsNamesOraPath(pathToCheck);

                    if (resolvedPath.IsNotEmpty())
                    {
                        break;
                    }
                }
            }
            else
            {
                resolvedPath = ResolveTnsNamesOraPath(path);
            }

            if (resolvedPath.IsEmpty())
            {
                return string.Empty;
            }

            Environment.SetEnvironmentVariable("TNS_ADMIN", resolvedPath, EnvironmentVariableTarget.Process);
            return resolvedPath;
        }

        private static string ResolveTnsNamesOraPath(string pathToCheck)
        {
            if (PathHasTnsNamesOra(pathToCheck))
            {
                return new DirectoryInfo(pathToCheck).FullName;
            }

            if (!pathToCheck.ContainsIgnoreCase("oracle"))
            {
                return string.Empty;
            }

            if (!pathToCheck.EndsWithIgnoreCase("bin"))
            {
                return string.Empty;
            }

            var adminPath = Path.Combine(pathToCheck, "..", "network", "admin");

            if (!PathHasTnsNamesOra(adminPath))
            {
                return string.Empty;
            }

            adminPath = new DirectoryInfo(adminPath).FullName;
            return adminPath;

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

        private static bool PathHasTnsNamesOra(string pathToCheck)
        {
            return Directory.Exists(pathToCheck) && File.Exists(Path.Combine(pathToCheck, "tnsnames.ora"));
        }
    }
}