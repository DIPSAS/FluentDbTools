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
        public SupportedDatabaseTypes DbType
        {
            get => DbTypeField ?? Configuration.GetDbType();
            set => DbTypeField = value;
        }
        
        private string UserField;
        public string User
        {
            get => UserField ?? Configuration.GetDbUser();
            set => UserField = value;
        }
        
        private string PasswordField;
        public string Password
        {
            get => PasswordField ?? Configuration.GetDbPassword();
            set => PasswordField = value;
        }
        
        private string AdminUserField;
        public string AdminUser
        {
            get => AdminUserField ?? Configuration.GetDbAdminUser();
            set => AdminUserField = value;
        }
        
        private string AdminPasswordField;
        public string AdminPassword
        {
            get => AdminPasswordField ?? Configuration.GetDbAdminPassword();
            set => AdminPasswordField = value;
        }

        private string HostnameField;
        public string Hostname
        {
            get => HostnameField ?? Configuration.GetDbHostname();
            set => HostnameField = value;
        }

        private string PortField;
        public string Port
        {
            get => PortField ?? Configuration.GetDbPort();
            set => PortField = value;
        }

        private string DatabaseConnectionNameField;
        public string DatabaseConnectionName
        {
            get => DatabaseConnectionNameField ?? Configuration.GetDbConnectionName();
            set => DatabaseConnectionNameField = value;
        }

        private bool? PoolingField;
        public bool Pooling
        {
            get => PoolingField ?? Configuration.GetDbPooling();
            set => PoolingField = value;
        }

        private string SchemaField;
        public string Schema
        {
            get => SchemaField ?? Configuration.GetDbSchema();
            set => SchemaField = value;
        }

        private string SchemaPasswordField;
        public string SchemaPassword
        {
            get => SchemaPasswordField ?? Configuration.GetDbSchemaPassword();
            set => SchemaPasswordField = value;
        }

        private string TablespaceField;
        public string DefaultTablespace
        {
            get => TablespaceField ?? Configuration.GetDbDefaultTablespace();
            set => TablespaceField = value;
        }

        private string TempTablespaceField;
        public string TempTablespace
        {
            get => TempTablespaceField ?? Configuration.GetDbTempTablespace();
            set => TempTablespaceField = value;
        }

        private string ConnectionStringTemplateField;
        public string ConnectionStringTemplate
        {
            get => ConnectionStringTemplateField ?? Configuration.GetDbConnectionStringTemplate();
            set => ConnectionStringTemplateField = value;
        }
    }
}