using System.Threading.Tasks;
using FluentDbTools.Common.Abstractions;
using Example.FluentDbTools.Database;
using Example.FluentDbTools.Migration;
using FluentDbTools.Migration;
using FluentDbTools.Migration.Abstractions;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Processors.Oracle;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.DependencyInjection;
using TestUtilities.FluentDbTools;
using Xunit;

namespace Test.FluentDbTools.DbProvider
{
    [Collection(TestCollectionFixtures.CollectionTag)]
    public class DbProviderTests
    {
        [Theory]
        [MemberData(nameof(TestParameters.DbParameters), MemberType = typeof(TestParameters))]
        public async Task DbProvider_ExampleRepository_Success(SupportedDatabaseTypes databaseType)
        {

            var overrideConfig = OverrideConfig.GetInMemoryOverrideConfig(databaseType);
            overrideConfig.Add("database:schemaprefix:id", "EX");
            overrideConfig.Add("database:migration:schemaprefix:id", "EX");

            var provider = MigrationBuilder.BuildMigration(databaseType, overrideConfig);
            using (provider as ServiceProvider)
            using (var scope = provider.CreateScope())
            {
                var migrationRunner = scope.ServiceProvider.GetService<IMigrationRunner>();
                migrationRunner.MigrateUp();
                await DbExampleExecutor.ExecuteDbExample(databaseType, overrideConfig);
                migrationRunner.DropSchema(scope.ServiceProvider.GetRequiredService<IVersionTableMetaData>());
            }
        }

        [Theory]
        [MemberData(nameof(TestParameters.DbParameters), MemberType = typeof(TestParameters))]
        public async Task DbProvider_ExampleRepository_WithDbProviderFactory_Success(SupportedDatabaseTypes databaseType)
        {
            var overrideConfig = OverrideConfig.GetInMemoryOverrideConfig(databaseType);
            await DbExampleExecutor.ExecuteDbExample(databaseType, overrideConfig);
        }
    }
}