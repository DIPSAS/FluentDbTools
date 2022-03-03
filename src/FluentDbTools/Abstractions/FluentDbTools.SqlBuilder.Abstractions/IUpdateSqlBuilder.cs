using System;
using FluentDbTools.SqlBuilder.Abstractions.Fields;
#pragma warning disable CS1591

namespace FluentDbTools.SqlBuilder.Abstractions
{
    public interface IUpdateSqlBuilder<TClass> : ISqlFieldBuilder<TClass>
    {
        IUpdateSqlBuilder<TClass> OnTable(string tableName);
        IUpdateSqlBuilder<TClass> OnSchema(string schemaName = null, Func<bool> setSchemaNameIfExpressionIsEvaluatedToTrue = null);
        IUpdateSqlBuilder<TClass> Fields(Action<IFieldSetterSelector<TClass>> selector);
        IUpdateSqlBuilder<TClass> Where(Action<IWhereFieldSelector<TClass>> selector);
        IUpdateSqlBuilder<TClass> WhereIf(Action<IWhereFieldSelector<TClass>> selector, Func<bool> statement);
    }
}