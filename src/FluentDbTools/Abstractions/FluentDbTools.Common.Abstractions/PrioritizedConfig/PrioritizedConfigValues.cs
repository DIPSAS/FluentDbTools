using System;
using System.Collections.Generic;
using System.Linq;

namespace FluentDbTools.Common.Abstractions.PrioritizedConfig
{
    public class PrioritizedConfigValues : IPrioritizedConfigValues 
    {
        private readonly IPrioritizedConfigKeys[] PrioritizedConfigKeys;
        private readonly Func<string[], string> GetConfigValueFunc;

        public PrioritizedConfigValues(
            Func<string[], string> getConfigValueFunc = null,
            IEnumerable<IPrioritizedConfigKeys> prioritizedConfigKeys = null)
        {
            PrioritizedConfigKeys = prioritizedConfigKeys?.Distinct()?.ToArray() ?? Array.Empty<IPrioritizedConfigKeys>();
            GetConfigValueFunc = getConfigValueFunc;
        }

        public SupportedDatabaseTypes? GetDbType()
        {

            var value = GetConfigValue(PrioritizedConfigKeys?.Where(x => x?.GetDbTypeKeys() != null)?
                .SelectMany(x => x?.GetDbTypeKeys())?.ToArray());
            if (value.IsEmpty())
            {
                return null;
            }

            return Enum.TryParse(value, true, out SupportedDatabaseTypes dbType)
                ? (SupportedDatabaseTypes?) dbType
                : null;
        }

        public virtual string GetDbSchema()
        {
            return GetConfigValue(PrioritizedConfigKeys?.Where(x => x?.GetDbSchemaKeys() != null)?
                .SelectMany(x => x?.GetDbSchemaKeys())?.ToArray());
        }

        public string GetDbSchemaPrefixIdString()
        {
            return GetConfigValue(PrioritizedConfigKeys?.Where(x => x?.GetDbSchemaPrefixIdStringKeys() != null)?
                .SelectMany(x => x?.GetDbSchemaPrefixIdStringKeys())?.ToArray());
        }

        public virtual string GetDbDatabaseName()
        {
            return GetConfigValue(PrioritizedConfigKeys?.Where(x => x?.GetDbDatabaseNameKeys() != null)?
                .SelectMany(x => x?.GetDbDatabaseNameKeys())?.ToArray());
        }

        public virtual string GetDbUser()
        {
            return GetConfigValue(PrioritizedConfigKeys?.Where(x => x?.GetDbUserKeys() != null)?
                .SelectMany(x => x?.GetDbUserKeys())?.ToArray());
        }

        public virtual string GetDbPassword()
        {
            return GetConfigValue(PrioritizedConfigKeys?.Where(x => x?.GetDbPasswordKeys() != null)?
                .SelectMany(x => x?.GetDbPasswordKeys())?.ToArray());
        }

        public virtual string GetDbAdminUser()
        {
            return GetConfigValue(PrioritizedConfigKeys?.Where(x => x?.GetDbAdminUserKeys() != null)?
                .SelectMany(x => x?.GetDbAdminUserKeys())?.ToArray());
        }

        public virtual string GetDbAdminPassword()
        {
            return GetConfigValue(PrioritizedConfigKeys?.Where(x => x?.GetDbAdminPasswordKeys() != null)?
                .SelectMany(x => x?.GetDbAdminPasswordKeys())?.ToArray());
        }

        public virtual string GetDbHostname()
        {
            return GetConfigValue(PrioritizedConfigKeys?.Where(x => x?.GetDbHostnameKeys() != null)?
                .SelectMany(x => x?.GetDbHostnameKeys())?.ToArray());
        }

        public virtual string GetDbPort()
        {
            return GetConfigValue(PrioritizedConfigKeys?.Where(x => x?.GetDbHostnameKeys() != null)?
                .SelectMany(x => x?.GetDbHostnameKeys())?.ToArray());
        }

        public virtual string GetDbDataSource()
        {
            return GetConfigValue(PrioritizedConfigKeys?.Where(x => x?.GetDbDataSourceKeys() != null)?
                .SelectMany(x => x?.GetDbDataSourceKeys())?.ToArray());
        }

        public virtual string GetDbConnectionTimeout()
        {
            return GetConfigValue(PrioritizedConfigKeys?.Where(x => x?.GetDbConnectionTimeoutKeys() != null)?
                .SelectMany(x => x?.GetDbConnectionTimeoutKeys())?.ToArray());
        }

        public virtual bool? GetDbPooling()
        {
            var value = GetConfigValue(PrioritizedConfigKeys?.Where(x => x?.GetDbPoolingKeys() != null)?
                .SelectMany(x => x?.GetDbPoolingKeys())?.ToArray());

            return value.IsNotEmpty() ? (bool?) value.IsTrue() : null;
        }

        public virtual string GetDbConnectionString()
        {
            return GetConfigValue(PrioritizedConfigKeys?.Where(x => x?.GetDbConnectionStringKeys() != null)?
                .SelectMany(x => x?.GetDbConnectionStringKeys())?.ToArray());
        }

        public virtual string GetDbAdminConnectionString()
        {
            return GetConfigValue(PrioritizedConfigKeys?.Where(x => x?.GetDbAdminConnectionStringKeys() != null)?
                .SelectMany(x => x?.GetDbAdminConnectionStringKeys())?.ToArray());
        }

        private string GetConfigValue(params string[] keys)
        {
            keys = keys?.Where(x => x != null).Distinct().ToArray();
            return keys == null || GetConfigValueFunc == null ? null : GetConfigValueFunc?.Invoke(keys);
        }
    }
}