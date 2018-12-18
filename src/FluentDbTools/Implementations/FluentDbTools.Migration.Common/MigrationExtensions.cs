using System;
using System.Threading;
using FluentDbTools.Migration.Abstractions;

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

    }
}