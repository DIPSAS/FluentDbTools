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
        [SuppressMessage("ReSharper", "InvertIf")]
        public static int ExecuteNonQuery(this IDbCommand command, string errorFilter)
        {
            try
            {
                return command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                if (exception.IsErrorFilterNumberMatch(GetErrorFilterNumbers(errorFilter)))
                {
                    return 0;
                }

                throw;
            }
        }

        public static int ExecuteNonQuery(this IDbCommand command, params long[] errorFilterNumbers)
        {
            try
            {
                return command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                if (exception.IsErrorFilterNumberMatch(errorFilterNumbers))
                {
                    return 0;
                }

                throw;
            }


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
            if (exception.GetType().Name != "OracleException")
            {
                return false;
            }

            // ReSharper disable once InvertIf
            if (errorFilterNumbers.Any())
            {
                if (exception.Message.StartsWithIgnoreCase("ORA-"))
                {
                    if (long.TryParse(exception.Message.SubstringTo(":").Replace(":","").ReplaceIgnoreCase("ORA-", ""), out var number))
                    {
                        if (errorFilterNumbers.Contains(number))
                        {
                            return true;
                        }

                    }
                }
            }

            return false;
        }

    }
}