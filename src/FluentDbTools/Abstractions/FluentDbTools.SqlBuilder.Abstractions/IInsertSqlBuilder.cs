using System;
using FluentDbTools.SqlBuilder.Abstractions.Fields;
#pragma warning disable CS1591

namespace FluentDbTools.SqlBuilder.Abstractions
{
    public interface IInsertSqlBuilder<TClass> : ISqlFieldBuilder<TClass>
    {
        IInsertSqlBuilder<TClass> OnTable(string tableName);
        IInsertSqlBuilder<TClass> OnSchema(string schemaName = null, Func<bool> setSchemaNameIfExpressionIsEvaluatedToTrue = null);
        IInsertSqlBuilder<TClass> Fields(Action<IFieldSetterSelector<TClass>> selector);
    }
}