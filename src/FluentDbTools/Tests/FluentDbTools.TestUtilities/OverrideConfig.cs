using System;
using System.Collections.Generic;
using FluentDbTools.Extensions.MSDependencyInjection;
using FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Example.Config;
using Microsoft.Extensions.Configuration;

namespace FluentDbTools.TestUtilities
{
    public static class OverrideConfig
    {
        const string ConfigFolder = "TestConfigs";
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

            if (databaseType != SupportedDatabaseTypes.Oracle)
            {
                overrideDict["database:databaseConnectionName"] = schema;
            }
            
            return overrideDict;
        }

        public static string GetJsonOverrideConfig(SupportedDatabaseTypes databaseType)
        {
            switch (databaseType)
            {
                case SupportedDatabaseTypes.Postgres:
                    return $"{ConfigFolder}/postgres.override.json";
                case SupportedDatabaseTypes.Oracle:
                    return $"{ConfigFolder}/oracle.override.json";
                default:
                    throw new ArgumentOutOfRangeException(nameof(databaseType), databaseType, null);
            }
        }

        public static IConfigurationRoot CreateTestConfiguration(SupportedDatabaseTypes databaseType, string schema = null)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile(GetJsonOverrideConfig(databaseType))
                .AddJsonFileIfTrue($"{ConfigFolder}/oracle.override.docker.json", () => BaseConfig.InContainer && !BaseConfig.UseExternalServiceHost && databaseType == SupportedDatabaseTypes.Oracle)
                .AddInMemoryCollection(GetInMemoryOverrideConfig(databaseType, schema))
                .Build();
            return config;
        }

        public static IDbConfig CreateTestDbConfig(SupportedDatabaseTypes databaseType, string schema = null)
        {
            return new DefaultDbConfig(CreateTestConfiguration(databaseType, schema));
        }
    }
}