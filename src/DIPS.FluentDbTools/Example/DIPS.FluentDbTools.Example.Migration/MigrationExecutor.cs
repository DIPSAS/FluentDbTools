using System.Collections.Generic;
using System.Reflection;
using DIPS.FluentDbTools.Example.Migration.MigrationModels;
using DIPS.FluentDbTools.Migration.Abstractions;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace DIPS.FluentDbTools.Example.Migration
{
    public static class MigrationExecutor
    {
        public static void ExecuteMigration(
            Dictionary<string, string> overrideConfig = null, 
            string additionalJsonConfig = null)
        {
            var provider = MigrationBuilder.BuildMigration(
                overrideConfig, 
                additionalJsonConfig);
            
            using (var scope = provider.CreateScope())
            {
                var migrationRunner = scope.ServiceProvider.GetService<IMigrationRunner>();
                
                migrationRunner.MigrateUp();
            }
            
        }
    }
}