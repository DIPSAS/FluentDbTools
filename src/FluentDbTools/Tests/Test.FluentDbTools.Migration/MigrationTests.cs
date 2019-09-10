using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Dapper;
using FluentDbTools.Common.Abstractions;
using Example.FluentDbTools.Config;
using Example.FluentDbTools.Database;
using Example.FluentDbTools.Migration;
using Example.FluentDbTools.Migration.MigrationModels;
using FluentAssertions;
using FluentDbTools.Extensions.DbProvider;
using FluentDbTools.Extensions.Migration;
using FluentDbTools.Extensions.MSDependencyInjection;
using FluentDbTools.Extensions.MSDependencyInjection.Oracle;
using FluentDbTools.Extensions.MSDependencyInjection.Postgres;
using FluentDbTools.Migration;
using FluentDbTools.Migration.Abstractions;
using FluentDbTools.Migration.Oracle;
using TestUtilities.FluentDbTools;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Processors.Oracle;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Oracle.ManagedDataAccess.Client;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable PossibleNullReferenceException

namespace Test.FluentDbTools.Migration
{
    public class MigrationTests
    {
        private readonly ITestOutputHelper TestOutputHelper;

        public MigrationTests(ITestOutputHelper testOutputHelper)
        {
            TestOutputHelper = testOutputHelper;
        }

