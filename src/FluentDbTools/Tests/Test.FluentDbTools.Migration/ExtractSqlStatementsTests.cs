using System;
using System.IO;
using System.Linq;
using Example.FluentDbTools.Config;
using Example.FluentDbTools.Migration;
using FluentAssertions;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Extensions.Migration;
using FluentDbTools.Migration;
using FluentDbTools.Migration.Abstractions;
using FluentDbTools.Migration.Oracle;
using FluentDbTools.Migration.Oracle.CustomProcessor;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using TestUtilities.FluentDbTools;
using Xunit;
using Xunit.Abstractions;

namespace Test.FluentDbTools.Migration
{
    public class ExtractSqlStatementsTests
    {
        private readonly ITestOutputHelper TestOutputHelper;

        public ExtractSqlStatementsTests(ITestOutputHelper testOutputHelper)
        {
            TestOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ExtractSqlStatements_CreateTableWithErrorFilter_ShouldHaveCount110()
        {
            var sql = TestSqlResources.CreateTableWithErrorFilter.ExtractSqlStatements().ToArray();
            sql.Should().HaveCount(12);
            sql[0].StartsWithIgnoreCase("/* ErrorFilter").Should().BeTrue();
            sql[1].StartsWithIgnoreCase("create table").Should().BeTrue();
            sql[2].StartsWithIgnoreCase("/* ErrorFilter").Should().BeTrue();
            sql[3].StartsWithIgnoreCase("alter table").Should().BeTrue();
        }

        [Fact]
        public void ExtractSqlStatements_SmallScriptSql_ShouldHaveCount110()
        {
            var sql = TestSqlResources.SmallScriptSql.ExtractSqlStatements().ToArray();
            sql.Should().HaveCount(9);
            sql[1].StartsWithIgnoreCase("declare").Should().BeTrue();
            sql[1].EndsWithIgnoreCase("end;").Should().BeTrue();
        }

        [Fact]
        public void ExtractSqlStatements_LargeScriptSql_ShouldHaveCount1()
        {
            var sql = TestSqlResources.LargeScriptSql.ExtractSqlStatements().ToArray();
            sql.Should().HaveCount(110);
            sql[1].StartsWithIgnoreCase("create or replace").Should().BeTrue();
            sql[1].EndsWithIgnoreCase("dipscoredb.cpfelles").Should().BeTrue();
        }

        [Fact]
        public void ExtractSqlStatements_Large2ScriptSql_ShouldHaveCount1()
        {
            var sql = TestSqlResources.Large2ScriptSql.ExtractSqlStatements().ToArray();
            sql.Should().HaveCount(4);
            sql[3].StartsWithIgnoreCase("declare").Should().BeTrue();
            //sql[1].EndsWithIgnoreCase("dipscoredb.cpfelles").Should().BeTrue();
        }


        [Fact]
        public void ExtractSqlStatements_LogonScriptSql_ShouldHaveCount2()
        {
            var sql = SqlResources.LogonScriptSql.ExtractSqlStatements();
            sql.Should().HaveCount(2);
        }

        [Fact]
        public void ExtractSqlStatements_OneLineSql_ShouldHaveCount1()
        {
            var sql = "select 1 from dual".ExtractSqlStatements();
            sql.Should().HaveCount(1);
        }

        [Fact]
        public void ExtractSqlStatements_TwoLineSqlWithoutSemicolon_ShouldHaveCount1()
        {
            var sql = "select 1 \nfrom dual".ExtractSqlStatements();
            sql.Should().HaveCount(1);
        }

        [Fact]
        public void ExtractSqlStatements_TwoLineSqlWithSemicolon_ShouldHaveCount1()
        {
            var sql = "select 1 \nfrom dual;".ExtractSqlStatements();
            sql.Should().HaveCount(1);
        }


        [Fact]
        public void CheckThat_LargeScriptSql_IsFullyExecuted()
        {
            var schema = "DIPSEXAMPLEDB";
            var dataSource = BaseConfig.InContainer ? null : "vt-tp-db-demo.dips.local/DIPS";

            Migrate(DoTheTest, dataSource, schema);

            void DoTheTest(IServiceProvider serviceProvider)
            {
                if (!BaseConfig.InContainer)
                {
                    var processor = serviceProvider.GetRequiredService<IExtendedMigrationProcessorOracle>();
                    var sql = serviceProvider.GetDbMigrationConfig().PrepareSql(TestSqlResources.LargeScriptSql);

                    processor.ExecuteSql(sql);
                }
            }
        }


        [Fact]
        public void CheckThat_CoSchemaPrefix_IsAppended()
        {
            var schema = "DIPSEXAMPLEDB";
            var dataSource = BaseConfig.InContainer ? null : "vt-tp-db-demo.dips.local/DIPS";

            Migrate(DoTheTest, dataSource, schema);

            void DoTheTest(IServiceProvider serviceProvider)
            {

                if (!BaseConfig.InContainer)
                {
                    var processor = serviceProvider.GetRequiredService<IExtendedMigrationProcessorOracle>();

                    var template = $"select 1 from dipscoredb.coschemaprefix where schema = '{schema}'";
                    processor.Exists(template).Should().BeTrue();

                }
            }
        }

        public void Migrate(Action<IServiceProvider> testFunction, string dataSource = null, string schemaName = null)
        {

            var schemaPrefixId = "EX";
            var schemaPrefixUniqueId = "utvoja+0000000002506";
            var databaseType = SupportedDatabaseTypes.Oracle;
            var inMemoryOverrideConfig = OverrideConfig.GetInMemoryOverrideConfig(databaseType, schemaName ?? OverrideConfig.NewRandomSchema);
            inMemoryOverrideConfig.TryGetValue("database:schema", out var schema);
            var logFile = $"Migration_Success_{schema}_{databaseType}.sql";

            inMemoryOverrideConfig.Add("Logging:Migration:ShowSql", "True");
            inMemoryOverrideConfig.Add("Logging:Migration:ShowElapsedTime", "True");
            inMemoryOverrideConfig.Add("Logging:Migration:File", logFile);
            inMemoryOverrideConfig.Add("database:schemaName", schemaPrefixId);
            inMemoryOverrideConfig.Add("database:schemaprefix:id", schemaPrefixId);
            inMemoryOverrideConfig.Add("database:migration:schemaprefix:uniqueId", schemaPrefixUniqueId);

            inMemoryOverrideConfig.Add("database:schemaprefix:tables:person:shortName", "sn");
            inMemoryOverrideConfig.Add("database:schemaprefix:tables:person:globalId", "glob");
            //inMemoryOverrideConfig.Add("database:migration:migrationName", "migrationName");
            inMemoryOverrideConfig.Add("database:migration:name", "name");

            inMemoryOverrideConfig.Add("database::name", "name");

            if (dataSource.IsNotEmpty())
            {
                inMemoryOverrideConfig["database:dataSource"] = dataSource;
            }



            File.Delete(logFile);

            var provider = MigrationBuilder.BuildMigration(SupportedDatabaseTypes.Oracle, inMemoryOverrideConfig, sc => sc.RegisterCustomMigrationProcessor()) ;
            using (provider as ServiceProvider)
            using (var scope = provider.CreateScope())

            {
                var migrationRunner = scope.ServiceProvider.GetService<IMigrationRunner>();
                
                migrationRunner.DropSchema(scope.ServiceProvider.GetVersionTableMetaData());

                migrationRunner.MigrateUp();
                try
                {
                    testFunction.Invoke(scope.ServiceProvider);
                }
                finally
                {
                    migrationRunner.MigrateDown(0);
                    migrationRunner.DropSchema(scope.ServiceProvider.GetVersionTableMetaData());
                }
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