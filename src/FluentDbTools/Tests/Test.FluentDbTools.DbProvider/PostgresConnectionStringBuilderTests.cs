using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Example.FluentDbTools.Config;
using FluentAssertions;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Extensions.DbProvider;
using FluentDbTools.Extensions.MSDependencyInjection;
using FluentDbTools.Extensions.MSDependencyInjection.Postgres;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Xunit;
// ReSharper disable PossibleNullReferenceException

namespace Test.FluentDbTools.DbProvider
{
    public class PostgresConnectionStringBuilderTests
    {

        [Fact]
        public void BuildConnectionString_WithDefaultConnectionParameters_ShouldReturnExpectedDataSourceString()
        {
            var overridedConfig = new Dictionary<string, string>
            {
                {"database:databaseName", "postgres"}
            };


            var dbConfig = GetDbConfig(overridedConfig);
            var expectedDataSource = $"tcp://{dbConfig.Hostname}:{dbConfig.Port}";
            var adminPrivilegeses = new[] { false, true };

            foreach (var adminPrivileges in adminPrivilegeses)
            {
                using (var connection = dbConfig.GetDbProviderFactory(adminPrivileges).CreateConnection())
                {
                    try
                    {
                        connection.Open();
                    }
                    catch (PostgresException exception)
                    {
                        if (adminPrivileges == false && exception.SqlState == "28P01")
                        {
                            continue;
                        }
                    }

                    connection.DataSource.Should().Be(expectedDataSource);
                    var connectionString = dbConfig.GetDbProviderFactory(true).CreateConnectionStringBuilder().ConnectionString;
                    connectionString = connectionString.ReplaceIgnoreCase(connectionString.SubstringFromAdnIncludeToString("password=", ";"), "");
                    connection.ConnectionString.Should().Be(connectionString);

                }
            }
        }
        [Theory(DisplayName = "Should Connect Successfully independent if Timeout is configured or not")]
        [InlineData(true)]
        [InlineData(false)]
        public void BuildConnectionString_WithDefaultConnectionParameters_VerifyThatConnectionSucceed(bool setTimeout)
        {
            var overridedConfig = new Dictionary<string, string>
            {
                {"database:databaseName", "postgres"}
            };

            var dbConfig = GetDbConfig(overridedConfig, setTimeout);
            var expectedDataSource = $"tcp://{dbConfig.Hostname}:{dbConfig.Port}";

            using (var connection = dbConfig.GetDbProviderFactory(true).CreateConnection())
            {
                connection.Open();
                connection.DataSource.Should().Be(expectedDataSource);
                var connectionString = dbConfig.GetDbProviderFactory(true).CreateConnectionStringBuilder().ConnectionString;
                connectionString = connectionString.ReplaceIgnoreCase(connectionString.SubstringFromAdnIncludeToString("password=", ";"), "");
                connection.ConnectionString.Should().Be(connectionString);
            }
        }
        [Fact]
        public void BuildConnectionString_WithInvalidDatabase_VerifyThatConnectionFails()
        {
            var overridedConfig = new Dictionary<string, string>
            {
                {"database:databaseName", "InvalidDatabase"}
            };

            var dbConfig = GetDbConfig(overridedConfig);
            var expectedDataSource = $"tcp://{dbConfig.Hostname}:{dbConfig.Port}";


            Action action = () =>
            {
                using (var connection = dbConfig.GetDbProviderFactory(true).CreateConnection())
                {
                    connection.Open();
                    connection.DataSource.Should().Be(expectedDataSource);
                    var connectionString = dbConfig.GetDbProviderFactory(true).CreateConnectionStringBuilder().ConnectionString;
                    connectionString = connectionString.ReplaceIgnoreCase(connectionString.SubstringFromAdnIncludeToString("password=", ";"), "");
                    connection.ConnectionString.Should().Be(connectionString);
                }
            };
            // 3D000: invalid_catalog_name - database "InvalidDatabase" does not exists
            action.Should().Throw<PostgresException>().Which.SqlState.Should().Be("3D000");
        }

        [Fact]
        public void BuildConnectionString_WithInvalidHost_VerifyThatConnectionFails()
        {
            var overridedConfig = new Dictionary<string, string>
            {
                {"database:databaseName", "postgres"},
                {"database:hostname", "InvalidHost"}
            };

            var dbConfig = GetDbConfig(overridedConfig);
            var expectedDataSource = $"tcp://{dbConfig.Hostname}:{dbConfig.Port}";


            Action action = () =>
            {
                using (var connection = dbConfig.GetDbProviderFactory(true).CreateConnection())
                {
                    connection.Open();
                    connection.DataSource.Should().Be(expectedDataSource);
                    var connectionString = dbConfig.GetDbProviderFactory(true).CreateConnectionStringBuilder().ConnectionString;
                    connectionString = connectionString.ReplaceIgnoreCase(connectionString.SubstringFromAdnIncludeToString("password=", ";"), "");
                    connection.ConnectionString.Should().Be(connectionString);
                }
            };
            // HostNotFound - "No such host is known"
            var whichSocketErrorCode = action.Should().Throw<SocketException>().Which.SocketErrorCode;
            (whichSocketErrorCode is SocketError.TryAgain or SocketError.HostNotFound).Should().BeTrue($"'{whichSocketErrorCode:G}' was expected to be '{SocketError.TryAgain:G}' or '{SocketError.HostNotFound}'");
        }

        private static DbConfig GetDbConfig(Dictionary<string, string> overrideConfig = null, bool ignoreSettingTimeout = false)
        {
            if (!ignoreSettingTimeout)
            {
                overrideConfig = overrideConfig ?? new Dictionary<string, string>();
                overrideConfig["database:connectionTimeoutInSecs"] = "5";
            }

            return new ServiceCollection()
                .UseExampleConfiguration(SupportedDatabaseTypes.Postgres, overrideConfig)
                .AddDefaultDbConfig()
                .AddPostgresDbProvider()
                .BuildServiceProvider()
                .GetDbConfig() as DbConfig;
        }

    }
}