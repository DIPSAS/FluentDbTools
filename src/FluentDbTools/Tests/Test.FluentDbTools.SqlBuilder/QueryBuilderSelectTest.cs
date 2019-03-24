using System.Collections.Generic;
using FluentDbTools.Extensions.SqlBuilder;
using FluentDbTools.Common.Abstractions;
using TestUtilities.FluentDbTools;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Test.FluentDbTools.SqlBuilder.TestEntities;
using Xunit;

namespace Test.FluentDbTools.SqlBuilder
{
    public class QueryBuilderSelectTest
    {
        [Theory]
        [InlineData(SupportedDatabaseTypes.Oracle, false, "SELECT e.Name, e.Description FROM Entity e WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, false, "SELECT e.Name, e.Description FROM Entity e WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, true, "SELECT e.Name, e.Description FROM {0}.Entity e WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, true, "SELECT e.Name, e.Description FROM {0}.Entity e WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        public void SelectTestOneTableOnly(SupportedDatabaseTypes databaseTypes, bool useSchema, string expectedSql)
        {
            using (var scope = TestServiceProvider.GetDatabaseExampleServiceProvider(databaseTypes).CreateScope())
            {
                var dbConfig = scope.ServiceProvider.GetService<IDbConfig>();
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
        }

        [Theory]
        [InlineData(SupportedDatabaseTypes.Oracle, false, "SELECT ce.Description, ce.Relation, e.Name, e.Description FROM Entity e INNER JOIN ChildEntity ce ON e.ChildEntityId = ce.Id LEFT OUTER JOIN ChildChildEntity cce ON ce.ChildChildEntityId = cce.Id WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, false, "SELECT ce.Description, ce.Relation, e.Name, e.Description FROM Entity e INNER JOIN ChildEntity ce ON e.ChildEntityId = ce.Id LEFT OUTER JOIN ChildChildEntity cce ON ce.ChildChildEntityId = cce.Id WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, true, "SELECT ce.Description, ce.Relation, e.Name, e.Description FROM {0}.Entity e INNER JOIN {0}.ChildEntity ce ON e.ChildEntityId = ce.Id LEFT OUTER JOIN {0}.ChildChildEntity cce ON ce.ChildChildEntityId = cce.Id WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, true, "SELECT ce.Description, ce.Relation, e.Name, e.Description FROM {0}.Entity e INNER JOIN {0}.ChildEntity ce ON e.ChildEntityId = ce.Id LEFT OUTER JOIN {0}.ChildChildEntity cce ON ce.ChildChildEntityId = cce.Id WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        public void SelectTestWithJoins(SupportedDatabaseTypes databaseTypes, bool useSchema, string expectedSql)
        {
            using (var scope = TestServiceProvider.GetDatabaseExampleServiceProvider(databaseTypes).CreateScope())
            {
                var dbConfig = scope.ServiceProvider.GetService<IDbConfig>();
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
        }

        [Theory]
        [InlineData(SupportedDatabaseTypes.Oracle, false, "SELECT e.Name, e.Description, ce.Description, ce.Relation FROM Entity e INNER JOIN ChildEntity ce ON e.ChildEntityId = ce.Id LEFT OUTER JOIN ChildChildEntity cce ON ce.ChildChildEntityId = cce.Id WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, false, "SELECT e.Name, e.Description, ce.Description, ce.Relation FROM Entity e INNER JOIN ChildEntity ce ON e.ChildEntityId = ce.Id LEFT OUTER JOIN ChildChildEntity cce ON ce.ChildChildEntityId = cce.Id WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, true, "SELECT e.Name, e.Description, ce.Description, ce.Relation FROM {0}.Entity e INNER JOIN {0}.ChildEntity ce ON e.ChildEntityId = ce.Id LEFT OUTER JOIN {0}.ChildChildEntity cce ON ce.ChildChildEntityId = cce.Id WHERE e.Id = :IdParam AND e.Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, true, "SELECT e.Name, e.Description, ce.Description, ce.Relation FROM {0}.Entity e INNER JOIN {0}.ChildEntity ce ON e.ChildEntityId = ce.Id LEFT OUTER JOIN {0}.ChildChildEntity cce ON ce.ChildChildEntityId = cce.Id WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        public void SelectTest2WithJoins(SupportedDatabaseTypes databaseTypes, bool useSchema, string expectedSql)
        {
            using (var scope = TestServiceProvider.GetDatabaseExampleServiceProvider(databaseTypes).CreateScope())
            {
                var dbConfig = scope.ServiceProvider.GetService<IDbConfig>();
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
        }

        [Theory]
        [InlineData(true, "SELECT e.Name, e.Description FROM Entity e WHERE e.Id = @IdParam AND e.Name = 'Arild'")]
        [InlineData(false, "SELECT e.Name, e.Description FROM Entity e WHERE e.Id = @IdParam")]
        public void SelectWithIfStatementTest(bool ifStatementResult, string expectedSql)
        {
            using (var scope = TestServiceProvider.GetDatabaseExampleServiceProvider().CreateScope())
            {
                var dbConfig = scope.ServiceProvider.GetService<IDbConfig>();
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
}