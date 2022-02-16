using System;
using System.Collections.Generic;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Extensions.Migration;
using FluentDbTools.Migration;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace Example.FluentDbTools.Migration
{
    public static class MigrationExecutor
    {
        public static void MigrateUp(
            SupportedDatabaseTypes databaseType,
            Dictionary<string, string> overrideConfig = null,
            bool drop = false)
        {
            var provider = MigrationBuilder.BuildMigration(
                databaseType,
                overrideConfig);
            try
            {
                using (var scope = provider.CreateScope())
                {
                    var migrationRunner = scope.ServiceProvider.GetService<IMigrationRunner>();
                    if (drop)
                    {
                        migrationRunner.DropSchema(scope.ServiceProvider.GetVersionTableMetaData());
                    }

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