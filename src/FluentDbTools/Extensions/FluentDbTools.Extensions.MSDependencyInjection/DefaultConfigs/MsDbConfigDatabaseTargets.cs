using System.Security;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Contracts;
using FluentDbTools.Extensions.DbProvider;
using Microsoft.Extensions.Configuration;

namespace FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs
{
    public class MsDbConfigDatabaseTargets : DbConfigDatabaseTargets
    {
        public MsDbConfigDatabaseTargets(
            IConfiguration configuration, 
            DefaultDbConfigValues dbConfigDatabaseTargets = null)
            : base(dbConfigDatabaseTargets ?? new MsDefaultDbConfigValues(configuration))
        {
        }

    }
}