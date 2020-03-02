using System.Collections.Generic;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Common.Abstractions.PrioritizedConfig;
using FluentDbTools.Contracts;
using Microsoft.Extensions.Configuration;

namespace FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs.NotInUse
{
    internal class MsDbConfigDatabaseTargets : DbConfigDatabaseTargets
    {
        public MsDbConfigDatabaseTargets(
            IConfiguration configuration,
            DefaultDbConfigValues dbConfigDatabaseTargets = null,
            IPrioritizedConfigValues prioritizedConfigValues = null,
            IEnumerable<IPrioritizedConfigKeys> prioritizedConfigKeys = null)
            : base(dbConfigDatabaseTargets ?? new MsDefaultDbConfigValues(configuration, prioritizedConfigValues, prioritizedConfigKeys))
        {
        }

    }
}