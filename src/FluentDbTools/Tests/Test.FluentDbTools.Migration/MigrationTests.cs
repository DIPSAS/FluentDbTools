using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Dapper;
using FluentDbTools.Common.Abstractions;
using Example.FluentDbTools.Config;
using Example.FluentDbTools.Database;
using Example.FluentDbTools.Migration;
using FluentAssertions;
using FluentDbTools.Extensions.DbProvider;
using FluentDbTools.Extensions.Migration;
using FluentDbTools.Extensions.MSDependencyInjection;
using FluentDbTools.Extensions.MSDependencyInjection.Oracle;
using FluentDbTools.Extensions.MSDependencyInjection.Postgres;
using FluentDbTools.Migration;
using FluentDbTools.Migration.Abstractions;
using FluentDbTools.Migration.Contracts.MigrationExpressions.Execute;
using FluentDbTools.Migration.Oracle.CustomProcessor;
using FluentMigrator.Expressions;
using TestUtilities.FluentDbTools;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using Xunit;
using Xunit.Abstractions;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly, DisableTestParallelization = true)]

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

        public class CustomSqlTitleConverter1 : ICustomSqlTitleConverter
        {
            public string ConvertToTitle(string sql)
            {
                return sql;
            }
        }

        public class CustomSqlTitleConverter2 : ICustomSqlTitleConverter
        {
            public string ConvertToTitle(string sql)
            {
                return sql.ConvertToSqlTitle();
            }
        }


        [Theory]
        [MemberData(nameof(TestParameters.DbParameters), MemberType = typeof(TestParameters))]
        public void Migration_Success(SupportedDatabaseTypes databaseType)
        {
            var inMemoryOverrideConfig = OverrideConfig.GetInMemoryOverrideConfig(databaseType, OverrideConfig.NewRandomSchema);
            inMemoryOverrideConfig.TryGetValue("database:schema", out var schema);
            var logFile = $"Migration_Success_{schema}_{databaseType}.sql";
            var loglogFile = $"Migration_Success_{schema}_{databaseType}.log";
            inMemoryOverrideConfig.Add("Logging:Migration:ShowSql", "True");
            inMemoryOverrideConfig.Add("Logging:Migration:ShowElapsedTime", "True");
            inMemoryOverrideConfig.Add("Logging:Migration:File", logFile);
            inMemoryOverrideConfig.Add("Logging:File", loglogFile);
            var provider = MigrationBuilder.BuildMigration(databaseType, inMemoryOverrideConfig,
                collection => collection
                    .AddSingleton<ICustomSqlTitleConverter, CustomSqlTitleConverter1>()
                    .AddSingleton<ICustomSqlTitleConverter, CustomSqlTitleConverter2>());

            using (var scope = provider.CreateScope())
            {


                try
                {

                    var versionTable = scope.ServiceProvider.GetService<IVersionTableMetaData>();
                    var processor = scope.ServiceProvider.GetRequiredService<IExtendedMigrationProcessorOracle>();
                    var prefixId = scope.ServiceProvider.GetDbMigrationConfig().GetSchemaPrefixId();
                    var migrationRunner = scope.ServiceProvider.GetService<IMigrationRunner>();

                    migrationRunner.MigrateUp();
                    if (databaseType == SupportedDatabaseTypes.Oracle)
                    {
                        processor.ExecuteSql($"create or replace synonym {versionTable.SchemaName}.{Table.Testing}8 for {versionTable.SchemaName}.{Table.Testing.GetPrefixedName(prefixId)}");
                        processor.ProcessSql($"create or replace synonym {versionTable.SchemaName}.{Table.Testing}9 for {versionTable.SchemaName}.{Table.Testing.GetPrefixedName(prefixId)}", "Sql-title");
                    }

                    migrationRunner.MigrateDown(0);

                }
                finally
                {
                    scope.ServiceProvider.DropSchema();
                }
            }

            ShowLogFileContent(logFile);
        }

        [Theory]
        [MemberData(nameof(TestParameters.DbParameters), MemberType = typeof(TestParameters))]
        public void Migration_NoConnection_NoAccessToDatabaseIsDone(SupportedDatabaseTypes databaseType)
        {
            var inMemoryOverrideConfig = OverrideConfig.GetInMemoryOverrideConfig(databaseType, OverrideConfig.NewRandomSchema);
            inMemoryOverrideConfig.TryGetValue("database:schema", out var schema);
            var logFile = $"Migration_Success_{schema}_{databaseType}.sql";
            inMemoryOverrideConfig.Add("Logging:Migration:ShowSql", "True");
            inMemoryOverrideConfig.Add("Logging:Migration:ShowElapsedTime", "True");
            inMemoryOverrideConfig.Add("Logging:Migration:File", logFile);
            inMemoryOverrideConfig.Add("database:adminPassword", "invalid");

            var provider = MigrationBuilder.BuildMigration(databaseType, inMemoryOverrideConfig,
                collection =>
                    collection.Configure<RunnerOptions>(opt => opt.NoConnection = true));

            using (var scope = provider.CreateScope())
            {
                try
                {

                    var migrationRunner = scope.ServiceProvider.GetService<IMigrationRunner>();

                    migrationRunner.MigrateUp();

                    migrationRunner.MigrateDown(0);

                }
                finally
                {
                    scope.ServiceProvider.DropSchema();
                }
            }

            ShowLogFileContent(logFile);
        }

        [Theory]
        [MemberData(nameof(TestParameters.DbParameters), MemberType = typeof(TestParameters))]
        public void Migration_PreviewOnlyAndInvalidAdminPassword_ShouldThrowDatabaseException(SupportedDatabaseTypes databaseType)
        {
            var inMemoryOverrideConfig = OverrideConfig.GetInMemoryOverrideConfig(databaseType, OverrideConfig.NewRandomSchema);
            inMemoryOverrideConfig.TryGetValue("database:schema", out var schema);
            var logFile = $"Migration_Success_{schema}_{databaseType}.sql";
            inMemoryOverrideConfig.Add("Logging:Migration:ShowSql", "True");
            inMemoryOverrideConfig.Add("Logging:Migration:ShowElapsedTime", "True");
            inMemoryOverrideConfig.Add("Logging:Migration:File", logFile);
            inMemoryOverrideConfig["database:adminPassword"] = "invalid";

            var provider = MigrationBuilder.BuildMigration(databaseType, inMemoryOverrideConfig,
                collection =>
                    collection.Configure<ProcessorOptions>(opt => opt.PreviewOnly = true));

            using (var scope = provider.CreateScope())
            {
                Action action = () => scope.ServiceProvider.GetService<IMigrationRunner>().MigrateUp();
                if (databaseType == SupportedDatabaseTypes.Postgres)
                {
                    action.Should().Throw<PostgresException>().Which.SqlState.Should().Be("28P01");
                }

                if (databaseType == SupportedDatabaseTypes.Oracle)
                {
                    action.Should().Throw<OracleException>().Which.Number.Should().Be(1017);
                }

            }

            ShowLogFileContent(logFile);
        }

        [Theory]
        [MemberData(nameof(TestParameters.DbParameters), MemberType = typeof(TestParameters))]
        public void Migration_PreviewOnly_ShouldBeOk(SupportedDatabaseTypes databaseType)
        {
            var inMemoryOverrideConfig = OverrideConfig.GetInMemoryOverrideConfig(databaseType, OverrideConfig.NewRandomSchema);
            inMemoryOverrideConfig.TryGetValue("database:schema", out var schema);

            var config2 = new Dictionary<string,string>(inMemoryOverrideConfig);

            if (databaseType == SupportedDatabaseTypes.Postgres)
            {
                using (var scope = MigrationBuilder.BuildMigration(databaseType, inMemoryOverrideConfig).CreateScope())
                {
                    scope.ServiceProvider.CreateSchema();
                }
            }

            var logFile = $"Migration_Success_{schema}_{databaseType}.sql";
            inMemoryOverrideConfig.Add("Logging:Migration:ShowSql", "True");
            inMemoryOverrideConfig.Add("Logging:Migration:ShowElapsedTime", "True");
            inMemoryOverrideConfig.Add("Logging:Migration:File", logFile);
            inMemoryOverrideConfig["database:adminPassword"] = "invalid";



            var provider = MigrationBuilder.BuildMigration(databaseType, config2,
                collection =>
                    collection
                        .Configure<RunnerOptions>(opt => opt.NoConnection = true)
                        .Configure<ProcessorOptions>(opt => opt.PreviewOnly = true));

            using (var scope = provider.CreateScope())
            {
                try
                {
                    var migrationRunner = scope.ServiceProvider.GetService<IMigrationRunner>();

                    migrationRunner.MigrateUp();
                    migrationRunner.MigrateDown(0);
                }
                finally
                {
                    if (databaseType == SupportedDatabaseTypes.Postgres)
                    {
                        using (var scope2 = MigrationBuilder.BuildMigration(databaseType, config2).CreateScope())
                        {
                            scope2.ServiceProvider.CreateSchema();
                        }
                    }
                }

            }

            ShowLogFileContent(logFile);
        }




        [Fact]
        public void Migration_Logging_Test()
        {
            var databaseType = SupportedDatabaseTypes.Oracle;
            var inMemoryOverrideConfig = OverrideConfig.GetInMemoryOverrideConfig(databaseType, OverrideConfig.NewRandomSchema);

            var logFile = "Migration_Logging_Test.sql";
            inMemoryOverrideConfig.Add("Logging:Migration:ShowSql", "True");
            inMemoryOverrideConfig.Add("Logging:Migration:ShowElapsedTime", "True");
            inMemoryOverrideConfig.Add("Logging:Migration:File", logFile);
            var provider = MigrationBuilder.BuildMigration(databaseType, inMemoryOverrideConfig);

            using (var scope = provider.CreateScope())
            {
                var processor = scope.ServiceProvider.GetRequiredService<IExtendedMigrationProcessorOracle>();
                processor.Process(new PerformDBOperationExpression() { Operation = ((connection, transaction) => connection.Execute("select 0 from dual")) });
                processor.ProcessSql("select 1 from dual");
                processor.ProcessSql("select 2 from dual");
                processor.ProcessSql("select 3 from dual");

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
                .AddSingleton<IConfiguration>(provider => configuration)
                .AddDefaultDbMigrationConfig()
                .AddOracleDbProvider()
                .AddPostgresDbProvider()
                .BuildServiceProvider();

            var dbConfig = serviceProvider.GetService<IDbMigrationConfig>();

            var assemblies = MigrationBuilder.MigrationAssemblies.ToList();
            assemblies.Add(typeof(ExampleVersionTableMetaData).Assembly);

            var runner = dbConfig.GetMigrationRunner(assemblies);

            try
            {
                runner.MigrateUp();
                VerifyExpectedVersionInfoTable(serviceProvider, nameof(ExampleVersionTableMetaData));
            }
            finally
            {
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
                try
                {
                    var migrationRunner = scope.ServiceProvider.GetService<IMigrationRunner>();
                    var config = scope.ServiceProvider.GetDbConfig();

                    config.Datasource.Should().Be(inMemoryOverrideConfig["database:dataSource"]);

                    config.GetDbProviderFactory(true)
                        .CreateConnection()
                        .DataSource.Should().Be(expectedDataSource);

                    migrationRunner.MigrateUp();

                    migrationRunner.MigrateDown(0);

                }
                finally
                {
                    scope.ServiceProvider.DropSchema();

                }

            }
        }
        [Fact]
        public void OracleMigration_PasswordsWithSecrets_HasExpectedValues()
        {

            var provider = MigrationBuilder.BuildMigration(SupportedDatabaseTypes.Oracle,
                new Dictionary<string, string>
                {
                    { "database:secret:encoded:SYSTEMUSER", Convert.ToBase64String(Encoding.UTF8.GetBytes("systemPwd")) },
                    { "database:secret:encrypted:TESTUSER", Convert.ToBase64String( new SymmetricCryptoProvider().Encrypt("testUserPwd")) },
                    { "database:user", "TestUser" },
                    { "database:type", "oracle" },
                    { "database:adminUser", "SystemUser" }
                }, loadExampleConfig: false);

            using (var scope = provider.CreateScope())
            {
                var migrationConfig = scope.ServiceProvider.GetService<IDbMigrationConfig>();

                migrationConfig.GetDbConfig().User.Should().Be("TestUser");
                migrationConfig.GetDbConfig().Password.Should().Be("testUserPwd");

                migrationConfig.GetDbConfig().AdminUser.Should().Be("SystemUser");
                migrationConfig.GetDbConfig().AdminPassword.Should().Be("systemPwd");


                migrationConfig.Schema.Should().Be("TESTUSER");
                migrationConfig.SchemaPassword.Should().Be("testUserPwd");
            }
        }


        [Fact]
        public void OracleMigration_AllMigrationConfigValuesShouldHaveExpectedValues()
        {

            var provider = MigrationBuilder.BuildMigration(SupportedDatabaseTypes.Oracle,
                new Dictionary<string, string>
                {
                    { "database:migration:schemaPrefix:id", "EX" },
                    { "database:migration:schemaPrefix:uniqueId", "EXAbc" }
            });

            using (var scope = provider.CreateScope())
            {
                var allValues = scope.ServiceProvider.GetService<IDbMigrationConfig>().GetAllMigrationConfigValues();
                allValues.Should().NotBeNull();
                allValues.Should().NotBeEmpty();

                Action action = () => allValues["dont:exist"].Should().BeNullOrEmpty();
                action.Should().Throw<KeyNotFoundException>();

                allValues["schemaprefix:id"].Should().Be("EX");
                allValues["schemaPrefix:id"].Should().Be("EX");

                allValues["schemaprefix:uniqueid"].Should().Be("EXAbc");
                allValues["schemaPrefix:uniqueId"].Should().Be("EXAbc");

                allValues["customMigrationValue1"].Should().Be("TEST:customMigrationValue1");
                allValues["customMigrationValue2"].Should().Be("TEST:customMigrationValue2");
                allValues["productXYValues:tableRoleName"].Should().Be("TEST:tableRoleName");
                allValues["productXYValues:coderolename"].Should().Be("TEST:codeRoleName");
                allValues["productXYValues:prefix"].Should().Be("XY");
                allValues["productXYValues:subsection:subconfig"].Should().Be("TEST:subconfig");
            }
        }

        [Fact]
        public void OracleMigration_SchemaPrefixHasExpectedValueAndMigrationSucceed()
        {
            FluentMigrationLoggingExtensions.UseLogFileAppendFluentMigratorLoggerProvider = true;

            var schemaPrefixId = "EX";
            var schemaPrefixUniqueId = "exabcd-0000000001000";
            var databaseType = SupportedDatabaseTypes.Oracle;
            var inMemoryOverrideConfig = OverrideConfig.GetInMemoryOverrideConfig(databaseType, OverrideConfig.NewRandomSchema);
            inMemoryOverrideConfig.TryGetValue("database:schema", out var schema);
            var logFile = $"Migration_Success_{schema}_{databaseType}.sql";

            inMemoryOverrideConfig.Add("Logging:Migration:ShowSql", "True");
            inMemoryOverrideConfig.Add("Logging:Migration:ShowElapsedTime", "True");
            inMemoryOverrideConfig.Add("Logging:Migration:File", logFile);
            inMemoryOverrideConfig.Add("database:schemaprefix:id", schemaPrefixId);
            inMemoryOverrideConfig.Add("database:migration:schemaprefix:uniqueId", schemaPrefixUniqueId);

            inMemoryOverrideConfig.Add("database:schemaprefix:tables:person:shortName", "sn");
            inMemoryOverrideConfig.Add("database:schemaprefix:tables:person:globalId", "glob");
            //inMemoryOverrideConfig.Add("database:migration:migrationName", "migrationName");
            inMemoryOverrideConfig.Add("database:migration:name", "name");


            inMemoryOverrideConfig.Add("database:migration:schemaprefix:triggersAndViewsGeneration:tables:person", "both");
            inMemoryOverrideConfig.Add("database:migration:schemaprefix:triggersAndViewsGeneration:tables:exparent", "triggers");

            File.Delete(logFile);

            var provider = MigrationBuilder.BuildMigration(SupportedDatabaseTypes.Oracle, inMemoryOverrideConfig, sc => sc.RegisterCustomMigrationProcessor());
            using (provider as ServiceProvider)
            using (var scope = provider.CreateScope())

            {
                var migrationRunner = scope.ServiceProvider.GetService<IMigrationRunner>();
                var dbMigrationConfig = scope.ServiceProvider.GetDbMigrationConfig();
                dbMigrationConfig.GetSchemaPrefixId().Should().Be(schemaPrefixId);
                dbMigrationConfig.GetSchemaPrefixUniqueId().Should().Be("exabcd-0000000001000");

                var personLog = new ChangeLogContext(dbMigrationConfig, Table.Person);
                var parentLog = new ChangeLogContext(dbMigrationConfig, Table.Parent);
                var unknownLog = new ChangeLogContext(dbMigrationConfig, "Unknown") { EnabledTriggersAndViewsGeneration = TriggersAndViewsGeneration.Disabled };
                var unknown2Log = new ChangeLogContext(dbMigrationConfig, "Unknown2");

                personLog.SchemaPrefixId.Should().Be(schemaPrefixId);
                personLog.SchemaPrefixUniqueId.Should().Be(schemaPrefixUniqueId);
                personLog.ShortName.Should().Be($"{schemaPrefixId}sn");
                personLog.GlobalId.Should().Be("glob");
                personLog.EnabledTriggersAndViewsGeneration.Should().Be(TriggersAndViewsGeneration.Both);


                parentLog.SchemaPrefixId.Should().Be(schemaPrefixId);
                parentLog.SchemaPrefixUniqueId.Should().Be(schemaPrefixUniqueId);
                parentLog.ShortName.Should().BeNullOrEmpty();
                parentLog.GlobalId.Should().BeNullOrEmpty();
                parentLog.EnabledTriggersAndViewsGeneration.Should().Be(TriggersAndViewsGeneration.Triggers);

                unknownLog.SchemaPrefixId.Should().Be(schemaPrefixId);
                unknownLog.SchemaPrefixUniqueId.Should().Be(schemaPrefixUniqueId);
                unknownLog.ShortName.Should().BeNullOrEmpty();
                unknownLog.GlobalId.Should().BeNullOrEmpty();
                unknownLog.EnabledTriggersAndViewsGeneration.Should().Be(TriggersAndViewsGeneration.Disabled);

                unknown2Log.SchemaPrefixId.Should().Be(schemaPrefixId);
                unknown2Log.SchemaPrefixUniqueId.Should().Be(schemaPrefixUniqueId);
                unknown2Log.ShortName.Should().BeNullOrEmpty();
                unknown2Log.GlobalId.Should().BeNullOrEmpty();
                unknown2Log.EnabledTriggersAndViewsGeneration.Should().BeNull();


                migrationRunner.MigrateUp();

                var oracleProcessor = scope.ServiceProvider.GetService<IExtendedMigrationProcessorOracle>();
                oracleProcessor.TableExists(dbMigrationConfig.Schema, Table.Person).Should().BeFalse();
                oracleProcessor.TableExists(dbMigrationConfig.Schema, Table.Parent).Should().BeFalse();
                oracleProcessor.TableExists(dbMigrationConfig.Schema, "Unknown").Should().BeFalse();

                oracleProcessor.TableExists(dbMigrationConfig.Schema, Table.Person.GetPrefixedName(schemaPrefixId)).Should().BeTrue();
                oracleProcessor.TableExists(dbMigrationConfig.Schema, Table.Parent.GetPrefixedName(schemaPrefixId)).Should().BeTrue();
                oracleProcessor.TableExists(dbMigrationConfig.Schema, "Unknown".GetPrefixedName(schemaPrefixId)).Should().BeFalse();

                oracleProcessor.SequenceExists(dbMigrationConfig.Schema, $"{Table.Person.GetPrefixedName(schemaPrefixId)}_seq");

                migrationRunner.MigrateDown(0);
                migrationRunner.DropSchema(scope.ServiceProvider.GetVersionTableMetaData());
            }

            provider = MigrationBuilder.BuildMigration(SupportedDatabaseTypes.Oracle, inMemoryOverrideConfig, sc => sc.RegisterCustomMigrationProcessor());

            using (provider as ServiceProvider)
            using (var scope = provider.CreateScope())
            {
                var migrationRunner = scope.ServiceProvider.GetService<IMigrationRunner>();
                migrationRunner.MigrateDown(0);
            }

            provider = MigrationBuilder.BuildMigration(SupportedDatabaseTypes.Oracle, inMemoryOverrideConfig, sc => sc.RegisterCustomMigrationProcessor());

            using (provider as ServiceProvider)
            using (var scope = provider.CreateScope())
            {
                var migrationRunner = scope.ServiceProvider.GetService<IMigrationRunner>();
                migrationRunner.DropSchema(scope.ServiceProvider.GetVersionTableMetaData());
            }

            ShowLogFileContent(logFile);
            FluentMigrationLoggingExtensions.UseLogFileAppendFluentMigratorLoggerProvider = false;
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
        public void OracleMigration_WhenDataSourceIsValidTnsAliasName_Success()
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
        public void OracleMigration_WhenDataSourceIsInvalidTnsAliasName_ShouldFailWithTnsResolvingError()
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
                var config = scope.ServiceProvider.GetDbConfig();

                config.Datasource.Should().Be(inMemoryOverrideConfig["database:dataSource"]);
                config
                    .GetDbProviderFactory(true).CreateConnection()
                    .DataSource.Should().Be(expectedDataSource);

                Action action = () =>
                {
                    try
                    {
                        migrationRunner.MigrateUp();

                        migrationRunner.MigrateDown(0);
                    }
                    finally
                    {
                        migrationRunner.DropSchema(versionTable);
                    }
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