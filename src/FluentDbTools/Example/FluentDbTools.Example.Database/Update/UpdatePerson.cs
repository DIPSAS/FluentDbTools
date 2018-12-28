using System.Data;
using System.Threading.Tasks;
using Dapper;
using FluentDbTools.Extensions.SqlBuilder;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Example.Database.Entities;

namespace FluentDbTools.Example.Database.Update
{
    public static class UpdatePerson
    {
        public static Task Execute(
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
            return dbConnection.ExecuteAsync(sql, @params);
        }
        
        private static string BuildSql(this IDbConfig dbConfig)
        {
            var sql = dbConfig.CreateSqlBuilder().Update<Person>()
                .OnSchema()
                .Fields(x => x.FP(item => item.Alive))
                .Fields(x => x.FP(item => item.Username))
                .Fields(x => x.FP(item => item.Password))
                .Where(x => x.WP(item => item.Id))
                .Build();
            return sql;
        }
    }
}