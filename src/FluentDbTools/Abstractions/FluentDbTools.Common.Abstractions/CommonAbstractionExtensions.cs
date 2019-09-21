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
            var nestedSqlStatementsCount = 0;
            var nestedCommentStatementsCount = 0;
            foreach (var line in allLines)
            {
                var trimmed = line.TrimEnd();
                if (string.IsNullOrWhiteSpace(trimmed))
                {
                    continue;
                }

                if (trimmed.Trim().StartsWith("-- "))
                {
                    sqlStatements.Add($"{trimmed.Replace("-- ","/* ")} */");
                    continue;
                }

                if (trimmed.Trim().TrimStart('\t').EqualsIgnoreCase("/*") ||
                    trimmed.Trim().TrimStart('\t').StartsWith("/* ", StringComparison.OrdinalIgnoreCase))
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
                    trimmed.StartsWith("declare ", StringComparison.OrdinalIgnoreCase) ||
                    trimmed.StartsWith("begin ", StringComparison.OrdinalIgnoreCase))
                {
                    nestedSqlStatementsCount++;
                }

                if (nestedSqlStatementsCount > 0 &&
                    (trimmed.EqualsIgnoreCase("end") ||
                     trimmed.EndsWithIgnoreCase("end") ||
                     trimmed.EndsWithIgnoreCase("end;")))
                {
                    if (!trimmed.EndsWithIgnoreCase("end;"))
                    {
                        nestedSqlStatements.Add(trimmed + ";");
                    }

                    nestedSqlStatementsCount--;
                    if (nestedSqlStatementsCount <= 0)
                    {
                        if (!nestedSqlStatements.Any())
                        {
                            nestedSqlStatements.Add(trimmed);
                        }
                        nestedSqlStatementsCount = 0;
                        var sqlStatement = string.Join(" ", nestedSqlStatements);
                        sqlStatements.Add(sqlStatement);
                        continue;
                    }
                }

                if (nestedSqlStatementsCount > 0)
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