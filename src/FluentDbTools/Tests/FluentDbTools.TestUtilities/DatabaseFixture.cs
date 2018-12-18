using System;
using System.Collections.Generic;
using System.Linq;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Example.Migration;
using FluentDbTools.Migration;
using FluentMigrator.Runner;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.DependencyInjection;

namespace FluentDbTools.TestUtilities
{  
    public class DatabaseFixture : IDisposable
    {
        public static string MigratedDatabaseSchema = "DefaultTestSchema";
        
        public DatabaseFixture()
        {
            MigratedDatabaseSchema = OverrideConfig.NewRandomSchema;
            foreach (var databaseType in SelectedDatabaseTypesToTest())
            {
                var overrideConfig = OverrideConfig.GetInMemoryOverrideConfig(databaseType, MigratedDatabaseSchema);
                var jsonConfig = OverrideConfig.GetJsonOverrideConfig(databaseType);
                MigrationExecutor.ExecuteMigration(overrideConfig, jsonConfig);
            }
        }
        
        public void Dispose()
        {
            foreach (var databaseType in SelectedDatabaseTypesToTest())
            {
                SafeDeleteTestData(databaseType);
            }
        }

        private static void SafeDeleteTestData(SupportedDatabaseTypes databaseType)
        {
            try
            {
                var provider = TestServiceProvider.GetMigrationExampleServiceProvider(databaseType);
                using (var scope = provider.CreateScope())
                {
                    DeleteTestData(scope);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        private static void DeleteTestData(IServiceScope scope)
        {
            var versionTable = scope.ServiceProvider.GetService<IVersionTableMetaData>();
            var migrationRunner = scope.ServiceProvider.GetService<IMigrationRunner>();
            migrationRunner.DropData(versionTable);
        }

        private static IEnumerable<SupportedDatabaseTypes> SelectedDatabaseTypesToTest()
        {
            return TestParameters.DbParameters.Select(dbParameter => (SupportedDatabaseTypes) dbParameter.First()).ToList();
        }
    }
}