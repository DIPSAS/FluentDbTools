using FluentDbTools.Contracts;
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