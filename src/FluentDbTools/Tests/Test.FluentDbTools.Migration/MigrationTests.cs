using FluentDbTools.Common.Abstractions;
using Example.FluentDbTools.Common;
using Example.FluentDbTools.Migration;
using FluentDbTools.Extensions.Migration;
using FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs;
using FluentDbTools.Migration;
using TestUtilities.FluentDbTools;
using FluentMigrator.Runner;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Test.FluentDbTools.Migration
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
        
        [Theory]
        [MemberData(nameof(TestParameters.DbParameters), MemberType = typeof(TestParameters))]
        public void MigrationWithExecutorExtension_Success(SupportedDatabaseTypes databaseType)
        {
            var inMemoryOverrideConfig = OverrideConfig.GetInMemoryOverrideConfig(databaseType, OverrideConfig.NewRandomSchema);
            var configuration = new ConfigurationBuilder()
                .AddDbToolsExampleConfiguration(databaseType)
                .AddInMemoryCollection(inMemoryOverrideConfig)
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddScoped<IConfiguration>(provider => configuration)
                .AddScoped<IDbConfig, DefaultDbConfig>()
                .BuildServiceProvider();

            var dbConfig = serviceProvider.GetService<IDbConfig>();

            var runner = dbConfig.GetMigrationRunner(MigrationBuilder.MigrationAssemblies);
            
            runner.MigrateUp();

            dbConfig.DropData(MigrationBuilder.MigrationAssemblies);
        }
    }
}