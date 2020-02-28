using System;
using FluentDbTools.Common.Abstractions;
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
        /// <param name="prioritizedConfig"></param>
        public MsDefaultDbConfigValues(IConfiguration configuration, IPrioritizedConfigValues prioritizedConfig = null)
        {
            prioritizedConfig = prioritizedConfig ?? new PrioritizedConfigValues();
            // DbConfigDatabaseTargets defaults
            GetDefaultDbType = () => GetConfigValueSupportedDatabaseTypes(prioritizedConfig.GetDbType, configuration.GetDbType);
            GetDefaultSchema = () => GetConfigValueString(prioritizedConfig.GetDbSchema, configuration.GetDbSchema);
            GetDefaultDatabaseName = () => GetConfigValueString(prioritizedConfig.GetDbDatabaseName, configuration.GetDbDatabaseName);
            GetDefaultSchemaPrefixIdString = () => GetConfigValueString(prioritizedConfig.GetDbSchemaPrefixIdString, () => string.Empty);

            // DbConfigCredentials defaults
            GetDefaultUser = () => GetConfigValueString(prioritizedConfig.GetDbUser, configuration.GetDbUser);
            GetDefaultPassword = () => GetConfigValueString(prioritizedConfig.GetDbPassword, configuration.GetDbPassword);
            GetDefaultAdminUser = () => GetConfigValueString(prioritizedConfig.GetDbAdminUser, configuration.GetDbAdminUser);
            GetDefaultAdminPassword = () => GetConfigValueString(prioritizedConfig.GetDbAdminPassword, configuration.GetDbAdminPassword);

            // DbConnectionStringBuilderConfig defaults
            GetDefaultHostName = () => GetConfigValueString(prioritizedConfig.GetDbHostname, configuration.GetDbHostname);
            GetDefaultPort = () => GetConfigValueString(prioritizedConfig.GetDbPort, configuration.GetDbPort);
            GetDefaultDataSource = () => GetConfigValueString(prioritizedConfig.GetDbDataSource, configuration.GetDbDataSource);
            GetDefaultConnectionTimeoutInSecs = () => GetConfigValueString(prioritizedConfig.GetDbConnectionTimeout, configuration.GetDbConnectionTimeout);
            GetDefaultPooling = () => GetConfigValueBool(prioritizedConfig.GetDbPooling, configuration.GetDbPooling);

            // DbConfig defaults
            GetDefaultConnectionString = () => GetConfigValueString(prioritizedConfig.GetDbConnectionString, configuration.GetDbConnectionString);
            GetDefaultAdminConnectionString = () => GetConfigValueString(prioritizedConfig.GetDbAdminConnectionString, configuration.GetDbAdminConnectionString);

            string GetConfigValueString(Func<string> firstPriority, Func<string> nextPriority)
            {
                return firstPriority?.Invoke() ?? nextPriority?.Invoke();
            }

            bool GetConfigValueBool(Func<bool?> firstPriority, Func<bool> nextPriority)
            {
                return firstPriority?.Invoke() ?? nextPriority.Invoke();
            }

            SupportedDatabaseTypes GetConfigValueSupportedDatabaseTypes(Func<SupportedDatabaseTypes?> firstPriority, Func<SupportedDatabaseTypes> nextPriority)
            {
                return firstPriority?.Invoke() ?? nextPriority.Invoke();
            }


        }

    }
}