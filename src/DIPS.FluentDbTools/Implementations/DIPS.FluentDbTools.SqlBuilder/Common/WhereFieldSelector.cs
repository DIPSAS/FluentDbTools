using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DIPS.FluentDbTools.Common.Abstractions;
using DIPS.FluentDbTools.SqlBuilder.Abstractions.Common;
using DIPS.FluentDbTools.SqlBuilder.Abstractions.Fields;

namespace DIPS.FluentDbTools.SqlBuilder.Common
{
    internal class WhereFieldSelector<TClass> : IWhereFieldSelector<TClass>
    {
        protected readonly IDbConfig DbConfig;
        private readonly List<string> Wheres = new List<string>();

        public WhereFieldSelector(IDbConfig dbConfig)
        {
            DbConfig = dbConfig;
        }

        public IWhereFieldSelector<TClass> WP<T>(Expression<Func<TClass, T>> field,
            string paramName = null,
            OP whereOperator = OP.EQ)
        {
            return WP(SqlBuilderHelper.GetNameFromExpression(field), paramName, whereOperator);
        }

        public IWhereFieldSelector<TClass> WP(string field, string paramName = null, OP whereOperator = OP.EQ)
        {
            paramName = SqlBuilderHelper.CheckParamNameAndUseFieldNameIfEmpty(field, paramName);

            Wheres.Add(CreateWhereFieldStringForParameter(field, paramName, whereOperator));

            return this;
        }

        public IWhereFieldSelector<TClass> WP<T>(Expression<Func<TClass, T>> field, string[] paramNames, OP whereOperator = OP.IN)
        {
            return WP<T>(SqlBuilderHelper.GetNameFromExpression(field), paramNames, whereOperator);
        }

        public IWhereFieldSelector<TClass> WP<T>(string field, string[] paramNames, OP whereOperator = OP.IN)
        {
            Wheres.Add(CreateWhereFieldStringForValue(field, whereOperator, paramNames));

            return this;
        }

        public IWhereFieldSelector<TClass> WV<T, TValue>(
            Expression<Func<TClass, T>> field,
            TValue value,
            OP whereOperator = OP.EQ,
            bool ignoreFormat = false)
        {
            return WV(SqlBuilderHelper.GetNameFromExpression(field), value, whereOperator, ignoreFormat);
        }

        public IWhereFieldSelector<TClass> WV<TValue>(string field, TValue value, OP whereOperator = OP.EQ, bool ignoreFormat = false)
        {
            var sendValue = SqlBuilderHelper.CreateStringValueFromGenericValue(value, ignoreFormat);

            Wheres.Add(CreateWhereFieldStringForValue(field, whereOperator, sendValue));

            return this;
        }

        public IEnumerable<string> Build()
        {
            return Wheres;
        }
        
        protected virtual string CreateWhereFieldStringForParameter(string field, string paramName, OP whereOperator)
        {
            return $"{field} {SqlBuilderHelper.GetStringForOperator(whereOperator)} {DbConfig.WithParameters(paramName)}";
        }

        protected virtual string CreateWhereFieldStringForValue(string field, OP whereOperator, params string[] value)
        {
            if (whereOperator == OP.NULL_OR_DI)
            {
                return string.Format(
                    "({0} IS NULL OR {0} {1} {2})",
                    field,
                    SqlBuilderHelper.GetStringForOperator(whereOperator),
                    value);
            }
            if (whereOperator == OP.NOT_IN && value.Length == 0)
            {
                return "/* NOT IN () */ 1 = 1";
            }

            if (whereOperator == OP.IN && value.Length == 0)
            {
                return "/* IN () */ 1 = 0";
            }

            return
                $"{field} {SqlBuilderHelper.GetStringForOperator(whereOperator)} {(value != null ? GetStringValueByOperator(whereOperator, value) : GetNullStringValueByOperator(whereOperator))}";
        }

        protected string GetStringValueByOperator(OP whereOperator, params string[] value)
        {
            var values = string.Join(", ", value);
            if ((whereOperator == OP.IN || whereOperator == OP.NOT_IN)
                && DbConfig.DbType == SupportedDatabaseTypes.Postgres)
            {
                values = "(" + values + ")";
            }

            return values;
        }

        protected static string GetNullStringValueByOperator(OP whereOperator)
        {
            if (whereOperator == OP.IS || whereOperator == OP.ISNOT)
            {
                return "null";
            }

            if (whereOperator == OP.IN || whereOperator == OP.NOT_IN)
            {
                return "(null)";
            }

            return null;
        }
    }
}