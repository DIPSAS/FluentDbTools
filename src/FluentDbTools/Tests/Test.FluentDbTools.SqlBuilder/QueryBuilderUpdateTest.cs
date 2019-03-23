using FluentDbTools.Extensions.SqlBuilder;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.SqlBuilder.Abstractions.Common;
using TestUtilities.FluentDbTools;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Test.FluentDbTools.SqlBuilder.TestEntities;
using Xunit;

namespace Test.FluentDbTools.SqlBuilder
{
    public class QueryBuilderUpdateTest
    {
        [Theory]
        [InlineData(SupportedDatabaseTypes.Oracle, false, "UPDATE Entity SET Description = :Description, Name = 'Arild' WHERE Id = :IdParam")]
        [InlineData(SupportedDatabaseTypes.Postgres, false, "UPDATE Entity SET Description = @Description, Name = 'Arild' WHERE Id = @IdParam")]
        [InlineData(SupportedDatabaseTypes.Oracle, true, "UPDATE {0}.Entity SET Description = :Description, Name = 'Arild' WHERE Id = :IdParam")]
        [InlineData(SupportedDatabaseTypes.Postgres, true, "UPDATE {0}.Entity SET Description = @Description, Name = 'Arild' WHERE Id = @IdParam")]
        public void UpdateTest(SupportedDatabaseTypes databaseTypes, bool useSchema, string expectedSql)
        {
            using (var scope = TestServiceProvider.GetDatabaseExampleServiceProvider(databaseTypes).CreateScope())
            {
                var dbConfig = scope.ServiceProvider.GetService<IDbConfig>();
                expectedSql = string.Format(expectedSql, dbConfig.Schema);
                
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
        [InlineData(SupportedDatabaseTypes.Oracle, false, "UPDATE Entity SET Description = :Description, Name = 'Arild' WHERE Id = :IdParam")]
        [InlineData(SupportedDatabaseTypes.Postgres, false, "UPDATE Entity SET Description = @Description, Name = 'Arild' WHERE Id = @IdParam")]
        [InlineData(SupportedDatabaseTypes.Oracle, true, "UPDATE Entity SET Description = :Description, Name = 'Arild' WHERE Id = :IdParam AND Name <> 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, true, "UPDATE Entity SET Description = @Description, Name = 'Arild' WHERE Id = @IdParam AND Name <> 'Arild'")]
        public void UpdateTestWithWhereIfTest(SupportedDatabaseTypes databaseTypes, bool ifStatementResult, string expectedSql)
        {
            using (var scope = TestServiceProvider.GetDatabaseExampleServiceProvider(databaseTypes).CreateScope())
            {
                var dbConfig = scope.ServiceProvider.GetService<IDbConfig>();
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
}