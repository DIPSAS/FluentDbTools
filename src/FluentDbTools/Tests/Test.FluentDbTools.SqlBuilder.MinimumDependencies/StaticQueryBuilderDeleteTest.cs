using FluentDbTools.Common.Abstractions;
using FluentDbTools.Extensions.SqlBuilder;
using FluentAssertions;
using FluentDbTools.Contracts;
using Test.FluentDbTools.SqlBuilder.MinimumDependencies.TestEntities;
using Xunit;

namespace Test.FluentDbTools.SqlBuilder.MinimumDependencies
{
    public class StaticQueryBuilderDeleteTest
    {
        [Theory]
        [InlineData(SupportedDatabaseTypes.Oracle, null, null, "DELETE FROM {1}Entity WHERE Id = :IdParam AND Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, null, "DELETE FROM {1}Entity WHERE Id = @IdParam AND Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", null, "DELETE FROM {0}.{1}Entity WHERE Id = :IdParam AND Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", null, "DELETE FROM {0}.{1}Entity WHERE Id = @IdParam AND Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, null, "EN", "DELETE FROM {1}Entity WHERE Id = :IdParam AND Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, "EN", "DELETE FROM {1}Entity WHERE Id = @IdParam AND Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", "EN", "DELETE FROM {0}.{1}Entity WHERE Id = :IdParam AND Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", "EN", "DELETE FROM {0}.{1}Entity WHERE Id = @IdParam AND Name = 'Arild'")]
        public void DeleteTest(SupportedDatabaseTypes databaseTypes, string schema, string schemaPrefixId, string expectedSql)
        {
            var useSchema = !string.IsNullOrEmpty(schema);

            var dbConfig = DbConfigDatabaseTargets.Create(databaseTypes, schema, schemaPrefixId: schemaPrefixId);
            expectedSql = string.Format(expectedSql, dbConfig.Schema, dbConfig.GetSchemaPrefixId() ?? string.Empty);

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
        [InlineData(SupportedDatabaseTypes.Oracle, null, null, "DELETE FROM {1}EntityTable WHERE Id = :IdParam AND Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, null, "DELETE FROM {1}EntityTable WHERE Id = @IdParam AND Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", null, "DELETE FROM {0}.{1}EntityTable WHERE Id = :IdParam AND Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", null, "DELETE FROM {0}.{1}EntityTable WHERE Id = @IdParam AND Name = 'Arild'")]
        public void DeleteTest_TableNameIsSet(SupportedDatabaseTypes databaseTypes, string schema, string schemaPrefixId, string expectedSql)
        {
            const string tableName = "EntityTable";
            var useSchema = !string.IsNullOrEmpty(schema);

            var dbConfig = DbConfigDatabaseTargets.Create(databaseTypes, schema, schemaPrefixId: schemaPrefixId);
            expectedSql = string.Format(expectedSql, dbConfig.Schema, dbConfig.GetSchemaPrefixId() ?? string.Empty);

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