using FluentDbTools.Extensions.DbProvider;
using Microsoft.Extensions.Configuration;

namespace FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs
{
    public class MsDbConfig : DbConfig
    {
        public MsDbConfig(
            IConfiguration configuration, 
            MsDefaultDbConfigValues defaultDbConfigValues = null) :
            base(defaultDbConfigValues ?? new MsDefaultDbConfigValues(configuration))
        {
        }
    }
}