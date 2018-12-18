using System;
using System.Linq.Expressions;
using FluentDbTools.SqlBuilder.Abstractions.Common;

namespace FluentDbTools.SqlBuilder.Abstractions.Fields
{
    public interface IWhereFieldSelector<TClass>
    {
        /// <summary>
        /// Where with Field [operator] parameter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="paramName"></param>
        /// <param name="whereOperator"></param>
        /// <returns></returns>
        IWhereFieldSelector<TClass> WP<T>(Expression<Func<TClass, T>> field, string paramName = null, OP whereOperator = OP.EQ);

        IWhereFieldSelector<TClass> WP(string field, string paramName = null, OP whereOperator = OP.EQ);
        
        /// <summary>
        /// Where with Field [operator] parameter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="paramName"></param>
        /// <param name="whereOperator"></param>
        /// <returns></returns>
        IWhereFieldSelector<TClass> WP<T>(Expression<Func<TClass, T>> field, string[] paramNames, OP whereOperator = OP.IN);
        
        IWhereFieldSelector<TClass> WP<T>(string field, string[] paramNames, OP whereOperator = OP.IN);

        /// <summary>
        /// Where with Field [operator] value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="whereOperator"></param>
        /// <returns></returns>
        IWhereFieldSelector<TClass> WV<T, TValue>(Expression<Func<TClass, T>> field, TValue value, OP whereOperator = OP.EQ, bool ignoreFormat = false);

        IWhereFieldSelector<TClass> WV<TValue>(string field, TValue value, OP whereOperator = OP.EQ, bool ignoreFormat = false);
    }
}