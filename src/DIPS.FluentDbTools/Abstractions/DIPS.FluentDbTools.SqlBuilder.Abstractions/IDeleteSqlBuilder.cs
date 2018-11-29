using System;
using DIPS.FluentDbTools.SqlBuilder.Abstractions.Common;
using DIPS.FluentDbTools.SqlBuilder.Abstractions.Fields;

namespace DIPS.FluentDbTools.SqlBuilder.Abstractions
{
    public interface IDeleteSqlBuilder<TClass> : ISqlBuildOnly
    {
        IDeleteSqlBuilder<TClass> OnSchema(string schemaName = null, Func<bool> setSchemaNameIfExpressionIsEvaluatedToTrue = null);
        IDeleteSqlBuilder<TClass> Where(Action<IWhereFieldSelector<TClass>> selector);
        IDeleteSqlBuilder<TClass> WhereIf(Action<IWhereFieldSelector<TClass>> selector, Func<bool> statement);
    }
}