using System;
using System.Collections.Generic;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.SqlBuilder.Abstractions;
using FluentDbTools.SqlBuilder.Abstractions.Fields;
using FluentDbTools.SqlBuilder.Common;

namespace FluentDbTools.SqlBuilder
{
    internal class SelectSqlBuilder : ISelectSqlBuilder
    {
        private readonly IDbConfigSchemaTargets DbConfigConfig;
        private readonly List<string> FieldsList = new List<string>();

        private readonly List<string> Joins = new List<string>();

        private readonly List<string> Wheres = new List<string>();

        private string TableName;

        private string TableAlias;
        private string SchemaName;
        private bool SelectCount = false;

        private string SchemaNamePrefix => string.IsNullOrEmpty(SchemaName) ? SchemaPrefixId : $"{SchemaName}.{SchemaPrefixId}";
        private string SchemaPrefixId => DbConfigConfig?.GetSchemaPrefixId() ?? string.Empty;

        public SelectSqlBuilder(IDbConfigSchemaTargets dbConfigConfig)
        {
            DbConfigConfig = dbConfigConfig;
            DbType = dbConfigConfig.DbType;
        }

        public SupportedDatabaseTypes DbType { get; }

        public ISelectSqlBuilder OnSchema(string schemaName = null, Func<bool> setSchemaNameIfExpressionIsEvaluatedToTrue = null)
        {
            if (setSchemaNameIfExpressionIsEvaluatedToTrue?.Invoke() ?? true)
            {
                SchemaName = schemaName ?? DbConfigConfig.Schema;
            }

            return this;
        }

        public ISelectSqlBuilder Count()
        {
            SelectCount = true;
            return this;
        }

        public ISelectSqlBuilder Fields<T>(Action<ISelectFieldSelector<T>> selector, string tableName = null, string tableAlias = null)
        {
            var fieldSelector = new SelectFieldSelector<T>();
            selector(fieldSelector);
            SetTableName<T>(
                () =>
                   {
                       if (string.IsNullOrEmpty(TableName) ||
                           !string.Equals(TableName, SqlBuilderHelper.GetTableName<T>(SchemaNamePrefix, tableName), StringExtensions.CurrentIgnoreCaseStringComparison))
                       {
                           return;
                       }

                       var alias = tableAlias.WithDefault(fieldSelector.GetFirstTableAlias(typeof(T)));
                       if (!string.IsNullOrEmpty(alias))
                       {
                           TableAlias = TableAlias.WithDefault(alias);
                       }
                   }, tableName, tableAlias);

            FieldsList.AddRange(fieldSelector.Build());

            return this;
        }

        private void SetTableName<T>(Action additionAction = null, string originalTableName = null, string tableAlias = null)
        {
            if (!string.IsNullOrEmpty(TableName))
            {
                if (TableAlias.IsEmpty())
                {
                    TableAlias = SqlAliasHelper.GetAliasForType<T>(tableAlias, TableName);
                }
                additionAction?.Invoke();
                return;
            }

            TableName = SqlBuilderHelper.GetTableName<T>(SchemaNamePrefix, originalTableName);
            TableAlias = SqlAliasHelper.GetAliasForType<T>(tableAlias, originalTableName);

            additionAction?.Invoke();
        }

        /// <summary>
        /// Where
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selector"></param>
        /// <param name="tableAlias"></param>
        /// <returns></returns>
        public ISelectSqlBuilder Where<T>(Action<IWhereFieldSelector<T>> selector, string tableAlias = null)
        {
            var fieldSelector = new WhereFieldSelectorWithSelect<T>(tableAlias, DbConfigConfig);
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
        /// <param name="tableAlias"></param>
        /// <returns></returns>
        public ISelectSqlBuilder WhereIf<T>(Action<IWhereFieldSelector<T>> selector, Func<bool> statement, string tableAlias = null)
        {
            if (statement.Invoke())
            {
                Where(selector, tableAlias);
            }
            return this;
        }

        public ISelectSqlBuilder InnerJoin<TFrom, TTo>(string fromField = null, string toField = null, string toTableAlias = null, string fromTableAlias = null,
            string fromTableName = null,
            string toTableName = null)
        {
            return TypeJoin<TFrom, TTo>("INNER", fromField, toField, toTableAlias, fromTableAlias, fromTableName, toTableName);
        }

        /// <summary>
        /// Outer Join
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTo"></typeparam>
        /// <returns></returns>
        public ISelectSqlBuilder LeftOuterJoin<TFrom, TTo>(string fromField = null, string toField = null, string toTableAlias = null, string fromTableAlias = null,
            string fromTableName = null,
            string toTableName = null)
        {
            return TypeJoin<TFrom, TTo>("LEFT OUTER", fromField, toField, toTableAlias, fromTableAlias, fromTableName, toTableName);
        }

        public ISelectSqlBuilder From<TFrom>(string fromTableName = null, string tableAlias = null)
        {
            SetTableName<TFrom>(originalTableName: fromTableName, tableAlias: tableAlias);
            return this;
        }

        private string GetTypeAlias<T>(string tableAlias, string tableName = null)
        {
            return SqlAliasHelper.GetAliasForType<T>(tableAlias, tableName);
        }


        private ISelectSqlBuilder TypeJoin<TFrom, TTo>(string joinType, string fromField = null, string toField = null, string toTableAlias = null, string fromTableAlias = null,
            string fromTableName = null,
            string toTableName = null)
        {
            SetTableName<TFrom>(originalTableName: fromTableName, tableAlias: fromTableAlias);
            if (string.IsNullOrEmpty(fromField))
            {
                fromField = SqlBuilderHelper.GetTableName<TTo>(null, toTableName) + "Id";
            }

            if (string.IsNullOrEmpty(toField))
            {
                toField = "Id";
            }

            var fromExpresstion = $"{GetTypeAlias<TFrom>(fromTableAlias, fromTableName)}.{fromField}";

            Joins.Add(string.Format("{0} JOIN {1} {2} ON {3} = {2}.{4}",
                joinType.ToUpper(),
                SqlBuilderHelper.GetTableName<TTo>(SchemaNamePrefix, toTableName),
                GetTypeAlias<TTo>(toTableAlias, toTableName),
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