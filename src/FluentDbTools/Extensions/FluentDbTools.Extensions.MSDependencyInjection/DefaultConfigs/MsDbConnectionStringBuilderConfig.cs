using FluentDbTools.Common.Abstractions;
using FluentDbTools.Contracts.DefaultConfigs;
using FluentDbTools.Extensions.DbProvider;
using Microsoft.Extensions.Configuration;

namespace FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs
{
    
    public class MsDbConnectionStringBuilderConfig : DbConnectionStringBuilderConfig
    {
        public MsDbConnectionStringBuilderConfig(
            IConfiguration configuration,
            DefaultDbConfigValues defaultDbConfigValues = null)
           : base(defaultDbConfigValues ?? new MsDefaultDbConfigValues(configuration)) 
        {
        }
    }
}