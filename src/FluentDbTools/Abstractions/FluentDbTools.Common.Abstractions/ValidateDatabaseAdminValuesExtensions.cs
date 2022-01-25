using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FluentDbTools.Contracts;
#pragma warning disable CS1591

[assembly: InternalsVisibleTo("FluentDbTools.Extensions.DbProvider")]
namespace FluentDbTools.Common.Abstractions
{
    public static class ValidateDatabaseAdminValuesExtensions
    {
        internal const string OneOfTheRequiredConfigurationParameters = "One of the required configuration parameters";

        internal const string AllRequiredConfigurationParameters = "All required configuration parameters";

        internal const string RequiredConfigurationParameter = "Required configuration parameter";

        /// <summary>
        /// Convert from "section:section2:value" to "'Section.Section2.Value'";
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public static string ConvertConfigKeyToParamNameStyle(this string configKey, string group = "'")
        {
            configKey = configKey
                .ToTitleCase()
                .ReplaceIgnoreCase("user", "User")
                .ReplaceIgnoreCase("password", "Password")
                .Replace(":", ".");

            return $"{group}{configKey}{group}";
        }

        /// <summary>
        /// <para>Throws exception if <paramref name="invalidAdminValues"/> contains any elements</para>
        /// - If <paramref name="invalidAdminValues"/> contains many elements, <see cref="AggregateException"></see> will be thrown<br/>
        /// - If <paramref name="invalidAdminValues"/> contains only one element, <see cref="ArgumentNullException"></see> or <see cref="ArgumentException"></see> will be thrown
        /// </summary>
        /// <param name="invalidAdminValues"></param>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static void ThrowIfInvalidDatabaseAdminValues(this InvalidAdminValue[] invalidAdminValues)
        {
            if (invalidAdminValues == null || invalidAdminValues.Length == 0)
            {
                return;
            }

            if (invalidAdminValues.Length == 2)
            {
                throw new AggregateException(invalidAdminValues.Select(x => x.GeneratedArgumentException).Where(x => x != null))
                {
                    Source = invalidAdminValues?.FirstOrDefault()?.InvalidAdminType.ToString("G")
                };
            }

            var exception = invalidAdminValues.FirstOrDefault()?.GeneratedArgumentException;
            if (exception != null)
            {
                throw exception;
            }
        }

        public static bool IsInvalidDatabaseAdminException(this Exception exception)
        {
            if (exception == null)
            {
                return false;
            }

            var isInvalidException =
                   exception.Source == InvalidAdminType.AdminUser.ToString("G") ||
                   exception.Source == InvalidAdminType.AdminPassword.ToString("G");

            if (isInvalidException)
            {
                return true;
            }

            var messagesContainsStrings = new[] { OneOfTheRequiredConfigurationParameters, AllRequiredConfigurationParameters, RequiredConfigurationParameter };

            switch (exception)
            {
                case ArgumentNullException argumentNullException:
                    return argumentNullException.ParamName != null && messagesContainsStrings.Any(x => argumentNullException.Message.ContainsIgnoreCase(x));
                case ArgumentException argumentException:
                    return messagesContainsStrings.Any(x => argumentException.Message.ContainsIgnoreCase(x));
                case AggregateException aggregateException:
                    return aggregateException.InnerExceptions.Any(innerException => messagesContainsStrings.Any(x => IsInvalidDatabaseAdminException(innerException)));
                default:
                    return false;
            }
        }
    }
}