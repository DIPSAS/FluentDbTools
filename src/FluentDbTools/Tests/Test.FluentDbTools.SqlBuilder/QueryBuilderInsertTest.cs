using System.Collections.Generic;
using FluentDbTools.Extensions.SqlBuilder;
using FluentDbTools.Common.Abstractions;
using TestUtilities.FluentDbTools;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Test.FluentDbTools.SqlBuilder.MinimumDependencies.TestEntities;
using Xunit;

namespace Test.FluentDbTools.SqlBuilder
{
    public class QueryBuilderInsertTest
    {
        [Theory]
        [InlineData(SupportedDatabaseTypes.Oracle, false, null, "INSERT INTO {1}Entity(Id, Name, Description) VALUES(:IdParam, 'Arild', :Description)")]
        [InlineData(SupportedDatabaseTypes.Postgres, false, null, "INSERT INTO {1}Entity(Id, Name, Description) VALUES(@IdParam, 'Arild', @Description)")]
        [InlineData(SupportedDatabaseTypes.Oracle, true, null, "INSERT INTO {0}.{1}Entity(Id, Name, Description) VALUES(:IdParam, 'Arild', :Description)")]
        [InlineData(SupportedDatabaseTypes.Postgres, true, null, "INSERT INTO {0}.{1}Entity(Id, Name, Description) VALUES(@IdParam, 'Arild', @Description)")]
        [InlineData(SupportedDatabaseTypes.Oracle, false, "PR", "INSERT INTO {1}Entity(Id, Name, Description) VALUES(:IdParam, 'Arild', :Description)")]
        [InlineData(SupportedDatabaseTypes.Postgres, false, "PR", "INSERT INTO {1}Entity(Id, Name, Description) VALUES(@IdParam, 'Arild', @Description)")]
        [InlineData(SupportedDatabaseTypes.Oracle, true, "PR", "INSERT INTO {0}.{1}Entity(Id, Name, Description) VALUES(:IdParam, 'Arild', :Description)")]
        [InlineData(SupportedDatabaseTypes.Postgres, true, "PR", "INSERT INTO {0}.{1}Entity(Id, Name, Description) VALUES(@IdParam, 'Arild', @Description)")]
        public void InsertTest1(SupportedDatabaseTypes databaseTypes, bool useSchema, string schemaPrefixId, string expectedSql)
        {
            var addDictionary = new Dictionary<string, string> { { "database:schemaPrefix:Id", schemaPrefixId } };
            using (var scope = TestServiceProvider.GetDatabaseExampleServiceProvider(databaseTypes, addDictionary).CreateScope())
            {
                var dbConfig = scope.ServiceProvider.GetService<IDbConfigDatabaseTargets>();
                expectedSql = string.Format(expectedSql, dbConfig.Schema, dbConfig.GetSchemaPrefixId());

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
        [InlineData(SupportedDatabaseTypes.Oracle, false, null, "INSERT INTO {1}Entity(Id, Name, Description) VALUES(seq.nextval, 'Arild', :Description)")]
        [InlineData(SupportedDatabaseTypes.Postgres, false, null, "INSERT INTO {1}Entity(Id, Name, Description) VALUES(nextval('seq'), 'Arild', @Description)")]
        [InlineData(SupportedDatabaseTypes.Oracle, true, null, "INSERT INTO {0}.{1}Entity(Id, Name, Description) VALUES(seq.nextval, 'Arild', :Description)")]
        [InlineData(SupportedDatabaseTypes.Postgres, true, null, "INSERT INTO {0}.{1}Entity(Id, Name, Description) VALUES(nextval('seq'), 'Arild', @Description)")]
        [InlineData(SupportedDatabaseTypes.Oracle, false, "En", "INSERT INTO {1}Entity(Id, Name, Description) VALUES(seq.nextval, 'Arild', :Description)")]
        [InlineData(SupportedDatabaseTypes.Postgres, false, "En", "INSERT INTO {1}Entity(Id, Name, Description) VALUES(nextval('seq'), 'Arild', @Description)")]
        [InlineData(SupportedDatabaseTypes.Oracle, true, "En", "INSERT INTO {0}.{1}Entity(Id, Name, Description) VALUES(seq.nextval, 'Arild', :Description)")]
        [InlineData(SupportedDatabaseTypes.Postgres, true, "En", "INSERT INTO {0}.{1}Entity(Id, Name, Description) VALUES(nextval('seq'), 'Arild', @Description)")]
        public void InsertTest2(SupportedDatabaseTypes databaseTypes, bool useSchema, string schemaPrefixId, string expectedSql)
        {
            var addDictionary = new Dictionary<string, string> { { "database:schemaPrefix:Id", schemaPrefixId } };
            using (var scope = TestServiceProvider.GetDatabaseExampleServiceProvider(databaseTypes, addDictionary).CreateScope())
            {
                var dbConfig = scope.ServiceProvider.GetService<IDbConfigDatabaseTargets>();
                expectedSql = string.Format(expectedSql, dbConfig.Schema, dbConfig.GetSchemaPrefixId());

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