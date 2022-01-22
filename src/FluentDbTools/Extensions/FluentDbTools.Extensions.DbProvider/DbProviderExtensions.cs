using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Contracts;
using FluentDbTools.DbProviders;

// ReSharper disable UnusedMember.Global
[assembly: InternalsVisibleTo("FluentDbTools.Extensions.MSDependencyInjection")]

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
                [SupportedDatabaseTypes.Oracle] = new OracleConnectionStringBuilder(),
                [SupportedDatabaseTypes.Postgres] = new PostgresConnectionStringBuilder()
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
        /// <param name="assert"></param>
        /// <returns></returns>
        public static IDbConnectionStringBuilder GetConnectionStringProvider(this SupportedDatabaseTypes dbType, bool assert = true)
        {
            if (!assert)
            {
                return DbConnectionProviders.TryGetValue(dbType, out var provider) ? provider : null;
            }

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

            return BuildConnectionString(dbConfig);
        }


        /// <summary>
        /// <para>Return dbConfig.AdminConnectionString if set, elsewhere AdminConnectionString is build from dbConfig settings</para>
        /// <para>
        /// if <paramref name="validateAdminUserAndPassword"/> is TRUE (default is FALSE), exception will be thrown if an invalid AdminUser or AdminPassword is found:<br/>
        /// - If both of them is invalid, an <see cref="AggregateException"></see> will be thrown<br/>
        /// - If only one of them is invalid, an <see cref="ArgumentNullException"></see> or <see cref="ArgumentException"></see> will be thrown
        /// </para>
        /// </summary>
        /// <param name="dbConfig"></param>
        /// <param name="validateAdminUserAndPassword"></param>
        /// <returns></returns>
        public static string GetAdminConnectionString(this IDbConfig dbConfig, bool validateAdminUserAndPassword = false)
        {
            if (validateAdminUserAndPassword)
            {
                // Will throw exception if fail
                dbConfig.ValidateDatabaseAdminValues();
            }

            if (!string.IsNullOrEmpty(dbConfig.AdminConnectionString))
            {
                return dbConfig.AdminConnectionString;
            }

            return BuildAdminConnectionString(dbConfig);
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

        internal static string BuildConnectionString(this IDbConfig dbConfig, bool assert = true)
        {
            var dbType = dbConfig.DbType;
            if (assert)
            {
                AssertDbConnectionImplemented(dbType);
            }

            return dbType.GetConnectionStringProvider(assert)?.BuildConnectionString(dbConfig);
        }

        internal static string BuildAdminConnectionString(this IDbConfig dbConfig, bool assert = true)
        {
            var dbType = dbConfig.DbType;
            if (assert)
            {
                AssertDbConnectionImplemented(dbType);
            }
            return dbType.GetConnectionStringProvider(assert)?.BuildAdminConnectionString(dbConfig);
        }

        /// <inheritdoc cref="ValidateDatabaseAdminValues(IDbConfig,bool,bool,bool)"/>
        public static InvalidAdminValue[] ValidateDatabaseAdminValues(
            this IServiceProvider sp,
            bool validateAdminUser = true,
            bool validateAdminPassword = true,
            bool throwIfFail = false)
        {
            return sp.GetService(typeof(IDbConfig)) is IDbConfig dbConfig
                ? dbConfig.ValidateDatabaseAdminValues(validateAdminUser, validateAdminPassword, throwIfFail)
                : Array.Empty<InvalidAdminValue>();
        }

        /// <summary>
        /// <para>Validate <see cref="IDbConfigCredentials.AdminUser">IDbConfig.AdminUser</see> and <see cref="IDbConfigCredentials.AdminPassword">IDbConfig.AdminPassword</see></para>
        /// <para>
        /// if <paramref name="throwIfFail"/> is TRUE (default is TRUE), exception will be thrown if any invalid values is found:<br/>
        /// - If more than one invalid value is found, an <see cref="AggregateException"></see> will be thrown<br/>
        /// - If only one invalid valid is found, an <see cref="ArgumentNullException"></see> or <see cref="ArgumentException"></see> will be thrown
        /// </para>
        /// </summary>
        /// <param name="dbConfig"></param>
        /// <param name="validateAdminUser">Enable validation check for invalid admin user (default is TRUE)</param>
        /// <param name="validateAdminPassword">Enable validation check for invalid admin password (default is TRUE)</param>
        /// <param name="throwIfFail">if TRUE (default is TRUE), exception will be thrown if any invalid values is found</param>
        /// <returns></returns>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static InvalidAdminValue[] ValidateDatabaseAdminValues(
            this IDbConfig dbConfig,
            bool validateAdminUser = true,
            bool validateAdminPassword = true,
            bool throwIfFail = true)
        {
            var isMissingAdminUser = string.IsNullOrEmpty(dbConfig.AdminUser) ||
                                     dbConfig.AdminUser == DefaultDbConfigValuesStatic.PossibleInvalidAdminUserAndPassword.AdminUser ||
                                     dbConfig.AdminUser.IndexOfAny(DefaultDbConfigValuesStatic.InvalidDatabaseUserCharacter) > -1;

            var isMissingAdminPassword = string.IsNullOrEmpty(dbConfig.AdminPassword) ||
                                         dbConfig.AdminPassword == DefaultDbConfigValuesStatic.PossibleInvalidAdminUserAndPassword.AdminPassword;

            validateAdminUser = validateAdminUser && isMissingAdminUser;
            validateAdminPassword = validateAdminPassword && isMissingAdminPassword;

            if (validateAdminUser == false && validateAdminPassword == false)
            {
                return Array.Empty<InvalidAdminValue>();
            }

            DbConfig dbConfigImplementation = null;
            if (dbConfig is DbConfig dbConfigImpl)
            {
                dbConfigImplementation = dbConfigImpl;
                var invalidAdminValues = dbConfigImplementation.InvalidAdminValuesInternal?.Where(x => x.IsAdminUser() && validateAdminUser || x.IsAdminPassword() && validateAdminPassword).ToArray();
                if (invalidAdminValues != null)
                {
                    if (throwIfFail && invalidAdminValues.Any())
                    {
                        invalidAdminValues.ThrowIfInvalidDatabaseAdminValues();
                    }

                    return invalidAdminValues;
                }
            }

            var array = dbConfig.GetPrioritizedConfigKeys();
            
            var dictionary = new Dictionary<InvalidAdminType, List<string[]>>();
            foreach (var a in array)
            {
                if (validateAdminUser)
                {
                    var keys = a.GetDbAdminUserKeys();
                    if (keys?.Any() == true)
                    {
                        if (dictionary.TryGetValue(InvalidAdminType.AdminUser, out var value))
                        {
                            value.Add(keys);
                        }
                        else
                        {
                            dictionary.Add(InvalidAdminType.AdminUser, new List<string[]> { keys });
                        }
                    }
                }

                if (validateAdminPassword)
                {
                    var keys = a.GetDbAdminPasswordKeys();
                    if (keys?.Any() == true)
                    {
                        if (dictionary.TryGetValue(InvalidAdminType.AdminPassword, out var value))
                        {
                            value.Add(keys);
                        }
                        else
                        {
                            dictionary.Add(InvalidAdminType.AdminPassword, new List<string[]> { keys });
                        }
                    }
                }
            }

            if (validateAdminUser)
            {
                var defaultKeys = new[] { "Database:AdminUser" };
                if (dictionary.TryGetValue(InvalidAdminType.AdminUser, out var list))
                {
                    list.Add(defaultKeys);
                }
                else
                {
                    dictionary.Add(InvalidAdminType.AdminUser, new List<string[]> { defaultKeys });
                }
            }

            if (validateAdminPassword)
            {
                var defaultKeys = new[] { "Database:AdminPassword" };
                if (dictionary.TryGetValue(InvalidAdminType.AdminPassword, out var list))
                {
                    list.Add(defaultKeys);
                }
                else
                {
                    dictionary.Add(InvalidAdminType.AdminPassword, new List<string[]> { defaultKeys });
                }
            }

            var result = dictionary.Select(ToInvalidAdmin).ToArray();

            if (dbConfigImplementation != null)
            {
                dbConfigImplementation.InvalidAdminValuesInternal = result;
            }

            if (throwIfFail && result.Any())
            {
                result.ThrowIfInvalidDatabaseAdminValues();
            }

            return result;

            InvalidAdminValue ToInvalidAdmin(KeyValuePair<InvalidAdminType, List<string[]>> x)
            {
                var value =
                    new InvalidAdminValue
                    {
                        InvalidAdminType = x.Key,
                        Value = x.Key == InvalidAdminType.AdminUser ? dbConfig.AdminUser : dbConfig.AdminPassword,
                        ConfigurationKeys = x.Value.SelectMany(s => s).Distinct().Select(s => $"{ValidateDatabaseAdminValuesExtensions.ConvertConfigKeyToParamNameStyle(s)}").ToArray()
                    };

                if (value.ConfigurationKeys == null)
                {
                    return value;
                }

                if (string.IsNullOrEmpty(value.Value))
                {
                    var first = value.ConfigurationKeys.Length == 1
                        ? "Required configuration parameter"
                        : "All required configuration parameters";

                    var next = value.ConfigurationKeys.Length == 1
                        ? value.ConfigurationKeys.FirstOrDefault()
                        : $"[{string.Join(", ", value.ConfigurationKeys)}]";

                    value.GeneratedArgumentException = new ArgumentNullException(value.ConfigurationKeys.FirstOrDefault()?.Replace("'", ""), $"{first} {next} is not set.")
                    {
                        Source = value.InvalidAdminType.ToString("G")
                    };
                }
                else
                {
                    var invalidKey = value.ConfigurationKeys.Length == 1
                        ? value.ConfigurationKeys.FirstOrDefault()
                        : null;
                    if (invalidKey == null)
                    {
                        foreach (var key in value.ConfigurationKeys)
                        {
                            var configKey = ToConfigKey(key);
                            if (dbConfig.GetConfigValue(configKey) == value.Value)
                            {
                                invalidKey = key;
                                break;
                            }
                        }
                    }

                    if (invalidKey != null)
                    {
                        value.ConfigurationKeys = new[] { invalidKey };
                    }


                    var first = value.ConfigurationKeys.Length == 1 || invalidKey != null
                        ? "Required configuration parameter"
                        : "One of the required configuration parameters";

                    var next = invalidKey ??
                               (value.ConfigurationKeys.Length == 1
                                   ? value.ConfigurationKeys.FirstOrDefault()
                                   : $"[{string.Join(", ", value.ConfigurationKeys)}]");


                    value.GeneratedArgumentException = new ArgumentException($"{first} {next} has an invalid value '{value.Value}'.")
                    {
                        Source = value.InvalidAdminType.ToString("G")
                    };
                }

                return value;
            }

            string ToConfigKey(string s)
            {
                s = s
                    .Replace(".", ":")
                    .Replace("'", "");

                return s;
            }
        }
    }
}