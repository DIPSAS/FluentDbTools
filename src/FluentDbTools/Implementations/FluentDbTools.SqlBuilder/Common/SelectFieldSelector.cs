using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentDbTools.SqlBuilder.Abstractions.Fields;

namespace FluentDbTools.SqlBuilder.Common
{
    internal class SelectFieldSelector<TClass> : ISelectFieldSelector<TClass>
    {
        private readonly List<string> FieldsList = new List<string>();

        private readonly IDictionary<Type, string> TableAliaseLookup = new Dictionary<Type, string>();

        public ISelectFieldSelector<TClass> F<T>(Expression<Func<TClass, T>> field, string alias = null, string tableAlias = null)
        {
            return F(SqlBuilderHelper.GetNameFromExpression(field), alias, tableAlias);
        }

        public ISelectFieldSelector<TClass> F(string field, string alias = null, string tableAlias = null)
        {
            var aliasForType = SqlAliasHelper.GetAliasForType<TClass>(tableAlias);

            AddTableAlias(aliasForType);

            if (string.IsNullOrEmpty(alias))
            {
                FieldsList.Add($"{aliasForType}.{field}");
            }
            else
            {
                FieldsList.Add($"{aliasForType}.{field} as {alias}");
            }

            return this;
        }

        private void AddTableAlias(string tablealias)
        {
            if (!TableAliaseLookup.Keys.Contains(typeof(TClass)))
            {
                TableAliaseLookup.Add(typeof(TClass), tablealias);
            }
        }

        public ISelectFieldSelector<TClass> All(string tablealias = null)
        {
            return F("*", null, tablealias);
        }

        public ISelectFieldSelector<TClass> Count()
        {
            FieldsList.Add("COUNT(*)");
            return this;
        }

        public IEnumerable<string> Build()
        {
            return FieldsList;
        }

        public string GetFirstTableAlias(Type type)
        {
            return TableAliaseLookup.ContainsKey(type) ? TableAliaseLookup[type] : null;
        }
    }
}