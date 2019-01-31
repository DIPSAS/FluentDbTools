using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using FluentDbTools.Extensions.SqlBuilder;
using FluentDbTools.Common.Abstractions;
using Example.FluentDbTools.Database.Entities;

namespace Example.FluentDbTools.Database.Insert
{
    public static class InsertPerson
    {
        public async static Task Execute(
            IDbConnection dbConnection,
            IDbConfig dbConfig,
            Person person)
        {
            var sql = dbConfig.BuildSql();
            var @params = new DynamicParameters();
            @params.Add(nameof(Person.Id), dbConfig.CreateParameterResolver().WithGuidParameterValue(person.Id));
            @params.Add(nameof(Person.Alive), dbConfig.CreateParameterResolver().WithBooleanParameterValue(person.Alive));
            @params.Add(nameof(Person.Username), person.Username);
            @params.Add(nameof(Person.Password), person.Password);
            await dbConnection.ExecuteAsync(sql, @params);
        }
        
        private static string BuildSql(this IDbConfig dbConfig)
        {
            var parameterResolver = dbConfig.CreateParameterResolver();
            var sql = dbConfig.CreateSqlBuilder().Insert<Person>()
                .OnSchema()
                .Fields(x => x.FP(item => item.Id))
                .Fields(x => x.FV(item => item.SequenceNumber, parameterResolver.WithNextTableSequence<Person>(), ignoreFormat: true))
                .Fields(x => x.FP(item => item.Alive))
                .Fields(x => x.FP(item => item.Username))
                .Fields(x => x.FP(item => item.Password))
                .Build();
            return sql;
        }
    }
}