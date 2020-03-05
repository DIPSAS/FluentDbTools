using System;
using System.Collections.Generic;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Common.Abstractions.PrioritizedConfig;
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
            DbConfigCredentials dbConfigCredentials = null,
            IPrioritizedConfigValues prioritizedConfigValues = null,
            IEnumerable<IPrioritizedConfigKeys> prioritizedConfigKeys = null) :
            base(defaultDbConfigValues ?? new MsDefaultDbConfigValues(configuration,prioritizedConfigValues, prioritizedConfigKeys), dbConfigCredentials)
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
    }
}