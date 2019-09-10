using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.SqlBuilder.Abstractions.Fields;

namespace FluentDbTools.SqlBuilder.Common
{
    public abstract class BaseFieldSetterSelector<TClass> : IFieldSetterSelector<TClass>
    {
        protected readonly IDbConfigDatabaseTargets DbConfig;

        protected class Field
        {
            public string FieldName { get; private set; }

            public string Value { get; private set; }

            public bool IsParameterized { get; private set; }

            public Field(string fieldName, string value, bool isParameterized)
            {
                FieldName = fieldName;
                IsParameterized = isParameterized;
                Value = value;
            }
        }

        protected BaseFieldSetterSelector(IDbConfigDatabaseTargets dbConfig)
        {
            DbConfig = dbConfig;
            SchemaPrefixId = dbConfig?.GetSchemaPrefixId() ?? string.Empty;
        }

        protected readonly List<Field> FieldsList = new List<Field>();

        public string TableName { get; private set; }
        public string SchemaName { get; private set; }
        public string SchemaPrefixId { get; }
        
        public IFieldSetterSelector<TClass> OnTable(string tableName)
        {
            TableName = tableName;
            return this;
        }

        public IFieldSetterSelector<TClass> OnSchema(string schemaName = null)
        {
            SchemaName = schemaName;
            return this;
        }



        protected string TableNameWithSchemaName => SqlBuilderHelper.GetTableName<TClass>(SchemaNamePrefix, TableName);

        protected string SchemaNamePrefix => string.IsNullOrEmpty(SchemaName) ? SchemaPrefixId : $"{SchemaName}.{SchemaPrefixId}";

        public IFieldSetterSelector<TClass> FP<T>(Expression<Func<TClass, T>> field, string param = null)
        {
            return FP(SqlBuilderHelper.GetNameFromExpression(field), param);
        }

        public IFieldSetterSelector<TClass> FP(string field, string param)
        {
            param = SqlBuilderHelper.CheckParamNameAndUseFieldNameIfEmpty(field, param);

            FieldsList.Add(new Field(field, param, true));
            return this;
        }

        public IFieldSetterSelector<TClass> FV<T, TValue>(Expression<Func<TClass, T>> field, TValue value, bool ignoreFormat = false)
        {
            return FV(SqlBuilderHelper.GetNameFromExpression(field), value, ignoreFormat);
        }

        public IFieldSetterSelector<TClass> FV<TValue>(string field, TValue value, bool ignoreFormat = false)
        {
            var fieldValue = SqlBuilderHelper.CreateStringValueFromGenericValue(value, ignoreFormat);

            FieldsList.Add(new Field(field, fieldValue, false));
            return this;
        }

        public abstract string Build();
    }
}