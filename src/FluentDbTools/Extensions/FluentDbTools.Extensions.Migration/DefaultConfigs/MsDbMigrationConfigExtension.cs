using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs;
using FluentDbTools.Migration.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FluentDbTools.Extensions.Migration.DefaultConfigs
{
    public static class MsDbMigrationConfigExtension
    {
        private const string DefaultDbDefaultTablespace = null;
        private const string DefaultDbTempTablespace = null;
        public static string GetMigrationSchemaPassword(this IConfiguration configuration)
        {
            return configuration?.GetMigrationSection()?.GetSectionStringValue("schemaPassword");
        }


        public static string GetMigrationDefaultTablespace(this IConfiguration configuration)
        {
            return (configuration?.GetMigrationSection()?.GetSectionStringValue("defaultTablespace") ??
                   configuration?.GetDbSection().GetSectionStringValue("defaultTablespace") ??
                   DefaultDbDefaultTablespace)?.ToUpper();
        }

        public static string GetDbTempTablespace(this IConfiguration configuration)
        {
            return (configuration?.GetMigrationSection()?.GetSectionStringValue("tempTablespace") ??
                   configuration?.GetDbSection().GetSectionStringValue("tempTablespace") ??
                    DefaultDbTempTablespace)?.ToUpper();
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

        /// <summary>
        /// Resolve underlying <see cref="IConfiguration"/> to the implemented class of <see cref="IDbMigrationConfig"/> 
        /// </summary>
        /// <param name="dbMigrationConfig"></param>
        /// <returns></returns>
        public static IConfiguration GetConfiguration(this IDbMigrationConfig dbMigrationConfig)
        {
            if (dbMigrationConfig != null && dbMigrationConfig is MsDbMigrationConfig msDbMigrationConfig)
            {
                return msDbMigrationConfig.Configuration;
            }
            return dbMigrationConfig?.GetDbConfig()?.GetConfiguration();
        }
    }
}