using System;
using System.Collections.Generic;
using Example.FluentDbTools.Config;
using FluentAssertions;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.DbProviders;
using FluentDbTools.Extensions.DbProvider;
using FluentDbTools.Extensions.MSDependencyInjection;
using FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs;
using FluentDbTools.Extensions.MSDependencyInjection.Oracle;
using Microsoft.Extensions.DependencyInjection;
using Oracle.ManagedDataAccess.Client;
using Xunit;
using OracleConnectionStringBuilder = FluentDbTools.DbProviders.OracleConnectionStringBuilder;

// ReSharper disable PossibleNullReferenceException

namespace Test.FluentDbTools.DbProvider
{
    public class OracleConnectionStringBuilderTests
    {
        [Fact]
        public void BuildConnectionString_WithEzConnect_ShouldReturnExpectedDataSourceString()
        {
            var overridedConfig = new Dictionary<string, string>
            {
                {"database:dataSource", "host/service"},
            };
            
            var dbConfig = GetDbConfig(overridedConfig);

            var expectedDataSource = overridedConfig["database:dataSource"];
            
            var adminPrivilegeses = new[] { false, true };

            foreach (var adminPrivileges in adminPrivilegeses)
            {
                using (var connection = dbConfig.GetDbProviderFactory(adminPrivileges).CreateConnection())
                {
                    connection.DataSource.Should().Be(expectedDataSource);
                }
            }
        }

        [Fact]
        public void BuildConnectionString_WithDefaultConnectionParameters_ShouldReturnExpectedDataSourceString()
        {
            var overridedConfig = new Dictionary<string, string>
            {
                {"database:servicename", "service"},
                {"database:hostname", "host"}
            };

            var dbConfig = GetDbConfig(overridedConfig);

            var expectedDataSource = OracleConnectionStringBuilder.GetDefaultDataSource("host", dbConfig.Port, "service");

            var adminPrivilegeses = new[] { false, true };

            foreach (var adminPrivileges in adminPrivilegeses)
            {
                using (var connection = dbConfig.GetDbProviderFactory(adminPrivileges).CreateConnection())
                {
                    connection.DataSource.Should().Be(expectedDataSource);
                }
            }
        }

        [Fact]
        public void BuildConnectionString_WithPoolingOff_ShouldReturnExpectedConnectionString()
        {
            var overridedConfig = new Dictionary<string, string>
            {
                {"database:servicename", "service"},
                {"database:hostname", "host"},
                {"database:pooling", "false"},
                {"database:poolingKeyValues", "Key1=Value1"},
            };

            var dbConfig = GetDbConfig(overridedConfig);


            var adminPrivilegeses = new[] { false, true };

            foreach (var adminPrivileges in adminPrivilegeses)
            {
                using (var connection = dbConfig.GetDbProviderFactory(adminPrivileges).CreateConnection() as OracleConnection)
                {
                    connection.ConnectionString.ContainsIgnoreCase("Pooled=False");
                }
            }
        }

        [Fact]
        public void BuildConnectionString_WithPoolingOn_ShouldReturnExpectedConnectionString()
        {
            var overridedConfig = new Dictionary<string, string>
            {
                {"database:servicename", "service"},
                {"database:hostname", "host"},
                {"database:pooling", "true"},
                {"database:poolingKeyValues", "MIN POOL SIZE=2;MAX POOL SIZE=50;INCR POOL SIZE=5;DECR POOL SIZE=2"},
            };

            var dbConfig = GetDbConfig(overridedConfig);


            var adminPrivilegeses = new[] { false, true };

            foreach (var adminPrivileges in adminPrivilegeses)
            {
                using (var connection = dbConfig.GetDbProviderFactory(adminPrivileges).CreateConnection() as OracleConnection)
                {
                    connection.ConnectionString.ContainsIgnoreCase("Pooled=True");
                    connection.ConnectionString.ContainsIgnoreCase("MIN POOL SIZE=5");
                }
            }
        }


        [Fact]
        public void BuildConnectionString_WhenDatasourceIsTnsAliasName_ShouldReturnExpectedDataSourceString()
        {
            var overridedConfig = new Dictionary<string, string>
            {
                {"database:dataSource", "TnsAliasName"},
            };

            var dbConfig = GetDbConfig(overridedConfig);

            var expectedDataSource = dbConfig.Datasource;

            var adminPrivilegeses = new[] { false, true };

            foreach (var adminPrivileges in adminPrivilegeses)
            {
                using (var connection = dbConfig.GetDbProviderFactory(adminPrivileges).CreateConnection())
                {
                    connection.DataSource.Should().Be(expectedDataSource);
                }
            }
        }

