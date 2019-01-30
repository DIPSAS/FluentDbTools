using FluentDbTools.Extensions.SqlBuilder;
using FluentDbTools.Common.Abstractions;
using TestUtilities.FluentDbTools;
using FluentAssertions;
using FTest.FluentDbTools.SqlBuilderTestEntities;
using Xunit;

namespace Test.FluentDbTools.SqlBuilder
{
    public class StaticQueryBuilderInsertTest
    {
        [Theory]
        [InlineData(SupportedDatabaseTypes.Oracle, null, "INSERT INTO Entity(Id, Name, Description) VALUES(:IdParam, 'Arild', :Description)")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, "INSERT INTO Entity(Id, Name, Description) VALUES(@IdParam, 'Arild', @Description)")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", "INSERT INTO {0}.Entity(Id, Name, Description) VALUES(:IdParam, 'Arild', :Description)")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", "INSERT INTO {0}.Entity(Id, Name, Description) VALUES(@IdParam, 'Arild', @Description)")]
        public void InsertTest1(SupportedDatabaseTypes databaseTypes, string schema, string expectedSql)
        {
            var useSchema = !string.IsNullOrEmpty(schema);

            var dbConfig = OverrideConfig.CreateTestDbConfig(databaseTypes, schema);
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

        [Theory]
        [InlineData(SupportedDatabaseTypes.Oracle, null, "INSERT INTO Entity(Id, Name, Description) VALUES(seq.nextval, 'Arild', :Description)")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, "INSERT INTO Entity(Id, Name, Description) VALUES(nextval('seq'), 'Arild', @Description)")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", "INSERT INTO {0}.Entity(Id, Name, Description) VALUES(seq.nextval, 'Arild', :Description)")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", "INSERT INTO {0}.Entity(Id, Name, Description) VALUES(nextval('seq'), 'Arild', @Description)")]
        public void InsertTest2(SupportedDatabaseTypes databaseTypes, string schema, string expectedSql)
        {
            var useSchema = !string.IsNullOrEmpty(schema);

            var dbConfig = OverrideConfig.CreateTestDbConfig(databaseTypes, schema);
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