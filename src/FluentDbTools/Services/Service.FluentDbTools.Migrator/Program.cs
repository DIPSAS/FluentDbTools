using Example.FluentDbTools.Config;
using Example.FluentDbTools.Migration;

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