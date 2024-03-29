﻿using System.Collections.Generic;
using FluentAssertions;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Contracts;
using FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Test.FluentDbTools.DbProvider
{
    public class DbConnectionConnectionStringTests
    {
        private const string EmptyOracleConnectionString = "User Id=USER;Password=password;Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=xe)));Pooling=True";
        private const string EmptyPostgresConnectionString = "Username=user;Password=password;Host=;Port=5432;Database=user;Pooling=True";

        [Theory]
        [InlineData(null, SupportedDatabaseTypes.Postgres)]
        [InlineData(SupportedDatabaseTypes.Postgres, SupportedDatabaseTypes.Postgres)]
        [InlineData(SupportedDatabaseTypes.Oracle, SupportedDatabaseTypes.Oracle)]
        public void DbConfigDbType_WithLibraryDefaultDbType_ShouldBeCorrectDbType(SupportedDatabaseTypes? configDbType, SupportedDatabaseTypes expectedDbType)
        {
            lock (TestCollectionFixtures.LockObj)
            {
                try
                {
                    var configuration = GetConfiguration(configDbType);

                    configuration.CreateDbConfig().DbType.Should().Be(expectedDbType);
                }
                finally
                {
                    DefaultDbConfigValues.WithLibraryDefaultDbType();
                }
            }
        }

        [Theory]
        [InlineData(null, SupportedDatabaseTypes.Oracle)]
        [InlineData(SupportedDatabaseTypes.Oracle, SupportedDatabaseTypes.Oracle)]
        [InlineData(SupportedDatabaseTypes.Postgres, SupportedDatabaseTypes.Postgres)]
        public void DbConfigDbType_WithDefaultDbTypeSetToOracle_ShouldBeCorrectDbType(SupportedDatabaseTypes? configDbType, SupportedDatabaseTypes expectedDbType)
        {
            lock (TestCollectionFixtures.LockObj)
            {
                try
                {
                    var configuration = GetConfiguration(configDbType);

                    DefaultDbConfigValues.WithOracleDefaultDbType();

                    configuration.CreateDbConfig().DbType.Should().Be(expectedDbType);
                }
                finally
                {
                    DefaultDbConfigValues.WithLibraryDefaultDbType();
                }
            }
        }

        [Theory]
        [InlineData(null, EmptyPostgresConnectionString)]
        [InlineData(SupportedDatabaseTypes.Postgres, EmptyPostgresConnectionString)]
        [InlineData(SupportedDatabaseTypes.Oracle, EmptyOracleConnectionString)]
        public void DbConfigConnectionString_WithLibraryDefaultDbType_ShouldBeCorrectConnectionString(SupportedDatabaseTypes? configDbType, string expectedConnectionString)
        {
            lock (TestCollectionFixtures.LockObj)
            {
                try
                {
                    var configuration = GetConfiguration(configDbType);

                    configuration.CreateDbConfig().ConnectionString.Should().Be(expectedConnectionString);

                    DefaultDbConfigValues.WithLibraryDefaultDbType();
                    DefaultDbConfigValues.WithLibraryDefaultAdminUserAndPassword();
                }
                finally
                {
                    DefaultDbConfigValues.WithLibraryDefaultDbType();
                    DefaultDbConfigValues.WithLibraryDefaultAdminUserAndPassword();
                }
            }
        }

        [Theory]
        [InlineData(null, EmptyOracleConnectionString)]
        [InlineData(SupportedDatabaseTypes.Oracle, EmptyOracleConnectionString)]
        [InlineData(SupportedDatabaseTypes.Postgres, EmptyPostgresConnectionString)]
        public void DbConfigConnectionString_WithDefaultDbTypeSetToOracle_ShouldBeCorrectConnectionString(SupportedDatabaseTypes? configDbType, string expectedConnectionString)
        {
            lock (TestCollectionFixtures.LockObj)
            {
                try
                {
                    var configuration = GetConfiguration(configDbType);

                    DefaultDbConfigValues.WithOracleDefaultDbType();
                    DefaultDbConfigValues.WithEmptyAdminUserAndPassword();

                    var dbConfig = configuration.CreateDbConfig();
                    dbConfig.ConnectionString.Should().Be(expectedConnectionString);
                    var useridStr = configDbType == SupportedDatabaseTypes.Postgres ? "username" : "user id";
                    dbConfig.AdminUser.Should().BeNull();
                    dbConfig.AdminPassword.Should().BeNull();
                    dbConfig.AdminConnectionString.ContainsIgnoreCase($"{useridStr}=;").Should().BeTrue();
                    dbConfig.AdminConnectionString.ContainsIgnoreCase("password=;").Should().BeTrue();
                }
                finally
                {
                    DefaultDbConfigValues.WithLibraryDefaultDbType();
                    DefaultDbConfigValues.WithLibraryDefaultAdminUserAndPassword();
                }
            }
        }


        private IConfiguration GetConfiguration(SupportedDatabaseTypes? dbType,
            IDictionary<string, string> defaultConfig = null)
        {
            var builder = new ConfigurationBuilder();
            if (defaultConfig != null)
            {
                builder.AddInMemoryCollection(defaultConfig);
            }

            if (dbType != null)
            {
                builder.AddInMemoryCollection(new Dictionary<string, string> { { "database:type", dbType.ToString() } });
            }

            return builder.Build();
        }
    }
}