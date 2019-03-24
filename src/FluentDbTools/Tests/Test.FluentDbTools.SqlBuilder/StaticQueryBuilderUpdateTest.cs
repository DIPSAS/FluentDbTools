using FluentDbTools.Extensions.SqlBuilder;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.SqlBuilder.Abstractions.Common;
using TestUtilities.FluentDbTools;
using FluentAssertions;
using Test.FluentDbTools.SqlBuilder.TestEntities;
using Xunit;

namespace Test.FluentDbTools.SqlBuilder
{
    public class StaticQueryBuilderUpdateTest
    {
        [Theory]
        [InlineData(SupportedDatabaseTypes.Oracle, null, "UPDATE Entity SET Description = :Description, Name = 'Arild' WHERE Id = :IdParam")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, "UPDATE Entity SET Description = @Description, Name = 'Arild' WHERE Id = @IdParam")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", "UPDATE {0}.Entity SET Description = :Description, Name = 'Arild' WHERE Id = :IdParam")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", "UPDATE {0}.Entity SET Description = @Description, Name = 'Arild' WHERE Id = @IdParam")]
        public void UpdateTest(SupportedDatabaseTypes databaseTypes, string schema, string expectedSql)
        {
            var dbConfig = OverrideConfig.CreateTestDbConfig(databaseTypes, schema);
            var useSchema = !string.IsNullOrEmpty(schema);
            expectedSql = string.Format(expectedSql, dbConfig.Schema);

            var builder = dbConfig.CreateSqlBuilder();
            var update = builder.Update<Entity>();

            var sql = update
                .OnSchema(setSchemaNameIfExpressionIsEvaluatedToTrue: () => useSchema)
                .Fields(x => x.FP(f => f.Description)
                              .FV(f => f.Name, "Arild"))
                .Where(x => x.WP(item => item.Id, "IdParam"))
                .Build();

            sql.Should().Be(expectedSql);
        }
        
        [Theory]
        [InlineData(SupportedDatabaseTypes.Oracle, null, "UPDATE EntityTable SET Description = :Description, Name = 'Arild' WHERE Id = :IdParam")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, "UPDATE EntityTable SET Description = @Description, Name = 'Arild' WHERE Id = @IdParam")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", "UPDATE {0}.EntityTable SET Description = :Description, Name = 'Arild' WHERE Id = :IdParam")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", "UPDATE {0}.EntityTable SET Description = @Description, Name = 'Arild' WHERE Id = @IdParam")]
        public void UpdateTest_WithTableNameSet(SupportedDatabaseTypes databaseTypes, string schema, string expectedSql)
        {
            const string tableName = "EntityTable";
            var dbConfig = OverrideConfig.CreateTestDbConfig(databaseTypes, schema);
            var useSchema = !string.IsNullOrEmpty(schema);
            expectedSql = string.Format(expectedSql, dbConfig.Schema);

            var builder = dbConfig.CreateSqlBuilder();
            var update = builder.Update<Entity>();

            var sql = update
                .OnTable(tableName)
                .OnSchema(setSchemaNameIfExpressionIsEvaluatedToTrue: () => useSchema)
                .Fields(x => x.FP(f => f.Description)
                    .FV(f => f.Name, "Arild"))
                .Where(x => x.WP(item => item.Id, "IdParam"))
                .Build();

            sql.Should().Be(expectedSql);
        }

        [Theory]
        [InlineData(SupportedDatabaseTypes.Oracle, null, "UPDATE Entity SET Description = :Description, Name = 'Arild' WHERE Id = :IdParam")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, "UPDATE Entity SET Description = @Description, Name = 'Arild' WHERE Id = @IdParam")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", "UPDATE Entity SET Description = :Description, Name = 'Arild' WHERE Id = :IdParam AND Name <> 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", "UPDATE Entity SET Description = @Description, Name = 'Arild' WHERE Id = @IdParam AND Name <> 'Arild'")]
        public void UpdateTestWithWhereIfTest(SupportedDatabaseTypes databaseTypes, string schema, string expectedSql)
        {
            var dbConfig = OverrideConfig.CreateTestDbConfig(databaseTypes, schema);
            var ifStatementResult = !string.IsNullOrEmpty(schema);
            expectedSql = string.Format(expectedSql, dbConfig.Schema);

            var builder = dbConfig.CreateSqlBuilder();
            var update = builder.Update<Entity>();

            var sql = update
                .Fields(x => x.FP(f => f.Description)
                    .FV(f => f.Name, "Arild"))
                .Where(x => x.WP(item => item.Id, "IdParam"))
                .WhereIf(x => x.WV(item => item.Name, "Arild", OP.DI), () => ifStatementResult)
                .Build();

            sql.Should().Be(expectedSql);
        }
    }
}