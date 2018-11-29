using DIPS.FluentDbTools.Common.Abstractions;
using Microsoft.Extensions.Configuration;

namespace DIPS.FluentDbTools.Example.Config
{
    public class DbConfig : IDbConfig
    {
        private readonly IConfiguration Configuration;

        public DbConfig(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public SupportedDatabaseTypes DbType => Configuration.GetDbType();
        public string User => Configuration.GetDbUser();
        public string Password => Configuration.GetDbPassword();
        public string AdminUser => Configuration.GetDbAdminUser();
        public string AdminPassword => Configuration.GetDbAdminPassword();
        public string Hostname => Configuration.GetDbHostname();
        public string Port => Configuration.GetDbPort();
        public string DatabaseConnectionName => Configuration.GetDbConnectionName();
        public bool Pooling => Configuration.GetDbPooling();
        public string Schema => Configuration.GetDbSchema();
        public string DefaultTablespace => Configuration.GetDbDefaultTablespace();
        public string TempTablespace => Configuration.GetDbTempTablespace();
    }
}