using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Example.FluentDbTools.Config;
using FluentDbTools.Common.Abstractions;
using Example.FluentDbTools.Database;
using Example.FluentDbTools.Migration;
using FluentDbTools.Extensions.MSDependencyInjection;
using FluentDbTools.Migration;
using FluentDbTools.Migration.Abstractions;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Processors.Oracle;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestUtilities.FluentDbTools;
using Xunit;
using Xunit.Abstractions;

[assembly: CollectionBehavior(DisableTestParallelization = true, MaxParallelThreads = 1)]

namespace Test.FluentDbTools.DbProvider
{
    [Collection(TestCollectionFixtures.CollectionTag)]
    public class DbProviderTests
    {
        private readonly ITestOutputHelper TestOutputHelper;

        public DbProviderTests(ITestOutputHelper testOutputHelper)
        {
            TestOutputHelper = testOutputHelper;
        }


        [Theory]
        [MemberData(nameof(TestParameters.DbParameters), MemberType = typeof(TestParameters))]
        public async Task DbProvider_ExampleRepository_Success(SupportedDatabaseTypes databaseType)
        {

            var overrideConfig = OverrideConfig.GetInMemoryOverrideConfig(databaseType);
            overrideConfig.TryGetValue("database:schema", out var schema);
            overrideConfig.Add("database:schemaprefix:id", "EX");
            overrideConfig.Add("database:migration:schemaprefix:id", "EX");
            var logFile = $"DbProvider_ExampleRepository_Success_{schema}_{databaseType}.sql";
            var loglogFile = $"DbProvider_ExampleRepository_Success_{schema}_{databaseType}.log";
            overrideConfig.Add("Logging:Migration:ShowSql", "True");
            overrideConfig.Add("Logging:Migration:ShowElapsedTime", "True");
            overrideConfig.Add("Logging:Migration:File", logFile);
            overrideConfig.Add("Logging:File", loglogFile);

            var provider = MigrationBuilder.BuildMigration(databaseType, overrideConfig);
            using var scope = provider.CreateScope();
            try
            {
                var migrationRunner = scope.ServiceProvider.GetService<IMigrationRunner>();
                var configuration = scope.ServiceProvider.GetService<IConfiguration>();
                logFile = configuration.GetMigrationLogFile();
                migrationRunner.MigrateUp();
                await DbExampleExecutor.ExecuteDbExample(databaseType, overrideConfig);
                migrationRunner.DropSchema(scope.ServiceProvider.GetRequiredService<IVersionTableMetaData>());
            }
            catch (InvalidOperationException)
            {
                //
            }

            ShowLogFileContent(logFile);
        }

        private void ShowLogFileContent(string logFile)
        {
            if (logFile.IsNotEmpty() && File.Exists(logFile))
            {
                var tmpFile = logFile + ".tmp";
                if (File.Exists(tmpFile))
                {
                    File.Delete(tmpFile);
                }

                File.Copy(logFile, tmpFile);
                var logContent = File.ReadAllText(logFile + ".tmp");
                var s = $"************* LogContent {logFile} **************";
                WriteLine($"\n{s}");
                WriteLine($"{"".PadRight(s.Length, '*')}");
                WriteLine(logContent);

                if (File.Exists(tmpFile))
                {
                    File.Delete(tmpFile);
                }
            }
        }

        private void WriteLine(string message)
        {
            if (BaseConfig.InContainer)
            {
                Console.WriteLine(message);
                return;
            }

            TestOutputHelper.WriteLine(message);
        }


    }
}