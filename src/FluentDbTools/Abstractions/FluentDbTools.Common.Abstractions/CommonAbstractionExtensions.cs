using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentDbTools.Common.Abstractions
{
    public static class Extensions
    {

        public static string GetPrefixedName(this IDbConfig dbConfig, string name)
        {
            return name.GetPrefixedName(dbConfig.GetSchemaPrefixId());
        }

        public static IEnumerable<string> ExtractSqlStatements(this string sql)
        {
            var allLines = sql.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var builder = new StringBuilder();
            var sqlStatements = new List<string>();
            var nestedSqlStatements = new List<string>();
            var nestedCommentStatements = new List<string>();
            var createOrReplaceStatements = new List<string>();

            var nestedCommentStatementsCount = 0;
            var nestedSqlStatementsControl = new Stack<string>();
            var previousStatement = string.Empty;
            foreach (var line in allLines)
            {
                var trimmed = line.TrimEnd();
                if (string.IsNullOrWhiteSpace(trimmed))
                {
                    continue;
                }

                if (trimmed.StartsWithIgnoreCase("CREATE OR REPLACE"))
                {
                    createOrReplaceStatements.Add(trimmed);
                    continue;
                }

                if (createOrReplaceStatements.Any())
                {
                    createOrReplaceStatements.Add(trimmed);
                    if (trimmed.StartsWithIgnoreCase("end "))
                    {
                        var name  = trimmed.Substring(4).Trim().TrimEnd(';');
                        if (createOrReplaceStatements.FirstOrDefault().ContainsIgnoreCase(name))
                        {
                            var sqlStatement = string.Join("\n", createOrReplaceStatements);
                            sqlStatements.Add(sqlStatement);
                            createOrReplaceStatements.Clear();
                        }
                    }
                    continue;
                }

                if (trimmed.Trim().StartsWith("-- "))
                {
                    sqlStatements.Add($"{trimmed.Replace("-- ","/* ")} */");
                    continue;
                }

                if (trimmed.Trim().TrimStart('\t').EqualsIgnoreCase("/*") ||
                    trimmed.Trim().TrimStart('\t').StartsWithIgnoreCase("/* "))
                {
                    nestedCommentStatementsCount++;
                }

                if (nestedCommentStatementsCount > 0 &&
                    (trimmed.Trim().TrimEnd('\t').EqualsIgnoreCase("*/") ||
                     trimmed.Trim().EndsWithIgnoreCase("*/")))
                {
                    nestedCommentStatementsCount--;

                    if (nestedCommentStatementsCount <= 0)
                    {
                        nestedCommentStatements.Add(trimmed);
                        nestedCommentStatementsCount = 0;
                        var sqlStatement = string.Join("\n", nestedCommentStatements);
                        sqlStatements.Add(sqlStatement);
                        continue;
                    }
                }
                if (nestedCommentStatementsCount > 0)
                {
                    nestedCommentStatements.Add(trimmed);
                    continue;
                }

                if (trimmed.EqualsIgnoreCase("declare") ||
                    trimmed.EqualsIgnoreCase("begin") ||
                    trimmed.StartsWithIgnoreCase("declare ") ||
                    trimmed.StartsWithIgnoreCase("begin "))
                {
                    nestedSqlStatementsControl.Push(trimmed);
                    previousStatement = sqlStatements.LastOrDefault();
                }

                if (nestedSqlStatementsControl.Any() &&
                    (trimmed.EqualsIgnoreCase("end") ||
                     trimmed.EndsWithIgnoreCase("end") ||
                     trimmed.EndsWithIgnoreCase("end;")) ||
                     (trimmed.StartsWithIgnoreCase("end ") && previousStatement.ContainsIgnoreCase("CREATE OR REPLACE"))
                    )
                {
                    nestedSqlStatements.Add((trimmed.TrimEnd(';') + ";"));

                    var item = nestedSqlStatementsControl.Pop();
                    if (item.StartsWithIgnoreCase("begin"))
                    {
                        var firstControl = nestedSqlStatementsControl.Count > 0 ? nestedSqlStatementsControl.Peek() : string.Empty;
                        if (nestedSqlStatementsControl.Count == 1 && 
                            firstControl.StartsWithIgnoreCase("declare"))
                        {
                            nestedSqlStatementsControl.Pop();
                        }
                    }
                    if (!nestedSqlStatementsControl.Any())
                    {
                        if (!nestedSqlStatements.Any())
                        {
                            nestedSqlStatements.Add(trimmed);
                        }
                        var sqlStatement = string.Join("\n", nestedSqlStatements);
                        sqlStatements.Add(sqlStatement);
                        continue;
                    }
                }

                if (nestedSqlStatementsControl.Any())
                {
                    nestedSqlStatements.Add(trimmed);
                    continue;
                }
                
                var isEndStatement = trimmed.Last() == ';';
                if (isEndStatement)
                {
                    trimmed = trimmed.TrimEnd(';');
                    if (!string.IsNullOrWhiteSpace(trimmed.Trim()))
                    {
                        builder.AppendLine(trimmed.Trim().StartsWith("-- ") ? $"/* {trimmed} */" : trimmed);
                    }

                    var sqlStatement = builder.ToString().Trim();
                    if (sqlStatement.IsNotEmpty())
                    {
                        sqlStatements.Add(sqlStatement);
                    }
                    builder.Clear();
                }
                else
                {
                    builder.AppendLine(line.Trim().StartsWith("-- ") ? $"/* {line} */" : line);
                }
            }

            return sqlStatements;
        }

        public static bool IsSqlComment(this string sql)
        {
            return sql.Trim().StartsWith("-- ") || sql.Trim().StartsWith("/*");
        }


        /// <summary>
        /// Get config value by keys.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static string GetValue(this IDictionary<string, string> dictionary, params string[] keys)
        {
            if (dictionary == null)
            {
                return null;
            }
            var hasComparer = new bool?();
            foreach (var key in keys)
            {
                if (dictionary.TryGetValue(key, out var value))
                {
                    return value;
                }

                if (hasComparer == null)
                {
                    hasComparer = (dictionary as Dictionary<string, string>)?.Comparer != null;
                }

                if (hasComparer.Value)
                {
                    continue;
                }

                var e = dictionary.FirstOrDefault(x => x.Key.EqualsIgnoreCase(key));
                if (e.Value != null)
                {
                    return e.Value;
                }
            }

            return null;

        }
    }

    
}