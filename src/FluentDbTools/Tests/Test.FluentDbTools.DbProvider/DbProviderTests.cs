using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Example.FluentDbTools.Common;
using Example.FluentDbTools.Config;
using FluentDbTools.Common.Abstractions;
using Example.FluentDbTools.Database;
using FluentAssertions;
using FluentDbTools.Extensions.DbProvider;
using FluentDbTools.DbProviders;
using FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Oracle.ManagedDataAccess.Client;
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
            await DbExampleExecutor.ExecuteDbExample(databaseType, false, overrideConfig);
        }

        [Theory]
        [MemberData(nameof(TestParameters.DbParameters), MemberType = typeof(TestParameters))]
        public async Task DbProvider_ExampleRepository_WithDbProviderFactory_Success(SupportedDatabaseTypes databaseType)
        {
            var overrideConfig = OverrideConfig.GetInMemoryOverrideConfig(databaseType);
            await DbExampleExecutor.ExecuteDbExample(databaseType, true, overrideConfig);
        }

        [Fact]
        public void GetConnectionString_OracleWithEzConnect_ShoulReturnExpectedDataSourceString()
        {
            var overridedConfig = new Dictionary<string, string>
            {
                {"database:databaseConnectionName", "host/service"},
                {"database:hostname", ""}
            };

            var dbConfig = GetDbConfig(overridedConfig);

            var provider = new OracleProvider();

            var expectedDataSource = string.Format(OracleProvider.DefaultDataSourceTemplate, "host", dbConfig.Port, "service");

            var connections = new[]
            {
                provider.GetConnectionString(dbConfig),
                provider.GetAdminConnectionString(dbConfig)

            };
            foreach (var connection in connections)
            {
                using (var oracleConnection = new OracleConnection(connection))
                {
                    oracleConnection.DataSource.Should().Be(expectedDataSource);
                }
            }
        }

        [Fact]
        public void GetConnectionString_OracleWithDefaultConnectionParameters_ShoulReturnExpectedDataSourceString()
        {
            var overridedConfig = new Dictionary<string, string>
            {
                {"database:databaseConnectionName", "service"},
                {"database:hostname", "host"}
            };

            var dbConfig = GetDbConfig(overridedConfig);

            var provider = new OracleProvider();

            var expectedDataSource = string.Format(OracleProvider.DefaultDataSourceTemplate, "host", dbConfig.Port, "service");

            var connections = new[]
            {
                provider.GetConnectionString(dbConfig),
                provider.GetAdminConnectionString(dbConfig)

            };
            foreach (var connection in connections)
            {
                using (var oracleConnection = new OracleConnection(connection))
                {
                    oracleConnection.DataSource.Should().Be(expectedDataSource);
                }
            }
        }


        [Fact]
        public void GetConnectionString_OracleWithServiceNameOnly_ShoulReturnExpectedDataSourceString()
        {
            var overridedConfig = new Dictionary<string, string>
            {
                {"database:databaseConnectionName", "service"},
                {"database:hostname", ""}
            };

            var dbConfig = GetDbConfig(overridedConfig);

            var provider = new OracleProvider();

            var expectedDataSource = dbConfig.DatabaseConnectionName;

            var connections = new[]
            {
                provider.GetConnectionString(dbConfig),
                provider.GetAdminConnectionString(dbConfig)

            };
            foreach (var connection in connections)
            {
                using (var oracleConnection = new OracleConnection(connection))
                {
                    oracleConnection.DataSource.Should().Be(expectedDataSource);
                }
            }
        }

        [Fact]
        public void GetConnectionString_OracleWithServiceNameOnly_ConnectionSuccess()
        {
            var overridedConfig = new Dictionary<string, string>
            {
                {"database:databaseConnectionName", BaseConfig.InContainer ? "TNSTEST_INDOCKER" : "TNSTEST"},
                {"database:hostname", ""}
            };

            var dbConfig = GetDbConfig(overridedConfig);
            var expectedDataSource = dbConfig.DatabaseConnectionName;
            using (var oracleConnection = new OracleConnection(new OracleProvider().GetAdminConnectionString(dbConfig)))
            {
                oracleConnection.DataSource.Should().Be(expectedDataSource);
                oracleConnection.Open();
            }

        }


        private IDbConfig GetDbConfig(Dictionary<string, string> overrideConfig = null)
        {
            return new ServiceCollection()
                .UseExampleConfiguration(SupportedDatabaseTypes.Oracle, overrideConfig)
                .AddScoped<IDbConfig, MSDbConfig>()
                .BuildServiceProvider()
                .GetRequiredService<IDbConfig>();
        }
    }
}