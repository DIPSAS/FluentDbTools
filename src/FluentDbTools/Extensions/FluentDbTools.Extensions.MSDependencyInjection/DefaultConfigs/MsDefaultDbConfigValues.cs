using System;
using System.Collections.Generic;
using System.Linq;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Common.Abstractions.PrioritizedConfig;
using FluentDbTools.Contracts;
using Microsoft.Extensions.Configuration;

namespace FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs
{
    /// <inheritdoc />
    internal class MsDefaultDbConfigValues : DefaultDbConfigValues
    {
        private readonly IConfiguration Configuration;
        private IDictionary<string, string> AllConfigValuesField;

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
            Configuration = configuration;

            PrioritizedConfigKeys = ToPrioritizedConfigKeysArray();
            
            prioritizedConfigValues = prioritizedConfigValues ?? new PrioritizedConfigValues(configuration.GetConfigValue, PrioritizedConfigKeys.Any() ? PrioritizedConfigKeys : new[] { new PrioritizedConfigKeys() });
            // DbConfigDatabaseTargets defaults
            GetDefaultDbType = () => GetConfigValueSupportedDatabaseTypes(prioritizedConfigValues.GetDbType, configuration.GetDbType);
            GetDefaultSchema = () => GetConfigValueString(prioritizedConfigValues.GetDbSchema, configuration.GetDbSchema);
            GetDefaultDatabaseName = () => GetConfigValueString(prioritizedConfigValues.GetDbDatabaseName, configuration.GetDbDatabaseName);
            GetDefaultSchemaPrefixIdString = () => GetConfigValueString(prioritizedConfigValues.GetDbSchemaPrefixIdString, () => GetAllDatabaseConfigValues().GetValue("schemaPrefix:Id") ?? GetAllDatabaseConfigValues(sectionName: "database:migration").GetValue("schemaPrefix:Id"));
            GetDefaultSchemaPrefixUniqueIdString = () => GetConfigValueString(prioritizedConfigValues.GetDbSchemaUniquePrefixIdString, () => GetAllDatabaseConfigValues(sectionName: "database:migration").GetValue("schemaPrefix:UniqueId") ?? GetAllDatabaseConfigValues().GetValue("schemaPrefix:UniqueId"));

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
            GetDefaultPoolingKeyValues = () => GetConfigValueDictionary(prioritizedConfigValues.GetDbPoolingKeyValues, configuration.GetDbPoolingKeyValues);

            // DbConfig defaults
            GetDefaultConnectionString = () => GetConfigValueString(prioritizedConfigValues.GetDbConnectionString, configuration.GetDbConnectionString);
            GetDefaultAdminConnectionString = () => GetConfigValueString(prioritizedConfigValues.GetDbAdminConnectionString, configuration.GetDbAdminConnectionString);

            string GetConfigValueString(Func<string> firstPriority, Func<string> nextPriority)
            {
                var value = firstPriority?.Invoke() ?? nextPriority?.Invoke();
                return string.IsNullOrEmpty(value) ? null : value;
            }

            bool GetConfigValueBool(Func<bool?> firstPriority, Func<bool> nextPriority)
            {
                return firstPriority?.Invoke() ?? nextPriority.Invoke();
            }

            SupportedDatabaseTypes GetConfigValueSupportedDatabaseTypes(Func<SupportedDatabaseTypes?> firstPriority, Func<SupportedDatabaseTypes> nextPriority)
            {
                return firstPriority?.Invoke() ?? nextPriority.Invoke();
            }

            IDictionary<string, string> GetConfigValueDictionary(Func<IDictionary<string, string>> firstPriority, Func<IDictionary<string, string>> nextPriority)
            {
                return firstPriority?.Invoke() ?? nextPriority.Invoke();
            }

            IPrioritizedConfigKeys[] ToPrioritizedConfigKeysArray()
            {
                var array = prioritizedConfigKeys as IPrioritizedConfigKeys[] ?? prioritizedConfigKeys?.ToArray() ?? prioritizedConfigValues?.GetPrioritizedConfigKeys() ?? GetDefaultPrioritizedConfigKeys();
                return array.Any() == false ? GetDefaultPrioritizedConfigKeys() : array.Distinct().OrderBy(x => x.Order).ToArray();
            }
        }

        /// <summary>
        /// GetAllMigrationConfigValues() : Get al values and subValues from configuration "database:migration". 
        /// </summary>
        public override IDictionary<string, string> GetAllDatabaseConfigValues(bool reload = false, string sectionName = null)
        {
            if (sectionName != null)
            {
                var section = Configuration.GetSection(sectionName) ??
                                    Configuration.GetDbSection().GetSection(sectionName);

                return section?.GetDbAllConfigValues();
            }

            if (AllConfigValuesField == null || reload)
            {
                AllConfigValuesField = Configuration.GetDbSection().GetDbAllConfigValues();
            }

            return AllConfigValuesField;
        }

        private IPrioritizedConfigKeys[] GetDefaultPrioritizedConfigKeys()
        {
            return new IPrioritizedConfigKeys[] { new PrioritizedConfigKeys() };
        }
    }
}