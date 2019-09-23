using System;
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
    }
}