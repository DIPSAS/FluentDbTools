using FluentDbTools.Contracts;
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