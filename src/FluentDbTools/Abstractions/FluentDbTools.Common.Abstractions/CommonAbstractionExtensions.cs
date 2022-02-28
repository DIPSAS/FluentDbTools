using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace FluentDbTools.Common.Abstractions
{
    /// <summary>
    /// Useful extension
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class Extensions
    {

        /// <summary>
        /// return prefix == null ? name : $"{prefix}{name}"; 
        /// </summary>
        /// <param name="dbConfig"></param>
        /// <param name="name">
        /// The object to add prefix to.<br/>
        /// If name start with value of {<see cref="IDbConfigSchemaTargets.GetSchemaPrefixId()"/>}, the value of <paramref name="name"/> will be returned
        /// </param>
        /// <returns></returns>
        public static string GetPrefixedName(this IDbConfig dbConfig, string name)
        {
            return name.GetPrefixedName(dbConfig.GetSchemaPrefixId());
        }

        /// <summary>
        /// Parse sql into smaller sql statements - especial for oracle where OracleClient is not found of multiple statements in same sql
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static IEnumerable<string> ExtractSqlStatements(this string sql)
        {
            string latestStatement = null;
            var previousStatement = string.Empty;

            string RegisterStatement(string statement)
            {
                return latestStatement = statement.ConvertSimpleSqlComment();
            }

            if (sql.IsEmpty())
            {
                yield break;
            }

            // ReSharper disable once IdentifierTypo
            var sqlsToRun = Regex.Split(sql, @"^\s*\/\s*$", RegexOptions.Multiline)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x))
                .ToArray();

            foreach (var sqlToRun in sqlsToRun)
            {
                var allLines = sqlToRun.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                if (allLines.Length == 1 || (!sqlToRun.Contains(";") && !allLines[0].IsSimpleSqlComment()))
                {
                    yield return RegisterStatement(sqlToRun);
                    continue;
                }


                var builder = new StringBuilder();
                var nestedSqlStatements = new List<string>();
                var nestedCommentStatements = new List<string>();
                var createOrReplaceStatements = new List<string>();

                var nestedCommentStatementsCount = 0;
                var nestedSqlStatementsControl = new Stack<string>();
                var lineNumber = 0;
                foreach (var line in allLines)
                {
                    lineNumber++;

                    var trimmed = line.TrimEnd();
                    var trimmedStart = line.Trim().TrimStart('\t');
                    var trimmedAll = line.Trim(' ', '\t', ';', '\n','\r');

                    if (string.IsNullOrWhiteSpace(trimmed))
                    {
                        continue;
                    }

                    if (trimmed.StartsWithIgnoreCase("CREATE OR REPLACE"))
                    {
                        if (IsEndStatement())
                        {
                            yield return RegisterStatement(trimmed.TrimEnd(';'));
                            continue;
                        }
                        createOrReplaceStatements.Add(trimmed);
                        continue;
                    }

                    if (createOrReplaceStatements.Any())
                    {
                        createOrReplaceStatements.Add(trimmed);
                        if (trimmed.StartsWithIgnoreCase("end "))
                        {
                            var name = trimmed.Substring(4).Trim().TrimEnd(';');
                            if (createOrReplaceStatements.FirstOrDefault().ContainsIgnoreCase(name))
                            {
                                yield return RegisterStatement(string.Join("\n", createOrReplaceStatements));
                                createOrReplaceStatements.Clear();
                            }
                        }
                        continue;
                    }

                    if (trimmed.Trim().StartsWith("-- "))
                    {
                        yield return RegisterStatement(trimmed);
                        continue;
                    }

                    if (trimmedStart.EqualsIgnoreCase("/*") ||
                        trimmedStart.StartsWithIgnoreCase("/* "))
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
                            yield return RegisterStatement(sqlStatement);
                            continue;
                        }
                    }
                    if (nestedCommentStatementsCount > 0)
                    {
                        nestedCommentStatements.Add(trimmed);
                        continue;
                    }

                    if (trimmedAll.EqualsIgnoreCase("declare") ||
                        trimmedAll.EqualsIgnoreCase("begin") ||
                        trimmedAll.StartsWithIgnoreCase("declare ") ||
                        trimmedAll.StartsWithIgnoreCase("begin "))
                    {
                        nestedSqlStatementsControl.Push(trimmed);
                        previousStatement = latestStatement;
                    }

                    if (nestedSqlStatementsControl.Any() &&
                        (trimmedAll.EqualsIgnoreCase("end") ||
                         trimmedAll.EndsWithIgnoreCase("end") ||
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
                            yield return RegisterStatement(string.Join("\n", nestedSqlStatements));
                            continue;
                        }
                    }

                    if (nestedSqlStatementsControl.Any())
                    {
                        nestedSqlStatements.Add(trimmed);
                        continue;
                    }

                    if (IsEndStatement())
                    {
                        trimmed = trimmed.TrimEnd(';');
                        if (!string.IsNullOrWhiteSpace(trimmed.Trim()))
                        {
                            builder.AppendLine(trimmed);
                        }

                        var sqlStatement = builder.ToString().Trim();
                        if (sqlStatement.IsNotEmpty())
                        {
                            yield return RegisterStatement(sqlStatement);
                        }
                        builder.Clear();
                    }
                    else
                    {
                        builder.AppendLine(line.ConvertSimpleSqlComment());
                    }

                    bool IsEndStatement()
                    {
                        return trimmed.Last() == ';' || trimmed.EndsWithIgnoreCase(";") || allLines.Length == lineNumber;
                    }
                }
            }
        }

        /// <summary>
        /// return true if sql start with "--" or "/*"
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static bool IsSqlComment(this string sql)
        {
            return sql.IsSimpleSqlComment() || (sql?.Trim().StartsWith("/*") ?? false);
        }

        /// <summary>
        /// Convert sql to format /* <paramref name="statement"/> */ if <paramref name="statement"/> starts with "--".<br/>
        /// Return original <paramref name="statement"/> if NOT.
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public static string ConvertSimpleSqlComment(this string statement)
        {
            return statement.IsSimpleSqlComment() ? $"/* {statement.TrimStart('-').TrimStart()} */" : statement;
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


        /// <summary>
        /// Load string from Embedded resource from <paramref name="assembly"/> at location <paramref name="location"/>
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static string GetStringFromEmbeddedResource(this Assembly assembly, string location)
        {
            string content;

            using (var stream = assembly.GetManifestResourceStream(location))
            {
                if (stream == null)
                {
                    return null;
                }
                using (var reader = new StreamReader(stream))
                {
                    content = reader.ReadToEnd();
                }
            }
            return content;
        }

        /// <summary>
        /// Load string from Embedded resource from <paramref name="type"/>.Assembly at location <paramref name="location"/>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static string GetStringFromEmbeddedResource(Type type, string location)
        {
            return type.Assembly.GetStringFromEmbeddedResource(location);
        }

        private static bool IsSimpleSqlComment(this string sql)
        {
            return sql?.Trim().StartsWith("--") ?? false;
        }

        /// <summary>
        /// Build configuration key by <see cref="IDbConfig.ConfigurationDelimiter"/> and <paramref name="keys"/>
        /// </summary>
        /// <param name="dbConfig"></param>
        /// <param name="keys"></param>
        /// <returns>FROM ["a","b","c"] with Delimiter=":" TO "a:b:c" </returns>
        public static string GetConfigKey(this IDbConfig dbConfig, params string[] keys)
        {
            return string.Join(dbConfig?.ConfigurationDelimiter ?? ":", keys);
        }

        /// <summary>
        /// <para>Get the oracle service name from config</para>
        /// 1. Get config "database:dataSource".<br/>
        /// 2. If config "database:dataSource" is NULL, get <see cref="IDbConfigDatabaseTargets.DatabaseName"><paramref name="dbConfig"/>.DatabaseName</see><br/><br/>
        /// ------ See <see cref="IDbConfigDatabaseTargets.DatabaseName"/> ---------------------------<br/>
        /// <br/>
        /// Get <inheritdoc cref="IDbConfigDatabaseTargets.DatabaseName" path="/summary/remarks[1]"/>
        /// </summary>
        /// <param name="dbConfig"></param>
        /// <returns></returns>
        public static string GetOracleServiceName(this IDbConnectionStringBuilderConfig dbConfig)
        {
            if (dbConfig is IDbConfig dbConfigFull)
            {
                return dbConfigFull.GetConfigValue(dbConfigFull.GetConfigKey("database", "dataSource")) ?? dbConfig.DatabaseName;
            }

            return dbConfig.DatabaseName;
        }
    }
}