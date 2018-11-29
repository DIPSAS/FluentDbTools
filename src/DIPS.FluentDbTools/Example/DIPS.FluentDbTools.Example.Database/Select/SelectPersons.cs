using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using DIPS.Extensions.FluentDbTools.SqlBuilder;
using DIPS.FluentDbTools.Common.Abstractions;
using DIPS.FluentDbTools.Example.Database.Entities;
using DIPS.FluentDbTools.SqlBuilder.Abstractions.Common;

namespace DIPS.FluentDbTools.Example.Database.Select
{
    public static class SelectPersons
    {
        public static Task<IEnumerable<Person>> Execute(
            IDbConnection dbConnection,
            IDbConfig dbConfig,
            Guid[] ids)
        {
            var sql = dbConfig.BuildSql(ids, out var @params);
            return dbConnection.QueryAsync<Person>(sql, @params);
        }
        
        private static string BuildSql(this IDbConfig dbConfig, Guid[] ids, out DynamicParameters @params)
        {
            @params = new DynamicParameters();
            var inSelections = dbConfig.CreateParameterResolver().AddArrayParameter(@params, nameof(Person.Id), ids);
            var sql = dbConfig.CreateSqlBuilder().Select()
                .OnSchema()
                .Fields<Person>(x => x.F(item => item.Id))
                .Fields<Person>(x => x.F(item => item.SequenceNumber))
                .Fields<Person>(x => x.F(item => item.Username))
                .Fields<Person>(x => x.F(item => item.Password))
                .Where<Person>(x => x.WP(item => item.Id, inSelections))
                .Build();
                
            return sql;
        }
    }
}