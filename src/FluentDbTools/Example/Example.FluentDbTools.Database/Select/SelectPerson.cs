using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using FluentDbTools.Extensions.SqlBuilder;
using FluentDbTools.Common.Abstractions;
using Example.FluentDbTools.Database.Entities;

namespace Example.FluentDbTools.Database.Select
{
    public static class SelectPerson
    {
        public static Task<Person> Execute(
            IDbConnection dbConnection,
            IDbConfigDatabaseTargets dbConfig,
            Guid id)
        {
            var sql = dbConfig.BuildSql();
            var @params = new DynamicParameters();
            @params.Add(nameof(Person.PersonId), dbConfig.CreateParameterResolver().WithGuidParameterValue(id));
            return dbConnection.QuerySingleAsync<Person>(sql, @params);
        }
        
        private static string BuildSql(this IDbConfigDatabaseTargets dbConfig)
        {
            var sql = dbConfig.CreateSqlBuilder().Select()
                .OnSchema()
                .Fields<Person>(x => x.F(item => item.PersonId))
                .Fields<Person>(x => x.F(item => item.SequenceNumber))
                .Fields<Person>(x => x.F(item => item.Alive))
                .Fields<Person>(x => x.F(item => item.Username))
                .Fields<Person>(x => x.F(item => item.Password))
                .Where<Person>(x => x.WP(item => item.PersonId))
                .Build();
            
            return sql;
        }
    }
}