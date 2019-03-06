using FluentDbTools.Extensions.SqlBuilder;
using FluentDbTools.Common.Abstractions;
using TestUtilities.FluentDbTools;
using FluentAssertions;
using FTest.FluentDbTools.SqlBuilderTestEntities;
using Xunit;

namespace Test.FluentDbTools.SqlBuilder
{
    public class StaticQueryBuilderDeleteTest
    {
        [Theory]
        [InlineData(SupportedDatabaseTypes.Oracle, null, "DELETE FROM Entity WHERE Id = :IdParam AND Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, "DELETE FROM Entity WHERE Id = @IdParam AND Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", "DELETE FROM {0}.Entity WHERE Id = :IdParam AND Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", "DELETE FROM {0}.Entity WHERE Id = @IdParam AND Name = 'Arild'")]
        public void DeleteTest(SupportedDatabaseTypes databaseTypes, string schema, string expectedSql)
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
        
        [Theory]
        [InlineData(SupportedDatabaseTypes.Oracle, null, "DELETE FROM EntityTable WHERE Id = :IdParam AND Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, "DELETE FROM EntityTable WHERE Id = @IdParam AND Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", "DELETE FROM {0}.EntityTable WHERE Id = :IdParam AND Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", "DELETE FROM {0}.EntityTable WHERE Id = @IdParam AND Name = 'Arild'")]
        public void DeleteTest_TableNameIsSet(SupportedDatabaseTypes databaseTypes, string schema, string expectedSql)
        {
            const string tableName = "EntityTable";
            var useSchema = !string.IsNullOrEmpty(schema);

            var dbConfig = OverrideConfig.CreateTestDbConfig(databaseTypes, schema);
            expectedSql = string.Format(expectedSql, dbConfig.Schema);

            var builder = dbConfig.CreateSqlBuilder();
            var delete = builder.Delete<Entity>();

            var sql = delete
                .OnTable(tableName)
                .OnSchema(setSchemaNameIfExpressionIsEvaluatedToTrue: () => useSchema)
                .Where(x => x.WP(item => item.Id, "IdParam"))
                .Where(x => x.WV(item => item.Name, "Arild"))
                .Build();

            sql.Should().Be(expectedSql);
        }
    }
}