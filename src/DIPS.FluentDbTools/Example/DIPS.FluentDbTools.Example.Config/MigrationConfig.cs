using DIPS.FluentDbTools.Common.Abstractions;
using Microsoft.Extensions.Configuration;

namespace DIPS.FluentDbTools.Example.Config
{
    public class MigrationConfig : IMigrationConfig
    {
        private readonly IConfiguration Configuration;

        public MigrationConfig(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public int Version => Configuration.GetMigrationVersion();
    }
}