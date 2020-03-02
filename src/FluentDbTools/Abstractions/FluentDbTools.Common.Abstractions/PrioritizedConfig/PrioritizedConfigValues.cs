using System;

namespace FluentDbTools.Common.Abstractions.PrioritizedConfig
{
    public class PrioritizedConfigValues : IPrioritizedConfigValues 
    {
        private readonly IPrioritizedConfigKeys PrioritizedConfigKeys;
        private readonly Func<string[], string> GetConfigValueFunc;

        public PrioritizedConfigValues(
            Func<string[], string> getConfigValueFunc = null,
            IPrioritizedConfigKeys prioritizedConfigKeys = null)
        {
            PrioritizedConfigKeys = prioritizedConfigKeys;
            GetConfigValueFunc = getConfigValueFunc;
        }

        public SupportedDatabaseTypes? GetDbType()
        {

            var value = GetConfigValue(PrioritizedConfigKeys.GetDbTypeKeys());
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
            return GetConfigValue(PrioritizedConfigKeys.GetDbSchemaKeys());
        }

        public string GetDbSchemaPrefixIdString()
        {
            return GetConfigValue(PrioritizedConfigKeys.GetDbSchemaPrefixIdStringKeys());
        }

        public virtual string GetDbDatabaseName()
        {
            return GetConfigValue(PrioritizedConfigKeys.GetDbDatabaseNameKeys());
        }

        public virtual string GetDbUser()
        {
            return GetConfigValue(PrioritizedConfigKeys.GetDbUserKeys());
        }

        public virtual string GetDbPassword()
        {
            return GetConfigValue(PrioritizedConfigKeys.GetDbPasswordKeys());
        }

        public virtual string GetDbAdminUser()
        {
            return GetConfigValue(PrioritizedConfigKeys.GetDbAdminUserKeys());
        }

        public virtual string GetDbAdminPassword()
        {
            return GetConfigValue(PrioritizedConfigKeys.GetDbAdminPasswordKeys());
        }

        public virtual string GetDbHostname()
        {
            return GetConfigValue(PrioritizedConfigKeys.GetDbHostnameKeys());
        }

        public virtual string GetDbPort()
        {
            return GetConfigValue(PrioritizedConfigKeys.GetDbPortKeys());
        }

        public virtual string GetDbDataSource()
        {
            return GetConfigValue(PrioritizedConfigKeys.GetDbDataSourceKeys());
        }

        public virtual string GetDbConnectionTimeout()
        {
            return GetConfigValue(PrioritizedConfigKeys.GetDbConnectionTimeoutKeys());
        }

        public virtual bool? GetDbPooling()
        {
            var value = GetConfigValue(PrioritizedConfigKeys.GetDbPoolingKeys());

            return value.IsNotEmpty() ? (bool?) value.IsTrue() : null;
        }

        public virtual string GetDbConnectionString()
        {
            return GetConfigValue(PrioritizedConfigKeys.GetDbConnectionStringKeys());
        }

        public virtual string GetDbAdminConnectionString()
        {
            return GetConfigValue(PrioritizedConfigKeys.GetDbAdminConnectionStringKeys());
        }

        private string GetConfigValue(params string[] keys)
        {
            return keys == null || GetConfigValueFunc == null ? null : GetConfigValueFunc?.Invoke(keys);
        }
    }
}