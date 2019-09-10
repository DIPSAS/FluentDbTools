using System;
using System.Linq.Expressions;

namespace FluentDbTools.SqlBuilder.Abstractions.Fields
{
    /// <summary>
    /// Generic interface for Fields (select fields)
    /// </summary>
    /// <typeparam name="TClass"></typeparam>
    public interface IFieldSetterSelector<TClass>
    {
        /// <summary>
        /// The table name field is connected to
        /// </summary>
        string TableName { get; }

        /// <summary>
        /// The Schema owner to then <see cref="TableName"/>
        /// </summary>
        string SchemaName { get; }
        
        /// <summary>
        /// Field Parameter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        IFieldSetterSelector<TClass> FP<T>(Expression<Func<TClass, T>> field, string param = null);

        IFieldSetterSelector<TClass> FP(string field, string param = null);

        IFieldSetterSelector<TClass> FV<T, TValue>(Expression<Func<TClass, T>> field, TValue value, bool ignoreFormat = false);

        IFieldSetterSelector<TClass> FV<TValue>(string field, TValue value, bool ignoreFormat = false);
    }
}