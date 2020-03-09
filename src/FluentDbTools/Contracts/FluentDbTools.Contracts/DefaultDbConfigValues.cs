using System;
using System.Collections.Generic;
using FluentDbTools.Common.Abstractions;

namespace FluentDbTools.Contracts
{
    /// <summary>
    /// Defines available Default functions
    /// </summary>
    public class DefaultDbConfigValues
    {
        private Dictionary<string, string> AllConfigValuesField;

        /// <summary>
        /// Default database type
        /// </summary>
        public const SupportedDatabaseTypes DefaultDbType = SupportedDatabaseTypes.Postgres;

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
        public Func<string> GetDefaultDatabaseName = null;
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
        public Func<string> GetDefaultAdminUser = () => "postgres";

        /// <summary>
        /// DbConfigCredentials defaults: <br/>
        /// - Function returning Default database admin user password
        /// </summary>
        public Func<string> GetDefaultAdminPassword = () => "postgres";
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
        public Func<string> GetDefaultPort = () => "5432";

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
    }
}