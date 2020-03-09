using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using FluentAssertions;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Extensions.Migration;
using FluentDbTools.Extensions.Migration.DefaultConfigs;
using FluentDbTools.Extensions.MSDependencyInjection;
using FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs;
using FluentDbTools.Migration.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace Test.FluentDbTools.Migration
{
    public class DbConfigReloadTests
    {
        [Fact]
        public void GetDbConfigValues_ChangeConfigFileContent_ChangedConfigDbValuesShouldBeReloaded()
        {
            using (var provider = GetServiceProvider())
            using (var scope = provider.CreateScope())
            {

                var dbConfig = scope.ServiceProvider.GetDbConfig();
                var dbMigration = scope.ServiceProvider.GetDbMigrationConfig();
                var user = dbConfig.User;
                var password = dbConfig.Password;
                var dbType = dbConfig.DbType;
                var port = dbConfig.Port;
                var pooling = dbConfig.Pooling;
                var databaseConfigValues = dbConfig.GetAllDatabaseConfigValues();

                var schemaPassword = dbMigration.SchemaPassword;
                var migrationName = dbMigration.GetMigrationName();
                var migrationConfigValues = dbMigration.GetAllMigrationConfigValues();

                ChangeTestConfigContent();

                Task.Delay(TimeSpan.FromSeconds(1)).Wait();

                dbConfig.User.Should().NotBe(user);
                dbConfig.Password.Should().NotBe(password);
                dbConfig.DbType.Should().NotBe(dbType);
                dbConfig.Port.Should().NotBe(port);
                dbConfig.Pooling.Equals(pooling).Should().BeFalse();

                dbMigration.SchemaPassword.Should().NotBe(schemaPassword);
                dbMigration.GetMigrationName().Should().NotBe(migrationName);

                dbConfig.User.EqualsIgnoreCase("Changed-" + nameof(dbConfig.User)).Should().BeTrue();
                dbConfig.Password.EqualsIgnoreCase("Changed-" + nameof(dbConfig.Password)).Should().BeTrue();
                dbConfig.DbType.Should().Be(SupportedDatabaseTypes.Postgres);
                dbConfig.Port.Should().Be("2");

                dbMigration.SchemaPassword.EqualsIgnoreCase("Changed-" + nameof(dbMigration.SchemaPassword)).Should().BeTrue();
                dbMigration.GetMigrationName().EqualsIgnoreCase("Changed-migrationName").Should().BeTrue();

                foreach (var (key, value) in databaseConfigValues)
                {
                    dbConfig.GetAllDatabaseConfigValues().GetValue(key).Should().NotBe(value);
                }

                foreach (var (key, value) in migrationConfigValues)
                {
                    dbMigration.GetAllMigrationConfigValues().GetValue(key).Should().NotBe(value);
                }
            }

        }

        private ServiceProvider GetServiceProvider()
        {
            var assemblies = new [] { Assembly.GetEntryAssembly(), Assembly.GetCallingAssembly(), Assembly.GetExecutingAssembly() };
            var configuration = (new ConfigurationBuilder().AddJsonFile(CreateTestConfig(), false, true)).Build();
            return new ServiceCollection()
                .AddSingleton<IConfiguration>(sc => configuration)
                .AddDefaultDbMigrationConfig()
                .BuildServiceProvider();
        }

        private static string CreateTestConfig()
        {
            var destFileName = GetTestConfigFile();
            File.Copy(GetDefaultConfigFile(), GetTestConfigFile(), true);
            return destFileName;
        }


        private static void ChangeTestConfigContent()
        {
            File.Copy(GetChangedConfigFile(), GetTestConfigFile(), true);
        }


        private static string GetTestConfigFile()
        {
            return Path.Join(GetDefaultConfigDirectory(), "test.config.json");
        }

        private static string GetChangedConfigFile()
        {
            return Path.Join(GetDefaultConfigDirectory(), "changed.config.json");
        }


        private static string GetDefaultConfigFile()
        {
            return Path.Join(GetDefaultConfigDirectory(), "default.config.json");
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private static string GetDefaultConfigDirectory()
        {
            try
            {
                var info = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent;

                var configDirInfo = new DirectoryInfo(Path.Join(info.FullName, "ConfigFiles"));
                return configDirInfo.Exists
                    ? configDirInfo.FullName
                    : new DirectoryInfo("ConfigFiles").FullName;

            }
            catch
            {
                return new DirectoryInfo("ConfigFiles").FullName;
            }
        }


    }

    [DataContract(Name = "database")]
    public class TestDbConfig : IDbConfig
    {
        public SupportedDatabaseTypes DbType { get; }
        public string Schema { get; set; }
        public string DatabaseName { get; set; }
        public string GetSchemaPrefixId()
        {
            return string.Empty;
        }

        public string GetSchemaPrefixUniqueId()
        {
            return string.Empty;
        }

        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public string User { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public string Password { get; set; }

        public string AdminUser { get; set; }
        public string AdminPassword { get; set; }
        public string Datasource { get; set; }
        public string Hostname { get; set; }
        public string Port { get; set; }
        public bool Pooling { get; set; }
        public string ConnectionTimeoutInSecs { get; set; }
        public string ConnectionString { get; set; }
        public string AdminConnectionString { get; set; }
        public IDictionary<string, string> GetAllDatabaseConfigValues(bool reload = false, string sectionName = null)
        {
            return new Dictionary<string, string>();
        }
    }
}