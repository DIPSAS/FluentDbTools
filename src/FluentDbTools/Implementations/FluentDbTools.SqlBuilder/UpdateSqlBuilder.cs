using System;
using System.Collections.Generic;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.SqlBuilder.Abstractions;
using FluentDbTools.SqlBuilder.Abstractions.Fields;
using FluentDbTools.SqlBuilder.Common;

namespace FluentDbTools.SqlBuilder
{
    internal class UpdateSqlBuilder<TClass> : IUpdateSqlBuilder<TClass>
    {
        private string UpdateTableWith;

        private readonly List<string> Wheres = new List<string>();
        private readonly UpdateFieldSelector<TClass> UpdateFieldSelector;
        private readonly IDbConfigSchemaTargets DbConfigConfig;
        private string TableName;

        private string SchemaNameField;
        private string SchemaName
        {
            //get => (SchemaNameField = SchemaNameField ?? DbConfig?.Schema);
            get => SchemaNameField;
            set => SchemaNameField = value;
        }

        public UpdateSqlBuilder(IDbConfigSchemaTargets dbConfigConfig, string tableName = null)
        {
            DbConfigConfig = dbConfigConfig;
            DbType = dbConfigConfig?.DbType ?? SupportedDatabaseTypes.Postgres;
            TableName = tableName;

            UpdateFieldSelector = new UpdateFieldSelector<TClass>(DbConfigConfig);
        }

        public SupportedDatabaseTypes DbType { get; }

        public IUpdateSqlBuilder<TClass> Fields(Action<IFieldSetterSelector<TClass>> selector)
        {
            UpdateFieldSelector.OnTable(TableName);
            UpdateFieldSelector.OnSchema(SchemaName);
            selector(UpdateFieldSelector);

            UpdateTableWith = UpdateFieldSelector.Build();
            return this;
        }

        public IUpdateSqlBuilder<TClass> Where(Action<IWhereFieldSelector<TClass>> selector)
        {
            var whereSelector = new WhereFieldSelector<TClass>(DbConfigConfig);
            selector(whereSelector);

            Wheres.AddRange(whereSelector.Build());

            return this;
        }

        public IUpdateSqlBuilder<TClass> WhereIf(Action<IWhereFieldSelector<TClass>> selector, Func<bool> statement)
        {
            if (statement.Invoke())
            {
                Where(selector);
            }
            return this;
        }

        public IUpdateSqlBuilder<TClass> OnTable(string tableName)
        {
            TableName = tableName;
            return this;
        }

        public IUpdateSqlBuilder<TClass> OnSchema(string schemaName = null, Func<bool> setSchemaNameIfExpressionIsEvaluatedToTrue = null)
        {
            if (setSchemaNameIfExpressionIsEvaluatedToTrue?.Invoke() ?? true)
            {
                SchemaName = schemaName ?? DbConfigConfig.Schema;
            }
            return this;
        }

        public ISqlFieldBuilder<TClass> AddFields(Action<IFieldSetterSelector<TClass>> selector)
        {
            return Fields(selector);
        }

        public ISqlFieldBuilder<TClass> AddDynamicFields(dynamic fields)
        {
            throw new NotImplementedException();
        }


        public string Build()
        {
            var sql = UpdateTableWith;

            if (Wheres.Count > 0)
            {
                var where = string.Join(" AND ", Wheres);
                sql = $"{sql} WHERE {@where}";
            }

            return sql;
        }
    }


}