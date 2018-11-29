using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using DIPS.Extensions.FluentDbTools.SqlBuilder;
using DIPS.FluentDbTools.Common.Abstractions;
using DIPS.FluentDbTools.Example.Database.Entities;

namespace DIPS.FluentDbTools.Example.Database.Delete
{
    public static class DeletePerson
    {
        public static Task Execute(
            IDbConnection dbConnection,
            IDbConfig dbConfig,
            Guid id)
        {
            var sql = dbConfig.BuildSql();
            var @params = new DynamicParameters();
            @params.Add(nameof(Person.Id), dbConfig.CreateParameterResolver().WithGuidParameterValue(id));
            return dbConnection.ExecuteAsync(sql, @params);
        }
        
        private static string BuildSql(this IDbConfig dbConfig)
        {
            var sql = dbConfig.CreateSqlBuilder().Delete<Person>()
                .OnSchema()
                .Where(x => x.WP(item => item.Id))
                .Build();
            return sql;
        }
    }
}