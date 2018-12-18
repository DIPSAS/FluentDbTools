using FluentDbTools.Extensions.SqlBuilder;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Example.Config;
using FluentDbTools.SqlBuilder.Abstractions;
using FluentDbTools.SqlBuilder.Tests.TestEntities;
using FluentDbTools.TestUtilities;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FluentDbTools.SqlBuilder.Tests
{
    public class StaticQueryBuilderDeleteTest
    {
        [Theory]
        [InlineData(SupportedDatabaseTypes.Oracle, null, "DELETE FROM Entity WHERE Id = :IdParam AND Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, "DELETE FROM Entity WHERE Id = @IdParam AND Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", "DELETE FROM {0}.Entity WHERE Id = :IdParam AND Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", "DELETE FROM {0}.Entity WHERE Id = @IdParam AND Name = 'Arild'")]
        public void DeleteTest1(SupportedDatabaseTypes databaseTypes, string schema, string expectedSql)
        {
            var useSchema = !string.IsNullOrEmpty(schema);

            var dbConfig = OverrideConfig.CreateTestDbConfig(databaseTypes, schema);
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