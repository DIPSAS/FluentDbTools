using FluentDbTools.Common.Abstractions;
using FluentDbTools.Contracts;
using Microsoft.Extensions.Configuration;

namespace FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs.NotInUse
{
    internal class MsDbConfigDatabaseTargets : DbConfigDatabaseTargets
    {
        public MsDbConfigDatabaseTargets(
            IConfiguration configuration,
            DefaultDbConfigValues dbConfigDatabaseTargets = null,
            IPrioritizedConfigValues prioritizedConfigValues = null)
            : base(dbConfigDatabaseTargets ?? new MsDefaultDbConfigValues(configuration, prioritizedConfigValues))
        {
        }

    }
}