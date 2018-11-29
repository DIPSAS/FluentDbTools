using DIPS.Extensions.FluentDbTools.SqlBuilder;
using DIPS.FluentDbTools.Common.Abstractions;
using DIPS.FluentDbTools.Example.Config;
using DIPS.FluentDbTools.SqlBuilder.Abstractions;
using DIPS.FluentDbTools.SqlBuilder.Tests.TestEntities;
using DIPS.FluentDbTools.TestUtilities;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DIPS.FluentDbTools.SqlBuilder.Tests
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