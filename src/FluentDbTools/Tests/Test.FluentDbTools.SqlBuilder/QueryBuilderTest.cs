using FluentAssertions;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.SqlBuilder;
using TestUtilities.FluentDbTools;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Test.FluentDbTools.SqlBuilder
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
                var dbConfig = scope.ServiceProvider.GetService<IDbConfigSchemaTargets>();
                
                dbConfig.GetParameterPrefix().Should().Be(prefix);
            }
        }
    }
}