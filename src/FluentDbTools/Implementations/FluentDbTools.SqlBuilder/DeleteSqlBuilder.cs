using System;
using System.Collections.Generic;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.SqlBuilder.Abstractions;
using FluentDbTools.SqlBuilder.Abstractions.Fields;
using FluentDbTools.SqlBuilder.Common;

namespace FluentDbTools.SqlBuilder
{
    internal class DeleteSqlBuilder<TClass> : IDeleteSqlBuilder<TClass>
    {
        private readonly List<string> Wheres = new List<string>();
        private readonly IDbConfigSchemaTargets DbConfigConfig;
        private string TableName;
        private string SchemaName;

        private string SchemaNamePrefix => string.IsNullOrEmpty(SchemaName) ? SchemaPrefixId : $"{SchemaName}.{SchemaPrefixId}";
        private string SchemaPrefixId => DbConfigConfig?.GetSchemaPrefixId() ?? string.Empty;

        public DeleteSqlBuilder(IDbConfigSchemaTargets dbConfigConfig, string tableName = null)
        {
            DbConfigConfig = dbConfigConfig;
            DbType = dbConfigConfig?.DbType ?? SupportedDatabaseTypes.Postgres;
            TableName = tableName;
        }

        public SupportedDatabaseTypes DbType { get; }


        public IDeleteSqlBuilder<TClass> Where(Action<IWhereFieldSelector<TClass>> selector)
        {
            var whereSelector = new WhereFieldSelector<TClass>(DbConfigConfig);
            selector(whereSelector);

            Wheres.AddRange(whereSelector.Build());

            return this;
        }

        public IDeleteSqlBuilder<TClass> WhereIf(Action<IWhereFieldSelector<TClass>> selector, Func<bool> statement)
        {
            if (statement.Invoke())
            {
                Where(selector);
            }
            return this;
        }

        public IDeleteSqlBuilder<TClass> OnTable(string tableName)
        {
            TableName = tableName;
            return this;
        }

        public IDeleteSqlBuilder<TClass> OnSchema(string schemaName = null, Func<bool> setSchemaNameIfExpressionIsEvaluatedToTrue = null)
        {
            if (setSchemaNameIfExpressionIsEvaluatedToTrue?.Invoke() ?? true)
            {
                SchemaName = schemaName ?? DbConfigConfig?.Schema ?? string.Empty;
            }
            return this;
        }

        public string Build()
        {
            var sql = $"DELETE FROM {SqlBuilderHelper.GetTableName<TClass>(SchemaNamePrefix, TableName)}";

            if (Wheres.Count > 0)
            {
                var where = string.Join(" AND ", Wheres);
                sql = $"{sql} WHERE {@where}";
            }

            return sql;
        }
    }


}