using System.Collections.Generic;
using System.IO;
using Example.FluentDbTools.Config;
using Example.FluentDbTools.Migration;

namespace Service.FluentDbTools.Migrator
{
    class Program
    {
        static void Main(string[] args)
        {
            var overrideConfig = new Dictionary<string, string>
            {
                { "Logging:Migration:ConsoleEnabled", "true" },
                { "Logging:Migration:File", "Program.sql" },
            };

            if (File.Exists("Program.sql"))
            {
                File.Delete("Program.sql");
            }

            MigrationExecutor.MigrateUp(BaseConfig.DatabaseSelectionFromEnvironment(), overrideConfig, true);
        }
    }
}