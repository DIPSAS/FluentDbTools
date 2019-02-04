using Example.FluentDbTools.Config;
using Example.FluentDbTools.Migration;
using FluentDbTools.Example.Migration;

namespace Service.FluentDbTools.Migrator
{
    class Program
    {
        static void Main(string[] args)
        {
            MigrationExecutor.MigrateUp(BaseConfig.DatabaseSelectionFromEnvironment());
        }
    }
}