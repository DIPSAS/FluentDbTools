using System;
using System.Collections.Generic;
using FluentDbTools.Extensions.MSDependencyInjection;
using FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs;
using FluentDbTools.Common.Abstractions;
using Example.FluentDbTools.Common;
using Example.FluentDbTools.Config;
using Microsoft.Extensions.Configuration;

namespace TestUtilities.FluentDbTools
{
    public static class OverrideConfig
    {
        private static readonly Random Random = new Random();

        public static string NewRandomSchema => $"DbToolsTest_{Random.Next().ToString()}";
        
        public static Dictionary<string, string> GetInMemoryOverrideConfig(
            SupportedDatabaseTypes databaseType = SupportedDatabaseTypes.Postgres, 
            string schema = null)
        {
            schema = schema ?? DatabaseFixture.MigratedDatabaseSchema;
            var overrideDict = new Dictionary<string, string>
            {
                {"database:schema", schema},
                {"database:user", schema},
            };

            switch (databaseType)
            {
                case SupportedDatabaseTypes.Postgres:
                    overrideDict["database:databaseConnectionName"] = schema;
                    break;
                case SupportedDatabaseTypes.Oracle:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(databaseType), databaseType, null);
            }
            
            return overrideDict;
        }

        public static IDbConfig CreateTestDbConfig(SupportedDatabaseTypes databaseType, string schema = null)
        {
            return new DefaultDbConfig(CreateTestConfiguration(databaseType, schema));
        }

        public static IConfigurationRoot CreateTestConfiguration(SupportedDatabaseTypes databaseType, string schema = null)
        {
            var config = new ConfigurationBuilder()
                .AddDbToolsExampleConfiguration(databaseType)
                .AddInMemoryCollection(GetInMemoryOverrideConfig(databaseType, schema))
                .Build();
            return config;
        }
    }
}