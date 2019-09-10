using System.Collections.Generic;
using FluentDbTools.Extensions.SqlBuilder;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.SqlBuilder.Abstractions.Common;
using TestUtilities.FluentDbTools;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Test.FluentDbTools.SqlBuilder.MinimumDependencies.TestEntities;
using Xunit;

namespace Test.FluentDbTools.SqlBuilder
{
    public class QueryBuilderUpdateTest
    {
        [Theory]
        [InlineData(SupportedDatabaseTypes.Oracle, false, null, "UPDATE {1}Entity SET Description = :Description, Name = 'Arild' WHERE Id = :IdParam")]
        [InlineData(SupportedDatabaseTypes.Postgres, false, null, "UPDATE {1}Entity SET Description = @Description, Name = 'Arild' WHERE Id = @IdParam")]
        [InlineData(SupportedDatabaseTypes.Oracle, true, null, "UPDATE {0}.{1}Entity SET Description = :Description, Name = 'Arild' WHERE Id = :IdParam")]
        [InlineData(SupportedDatabaseTypes.Postgres, true, null, "UPDATE {0}.{1}Entity SET Description = @Description, Name = 'Arild' WHERE Id = @IdParam")]
        [InlineData(SupportedDatabaseTypes.Oracle, false, "UP", "UPDATE {1}Entity SET Description = :Description, Name = 'Arild' WHERE Id = :IdParam")]
        [InlineData(SupportedDatabaseTypes.Postgres, false, "UP", "UPDATE {1}Entity SET Description = @Description, Name = 'Arild' WHERE Id = @IdParam")]
        [InlineData(SupportedDatabaseTypes.Oracle, true, "UP", "UPDATE {0}.{1}Entity SET Description = :Description, Name = 'Arild' WHERE Id = :IdParam")]
        [InlineData(SupportedDatabaseTypes.Postgres, true, "UP", "UPDATE {0}.{1}Entity SET Description = @Description, Name = 'Arild' WHERE Id = @IdParam")]
        public void UpdateTest(SupportedDatabaseTypes databaseTypes, bool useSchema,string schemaPrefixId, string expectedSql)
        {
            var addDictionary = new Dictionary<string, string> {{"database:schemaPrefix:Id", schemaPrefixId}};
            using (var scope = TestServiceProvider.GetDatabaseExampleServiceProvider(databaseTypes, addDictionary).CreateScope())
            {
                var dbConfig = scope.ServiceProvider.GetService<IDbConfigDatabaseTargets>();
                expectedSql = string.Format(expectedSql, dbConfig.Schema, dbConfig.GetSchemaPrefixId());
                
                var builder = dbConfig.CreateSqlBuilder();
                var update = builder.Update<Entity>();
                
                var sql = update
                    .OnSchema(setSchemaNameIfExpressionIsEvaluatedToTrue:() => useSchema)    
                    .Fields(x => x.FP(f => f.Description)
                                  .FV(f => f.Name, "Arild"))
                    .Where(x => x.WP(item => item.Id, "IdParam"))
                    .Build();

                sql.Should().Be(expectedSql);
            }
        }

        [Theory]
        [InlineData(SupportedDatabaseTypes.Oracle, false, null, "UPDATE {1}Entity SET Description = :Description, Name = 'Arild' WHERE Id = :IdParam")]
        [InlineData(SupportedDatabaseTypes.Postgres, false, null, "UPDATE {1}Entity SET Description = @Description, Name = 'Arild' WHERE Id = @IdParam")]
        [InlineData(SupportedDatabaseTypes.Oracle, true, null, "UPDATE {0}.{1}Entity SET Description = :Description, Name = 'Arild' WHERE Id = :IdParam AND Name <> 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, true, null, "UPDATE {0}.{1}Entity SET Description = @Description, Name = 'Arild' WHERE Id = @IdParam AND Name <> 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, false, "UPD", "UPDATE {1}Entity SET Description = :Description, Name = 'Arild' WHERE Id = :IdParam")]
        [InlineData(SupportedDatabaseTypes.Postgres, false, "UPD", "UPDATE {1}Entity SET Description = @Description, Name = 'Arild' WHERE Id = @IdParam")]
        [InlineData(SupportedDatabaseTypes.Oracle, true, "UPD", "UPDATE {0}.{1}Entity SET Description = :Description, Name = 'Arild' WHERE Id = :IdParam AND Name <> 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, true, "UPD", "UPDATE {0}.{1}Entity SET Description = @Description, Name = 'Arild' WHERE Id = @IdParam AND Name <> 'Arild'")]
        public void UpdateTestWithWhereIfTest(SupportedDatabaseTypes databaseTypes, bool ifStatementResult, string schemaPrefixId, string expectedSql)
        {
            var addDictionary = new Dictionary<string, string> {{"database:schemaPrefix:Id", schemaPrefixId}};
            using (var scope = TestServiceProvider.GetDatabaseExampleServiceProvider(databaseTypes, addDictionary).CreateScope())
            {
                var dbConfig = scope.ServiceProvider.GetService<IDbConfigDatabaseTargets>();
                expectedSql = string.Format(expectedSql, dbConfig.Schema, dbConfig.GetSchemaPrefixId());
                var builder = dbConfig.CreateSqlBuilder();
                var update = builder.Update<Entity>();

                var sql = update
                    .OnSchema(setSchemaNameIfExpressionIsEvaluatedToTrue:() => ifStatementResult)    
                    .Fields(x => x.FP(f => f.Description)
                        .FV(f => f.Name, "Arild"))
                    .Where(x => x.WP(item => item.Id, "IdParam"))
                    .WhereIf(x => x.WV(item => item.Name, "Arild", OP.DI), () => ifStatementResult)
                    .Build();

                sql.Should().Be(expectedSql);
            }
        }
    }
}