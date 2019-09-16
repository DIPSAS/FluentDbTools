using System.Dynamic;
using FluentDbTools.Common.Abstractions;

namespace FluentDbTools.Contracts
{
    /// <inheritdoc />
    public class DbConfigDatabaseTargets : IDbConfigDatabaseTargets
    {
        private SupportedDatabaseTypes? DbTypeField;
        private string SchemaField;
        private string DatabaseConnectionNameField;
        private string SchemaPrefixUniqueIdField;

        /// <summary>
        /// All default values
        /// </summary>
        public DefaultDbConfigValues Defaults { get; protected set; }
        
        protected string ScemaPrefixIdField { get; set; }
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
            get => DatabaseConnectionNameField ?? Defaults.GetDefaulDatabaseName?.Invoke();
            set => DatabaseConnectionNameField = value;
        }

        /// <inheritdoc />
        public virtual string GetSchemaPrefixId() => ScemaPrefixIdField ?? Defaults.GetDefaultSchemaPrefixIdString();

        /// <inheritdoc />
        public virtual string GetSchemaPrefixUniqueId() => SchemaPrefixUniqueIdField ?? Defaults.GetDefaultSchemaPrefixUniqueIdString();

        public static DbConfigDatabaseTargets Create(
            SupportedDatabaseTypes dbType, 
            string schema,
            string databaseName = null,
            string schemaPrefixId = null,
            string schemaPrefixUniqueId = null)
        {
            return new DbConfigDatabaseTargets()
            {
                DbType = dbType,
                Schema = schema,
                DatabaseName = databaseName,
                ScemaPrefixIdField = schemaPrefixId,
                SchemaPrefixUniqueIdField =  schemaPrefixUniqueId
            };
        }
    }
}