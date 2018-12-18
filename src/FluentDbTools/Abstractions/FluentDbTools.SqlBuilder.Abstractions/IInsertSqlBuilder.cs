using System;
using FluentDbTools.SqlBuilder.Abstractions.Fields;

namespace FluentDbTools.SqlBuilder.Abstractions
{
    public interface IInsertSqlBuilder<TClass> : ISqlFieldBuilder<TClass>
    {
        IInsertSqlBuilder<TClass> OnSchema(string schemaName = null, Func<bool> setSchemaNameIfExpressionIsEvaluatedToTrue = null);
        IInsertSqlBuilder<TClass> Fields(Action<IFieldSetterSelector<TClass>> selector);
    }
}