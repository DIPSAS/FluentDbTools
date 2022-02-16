using System.Collections.Generic;
using System.IO;
using Example.FluentDbTools.Config;
using Example.FluentDbTools.Migration;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Extensions.MSDependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Service.FluentDbTools.Migrator
{
    class Program
    {
        static void Main(string[] args)
        {
            var overrideConfig = new Dictionary<string, string>();
            //{
            //    { "Logging:Migration:ConsoleEnabled", "true" },
            //    { "Logging:Migration:File", "Program.sql" },
            //};

            foreach (var pair in new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().AsEnumerable())
            {
                if (overrideConfig.ContainsKey(pair.Key))
                {
                    overrideConfig[pair.Key] = pair.Value;
                }
                else
                {
                    overrideConfig.Add(pair.Key, pair.Value);
                }
            }

            var logFile = new ConfigurationBuilder().AddInMemoryCollection(overrideConfig).Build().GetMigrationLogFile();
            if (logFile.IsNotEmpty() && File.Exists(logFile))
            {
                File.Delete(logFile);
            }

            MigrationExecutor.MigrateUp(BaseConfig.DatabaseSelectionFromEnvironment(), overrideConfig, true);
        }
    }
}