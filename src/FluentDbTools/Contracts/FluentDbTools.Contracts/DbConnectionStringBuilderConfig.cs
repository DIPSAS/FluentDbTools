using FluentDbTools.Common.Abstractions;

namespace FluentDbTools.Contracts
{
    public class DbConnectionStringBuilderConfig : DbConfigDatabaseTargets, IDbConnectionStringBuilderConfig
    {
        public DbConfigCredentials DbConfigCredentials { get; set; }

        public DbConnectionStringBuilderConfig(DefaultDbConfigValues defaultDbConfigValues = null)
            : base(defaultDbConfigValues)
        {
            if (Defaults.GetDefaultSchema == null)
            {
                Defaults.GetDefaultSchema = () => User;
            }

            DbConfigCredentials = new DbConfigCredentials(Defaults);
        }

        public virtual string User
        {
            get => DbConfigCredentials.User;
            set => DbConfigCredentials.User = value;
        }

        public virtual string Password
        {
            get => DbConfigCredentials.Password;
            set => DbConfigCredentials.Password = value;
        }

        public virtual string AdminUser
        {
            get => DbConfigCredentials.AdminUser;
            set => DbConfigCredentials.AdminUser = value;
        }

        public virtual string AdminPassword
        {
            get => DbConfigCredentials.AdminPassword;
            set => DbConfigCredentials.AdminPassword = value;
        }

        private string HostnameField;
        public virtual string Hostname
        {
            get => HostnameField ?? Defaults.GetDefaultHostName?.Invoke();
            set => HostnameField = value;
        }

        private string PortField;
        public virtual string Port
        {
            get => PortField ?? Defaults.GetDefaultPort?.Invoke();
            set => PortField = value;
        }

        private bool? PoolingField;
        public virtual bool Pooling
        {
            get => PoolingField ?? Defaults.GetDefaultPooling?.Invoke() ?? true;
            set => PoolingField = value;
        }

        private string DatasourceField;
        public virtual string Datasource
        {
            get => DatasourceField ?? Defaults.GetDefaultDatasource?.Invoke();
            set => DatasourceField = value;
        }

        private string ConnectionTimeoutInSecsField;
        public virtual string ConnectionTimeoutInSecs
        {
            get => ConnectionTimeoutInSecsField ?? Defaults.GetDefaultConnectionTimeoutInSecs?.Invoke();
            set => ConnectionTimeoutInSecsField = value;
        }
    }
}