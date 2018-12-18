using System.Collections.Generic;
using System.Reflection;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Example.Migration.MigrationModels;
using FluentDbTools.Migration.Abstractions;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace FluentDbTools.Example.Migration
{
    public static class MigrationExecutor
    {
        public static void ExecuteMigration(
            SupportedDatabaseTypes databaseType,
            Dictionary<string, string> overrideConfig = null)
        {
            var provider = MigrationBuilder.BuildMigration(
                databaseType,
                overrideConfig);
            
            using (var scope = provider.CreateScope())
            {
                var migrationRunner = scope.ServiceProvider.GetService<IMigrationRunner>();
                
                migrationRunner.MigrateUp();
            }
            
        }
    }
}