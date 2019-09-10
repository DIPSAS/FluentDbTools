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
    public class QueryBuilderDeleteTest
    {      
        [Theory]
        [InlineData(SupportedDatabaseTypes.Oracle, false, null, "DELETE FROM {1}Entity WHERE Id = :IdParam AND Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, false, null, "DELETE FROM {1}Entity WHERE Id = @IdParam AND Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, true, null, "DELETE FROM {0}.{1}Entity WHERE Id = :IdParam AND Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, true, null, "DELETE FROM {0}.{1}Entity WHERE Id = @IdParam AND Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, false, "PR", "DELETE FROM {1}Entity WHERE Id = :IdParam AND Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, false, "PR", "DELETE FROM {1}Entity WHERE Id = @IdParam AND Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Oracle, true, "PR", "DELETE FROM {0}.{1}Entity WHERE Id = :IdParam AND Name = 'Arild'")]
        [InlineData(SupportedDatabaseTypes.Postgres, true, "PR", "DELETE FROM {0}.{1}Entity WHERE Id = @IdParam AND Name = 'Arild'")]
        public void DeleteTest1(SupportedDatabaseTypes databaseTypes, bool useSchema, string schemaPrefixId, string expectedSql)
        {
            var addDictionary = new Dictionary<string, string> {{"database:schemaPrefix:Id", schemaPrefixId}};
            using (var scope = TestServiceProvider.GetDatabaseExampleServiceProvider(databaseTypes, addDictionary).CreateScope())
            {
                var dbConfig = scope.ServiceProvider.GetService<IDbConfigDatabaseTargets>();
                expectedSql = string.Format(expectedSql, dbConfig.Schema, dbConfig.GetSchemaPrefixId());
                
                var builder = dbConfig.CreateSqlBuilder();
                var delete = builder.Delete<Entity>();

                var sql = delete
                    .OnSchema(setSchemaNameIfExpressionIsEvaluatedToTrue: () => useSchema)
                    .Where(x => x.WP(item => item.Id, "IdParam"))
                    .Where(x => x.WV(item => item.Name, "Arild"))
                    .Build();

                sql.Should().Be(expectedSql);
            }
        }
    }
}