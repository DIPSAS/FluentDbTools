using FluentDbTools.Common.Abstractions;
using FluentDbTools.SqlBuilder.Abstractions.Common;
using FluentAssertions;
using FluentDbTools.SqlBuilder;
using Test.FluentDbTools.SqlBuilder.MinimumDependencies.TestEntities;
using Xunit;

namespace Test.FluentDbTools.SqlBuilder.MinimumDependencies
{
    public class StaticQueryBuilderUpdateTest
    {
        [Theory]
        [InlineData(SupportedDatabaseTypes.Oracle, null, null, "UPDATE {1}Entity SET Description = :Description, Name = 'Arild' WHERE Id = :IdParam")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, null, "UPDATE {1}Entity SET Description = @Description, Name = 'Arild' WHERE Id = @IdParam")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", null, "UPDATE {0}.{1}Entity SET Description = :Description, Name = 'Arild' WHERE Id = :IdParam")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", null, "UPDATE {0}.{1}Entity SET Description = @Description, Name = 'Arild' WHERE Id = @IdParam")]
        [InlineData(SupportedDatabaseTypes.Oracle, null, "UP", "UPDATE {1}Entity SET Description = :Description, Name = 'Arild' WHERE Id = :IdParam")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, "UP", "UPDATE {1}Entity SET Description = @Description, Name = 'Arild' WHERE Id = @IdParam")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", "UP", "UPDATE {0}.{1}Entity SET Description = :Description, Name = 'Arild' WHERE Id = :IdParam")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", "UP", "UPDATE {0}.{1}Entity SET Description = @Description, Name = 'Arild' WHERE Id = @IdParam")]
        public void UpdateTest(SupportedDatabaseTypes databaseTypes, string schema, string schemaPrefixId, string expectedSql)
        {
            var dbConfig = SqlBuilderFactory.DbConfigSchemaTargets(schema, schemaPrefixId, databaseTypes);
            var useSchema = !string.IsNullOrEmpty(schema);
            expectedSql = string.Format(expectedSql, dbConfig.Schema, dbConfig.GetSchemaPrefixId());

            var builder = dbConfig.SqlBuilder();
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
        [InlineData(SupportedDatabaseTypes.Oracle, null, null, "UPDATE {1}EntityTable SET Description = :Description, Name = 'Arild' WHERE Id = :IdParam")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, null, "UPDATE {1}EntityTable SET Description = @Description, Name = 'Arild' WHERE Id = @IdParam")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", null, "UPDATE {0}.{1}EntityTable SET Description = :Description, Name = 'Arild' WHERE Id = :IdParam")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", null, "UPDATE {0}.{1}EntityTable SET Description = @Description, Name = 'Arild' WHERE Id = @IdParam")]
        [InlineData(SupportedDatabaseTypes.Oracle, null, "LONGPREFIX", "UPDATE {1}EntityTable SET Description = :Description, Name = 'Arild' WHERE Id = :IdParam")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, "LONGPREFIX", "UPDATE {1}EntityTable SET Description = @Description, Name = 'Arild' WHERE Id = @IdParam")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", "LONGPREFIX", "UPDATE {0}.{1}EntityTable SET Description = :Description, Name = 'Arild' WHERE Id = :IdParam")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", "LONGPREFIX", "UPDATE {0}.{1}EntityTable SET Description = @Description, Name = 'Arild' WHERE Id = @IdParam")]
        public void UpdateTest_WithTableNameSet(SupportedDatabaseTypes databaseTypes, string schema, string schemaPrefixId, string expectedSql)
        {
            const string tableName = "EntityTable";
            var dbConfig = SqlBuilderFactory.DbConfigSchemaTargets(schema, schemaPrefixId, databaseTypes);
            var useSchema = !string.IsNullOrEmpty(schema);
            expectedSql = string.Format(expectedSql, dbConfig.Schema, dbConfig.GetSchemaPrefixId());

            var builder = dbConfig.SqlBuilder();
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
        [InlineData(SupportedDatabaseTypes.Oracle, null, null, "UPDATE {1}Entity SET Description = :Description, Name = 'Arild' WHERE Id = :IdParam")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, null, "UPDATE {1}Entity SET Description = @Description, Name = 'Arild' WHERE Id = @IdParam")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", null, "UPDATE {0}.{1}Entity SET Description = :Description, Name = 'Arild' WHERE Id = :IdParam AND Name <> 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", null, "UPDATE {0}.{1}Entity SET Description = @Description, Name = 'Arild' WHERE Id = @IdParam AND Name <> 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, null, "UPD", "UPDATE {1}Entity SET Description = :Description, Name = 'Arild' WHERE Id = :IdParam")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, "UPD", "UPDATE {1}Entity SET Description = @Description, Name = 'Arild' WHERE Id = @IdParam")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", "UPD", "UPDATE {0}.{1}Entity SET Description = :Description, Name = 'Arild' WHERE Id = :IdParam AND Name <> 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", "UPD", "UPDATE {0}.{1}Entity SET Description = @Description, Name = 'Arild' WHERE Id = @IdParam AND Name <> 'Arild'")]
        public void UpdateTestWithWhereIfTest(SupportedDatabaseTypes databaseTypes, string schema, string schemaPrefixId, string expectedSql)
        {
            var dbConfig = SqlBuilderFactory.DbConfigSchemaTargets(schema, schemaPrefixId, databaseTypes);
            var ifStatementResult = !string.IsNullOrEmpty(schema);
            expectedSql = string.Format(expectedSql, dbConfig.Schema, dbConfig.GetSchemaPrefixId());

            var builder = dbConfig.SqlBuilder();
            var update = builder.Update<Entity>();

            var sql = update
                .OnSchema(schema, () => schema != null)
                .Fields(x => x.FP(f => f.Description)
                    .FV(f => f.Name, "Arild"))
                .Where(x => x.WP(item => item.Id, "IdParam"))
                .WhereIf(x => x.WV(item => item.Name, "Arild", OP.DI), () => ifStatementResult)
                .Build();

            sql.Should().Be(expectedSql);
        }
    }
}