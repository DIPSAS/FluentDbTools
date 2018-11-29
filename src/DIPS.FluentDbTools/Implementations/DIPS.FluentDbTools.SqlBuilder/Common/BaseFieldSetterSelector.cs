using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DIPS.FluentDbTools.Common.Abstractions;
using DIPS.FluentDbTools.SqlBuilder.Abstractions.Fields;

namespace DIPS.FluentDbTools.SqlBuilder.Common
{
    public abstract class BaseFieldSetterSelector<TClass> : IFieldSetterSelector<TClass>
    {
        protected readonly IDbConfig DbConfig;

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

        protected BaseFieldSetterSelector(IDbConfig dbConfig)
        {
            DbConfig = dbConfig;
        }

        protected readonly List<Field> FieldsList = new List<Field>();

        public string SchemaName { get; set; }

        public IFieldSetterSelector<TClass> OnSchema(string schemaName = null)
        {
            SchemaName = schemaName;
            return this;
        }

        protected string SchemaNamePrefix => string.IsNullOrEmpty(SchemaName) ? string.Empty : $"{SchemaName}.";

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