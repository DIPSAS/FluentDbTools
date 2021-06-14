using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Common.Abstractions.PrioritizedConfig;
using FluentDbTools.Contracts;
using FluentDbTools.Extensions.DbProvider;
using Microsoft.Extensions.Configuration;

[assembly: InternalsVisibleTo("FluentDbTools.Extensions.Migration")]
namespace FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs
{
    public static class MsDbConfigExtensions
    {
        private const SupportedDatabaseTypes DefaultDatabaseType = SupportedDatabaseTypes.Postgres;
        private const string DefaultDbUser = "user";
        private const string DefaultDbPassword = "password";
        private const bool DefaultDbPooling = true;


        internal static SupportedDatabaseTypes GetDbType(this IConfiguration configuration)
        {
            var section = configuration?.GetDbSection();
            if (!Enum.TryParse(section?["type"], true, out SupportedDatabaseTypes availableDatabaseType))
            {
                availableDatabaseType = DefaultDatabaseType;
            }
            return availableDatabaseType;
        }

        internal static string GetDbUser(this IConfiguration configuration)
        {
            var section = configuration?.GetDbSection();
            return section.GetSectionStringValue("user", DefaultDbUser);
        }

        internal static string GetDbPassword(this IConfiguration configuration)
        {
            var section = configuration?.GetDbSection();
            return section.GetSectionStringValue("password", configuration.GetSecret(GetDbUser(configuration)).WithDefault(DefaultDbPassword));
        }

        public static string GetSecret(this IConfiguration configuration, string user, string section = "database:secret")
        {
            var secret = configuration.GetSection($"{section}:encrypted").GetConfigValue(user);
            if (secret.IsNotEmpty())
            {
                var sectionConfig = configuration.GetSection($"{section}:encrypted");
                return Encoding.UTF8.GetString(new SymmetricCryptoProvider(key => sectionConfig?.GetConfigValue(key)).Decrypt(Convert.FromBase64String(secret)));
            }

            secret = configuration.GetSection($"{section}:encoded").GetConfigValue(user);
            if (secret.IsNotEmpty())
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(secret));
            }

