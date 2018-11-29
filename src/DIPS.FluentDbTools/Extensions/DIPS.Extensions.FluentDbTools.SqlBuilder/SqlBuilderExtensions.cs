using DIPS.FluentDbTools.Common.Abstractions;
using DIPS.FluentDbTools.SqlBuilder.Abstractions;
using DIPS.FluentDbTools.SqlBuilder.Abstractions.Parameters;
using DIPS.FluentDbTools.SqlBuilder.Parameters;
using DIPS.FluentDbTools.SqlBuilder.TypeHandlers;

namespace DIPS.Extensions.FluentDbTools.SqlBuilder
{
    public static class SqlBuilderExtensions
    {
        static SqlBuilderExtensions()
        {
            TypeHandlerRegistration.RegisterTypeHandlers();
        }
        
        public static ISqlBuilder CreateSqlBuilder(this IDbConfig dbConfig)
        {
            return new DIPS.FluentDbTools.SqlBuilder.SqlBuilder(dbConfig);
        }

        public static IDatabaseParameterResolver CreateParameterResolver(this IDbConfig dbConfig)
        {
            return new DatabaseParameterResolver(dbConfig);
        }
    }
}