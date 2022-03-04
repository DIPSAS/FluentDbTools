﻿using System;
using FluentDbTools.SqlBuilder.Abstractions.Common;
using FluentDbTools.SqlBuilder.Abstractions.Fields;
#pragma warning disable CS1591

namespace FluentDbTools.SqlBuilder.Abstractions
{
    public interface ISelectSqlBuilder : ISqlBuildOnly
    {
        /// <summary>
        /// Count
        /// </summary>
        /// <returns type="ISelectSqlBuilder"></returns>
        ISelectSqlBuilder Count();

        ISelectSqlBuilder OnSchema(string schemaName = null, Func<bool> setSchemaNameIfExpressionIsEvaluatedToTrue = null);

        ISelectSqlBuilder Fields<T>(Action<ISelectFieldSelector<T>> selector, string tableName = null, string tableAlias = null);

        /// <summary>
        /// Where
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selector"></param>
        /// <param name="tableAlias"></param>
        /// <returns type="ISelectSqlBuilder"></returns>
        //        ISelectSqlBuilder Where<T>(Action<IWhereFieldSelector<T>> selector);
        ISelectSqlBuilder Where<T>(Action<IWhereFieldSelector<T>> selector, string tableAlias = null);

        /// <summary>
        /// Where
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selector"></param>
        /// <param name="statement"></param>
        /// <param name="tableAlias"></param>
        /// <returns type="ISelectSqlBuilder"></returns>
        //        ISelectSqlBuilder WhereIf<T>(Action<IWhereFieldSelector<T>> selector, Func<bool> statement);
        ISelectSqlBuilder WhereIf<T>(Action<IWhereFieldSelector<T>> selector, Func<bool> statement, string tableAlias = null);


        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTo"></typeparam>
        /// <param name="fromField"></param>
        /// <param name="toField"></param>
        /// <param name="toTableAlias"></param>
        /// <param name="fromTableAlias"></param>
        /// <param name="fromTableName"></param>
        /// <param name="toTableName"></param>
        /// <returns type="ISelectSqlBuilder"></returns>
        ISelectSqlBuilder InnerJoin<TFrom, TTo>(
            string fromField = null,
            string toField = null,
            string toTableAlias = null,
            string fromTableAlias = null,
            string fromTableName = null,
            string toTableName = null);

        /// <summary>
        /// Outer Join
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTo"></typeparam>
        /// <param name="fromField"></param>
        /// <param name="toField"></param>
        /// <param name="toTableAlias"></param>
        /// <param name="fromTableAlias"></param>
        /// <param name="fromTableName"></param>
        /// <param name="toTableName"></param>
        /// <returns type="ISelectSqlBuilder"></returns>
        ISelectSqlBuilder LeftOuterJoin<TFrom, TTo>(
            string fromField = null,
            string toField = null,
            string toTableAlias = null,
            string fromTableAlias = null,
            string fromTableName = null,
            string toTableName = null);

        /// <summary>
        /// From table
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <param name="fromTableName"></param>
        /// <param name="tableAlias"></param>
        /// <returns type="ISelectSqlBuilder"></returns>
        ISelectSqlBuilder From<TFrom>(string fromTableName = null, string tableAlias = null);
    }
}