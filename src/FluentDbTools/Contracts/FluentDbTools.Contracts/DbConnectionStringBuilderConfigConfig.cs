using System;
using System.Collections.Generic;
using FluentDbTools.Common.Abstractions;

namespace FluentDbTools.Contracts
{
    /// <inheritdoc cref="DbConfigDatabaseTargets" />
    /// <inheritdoc cref="IDbConnectionStringBuilderConfig" />
    public class DbConnectionStringBuilderConfig : DbConfigDatabaseTargets, IDbConnectionStringBuilderConfig
    {
        private string HostnameField;
        private string PortField;
        private string DataSourceField;
        private bool? PoolingField;
        private string ConnectionTimeoutInSecsField;
        private IDictionary<string, string> PoolingKeyValuesField;

        /// <inheritdoc />
        public DbConfigCredentials DbConfigCredentials { get; set; }

        /// <inheritdoc />
        public DbConnectionStringBuilderConfig(
            DefaultDbConfigValues defaultDbConfigValues = null,
            DbConfigCredentials dbConfigCredentials = null)
            : base(defaultDbConfigValues)
        {
            if (Defaults.GetDefaultSchema == null)
            {
                Defaults.GetDefaultSchema = () => User;
            }

            DbConfigCredentials = dbConfigCredentials ?? new DbConfigCredentials(Defaults);
        }

        /// <inheritdoc />
        public virtual string User
        {
            get => DbConfigCredentials.User;
            set => DbConfigCredentials.User = value;
        }

        /// <inheritdoc />
        public virtual string Password
        {
            get => DbConfigCredentials.Password;
            set => DbConfigCredentials.Password = value;
        }

        /// <inheritdoc />
        public virtual string AdminUser
        {
            get => DbConfigCredentials.AdminUser;
            set => DbConfigCredentials.AdminUser = value;
        }

        /// <inheritdoc />
        public virtual string AdminPassword
        {
            get => DbConfigCredentials.AdminPassword;
            set => DbConfigCredentials.AdminPassword = value;
        }

        /// <inheritdoc />
        public virtual string Hostname
        {
            get => HostnameField ?? Defaults.GetDefaultHostName?.Invoke();
            set => HostnameField = value;
        }

        /// <inheritdoc />
        public virtual string Port
        {
            get => PortField ?? Defaults.GetDefaultPort?.Invoke();
            set => PortField = value;
        }

        /// <inheritdoc />
        public virtual bool Pooling
        {
            get => PoolingField ?? Defaults.GetDefaultPooling?.Invoke() ?? true;
            set => PoolingField = value;
        }

        /// <inheritdoc />
        public IDictionary<string, string> PoolingKeyValues
        {
            get => PoolingKeyValuesField ?? Defaults.GetDefaultPoolingKeyValues?.Invoke();
            set => PoolingKeyValuesField = value;
        }
        
        /// <inheritdoc />
        public virtual string Datasource
        {
            get => DataSourceField ?? Defaults.GetDefaultDataSource?.Invoke();
            set => DataSourceField = value;
        }

        /// <inheritdoc />
        public virtual string ConnectionTimeoutInSecs
        {
            get => ConnectionTimeoutInSecsField ?? Defaults.GetDefaultConnectionTimeoutInSecs?.Invoke();
            set => ConnectionTimeoutInSecsField = value;
        }

        /// <inheritdoc />
        protected override void OnConfigurationChanged(Func<string[], string> getValueFunc)
        {
            ConnectionTimeoutInSecsField = null;
            DataSourceField = null;
            PoolingField = null;
            PortField = null;
            HostnameField = null;
            PoolingKeyValues = null;

            base.OnConfigurationChanged(getValueFunc);

            if (!DbConfigCredentials.IgnoreManualCallOnConfigurationChanged)
            {
                DbConfigCredentials.OnConfigurationChanged(getValueFunc);
            }

        }
    }
}