using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using FluentDbTools.Extensions.SqlBuilder;
using FluentDbTools.Common.Abstractions;
using Example.FluentDbTools.Database.Entities;

namespace Example.FluentDbTools.Database.Delete
{
    public static class DeletePerson
    {
        public static Task Execute(
            IDbConnection dbConnection,
            IDbConfigSchemaTargets dbConfigConfig,
            Guid id)
        {
            var sql = dbConfigConfig.BuildSql();
            var @params = new DynamicParameters();
            @params.Add(nameof(Person.PersonId), dbConfigConfig.DatabaseParameterResolver().WithGuidParameterValue(id));
            return dbConnection.ExecuteAsync(sql, @params);
        }
        
        private static string BuildSql(this IDbConfigSchemaTargets dbConfigConfig)
        {
            var sql = dbConfigConfig.SqlBuilder().Delete<Person>()
                .OnSchema()
                .Where(x => x.WP(item => item.PersonId))
                .Build();
            return sql;
        }
    }
}