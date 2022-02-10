using System;
using System.Collections.Generic;
using FluentDbTools.Common.Abstractions;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace Example.FluentDbTools.Migration
{
    public static class MigrationExecutor
    {
        public static void MigrateUp(
            SupportedDatabaseTypes databaseType,
            Dictionary<string, string> overrideConfig = null)
        {
            var provider = MigrationBuilder.BuildMigration(
                databaseType,
                overrideConfig);
            try
            {
                using (var scope = provider.CreateScope())
                {
                    var migrationRunner = scope.ServiceProvider.GetService<IMigrationRunner>();
                    migrationRunner.MigrateUp();
                }
            }
            catch (InvalidOperationException)
            {
                //
            }
        }
    }
}