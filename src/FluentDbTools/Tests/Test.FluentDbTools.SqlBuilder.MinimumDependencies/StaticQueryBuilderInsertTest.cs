using FluentDbTools.Common.Abstractions;
using FluentAssertions;
using FluentDbTools.SqlBuilder;
using Test.FluentDbTools.SqlBuilder.MinimumDependencies.TestEntities;
using Xunit;

namespace Test.FluentDbTools.SqlBuilder.MinimumDependencies
{
    public class StaticQueryBuilderInsertTest
    {
        [Theory]
        [InlineData(SupportedDatabaseTypes.Oracle, null, null, "INSERT INTO {1}Entity(Id, Name, Description) VALUES(:IdParam, 'Arild', :Description)")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, null, "INSERT INTO {1}Entity(Id, Name, Description) VALUES(@IdParam, 'Arild', @Description)")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", null, "INSERT INTO {0}.{1}Entity(Id, Name, Description) VALUES(:IdParam, 'Arild', :Description)")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", null, "INSERT INTO {0}.{1}Entity(Id, Name, Description) VALUES(@IdParam, 'Arild', @Description)")]
        [InlineData(SupportedDatabaseTypes.Oracle, null, "En", "INSERT INTO {1}Entity(Id, Name, Description) VALUES(:IdParam, 'Arild', :Description)")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, "En", "INSERT INTO {1}Entity(Id, Name, Description) VALUES(@IdParam, 'Arild', @Description)")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", "En", "INSERT INTO {0}.{1}Entity(Id, Name, Description) VALUES(:IdParam, 'Arild', :Description)")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", "En", "INSERT INTO {0}.{1}Entity(Id, Name, Description) VALUES(@IdParam, 'Arild', @Description)")]
        public void InsertTest(SupportedDatabaseTypes databaseTypes, string schema, string schemaPrefixId, string expectedSql)
        {
            var useSchema = !string.IsNullOrEmpty(schema);

            var dbConfig = SqlBuilderFactory.CreateDbConfigSchemaTargets(schema, schemaPrefixId, databaseTypes);
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

        [Theory]
        [InlineData(SupportedDatabaseTypes.Oracle, null, null, "INSERT INTO {1}EntityTable(Id, Name, Description) VALUES(:IdParam, 'Arild', :Description)")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, null, "INSERT INTO {1}EntityTable(Id, Name, Description) VALUES(@IdParam, 'Arild', @Description)")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", null, "INSERT INTO {0}.{1}EntityTable(Id, Name, Description) VALUES(:IdParam, 'Arild', :Description)")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", null, "INSERT INTO {0}.{1}EntityTable(Id, Name, Description) VALUES(@IdParam, 'Arild', @Description)")]
        [InlineData(SupportedDatabaseTypes.Oracle, null, "En", "INSERT INTO {1}EntityTable(Id, Name, Description) VALUES(:IdParam, 'Arild', :Description)")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, "En", "INSERT INTO {1}EntityTable(Id, Name, Description) VALUES(@IdParam, 'Arild', @Description)")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", "En", "INSERT INTO {0}.{1}EntityTable(Id, Name, Description) VALUES(:IdParam, 'Arild', :Description)")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", "En", "INSERT INTO {0}.{1}EntityTable(Id, Name, Description) VALUES(@IdParam, 'Arild', @Description)")]
        public void InsertTest_WithTableNameSet(SupportedDatabaseTypes databaseTypes, string schema, string schemaPrefixId, string expectedSql)
        {
            const string tableName = "EntityTable";
            var useSchema = !string.IsNullOrEmpty(schema);

            var dbConfig = SqlBuilderFactory.CreateDbConfigSchemaTargets(schema, schemaPrefixId, databaseTypes);
            expectedSql = string.Format(expectedSql, dbConfig.Schema, dbConfig.GetSchemaPrefixId());

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
        [InlineData(SupportedDatabaseTypes.Oracle, null, null, "INSERT INTO {1}Entity(Id, Name, Description) VALUES(seq.nextval, 'Arild', :Description)")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, null, "INSERT INTO {1}Entity(Id, Name, Description) VALUES(nextval('seq'), 'Arild', @Description)")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", null, "INSERT INTO {0}.{1}Entity(Id, Name, Description) VALUES(seq.nextval, 'Arild', :Description)")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", null, "INSERT INTO {0}.{1}Entity(Id, Name, Description) VALUES(nextval('seq'), 'Arild', @Description)")]
        [InlineData(SupportedDatabaseTypes.Oracle, null, "pr1", "INSERT INTO {1}Entity(Id, Name, Description) VALUES(seq.nextval, 'Arild', :Description)")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, "pr1", "INSERT INTO {1}Entity(Id, Name, Description) VALUES(nextval('seq'), 'Arild', @Description)")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", "pr1", "INSERT INTO {0}.{1}Entity(Id, Name, Description) VALUES(seq.nextval, 'Arild', :Description)")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", "pr1", "INSERT INTO {0}.{1}Entity(Id, Name, Description) VALUES(nextval('seq'), 'Arild', @Description)")]
        public void InsertTest_WithSequence(SupportedDatabaseTypes databaseTypes, string schema, string schemaPrefixId, string expectedSql)
        {
            var useSchema = !string.IsNullOrEmpty(schema);

            var dbConfig = SqlBuilderFactory.CreateDbConfigSchemaTargets(schema, schemaPrefixId, databaseTypes);
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

        [Theory]
        [InlineData(SupportedDatabaseTypes.Oracle, null, null, "INSERT INTO {1}Entity(Id, Name, Description, EntityEnum, SomeProperty) VALUES(seq.nextval, 'Arild', :Description, :EntityEnum, :SomeProperty)")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, null, "INSERT INTO {1}Entity(Id, Name, Description, EntityEnum, SomeProperty) VALUES(nextval('seq'), 'Arild', @Description, @EntityEnum, @SomeProperty)")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", null, "INSERT INTO {0}.{1}Entity(Id, Name, Description, EntityEnum, SomeProperty) VALUES(seq.nextval, 'Arild', :Description, :EntityEnum, :SomeProperty)")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", null, "INSERT INTO {0}.{1}Entity(Id, Name, Description, EntityEnum, SomeProperty) VALUES(nextval('seq'), 'Arild', @Description, @EntityEnum, @SomeProperty)")]
        [InlineData(SupportedDatabaseTypes.Oracle, null, "pr", "INSERT INTO {1}Entity(Id, Name, Description, EntityEnum, SomeProperty) VALUES(seq.nextval, 'Arild', :Description, :EntityEnum, :SomeProperty)")]
        [InlineData(SupportedDatabaseTypes.Postgres, null, "pr", "INSERT INTO {1}Entity(Id, Name, Description, EntityEnum, SomeProperty) VALUES(nextval('seq'), 'Arild', @Description, @EntityEnum, @SomeProperty)")]
        [InlineData(SupportedDatabaseTypes.Oracle, "schema", "pr", "INSERT INTO {0}.{1}Entity(Id, Name, Description, EntityEnum, SomeProperty) VALUES(seq.nextval, 'Arild', :Description, :EntityEnum, :SomeProperty)")]
        [InlineData(SupportedDatabaseTypes.Postgres, "schema", "pr", "INSERT INTO {0}.{1}Entity(Id, Name, Description, EntityEnum, SomeProperty) VALUES(nextval('seq'), 'Arild', @Description, @EntityEnum, @SomeProperty)")]
        public void InsertTest_WithEnumAndDirectProperty(SupportedDatabaseTypes databaseTypes, string schema, string schemaPrefixId, string expectedSql)
        {
            var useSchema = !string.IsNullOrEmpty(schema);
            var dbConfig = SqlBuilderFactory.CreateDbConfigSchemaTargets(schema, schemaPrefixId, databaseTypes);
            expectedSql = string.Format(expectedSql, dbConfig.Schema, dbConfig.GetSchemaPrefixId());
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