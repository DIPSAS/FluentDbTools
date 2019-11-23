using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

// ReSharper disable UnusedMember.Global

namespace FluentDbTools.Common.Abstractions
{
    /// <summary>
    /// String extension functions
    /// </summary>
    public static class StringExtensions
    {
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
        /// Contains(searchValue) with StringComparison.CurrentCultureIgnoreCase
        /// </summary>
        /// <param name="value"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static bool ContainsIgnoreCase(this string value, string searchValue)
        {
            return value.Contains(searchValue, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// EndsWith(searchValue) with StringComparison.CurrentCultureIgnoreCase
        /// </summary>
        /// <param name="value"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static bool EndsWithIgnoreCase(this string value, string searchValue)
        {
            return value.EndsWith(searchValue, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// EndsWith(searchValue) with StringComparison.CurrentCultureIgnoreCase
        /// </summary>
        /// <param name="value"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static bool StartsWithIgnoreCase(this string value, string searchValue)
        {
            return value.StartsWith(searchValue, StringComparison.CurrentCultureIgnoreCase);
        }


        /// <summary>
        /// Equals(searchValue) with StringComparison.CurrentCultureIgnoreCase
        /// </summary>
        /// <param name="value"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(
            this string value,
            string searchValue)
        {
            return value?.Equals(searchValue, StringComparison.CurrentCultureIgnoreCase) ?? false;
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
            var pos = value.IndexOf(oldValue, StringComparison.CurrentCultureIgnoreCase);
            if (pos == -1)
            {
                return value;
            }

            var oldValueToReplace = value.Substring(pos, oldValue.Length);

            return value.Replace(oldValueToReplace, newValue);
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
            var pos = value.IndexOf(to, StringComparison.CurrentCultureIgnoreCase);
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
        public static string SubstringFrom(this string value, string from, params string [] to)
        {
            var pos = value.IndexOf(from, StringComparison.CurrentCultureIgnoreCase);

            if (pos <= -1)
            {
                return value;
            }

            var subStr = value.Substring(pos);
            pos = -1;
            foreach (var s in to)
            {
                var posFount = subStr.IndexOf(s, from.Length - 1,  StringComparison.CurrentCultureIgnoreCase);
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
        public static string SubstringFromAdnIncludeToString(this string value, string from, params string [] to)
        {
            var pos = value.IndexOf(from, StringComparison.CurrentCultureIgnoreCase);

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
                var posFount = subStr.IndexOf(s, from.Length - 1,  StringComparison.CurrentCultureIgnoreCase);
                if (posFount > -1 && (pos == -1 || posFount < pos))
                {
                    pos = posFount;
                }
            }
            return pos > -1 ? $"{subStr.Substring(0, pos).Trim()}{toString}" : subStr.Trim();
        }



        /// <summary>
        /// Strip sql for logging
        /// </summary>
        /// <param name="value"></param>
        /// <param name="additionalSqlTitleConverterFunc"></param>
        /// <returns></returns>
        public static string ConvertToSqlTitle(this string value, Func<string,string> additionalSqlTitleConverterFunc = null)
        {
            value = StripForLoggingRemoveComment(value, out var isComment);
            if (isComment)
            {
                return value;
            }
            if (additionalSqlTitleConverterFunc != null)
            {
                var newValue = additionalSqlTitleConverterFunc.Invoke(value);
                if (!newValue.EqualsIgnoreCase(value))
                {
                    return newValue;
                }
            }

            value = StripForLoggingRemoveCreateStatement(value);
            return value;
        }

        private static string StripForLoggingRemoveComment(string value, out bool isComment)
        {
            isComment = false;
            if (value.Split('\n').Count(IsNotEmpty) == 1)
            {
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
                var strings = value.Split('\n').Select(x => $"-- {x.Trim()}").ToArray();
                value = string.Join("\n", strings);
                return value;
            }

            if (value.ContainsIgnoreCase("-- Title "))
            {
                isComment = true;
                if (value.ContainsIgnoreCase("-- EndTitle"))
                {
                    value = value.SubstringFrom("-- Title ", "-- EndTitle").TrimEnd(' ','\n');
                    value = value.ReplaceIgnoreCase("-- Title ", "").Replace("--", "");
                    value = "-- Title " + value.Trim('\n',' ');
                    return value;
                }
                value = value.SubstringFrom("-- Title ", "\n");
                return value;
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string StripForLoggingRemoveCreateStatement(string value)
        {
            value = StripForLoggingRemoveCreateUser(value);
            value = StripForLoggingRemoveCreatePackageBody(value);
            value = StripForLoggingRemoveCreatePackage(value);
            value = StripForLoggingRemoveCreateSequence(value);
            value = StripForLoggingRemoveCreateSynonyms(value);
            value = StripForLoggingRemoveCreateProcedure(value);
            value = StripForLoggingRemoveCreateFunction(value);
            value = StripForLoggingRemoveCreateIndex(value);
            value = StripForLoggingRemoveCreateComment(value);
            value = StripForLoggingRemoveCreateTable(value);
            value = StripForLoggingRemoveAlterTable(value);
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
            value = value.SubstringFrom(search, "IDENTIFIED" ,"ENABLE", "ACCOUNT", "DEFAULT","TEMPORARY","TABLESPACE", "\n");
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
            value = value.SubstringFrom(search, " IS " ," IS\n", " IS"," AS " ," AS\n", " AS", "\n");
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
            value = value.SubstringFrom(search, " IS " ," IS\n", " IS"," AS " ," AS\n", " AS", "\n");
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
            value = value.SubstringFrom(search, "minvalue" ,"\n");
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
            value = value.SubstringFrom(search, "(", " IS " ," IS\n", " IS"," AS " ," AS\n", " AS", "\n");
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

            value = $"Create Index [{trim.ReplaceIgnoreCase(" on ", " => ")}{org.SubstringFromAdnIncludeToString("(",")")}]";
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string StripForLoggingRemoveCreateTable(string value)
        {
            var org = value;
            var search = "CREATE TABLE".ToTitleCase(true);
            value = value.SubstringFrom(search, "(", "\n");
            if (org.EqualsIgnoreCase(value))
            {
                return org;
            }

            var trim = value.ReplaceIgnoreCase(search, "").Trim();
            if (!org.ContainsIgnoreCase("("))
            {
                return org;
            }
            value = $"{search} [{trim}]";
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
            value = value.SubstringFrom(search, "add", "remove", "\n");
            if (org.EqualsIgnoreCase(value))
            {
                return org;
            }

            value = $"{search} [{value.ReplaceIgnoreCase(search, "").Trim()}]";
            return value;
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string StripForLoggingRemoveCreateComment(string value)
        {
            var org = value;
            var search = "comment on column".ToTitleCase(true);
            value = value.SubstringFrom(search, "is", "\n");
            if (org.EqualsIgnoreCase(value))
            {
                return org;
            }

            var comment = org.SubstringFrom("is '").Substring(2).Trim();
            value = $"Add Column Comment [{value.ReplaceIgnoreCase(search, "").Trim()} => {comment}]";
            return value;
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
    }
}