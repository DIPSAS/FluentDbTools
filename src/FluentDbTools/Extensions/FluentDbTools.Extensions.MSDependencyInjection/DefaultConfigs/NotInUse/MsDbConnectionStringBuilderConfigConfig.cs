﻿using System.Collections.Generic;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Common.Abstractions.PrioritizedConfig;
using FluentDbTools.Contracts;
using Microsoft.Extensions.Configuration;

namespace FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs.NotInUse
{
    /// <inheritdoc />
    internal class MsDbConnectionStringBuilderConfig : DbConnectionStringBuilderConfig
    {
        /// <inheritdoc />
        public MsDbConnectionStringBuilderConfig(
            IConfiguration configuration,
            DefaultDbConfigValues defaultDbConfigValues = null,
            DbConfigCredentials dbConfigCredentials = null,
            IPrioritizedConfigValues prioritizedConfigValues = null,
            IEnumerable<IPrioritizedConfigKeys> prioritizedConfigKeys = null)
           : base(defaultDbConfigValues ?? new MsDefaultDbConfigValues(configuration, prioritizedConfigValues, prioritizedConfigKeys), dbConfigCredentials)
        {

        }
    }
}