        [Theory]
        [MemberData(nameof(TestParameters.DbParameters), MemberType = typeof(TestParameters))]
        public void Migration_Success(SupportedDatabaseTypes databaseType)
        {
            var inMemoryOverrideConfig = OverrideConfig.GetInMemoryOverrideConfig(databaseType, OverrideConfig.NewRandomSchema);
            inMemoryOverrideConfig.TryGetValue("database:schema", out var schema);
            var logFile = $"Migration_Success_{schema}_{databaseType}.sql";
            inMemoryOverrideConfig.Add("Logging:Migration:ShowSql", "True");
            inMemoryOverrideConfig.Add("Logging:Migration:ShowElapsedTime", "True");
            inMemoryOverrideConfig.Add("Logging:Migration:File", logFile);
            var provider = MigrationBuilder.BuildMigration(databaseType, inMemoryOverrideConfig);

            using (var scope = provider.CreateScope())
            {
                var migrationRunner = scope.ServiceProvider.GetService<IMigrationRunner>();
                var versionTable = scope.ServiceProvider.GetService<IVersionTableMetaData>();

                migrationRunner.MigrateUp();

                migrationRunner.MigrateDown(0);

                migrationRunner.DropSchema(versionTable);
            }

            ShowLogFileContent(logFile);
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

        [Theory]
        [MemberData(nameof(TestParameters.DbParameters), MemberType = typeof(TestParameters))]
        public void MigrationWithExecutorExtensionWithVersionInfo_Success(SupportedDatabaseTypes databaseType)
        {
            var inMemoryOverrideConfig = OverrideConfig.GetInMemoryOverrideConfig(databaseType, OverrideConfig.NewRandomSchema);
            var configuration = new ConfigurationBuilder()
                .AddDbToolsExampleConfiguration(databaseType)
                .AddInMemoryCollection(inMemoryOverrideConfig)
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddScoped<IConfiguration>(provider => configuration)
                .AddDefaultDbMigrationConfig()
                .AddOracleDbProvider()
                .AddPostgresDbProvider()
                .BuildServiceProvider();
            
            var dbConfig = serviceProvider.GetService<IDbMigrationConfig>();

            var assemblies = MigrationBuilder.MigrationAssemblies.ToList();
            assemblies.Add(typeof(ExampleVersionTable).Assembly);

            var runner = dbConfig.GetMigrationRunner(assemblies);

            try
            {
                runner.MigrateUp();
                VerifyExpectedVersionInfoTable(serviceProvider, nameof(ExampleVersionTable));
            }
            finally
            {
                //runner.MigrateDown(0);
                dbConfig.DropSchema(assemblies);
            }
        }


        [Fact]
        public void OracleMigration_WhenDataSourceIsValidEzConnect_Success()
        {
            var databaseType = SupportedDatabaseTypes.Oracle;
            var defaulDbConfig = MigrationBuilder.BuildMigration(databaseType).GetDbConfig();

            var inMemoryOverrideConfig = OverrideConfig.GetInMemoryOverrideConfig(databaseType, OverrideConfig.NewRandomSchema);
            inMemoryOverrideConfig["database:dataSource"] = $"{defaulDbConfig.Hostname}/{defaulDbConfig.DatabaseName}";
            inMemoryOverrideConfig["database:connectionTimeoutInSecs"] = "5";

            var expectedDataSource = inMemoryOverrideConfig["database:dataSource"];
            var provider = MigrationBuilder.BuildMigration(databaseType, inMemoryOverrideConfig, sp => sp.AddOracleDbProvider());

            using (var scope = provider.CreateScope())
            {
                var migrationRunner = scope.ServiceProvider.GetService<IMigrationRunner>();
                var versionTable = scope.ServiceProvider.GetService<IVersionTableMetaData>();
                var dbconfig = scope.ServiceProvider.GetDbConfig();

                dbconfig.Datasource.Should().Be(inMemoryOverrideConfig["database:dataSource"]);

                dbconfig.GetDbProviderFactory(true)
                    .CreateConnection()
                    .DataSource.Should().Be(expectedDataSource);

                migrationRunner.MigrateUp();

                migrationRunner.MigrateDown(0);

                migrationRunner.DropSchema(versionTable);
            }
        }

        [Fact]
        public void OracleMigration_AllMigrationConfigValuesShouldHaveExpectedValues()
        {

            var provider = MigrationBuilder.BuildMigration(SupportedDatabaseTypes.Oracle);

            using (var scope = provider.CreateScope())
            {
                var allValues = scope.ServiceProvider.GetService<IDbMigrationConfig>().GetAllMigrationConfigValues();

                allValues.Should().NotBeNull();
                allValues.Should().NotBeEmpty();
                allValues["customMigrationValue1"].Should().Be("TEST:customMigrationValue1");
                allValues["customMigrationValue2"].Should().Be("TEST:customMigrationValue2");
                allValues["productXYValues:tableRoleName"].Should().Be("TEST:tableRoleName");
                allValues["productXYValues:codeRoleName"].Should().Be("TEST:codeRoleName");
                allValues["productXYValues:prefix"].Should().Be("XY");
                allValues["productXYValues:subsection:subconfig"].Should().Be("TEST:subconfig");
            }
        }

        [Fact]
        public void OracleMigration_SchemaPrefixHasExpectedValueAndMigrationSucceed()
        {
            var databaseType = SupportedDatabaseTypes.Oracle;
            var inMemoryOverrideConfig = OverrideConfig.GetInMemoryOverrideConfig(databaseType, OverrideConfig.NewRandomSchema);
            inMemoryOverrideConfig.TryGetValue("database:schema", out var schema);
            var logFile = $"Migration_Success_{schema}_{databaseType}.sql";

            inMemoryOverrideConfig.Add("Logging:Migration:ShowSql", "True");
            inMemoryOverrideConfig.Add("Logging:Migration:ShowElapsedTime", "True");
            inMemoryOverrideConfig.Add("Logging:Migration:File", logFile);
            inMemoryOverrideConfig.Add("database:migration:schemaPrefix:Id", "EX");
            inMemoryOverrideConfig.Add("database:migration:schemaPrefix:UniqueId", "exabcd-0000000001000");

            var provider = MigrationBuilder.BuildMigration(SupportedDatabaseTypes.Oracle, inMemoryOverrideConfig, sc =>
            {
                sc.AddSingleton<ICustomMigrationProcessor<OracleProcessor>,TestOracleCustomMigrationProcessor>();
                return sc;
            });

            using (var scope = provider.CreateScope())
            {
                var migrationRunner = scope.ServiceProvider.GetService<IMigrationRunner>();
                var dbMigrationConfig = scope.ServiceProvider.GetDbMigrationConfig();
                dbMigrationConfig.GetSchemaPrefixId().Should().Be("EX");
                dbMigrationConfig.GetSchemaPrefixUniqueId().Should().Be("exabcd-0000000001000");

                migrationRunner.MigrateUp();
                var oracleProcessor = scope.ServiceProvider.GetService<OracleProcessorBase>();
                oracleProcessor.TableExists(dbMigrationConfig.Schema, Table.Person.GetPrefixedName("EX"));
                oracleProcessor.SequenceExists(dbMigrationConfig.Schema, $"{Table.Person.GetPrefixedName("EX")}_seq");

                migrationRunner.MigrateDown(0);
                migrationRunner.DropSchema(scope.ServiceProvider.GetVersionTableMetaData());
            }

            ShowLogFileContent(logFile);
        }


        [Fact]
        public void OracleMigration_AllDatabaseConfigValuesShouldHaveExpectedValues()
        {

            var provider = MigrationBuilder.BuildMigration(SupportedDatabaseTypes.Oracle);

            using (var scope = provider.CreateScope())
            {
                var dbConfig = scope.ServiceProvider.GetService<IDbMigrationConfig>().GetDbConfig();
                var allValues = dbConfig
                    .GetAllDatabaseConfigValues();

                allValues.Should().NotBeNull();
                allValues.Should().NotBeEmpty();
                allValues["type"].Should().Be("Oracle");
                allValues["user"].Should().Be(dbConfig.User);
                allValues["adminUser"].Should().Be(dbConfig.AdminUser);
                allValues["hostname"].Should().Be(dbConfig.Hostname);
                allValues["port"].Should().Be(dbConfig.Port);

                allValues["migration:customMigrationValue1"].Should().Be("TEST:customMigrationValue1");
                allValues["migration:customMigrationValue2"].Should().Be("TEST:customMigrationValue2");
                allValues["migration:productXYValues:tableRoleName"].Should().Be("TEST:tableRoleName");
                allValues["migration:productXYValues:codeRoleName"].Should().Be("TEST:codeRoleName");
                allValues["migration:productXYValues:prefix"].Should().Be("XY");
                allValues["migration:productXYValues:subsection:subconfig"].Should().Be("TEST:subconfig");

            }
        }

        [Fact]
        public void OracleMigration_WhenDatasSourceIsValidTnsAliastName_Success()
        {
            var databaseType = SupportedDatabaseTypes.Oracle;

            var inMemoryOverrideConfig = OverrideConfig.GetInMemoryOverrideConfig(databaseType, OverrideConfig.NewRandomSchema);
            inMemoryOverrideConfig["database:dataSource"] = BaseConfig.InContainer ? "TNSTEST_INDOCKER" : "TNSTEST";
            inMemoryOverrideConfig["database:connectionTimeoutInSecs"] = "5";

            var expectedDataSource = inMemoryOverrideConfig["database:dataSource"];
            var provider = MigrationBuilder.BuildMigration(databaseType, inMemoryOverrideConfig, sp => sp.AddOracleDbProvider());

            using (var scope = provider.CreateScope())
            {
                var migrationRunner = scope.ServiceProvider.GetService<IMigrationRunner>();
                var versionTable = scope.ServiceProvider.GetService<IVersionTableMetaData>();
                var dbconfig = scope.ServiceProvider.GetDbConfig();

                dbconfig.Datasource.Should().Be(inMemoryOverrideConfig["database:dataSource"]);

                dbconfig
                    .GetDbProviderFactory(true).CreateConnection()
                    .DataSource.Should().Be(expectedDataSource);

                migrationRunner.MigrateUp();

                migrationRunner.MigrateDown(0);

                migrationRunner.DropSchema(versionTable);
            }
        }

        [Fact]
        public void OracleMigration_WhenDataSourceIsInvalidTnsAliastName_ShouldFailWithTnsResolvingError()
        {
            var databaseType = SupportedDatabaseTypes.Oracle;

            var inMemoryOverrideConfig = OverrideConfig.GetInMemoryOverrideConfig(databaseType, OverrideConfig.NewRandomSchema);
            inMemoryOverrideConfig["database:dataSource"] = "InvalidTnsAlias";
            inMemoryOverrideConfig["database:connectionTimeoutInSecs"] = "5";
            inMemoryOverrideConfig.TryGetValue("database:schema", out var schema);
            var logFile = $"Migration_Success_{schema}_{databaseType}.sql";

            inMemoryOverrideConfig.Add("Logging:Migration:ShowSql", "True");
            inMemoryOverrideConfig.Add("Logging:Migration:ShowElapsedTime", "True");
            inMemoryOverrideConfig.Add("Logging:Migration:File", logFile);

            var expectedDataSource = inMemoryOverrideConfig["database:dataSource"];
            var provider = MigrationBuilder.BuildMigration(databaseType, inMemoryOverrideConfig, sp => sp.AddOracleDbProvider());

            using (var scope = provider.CreateScope())
            {
                var migrationRunner = scope.ServiceProvider.GetService<IMigrationRunner>();
                var versionTable = scope.ServiceProvider.GetService<IVersionTableMetaData>();
                var dbconfig = scope.ServiceProvider.GetDbConfig();

                dbconfig.Datasource.Should().Be(inMemoryOverrideConfig["database:dataSource"]);
                dbconfig
                    .GetDbProviderFactory(true).CreateConnection()
                    .DataSource.Should().Be(expectedDataSource);

                Action action = () =>
                {
                    migrationRunner.MigrateUp();

                    migrationRunner.MigrateDown(0);

                    migrationRunner.DropSchema(versionTable);

                };

                // Unable to resolve ORA-12154: TNS:could not resolve the connect identifier specified 
                action.Should().Throw<OracleException>().Which.Number.Should().Be(12154);

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


        private static void VerifyExpectedVersionInfoTable(IServiceProvider serviceProvider, string versionTable)
        {
            using (var connection = serviceProvider
                .CreateScope()
                .ServiceProvider
                .GetRequiredService<IDbConnection>())
            {
                var version = connection.Query<QueryExampleVersionTable>($"select * from {versionTable}");
                version.Should().NotBeNull();
            }
        }

        private class QueryExampleVersionTable
        {
            public string Version { get; set; }
            public DateTimeOffset AppliedOn { get; set; }
            public string Description { get; set; }
        }
    }
}