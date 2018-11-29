using System;
using System.Collections.Generic;
using DIPS.FluentDbTools.Common.Abstractions;
using DIPS.FluentDbTools.SqlBuilder.Abstractions;
using DIPS.FluentDbTools.SqlBuilder.Abstractions.Common;
using DIPS.FluentDbTools.SqlBuilder.Abstractions.Fields;
using DIPS.FluentDbTools.SqlBuilder.Common;

namespace DIPS.FluentDbTools.SqlBuilder
{
    internal class DeleteSqlBuilder<TClass> : IDeleteSqlBuilder<TClass>
    {
        private readonly List<string> Wheres = new List<string>();
        private readonly IDbConfig DbConfig;
        private string SchemaName;

        private string SchemaNamePrefix => string.IsNullOrEmpty(SchemaName) ? string.Empty : $"{SchemaName}.";

        public DeleteSqlBuilder(IDbConfig dbConfig)
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
            var sql = $"DELETE FROM {SqlBuilderHelper.GetTableNameForType<TClass>(SchemaNamePrefix)}";

            if (Wheres.Count > 0)
            {
                var where = string.Join(" AND ", Wheres);
                sql = $"{sql} WHERE {@where}";
            }

            return sql;
        }
    }


}