using FluentDbTools.Extensions.SqlBuilder;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.SqlBuilder.Abstractions;
using FluentDbTools.SqlBuilder.Abstractions.Parameters;
using FluentDbTools.SqlBuilder.Tests.TestEntities;
using FluentDbTools.TestUtilities;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FluentDbTools.SqlBuilder.Tests
{
    public class QueryBuilderInsertTest
    {
        [Theory]
        [InlineData(SupportedDatabaseTypes.Oracle, false, "INSERT INTO Entity(Id, Name, Description) VALUES(:IdParam, 'Arild', :Description)")]
        [InlineData(SupportedDatabaseTypes.Postgres, false, "INSERT INTO Entity(Id, Name, Description) VALUES(@IdParam, 'Arild', @Description)")]
        [InlineData(SupportedDatabaseTypes.Oracle, true, "INSERT INTO {0}.Entity(Id, Name, Description) VALUES(:IdParam, 'Arild', :Description)")]
        [InlineData(SupportedDatabaseTypes.Postgres, true, "INSERT INTO {0}.Entity(Id, Name, Description) VALUES(@IdParam, 'Arild', @Description)")]
        public void InsertTest1(SupportedDatabaseTypes databaseTypes, bool useSchema, string expectedSql)
        {
            using (var scope = TestServiceProvider.GetDatabaseExampleServiceProvider(databaseTypes).CreateScope())
            {
                var dbConfig = scope.ServiceProvider.GetService<IDbConfig>();
                expectedSql = string.Format(expectedSql, dbConfig.Schema);
                
                var builder = dbConfig.CreateSqlBuilder();
                var insert = builder.Insert<Entity>();
                
                var sql = insert
                            .OnSchema(setSchemaNameIfExpressionIsEvaluatedToTrue: () => useSchema)
                            .Fields(x => x.FP(f => f.Id, "IdParam"))
                            .Fields(x => x.FV(f => f.Name, "Arild"))
                            .Fields(x => x.FP(f => f.Description))
                            .Build();

                sql.Should().Be(expectedSql);
            }
        }
        
        [Theory]
        [InlineData(SupportedDatabaseTypes.Oracle, false, "INSERT INTO Entity(Id, Name, Description) VALUES(seq.nextval, 'Arild', :Description)")]
        [InlineData(SupportedDatabaseTypes.Postgres, false, "INSERT INTO Entity(Id, Name, Description) VALUES(nextval('seq'), 'Arild', @Description)")]
        [InlineData(SupportedDatabaseTypes.Oracle, true, "INSERT INTO {0}.Entity(Id, Name, Description) VALUES(seq.nextval, 'Arild', :Description)")]
        [InlineData(SupportedDatabaseTypes.Postgres, true, "INSERT INTO {0}.Entity(Id, Name, Description) VALUES(nextval('seq'), 'Arild', @Description)")]
        public void InsertTest2(SupportedDatabaseTypes databaseTypes, bool useSchema, string expectedSql)
        {
            using (var scope = TestServiceProvider.GetDatabaseExampleServiceProvider(databaseTypes).CreateScope())
            {
                var dbConfig = scope.ServiceProvider.GetService<IDbConfig>();
                expectedSql = string.Format(expectedSql, dbConfig.Schema);
                
                var builder = dbConfig.CreateSqlBuilder();
                var resolver = dbConfig.CreateParameterResolver();
                var insert = builder.Insert<Entity>();
                var sql = insert
                    .OnSchema(setSchemaNameIfExpressionIsEvaluatedToTrue: () => useSchema)
                    .Fields(x => x.FV(f => f.Id, resolver.WithNextSequence("seq"), true))
                    .Fields(x => x.FV(f => f.Name, "Arild"))
                    .Fields(x => x.FP(f => f.Description))
                    .Build();

                sql.Should().Be(expectedSql);
            }
        }
    }
}