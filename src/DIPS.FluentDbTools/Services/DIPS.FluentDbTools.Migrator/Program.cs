using System;
using DIPS.FluentDbTools.Example.Migration;

namespace DIPS.FluentDbTools.Migrator
{
    class Program
    {
        static void Main(string[] args)
        {
            MigrationExecutor.ExecuteMigration();
        }
    }
}