using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FluentDbTools.Common.Abstractions;

namespace FluentDbTools.SqlBuilder.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class SqlAliasHelper
    {
        private static readonly ConcurrentDictionary<Type, string> PreDefinedAliases = new ConcurrentDictionary<Type, string>();
        private static readonly List<string> BlacklistedAliases = new List<string>();
        private static readonly ConcurrentDictionary<Type, string> Alias = new ConcurrentDictionary<Type, string>();
        private static bool AvoidDuplicateAliasValues;

        static SqlAliasHelper()
        {
            AddBlacklistedAlias("as", "on", "in", "or", "and", "exists");
            AddBlacklistedAlias("from", "where", "union");
            AddBlacklistedAlias("select", "insert", "update", "delete");
            AvoidDuplicateAliasValues = false;
        }

        public static void SetAvoidDuplicateAliasValues(bool value = true)
        {
            AvoidDuplicateAliasValues = value;
        }

        public static void ClearAliases()
        {
            Alias.Clear();
        }
        public static void ClearPreDefinedAliases()
        {
            PreDefinedAliases.Clear();
        }

        public static void ClearBlacklistedAliases()
        {
            BlacklistedAliases.Clear();
        }

        public static void AddBlacklistedAlias(params string[] keys)
        {
            foreach (var key in keys)
            {
                if (BlacklistedAliases.Contains(key, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                BlacklistedAliases.Add(key);
            }
        }

        public static void PreDefinedAlias(Type key, string alias)
        {
            if (PreDefinedAliases.ContainsKey(key))
            {
                PreDefinedAliases[key] = alias;
                return;
            }

            PreDefinedAliases.TryAdd(key, alias);
        }

        internal static string GetAliasForType<T>(string tableAlias = null, string tableName = null)
        {
            var type = typeof(T);
            if (tableAlias.IsNotEmpty() &&
                !BlacklistedAliases.Contains(tableAlias, StringComparer.OrdinalIgnoreCase))
            {
                if (AvoidDuplicateAliasValues)
                {
                    return tableAlias;
                }
                if (Alias.TryGetValue(type, out var oldTableAlias))
                {
                    if (!tableAlias.EqualsIgnoreCase(oldTableAlias))
                    {
                        Alias.TryUpdate(type, tableAlias, oldTableAlias);
                    }
                }
                else
                {
                    Alias.TryAdd(type, tableAlias);
                }

                return tableAlias;
            }

            if (!Alias.TryGetValue(type, out var alias))
            {
                alias = CreateAliasForType(type, tableName, tableAlias);
                Alias.TryAdd(type, alias);
            }

            return alias;
        }

        private static string CreateAliasForType(Type type, string typeName, string tableAlias = null)
        {
            typeName = typeName.WithDefault(type.Name);
            if (PreDefinedAliases.TryGetValue(type, out var alias))
            {
                if (!BlacklistedAliases.Contains(alias, StringComparer.OrdinalIgnoreCase))
                {
                    return alias;
                }

                PreDefinedAliases.TryRemove(type, out _);
            }

            var typeNameLength = typeName.Length;
            string[] existingAliasValues = null;

            var aliasFromUpperCase = tableAlias.WithDefault(new string(typeName.Where(char.IsUpper).ToArray()).ToLower());
            if (aliasFromUpperCase.IsEmpty())
            {
                return string.Empty;
            }

            for (var i = 0; i < 5; i++)
            {
                alias = aliasFromUpperCase;

                if (i != 0)
                {
                    var pos = typeNameLength - i;
                    if (pos > -1)
                    {
                        alias += typeName.Substring(pos, 1);
                    }
                }

                existingAliasValues = existingAliasValues ?? BlacklistedAliases.Union(PreDefinedAliases.Values).ToArray();

                if (!existingAliasValues.Contains(alias, StringComparer.OrdinalIgnoreCase))
                {
                    return alias;
                }
            }

            existingAliasValues = existingAliasValues ?? BlacklistedAliases.Union(PreDefinedAliases.Values).ToArray();

            for (var i = 1; i < 20; i++)
            {
                alias = $"{aliasFromUpperCase}{i}";

                if (!existingAliasValues.Contains(alias, StringComparer.OrdinalIgnoreCase))
                {
                    return alias;
                }
            }

            return string.Empty;

        }
    }
}