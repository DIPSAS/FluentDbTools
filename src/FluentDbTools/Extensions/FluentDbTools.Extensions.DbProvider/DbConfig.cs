using System;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Contracts;

namespace FluentDbTools.Extensions.DbProvider
{
    public class DbConfig : DbConnectionStringBuilderConfig, IDbConfig
    {
        public DbConfig(DefaultDbConfigValues defaultDbConfigValues = null)
            : base(defaultDbConfigValues)
        {
            
        }
        private string ConnectionStringField;
        public virtual string ConnectionString
        {
            get => ConnectionStringField ?? Defaults.GetDefaultConnectionString();
            set => ConnectionStringField = value;
        }

        private string AdminConnectionStringField;

        public virtual string AdminConnectionString
        {
            get => AdminConnectionStringField ?? Defaults.GetDefaultAdminConnectionString();
            set => AdminConnectionStringField = value;
        }
    }
}