        [Fact]
        public void BuildConnectionString_WhenDatasourceIsValidTnsAliasName_VerifyThatConnectionSucceed()
        {
            var overridedConfig = new Dictionary<string, string>
            {
                {"database:dataSource", BaseConfig.InContainer ? "TNSTEST_INDOCKER" : "TNSTEST"},
            };

            var dbConfig = GetDbConfig(overridedConfig);
            var expectedDataSource = dbConfig.Datasource;

            Action action = () =>
            {
                using (var connection = dbConfig.GetDbProviderFactory(true).CreateConnection())
                {
                    connection.DataSource.Should().Be(expectedDataSource);
                    connection.SafeOpen();
                }
            };

            action.Should().NotThrow();
        }

        [Fact]
        public void BuildConnectionString_WhenDatasourceIsInvalidTnsAliasName_VerifyThatConnectionFailsWithTnsResolvingError()
        {
            var overridedConfig = new Dictionary<string, string>
            {
                {"database:dataSource", "InvalidTnsAlias"},
            };

            var dbConfig = GetDbConfig(overridedConfig);
            var expectedDataSource = dbConfig.Datasource;

            Action action = () =>
            {
                using (var oracleConnection = dbConfig.GetDbProviderFactory(true).CreateConnection())
                {
                    oracleConnection.DataSource.Should().Be(expectedDataSource);
                    oracleConnection.SafeOpen();
                }
            };

            // Unable to resolve ORA-12154: TNS:could not resolve the connect identifier specified 
            action.Should().Throw<OracleException>().Which.Number.Should().Be(12154);
        }

        [Fact]
        public void BuildConnectionString_WithStrangeConnectionStringFromConfig_ShouldReturnSameStrangeConnectionString()
        {

            const string strangeConnectionString = "User Id=user;Password=pwd;Some invalid text;InvalidHostAttribute=hos;InvalidPortAttribute=port;InvalidServiceNameAttribute=servicename;InvalidPoolingAttribute=pool;More invalid text;";
            var overridedConfig = new Dictionary<string, string>
            {
                {"database:connectionString", strangeConnectionString},
                {"database:adminConnectionString", strangeConnectionString},
            };
            var dbConfig = GetDbConfig(overridedConfig);

            var adminPrivilegeses = new[] { false, true };
            foreach (var adminPrivileges in adminPrivilegeses)
            {
                dbConfig.GetConnectionString().Should().Be(strangeConnectionString);
                Action action = () =>
                {
                    var conn = dbConfig.GetDbProviderFactory(adminPrivileges).CreateConnectionStringBuilder().ConnectionString;
                };
                var exp = action.Should().Throw<ArgumentException>();

            }
        }

        [Fact]
        public void BuildConnectionString_WithConnectionStringFromConfig_ShouldReturnExpectedDataSourceString()
        {
            const string expectedDataSource = "username/password@myserver//instancename";
            var expectedConnectionString = $"Data Source={expectedDataSource}";
            var overridedConfig = new Dictionary<string, string>
            {
                {"database:connectionString", expectedConnectionString},
                {"database:adminConnectionString", expectedConnectionString},
            };

            var dbConfig = GetDbConfig(overridedConfig);

            var adminPrivilegeses = new[] { false, true };
            foreach (var adminPrivileges in adminPrivilegeses)
            {
                using (var connection = dbConfig.GetDbProviderFactory(adminPrivileges).CreateConnection())
                {
                    connection.ConnectionString.Should().Be(expectedConnectionString);
                    connection.DataSource.Should().Be(expectedDataSource);
                }
            }
        }


        [Fact]
        public void VerifyOracleConnectionOpen_WhenDataSourceIsValidTnsAliasName_VerifyThatConnectionSucceed()
        {
            var overridedConfig = new Dictionary<string, string>
            {
                {"database:dataSource", BaseConfig.InContainer ? "TNSTEST_INDOCKER" : "TNSTEST"},
            };

            var dbConfig = GetDbConfig(overridedConfig);
            var expectedDataSource = dbConfig.Datasource;

            var connectionString = OracleConnectionStringBuilder.BuildConnectionString(dbConfig, expectedDataSource, true);

            Action action = () =>
            {
                using (var connection = new OracleConnection(connectionString))
                {
                    connection.SafeOpen();
                }
            };

            action.Should().NotThrow();
        }

