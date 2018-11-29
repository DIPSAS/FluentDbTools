using System;
using System.Collections.Generic;

namespace DIPS.FluentDbTools.Example.Config
{
    public static class BaseConfig
    {
        public const string ConfigFilename = "Config.json";
        public const string ConfigDockerFilename = "Config.Docker.json";
        
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