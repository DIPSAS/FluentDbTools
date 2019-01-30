using FluentDbTools.Example.Config;
using FluentDbTools.Example.Migration;

namespace Service.FluentDbTools.Migrator
{
    class Program
    {
        static void Main(string[] args)
        {
            MigrationExecutor.ExecuteMigration(BaseConfig.DatabaseSelectionFromEnvironment());
        }
    }
}