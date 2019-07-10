using System;

namespace FluentDbTools.Common.Abstractions
{
    public static class StringExtensions
    {
        public static string WithDefault(this string value, string defaultValue)
        {
            return value.IsEmpty() ? defaultValue : value;
        }

        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool ContainsIgnoreCase(this string value, string contains)
        {
            return value.Contains(contains, StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool EndsWithIgnoreCase(this string value, string contains)
        {
            return value.EndsWith(contains, StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool Contains(
            this string value,
            string contains,
            StringComparison stringComparison)
        {
            return (value?.IndexOf(contains, stringComparison) ?? -1) > -1;
        }
    }
}