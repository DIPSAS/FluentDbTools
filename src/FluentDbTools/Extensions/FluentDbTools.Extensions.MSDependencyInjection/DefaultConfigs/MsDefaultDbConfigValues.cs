using FluentDbTools.Contracts;
using Microsoft.Extensions.Configuration;

namespace FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs
{
    /// <inheritdoc />
    internal class MsDefaultDbConfigValues : DefaultDbConfigValues
    {
        /// <summary>
        /// Overrides all default functions in <see cref="DefaultDbConfigValues"/> by <paramref name="configuration"/> extension methods
        /// </summary>
        /// <param name="configuration"></param>
        public MsDefaultDbConfigValues(IConfiguration configuration)
        {
            // DbConfigDatabaseTargets defaults
            GetDefaultDbType = configuration.GetDbType;
            GetDefaultSchema = configuration.GetDbSchema;
            GetDefaultDatabaseName = configuration.GetDbDatabaseName;
            GetDefaultSchemaPrefixIdString = () => string.Empty;

            // DbConfigCredentials defaults
            GetDefaultUser = configuration.GetDbUser;
            GetDefaultPassword = configuration.GetDbPassword;
            GetDefaultAdminUser = configuration.GetDbAdminUser;
            GetDefaultAdminPassword = configuration.GetDbAdminPassword;

            // DbConnectionStringBuilderConfig defaults
            GetDefaultHostName = configuration.GetDbHostname;
            GetDefaultPort = configuration.GetDbPort;
            GetDefaultDataSource = configuration.GetDbDataSource;
            GetDefaultConnectionTimeoutInSecs = configuration.GetDbConnectionTimeout;
            GetDefaultPooling = configuration.GetDbPooling;

            // DbConfig defaults
            GetDefaultConnectionString = configuration.GetDbConnectionString;
            GetDefaultAdminConnectionString = configuration.GetDbAdminConnectionString;
        }
    }
}