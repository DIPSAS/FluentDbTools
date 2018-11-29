using DIPS.FluentDbTools.Common.Abstractions;
using DIPS.FluentDbTools.SqlBuilder.Abstractions;
using DIPS.FluentDbTools.TestUtilities;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DIPS.FluentDbTools.SqlBuilder.Tests
{
    public class QueryBuilderTest
    {
        [Theory]
        [InlineData(SupportedDatabaseTypes.Oracle, ":")]
        [InlineData(SupportedDatabaseTypes.Postgres, "@")]
        public void GetParameterPrefix_ShouldHaveExpectedValue(SupportedDatabaseTypes databaseTypes, string prefix)
        {
            using (var scope = TestServiceProvider.GetDatabaseExampleServiceProvider(databaseTypes).CreateScope())
            {
                var dbConfig = scope.ServiceProvider.GetService<IDbConfig>();
                
                dbConfig.GetParameterPrefix().Should().Be(prefix);
            }
        }
    }
}