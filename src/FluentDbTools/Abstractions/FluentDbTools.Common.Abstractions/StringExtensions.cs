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
            return string.IsNullOrEmpty(value);
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
            return int.TryParse(value, out var parsedValue) ? parsedValue : defaultValue;
        }

        /// <summary>
        /// Convert 'value' to long?
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static long? TryConvertToLong(this string value, long? defaultValue = null)
        {
            return long.TryParse(value, out var parsedValue) ? parsedValue : defaultValue;
        }

        /// <summary>
        /// Convert 'value' to double?
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double? TryConvertToDouble(this string value, double? defaultValue = null)
        {
            return double.TryParse(value, out var parsedValue) ? parsedValue : defaultValue;
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
            prefix = prefix ?? defaultPrefix ?? string.Empty;

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
            prefix = prefix ?? defaultPrefix ?? string.Empty;
            if (prefix.IsEmpty() || name.IsEmpty())
            {
                return name;
            }

            return name.StartsWith(prefix, StringComparison.Ordinal) ? name.Substring(prefix.Length) : name;
        }
    }
}