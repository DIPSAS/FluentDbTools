using System;
using System.Linq;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.SqlBuilder.Abstractions;
using FluentDbTools.SqlBuilder.Abstractions.Fields;
using FluentDbTools.SqlBuilder.Common;

namespace FluentDbTools.SqlBuilder
{
    internal class InsertSqlBuilder<TClass> : IInsertSqlBuilder<TClass>
    {
        private readonly IDbConfigSchemaTargets DbConfigConfig;

        public InsertSqlBuilder(IDbConfigSchemaTargets dbConfigConfig, string tableName = null)
        {
            DbConfigConfig = dbConfigConfig;
            DbType = dbConfigConfig?.DbType ?? SupportedDatabaseTypes.Postgres;
            TableName = tableName;
        }

        public SupportedDatabaseTypes DbType { get; }
        
        private string Sql;
        private InsertFieldSelector FieldSelector;
        private string TableName;
        private string SchemaName;

        private class InsertFieldSelector : BaseFieldSetterSelector<TClass>
        {
            public InsertFieldSelector(IDbConfigSchemaTargets dbConfigConfig)
                : base (dbConfigConfig)
            {
            }
            public override string Build()
            {
                return
                    $"INSERT INTO {TableNameWithSchemaName}({string.Join(", ", FieldsList.Select(x => x.FieldName))}) VALUES({string.Join(", ", FieldsList.Select(CreateFieldValue))})";
            }

            private string CreateFieldValue(Field field)
            {
                return field.IsParameterized ? DbConfigConfig?.WithParameters(field.Value) : field.Value;
            }
        }

        public IInsertSqlBuilder<TClass> Fields(Action<IFieldSetterSelector<TClass>> selector)
        {
            FieldSelector = FieldSelector ?? new InsertFieldSelector(DbConfigConfig);
            FieldSelector.OnTable(TableName);
            FieldSelector.OnSchema(SchemaName);
            selector(FieldSelector);

            Sql = FieldSelector.Build();

            return this;
        }

        public IInsertSqlBuilder<TClass> OnTable(string tableName)
        {
            TableName = tableName;
            return this;
        }

        public IInsertSqlBuilder<TClass> OnSchema(string schemaName = null, Func<bool> setSchemaNameIfExpressionIsEvaluatedToTrue = null)
        {
            if (setSchemaNameIfExpressionIsEvaluatedToTrue?.Invoke() ?? true)
            {
                SchemaName = schemaName ?? DbConfigConfig.Schema ?? string.Empty;
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
            return Sql;
        }
    }
}