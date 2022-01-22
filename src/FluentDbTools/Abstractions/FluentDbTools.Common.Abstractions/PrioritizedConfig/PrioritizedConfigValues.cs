using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FluentDbTools.Common.Abstractions.PrioritizedConfig
{
    /// <summary>
    /// <inheritdoc cref="IPrioritizedConfigValues"/>
    /// <para>_______________________________________________________________________________</para>
    /// <para>All <see cref="IPrioritizedConfigKeys"/>-methods in this class return <c>null</c></para>
    /// <para>It is recommended to use this class as a base-class and override those methods you want to implement..</para>
    /// </summary>
    [SuppressMessage("ReSharper", "ConstantConditionalAccessQualifier")]
    public class PrioritizedConfigValues : IPrioritizedConfigValues
    {
        private readonly IPrioritizedConfigKeys[] PrioritizedConfigKeys;
        private readonly Func<string[], string> GetConfigValueFunc;
        private readonly string Delimiter;

        /// <summary>
        /// <para>Initialize the class by the array for <see cref="IPrioritizedConfigKeys"/>. The Array will be sorted by <see cref="IPrioritizedConfigKeys.Order"/> prop</para>
        /// <para>All <see cref="IPrioritizedConfigValues"/>-methods is implemented to get keys from <see cref="PrioritizedConfigKeys"/> array, and return first value different from null-or-empty</para>
        /// </summary>
        /// <param name="getConfigValueFunc"></param>
        /// <param name="prioritizedConfigKeys"></param>
        /// <param name="configurationDelimiter"></param>
        public PrioritizedConfigValues(
            Func<string[], string> getConfigValueFunc = null,
            IEnumerable<IPrioritizedConfigKeys> prioritizedConfigKeys = null,
            IConfigurationDelimiter configurationDelimiter = null)
        {
            PrioritizedConfigKeys = ToPrioritizedConfigKeysArray();
            GetConfigValueFunc = getConfigValueFunc;

            Delimiter = configurationDelimiter?.Delimiter ?? ":";

            IPrioritizedConfigKeys[] ToPrioritizedConfigKeysArray()
            {
                var array = prioritizedConfigKeys as IPrioritizedConfigKeys[] ?? prioritizedConfigKeys?.ToArray() ?? GetDefaultPrioritizedConfigKeys();
                return array.Any() == false ? GetDefaultPrioritizedConfigKeys() : array.Distinct().OrderBy(x => x.Order).ToArray();
            }
        }

        /// <inheritdoc />
        public virtual SupportedDatabaseTypes? GetDbType()
        {
            var value = GetConfigValue(PrioritizedConfigKeys?.Where(x => x?.GetDbTypeKeys()?.Any() == true)?
                .SelectMany(x => x?.GetDbTypeKeys())?.ToArray());
            if (value.IsEmpty())
            {
                return null;
            }

            return Enum.TryParse(value, true, out SupportedDatabaseTypes dbType)
                ? (SupportedDatabaseTypes?)dbType
                : null;
        }

        /// <inheritdoc />
        public virtual string GetDbSchema()
        {
            return GetConfigValue(PrioritizedConfigKeys?.Where(x => x?.GetDbSchemaKeys()?.Any() == true)?
                .SelectMany(x => x?.GetDbSchemaKeys())?.ToArray());
        }

        /// <inheritdoc />
        public virtual string GetDbSchemaPassword()
        {
            if (GetPasswordByUserName(GetDbSchema(), out var password))
            {
                return password;
            }
            
            return GetConfigValue(PrioritizedConfigKeys?.Where(x => x?.GetDbSchemaPasswordKeys()?.Any() == true)?
                .SelectMany(x => x?.GetDbSchemaPasswordKeys())?.ToArray());
        }

        /// <inheritdoc />
        public virtual string GetDbSchemaPrefixIdString()
        {
            return GetConfigValue(PrioritizedConfigKeys?.Where(x => x?.GetDbSchemaPrefixIdStringKeys()?.Any() == true)?
                .SelectMany(x => x?.GetDbSchemaPrefixIdStringKeys())?.ToArray());
        }

        /// <inheritdoc />
        public virtual string GetDbSchemaUniquePrefixIdString()
        {
            return GetConfigValue(PrioritizedConfigKeys?.Where(x => x?.GetDbSchemaUniquePrefixIdStringKeys()?.Any() == true)?
                .SelectMany(x => x?.GetDbSchemaUniquePrefixIdStringKeys())?.ToArray());
        }

        /// <inheritdoc />
        public virtual string GetDbDatabaseName()
        {
            return GetConfigValue(PrioritizedConfigKeys?.Where(x => x?.GetDbDatabaseNameKeys()?.Any() == true)?
                .SelectMany(x => x?.GetDbDatabaseNameKeys())?.ToArray());
        }

        /// <inheritdoc />
        public virtual string GetDbUser()
        {
            return GetConfigValue(PrioritizedConfigKeys?.Where(x => x?.GetDbUserKeys()?.Any() == true)?
                .SelectMany(x => x?.GetDbUserKeys())?.ToArray());
        }

        /// <inheritdoc />
        public virtual string GetDbPassword()
        {
            if (GetPasswordByUserName(GetDbUser(), out var password))
            {
                return password;
            }
            return GetConfigValue(PrioritizedConfigKeys?.Where(x => x?.GetDbPasswordKeys()?.Any() == true)?
                .SelectMany(x => x?.GetDbPasswordKeys())?.ToArray());
        }

        /// <inheritdoc />
        public virtual string GetDbAdminUser()
        {
            return GetConfigValue(PrioritizedConfigKeys?.Where(x => x?.GetDbAdminUserKeys()?.Any() == true)?
                .SelectMany(x => x?.GetDbAdminUserKeys())?.ToArray());
        }

        /// <inheritdoc />
        public virtual string GetDbAdminPassword()
        {
            if (GetPasswordByUserName(GetDbAdminUser(), out var password))
            {
                return password;
            }

            return GetConfigValue(PrioritizedConfigKeys?.Where(x => x?.GetDbAdminPasswordKeys()?.Any() == true)?
                .SelectMany(x => x?.GetDbAdminPasswordKeys())?.ToArray());
        }

        /// <inheritdoc />
        public virtual string GetDbHostname()
        {
            return GetConfigValue(PrioritizedConfigKeys?.Where(x => x?.GetDbHostnameKeys()?.Any() == true)?
                .SelectMany(x => x?.GetDbHostnameKeys())?.ToArray());
        }

        /// <inheritdoc />
        public virtual string GetDbPort()
        {
            return GetConfigValue(PrioritizedConfigKeys?.Where(x => x?.GetDbHostnameKeys()?.Any() == true)?
                .SelectMany(x => x?.GetDbHostnameKeys())?.ToArray());
        }

        /// <inheritdoc />
        public virtual string GetDbDataSource()
        {
            return GetConfigValue(PrioritizedConfigKeys?.Where(x => x?.GetDbDataSourceKeys()?.Any() == true)?
                .SelectMany(x => x?.GetDbDataSourceKeys())?.ToArray());
        }

        /// <inheritdoc />
        public virtual string GetDbConnectionTimeout()
        {
            return GetConfigValue(PrioritizedConfigKeys?.Where(x => x?.GetDbConnectionTimeoutKeys()?.Any() == true)?
                .SelectMany(x => x?.GetDbConnectionTimeoutKeys())?.ToArray());
        }

        /// <inheritdoc />
        public virtual bool? GetDbPooling()
        {
            var value = GetConfigValue(PrioritizedConfigKeys?.Where(x => x?.GetDbPoolingKeys()?.Any() == true)?
                .SelectMany(x => x?.GetDbPoolingKeys())?.ToArray());

            return value.IsNotEmpty() ? (bool?)value.IsTrue() : null;
        }

        /// <inheritdoc />
        public virtual IDictionary<string, string> GetDbPoolingKeyValues()
        {
            var value = GetConfigValue(PrioritizedConfigKeys?.Where(x => x?.GetDbPoolingKeyValuesKeys()?.Any() == true)?
                .SelectMany(x => x?.GetDbPoolingKeyValuesKeys())?.ToArray());

            return value.IsNotEmpty() ? value.ToDictionary() : null;
        }


        /// <inheritdoc />
        public virtual string GetDbConnectionString()
        {
            return GetConfigValue(PrioritizedConfigKeys?.Where(x => x?.GetDbConnectionStringKeys()?.Any() == true)?
                .SelectMany(x => x?.GetDbConnectionStringKeys())?.ToArray());
        }

        /// <inheritdoc />
        public virtual string GetDbAdminConnectionString()
        {
            return GetConfigValue(PrioritizedConfigKeys?.Where(x => x?.GetDbAdminConnectionStringKeys()?.Any() == true)?
                .SelectMany(x => x?.GetDbAdminConnectionStringKeys())?.ToArray());
        }

        /// <inheritdoc />
        public IPrioritizedConfigKeys[] GetPrioritizedConfigKeys()
        {
            return PrioritizedConfigKeys;
        }

        private string GetConfigValue(params string[] keys)
        {
            keys = keys?.Where(x => x != null).Distinct().ToArray();
            var value = keys == null || GetConfigValueFunc == null ? null : GetConfigValueFunc?.Invoke(keys);
            return string.IsNullOrEmpty(value) ? null : value;
        }

        private bool GetPasswordByUserName(string user, out string passwordByUser)
        {
            if (user.IsEmpty())
            {
                passwordByUser = null;
                return false;
            }

            passwordByUser = GetConfigValue(
                $"database{Delimiter}{user}{Delimiter}password",
                $"database{Delimiter}{user.Replace("_","")}{Delimiter}password");
            return passwordByUser.IsNotEmpty();
        }

        private IPrioritizedConfigKeys[] GetDefaultPrioritizedConfigKeys()
        {
            return new IPrioritizedConfigKeys[] { new PrioritizedConfigKeys() };
        }
    }
}