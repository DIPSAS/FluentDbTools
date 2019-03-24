using System;
using System.Collections.Generic;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.SqlBuilder.Abstractions;
using FluentDbTools.SqlBuilder.Abstractions.Common;
using FluentDbTools.SqlBuilder.Abstractions.Fields;
using FluentDbTools.SqlBuilder.Common;

namespace FluentDbTools.SqlBuilder
{
    internal class DeleteSqlBuilder<TClass> : IDeleteSqlBuilder<TClass>
    {
        private readonly List<string> Wheres = new List<string>();
        private readonly IDbConfigDatabaseTargets DbConfig;
        private string TableName;
        private string SchemaName;

        private string SchemaNamePrefix => string.IsNullOrEmpty(SchemaName) ? string.Empty : $"{SchemaName}.";

        public DeleteSqlBuilder(IDbConfigDatabaseTargets dbConfig)
        {
            DbConfig = dbConfig;
            DbType = dbConfig?.DbType ?? SupportedDatabaseTypes.Postgres;
        }

        public SupportedDatabaseTypes DbType { get; }


        public IDeleteSqlBuilder<TClass> Where(Action<IWhereFieldSelector<TClass>> selector)
        {
            var whereSelector = new WhereFieldSelector<TClass>(DbConfig);
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
                SchemaName = schemaName ?? DbConfig?.Schema ?? string.Empty;
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