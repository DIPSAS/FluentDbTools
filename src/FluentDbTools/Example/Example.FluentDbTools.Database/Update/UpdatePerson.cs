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
            IDbConfigSchemaTargets dbConfigConfig,
            Person person)
        {
            var sql = dbConfigConfig.BuildSql();
            var @params = new DynamicParameters();
            @params.Add(nameof(Person.PersonId), dbConfigConfig.CreateDatabaseParameterResolver().WithGuidParameterValue(person.PersonId));
            @params.Add(nameof(Person.Alive), dbConfigConfig.CreateDatabaseParameterResolver().WithBooleanParameterValue(person.Alive));
            @params.Add(nameof(Person.Username), person.Username);
            @params.Add(nameof(Person.Password), person.Password);
            return dbConnection.ExecuteAsync(sql, @params);
        }
        
        private static string BuildSql(this IDbConfigSchemaTargets dbConfigConfig)
        {
            var sql = dbConfigConfig.CreateSqlBuilder().Update<Person>()
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