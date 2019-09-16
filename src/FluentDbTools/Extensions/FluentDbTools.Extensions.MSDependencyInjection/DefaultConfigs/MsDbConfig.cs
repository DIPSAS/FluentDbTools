using System.Collections.Generic;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Contracts;
using FluentDbTools.Extensions.DbProvider;
using Microsoft.Extensions.Configuration;

namespace FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs
{
    /// <inheritdoc />
    public class MsDbConfig : DbConfig
    {
        private readonly IConfiguration Configuration;

        /// <inheritdoc />
        public MsDbConfig(
            IConfiguration configuration, 
            MsDefaultDbConfigValues defaultDbConfigValues = null) :
            base(defaultDbConfigValues ?? new MsDefaultDbConfigValues(configuration))
        {
            Configuration = configuration;
        }

        public override IDictionary<string, string> GetAllDatabaseConfigValues(bool reload = false)
        {
            if (AllConfigValuesField == null || reload)
            {
                AllConfigValuesField = Configuration.GetDbSection().GetDbAllConfigValues();
            }

            return AllConfigValuesField;
        }


        /// <inheritdoc />
        public override string GetSchemaPrefixId()
        {
            return GetAllDatabaseConfigValues().GetValue("schemaPrefix:Id") ??
                   Defaults.GetDefaultSchemaPrefixIdString?.Invoke() ?? string.Empty;
        }

        /// <inheritdoc />
        public override string GetSchemaPrefixUniqueId()
        {
            return GetAllDatabaseConfigValues().GetValue("schemaPrefix:UniqueId") ??
                   Defaults.GetDefaultSchemaPrefixUniqueIdString?.Invoke() ?? string.Empty;
        }

    }
}