using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Contracts;
using FluentDbTools.Extensions.DbProvider;
using FluentDbTools.Extensions.MSDependencyInjection;
using FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

// ReSharper disable PossibleNullReferenceException

namespace Test.FluentDbTools.DbProvider
{
    public class DbConfigAdminExtensionTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ValidateDatabaseAdminValues_BothValidAdminUserAndAdminPassword_ShouldNotFail(bool throwIfFail)
        {
            var provider = new ServiceCollection()
                .AddSingleton(GetConfiguration(new Dictionary<string, string> {
                    {
                        "database:type", "Oracle"
                    },
                    {
                        "database:datasource", "datasource"
                    },
                    {
                        "database:adminUser", "xxx"
                    },
                    {
                        "database:adminPassword", "xxx"
                    } }))
                .AddDefaultDbConfig()
                .BuildServiceProvider();

            var dbConfig = provider.GetDbConfig();

            if (throwIfFail)
            {
                Action validateWithThrow = () => dbConfig.ValidateAdminValues();
                validateWithThrow.Should().NotThrow<AggregateException>();
                return;
            }

            var invalidAdminValues = dbConfig.ValidateDatabaseAdminValues(throwIfFail: false);

            var useridStr = dbConfig.DbType == SupportedDatabaseTypes.Postgres ? "username" : "user id";

            dbConfig.AdminConnectionString.ContainsIgnoreCase($"{useridStr}=;").Should().BeFalse();
            dbConfig.AdminConnectionString.ContainsIgnoreCase("password=;").Should().BeFalse();

