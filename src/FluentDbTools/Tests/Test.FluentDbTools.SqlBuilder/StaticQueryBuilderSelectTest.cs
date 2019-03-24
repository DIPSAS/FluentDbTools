using System.Collections.Generic;
using FluentDbTools.Extensions.SqlBuilder;
using FluentDbTools.Common.Abstractions;
using TestUtilities.FluentDbTools;
using FluentAssertions;
using Test.FluentDbTools.SqlBuilder.TestEntities;
using Xunit;

namespace Test.FluentDbTools.SqlBuilder
{
    public class StaticQueryBuilderSelectTest
    {
        [Theory]
        [InlineData(SupportedDatabaseTypes.Oracle, null, "SELECT e.Name, e.Description FROM Entity e WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, "SELECT e.Name, e.Description FROM Entity e WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", "SELECT e.Name, e.Description FROM {0}.Entity e WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", "SELECT e.Name, e.Description FROM {0}.Entity e WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        public void SelectTestOneTableOnly(SupportedDatabaseTypes databaseTypes, string schema, string expectedSql)
        {
            var dbConfig = OverrideConfig.CreateTestDbConfig(databaseTypes, schema);
            var useSchema = !string.IsNullOrEmpty(schema);
            expectedSql = string.Format(expectedSql, dbConfig.Schema);

            var builder = dbConfig.CreateSqlBuilder();
            var select = builder.Select();
            var sqls = new List<string>
                {
                    select
                        .OnSchema(setSchemaNameIfExpressionIsEvaluatedToTrue: () => useSchema)
                        .Fields<Entity>(x => x.F(item => item.Name))
                        .Fields<Entity>(x => x.F(item => item.Description))
                        .Where<Entity>(x => x.WP(item => item.Id, "IdParam"))
                        .Where<Entity>(x => x.WV(item => item.Name, "Arild"))
                        .Build(),
                    select
                        .OnSchema(setSchemaNameIfExpressionIsEvaluatedToTrue: () => useSchema)
                        .From<Entity>()
                        .Fields<Entity>(x => x.F(item => item.Name))
                        .Fields<Entity>(x => x.F(item => item.Description))
                        .Where<Entity>(x => x.WP(item => item.Id, "IdParam"))
                        .Where<Entity>(x => x.WV(item => item.Name, "Arild"))
                        .Build()
                };
            foreach (var sql in sqls)
            {
                sql.Should().Be(expectedSql);
            }
        }
        
        [Theory]
        [InlineData(SupportedDatabaseTypes.Oracle, null, "SELECT e.Name, e.Description FROM EntityTable e WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, "SELECT e.Name, e.Description FROM EntityTable e WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", "SELECT e.Name, e.Description FROM {0}.EntityTable e WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", "SELECT e.Name, e.Description FROM {0}.EntityTable e WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        public void SelectTestOneTableOnly_WithTableNameSet(SupportedDatabaseTypes databaseTypes, string schema, string expectedSql)
        {
            const string tableName = "EntityTable";
            var dbConfig = OverrideConfig.CreateTestDbConfig(databaseTypes, schema);
            var useSchema = !string.IsNullOrEmpty(schema);
            expectedSql = string.Format(expectedSql, dbConfig.Schema);

            var builder = dbConfig.CreateSqlBuilder();
            var select = builder.Select();
            var sqls = new List<string>
                {
                    select
                        .OnSchema(setSchemaNameIfExpressionIsEvaluatedToTrue: () => useSchema)
                        .Fields<Entity>(x => x.F(item => item.Name), tableName)
                        .Fields<Entity>(x => x.F(item => item.Description), tableName)
                        .Where<Entity>(x => x.WP(item => item.Id, "IdParam"))
                        .Where<Entity>(x => x.WV(item => item.Name, "Arild"))
                        .Build(),
                    select
                        .OnSchema(setSchemaNameIfExpressionIsEvaluatedToTrue: () => useSchema)
                        .From<Entity>(tableName)
                        .Fields<Entity>(x => x.F(item => item.Name))
                        .Fields<Entity>(x => x.F(item => item.Description))
                        .Where<Entity>(x => x.WP(item => item.Id, "IdParam"))
                        .Where<Entity>(x => x.WV(item => item.Name, "Arild"))
                        .Build()
                };
            foreach (var sql in sqls)
            {
                sql.Should().Be(expectedSql);
            }
        }

        [Theory]
        [InlineData(SupportedDatabaseTypes.Oracle, null, "SELECT COUNT(e.Name, e.Description) FROM Entity e WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, "SELECT COUNT(e.Name, e.Description) FROM Entity e WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", "SELECT COUNT(e.Name, e.Description) FROM {0}.Entity e WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", "SELECT COUNT(e.Name, e.Description) FROM {0}.Entity e WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        public void SelectCountTestOneTableOnly(SupportedDatabaseTypes databaseTypes, string schema, string expectedSql)
        {
            var dbConfig = OverrideConfig.CreateTestDbConfig(databaseTypes, schema);
            var useSchema = !string.IsNullOrEmpty(schema);
            expectedSql = string.Format(expectedSql, dbConfig.Schema);

            var builder = dbConfig.CreateSqlBuilder();
            var select = builder.Select();
            var sqls = new List<string>
                {
                    select
                        .Count()
                        .OnSchema(setSchemaNameIfExpressionIsEvaluatedToTrue: () => useSchema)
                        .Fields<Entity>(x => x.F(item => item.Name))
                        .Fields<Entity>(x => x.F(item => item.Description))
                        .Where<Entity>(x => x.WP(item => item.Id, "IdParam"))
                        .Where<Entity>(x => x.WV(item => item.Name, "Arild"))
                        .Build(),
                    select
                        .Count()
                        .OnSchema(setSchemaNameIfExpressionIsEvaluatedToTrue: () => useSchema)
                        .From<Entity>()
                        .Fields<Entity>(x => x.F(item => item.Name))
                        .Fields<Entity>(x => x.F(item => item.Description))
                        .Where<Entity>(x => x.WP(item => item.Id, "IdParam"))
                        .Where<Entity>(x => x.WV(item => item.Name, "Arild"))
                        .Build()
                };
            foreach (var sql in sqls)
            {
                sql.Should().Be(expectedSql);
            }
        }

        [Theory]
        [InlineData(SupportedDatabaseTypes.Oracle, null, "SELECT ce.Description, ce.Relation, e.Name, e.Description FROM Entity e INNER JOIN ChildEntity ce ON e.ChildEntityId = ce.Id LEFT OUTER JOIN ChildChildEntity cce ON ce.ChildChildEntityId = cce.Id WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, "SELECT ce.Description, ce.Relation, e.Name, e.Description FROM Entity e INNER JOIN ChildEntity ce ON e.ChildEntityId = ce.Id LEFT OUTER JOIN ChildChildEntity cce ON ce.ChildChildEntityId = cce.Id WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", "SELECT ce.Description, ce.Relation, e.Name, e.Description FROM {0}.Entity e INNER JOIN {0}.ChildEntity ce ON e.ChildEntityId = ce.Id LEFT OUTER JOIN {0}.ChildChildEntity cce ON ce.ChildChildEntityId = cce.Id WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", "SELECT ce.Description, ce.Relation, e.Name, e.Description FROM {0}.Entity e INNER JOIN {0}.ChildEntity ce ON e.ChildEntityId = ce.Id LEFT OUTER JOIN {0}.ChildChildEntity cce ON ce.ChildChildEntityId = cce.Id WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        public void SelectTestWithJoins(SupportedDatabaseTypes databaseTypes, string schema, string expectedSql)
        {
            var dbConfig = OverrideConfig.CreateTestDbConfig(databaseTypes, schema);
            var useSchema = !string.IsNullOrEmpty(schema);
            expectedSql = string.Format(expectedSql, dbConfig.Schema);
            
            var builder = dbConfig.CreateSqlBuilder();
            var select = builder.Select();

            var sqls = new List<string>
                {
                    @select
                        .OnSchema(setSchemaNameIfExpressionIsEvaluatedToTrue:() => useSchema)
                        .From<Entity>()
                        .InnerJoin<Entity, ChildEntity>()
                        .LeftOuterJoin<ChildEntity, ChildChildEntity>()
                        .Fields<ChildEntity>(x => x.F(item => item.Description))
                        .Fields<ChildEntity>(x => x.F(item => item.Relation))
                        .Fields<Entity>(x => x.F(item => item.Name))
                        .Fields<Entity>(x => x.F(item => item.Description))
                        .Where<Entity>(x => x.WP(item => item.Id, "IdParam"))
                        .Where<Entity>(x => x.WV(item => item.Name, "Arild"))
                        .Build(),
                    @select
                        .OnSchema(setSchemaNameIfExpressionIsEvaluatedToTrue:() => useSchema)
                        .From<Entity>()
                        .Fields<ChildEntity>(x => x.F(item => item.Description))
                        .Fields<ChildEntity>(x => x.F(item => item.Relation))
                        .Fields<Entity>(x => x.F(item => item.Name))
                        .Fields<Entity>(x => x.F(item => item.Description))
                        .InnerJoin<Entity, ChildEntity>()
                        .LeftOuterJoin<ChildEntity, ChildChildEntity>()
                        .Where<Entity>(x => x.WP(item => item.Id, "IdParam"))
                        .Where<Entity>(x => x.WV(item => item.Name, "Arild"))
                        .Build(),
                    @select
                        .OnSchema(setSchemaNameIfExpressionIsEvaluatedToTrue:() => useSchema)
                        .InnerJoin<Entity, ChildEntity>()
                        .LeftOuterJoin<ChildEntity, ChildChildEntity>()
                        .Fields<ChildEntity>(x => x.F(item => item.Description))
                        .Fields<ChildEntity>(x => x.F(item => item.Relation))
                        .Fields<Entity>(x => x.F(item => item.Name))
                        .Fields<Entity>(x => x.F(item => item.Description))
                        .Where<Entity>(x => x.WP(item => item.Id, "IdParam"))
                        .Where<Entity>(x => x.WV(item => item.Name, "Arild"))
                        .Build(),
                };
            foreach (var sql in sqls)
            {
                sql.Should().Be(expectedSql);
            }
        }

        [Theory]
        [InlineData(SupportedDatabaseTypes.Oracle, null, "SELECT e.Name, e.Description, ce.Description, ce.Relation FROM Entity e INNER JOIN ChildEntity ce ON e.ChildEntityId = ce.Id LEFT OUTER JOIN ChildChildEntity cce ON ce.ChildChildEntityId = cce.Id WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, "SELECT e.Name, e.Description, ce.Description, ce.Relation FROM Entity e INNER JOIN ChildEntity ce ON e.ChildEntityId = ce.Id LEFT OUTER JOIN ChildChildEntity cce ON ce.ChildChildEntityId = cce.Id WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, "oracle_schema", "SELECT e.Name, e.Description, ce.Description, ce.Relation FROM {0}.Entity e INNER JOIN {0}.ChildEntity ce ON e.ChildEntityId = ce.Id LEFT OUTER JOIN {0}.ChildChildEntity cce ON ce.ChildChildEntityId = cce.Id WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, "postgres_schema", "SELECT e.Name, e.Description, ce.Description, ce.Relation FROM {0}.Entity e INNER JOIN {0}.ChildEntity ce ON e.ChildEntityId = ce.Id LEFT OUTER JOIN {0}.ChildChildEntity cce ON ce.ChildChildEntityId = cce.Id WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        public void SelectTest2WithJoins(SupportedDatabaseTypes databaseTypes, string schema, string expectedSql)
        {
            var dbConfig = OverrideConfig.CreateTestDbConfig(databaseTypes, schema);
            var useSchema = !string.IsNullOrEmpty(schema);
            expectedSql = string.Format(expectedSql, dbConfig.Schema);

            var builder = dbConfig.CreateSqlBuilder();
            var select = builder.Select();

            var sqls = new List<string>
                {
                    @select
                        .OnSchema(setSchemaNameIfExpressionIsEvaluatedToTrue:() => useSchema)
                        .From<Entity>()
                        .InnerJoin<Entity, ChildEntity>()
                        .LeftOuterJoin<ChildEntity, ChildChildEntity>()
                        .Fields<Entity>(x => x.F(item => item.Name))
                        .Fields<Entity>(x => x.F(item => item.Description))
                        .Fields<ChildEntity>(x => x.F(item => item.Description))
                        .Fields<ChildEntity>(x => x.F(item => item.Relation))
                        .Where<Entity>(x => x.WP(item => item.Id, "IdParam"))
                        .Where<Entity>(x => x.WV(item => item.Name, "Arild"))
                        .Build(),
                    @select
                        .OnSchema(setSchemaNameIfExpressionIsEvaluatedToTrue:() => useSchema)
                        .From<Entity>()
                        .Fields<Entity>(x => x.F(item => item.Name))
                        .Fields<Entity>(x => x.F(item => item.Description))
                        .Fields<ChildEntity>(x => x.F(item => item.Description))
                        .Fields<ChildEntity>(x => x.F(item => item.Relation))
                        .InnerJoin<Entity, ChildEntity>()
                        .LeftOuterJoin<ChildEntity, ChildChildEntity>()
                        .Where<Entity>(x => x.WP(item => item.Id, "IdParam"))
                        .Where<Entity>(x => x.WV(item => item.Name, "Arild"))
                        .Build(),
                    @select
                        .OnSchema(setSchemaNameIfExpressionIsEvaluatedToTrue:() => useSchema)
                        .InnerJoin<Entity, ChildEntity>()
                        .LeftOuterJoin<ChildEntity, ChildChildEntity>()
                        .Fields<Entity>(x => x.F(item => item.Name))
                        .Fields<Entity>(x => x.F(item => item.Description))
                        .Fields<ChildEntity>(x => x.F(item => item.Description))
                        .Fields<ChildEntity>(x => x.F(item => item.Relation))
                        .Where<Entity>(x => x.WP(item => item.Id, "IdParam"))
                        .Where<Entity>(x => x.WV(item => item.Name, "Arild"))
                        .Build(),
                    @select
                        .OnSchema(setSchemaNameIfExpressionIsEvaluatedToTrue:() => useSchema)
                        .Fields<Entity>(x => x.F(item => item.Name))
                        .Fields<Entity>(x => x.F(item => item.Description))
                        .Fields<ChildEntity>(x => x.F(item => item.Description))
                        .Fields<ChildEntity>(x => x.F(item => item.Relation))
                        .InnerJoin<Entity, ChildEntity>()
                        .LeftOuterJoin<ChildEntity, ChildChildEntity>()
                        .Where<Entity>(x => x.WP(item => item.Id, "IdParam"))
                        .Where<Entity>(x => x.WV(item => item.Name, "Arild"))
                        .Build(),
                    @select
                        .OnSchema(setSchemaNameIfExpressionIsEvaluatedToTrue:() => useSchema)
                        .Fields<Entity>(x => x.F(item => item.Name))
                        .Fields<Entity>(x => x.F(item => item.Description))
                        .Fields<ChildEntity>(x => x.F(item => item.Description))
                        .Fields<ChildEntity>(x => x.F(item => item.Relation))
                        .From<Entity>()
                        .InnerJoin<Entity, ChildEntity>()
                        .LeftOuterJoin<ChildEntity, ChildChildEntity>()
                        .Where<Entity>(x => x.WP(item => item.Id, "IdParam"))
                        .Where<Entity>(x => x.WV(item => item.Name, "Arild"))
                        .Build(),
                };
            foreach (var sql in sqls)
            {
                sql.Should().Be(expectedSql);
            }
        }
        
        [Theory]
        [InlineData(SupportedDatabaseTypes.Oracle, null, "SELECT ce.Description, ce.Relation, e.Name, e.Description FROM EntityTable e INNER JOIN ChildEntityTable ce ON e.ChildEntityTableId = ce.Id LEFT OUTER JOIN ChildChildEntityTable cce ON ce.ChildChildEntityTableId = cce.Id WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, "SELECT ce.Description, ce.Relation, e.Name, e.Description FROM EntityTable e INNER JOIN ChildEntityTable ce ON e.ChildEntityTableId = ce.Id LEFT OUTER JOIN ChildChildEntityTable cce ON ce.ChildChildEntityTableId = cce.Id WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", "SELECT ce.Description, ce.Relation, e.Name, e.Description FROM {0}.EntityTable e INNER JOIN {0}.ChildEntityTable ce ON e.ChildEntityTableId = ce.Id LEFT OUTER JOIN {0}.ChildChildEntityTable cce ON ce.ChildChildEntityTableId = cce.Id WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", "SELECT ce.Description, ce.Relation, e.Name, e.Description FROM {0}.EntityTable e INNER JOIN {0}.ChildEntityTable ce ON e.ChildEntityTableId = ce.Id LEFT OUTER JOIN {0}.ChildChildEntityTable cce ON ce.ChildChildEntityTableId = cce.Id WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        public void SelectTestWithJoins_WithTableNameSet(SupportedDatabaseTypes databaseTypes, string schema, string expectedSql)
        {
            const string entityTableName = "EntityTable";
            const string childTableName = "ChildEntityTable";
            const string childChildTableName = "ChildChildEntityTable";
            var dbConfig = OverrideConfig.CreateTestDbConfig(databaseTypes, schema);
            var useSchema = !string.IsNullOrEmpty(schema);
            expectedSql = string.Format(expectedSql, dbConfig.Schema);
            
            var builder = dbConfig.CreateSqlBuilder();
            var select = builder.Select();

            var sqls = new List<string>
                {
                    @select
                        .OnSchema(setSchemaNameIfExpressionIsEvaluatedToTrue:() => useSchema)
                        .From<Entity>(entityTableName)
                        .InnerJoin<Entity, ChildEntity>(fromTableName: entityTableName, toTableName: childTableName)
                        .LeftOuterJoin<ChildEntity, ChildChildEntity>(fromTableName: childTableName, toTableName: childChildTableName)
                        .Fields<ChildEntity>(x => x.F(item => item.Description))
                        .Fields<ChildEntity>(x => x.F(item => item.Relation))
                        .Fields<Entity>(x => x.F(item => item.Name))
                        .Fields<Entity>(x => x.F(item => item.Description))
                        .Where<Entity>(x => x.WP(item => item.Id, "IdParam"))
                        .Where<Entity>(x => x.WV(item => item.Name, "Arild"))
                        .Build(),
                    @select
                        .OnSchema(setSchemaNameIfExpressionIsEvaluatedToTrue:() => useSchema)
                        .From<Entity>(entityTableName)
                        .Fields<ChildEntity>(x => x.F(item => item.Description))
                        .Fields<ChildEntity>(x => x.F(item => item.Relation))
                        .Fields<Entity>(x => x.F(item => item.Name))
                        .Fields<Entity>(x => x.F(item => item.Description))
                        .InnerJoin<Entity, ChildEntity>(fromTableName: entityTableName, toTableName: childTableName)
                        .LeftOuterJoin<ChildEntity, ChildChildEntity>(fromTableName: childTableName, toTableName: childChildTableName)
                        .Where<Entity>(x => x.WP(item => item.Id, "IdParam"))
                        .Where<Entity>(x => x.WV(item => item.Name, "Arild"))
                        .Build(),
                    @select
                        .OnSchema(setSchemaNameIfExpressionIsEvaluatedToTrue:() => useSchema)
                        .InnerJoin<Entity, ChildEntity>(fromTableName: entityTableName, toTableName: childTableName)
                        .LeftOuterJoin<ChildEntity, ChildChildEntity>(fromTableName: childTableName, toTableName: childChildTableName)
                        .Fields<ChildEntity>(x => x.F(item => item.Description))
                        .Fields<ChildEntity>(x => x.F(item => item.Relation))
                        .Fields<Entity>(x => x.F(item => item.Name))
                        .Fields<Entity>(x => x.F(item => item.Description))
                        .Where<Entity>(x => x.WP(item => item.Id, "IdParam"))
                        .Where<Entity>(x => x.WV(item => item.Name, "Arild"))
                        .Build(),
                };
            foreach (var sql in sqls)
            {
                sql.Should().Be(expectedSql);
            }
        }

        [Theory]
        [InlineData(true, "SELECT e.Name, e.Description FROM Entity e WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        [InlineData(false, "SELECT e.Name, e.Description FROM Entity e WHERE e.Id = @IdParam")]
        public void SelectWithIfStatementTest(bool ifStatementResult, string expectedSql)
        {
            var dbConfig = OverrideConfig.CreateTestDbConfig(SupportedDatabaseTypes.Postgres);
            expectedSql = string.Format(expectedSql, dbConfig.Schema);

            var builder = dbConfig.CreateSqlBuilder();
            var sql =
                builder.Select()
                    .Fields<Entity>(x => x.F(item => item.Name))
                    .Fields<Entity>(x => x.F(item => item.Description))
                    .Where<Entity>(x => x.WP(item => item.Id, "IdParam"))
                    .WhereIf<Entity>(x => x.WV(item => item.Name, "Arild"), () => ifStatementResult)
                    .Build();

            sql.Should().Be(expectedSql);
        }


    }
}