using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using FluentDbTools.Extensions.SqlBuilder;
using FluentDbTools.Common.Abstractions;
using Example.FluentDbTools.Database.Entities;

namespace Example.FluentDbTools.Database.Select
{
    public static class SelectPersonsCount
    {
        public static async Task<long> Execute(
            IDbConnection dbConnection,
            IDbConfigSchemaTargets dbConfigConfig)
        {
            var sql = dbConfigConfig.BuildSql(out var @params);
            var res = await dbConnection.QuerySingleAsync<long>(sql, @params);
            return res;
        }
        
        private static string BuildSql(this IDbConfigSchemaTargets dbConfigConfig, out DynamicParameters @params)
        {
            @params = new DynamicParameters();
            @params.Add(nameof(Person.Alive), dbConfigConfig.DatabaseParameterResolver().WithBooleanParameterValue(true));
            var sql = dbConfigConfig.SqlBuilder().Select()
                .OnSchema()
                .Count()
                .Fields<Person>(x => x.F(item => item.PersonId))
                .Where<Person>(x => x.WP(item => item.Alive))
                .Build();
                
            return sql;
        }
    }
}