using System;
using System.Text.RegularExpressions;
using System.Threading;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Migration.Abstractions;
using FluentMigrator.Runner;
using Microsoft.Extensions.Logging;

namespace FluentDbTools.Migration.Common
{
    internal static class MigrationExtensions
    {
        public static void ExecuteCodeBlockUntilNoExeception(this IExtendedMigrationProcessor _, Action act, Action<Exception> errorMessageAction, int retryTimes = 10, int retryWaitInMs = 1000)
        {
            ExecuteCodeBlockUntilNoExeception(act,errorMessageAction,retryTimes,retryWaitInMs);
        }

        public static void ExecuteCodeBlockUntilNoExeception(Action act, Action<Exception> errorMessageAction, int retryTimes = 20, int retryWaitInMs = 200)
        {
            var retryWait = TimeSpan.FromMilliseconds(retryWaitInMs);
            try
            {
                act.Invoke();
            }
            catch (Exception ex)
            {
                if (--retryTimes <= 0)
                {
                    errorMessageAction.Invoke(ex);
                    throw;
                }
                Thread.Sleep(retryWait);

                ExecuteCodeBlockUntilNoExeception(act, errorMessageAction, retryTimes, retryWaitInMs);
            }
        }

        public static void LogSqlInternal(this ILogger logger, string sql, bool logSql = true)
        {
            var forLogging = sql.ConvertToSqlTitle();
            if (forLogging != sql)
            {
                forLogging = Regex.Replace(forLogging, "(^)-- ", "");
                logger.LogSay(forLogging);
            }

            if (logSql == false)
            {
                return;
            }

            var sqlToLog = sql.IsSqlComment() ? sql.SqlWithoutComment() : sql;
            if (sqlToLog.IsNotEmpty())
            {
                logger.LogSql(sqlToLog);
            }

        }
    }
}