using FluentDbTools.Common.Abstractions;

namespace FluentDbTools.Contracts
{
    public class DbConfigDatabaseTargets : IDbConfigDatabaseTargets
    {
        protected DefaultDbConfigValues Defaults;

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

        public static DbConfigDatabaseTargets Create(SupportedDatabaseTypes dbType, string schema,
            string databaseName = null)
        {
            return new DbConfigDatabaseTargets()
            {
                DbType = dbType,
                Schema = schema,
                DatabaseName = databaseName
            };
        }
    }
}