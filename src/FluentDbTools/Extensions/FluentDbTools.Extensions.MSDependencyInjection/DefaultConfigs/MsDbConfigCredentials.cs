using FluentDbTools.Common.Abstractions;
using FluentDbTools.Contracts.DefaultConfigs;
using FluentDbTools.Extensions.DbProvider;
using Microsoft.Extensions.Configuration;

namespace FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs
{
    public class MsDbConfigCredentials : DbConfigCredentials
    {
        public MsDbConfigCredentials(
            IConfiguration configuration, 
            MsDefaultDbConfigValues defaultDbConfigValues = null)
            : base(defaultDbConfigValues ?? new MsDefaultDbConfigValues(configuration))
        {
        }
    }
}