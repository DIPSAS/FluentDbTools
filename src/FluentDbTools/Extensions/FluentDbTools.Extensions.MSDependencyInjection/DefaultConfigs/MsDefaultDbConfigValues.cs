using FluentDbTools.Contracts;
using Microsoft.Extensions.Configuration;

namespace FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs
{
    public class MsDefaultDbConfigValues : DefaultDbConfigValues
    {
        public MsDefaultDbConfigValues(IConfiguration configuration)
        {
            // DbConfigDatabaseTargets defaults
            GetDefaultDbType = configuration.GetDbType;
            GetDefaultSchema = configuration.GetDbSchema;
            GetDefaulDatabaseName = configuration.GetDbDatabaseName;

            // DbConfigCredentials defaults
            GetDefaultUser = configuration.GetDbUser;
            GetDefaultPassword = configuration.GetDbPassword;
            GetDefaultAdminUser = configuration.GetDbAdminUser;
            GetDefaultAdminPassword = configuration.GetDbAdminPassword;

            // DbConnectionStringBuilderConfig defaults
            GetDefaultHostName = configuration.GetDbHostname;
            GetDefaultPort = configuration.GetDbPort;
            GetDefaultDatasource = configuration.GetDbDataSource;
            GetDefaultConnectionTimeoutInSecs = configuration.GetDbConnectionTimeout;
            GetDefaultPooling = configuration.GetDbPooling;

            // DbConfig defaults
            GetDefaultConnectionString = configuration.GetDbConnectionString;
            GetDefaultAdminConnectionString = configuration.GetDbAdminConnectionString;
        }
    }
}