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
        private readonly IDbConfig DbConfig;

        public InsertSqlBuilder(IDbConfig dbConfig)
        {
            DbConfig = dbConfig;
            DbType = dbConfig?.DbType ?? SupportedDatabaseTypes.Postgres;
        }

        public SupportedDatabaseTypes DbType { get; }
        
        private string Sql;
        private InsertFieldSelector FieldSelector;
        private string SchemaName;

        private class InsertFieldSelector : BaseFieldSetterSelector<TClass>
        {
            public InsertFieldSelector(IDbConfig dbConfig)
                : base (dbConfig)
            {
            }
            public override string Build()
            {
                return
                    $"INSERT INTO {SqlBuilderHelper.GetTableNameForType<TClass>(SchemaNamePrefix)}({string.Join(", ", FieldsList.Select(x => x.FieldName))}) VALUES({string.Join(", ", FieldsList.Select(CreateFieldValue))})";
            }

            private string CreateFieldValue(Field field)
            {
                return field.IsParameterized ? DbConfig?.WithParameters(field.Value) : field.Value;
            }
        }

        public IInsertSqlBuilder<TClass> Fields(Action<IFieldSetterSelector<TClass>> selector)
        {
            FieldSelector = FieldSelector ?? new InsertFieldSelector(DbConfig);
            FieldSelector.OnSchema(SchemaName);
            selector(FieldSelector);

            Sql = FieldSelector.Build();

            return this;
        }

        public IInsertSqlBuilder<TClass> OnSchema(string schemaName = null, Func<bool> setSchemaNameIfExpressionIsEvaluatedToTrue = null)
        {
            if (setSchemaNameIfExpressionIsEvaluatedToTrue?.Invoke() ?? true)
            {
                SchemaName = schemaName ?? DbConfig.Schema ?? string.Empty;
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