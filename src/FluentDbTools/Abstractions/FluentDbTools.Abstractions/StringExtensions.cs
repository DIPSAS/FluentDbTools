using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

// ReSharper disable UnusedMember.Global
[assembly:InternalsVisibleTo("FluentDbTools.Migration.Oracle")]
namespace FluentDbTools.Common.Abstractions
{
    /// <summary>
    /// String extension functions
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Default IgnoreCase StringComparison
        /// </summary>
        public static StringComparison CurrentIgnoreCaseStringComparison = StringComparison.OrdinalIgnoreCase;


        /// <summary>
        /// Removes all leading and trailing occurrences of [' ', '\n', '\n'] from the current string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="additionalChars"></param>
        /// <returns></returns>
        public static string TrimWhiteSpaces(this string value, params char[] additionalChars)
        {
            value = value?.Trim('\n', '\r', ' ');
            if (additionalChars.Any())
            {
                value = value?.Trim(additionalChars);
            }

            return value;
        }

        /// <summary>
        /// Removes all trailing occurrences of [' ', '\n', '\n'] from the current string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string TrimEndWhiteSpaces(this string value)
        {
            return value?.TrimEnd('\n', '\r', ' ');
        }

        /// <summary>
        /// Tells if string is one line. (Not containing <paramref name="splitChar"/> character)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="splitChar"></param>
        /// <returns></returns>
        public static bool IsOneLine(this string value, char splitChar = '\n')
        {
            return value.Split(splitChar).Count(IsNotEmpty) == 1;
        }

        /// <summary>
        /// <para>Return a multi-lines string.</para>
        /// <remarks>Code behind is:
        /// <code>value.Split('\n')</code></remarks>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="splitChar"></param>
        /// <returns></returns>
        public static string[] ToMultiLine(this string value, char splitChar = '\n')
        {
            if (value.IsOneLine(splitChar))
            {
                return new[] { value };
            }

            return value.Split(splitChar).Where(IsNotEmpty).ToArray();
        }

        /// <summary>
        /// return 'defaultValue' if 'value' is [null | string.Empty], elsewhere 'value' is returned
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string WithDefault(this string value, string defaultValue)
        {
            return value.IsEmpty() ? defaultValue : value;
        }

        /// <summary>
        /// return 'defaultValue' if 'value' is [null | string.Empty], elsewhere 'value' is returned
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string WithDefault(this string value, bool defaultValue)
        {
            return value.IsEmpty() ? defaultValue.ToString().ToLower() : value;
        }

        /// <summary>
        /// return true if value is [true] - Case is Ignored
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsTrue(this string value)
        {
            return (value.IsEmpty() ? false.ToString().ToLower() : value)
                .EqualsIgnoreCase(true.ToString());
        }

        /// <summary>
        /// return true if value is [null | string.Empty | false] - Case is Ignored
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsFalse(this string value)
        {
            return (value.IsEmpty() ? false.ToString().ToLower() : value)
                .EqualsIgnoreCase(false.ToString());
        }


        /// <summary>
        /// return true if value is [null | string.Empty]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEmpty(this string value)
        {
            return String.IsNullOrEmpty(value);
        }

        /// <summary>
        /// return true if value is NOT [null | string.Empty]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNotEmpty(this string value)
        {
            return !value.IsEmpty();
        }


        /// <summary>
        /// Contains(searchValue) with StringComparison=<see cref="CurrentIgnoreCaseStringComparison"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static bool ContainsIgnoreCase(this string value, string searchValue)
        {
            return value.Contains(searchValue, CurrentIgnoreCaseStringComparison);
        }

        /// <summary>
        /// EndsWith(searchValue) with StringComparison=<see cref="CurrentIgnoreCaseStringComparison"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static bool EndsWithIgnoreCase(this string value, string searchValue)
        {
            return value.EndsWith(searchValue, CurrentIgnoreCaseStringComparison);
        }

