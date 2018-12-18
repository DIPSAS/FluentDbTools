using System;
using System.Collections.Generic;
using System.Linq;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Example.Database;
using FluentDbTools.Example.Migration;

namespace FluentDbTools.TestUtilities
{
    public static class TestServiceProvider
    {
        public static IServiceProvider GetDatabaseExampleServiceProvider(
            SupportedDatabaseTypes databaseType = SupportedDatabaseTypes.Postgres,
            Dictionary<string, string> additionalOverrideConfig = null)
        {
            var overrideConfig = OverrideConfig.GetInMemoryOverrideConfig(databaseType);
            additionalOverrideConfig?.ToList().ForEach(x => overrideConfig[x.Key] = x.Value);
            var jsonConfig = OverrideConfig.GetJsonOverrideConfig(databaseType);
            return DbExampleBuilder.BuildDbExample(overrideConfig, jsonConfig);
        }
        
        public static IServiceProvider GetMigrationExampleServiceProvider(
            SupportedDatabaseTypes databaseType = SupportedDatabaseTypes.Postgres,
            Dictionary<string, string> additionalOverrideConfig = null)
        {
            var overrideConfig = OverrideConfig.GetInMemoryOverrideConfig(databaseType);
            additionalOverrideConfig?.ToList().ForEach(x => overrideConfig[x.Key] = x.Value);
            var jsonConfig = OverrideConfig.GetJsonOverrideConfig(databaseType);
            return MigrationBuilder.BuildMigration(overrideConfig, jsonConfig);
        }
    }
}