            invalidAdminValues.Should().BeEmpty();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ValidateDatabaseAdminValues_NoConfiguration_ShouldValidateFailOnBothAdminUserAndAdminPassword(bool throwIfFail)
        {
            lock (TestCollectionFixtures.LockObj)
            {
                try
                {
                    var provider = new ServiceCollection()
                        .AddSingleton(GetConfiguration())
                        .AddDefaultDbConfig()
                        .BuildServiceProvider();

                    DefaultDbConfigValues.WithEmptyAdminUserAndPassword();
                    var dbConfig = provider.GetRequiredService<IConfiguration>().CreateDbConfig();
                    var adminConnectionString = dbConfig.AdminConnectionString;
                    if (throwIfFail)
                    {
                        Action validateWithThrow = () => dbConfig.ValidateAdminValues();
                        var exception =  validateWithThrow.Should().Throw<AggregateException>().Subject.FirstOrDefault();
                        exception.InnerExceptions.Should().HaveCount(2);
                        exception.InnerExceptions.All(x => x is ArgumentNullException).Should().BeTrue();
                        exception.IsInvalidDatabaseAdminException().Should().BeTrue();
                        return;
                    }

                    var invalidAdminValues = dbConfig.ValidateDatabaseAdminValues(throwIfFail: false);
                    DefaultDbConfigValues.WithLibraryDefaultAdminUserAndPassword();

                    invalidAdminValues.Should().HaveCount(2);

                    var adminUser = invalidAdminValues.FirstOrDefault(x => x.IsAdminUser());
                    var adminPassword = invalidAdminValues.FirstOrDefault(x => x.IsAdminPassword());
                    adminUser.Should().NotBeNull();
                    adminPassword.Should().NotBeNull();

                    adminUser.ConfigurationKeys.Should()
                        .Contain("'Database.Upgrade.User'")
                        .And
                        .Contain("'Database.AdminUser'");

                    adminPassword.ConfigurationKeys.Should()
                        .Contain("'Database.Upgrade.Password'")
                        .And
                        .Contain("'Database.AdminPassword'");

                    adminUser.Value.Should().BeNullOrEmpty();
                    adminPassword.Value.Should().BeNullOrEmpty();

                    adminUser.GeneratedArgumentException.GetType().Should().Be<ArgumentNullException>();
                    adminPassword.GeneratedArgumentException.GetType().Should().Be<ArgumentNullException>();

                    adminUser.GeneratedArgumentException.ParamName.Should().Be("Database.Upgrade.User");
                    adminPassword.GeneratedArgumentException.ParamName.Should().Be("Database.Upgrade.Password");

                    var useridStr = dbConfig.DbType == SupportedDatabaseTypes.Postgres ? "username" : "user id";

                    adminConnectionString.ContainsIgnoreCase($"{useridStr}=;").Should().BeTrue();
                    adminConnectionString.ContainsIgnoreCase("password=;").Should().BeTrue();

                    adminUser.GeneratedArgumentException.IsInvalidDatabaseAdminException().Should().BeTrue();
                    adminPassword.GeneratedArgumentException.IsInvalidDatabaseAdminException().Should().BeTrue();

                }
                finally
                {
                    DefaultDbConfigValues.WithLibraryDefaultAdminUserAndPassword();
                }
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData(null, null, true)]
        [InlineData(DefaultDbConfigValuesStatic.DefaultPossibleInvalidDbUpgradeUser)]
        [InlineData(DefaultDbConfigValuesStatic.DefaultPossibleInvalidDbUpgradeUser, "database:upgrade:user")]
        [InlineData(DefaultDbConfigValuesStatic.DefaultPossibleInvalidDbUpgradeUser, "database:upgrade:user", true)]
        public void ValidateDatabaseAdminValues_NullOrInvalidAdminUserButAdminPasswordSpecified_ShouldValidateFailOnAdminUserOnly(string adminUserValue, string adminUserKey = null, bool throwIfFail = false)
        {
            lock (TestCollectionFixtures.LockObj)
            {
                try
                {
                    DefaultDbConfigValues.WithEmptyAdminUserAndPassword();
                    adminUserKey ??= "database:adminUser";
                    var provider = new ServiceCollection()
                        .AddSingleton(GetConfiguration(new Dictionary<string, string>
                        {
                            { "database:type", "Oracle" },
                            { adminUserKey, adminUserValue },
                            { "database:adminPassword", "xxx" }
                        }))
                        .AddDefaultDbConfig()
                        .BuildServiceProvider();

                    var dbConfig = provider.GetDbConfig();
                    var adminConnectionString = dbConfig.AdminConnectionString;

                    Func<InvalidAdminValue[]> validate = () => dbConfig.ValidateDatabaseAdminValues(throwIfFail: throwIfFail);

                    if (throwIfFail)
                    {
                        ArgumentException exception;
                        if (string.IsNullOrEmpty(adminUserValue))
                        {
                            exception = validate.Should().Throw<ArgumentNullException>().Subject.FirstOrDefault();
                            exception.ParamName.Should().Be("Database.Upgrade.User");
                            exception.Message.Should().Be($"All required configuration parameters ['{exception.ParamName}', 'Database.AdminUser'] is not set. (Parameter '{exception.ParamName}')");
                        }
                        else
                        {
                            exception = validate.Should().Throw<ArgumentException>().Subject.FirstOrDefault();
                            exception.Message.Should().Be($"Required configuration parameter {adminUserKey.ConvertConfigKeyToParamNameStyle()} has an invalid value '{adminUserValue}'.");
                        }

                        exception.IsInvalidDatabaseAdminException().Should().BeTrue();
                        return;
                    }

                    var invalidAdminValues = validate();

                    DefaultDbConfigValues.WithLibraryDefaultAdminUserAndPassword();

                    invalidAdminValues.Should().HaveCount(1);

                    var adminUser = invalidAdminValues.FirstOrDefault(x => x.IsAdminUser());
                    var adminPassword = invalidAdminValues.FirstOrDefault(x => x.IsAdminPassword());
                    adminUser.Should().NotBeNull();
                    adminPassword.Should().BeNull();

                    var argException = adminUser?.GeneratedArgumentException;
                    var confKeys = adminUser.ConfigurationKeys;
                    var value = adminUser.Value;
                    var configuredValue = adminUserValue;
                    var adminKey = adminUserKey.ConvertConfigKeyToParamNameStyle();
                    if (string.IsNullOrEmpty(adminUserValue))
                    {
                        confKeys.Should()
                            .Contain("'Database.Upgrade.User'")
                            .And
                            .Contain("'Database.AdminUser'");

                        value.Should().BeNullOrEmpty();
                        argException.GetType().Should().Be<ArgumentNullException>();
                        argException.ParamName.Should().Be(confKeys.FirstOrDefault().Replace("'", ""));

                        argException.Message.Should().Be($"All required configuration parameters ['{argException.ParamName}', {confKeys.LastOrDefault()}] is not set. (Parameter '{argException.ParamName}')");
                    }
                    else
                    {
                        confKeys.Should().HaveCount(1);
                        confKeys.Should().Contain(adminKey);

                        value.Should().Be(configuredValue);
                        argException.GetType().Should().Be<ArgumentException>();
                        argException.Message.Should().Be($"Required configuration parameter {adminKey} has an invalid value '{configuredValue}'.");
                    }

                    argException.IsInvalidDatabaseAdminException().Should().BeTrue();

                    var useridStr = dbConfig.DbType == SupportedDatabaseTypes.Postgres ? "username" : "user id";

                    adminConnectionString.ContainsIgnoreCase($"{useridStr}=;").Should().BeTrue();
                    adminConnectionString.ContainsIgnoreCase("password=;").Should().BeTrue();

                }
                finally
                {
                    DefaultDbConfigValues.WithLibraryDefaultAdminUserAndPassword();
                }
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData(null, null, true)]
        [InlineData(DefaultDbConfigValuesStatic.DefaultPossibleInvalidDbUpgradePassword)]
        [InlineData(DefaultDbConfigValuesStatic.DefaultPossibleInvalidDbUpgradePassword, "database:upgrade:password")]
        [InlineData(DefaultDbConfigValuesStatic.DefaultPossibleInvalidDbUpgradePassword, "database:upgrade:password", true)]
        public void ValidateDatabaseAdminValues_NullOrInvalidAdminPasswordButAdminUserSpecified_ShouldValidateFailOnAdminPasswordOnly(string adminPasswordValue, string adminPasswordKey = null, bool throwIfFail = false)
        {
            lock (TestCollectionFixtures.LockObj)
            {
                try
                {
                    DefaultDbConfigValues.WithEmptyAdminUserAndPassword();
                    adminPasswordKey ??= "database:adminPassword";

                    var provider = new ServiceCollection()
                        .AddSingleton(GetConfiguration(new Dictionary<string, string>
                        {
                            { "database:adminUser", "xxx" },
                            { adminPasswordKey, adminPasswordValue }
                        }))
                        .AddDefaultDbConfig()
                        .BuildServiceProvider();

                    Func<InvalidAdminValue[]> validate = () => provider.GetRequiredService<IConfiguration>().CreateDbConfig().ValidateDatabaseAdminValues(throwIfFail: throwIfFail);

                    if (throwIfFail)
                    {
                        ArgumentException exception;

                        if (string.IsNullOrEmpty(adminPasswordValue))
                        {
                            exception = validate.Should().Throw<ArgumentNullException>().Subject.FirstOrDefault();
                            exception.ParamName.Should().Be("Database.Upgrade.Password");
                            exception.Message.Should().Be($"All required configuration parameters ['{exception.ParamName}', 'Database.AdminPassword'] is not set. (Parameter '{exception.ParamName}')");
                        }
                        else
                        {
                            exception = validate.Should().Throw<ArgumentException>().Subject.FirstOrDefault();
                            exception.Message.Should().Be($"Required configuration parameter {adminPasswordKey.ConvertConfigKeyToParamNameStyle()} has an invalid value '{adminPasswordValue}'.");
                        }

                        exception.IsInvalidDatabaseAdminException().Should().BeTrue();
                        return;
                    }

                    var invalidAdminValues = provider.ValidateDatabaseAdminValues(throwIfFail: false);
                    invalidAdminValues.Should().HaveCount(1);

                    var adminUser = invalidAdminValues.FirstOrDefault(x => x.IsAdminUser());
                    var adminPassword = invalidAdminValues.FirstOrDefault(x => x.IsAdminPassword());
                    adminUser.Should().BeNull();
                    adminPassword.Should().NotBeNull();

                    var argException = adminPassword?.GeneratedArgumentException;
                    var confKeys = adminPassword.ConfigurationKeys;
                    var value = adminPassword.Value;
                    var configuredValue = adminPasswordValue;
                    var adminKey = adminPasswordKey.ConvertConfigKeyToParamNameStyle();

                    if (string.IsNullOrEmpty(adminPasswordValue))
                    {
                        adminPassword.ConfigurationKeys.Should()
                            .Contain("'Database.Upgrade.Password'")
                            .And
                            .Contain("'Database.AdminPassword'");

                        value.Should().BeNullOrEmpty();
                        argException.GetType().Should().Be<ArgumentNullException>();
                        argException.ParamName.Should().Be(confKeys.FirstOrDefault().Replace("'", ""));

                        argException.Message.Should().Be($"All required configuration parameters ['{argException.ParamName}', {confKeys.LastOrDefault()}] is not set. (Parameter '{argException.ParamName}')");
                    }
                    else
                    {
                        confKeys.Should().HaveCount(1);
                        confKeys.Should().Contain(adminKey);

                        value.Should().Be(configuredValue);
                        argException.GetType().Should().Be<ArgumentException>();
                        argException.Message.Should().Be($"Required configuration parameter {adminKey} has an invalid value '{configuredValue}'.");
                    }
                    argException.IsInvalidDatabaseAdminException().Should().BeTrue();

                }
                finally
                {
                    DefaultDbConfigValues.WithLibraryDefaultAdminUserAndPassword();
                }
            }
        }




        private IConfiguration GetConfiguration(IDictionary<string, string> defaultConfig = null)
        {
            var builder = new ConfigurationBuilder();
            if (defaultConfig != null)
            {
                //builder.AddInMemoryCollection(new Dictionary<string,string>(defaultConfig, StringComparer.OrdinalIgnoreCase));
                builder.AddInMemoryCollection(defaultConfig);
            }

            return builder.Build();
        }

    }
}