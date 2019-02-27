using System;
using System.Security.Cryptography.X509Certificates;
using FluentDbTools.Common.Abstractions;
using Microsoft.Extensions.Configuration;

namespace FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs
{
    internal static class DefaultDbConfigExtensions
    {
        private const SupportedDatabaseTypes DefaultDatabaseType = SupportedDatabaseTypes.Postgres;
        private const string DefaultDbUser = "user";
        private const string DefaultDbPassword = "password";
        private const string DefaultDbHostname = "localhost";
        private const bool DefaultDbPooling = true;
        private const string DefaultDbDefaultTablespace = "FLUENT_DATA";
        private const string DefaultDbTempTablespace = "FLUENT_TEMP";

        private static IConfigurationSection GetDbSection(this IConfiguration configuration)
        {
            return configuration?.GetSection("database");
        }

        public static SupportedDatabaseTypes GetDbType(this IConfiguration configuration)
        {
            var section = configuration?.GetDbSection();
            if (!Enum.TryParse(section?["type"], true, out SupportedDatabaseTypes availableDatabaseType))
            {
                availableDatabaseType = DefaultDatabaseType;
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
            return section.GetSectionStringValue("password", DefaultDbPassword);
        }

        public static string GetDbAdminUser(this IConfiguration configuration)
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

        public static string GetDbAdminPassword(this IConfiguration configuration)
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
            return section.GetSectionStringValue("adminPassword", defaultDbAdminPassword);
        }

        public static string GetDbHostname(this IConfiguration configuration)
        {
            var section = configuration?.GetDbSection();
            return section.GetSectionStringValue("hostname", DefaultDbHostname);
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

        public static string GetDbConnectionName(this IConfiguration configuration)
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
            return section.GetSectionStringValue("databaseConnectionName", defaultConnectionName);
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

        public static string GetDbSchema(this IConfiguration configuration)
        {
            var section = configuration?.GetDbSection();
            return section.GetSectionStringValue("schema", configuration.GetDbUser()).ToLower();
        }

        public static string GetDbSchemaPassword(this IConfiguration configuration)
        {
            var section = configuration?.GetDbSection();
            return section.GetSectionStringValue("schemaPassword", configuration.GetDbPassword());
        }


        public static string GetDbDefaultTablespace(this IConfiguration configuration)
        {
            var section = configuration?.GetDbSection();
            return section.GetSectionStringValue("defaultTablespace", DefaultDbDefaultTablespace).ToUpper();
        }

        public static string GetDbTempTablespace(this IConfiguration configuration)
        {
            var section = configuration?.GetDbSection();
            return section.GetSectionStringValue("tempTablespace", DefaultDbTempTablespace).ToUpper();
        }
        
        public static string GetDbConnectionStringTemplate(this IConfiguration configuration)
        {
            var section = configuration?.GetDbSection();
            return section.GetSectionStringValue("connectionStringTemplate");
        }

        private static string GetSectionStringValue(this IConfigurationSection section, string key, string defaultValue = null)
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
    }
}