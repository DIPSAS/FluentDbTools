using FluentDbTools.Extensions.DbProvider;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Example.Migration;
using FluentDbTools.TestUtilities;
using FluentMigrator.Runner;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FluentDbTools.Migration.Tests
{
    public class MigrationTests
    {
        [Theory]
        [MemberData(nameof(TestParameters.DbParameters), MemberType = typeof(TestParameters))]
        public void Migration_Success(SupportedDatabaseTypes databaseType)
        {
            var inMemoryOverrideConfig = OverrideConfig.GetInMemoryOverrideConfig(databaseType, OverrideConfig.NewRandomSchema);
            var provider = MigrationBuilder.BuildMigration(databaseType, inMemoryOverrideConfig);
            
            using (var scope = provider.CreateScope())
            {
                var migrationRunner = scope.ServiceProvider.GetService<IMigrationRunner>();
                var versionTable = scope.ServiceProvider.GetService<IVersionTableMetaData>();
                
                migrationRunner.MigrateUp();
                
                migrationRunner.MigrateDown(0);
                
                migrationRunner.DropData(versionTable);
            }
        }
    }
}