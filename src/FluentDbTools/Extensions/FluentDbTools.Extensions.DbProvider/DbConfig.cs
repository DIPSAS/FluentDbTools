using System;
using System.Collections.Generic;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Contracts;

namespace FluentDbTools.Extensions.DbProvider
{
    public class DbConfig : DbConnectionStringBuilderConfig, IDbConfig
    {
        protected IDictionary<string, string> AllConfigValuesField;
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

        public virtual IDictionary<string, string> GetAllDatabaseConfigValues(bool reload = false)
        {
            if (AllConfigValuesField == null || reload)
            {
                AllConfigValuesField = new Dictionary<string, string>();
            }

            return AllConfigValuesField;
        }

        public virtual string GetSchemaPrefixUniqueId()
        {
            return null;
        }
    }
}