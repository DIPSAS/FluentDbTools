using System.Collections.Generic;
using FluentDbTools.Common.Abstractions;
using FluentAssertions;
using FluentDbTools.SqlBuilder;
using FluentDbTools.SqlBuilder.Common;
using Test.FluentDbTools.SqlBuilder.MinimumDependencies.TestEntities;
using Xunit;

namespace Test.FluentDbTools.SqlBuilder.MinimumDependencies
{
    public class StaticQueryBuilderSelectTest
    {
        [Theory]
        [InlineData(SupportedDatabaseTypes.Oracle, null, null, "SELECT e.Name, e.Description FROM {1}Entity e WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, null, "SELECT e.Name, e.Description FROM {1}Entity e WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", null, "SELECT e.Name, e.Description FROM {0}.{1}Entity e WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", null, "SELECT e.Name, e.Description FROM {0}.{1}Entity e WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, null, "XY", "SELECT e.Name, e.Description FROM {1}Entity e WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, "XY", "SELECT e.Name, e.Description FROM {1}Entity e WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", "XY", "SELECT e.Name, e.Description FROM {0}.{1}Entity e WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", "XY", "SELECT e.Name, e.Description FROM {0}.{1}Entity e WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        public void SelectTestOneTableOnly(SupportedDatabaseTypes databaseTypes, string schema, string schemaPrefixId, string expectedSql)
        {
            SqlAliasHelper.ClearAliases();

            var dbConfig = SqlBuilderFactory.DbConfigSchemaTargets(schema, schemaPrefixId, databaseTypes);

            var useSchema = !string.IsNullOrEmpty(schema);
            expectedSql = string.Format(expectedSql, dbConfig.Schema, dbConfig.GetSchemaPrefixId() ?? string.Empty);

            var builder = dbConfig.SqlBuilder();
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
        [InlineData(SupportedDatabaseTypes.Oracle, null, null, "SELECT e.Name, e.Description FROM {1}EntityTable e WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, null, "SELECT e.Name, e.Description FROM {1}EntityTable e WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", null, "SELECT e.Name, e.Description FROM {0}.{1}EntityTable e WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", null, "SELECT e.Name, e.Description FROM {0}.{1}EntityTable e WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, null, "XY", "SELECT e.Name, e.Description FROM {1}EntityTable e WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, "XY", "SELECT e.Name, e.Description FROM {1}EntityTable e WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", "XY", "SELECT e.Name, e.Description FROM {0}.{1}EntityTable e WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", "XY", "SELECT e.Name, e.Description FROM {0}.{1}EntityTable e WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        public void SelectTestOneTableOnly_WithTableNameSet(SupportedDatabaseTypes databaseTypes, string schema, string schemaPrefixId, string expectedSql)
        {
            SqlAliasHelper.ClearAliases();

            const string tableName = "EntityTable";
            var dbConfig = SqlBuilderFactory.DbConfigSchemaTargets(schema, schemaPrefixId, databaseTypes);
            var useSchema = !string.IsNullOrEmpty(schema);
            expectedSql = string.Format(expectedSql, dbConfig.Schema, dbConfig.GetSchemaPrefixId() ?? string.Empty);

            var builder = dbConfig.SqlBuilder();
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
        [InlineData(SupportedDatabaseTypes.Oracle, null, null, "SELECT COUNT(e.Name, e.Description) FROM {1}Entity e WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, null, "SELECT COUNT(e.Name, e.Description) FROM {1}Entity e WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", null, "SELECT COUNT(e.Name, e.Description) FROM {0}.{1}Entity e WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", null, "SELECT COUNT(e.Name, e.Description) FROM {0}.{1}Entity e WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, null, "XY", "SELECT COUNT(e.Name, e.Description) FROM {1}Entity e WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, "XY", "SELECT COUNT(e.Name, e.Description) FROM {1}Entity e WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", "XY", "SELECT COUNT(e.Name, e.Description) FROM {0}.{1}Entity e WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", "XY", "SELECT COUNT(e.Name, e.Description) FROM {0}.{1}Entity e WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        public void SelectCountTestOneTableOnly(SupportedDatabaseTypes databaseTypes, string schema, string schemaPrefixId, string expectedSql)
        {
            SqlAliasHelper.ClearAliases();

            var dbConfig = SqlBuilderFactory.DbConfigSchemaTargets(schema, schemaPrefixId, databaseTypes);
            var useSchema = !string.IsNullOrEmpty(schema);
            expectedSql = string.Format(expectedSql, dbConfig.Schema, dbConfig.GetSchemaPrefixId() ?? string.Empty);

            var builder = dbConfig.SqlBuilder();
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
        [InlineData(SupportedDatabaseTypes.Oracle, null, null, "SELECT ce.Description, ce.Relation, e.Name, e.Description FROM Entity e INNER JOIN ChildEntity ce ON e.ChildEntityId = ce.Id LEFT OUTER JOIN ChildChildEntity cce ON ce.ChildChildEntityId = cce.Id WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, null, "SELECT ce.Description, ce.Relation, e.Name, e.Description FROM Entity e INNER JOIN ChildEntity ce ON e.ChildEntityId = ce.Id LEFT OUTER JOIN ChildChildEntity cce ON ce.ChildChildEntityId = cce.Id WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", null, "SELECT ce.Description, ce.Relation, e.Name, e.Description FROM {0}.Entity e INNER JOIN {0}.ChildEntity ce ON e.ChildEntityId = ce.Id LEFT OUTER JOIN {0}.ChildChildEntity cce ON ce.ChildChildEntityId = cce.Id WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", null, "SELECT ce.Description, ce.Relation, e.Name, e.Description FROM {0}.Entity e INNER JOIN {0}.ChildEntity ce ON e.ChildEntityId = ce.Id LEFT OUTER JOIN {0}.ChildChildEntity cce ON ce.ChildChildEntityId = cce.Id WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, null, "XY", "SELECT ce.Description, ce.Relation, e.Name, e.Description FROM {1}Entity e INNER JOIN {1}ChildEntity ce ON e.ChildEntityId = ce.Id LEFT OUTER JOIN {1}ChildChildEntity cce ON ce.ChildChildEntityId = cce.Id WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, "XY", "SELECT ce.Description, ce.Relation, e.Name, e.Description FROM {1}Entity e INNER JOIN {1}ChildEntity ce ON e.ChildEntityId = ce.Id LEFT OUTER JOIN {1}ChildChildEntity cce ON ce.ChildChildEntityId = cce.Id WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", "XY", "SELECT ce.Description, ce.Relation, e.Name, e.Description FROM {0}.{1}Entity e INNER JOIN {0}.{1}ChildEntity ce ON e.ChildEntityId = ce.Id LEFT OUTER JOIN {0}.{1}ChildChildEntity cce ON ce.ChildChildEntityId = cce.Id WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", "XY", "SELECT ce.Description, ce.Relation, e.Name, e.Description FROM {0}.{1}Entity e INNER JOIN {0}.{1}ChildEntity ce ON e.ChildEntityId = ce.Id LEFT OUTER JOIN {0}.{1}ChildChildEntity cce ON ce.ChildChildEntityId = cce.Id WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        public void SelectTestWithJoins(SupportedDatabaseTypes databaseTypes, string schema, string schemaPrefixId, string expectedSql)
        {
            SqlAliasHelper.ClearAliases();

            var dbConfig = SqlBuilderFactory.DbConfigSchemaTargets(schema, schemaPrefixId, databaseTypes);

            var useSchema = !string.IsNullOrEmpty(schema);
            expectedSql = string.Format(expectedSql, dbConfig.Schema, dbConfig.GetSchemaPrefixId());

            var builder = dbConfig.SqlBuilder();
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
        [InlineData(SupportedDatabaseTypes.Oracle, null, null, "SELECT e.Name, e.Description, ce.Description, ce.Relation FROM {1}Entity e INNER JOIN {1}ChildEntity ce ON e.ChildEntityId = ce.Id LEFT OUTER JOIN {1}ChildChildEntity cce ON ce.ChildChildEntityId = cce.Id WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, null, "SELECT e.Name, e.Description, ce.Description, ce.Relation FROM {1}Entity e INNER JOIN {1}ChildEntity ce ON e.ChildEntityId = ce.Id LEFT OUTER JOIN {1}ChildChildEntity cce ON ce.ChildChildEntityId = cce.Id WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, "oracle_schema", null, "SELECT e.Name, e.Description, ce.Description, ce.Relation FROM {0}.{1}Entity e INNER JOIN {0}.{1}ChildEntity ce ON e.ChildEntityId = ce.Id LEFT OUTER JOIN {0}.{1}ChildChildEntity cce ON ce.ChildChildEntityId = cce.Id WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, "postgres_schema", null, "SELECT e.Name, e.Description, ce.Description, ce.Relation FROM {0}.{1}Entity e INNER JOIN {0}.{1}ChildEntity ce ON e.ChildEntityId = ce.Id LEFT OUTER JOIN {0}.{1}ChildChildEntity cce ON ce.ChildChildEntityId = cce.Id WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, null, "XY", "SELECT e.Name, e.Description, ce.Description, ce.Relation FROM {1}Entity e INNER JOIN {1}ChildEntity ce ON e.ChildEntityId = ce.Id LEFT OUTER JOIN {1}ChildChildEntity cce ON ce.ChildChildEntityId = cce.Id WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, "XY", "SELECT e.Name, e.Description, ce.Description, ce.Relation FROM {1}Entity e INNER JOIN {1}ChildEntity ce ON e.ChildEntityId = ce.Id LEFT OUTER JOIN {1}ChildChildEntity cce ON ce.ChildChildEntityId = cce.Id WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, "oracle_schema", "XY", "SELECT e.Name, e.Description, ce.Description, ce.Relation FROM {0}.{1}Entity e INNER JOIN {0}.{1}ChildEntity ce ON e.ChildEntityId = ce.Id LEFT OUTER JOIN {0}.{1}ChildChildEntity cce ON ce.ChildChildEntityId = cce.Id WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, "postgres_schema", "XY", "SELECT e.Name, e.Description, ce.Description, ce.Relation FROM {0}.{1}Entity e INNER JOIN {0}.{1}ChildEntity ce ON e.ChildEntityId = ce.Id LEFT OUTER JOIN {0}.{1}ChildChildEntity cce ON ce.ChildChildEntityId = cce.Id WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        public void SelectTest2WithJoins(SupportedDatabaseTypes databaseTypes, string schema, string schemaPrefixId, string expectedSql)
        {
            SqlAliasHelper.ClearAliases();

            var dbConfig = SqlBuilderFactory.DbConfigSchemaTargets(schema, schemaPrefixId, databaseTypes);
            var useSchema = !string.IsNullOrEmpty(schema);
            expectedSql = string.Format(expectedSql, dbConfig.Schema, dbConfig.GetSchemaPrefixId() ?? string.Empty);

            var builder = dbConfig.SqlBuilder();
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
        [InlineData(SupportedDatabaseTypes.Oracle, null, null, "SELECT cet.Description, cet.Relation, et.Name, et.Description FROM {1}EntityTable et INNER JOIN {1}ChildEntityTable cet ON et.ChildEntityTableId = cet.Id LEFT OUTER JOIN {1}ChildChildEntityTable ccet ON cet.ChildChildEntityTableId = ccet.Id WHERE et.Id = :IdParam AND et.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, null, "SELECT cet.Description, cet.Relation, et.Name, et.Description FROM {1}EntityTable et INNER JOIN {1}ChildEntityTable cet ON et.ChildEntityTableId = cet.Id LEFT OUTER JOIN {1}ChildChildEntityTable ccet ON cet.ChildChildEntityTableId = ccet.Id WHERE et.Id = @IdParam AND et.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", null, "SELECT cet.Description, cet.Relation, et.Name, et.Description FROM {0}.{1}EntityTable et INNER JOIN {0}.{1}ChildEntityTable cet ON et.ChildEntityTableId = cet.Id LEFT OUTER JOIN {0}.{1}ChildChildEntityTable ccet ON cet.ChildChildEntityTableId = ccet.Id WHERE et.Id = :IdParam AND et.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", null, "SELECT cet.Description, cet.Relation, et.Name, et.Description FROM {0}.{1}EntityTable et INNER JOIN {0}.{1}ChildEntityTable cet ON et.ChildEntityTableId = cet.Id LEFT OUTER JOIN {0}.{1}ChildChildEntityTable ccet ON cet.ChildChildEntityTableId = ccet.Id WHERE et.Id = @IdParam AND et.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, null, "XY", "SELECT cet.Description, cet.Relation, et.Name, et.Description FROM {1}EntityTable et INNER JOIN {1}ChildEntityTable cet ON et.ChildEntityTableId = cet.Id LEFT OUTER JOIN {1}ChildChildEntityTable ccet ON cet.ChildChildEntityTableId = ccet.Id WHERE et.Id = :IdParam AND et.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, "XY", "SELECT cet.Description, cet.Relation, et.Name, et.Description FROM {1}EntityTable et INNER JOIN {1}ChildEntityTable cet ON et.ChildEntityTableId = cet.Id LEFT OUTER JOIN {1}ChildChildEntityTable ccet ON cet.ChildChildEntityTableId = ccet.Id WHERE et.Id = @IdParam AND et.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", "XY", "SELECT cet.Description, cet.Relation, et.Name, et.Description FROM {0}.{1}EntityTable et INNER JOIN {0}.{1}ChildEntityTable cet ON et.ChildEntityTableId = cet.Id LEFT OUTER JOIN {0}.{1}ChildChildEntityTable ccet ON cet.ChildChildEntityTableId = ccet.Id WHERE et.Id = :IdParam AND et.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", "XY", "SELECT cet.Description, cet.Relation, et.Name, et.Description FROM {0}.{1}EntityTable et INNER JOIN {0}.{1}ChildEntityTable cet ON et.ChildEntityTableId = cet.Id LEFT OUTER JOIN {0}.{1}ChildChildEntityTable ccet ON cet.ChildChildEntityTableId = ccet.Id WHERE et.Id = @IdParam AND et.Name = 'Arild'")]
        public void SelectTestWithJoins_WithTableNameSet(SupportedDatabaseTypes databaseTypes, string schema, string schemaPrefixId, string expectedSql)
        {
            SqlAliasHelper.ClearAliases();

            const string entityTableName = "EntityTable";
            const string childTableName = "ChildEntityTable";
            const string childChildTableName = "ChildChildEntityTable";
            var dbConfig = SqlBuilderFactory.DbConfigSchemaTargets(schema, schemaPrefixId, databaseTypes);
            var useSchema = !string.IsNullOrEmpty(schema);
            expectedSql = string.Format(expectedSql, dbConfig.Schema, dbConfig.GetSchemaPrefixId() ?? string.Empty);

            var builder = dbConfig.SqlBuilder();
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
        [InlineData(true, null, "SELECT e.Name, e.Description FROM {1}Entity e WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        [InlineData(false, null, "SELECT e.Name, e.Description FROM {1}Entity e WHERE e.Id = @IdParam")]
        [InlineData(true, "XY", "SELECT e.Name, e.Description FROM {1}Entity e WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        [InlineData(false, "XY", "SELECT e.Name, e.Description FROM {1}Entity e WHERE e.Id = @IdParam")]
        public void SelectWithIfStatementTest(bool ifStatementResult, string schemaPrefixId, string expectedSql)
        {
            SqlAliasHelper.ClearAliases();
            var dbConfig = SqlBuilderFactory.DbConfigSchemaTargets(null, schemaPrefixId, SupportedDatabaseTypes.Postgres);
            expectedSql = string.Format(expectedSql, dbConfig.Schema, dbConfig.GetSchemaPrefixId() ?? string.Empty);

            var builder = dbConfig.SqlBuilder();
            var sql =
                builder.Select()
                    .Fields<Entity>(x => x.F(item => item.Name))
                    .Fields<Entity>(x => x.F(item => item.Description))
                    .Where<Entity>(x => x.WP(item => item.Id, "IdParam"))
                    .WhereIf<Entity>(x => x.WV(item => item.Name, "Arild"), () => ifStatementResult)
                    .Build();

            sql.Should().Be(expectedSql);
        }

        [Theory]
        [InlineData(true, null, "SELECT ask.Name, ask.Description FROM {1}AggressiveStork ask WHERE ask.Id = @IdParam AND ask.Name = 'Arild'")]
        [InlineData(false, null, "SELECT ask.Name, ask.Description FROM {1}AggressiveStork ask WHERE ask.Id = @IdParam")]
        [InlineData(true, "XY", "SELECT ask.Name, ask.Description FROM {1}AggressiveStork ask WHERE ask.Id = @IdParam AND ask.Name = 'Arild'")]
        [InlineData(false, "XY", "SELECT ask.Name, ask.Description FROM {1}AggressiveStork ask WHERE ask.Id = @IdParam")]
        public void SelectInvalidAliasesStatementTest(bool ifStatementResult, string schemaPrefixId, string expectedSql)
        {
            SqlAliasHelper.ClearAliases();

            var dbConfig = SqlBuilderFactory.DbConfigSchemaTargets(null, schemaPrefixId, SupportedDatabaseTypes.Postgres);
            expectedSql = string.Format(expectedSql, dbConfig.Schema, dbConfig.GetSchemaPrefixId() ?? string.Empty);

            var builder = dbConfig.SqlBuilder();
            var sql =
                builder.Select()
                    .Fields<AggressiveStork>(x => x.F(item => item.Name))
                    .Fields<AggressiveStork>(x => x.F(item => item.Description))
                    .Where<AggressiveStork>(x => x.WP(item => item.Id, "IdParam"))
                    .WhereIf<AggressiveStork>(x => x.WV(item => item.Name, "Arild"), () => ifStatementResult)
                    .Build();

            sql.Should().Be(expectedSql);
        }
    }
}