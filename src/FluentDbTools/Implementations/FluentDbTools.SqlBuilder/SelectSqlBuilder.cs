using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.SqlBuilder.Abstractions;
using FluentDbTools.SqlBuilder.Abstractions.Common;
using FluentDbTools.SqlBuilder.Abstractions.Fields;
using FluentDbTools.SqlBuilder.Abstractions.Parameters;
using FluentDbTools.SqlBuilder.Common;

namespace FluentDbTools.SqlBuilder
{
    internal class SelectSqlBuilder : ISelectSqlBuilder
    {
        private readonly IDbConfig DbConfig;
        private readonly List<string> FieldsList = new List<string>();

        private readonly List<string> Joins = new List<string>();

        private readonly List<string> Wheres = new List<string>();

        private string TableName;

        private string TableAlias;
        private string SchemaName;
        private bool SelectCount = false;

        private string SchemaNamePrefix => string.IsNullOrEmpty(SchemaName) ? string.Empty : $"{SchemaName}.";


        public SelectSqlBuilder(IDbConfig dbConfig)
        {
            DbConfig = dbConfig;
            DbType = dbConfig.DbType;
        }

        public SupportedDatabaseTypes DbType { get; }


        public ISelectSqlBuilder OnSchema(string schemaName = null, Func<bool> setSchemaNameIfExpressionIsEvaluatedToTrue = null)
        {
            if (setSchemaNameIfExpressionIsEvaluatedToTrue?.Invoke() ?? true)
            {
                SchemaName = schemaName ?? DbConfig.Schema;
            }

            return this;
        }

        public ISelectSqlBuilder Count()
        {
            SelectCount = true;
            return this;
        }

        public ISelectSqlBuilder Fields<T>(Action<ISelectFieldSelector<T>> selector)
        {
            var fieldSelector = new SelectFieldSelector<T>();
            selector(fieldSelector);
            SetTableName<T>(
                () =>
                   {
                       if (string.IsNullOrEmpty(TableName) || !string.Equals(TableName, SqlBuilderHelper.GetTableNameForType<T>(SchemaNamePrefix), StringComparison.OrdinalIgnoreCase))
                       {
                           return;
                       }

                       var alias = fieldSelector.GetFirstTableAlias(typeof(T));
                       if (!string.IsNullOrEmpty(alias))
                       {
                           TableAlias = alias;
                       }
                   });

            FieldsList.AddRange(fieldSelector.Build());

            return this;
        }

        private void SetTableName<T>(Action additionAction = null)
        {
            if (!string.IsNullOrEmpty(TableName))
            {
                additionAction?.Invoke();
                return;
            }

            TableName = SqlBuilderHelper.GetTableNameForType<T>(SchemaNamePrefix);
            TableAlias = SqlBuilderHelper.GetAliasForType<T>();

            additionAction?.Invoke();
        }

        /// <summary>
        /// Where
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selector"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public ISelectSqlBuilder Where<T>(Action<IWhereFieldSelector<T>> selector, string prefix = null)
        {
            var fieldSelector = new WhereFieldSelectorWithSelect<T>(prefix, DbConfig);
            selector(fieldSelector);

            Wheres.AddRange(fieldSelector.Build());
            return this;
        }

        /// <summary>
        /// Where
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selector"></param>
        /// <param name="statement"></param>
        /// <returns></returns>
        public ISelectSqlBuilder WhereIf<T>(Action<IWhereFieldSelector<T>> selector, Func<bool> statement, string prefix = null)
        {
            if (statement.Invoke())
            {
                Where(selector, prefix);
            }
            return this;
        }

        public ISelectSqlBuilder InnerJoin<TFrom, TTo>(string fromField = null, string toField = null, string toPrefix = null, string fromPrefix = null)
        {
            return TypeJoin<TFrom, TTo>("INNER", fromField, toField, toPrefix, fromPrefix);
        }

        /// <summary>
        /// Outer Join
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTo"></typeparam>
        /// <returns></returns>
        public ISelectSqlBuilder LeftOuterJoin<TFrom, TTo>(string fromField = null, string toField = null, string toPrefix = null, string fromPrefix = null)
        {
            return TypeJoin<TFrom, TTo>("LEFT OUTER", fromField, toField, toPrefix, fromPrefix);
        }

        public ISelectSqlBuilder From<TFrom>()
        {
            SetTableName<TFrom>();
            return this;
        }

        private string GetTypeAlias<T>(string tablealias)
        {
            return SqlBuilderHelper.GetAliasForType<T>(tablealias);
        }


        private ISelectSqlBuilder TypeJoin<TFrom, TTo>(string joinType, string fromField = null, string toField = null, string toPrefix = null, string fromPrefix = null)
        {
            SetTableName<TFrom>();
            if (string.IsNullOrEmpty(fromField))
            {
                fromField = SqlBuilderHelper.GetTableNameForType<TTo>(null) + "Id";
            }

            if (string.IsNullOrEmpty(toField))
            {
                toField = "Id";
            }

            var fromExpresstion = $"{GetTypeAlias<TFrom>(fromPrefix)}.{fromField}";

            Joins.Add(string.Format("{0} JOIN {1} {2} ON {3} = {2}.{4}",
                joinType.ToUpper(),
                SqlBuilderHelper.GetTableNameForType<TTo>(SchemaNamePrefix),
                GetTypeAlias<TTo>(toPrefix),
                fromExpresstion,
                toField));
            return this;
        }


        public string Build()
        {
            var fields = string.Join(", ", FieldsList);
            var countStart = SelectCount ? "COUNT(" : string.Empty;
            var countEnd = SelectCount ? ")" : string.Empty;

            var select = $"SELECT {countStart}{fields}{countEnd} FROM {TableName} {TableAlias}";

            if (Joins.Count > 0)
            {
                var joins = string.Join(" ", Joins);

                select = $"{@select} {joins}";
            }

            if (Wheres.Count > 0)
            {
                var wheres = string.Join(" AND ", Wheres);
                select = $"{@select} WHERE {wheres}";
            }

            Reset();

            return select;
        }

        private void Reset()
        {
            Joins.Clear();
            Wheres.Clear();
            FieldsList.Clear();
            SchemaName = null;
            TableName = null;
            TableAlias = null;
            SelectCount = false;
        }
    }
}