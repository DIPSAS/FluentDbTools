using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using DIPS.Extensions.FluentDbTools.SqlBuilder;
using DIPS.FluentDbTools.Common.Abstractions;
using DIPS.FluentDbTools.Example.Database.Entities;

namespace DIPS.FluentDbTools.Example.Database.Select
{
    public static class SelectPerson
    {
        public static Task<Person> Execute(
            IDbConnection dbConnection,
            IDbConfig dbConfig,
            Guid id)
        {
            var sql = dbConfig.BuildSql();
            var @params = new DynamicParameters();
            @params.Add(nameof(Person.Id), dbConfig.CreateParameterResolver().WithGuidParameterValue(id));
            return dbConnection.QuerySingleAsync<Person>(sql, @params);
        }
        
        private static string BuildSql(this IDbConfig dbConfig)
        {
            var sql = dbConfig.CreateSqlBuilder().Select()
                .OnSchema()
                .Fields<Person>(x => x.F(item => item.Id))
                .Fields<Person>(x => x.F(item => item.SequenceNumber))
                .Fields<Person>(x => x.F(item => item.Username))
                .Fields<Person>(x => x.F(item => item.Password))
                .Where<Person>(x => x.WP(item => item.Id))
                .Build();
            
            return sql;
        }
    }
}