using System;
using System.Collections.Generic;
using FluentDbTools.Common.Abstractions;

namespace FluentDbTools.Example.Config
{
    public static class BaseConfig
    {
        public static string ConfigFilename(SupportedDatabaseTypes databaseType)
        {
            switch (databaseType)
            {
                case SupportedDatabaseTypes.Postgres:
                    return PostgresConfigFilename;
                case SupportedDatabaseTypes.Oracle:
                    return OracleConfigFilename;
                default:
                    throw new ArgumentOutOfRangeException(nameof(databaseType), databaseType, null);
            }
        }
        
        public static string ConfigDockerFilename(SupportedDatabaseTypes dbType)
        {
            switch (dbType)
            {
                case SupportedDatabaseTypes.Postgres:
                    return PostgresConfigDockerFilename;
                case SupportedDatabaseTypes.Oracle:
                    return OracleConfigDockerFilename;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dbType), dbType, null);
            }
        }
        
        private const string PostgresConfigFilename = "config.postgres.json";
        private const string PostgresConfigDockerFilename = "config.postgres.docker.json";
        
        private const string OracleConfigFilename = "config.oracle.json";
        private const string OracleConfigDockerFilename = "config.oracle.docker.json";
        
        public static bool InContainer => 
            Environment.GetEnvironmentVariable("RUNNING_IN_CONTAINER") == "true";

        public static bool UseExternalServiceHost => 
            !string.IsNullOrEmpty(ExternalServiceHost);
        
        public static string ExternalServiceHost =>
            Environment.GetEnvironmentVariable("EXTERNAL_SERVICE_HOST");

        public static Dictionary<string, string> ExternalServiceHostConfiguration =>
            new Dictionary<string, string>
            {
                {"database:hostname", ExternalServiceHost}
            };
    }
}