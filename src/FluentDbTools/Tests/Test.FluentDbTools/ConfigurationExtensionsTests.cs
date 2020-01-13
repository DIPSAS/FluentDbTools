using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection.Emit;
using FluentAssertions;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Extensions.MSDependencyInjection;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Test.FluentDbTools
{
    public class ConfigurationExtensionsTests
    {
        [Theory]
        [InlineData(StringComparison.OrdinalIgnoreCase)]
        [InlineData(StringComparison.InvariantCultureIgnoreCase)]
        [InlineData(StringComparison.CurrentCultureIgnoreCase)]
        [InlineData(StringComparison.OrdinalIgnoreCase, "en-GB")]
        [InlineData(StringComparison.InvariantCultureIgnoreCase, "en-GB")]
        [InlineData(StringComparison.CurrentCultureIgnoreCase,"en-GB")]
        public void GetConfigValue_ConfigurationExtensions_ShouldBeOk(StringComparison stringComparison, string cultureName = "nb-NO")
        {
            var current = CultureInfo.CurrentCulture;
            var currentIgnoreCaseStringComparison = StringExtensions.CurrentIgnoreCaseStringComparison;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(cultureName ?? "nb-NO");
                StringExtensions.CurrentIgnoreCaseStringComparison = stringComparison;

                var config = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>(StringComparer.FromComparison(stringComparison)) { { "database:paramKey", "val_" } })
                    .AddInMemoryCollection(new Dictionary<string, string>(StringComparer.FromComparison(stringComparison)) { { "Database:ParamKey", "val__" } })
                    .AddInMemoryCollection(new Dictionary<string, string>(StringComparer.FromComparison(stringComparison)) { { "DATABASE:PARAMKEY", "val" } })
                    .Build();

                config.GetConfigValue("database:paramKey").Should().Be("val");

            }
            finally
            {
                CultureInfo.CurrentCulture = current;
                StringExtensions.CurrentIgnoreCaseStringComparison = currentIgnoreCaseStringComparison;
            }
        }
    }
}