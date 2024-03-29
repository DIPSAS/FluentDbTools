﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        private string[] AdditionalRolesGrantsField = null;
        private string[] UserGrantsField = null;

        /// <inheritdoc />
        public MsDbConfig(
            IConfiguration configuration, IConfigurationChangedHandler configurationChangedHandler = null,
            DefaultDbConfigValues defaultDbConfigValues = null,
            DbConfigCredentials dbConfigCredentials = null,
            IPrioritizedConfigValues prioritizedConfigValues = null,
            IEnumerable<IPrioritizedConfigKeys> prioritizedConfigKeys = null,
            IConfigurationDelimiter delimiter = null) :
            base(defaultDbConfigValues ?? new MsDefaultDbConfigValues(configuration,prioritizedConfigValues, prioritizedConfigKeys), dbConfigCredentials)
        {
            Configuration = configuration;
            configurationChangedHandler?.RegisterConfigurationChangedCallback(OnConfigurationChanged);

            ConfigurationDelimiter = delimiter?.Delimiter ?? ":";
        }

        /// <inheritdoc />
        public override string GetConfigValue(params string[] keys)
        {
            return Configuration.GetConfigValue(keys);
        }

        /// <inheritdoc />
        public override IPrioritizedConfigKeys[] GetPrioritizedConfigKeys()
        {
            return Defaults.PrioritizedConfigKeys;
        }

        /// <inheritdoc />
        protected override void OnConfigurationChanged(Func<string[], string> getValueFunc)
        {
            GetAllDatabaseConfigValues(true);
            base.OnConfigurationChanged(getValueFunc);
        }

        /// <inheritdoc />
        public override string[] AdditionalRolesGrants => AdditionalRolesGrantsField ?? (AdditionalRolesGrantsField = this.GetAdditionalRolesGrantsFromConfig()?.ToArray());

        /// <inheritdoc />
        public override string[] UserGrants => UserGrantsField ?? (UserGrantsField = this.GetUserGrantsFromConfig()?.ToArray());
    }
}