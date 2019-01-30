using FluentDbTools.Extensions.SqlBuilder;
using FluentDbTools.Common.Abstractions;
using TestUtilities.FluentDbTools;
using FluentAssertions;
using FTest.FluentDbTools.SqlBuilderTestEntities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Test.FluentDbTools.SqlBuilder
{
    public class QueryBuilderDeleteTest
    {      
        [Theory]
        [InlineData(SupportedDatabaseTypes.Oracle, false, "DELETE FROM Entity WHERE Id = :IdParam AND Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, false, "DELETE FROM Entity WHERE Id = @IdParam AND Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, true, "DELETE FROM {0}.Entity WHERE Id = :IdParam AND Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, true, "DELETE FROM {0}.Entity WHERE Id = @IdParam AND Name = 'Arild'")]
        public void DeleteTest1(SupportedDatabaseTypes databaseTypes, bool useSchema, string expectedSql)
        {
            using (var scope = TestServiceProvider.GetDatabaseExampleServiceProvider(databaseTypes).CreateScope())
            {
                var dbConfig = scope.ServiceProvider.GetService<IDbConfig>();
                expectedSql = string.Format(expectedSql, dbConfig.Schema);
                
                var builder = dbConfig.CreateSqlBuilder();
                var delete = builder.Delete<Entity>();

                var sql = delete
                    .OnSchema(setSchemaNameIfExpressionIsEvaluatedToTrue: () => useSchema)
                    .Where(x => x.WP(item => item.Id, "IdParam"))
                    .Where(x => x.WV(item => item.Name, "Arild"))
                    .Build();

                sql.Should().Be(expectedSql);
            }
        }
    }
}