using System;
using FluentDbTools.Common.Abstractions;

namespace FluentDbTools.Contracts
{
    /// <inheritdoc />
    public class DbConfigDatabaseTargets : IDbConfigDatabaseTargets
    {
        private SupportedDatabaseTypes? DbTypeField;
        private string SchemaField;
        private string DatabaseConnectionNameField;

        /// <summary>
        /// All default values
        /// </summary>
        public DefaultDbConfigValues Defaults { get; protected set; }
        
        /// <summary>
        /// Current SchemaPrefixId
        /// </summary>
        protected string SchemaPrefixIdField { get; set; }

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="defaultDbConfigValues"></param>
        public DbConfigDatabaseTargets(DefaultDbConfigValues defaultDbConfigValues = null)
        {
            Defaults = defaultDbConfigValues ?? new DefaultDbConfigValues();
        }


        /// <inheritdoc />
        public virtual SupportedDatabaseTypes DbType
        {
            get => DbTypeField ?? Defaults.GetDefaultDbType?.Invoke() ?? DefaultDbConfigValues.DefaultDbType;
            set => DbTypeField = value;
        }

        /// <inheritdoc />
        public virtual string Schema
        {
            get => SchemaField ?? Defaults.GetDefaultSchema?.Invoke();
            set => SchemaField = value;
        }

        /// <inheritdoc />
        public virtual string DatabaseName
        {
            get => DatabaseConnectionNameField ?? Defaults.GetDefaultDatabaseName?.Invoke();
            set => DatabaseConnectionNameField = value;
        }

        /// <inheritdoc />
        public virtual string GetSchemaPrefixId() => SchemaPrefixIdField ?? Defaults.GetDefaultSchemaPrefixIdString();

        /// <summary>
        /// Factory method for create <see cref="DbConfigDatabaseTargets"/>
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="schema"></param>
        /// <param name="databaseName"></param>
        /// <param name="schemaPrefixId"></param>
        /// <returns></returns>
        public static DbConfigDatabaseTargets Create(
            SupportedDatabaseTypes dbType, 
            string schema,
            string databaseName = null,
            string schemaPrefixId = null)
        {
            return new DbConfigDatabaseTargets()
            {
                DbType = dbType,
                Schema = schema,
                DatabaseName = databaseName,
                SchemaPrefixIdField = schemaPrefixId,
            };
        }

        /// <summary>
        /// Can be called when configuration is changed/reloaded
        /// </summary>
        /// <param name="getValueFunc"></param>
        protected virtual void OnConfigurationChanged(Func<string[], string> getValueFunc)
        {
            DbTypeField = null;
            SchemaField = null;
            DatabaseConnectionNameField = null;
            SchemaPrefixIdField = null;
        }

    }
}