using System;
using System.Collections.Generic;
using System.Reflection;
using Example.FluentDbTools.Migration;
using FluentDbTools.Common.Abstractions;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace FluentDbTools.Example.Migration
{
    public static class MigrationExecutor
    {

        [Obsolete("Please use MigrateUp")]
        public static void ExecuteMigration(
            SupportedDatabaseTypes databaseType,
            Dictionary<string, string> overrideConfig = null)
        {
            MigrateUp(databaseType, overrideConfig);
        }

        public static void MigrateUp(
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