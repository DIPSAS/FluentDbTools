using FluentDbTools.Common.Abstractions;
using Microsoft.Extensions.Configuration;

namespace FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs
{
    public class DefaultDbConfig : IDbConfig
    {
        private readonly IConfiguration Configuration;

        public DefaultDbConfig(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private SupportedDatabaseTypes? DbTypeField;
        public virtual SupportedDatabaseTypes DbType
        {
            get => DbTypeField ?? Configuration.GetDbType();
            set => DbTypeField = value;
        }
        
        private string UserField;
        public virtual string User
        {
            get => UserField ?? Configuration.GetDbUser();
            set => UserField = value;
        }
        
        private string PasswordField;
        public virtual string Password
        {
            get => PasswordField ?? Configuration.GetDbPassword();
            set => PasswordField = value;
        }
        
        private string AdminUserField;
        public virtual string AdminUser
        {
            get => AdminUserField ?? Configuration.GetDbAdminUser();
            set => AdminUserField = value;
        }
        
        private string AdminPasswordField;
        public virtual string AdminPassword
        {
            get => AdminPasswordField ?? Configuration.GetDbAdminPassword();
            set => AdminPasswordField = value;
        }

        private string HostnameField;
        public virtual string Hostname
        {
            get => HostnameField ?? Configuration.GetDbHostname();
            set => HostnameField = value;
        }

        private string PortField;
        public virtual string Port
        {
            get => PortField ?? Configuration.GetDbPort();
            set => PortField = value;
        }

        private string DatabaseConnectionNameField;
        public virtual string DatabaseConnectionName
        {
            get => DatabaseConnectionNameField ?? Configuration.GetDbConnectionName();
            set => DatabaseConnectionNameField = value;
        }

        private bool? PoolingField;
        public virtual bool Pooling
        {
            get => PoolingField ?? Configuration.GetDbPooling();
            set => PoolingField = value;
        }

        private string SchemaField;
        public virtual string Schema
        {
            get => SchemaField ?? Configuration.GetDbSchema();
            set => SchemaField = value;
        }

        private string SchemaPasswordField;
        public virtual string SchemaPassword
        {
            get => SchemaPasswordField ?? Configuration.GetDbSchemaPassword();
            set => SchemaPasswordField = value;
        }

        private string TablespaceField;
        public virtual string DefaultTablespace
        {
            get => TablespaceField ?? Configuration.GetDbDefaultTablespace();
            set => TablespaceField = value;
        }

        private string TempTablespaceField;
        public virtual string TempTablespace
        {
            get => TempTablespaceField ?? Configuration.GetDbTempTablespace();
            set => TempTablespaceField = value;
        }

        private string ConnectionStringTemplateField;
        public virtual string ConnectionStringTemplate
        {
            get => ConnectionStringTemplateField ?? Configuration.GetDbConnectionStringTemplate();
            set => ConnectionStringTemplateField = value;
        }
    }
}