using System.Dynamic;
using FluentDbTools.Common.Abstractions;

namespace FluentDbTools.Contracts
{
    public class DbConfigDatabaseTargets : IDbConfigDatabaseTargets
    {
        public DefaultDbConfigValues Defaults { get; protected set; }

        protected string ScemaPrefixIdField { get; set; }
        public DbConfigDatabaseTargets(DefaultDbConfigValues defaultDbConfigValues = null)
        {
            Defaults = defaultDbConfigValues ?? new DefaultDbConfigValues();
        }


        protected SupportedDatabaseTypes? DbTypeBacking { get; set; }
        public virtual SupportedDatabaseTypes DbType
        {
            get => DbTypeBacking ?? Defaults.GetDefaultDbType?.Invoke() ?? DefaultDbConfigValues.DefaultDbType;
            set => DbTypeBacking = value;
        }

        private string SchemaField;
        public virtual string Schema
        {
            get => SchemaField ?? Defaults.GetDefaultSchema?.Invoke();
            set => SchemaField = value;
        }

        private string DatabaseConnectionNameField;
        public virtual string DatabaseName
        {
            get => DatabaseConnectionNameField ?? Defaults.GetDefaulDatabaseName?.Invoke();
            set => DatabaseConnectionNameField = value;
        }

        public virtual string GetSchemaPrefixId() => ScemaPrefixIdField ?? Defaults.GetDefaultSchemaPrefixIdString();

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
                ScemaPrefixIdField = schemaPrefixId
            };
        }
    }
}