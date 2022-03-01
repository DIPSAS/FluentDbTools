using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentDbTools.Common.Abstractions;
// ReSharper disable UnusedMember.Global
#pragma warning disable 1591

// ReSharper disable once CheckNamespace
namespace System.Data
{
    public static class SystemDataExtensions
    {
        private static Exception LatestFilteredException;

        [SuppressMessage("ReSharper", "InvertIf")]
        public static int ExecuteNonQuery(this IDbCommand command, string errorFilter)
        {
            try
            {
                LatestFilteredException = null;
                return command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                if (exception.IsErrorFilterNumberMatchAndReturnErrorNumber(GetErrorFilterNumbers(errorFilter), out var errorNumber))
                {
                    LatestFilteredException = exception;
                    return 0 - errorNumber;
                }

                throw;
            }
        }

        public static int ExecuteNonQuery(this IDbCommand command, params long[] errorFilterNumbers)
        {
            try
            {
                LatestFilteredException = null;
                return command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                if (exception.IsErrorFilterNumberMatchAndReturnErrorNumber(errorFilterNumbers, out var errorNumber))
                {
                    LatestFilteredException = exception;
                    return 0 - errorNumber;
                }

                throw;
            }
        }

        public static Exception GetLatestFilteredException(this IDbCommand _)
        {
            return LatestFilteredException;
        }

        public static long[] GetErrorFilterNumbers(this string errorFilter)
        {
            var errorFilterNumbers = new List<long>();
            if (!string.IsNullOrEmpty(errorFilter))
            {
                if (errorFilter.ContainsIgnoreCase("ErrorFilter"))
                {
                    var split = errorFilter.Split('=').Select(x => x.Trim('/', '\\', '*', ' '))
                        .Where(x => !string.IsNullOrEmpty(x)).ToArray();

                    for (var i = 0; i < split.Count(); i++)
                    {
                        if (split[i].ContainsIgnoreCase("ErrorFilter"))
                        {
                            var foundErrorFilter = split[i + 1];
                            foreach (var errorFilterItem in foundErrorFilter.Split(',').Where(x => !string.IsNullOrEmpty(x))
                                .Select(x => x.ReplaceIgnoreCase("ORA-", "")))
                            {
                                if (long.TryParse(errorFilterItem, out var errorNumber))
                                {
                                    errorFilterNumbers.Add(errorNumber);
                                }
                            }
                        }
                    }
                }
            }

            return errorFilterNumbers.ToArray();
        }

        [SuppressMessage("ReSharper", "InvertIf")]
        public static bool IsErrorFilterNumberMatch(this Exception exception, long[] errorFilterNumbers)
        {
            return IsErrorFilterNumberMatchAndReturnErrorNumber(exception, errorFilterNumbers, out _);
        }

        [SuppressMessage("ReSharper", "InvertIf")]
        public static bool IsErrorFilterNumberMatchAndReturnErrorNumber(this Exception exception, long[] errorFilterNumbers, out int number)
        {
            number = 0;
            if (exception.GetType().Name != "OracleException")
            {
                return false;
            }

            // ReSharper disable once InvertIf
            if (errorFilterNumbers?.Any() == true)
            {
                var actualErrors = new List<long>();
                while (exception != null)
                {
                    var message = exception.Message;
                    if (message.StartsWithIgnoreCase("ORA-"))
                    {
                        if (int.TryParse(message.SubstringTo(":").Replace(":", "").ReplaceIgnoreCase("ORA-", ""), out number))
                        {
                            actualErrors.Add(number);
                        }
                    }

                    exception = exception.InnerException;
                }


                if (actualErrors.Any())
                {
                    actualErrors = actualErrors.Distinct().ToList();
                    number = (int)actualErrors.FirstOrDefault();
                    return actualErrors.All(errorFilterNumbers.Contains);
                }
            }

            return false;
        }

    }
}