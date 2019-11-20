using System;
using System.Collections.Generic;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Contracts;

namespace FluentDbTools.Extensions.DbProvider
{
    /// <inheritdoc cref="IDbConfig" />
    public class DbConfig : DbConnectionStringBuilderConfig, IDbConfig
    {
#pragma warning disable 1591
        protected IDictionary<string, string> AllConfigValuesField;
#pragma warning restore 1591

        /// <inheritdoc />
        public DbConfig(
            DefaultDbConfigValues defaultDbConfigValues = null,
            DbConfigCredentials dbConfigCredentials = null)
            : base(defaultDbConfigValues, dbConfigCredentials)
        {

        }

        private string ConnectionStringField;

        /// <inheritdoc />
        public virtual string ConnectionString
        {
            get => ConnectionStringField ?? Defaults.GetDefaultConnectionString();
            set => ConnectionStringField = value;
        }

        private string AdminConnectionStringField;

        /// <inheritdoc />
        public virtual string AdminConnectionString
        {
            get => AdminConnectionStringField ?? Defaults.GetDefaultAdminConnectionString();
            set => AdminConnectionStringField = value;
        }

        /// <inheritdoc />
        public virtual IDictionary<string, string> GetAllDatabaseConfigValues(bool reload = false)
        {
            if (AllConfigValuesField == null || reload)
            {
                AllConfigValuesField = new Dictionary<string, string>();
            }

            return AllConfigValuesField;
        }

        /// <inheritdoc />
        protected override void OnConfigurationChanged(Func<string[], string> getValueFunc)
        {
            AdminConnectionStringField = null;
            ConnectionStringField = null;
            base.OnConfigurationChanged(getValueFunc);

        }
    }
}