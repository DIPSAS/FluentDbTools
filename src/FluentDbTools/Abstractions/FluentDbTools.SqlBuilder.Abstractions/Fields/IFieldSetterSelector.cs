using System;
using System.Linq.Expressions;

namespace FluentDbTools.SqlBuilder.Abstractions.Fields
{
    public interface IFieldSetterSelector<TClass>
    {
        string TableName { get; }
        string SchemaName { get; }
        
        IFieldSetterSelector<TClass> FP<T>(Expression<Func<TClass, T>> field, string param = null);

        IFieldSetterSelector<TClass> FP(string field, string param = null);

        IFieldSetterSelector<TClass> FV<T, TValue>(Expression<Func<TClass, T>> field, TValue value, bool ignoreFormat = false);

        IFieldSetterSelector<TClass> FV<TValue>(string field, TValue value, bool ignoreFormat = false);
    }
}