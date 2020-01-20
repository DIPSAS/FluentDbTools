using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using FluentDbTools.Extensions.SqlBuilder;
using FluentDbTools.Common.Abstractions;
using Example.FluentDbTools.Database.Entities;
using FluentDbTools.SqlBuilder.Abstractions.Common;
using FluentDbTools.SqlBuilder.Parameters;
namespace Example.FluentDbTools.Database.Select
{
    public static class SelectPersons
    {
        public static async Task<IEnumerable<Person>> Execute(
            IDbConnection dbConnection,
            IDbConfigSchemaTargets dbConfigConfig,
            Guid[] ids)
        {
            var sql = dbConfigConfig.BuildSql(ids, out var @params);
            var res = await dbConnection.QueryAsync<Person>(sql, @params);
            return res;
        }
        
        private static string BuildSql(this IDbConfigSchemaTargets dbConfigConfig, Guid[] ids, out DynamicParameters @params)
        {
            @params = new DynamicParameters();

            var inSelections = dbConfigConfig.DatabaseParameterResolver().AddArrayParameter(@params, nameof(Person.PersonId), ids);
            var sql = dbConfigConfig.SqlBuilder().Select()
                .OnSchema()
                .Fields<Person>(x => x.F(item => item.PersonId))
                .Fields<Person>(x => x.F(item => item.SequenceNumber))
                .Fields<Person>(x => x.F(item => item.Alive))
                .Fields<Person>(x => x.F(item => item.Username))
                .Fields<Person>(x => x.F(item => item.Password))
                .Where<Person>(x => x.WP(item => item.PersonId, inSelections))
                .Build();
                
            return sql;
        }
    }
}