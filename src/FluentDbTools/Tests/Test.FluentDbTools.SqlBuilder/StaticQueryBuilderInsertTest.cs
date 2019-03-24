using FluentDbTools.Extensions.SqlBuilder;
using FluentDbTools.Common.Abstractions;
using TestUtilities.FluentDbTools;
using FluentAssertions;
using Test.FluentDbTools.SqlBuilder.TestEntities;
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
        public void InsertTest(SupportedDatabaseTypes databaseTypes, string schema, string expectedSql)
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
        [InlineData(SupportedDatabaseTypes.Oracle, null, "INSERT INTO EntityTable(Id, Name, Description) VALUES(:IdParam, 'Arild', :Description)")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, "INSERT INTO EntityTable(Id, Name, Description) VALUES(@IdParam, 'Arild', @Description)")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", "INSERT INTO {0}.EntityTable(Id, Name, Description) VALUES(:IdParam, 'Arild', :Description)")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", "INSERT INTO {0}.EntityTable(Id, Name, Description) VALUES(@IdParam, 'Arild', @Description)")]
        public void InsertTest_WithTableNameSet(SupportedDatabaseTypes databaseTypes, string schema, string expectedSql)
        {
            const string tableName = "EntityTable";
            var useSchema = !string.IsNullOrEmpty(schema);

            var dbConfig = OverrideConfig.CreateTestDbConfig(databaseTypes, schema);
            expectedSql = string.Format(expectedSql, dbConfig.Schema);

            var builder = dbConfig.CreateSqlBuilder();
            var insert = builder.Insert<Entity>();

            var sql = insert
                .OnTable(tableName)
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
        public void InsertTest_WithSequence(SupportedDatabaseTypes databaseTypes, string schema, string expectedSql)
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
        
        [Theory]
        [InlineData(SupportedDatabaseTypes.Oracle, null, "INSERT INTO Entity(Id, Name, Description, EntityEnum, SomeProperty) VALUES(seq.nextval, 'Arild', :Description, :EntityEnum, :SomeProperty)")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, "INSERT INTO Entity(Id, Name, Description, EntityEnum, SomeProperty) VALUES(nextval('seq'), 'Arild', @Description, @EntityEnum, @SomeProperty)")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", "INSERT INTO {0}.Entity(Id, Name, Description, EntityEnum, SomeProperty) VALUES(seq.nextval, 'Arild', :Description, :EntityEnum, :SomeProperty)")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", "INSERT INTO {0}.Entity(Id, Name, Description, EntityEnum, SomeProperty) VALUES(nextval('seq'), 'Arild', @Description, @EntityEnum, @SomeProperty)")]
        public void InsertTest_WithEnumAndDirectProperty(SupportedDatabaseTypes databaseTypes, string schema, string expectedSql)
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
                .Fields(x => x.FP(f => f.EntityEnum))
                .Fields(x => x.FP("SomeProperty"))
                .Build();

            sql.Should().Be(expectedSql);
        }
    }
}