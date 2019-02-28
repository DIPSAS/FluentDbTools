using FluentDbTools.Common.Abstractions;

namespace FluentDbTools.Extensions.DbProvider
{
    public class DbConfig : IDbConfig
    {
        private SupportedDatabaseTypes? DbTypeField;
        public virtual SupportedDatabaseTypes DbType
        {
            get => DbTypeField ?? SupportedDatabaseTypes.Postgres;
            set => DbTypeField = value;
        }
        
        private string UserField;
        public virtual string User
        {
            get => UserField ?? "user";
            set => UserField = value;
        }
        
        private string PasswordField;
        public virtual string Password
        {
            get => PasswordField ?? "password";
            set => PasswordField = value;
        }
        
        private string AdminUserField;
        public virtual string AdminUser
        {
            get => AdminUserField ?? "postgres";
            set => AdminUserField = value;
        }
        
        private string AdminPasswordField;
        public virtual string AdminPassword
        {
            get => AdminPasswordField ?? "postgres";
            set => AdminPasswordField = value;
        }

        private string HostnameField;
        public virtual string Hostname
        {
            get => HostnameField ?? "localhost";
            set => HostnameField = value;
        }

        private string PortField;
        public virtual string Port
        {
            get => PortField ?? "5432";
            set => PortField = value;
        }

        private string DatabaseConnectionNameField;
        public virtual string DatabaseConnectionName
        {
            get => DatabaseConnectionNameField ?? Schema;
            set => DatabaseConnectionNameField = value;
        }

        private bool? PoolingField;
        public virtual bool Pooling
        {
            get => PoolingField ?? true;
            set => PoolingField = value;
        }

        private string SchemaField;
        public virtual string Schema
        {
            get => SchemaField ?? User;
            set => SchemaField = value;
        }

        private string SchemaPasswordField;
        public virtual string SchemaPassword
        {
            get => SchemaPasswordField ?? Password;
            set => SchemaPasswordField = value;
        }

        private string TablespaceField;
        public virtual string DefaultTablespace
        {
            get => TablespaceField ?? "FLUENT_DATA";
            set => TablespaceField = value;
        }

        private string TempTablespaceField;
        public virtual string TempTablespace
        {
            get => TempTablespaceField ?? "FLUENT_TEMP";
            set => TempTablespaceField = value;
        }

        public virtual string ConnectionStringTemplate { get; set; }
    }
}