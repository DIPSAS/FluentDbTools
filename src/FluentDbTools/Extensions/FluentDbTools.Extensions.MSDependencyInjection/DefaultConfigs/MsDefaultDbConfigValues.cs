using System;
using System.Collections.Generic;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Common.Abstractions.PrioritizedConfig;
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
        /// <param name="prioritizedConfigValues"></param>
        /// <param name="prioritizedConfigKeys"></param>
        public MsDefaultDbConfigValues(
            IConfiguration configuration, 
            IPrioritizedConfigValues prioritizedConfigValues = null, 
            IEnumerable<IPrioritizedConfigKeys> prioritizedConfigKeys = null)
        {
            prioritizedConfigValues = prioritizedConfigValues ?? new PrioritizedConfigValues(configuration.GetConfigValue, prioritizedConfigKeys ?? new []{ new PrioritizedConfigKeys()});
            // DbConfigDatabaseTargets defaults
            GetDefaultDbType = () => GetConfigValueSupportedDatabaseTypes(prioritizedConfigValues.GetDbType, configuration.GetDbType);
            GetDefaultSchema = () => GetConfigValueString(prioritizedConfigValues.GetDbSchema, configuration.GetDbSchema);
            GetDefaultDatabaseName = () => GetConfigValueString(prioritizedConfigValues.GetDbDatabaseName, configuration.GetDbDatabaseName);
            GetDefaultSchemaPrefixIdString = () => GetConfigValueString(prioritizedConfigValues.GetDbSchemaPrefixIdString, () => string.Empty);

            // DbConfigCredentials defaults
            GetDefaultUser = () => GetConfigValueString(prioritizedConfigValues.GetDbUser, configuration.GetDbUser);
            GetDefaultPassword = () => GetConfigValueString(prioritizedConfigValues.GetDbPassword, configuration.GetDbPassword);
            GetDefaultAdminUser = () => GetConfigValueString(prioritizedConfigValues.GetDbAdminUser, configuration.GetDbAdminUser);
            GetDefaultAdminPassword = () => GetConfigValueString(prioritizedConfigValues.GetDbAdminPassword, configuration.GetDbAdminPassword);

            // DbConnectionStringBuilderConfig defaults
            GetDefaultHostName = () => GetConfigValueString(prioritizedConfigValues.GetDbHostname, configuration.GetDbHostname);
            GetDefaultPort = () => GetConfigValueString(prioritizedConfigValues.GetDbPort, configuration.GetDbPort);
            GetDefaultDataSource = () => GetConfigValueString(prioritizedConfigValues.GetDbDataSource, configuration.GetDbDataSource);
            GetDefaultConnectionTimeoutInSecs = () => GetConfigValueString(prioritizedConfigValues.GetDbConnectionTimeout, configuration.GetDbConnectionTimeout);
            GetDefaultPooling = () => GetConfigValueBool(prioritizedConfigValues.GetDbPooling, configuration.GetDbPooling);

            // DbConfig defaults
            GetDefaultConnectionString = () => GetConfigValueString(prioritizedConfigValues.GetDbConnectionString, configuration.GetDbConnectionString);
            GetDefaultAdminConnectionString = () => GetConfigValueString(prioritizedConfigValues.GetDbAdminConnectionString, configuration.GetDbAdminConnectionString);

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