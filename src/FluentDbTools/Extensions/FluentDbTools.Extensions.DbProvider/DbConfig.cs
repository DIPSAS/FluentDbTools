using System;
using System.Collections.Generic;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Contracts;

namespace FluentDbTools.Extensions.DbProvider
{
    /// <inheritdoc cref="IDbConfig" />
    public class DbConfig : DbConnectionStringBuilderConfig, IDbConfig
    {

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
            return Defaults?.GetAllDatabaseConfigValues(reload) ?? new Dictionary<string, string>();
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