        [Fact]
        public void VerifyOracleConnectionOpen_WhenDatasourcIsSimpleEzConnect_VerifyThatConnectionSucceed()
        {

            var dbConfig = GetDbConfig();
            var expectedDataSource = dbConfig.Datasource = $"{dbConfig.Hostname}/{dbConfig.DatabaseName}";

            var connectionString = OracleConnectionStringBuilder.BuildConnectionString(dbConfig, expectedDataSource, true);

            Action action = () =>
            {
                using (var connection = new OracleConnection(connectionString))
                {
                    connection.SafeOpen();
                }
            };

            action.Should().NotThrow();
        }

        [Fact]
        public void VerifyOracleConnectionOpen_WhenDatasourcIsAdvanceEzConnect1_VerifyThatConnectionSucceed()
        {

            var dbConfig = GetDbConfig();
            // hostname:port//instancename
            var expectedDataSource = dbConfig.Datasource = $"{dbConfig.Hostname}:{dbConfig.Port}//{dbConfig.DatabaseName}";

            var connectionString = OracleConnectionStringBuilder.BuildConnectionString(dbConfig, expectedDataSource, true);

            Action action = () =>
            {
                using (var connection = new OracleConnection(connectionString))
                {
                    connection.SafeOpen();
                }
            };

            action.Should().NotThrow();
        }

        [Fact]
        public void VerifyOracleConnectionOpen_WhenDatasourcIsAdvanceEzConnect2_VerifyThatConnectionSucceed()
        {

            var dbConfig = GetDbConfig();
            // hostname:port/serviceName
            var expectedDataSource = dbConfig.Datasource = $"{dbConfig.Hostname}:{dbConfig.Port}/{dbConfig.DatabaseName}";

            var connectionString = OracleConnectionStringBuilder.BuildConnectionString(dbConfig, expectedDataSource, true);

            Action action = () =>
            {
                using (var connection = new OracleConnection(connectionString))
                {
                    connection.SafeOpen();
                }
            };

            action.Should().NotThrow();
        }



        [Fact]
        public void VerifyOracleConnectionOpen_WithDecriptionDatasourceWithoutHostNameAndServiceNameIsValidTnsAliasName_VerifyThatConnectionFails()
        {
            var overridedConfig = new Dictionary<string, string>
            {
                {"database:serviceName", BaseConfig.InContainer ? "TNSTEST_INDOCKER" : "TNSTEST"},
            };

            var dbConfig = GetDbConfig(overridedConfig);
            var expectedDataSource = OracleConnectionStringBuilder.GetDefaultDataSource(dbConfig.Hostname, dbConfig.Port, dbConfig.DatabaseName);

            var connectionString = OracleConnectionStringBuilder.BuildConnectionString(dbConfig, expectedDataSource, true);

            Action action = () =>
            {
                using (var connection = new OracleConnection(connectionString))
                {
                    connection.SafeOpen();
                }
            };
            // ORA-12514: TNS:listener does not currently know of service requested in connect descriptor
            action.Should().Throw<OracleException>().Which.Number.Should().Be(12514);
        }

        [Fact]
        public void VerifyOracleConnectionOpen_WhenServiceNameIsEzConnect_VerifyThatConnectionFails()
        {

            var dbConfig = GetDbConfig();
            dbConfig.DatabaseName = $"{dbConfig.Hostname}/{dbConfig.DatabaseName}";

            var connectionString = dbConfig.GetAdminConnectionString();

            Action action = () =>
            {
                using (var connection = new OracleConnection(connectionString))
                {
                    connection.SafeOpen();
                }
            };

            // ORA-12514: TNS:listener does not currently know of service requested in connect descriptor
            action.Should().Throw<OracleException>().Which.Number.Should().Be(12514);
        }

        private static DbConfig GetDbConfig(Dictionary<string, string> overrideConfig = null)
        {
            overrideConfig = overrideConfig ?? new Dictionary<string, string>();
            overrideConfig["database:connectionTimeoutInSecs"] = "5";
            var sc = new ServiceCollection();
            return sc
                .UseExampleConfiguration(SupportedDatabaseTypes.Oracle, overrideConfig)
                .AddDbProvider(sc.GetDependecyInjectionDbConfigType())
                .AddOracleDbProvider()
                .BuildServiceProvider()
                .GetDbConfig() as DbConfig;
        }

    }
}