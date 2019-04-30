using System;
using System.Collections.Generic;
using System.Linq;
using FluentDbTools.Common.Abstractions;
using Example.FluentDbTools.Database;
using Example.FluentDbTools.Migration;

namespace TestUtilities.FluentDbTools
{
    public static class TestServiceProvider
    {
        public static IServiceProvider GetDatabaseExampleServiceProvider(
            SupportedDatabaseTypes databaseType = SupportedDatabaseTypes.Postgres,
            Dictionary<string, string> additionalOverrideConfig = null)
        {
            var overrideConfig = OverrideConfig.GetInMemoryOverrideConfig(databaseType);
            additionalOverrideConfig?.ToList().ForEach(x => overrideConfig[x.Key] = x.Value);
            return DbExampleBuilder.BuildDbExample(databaseType, overrideConfig);
        }
        
        public static IServiceProvider GetMigrationExampleServiceProvider(
            SupportedDatabaseTypes databaseType = SupportedDatabaseTypes.Postgres,
            Dictionary<string, string> additionalOverrideConfig = null)
        {
            var overrideConfig = OverrideConfig.GetInMemoryOverrideConfig(databaseType);
            additionalOverrideConfig?.ToList().ForEach(x => overrideConfig[x.Key] = x.Value);
            return MigrationBuilder.BuildMigration(databaseType, overrideConfig);
        }
    }
}