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
        public async static Task<long> Execute(
            IDbConnection dbConnection,
            IDbConfigDatabaseTargets dbConfig)
        {
            var sql = dbConfig.BuildSql(out var @params);
            var res = await dbConnection.QuerySingleAsync<long>(sql, @params);
            return res;
        }
        
        private static string BuildSql(this IDbConfigDatabaseTargets dbConfig, out DynamicParameters @params)
        {
            @params = new DynamicParameters();
            @params.Add(nameof(Person.Alive), dbConfig.CreateParameterResolver().WithBooleanParameterValue(true));
            var sql = dbConfig.CreateSqlBuilder().Select()
                .OnSchema()
                .Count()
                .Fields<Person>(x => x.F(item => item.Id))
                .Where<Person>(x => x.WP(item => item.Alive))
                .Build();
                
            return sql;
        }
    }
}