        /// <summary>
        /// EndsWith(searchValue) with StringComparison=<see cref="CurrentIgnoreCaseStringComparison"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static bool StartsWithIgnoreCase(this string value, string searchValue)
        {
            return value.StartsWith(searchValue, CurrentIgnoreCaseStringComparison);
        }


        /// <summary>
        /// Equals(searchValue) with tringComparison=<see cref="CurrentIgnoreCaseStringComparison"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(
            this string value,
            string searchValue)
        {
            return value?.Equals(searchValue, CurrentIgnoreCaseStringComparison) ?? false;
        }

        /// <summary>
        /// Contains(searchValue) with StringComparison
        /// </summary>
        /// <param name="value"></param>
        /// <param name="searchValue"></param>
        /// <param name="stringComparison"></param>
        /// <returns></returns>
        public static bool Contains(
            this string value,
            string searchValue,
            StringComparison stringComparison)
        {
            return (value?.IndexOf(searchValue, stringComparison) ?? -1) > -1;
        }

        /// <summary>
        /// Convert 'value' to int?
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int? TryConvertToInt(this string value, int? defaultValue = null)
        {
            return Int32.TryParse(value, out var parsedValue) ? parsedValue : defaultValue;
        }

        /// <summary>
        /// Convert 'value' to long?
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static long? TryConvertToLong(this string value, long? defaultValue = null)
        {
            return Int64.TryParse(value, out var parsedValue) ? parsedValue : defaultValue;
        }

        /// <summary>
        /// Convert 'value' to double?
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double? TryConvertToDouble(this string value, double? defaultValue = null)
        {
            return Double.TryParse(value, out var parsedValue) ? parsedValue : defaultValue;
        }

        /// <summary>
        /// return prefix == null ? name : $"{prefix}{name}"; 
        /// </summary>
        /// <param name="name">
        /// name of the object to add prefix to.<br/>
        /// If name start with value of {<paramref name="prefix"/>}, the value of <paramref name="name"/> will be returned
        /// </param>
        /// <param name="prefix">if <paramref name="prefix"/> is NULL, the value will be defaulted to <paramref name="defaultPrefix'"/></param>
        /// <param name="defaultPrefix"></param>
        /// <returns></returns>
        public static string GetPrefixedName(this string name, string prefix, string defaultPrefix = null)
        {
            prefix = prefix ?? defaultPrefix ?? String.Empty;

            if (prefix.IsEmpty() || name.IsEmpty())
            {
                return name;
            }

            return name.StartsWith(prefix, StringComparison.Ordinal) ? name : $"{prefix}{name}";
        }


        /// <summary>
        /// remove {prefix} if name start with {prefix}
        /// </summary>
        /// <param name="name"></param>
        /// <param name="prefix">if <paramref name="prefix"/> is NULL, the value will be defaulted to <paramref name="defaultPrefix'"/></param>
        /// <param name="defaultPrefix"></param>
        /// <returns></returns>
        public static string TrimPrefixName(this string name, string prefix, string defaultPrefix = null)
        {
            prefix = prefix ?? defaultPrefix ?? String.Empty;
            if (prefix.IsEmpty() || name.IsEmpty())
            {
                return name;
            }

            return name.StartsWith(prefix, StringComparison.Ordinal) ? name.Substring(prefix.Length) : name;
        }

        /// <summary>
        /// Returns a new string in which all occurrences of a specified string in the current instance are replaced with another specified string.<br/>
        /// This function is case-insensitive
        /// </summary>
        /// <param name="value">source string-instance</param>
        /// <param name="oldValue">The string to be replaced.</param>
        /// <param name="newValue">The string to replace all occurrences of oldValue.</param>
        /// <returns>A string that is equivalent to the current string except that all instances of <paramref name="oldValue">oldValue</paramref> are replaced with <paramref name="newValue">newValue</paramref>. If <paramref name="oldValue">oldValue</paramref> is not found in the current instance, the method returns the current instance unchanged.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="oldValue">oldValue</paramref> is null.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="oldValue">oldValue</paramref> is the empty string ("").</exception>
        public static string ReplaceIgnoreCase(this string value, string oldValue, string newValue)
        {
            if (value.IsEmpty() || oldValue.IsEmpty())
            {
                return value;
            }

            var pos = value?.IndexOf(oldValue, CurrentIgnoreCaseStringComparison) ?? -1;
            if (pos == -1)
            {
                return value;
            }

            var oldValueToReplace = value?.Substring(pos, oldValue.Length);

            return oldValueToReplace.IsEmpty() ? value : value?.Replace(oldValueToReplace, newValue);
        }

        /// <summary>
        /// Retrieves a substring from this instance. The substring is from position 0 to the position of <paramref name="to"/> string <br/>
        /// If position of <paramref name="to"/> is less 0, the incoming string is returned.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static string SubstringTo(this string value, string to)
        {
            var pos = value.IndexOf(to, CurrentIgnoreCaseStringComparison);
            return pos > -1 ? value.Substring(0, pos) : value;
        }

        /// <summary>
        /// Retrieves a substring from this instance. The substring is from position 0 to the position of <paramref name="to"/> string <br/>
        /// If position of <paramref name="to"/> is less 0, the incoming string is returned.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static string SubstringFrom(this string value, string from, params string[] to)
        {
            var pos = value.IndexOf(from, CurrentIgnoreCaseStringComparison);

            if (pos <= -1)
            {
                return value;
            }

            var subStr = value.Substring(pos);
            pos = -1;
            foreach (var s in to)
            {
                var posFount = subStr.IndexOf(s, from.Length - 1, CurrentIgnoreCaseStringComparison);
                if (posFount > -1 && (pos == -1 || posFount < pos))
                {
                    pos = posFount;
                }
            }
            return pos > -1 ? subStr.Substring(0, pos).Trim() : subStr.Trim();
        }

        /// <summary>
        /// Retrieves a substring from this instance. The substring is from position 0 to the position of <paramref name="to"/> string <br/>
        /// If position of <paramref name="to"/> is less 0, the incoming string is returned.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static string SubstringFromAdnIncludeToString(this string value, string from, params string[] to)
        {
            var pos = value.IndexOf(from, CurrentIgnoreCaseStringComparison);

            if (pos <= -1)
            {
                return value;
            }

            var subStr = value.Substring(pos);
            pos = -1;
            var toString = string.Empty;
            foreach (var s in to)
            {
                toString = s;
                var posFount = subStr.IndexOf(s, from.Length - 1, CurrentIgnoreCaseStringComparison);
                if (posFount > -1 && (pos == -1 || posFount < pos))
                {
                    pos = posFount;
                }
            }
            return pos > -1 ? $"{subStr.Substring(0, pos).Trim()}{toString}" : subStr.Trim();
        }

        /// <summary>
        /// Remove comments from sql
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SqlWithoutComment(this string value)
        {
            var sqlWithoutComment =  string.Join("\n",value.ToMultiLine().Where(x => x.Trim().StartsWith("/*") == false && x.Trim().EndsWith("*/") == false && x.Trim().StartsWith("--") == false).Where(IsNotEmpty).ToArray());
            return sqlWithoutComment;
        }

        /// <summary>
        /// Strip sql for logging
        /// </summary>
        /// <param name="value"></param>
        /// <param name="additionalSqlTitleConverterFunc"></param>
        /// <returns></returns>
        public static string ConvertToSqlTitle(this string value, Func<string, string> additionalSqlTitleConverterFunc = null)
        {
            value = value.TrimWhiteSpaces();
            value = StripForLoggingRemoveComment(value, out var isComment, out var isOneLine);
            if (isComment)
            {
                return isOneLine ? StripForLoggingRemoveTitlePrefix(value, ref isComment) : value;
            }

            if (additionalSqlTitleConverterFunc != null)
            {
                var newValue = additionalSqlTitleConverterFunc.Invoke(value);
                if (!newValue.EqualsIgnoreCase(value))
                {
                    return newValue;
                }
            }

            value = StripForLoggingRemoveCreateStatement(value, isOneLine);
            return value;
        }

        private static string StripForLoggingRemoveComment(string value, out bool isComment, out bool isOneLine)
        {
            isOneLine = false;
            isComment = false;
            if (IsOneLine(value))
            {
                isOneLine = true;
                if (value.StartsWith("--"))
                {
                    isComment = true;
                    return value;
                }
            }

            if (value.StartsWith("/*"))
            {
                isComment = true;
                value = value.Replace("/*", "").Replace("*/", "");
                var strings = value.Split('\n').Select(x => $"-- {x.TrimWhiteSpaces()}").ToArray();
                value = string.Join("\n", strings);
                return value;
            }

            value = StripForLoggingRemoveTitlePrefix(value, ref isComment);

            return value;
        }

        internal static string StripForLoggingRemoveTitlePrefix(this string value, ref bool isTitleComment)
        {
            var titlePattern = "\\-\\- Title( |= |=| = | =)";
            var match = Regex.Match(value, titlePattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                isTitleComment = true;

                if (value.ContainsIgnoreCase("-- EndTitle"))
                {
                    value = value.SubstringFrom(match.Value, "-- EndTitle").TrimEndWhiteSpaces();
                }
                else
                {

                    var lines = value.Split('\n');
                    if (lines.Length > 1)
                    {
                        foreach (var line in lines)
                        {
                            if (line.StartsWithIgnoreCase("-- Title")) continue;
                            value = value.SubstringTo(line).TrimWhiteSpaces();
                            break;
                        }
                    }
                }

                value = Regex.Replace(value, titlePattern, "", RegexOptions.IgnoreCase);

                value = Regex.Replace(value, "(^)(= |=)", "", RegexOptions.Multiline).Replace("--", "");
                value = "-- Title " + value.TrimWhiteSpaces();
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string StripForLoggingRemoveCreateStatement(string value, bool? isOneLineOrNull = null)
        {
            var isOneLine = isOneLineOrNull ?? value.IsOneLine();

            value = StripForLoggingRemoveCreateUser(value);
            value = StripForLoggingRemoveCreatePackageBody(value);
            value = StripForLoggingRemoveCreatePackage(value);
            value = StripForLoggingRemoveCreateSequence(value);
            value = StripForLoggingRemoveCreateSynonyms(value);
            value = StripForLoggingRemoveCreateProcedure(value);
            value = StripForLoggingRemoveCreateFunction(value);
            value = StripForLoggingRemoveCreateIndex(value);
            value = StripForLoggingRemoveCreateTable(value, out var isCreateTable);
            if (isCreateTable == false)
            {
                value = StripForLoggingRemoveAlterTable(value);
                value = StripForLoggingRemoveCreateComment(value, out _);
            }
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string StripForLoggingRemoveCreateUser(string value)
        {
            var org = value;
            var search = "CREATE USER".ToTitleCase(true);
            value = value.SubstringFrom(search, "IDENTIFIED", "ENABLE", "ACCOUNT", "DEFAULT", "TEMPORARY", "TABLESPACE", "\n");
            if (org.EqualsIgnoreCase(value))
            {
                return org;
            }
            value = $"{search.ReplaceIgnoreCase(search, search)} [{value.ReplaceIgnoreCase(search, "").Trim()}]";
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string StripForLoggingRemoveCreatePackageBody(string value)
        {
            var org = value;
            var search = "CREATE OR REPLACE PACKAGE BODY".ToTitleCase(true);
            value = value.SubstringFrom(search, " IS ", " IS\n", " IS", " AS ", " AS\n", " AS", "\n");
            if (org.EqualsIgnoreCase(value))
            {
                return org;
            }

            value = $"Create Package Body [{value.ReplaceIgnoreCase(search, "").Trim()}]";
            return value;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string StripForLoggingRemoveCreatePackage(string value)
        {
            var org = value;
            var search = "CREATE OR REPLACE PACKAGE".ToTitleCase(true);
            value = value.SubstringFrom(search, " IS ", " IS\n", " IS", " AS ", " AS\n", " AS", "\n");
            if (org.EqualsIgnoreCase(value))
            {
                return org;
            }

            value = $"Create Package [{value.ReplaceIgnoreCase(search, "").Trim()}]";
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string StripForLoggingRemoveCreateSequence(string value)
        {
            var org = value;
            var search = "CREATE SEQUENCE".ToTitleCase(true);
            value = value.SubstringFrom(search, "minvalue", "\n");
            if (org.EqualsIgnoreCase(value))
            {
                return org;
            }
            value = $"{search.ReplaceIgnoreCase(search, search)} [{value.ReplaceIgnoreCase(search, "").Trim()}]";
            return value;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string StripForLoggingRemoveCreateSynonyms(string value)
        {
            var org = value;
            var search = "CREATE OR REPLACE SYNONYM".ToTitleCase(true);
            value = value.SubstringFrom(search, ";", "\n");
            if (org.EqualsIgnoreCase(value) && !org.ContainsIgnoreCase(search))
            {
                return org;
            }
            value = $"Create Synonym [{value.ReplaceIgnoreCase(search, "").Trim()}]";
            return value;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string StripForLoggingRemoveCreateProcedure(string value)
        {
            var org = value;
            var search = "CREATE OR REPLACE PROCEDURE".ToTitleCase(true);
            value = value.SubstringFrom(search, "(", " IS ", " IS\n", " IS", " AS ", " AS\n", " AS", "\n");
            if (org.EqualsIgnoreCase(value))
            {
                return org;
            }

            value = $"Create Procedure [{value.ReplaceIgnoreCase(search, "").Trim()}]";
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string StripForLoggingRemoveCreateFunction(string value)
        {
            var org = value;
            var search = "CREATE OR REPLACE FUNCTION".ToTitleCase(true);
            value = value.SubstringFrom(search, "(", "Return", "\n");
            if (org.EqualsIgnoreCase(value))
            {
                return org;
            }

            value = $"Create Function [{value.ReplaceIgnoreCase(search, "").Trim()}]";
            return value;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string StripForLoggingRemoveCreateIndex(string value)
        {
            var org = value;
            var search = "CREATE INDEX".ToTitleCase(true);
            value = value.SubstringFrom(search, "(", "TABLESPACE", "\n");
            if (org.EqualsIgnoreCase(value))
            {
                return org;
            }

            var trim = value.ReplaceIgnoreCase(search, "").Trim();
            if (trim.ContainsIgnoreCase(" on ") == false)
            {
                return org;
            }

            value = $"Create Index [{trim.ReplaceIgnoreCase(" on ", " => ")}{org.SubstringFromAdnIncludeToString("(", ")")}]";
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isCreateTable"></param>
        /// <returns></returns>
        public static string StripForLoggingRemoveCreateTable(string value, out bool isCreateTable)
        {
            var org = value;
            var search = "CREATE TABLE".ToTitleCase(true);
            value = value.SubstringFrom(search, "(", "\n");
            isCreateTable = org.EqualsIgnoreCase(value) == false || org.StartsWithIgnoreCase(search);
            if (isCreateTable == false)
            {
                return org;
            }

            var trim = value.ReplaceIgnoreCase(search, "").TrimWhiteSpaces();
            var pos = org.IndexOf("(", StringComparison.OrdinalIgnoreCase);
            if (pos == -1)
            {
                return org.StartsWithIgnoreCase(search) ? $"{search} [{trim}]" : org;
            }

            var substring = org.Substring(pos + 1).SubstringTo(";").Replace("\n", " ");
            var arr = substring.Split(',').Select(x => x.TrimStart().SubstringTo(" ").TrimWhiteSpaces('(', ')', ',')).Where(x => string.IsNullOrWhiteSpace(x) == false).Where(y => int.TryParse(y, out _) == false).Where(x => x.StartsWithIgnoreCase("CONSTRAINT") == false).ToArray();
            var colStr = string.Join(", ", arr);
            isCreateTable = true;
            value = $"{search} [{trim} ({colStr})]";
            var comment = StripForLoggingRemoveCreateComment(org, out var isAddComment);
            if (isAddComment && comment.IsNotEmpty())
            {
                comment = comment.ReplaceIgnoreCase("Add ", "With ").ReplaceIgnoreCase($"{trim}.", "");
                value = $"{value}\n{comment}";
            }
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string StripForLoggingRemoveAlterTable(string value)
        {
            var org = value;
            var search = "ALTER TABLE".ToTitleCase(true);
            value = value.SubstringFrom(search, "add", "remove", "rename", "drop", "modify", "\n", ";");
            if (org.EqualsIgnoreCase(value))
            {
                return org;
            }

            value = $"{search} [{value.ReplaceIgnoreCase(search, "").Trim()}]";
            return value;
        }


        public static string StripForLoggingRemoveCreateComment(string value, out bool isAddComment)
        {
            isAddComment = false;
            var isAddCommentValue = false;
            var isOneLine = value.IsOneLine(';');

            var search = "comment on column".ToTitleCase(true);

            if (isOneLine)
            {
                var value2 = value.SubstringFrom(search, "is", ";", "\n").Replace("\n", " ");

                value = value.EqualsIgnoreCase(value2) ? value2 : ToTitle(value);
                isAddComment = isAddCommentValue;
                return value;
            }

            var lines = value.ToMultiLine();
            if (lines.Length == 1)
            {
                lines = value.ToMultiLine(';');
            }

            var parsedLines = lines.Select(ToTitle).Where(IsNotEmpty).ToList();
            isAddComment = isAddCommentValue;
            return string.Join("\n", parsedLines);

            string ToTitle(string valueToParse)
            {
                if (valueToParse.ContainsIgnoreCase(search) == false)
                {
                    return null;
                }
                var comment = valueToParse.SubstringFrom("is '").Substring(2).TrimWhiteSpaces(';');
                valueToParse = valueToParse.SubstringFrom(search, "is", "\n");
                var addColumnComment = "Add Column Comment";
                var parsed = $"{addColumnComment} [{valueToParse.ReplaceIgnoreCase(search, "").TrimWhiteSpaces(';')} => {comment}]";
                parsed = parsed.Replace("'''", "'").Replace("''", "'");
                isAddCommentValue = parsed.StartsWithIgnoreCase(addColumnComment);
                return parsed;
            }
        }

        public static string[] SplitOnCapitalLetters(this string @string)
        {
            var r = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

            return r.Split(@string);
        }

        public static string ToTitleCase(this string @string, bool toLower = false)
        {
            if (toLower)
            {
                @string = @string.ToLower();
            }
            return string.Join(" ",
                @string
                    .Split(' ')
                    .Select(x => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(x.Trim()))
                    .ToArray());
        }

        /// <summary>
        /// <para>Convert string to <see cref="IDictionary"/>&lt;<see cref="string"/>,<see cref="string"/>&gt; </para>
        /// <para>
        /// Must be on format Key1=Value1,Key2=Value2<br/>
        /// OR Key1=Value1;Key2=Value2
        /// </para>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IDictionary<string, string> ToDictionary(this string value)
        {
            var splitChar = value.Contains(",") ? ',' : ';';
            var split = value.Split(splitChar);
            if (!split.Any())
            {
                return null;
            }

            var dict = new Dictionary<string, string>();
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var keyValueString in split)
            {
                var keyValue = keyValueString.Trim().Split('=');
                if (keyValue.Length <= 1)
                {
                    continue;
                }
                dict.Add(keyValue[0].Trim(), keyValue[1].Trim());
            }

            return dict;

        }
    }
}