using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Common.Abstractions.PrioritizedConfig;
using FluentDbTools.Contracts;
using FluentDbTools.Extensions.DbProvider;
using Microsoft.Extensions.Configuration;
#pragma warning disable CS1591

[assembly: InternalsVisibleTo("FluentDbTools.Extensions.Migration")]
namespace FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs
{
    public static class MsDbConfigExtensions
    {
        private const string DefaultDbUser = "user";
        private const string DefaultDbPassword = "password";
        private const bool DefaultDbPooling = true;

        public static SupportedDatabaseTypes GetDbType(this IConfiguration configuration)
        {
            var section = configuration?.GetDbSection();
            if (!Enum.TryParse(section?["type"], true, out SupportedDatabaseTypes availableDatabaseType))
            {
                availableDatabaseType = DefaultDbConfigValues.DefaultDbType;
            }
            return availableDatabaseType;
        }

        public static string GetDbUser(this IConfiguration configuration)
        {
            var section = configuration?.GetDbSection();
            return section.GetSectionStringValue("user", DefaultDbUser);
        }

        public static string GetDbPassword(this IConfiguration configuration)
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

        public static string GetDbAdminUser(this IConfiguration configuration)
        {
            string defaultDbAdminUser;
            switch (configuration.GetDbType())
            {
                case SupportedDatabaseTypes.Postgres:
                    defaultDbAdminUser = DefaultDbConfigValues.DefaultPostgresAdminUserAndPassword.Item1;
                    break;
                case SupportedDatabaseTypes.Oracle:
                    defaultDbAdminUser = DefaultDbConfigValues.DefaultOracleAdminUserAndPassword.Item1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var section = configuration?.GetDbSection();
            return section.GetSectionStringValue("adminUser", defaultDbAdminUser);
        }

        public static string GetDbAdminPassword(this IConfiguration configuration)
        {
            string defaultDbAdminPassword;
            switch (configuration.GetDbType())
            {
                case SupportedDatabaseTypes.Postgres:
                    defaultDbAdminPassword = DefaultDbConfigValues.DefaultPostgresAdminUserAndPassword.Item2;
                    break;
                case SupportedDatabaseTypes.Oracle:
                    defaultDbAdminPassword = DefaultDbConfigValues.DefaultOracleAdminUserAndPassword.Item2;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var section = configuration?.GetDbSection();
            return section.GetSectionStringValue("adminPassword", configuration.GetSecret(GetDbAdminUser(configuration)).WithDefault(defaultDbAdminPassword));
        }

        public static string GetDbHostname(this IConfiguration configuration)
        {
            var section = configuration?.GetDbSection();
            return section.GetSectionStringValue("hostname");
        }

        public static string GetDbPort(this IConfiguration configuration)
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

        public static string GetDbDatabaseName(this IConfiguration configuration)
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

        public static bool GetDbPooling(this IConfiguration configuration)
        {
            var section = configuration?.GetDbSection();
            if (!bool.TryParse(section?["pooling"], out var pooling))
            {
                pooling = DefaultDbPooling;
            }
            return pooling;
        }

        public static IDictionary<string, string> GetDbPoolingKeyValues(this IConfiguration configuration)
        {
            var section = configuration?.GetDbSection();
            var value = section.GetSectionStringValue("poolingKeyValues");
            return string.IsNullOrEmpty(value) ? null : value.ToDictionary();
        }

        public static string GetDbSchema(this IConfiguration configuration)
        {
            var section = configuration?.GetDbSection();

            var schema = section.GetSectionStringValue("schema", configuration.GetDbUser());

            schema = configuration?.GetDbType() == SupportedDatabaseTypes.Oracle
                ? schema.ToUpper()
                : schema.ToLower();
            return schema;
        }

        public static string GetDbConnectionString(this IConfiguration configuration)
        {
            var section = configuration?.GetDbSection();
            return section.GetSectionStringValue("connectionString");
        }

        public static string GetDbAdminConnectionString(this IConfiguration configuration)
        {
            var section = configuration?.GetDbSection();
            return section.GetSectionStringValue("adminConnectionString");
        }


        public static string GetDbConnectionTimeout(this IConfiguration configuration)
        {
            var section = configuration?.GetDbSection();
            return section.GetSectionStringValue("connectionTimeoutInSecs");
        }

        public static string GetDbDataSource(this IConfiguration configuration)
        {
            var section = configuration?.GetDbSection();
            return section.GetSectionStringValue("dataSource");
        }

        public static IConfigurationSection GetDbSection(this IConfiguration configuration)
        {
            return configuration?.GetSection("database");
        }


        public static string GetSectionStringValue(this IConfigurationSection section, string key, string defaultValue = null)
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

        public static IDictionary<string, string> GetDbAllConfigValues(
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