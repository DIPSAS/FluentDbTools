using System;
using System.Collections.Generic;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Common.Abstractions.PrioritizedConfig;

// ReSharper disable UnusedMember.Global
#pragma warning disable CS1591

namespace FluentDbTools.Contracts
{
    /// <summary>
    /// Defines available Default functions
    /// </summary>
    public class DefaultDbConfigValues
    {
        private Dictionary<string, string> AllConfigValuesField;

        /// <summary>
        /// <para>Get Prioritized config keys</para>
        /// <para>Name 'Prioritized' is used to signal that these keys is prioritized before default-keys</para>
        /// </summary>
        public IPrioritizedConfigKeys[] PrioritizedConfigKeys { get; set; } = { new PrioritizedConfigKeys() };

        /// <summary>
        /// Default database type
        /// </summary>
        public static SupportedDatabaseTypes DefaultDbType { get; set; } = WithLibraryDefaultDbType();

        #region DbConfigDatabaseTargets defaults       
        /// <summary>
        /// DbConfigDatabaseTargets defaults: <br/>
        /// - Function returning Default database type
        /// </summary>
        public Func<SupportedDatabaseTypes> GetDefaultDbType = () => DefaultDbType;

        /// <summary>
        /// DbConfigDatabaseTargets defaults: <br/>
        /// - Function returning Default schema name
        /// </summary>
        public Func<string> GetDefaultSchema = null;

        /// <summary>
        /// DbConfigDatabaseTargets defaults: <br/>
        /// - Function returning Default database name. Not in use for Oracle connection
        /// </summary>
        public Func<string> GetDefaultDatabaseName = () => DefaultDbConfigValuesStatic.DefaultServiceName;
        #endregion

        #region DbConfigCredentials defaults
        /// <summary>
        /// DbConfigCredentials defaults: <br/>
        /// - Function returning Default database user
        /// </summary>
        public Func<string> GetDefaultUser = () => "user";

        /// <summary>
        /// DbConfigCredentials defaults: <br/>
        /// - Function returning Default database user password
        /// </summary>
        public Func<string> GetDefaultPassword = () => "password";

        /// <summary>
        /// DbConfigCredentials defaults: <br/>
        /// - Function returning Default database admin user
        /// </summary>
        public Func<string> GetDefaultAdminUser = () => DefaultDbConfigValuesStatic.DefaultPostgresAdminUserAndPassword.AdminUser;

        /// <summary>
        /// DbConfigCredentials defaults: <br/>
        /// - Function returning Default database admin user password
        /// </summary>
        public Func<string> GetDefaultAdminPassword = () => DefaultDbConfigValuesStatic.DefaultPostgresAdminUserAndPassword.AdminPassword;
        #endregion

        #region DbConnectionStringBuilderConfig defaults
        /// <summary>
        /// DbConnectionStringBuilderConfig defaults: <br/>
        /// - Function returning Default database host name
        /// </summary>
        public Func<string> GetDefaultHostName = () => "localhost";

        /// <summary>
        /// DbConnectionStringBuilderConfig defaults: <br/>
        /// - Function returning Default database port number
        /// </summary>
        public Func<string> GetDefaultPort = () => DefaultDbType == SupportedDatabaseTypes.Oracle ? DefaultDbConfigValuesStatic.DefaultOraclePort : DefaultDbConfigValuesStatic.DefaultPostgresPort;

        /// <summary>
        /// DbConnectionStringBuilderConfig defaults: <br/>
        /// - Function returning Default connection timeout (in seconds)
        /// </summary>
        public Func<string> GetDefaultConnectionTimeoutInSecs = () => null;

        /// <summary>
        /// DbConnectionStringBuilderConfig defaults: <br/>
        /// - Function returning Default connection pooling switch (true/false)
        /// </summary>
        public Func<bool> GetDefaultPooling = () => true;

        /// <summary>
        /// DbConnectionStringBuilderConfig defaults: <br/>
        /// - Function returning Default connection pooling switch (true/false)
        /// </summary>
        public Func<IDictionary<string, string>> GetDefaultPoolingKeyValues = () => null;


        /// <summary>
        /// DbConnectionStringBuilderConfig defaults: <br/>
        /// - Function returning Default database dataSource. Overrides <see cref="GetDefaultHostName"/> and <see cref="GetDefaultPort"/>
        /// </summary>
        public Func<string> GetDefaultDataSource = () => null;
        #endregion

        #region DbConfig connectionString defaults

        /// <summary>
        /// ConnectionString defaults: <br/>
        /// - Function returning Default database connection string<br/>
        /// Overrides all above functions 
        /// </summary>
        public Func<string> GetDefaultConnectionString = () => null;

        /// <summary>
        /// ConnectionString defaults: <br/>
        /// - Function returning Default database admin connection string<br/>
        /// Overrides all above functions 
        /// </summary>
        public Func<string> GetDefaultAdminConnectionString = () => null;
        #endregion

        #region SchemaPrefix defaults
        /// <summary>
        /// SchemaPrefix defaults: <br/>
        /// - Function returning Default SchemaPrefixId <br/>
        /// </summary>
        public Func<string> GetDefaultSchemaPrefixIdString = () => null;

        /// <summary>
        /// SchemaPrefix defaults: <br/>
        /// - Function returning Default SchemaPrefixUniqueId <br/>
        /// </summary>
        public Func<string> GetDefaultSchemaPrefixUniqueIdString = () => null;

        #endregion

        /// <summary>
        /// GetAllMigrationConfigValues() : Get al values and subValues from configuration "database:migration". 
        /// </summary>
        public virtual IDictionary<string, string> GetAllDatabaseConfigValues(bool reload = false, string sectionName = null)
        {
            if (AllConfigValuesField == null || reload)
            {
                AllConfigValuesField = new Dictionary<string, string>();
            }

            return AllConfigValuesField;
        }

