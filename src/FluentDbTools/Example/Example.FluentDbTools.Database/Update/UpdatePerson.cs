using System.Data;
using System.Threading.Tasks;
using Dapper;
using FluentDbTools.Extensions.SqlBuilder;
using FluentDbTools.Common.Abstractions;
using Example.FluentDbTools.Database.Entities;
using FluentDbTools.Extensions.DbProvider;

namespace Example.FluentDbTools.Database.Update
{
    public static class UpdatePerson
    {
        public static Task Execute(
            IDbConnection dbConnection,
            IDbConfigDatabaseTargets dbConfig,
            Person person)
        {
            var sql = dbConfig.BuildSql();
            var @params = new DynamicParameters();
            @params.Add(nameof(Person.PersonId), dbConfig.CreateParameterResolver().WithGuidParameterValue(person.PersonId));
            @params.Add(nameof(Person.Alive), dbConfig.CreateParameterResolver().WithBooleanParameterValue(person.Alive));
            @params.Add(nameof(Person.Username), person.Username);
            @params.Add(nameof(Person.Password), person.Password);
            return dbConnection.ExecuteAsync(sql, @params);
        }
        
        private static string BuildSql(this IDbConfigDatabaseTargets dbConfig)
        {
            var sql = dbConfig.CreateSqlBuilder().Update<Person>()
                .OnSchema()
                .Fields(x => x.FP(item => item.Alive))
                .Fields(x => x.FP(item => item.Username))
                .Fields(x => x.FP(item => item.Password))
                .Where(x => x.WP(item => item.PersonId))
                .Build();
            return sql;
        }
    }
}