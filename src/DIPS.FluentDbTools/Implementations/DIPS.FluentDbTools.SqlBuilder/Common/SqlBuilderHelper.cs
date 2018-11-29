using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DIPS.FluentDbTools.SqlBuilder.Abstractions.Common;
using DIPS.FluentDbTools.SqlBuilder.Abstractions.Parameters;
using DIPS.FluentDbTools.SqlBuilder.Parameters;

namespace DIPS.FluentDbTools.SqlBuilder.Common
{
    public static class SqlBuilderHelper
    {
        private static readonly ConcurrentDictionary<Type, string> Alias = new ConcurrentDictionary<Type, string>();

        public static string GetParameterWithIdPostfix<T>(string postfix = "Id")
        {
            return DatabaseParameterExtensions.GetParameterWithIdPostfix<T>(postfix);
        }

        public static string GetAliasForType<T>(string tablealias = null)
        {
            if (!string.IsNullOrWhiteSpace(tablealias))
            {
                return tablealias;
            }

            if (!Alias.TryGetValue(typeof(T), out var alias))
            {
                alias = CreateAliasForType<T>();
                Alias?.TryAdd(typeof(T), alias);
            }

            return alias;
        }

        public static string GetTableNameForType<T>(string schemaNamePrefix)
        {
            if (!string.IsNullOrEmpty(schemaNamePrefix) && !schemaNamePrefix.EndsWith("."))
            {
                schemaNamePrefix = $"{schemaNamePrefix}.";
            }

            schemaNamePrefix = schemaNamePrefix ?? string.Empty;
            return $"{schemaNamePrefix}{typeof(T).Name}";
        }

        public static string GetNameFromExpression<TClass, T>(Expression<Func<TClass, T>> field)
        {
            if (!(field.Body is MemberExpression memberExpression))
            {
                throw new ArgumentException("field");
            }

            var name = memberExpression.Member.Name;
            var baseType = memberExpression.Type.BaseType;

            if (baseType == null || baseType == typeof(ValueType) || baseType == typeof(Object))
            {
                return name;
            }

            return name + "Id";
        }

        public static string GetStringForOperator(OP op)
        {
            if (op == OP.DI || op == OP.NULL_OR_DI)
            {
                return "<>";
            }
            if (op == OP.EQ)
            {
                return "=";
            }
            if (op == OP.GT)
            {
                return ">";
            }
            if (op == OP.LT)
            {
                return "<";
            }
            if (op == OP.IS)
            {
                return "IS";
            }
            if (op == OP.ISNOT)
            {
                return "IS NOT";
            }
            if (op == OP.GTEQ)
            {
                return ">=";
            }
            if (op == OP.LTEQ)
            {
                return "<=";
            }

            if (op == OP.IN)
            {
                return "IN";
            }

            if (op == OP.NOT_IN)
            {
                return "NOT IN";
            }

            throw new ArgumentException("op");
        }

        private static string CreateAliasForType<T>()
        {
            return new string(typeof(T).Name.Where(char.IsUpper).ToArray()).ToLower();
        }

        public static string CheckParamNameAndUseFieldNameIfEmpty(string field, string paramName)
        {
            if (String.IsNullOrEmpty(paramName))
            {
                paramName = field;
            }
            return paramName;
        }

        public static string CreateStringValueFromGenericValue<TValue>(TValue value, bool ignoreFormat = false)
        {
            if (value is DBNull || typeof(TValue) == typeof(DBNull))
            {
                return "NULL";
            }

            if (typeof(TValue).IsListType())
            {
                var list = new List<string>();
                foreach (var element in (IEnumerable)value)
                {
                    var v = Convert.ToString(element);
                    if (!ignoreFormat && ShouldQuoteValue(element))
                    {
                        v = "'" + v + "'";
                    }
                    list.Add(v);
                }
                return string.Join(", ", list);
            }

            var stringValue = Convert.ToString(value);
            if (ignoreFormat)
            {
                return stringValue;
            }
            
            if (ShouldQuote<TValue>(stringValue))
            {
                stringValue = "'" + stringValue + "'";
            }

            return stringValue;
        }

        private static bool ShouldQuote<T>(string sendValue)
        {
            if (sendValue == "NULL")
            {
                return false;
            }

            if (typeof(T) == typeof(string))
            {
                return true;
            }

            if (typeof(T) == typeof(DateTime))
            {
                return true;
            }

            return !typeof(T).IsValueType;
        }

        private static bool ShouldQuoteValue(object sendValue)
        {
            var type = sendValue?.GetType();

            if (type == typeof(string))
            {
                return (string)sendValue != "NULL";
            }

            if (type == typeof(DateTime))
            {
                return true;
            }

            return !type.IsValueType;
        }


        public static void ClearAlias()
        {
            Alias.Clear();
        }

        public static string GetParameterPrefixIfNull(string parameterPrefix)
        {
            return string.IsNullOrEmpty(parameterPrefix) ? "@" : parameterPrefix;
        }

    }
}