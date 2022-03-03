using System;
using FluentDbTools.SqlBuilder.Abstractions.Common;
using FluentDbTools.SqlBuilder.Abstractions.Fields;
#pragma warning disable CS1591

namespace FluentDbTools.SqlBuilder.Abstractions
{
    public interface IDeleteSqlBuilder<TClass> : ISqlBuildOnly
    {
        IDeleteSqlBuilder<TClass> OnTable(string tableName);
        IDeleteSqlBuilder<TClass> OnSchema(string schemaName = null, Func<bool> setSchemaNameIfExpressionIsEvaluatedToTrue = null);
        IDeleteSqlBuilder<TClass> Where(Action<IWhereFieldSelector<TClass>> selector);
        IDeleteSqlBuilder<TClass> WhereIf(Action<IWhereFieldSelector<TClass>> selector, Func<bool> statement);
    }
}