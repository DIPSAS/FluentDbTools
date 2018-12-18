using System;
using System.Collections.Generic;

namespace FluentDbTools.Example.Config
{
    public static class BaseConfig
    {
        public const string DefaultConfigFilename = PostgresConfigFilename;
        public const string DefaultConfigDockerFilename = PostgresConfigDockerFilename;
        
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