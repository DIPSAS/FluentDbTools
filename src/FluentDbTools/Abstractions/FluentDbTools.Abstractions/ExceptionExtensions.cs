using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
#pragma warning disable CS1591

namespace FluentDbTools.Common.Abstractions
{
    public static class ExceptionExtensions
    {
        public const string OneOfTheRequiredConfigurationParameters = "One of the required configuration parameters";

        public const string AllRequiredConfigurationParameters = "All required configuration parameters";

        public const string RequiredConfigurationParameter = "Required configuration parameter";

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