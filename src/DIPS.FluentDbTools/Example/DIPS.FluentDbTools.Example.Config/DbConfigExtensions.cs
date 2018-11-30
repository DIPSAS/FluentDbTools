using System;
using DIPS.FluentDbTools.Common.Abstractions;
using Microsoft.Extensions.Configuration;

namespace DIPS.FluentDbTools.Example.Config
{
    internal static class DbConfigExtensions
    {
        private const SupportedDatabaseTypes DefaultDatabaseType = SupportedDatabaseTypes.Postgres;
        private const string DefaultDbUser = "user";
        private const string DefaultDbPassword = "password";
        private const string DefaultDbAdminUser = "admin";
        private const string DefaultDbAdminPassword = "admin";
        private const string DefaultDbHostname = "localhost";
        private const string DefaultDbPort = "5432";
        private const string DefaultDbConnectionName = "postgres";
        private const bool DefaultDbPooling = false;
        private const string DefaultDbSchema = DefaultDbUser;
        private const string DefaultDbDefaultTablespace = "FLUENT_DATA";
        private const string DefaultDbTempTablespace = "FLUENT_TEMP";

        private static IConfigurationSection GetDbSection(this IConfiguration configuration)
        {
            return configuration.GetSection("database");
        }
        
        public static SupportedDatabaseTypes GetDbType(this IConfiguration configuration)
        {
            var section = configuration.GetDbSection();
            if (!Enum.TryParse(section["type"], true, out SupportedDatabaseTypes availableDatabaseType))
            {
                availableDatabaseType = DefaultDatabaseType;
            }
            return availableDatabaseType;
        }
        
        public static string GetDbUser(this IConfiguration configuration)
        {
            var section = configuration.GetDbSection();
            return section["user"] ?? DefaultDbUser;
        }
        
        public static string GetDbPassword(this IConfiguration configuration)
        {
            var section = configuration.GetDbSection();
            return section["password"] ?? DefaultDbPassword;
        }
        
        public static string GetDbAdminUser(this IConfiguration configuration)
        {
            var section = configuration.GetDbSection();
            return section["adminUser"] ?? DefaultDbAdminUser;
        }
        
        public static string GetDbAdminPassword(this IConfiguration configuration)
        {
            var section = configuration.GetDbSection();
            return section["adminPassword"] ?? DefaultDbAdminPassword;
        }
        
        public static string GetDbHostname(this IConfiguration configuration)
        {
            var section = configuration.GetDbSection();
            return section["hostname"] ?? DefaultDbHostname;
        }
        
        public static string GetDbPort(this IConfiguration configuration)
        {
            var section = configuration.GetDbSection();
            return section["port"] ?? DefaultDbPort;
        }
        
        public static string GetDbConnectionName(this IConfiguration configuration)
        {
            var section = configuration.GetDbSection();
            return section["databaseConnectionName"] ?? DefaultDbConnectionName;
        }
        
        public static bool GetDbPooling(this IConfiguration configuration)
        {
            var section = configuration.GetDbSection();
            if (!bool.TryParse(section["pooling"], out var pooling))
            {
                pooling = DefaultDbPooling;
            }
            return pooling;
        }
        
        public static string GetDbSchema(this IConfiguration configuration)
        {
            var section = configuration.GetDbSection();
            return (section["schema"] ?? DefaultDbSchema).ToLower();
        }
        
        public static string GetDbDefaultTablespace(this IConfiguration configuration)
        {
            var section = configuration.GetDbSection();
            return (section["defaultTablespace"] ?? DefaultDbDefaultTablespace).ToUpper();
        }
        
        public static string GetDbTempTablespace(this IConfiguration configuration)
        {
            var section = configuration.GetDbSection();
            return (section["tempTablespace"] ?? DefaultDbTempTablespace).ToUpper();
        }
    }
}