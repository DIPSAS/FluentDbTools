using System.Collections.Generic;
using System.Linq;
using FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs;
using Microsoft.Extensions.Configuration;

namespace FluentDbTools.Extensions.Migration.DefaultConfigs
{
    public static class MsDbMigrationConfigExtension
    {
        private const string DefaultDbDefaultTablespace = "FLUENT_DATA";
        private const string DefaultDbTempTablespace = "FLUENT_TEMP";
        public static string GetMigrationSchemaPassword(this IConfiguration configuration)
        {
            return configuration?.GetMigrationSection()?.GetSectionStringValue("schemaPassword");
        }


        public static string GetMigrationDefaultTablespace(this IConfiguration configuration)
        {
            return (configuration?.GetMigrationSection()?.GetSectionStringValue("defaultTablespace") ??
                   configuration?.GetDbSection().GetSectionStringValue("defaultTablespace") ??
                   DefaultDbDefaultTablespace).ToUpper();
        }

        public static string GetDbTempTablespace(this IConfiguration configuration)
        {
            return (configuration?.GetMigrationSection()?.GetSectionStringValue("tempTablespace") ??
                   configuration?.GetDbSection().GetSectionStringValue("tempTablespace") ??
                    DefaultDbTempTablespace).ToUpper();
        }

        public static string GetMigrationDatabaseOwner(this IConfiguration configuration)
        {
            return configuration?.GetMigrationSection().GetSectionStringValue("databaseOwner");
        }

        public static IDictionary<string, string> GetDbAllMigrationConfigValues(this IConfiguration configuration)
        {
            return configuration.GetMigrationSection().GetDbAllConfigValues();
        }

        internal static IConfigurationSection GetMigrationSection(this IConfiguration configuration)
        {
            return configuration?.GetDbSection()?.GetSection("migration");
        }


    }
}