            return null;
        }

        internal static string GetDbAdminUser(this IConfiguration configuration)
        {
            string defaultDbAdminUser;
            switch (configuration.GetDbType())
            {
                case SupportedDatabaseTypes.Postgres:
                    defaultDbAdminUser = "postgres";
                    break;
                case SupportedDatabaseTypes.Oracle:
                    defaultDbAdminUser = "system";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var section = configuration?.GetDbSection();
            return section.GetSectionStringValue("adminUser", defaultDbAdminUser);
        }

        internal static string GetDbAdminPassword(this IConfiguration configuration)
        {
            string defaultDbAdminPassword;
            switch (configuration.GetDbType())
            {
                case SupportedDatabaseTypes.Postgres:
                    defaultDbAdminPassword = "postgres";
                    break;
                case SupportedDatabaseTypes.Oracle:
                    defaultDbAdminPassword = "oracle";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var section = configuration?.GetDbSection();
            return section.GetSectionStringValue("adminPassword", configuration.GetSecret(GetDbAdminUser(configuration)).WithDefault(defaultDbAdminPassword));
        }

        internal static string GetDbHostname(this IConfiguration configuration)
        {
            var section = configuration?.GetDbSection();
            return section.GetSectionStringValue("hostname");
        }

        internal static string GetDbPort(this IConfiguration configuration)
        {
            string defaultDbPort;
            switch (configuration.GetDbType())
            {
                case SupportedDatabaseTypes.Postgres:
                    defaultDbPort = "5432";
                    break;
                case SupportedDatabaseTypes.Oracle:
                    defaultDbPort = "1521";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var section = configuration?.GetDbSection();
            return section.GetSectionStringValue("port", defaultDbPort);
        }

        internal static string GetDbDatabaseName(this IConfiguration configuration)
        {
            string defaultConnectionName;
            switch (configuration.GetDbType())
            {
                case SupportedDatabaseTypes.Postgres:
                    defaultConnectionName = configuration.GetDbSchema();
                    break;
                case SupportedDatabaseTypes.Oracle:
                    defaultConnectionName = "xe";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var section = configuration?.GetDbSection();
            return section.GetSectionStringValue("databaseName")
                ?? section.GetSectionStringValue("databaseConnectionName")
                ?? section.GetSectionStringValue("servicename")
                ?? defaultConnectionName;
        }

        internal static bool GetDbPooling(this IConfiguration configuration)
        {
            var section = configuration?.GetDbSection();
            if (!bool.TryParse(section?["pooling"], out var pooling))
            {
                pooling = DefaultDbPooling;
            }
            return pooling;
        }

        internal static IDictionary<string, string> GetDbPoolingKeyValues(this IConfiguration configuration)
        {
            var section = configuration?.GetDbSection();
            var value = section.GetSectionStringValue("poolingKeyValues");
            return string.IsNullOrEmpty(value) ? null : value.ToDictionary();
        }

        internal static string GetDbSchema(this IConfiguration configuration)
        {
            var section = configuration?.GetDbSection();

            var schema = section.GetSectionStringValue("schema", configuration.GetDbUser());

            schema = configuration?.GetDbType() == SupportedDatabaseTypes.Oracle
                ? schema.ToUpper()
                : schema.ToLower();
            return schema;
        }

        internal static string GetDbConnectionString(this IConfiguration configuration)
        {
            var section = configuration?.GetDbSection();
            return section.GetSectionStringValue("connectionString");
        }

        internal static string GetDbAdminConnectionString(this IConfiguration configuration)
        {
            var section = configuration?.GetDbSection();
            return section.GetSectionStringValue("adminConnectionString");
        }


        internal static string GetDbConnectionTimeout(this IConfiguration configuration)
        {
            var section = configuration?.GetDbSection();
            return section.GetSectionStringValue("connectionTimeoutInSecs");
        }

        internal static string GetDbDataSource(this IConfiguration configuration)
        {
            var section = configuration?.GetDbSection();
            return section.GetSectionStringValue("dataSource");
        }

        internal static IConfigurationSection GetDbSection(this IConfiguration configuration)
        {
            return configuration?.GetSection("database");
        }


        internal static string GetSectionStringValue(this IConfigurationSection section, string key, string defaultValue = null)
        {
            if (section == null)
            {
                return defaultValue;
            }

            var value = section[key];
            if (string.IsNullOrEmpty(value))
            {
                value = defaultValue;
            }

            return value;
        }

        internal static IDictionary<string, string> GetDbAllConfigValues(
            this IConfigurationSection sections)
        {
            var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (sections == null)
            {
                return dictionary;
            }

            foreach (var section in sections.GetChildren())
            {
                if (section.Value == null && sections.GetSection(section.Key).Exists())
                {
                    var values = GetDbAllConfigValues(sections.GetSection(section.Key));
                    foreach (var value in values)
                    {
                        var key = $"{section.Key}:{value.Key}";
                        if (!dictionary.ContainsKey(key))
                        {
                            dictionary.Add(key, value.Value);
                        }

                    }
                    continue;
                }
                if (!dictionary.ContainsKey(section.Key))
                {
                    dictionary.Add(section.Key, section.Value);
                }
            }

            return dictionary;
        }

        /// <summary>
        /// Resolve underlying <see cref="IConfiguration"/> to the implemented class of <see cref="IDbConfig"/> 
        /// </summary>
        /// <param name="dbConfig"></param>
        /// <returns></returns>
        public static IConfiguration GetConfiguration(this IDbConfig dbConfig)
        {
            if (dbConfig != null && dbConfig is MsDbConfig msDbConfig)
            {
                return msDbConfig.Configuration;
            }

            return null;
        }

        /// <summary>
        /// Create the DependencyInjection implementing class of <see cref="IDbConfig"/> (strong type Ms<see cref="DbConfig"/>) 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="defaultDbConfigValues"></param>
        /// <param name="dbConfigCredentials"></param>
        /// <param name="prioritizedConfigValues"></param>
        /// <returns></returns>
        public static DbConfig CreateDbConfig(this IConfiguration configuration,
            DefaultDbConfigValues defaultDbConfigValues = null,
            DbConfigCredentials dbConfigCredentials = null,
            IPrioritizedConfigValues prioritizedConfigValues = null)
        {
            return new MsDbConfig(configuration, null, defaultDbConfigValues, dbConfigCredentials, prioritizedConfigValues);
        }
    }
}