        /// <summary>
        /// Change <see cref="DefaultDbType">DefaultDbType</see> back to <see cref="SupportedDatabaseTypes.Postgres">Postgres</see>
        /// </summary>
        /// <returns></returns>
        public static SupportedDatabaseTypes WithLibraryDefaultDbType()
        {
            return WithDefaultDbType(DefaultDbConfigValuesStatic.LibraryDefaultDbType);
        }

        /// <summary>
        /// Change <see cref="DefaultDbType">DefaultDbType</see> to <see cref="SupportedDatabaseTypes.Oracle">Oracle</see>
        /// </summary>
        /// <returns></returns>
        public static SupportedDatabaseTypes WithOracleDefaultDbType()
        {
            return WithDefaultDbType(SupportedDatabaseTypes.Oracle);
        }

        /// <summary>
        /// Change <see cref="DefaultDbType">DefaultDbType</see> to <paramref name="dbType"/>
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static SupportedDatabaseTypes WithDefaultDbType(SupportedDatabaseTypes dbType)
        {
            DefaultDbType = dbType;
            return DefaultDbType;
        }

        /// <summary>
        /// <para>Change default Oracle and Postgres Database.AdminUser and Database.AdminPassword to DEFAULT values</para>
        /// - Change <see cref="DefaultDbConfigValuesStatic.DefaultOracleAdminUserAndPassword"/> back to library default <see cref="DefaultDbConfigValuesStatic.LibraryDefaultOracleAdminUserAndPassword"/><br/>
        /// - Change <see cref="DefaultDbConfigValuesStatic.DefaultPostgresAdminUserAndPassword"/> back to library default <see cref="DefaultDbConfigValuesStatic.LibraryDefaultPostgresAdminUserAndPassword"/><br/>
        /// </summary>
        public static void WithLibraryDefaultAdminUserAndPassword()
        {
            DefaultDbConfigValuesStatic.DefaultOracleAdminUserAndPassword = DefaultDbConfigValuesStatic.LibraryDefaultOracleAdminUserAndPassword;
            DefaultDbConfigValuesStatic.DefaultPostgresAdminUserAndPassword = DefaultDbConfigValuesStatic.LibraryDefaultPostgresAdminUserAndPassword;
        }

        /// <summary>
        /// <para>Change default Oracle and Postgres Database.AdminUser and Database.AdminPassword to EMPTY values <see cref="DefaultDbConfigValuesStatic.EmptyAdminUserAndPassword"/></para>
        /// - Change <see cref="DefaultDbConfigValuesStatic.DefaultOracleAdminUserAndPassword"/><br/>
        /// - Change <see cref="DefaultDbConfigValuesStatic.DefaultPostgresAdminUserAndPassword"/><br/>
        /// </summary>
        public static void WithEmptyAdminUserAndPassword()
        {
            DefaultDbConfigValuesStatic.DefaultOracleAdminUserAndPassword = DefaultDbConfigValuesStatic.EmptyAdminUserAndPassword;
            DefaultDbConfigValuesStatic.DefaultPostgresAdminUserAndPassword = DefaultDbConfigValuesStatic.EmptyAdminUserAndPassword;
        }

        /// <summary>
        /// Change the <see cref="DefaultDbConfigValuesStatic.PossibleInvalidAdminUserAndPassword"/> back to library default <see cref="DefaultDbConfigValuesStatic.LibraryDefaultPossibleInvalidAdminUserAndPassword"/>
        /// </summary>
        public static void WithLibraryDefaultPossibleInvalidAdminUserAndPassword()
        {
            DefaultDbConfigValuesStatic.PossibleInvalidAdminUserAndPassword = DefaultDbConfigValuesStatic.LibraryDefaultPossibleInvalidAdminUserAndPassword;
        }

        /// <summary>
        /// Change the <see cref="DefaultDbConfigValuesStatic.PossibleInvalidAdminUserAndPassword"/> by parameters <paramref name="invalidAdminUser"/> and <paramref name="invalidAdminPassword"/>
        /// </summary>
        /// <param name="invalidAdminUser"></param>
        /// <param name="invalidAdminPassword"></param>
        public static void WithPossibleInvalidAdminUserAndPassword(string invalidAdminUser = DefaultDbConfigValuesStatic.DefaultPossibleInvalidDbUpgradeUser, string invalidAdminPassword = DefaultDbConfigValuesStatic.DefaultPossibleInvalidDbUpgradePassword)
        {
            DefaultDbConfigValuesStatic.PossibleInvalidAdminUserAndPassword = (invalidAdminUser, invalidAdminPassword);
        }

        /// <summary>
        /// Get the value from <see cref="DefaultDbConfigValuesStatic.PossibleInvalidAdminUserAndPassword"/>
        /// </summary>
        /// <returns></returns>
        public static (string AdminUser, string AdminPassword) GetPossibleInvalidAdminUserAndPassword()
        {
            return DefaultDbConfigValuesStatic.PossibleInvalidAdminUserAndPassword;
        }
        
        /// Change the <see cref="DefaultDbConfigValuesStatic.DefaultServiceName"/> back to <paramref name="serviceName"/>
        public static void WithDefaultServiceName(string serviceName)
        {
            DefaultDbConfigValuesStatic.DefaultServiceName = serviceName;
        }

        /// Change the <see cref="DefaultDbConfigValuesStatic.DefaultServiceName"/> back to library default <see cref="DefaultDbConfigValuesStatic.LibraryDefaultServiceName"/>
        public static void WithLibraryDefaultServiceName()
        {
            DefaultDbConfigValuesStatic.DefaultServiceName = DefaultDbConfigValuesStatic.LibraryDefaultServiceName;
        }
    }
}