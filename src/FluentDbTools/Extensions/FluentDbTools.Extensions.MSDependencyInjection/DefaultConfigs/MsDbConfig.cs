using System;
using System.Collections.Generic;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Contracts;
using FluentDbTools.Extensions.DbProvider;
using Microsoft.Extensions.Configuration;

namespace FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs
{
    /// <inheritdoc />
    internal class MsDbConfig : DbConfig
    {
        internal readonly IConfiguration Configuration;

        /// <inheritdoc />
        public MsDbConfig(
            IConfiguration configuration,
            IConfigurationChangedHandler configurationChangedHandler = null,
            DefaultDbConfigValues defaultDbConfigValues = null,
            DbConfigCredentials dbConfigCredentials = null) :
            base(defaultDbConfigValues ?? new MsDefaultDbConfigValues(configuration), dbConfigCredentials)
        {
            Configuration = configuration;
            configurationChangedHandler?.RegisterConfigurationChangedCallback(OnConfigurationChanged);
        }

        /// <inheritdoc />
        protected override void OnConfigurationChanged(Func<string[], string> getValueFunc)
        {
            GetAllDatabaseConfigValues(true);
            base.OnConfigurationChanged(getValueFunc);
        }

        /// <inheritdoc />
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