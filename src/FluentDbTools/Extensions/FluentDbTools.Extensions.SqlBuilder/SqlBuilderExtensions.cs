using FluentDbTools.Common.Abstractions;
using FluentDbTools.SqlBuilder.Abstractions;
using FluentDbTools.SqlBuilder.Abstractions.Parameters;
using FluentDbTools.SqlBuilder.Parameters;
using FluentDbTools.SqlBuilder.TypeHandlers;

namespace FluentDbTools.Extensions.SqlBuilder
{
    public static class SqlBuilderExtensions
    {
        static SqlBuilderExtensions()
        {
            TypeHandlerRegistration.RegisterTypeHandlers();
        }
        
        public static ISqlBuilder CreateSqlBuilder(this IDbConfig dbConfig)
        {
            return new FluentDbTools.SqlBuilder.SqlBuilder(dbConfig);
        }

        public static IDatabaseParameterResolver CreateParameterResolver(this IDbConfig dbConfig)
        {
            return new DatabaseParameterResolver(dbConfig);
        }
    }
}