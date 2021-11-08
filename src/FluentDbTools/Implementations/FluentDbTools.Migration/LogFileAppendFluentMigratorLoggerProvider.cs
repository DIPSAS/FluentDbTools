using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FluentDbTools.Common.Abstractions;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Logging;
using Microsoft.Extensions.Options;
[assembly: InternalsVisibleTo("Test.FluentDbTools.Migration")]
namespace FluentDbTools.Migration
{
    /// <summary>
    /// Append the SQL statements to then end of a log file
    /// </summary>
    public class LogFileAppendFluentMigratorLoggerProvider : SqlScriptFluentMigratorLoggerProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogFileAppendFluentMigratorLoggerProvider"/> class.<br/>
        /// Will try 10 times with wait of 250ms to create StreamWriter if IOException is thrown 
        /// </summary>
        /// <param name="assemblySource">The assembly source</param>
        /// <param name="options">The log file logger options</param>
        /// <param name="disposeWriter">A value indicating whether the internal writer should be disposed by this logger provider</param>
        public LogFileAppendFluentMigratorLoggerProvider(
                IAssemblySource assemblySource,
                IOptions<LogFileFluentMigratorLoggerOptions> options,
                bool disposeWriter = true)
            : base(GetStreamWriter(assemblySource, options?.Value, out _), options?.Value, disposeWriter)
        {
        }

        internal static StreamWriter GetStreamWriter(IAssemblySource assemblySource, LogFileFluentMigratorLoggerOptions options, out string logFilename)
        {
            const int retryInitStreamWriter = 2;

            for (var i = 0; i < 10; i++)
            {
                // ReSharper disable once StringLiteralTypo
                var postfix = i == 0 ? null : $"{DateTime.Now:HHmmss}";
                logFilename = GetOutputFileName(assemblySource, options, postfix);
                if (GetStreamWriterLocal(logFilename, out var streamWriter))
                {
                    return streamWriter;
                }
            }

            logFilename = GetOutputFileName(assemblySource, options, new Random().Next(100000, 999999).ToString());
            return new StreamWriter(logFilename, true, Encoding.UTF8);

            bool GetStreamWriterLocal(string logFile, out StreamWriter streamWriter)
            {
                streamWriter = null;
                for (var retries = 0; retries < retryInitStreamWriter; retries++)
                {
                    try
                    {
                        streamWriter = new StreamWriter(logFile, true, Encoding.UTF8);
                        return true;
                    }
                    catch (Exception)
                    {
                        Task.Delay(TimeSpan.FromMilliseconds(250)).Wait();
                    }
                }

                return false;
            }
        }

        internal static string GetOutputFileName(
            IAssemblySource assemblySource,
            LogFileFluentMigratorLoggerOptions options,
            string postfix = null)
        {
            string logFile = null;
            if (!string.IsNullOrEmpty(options.OutputFileName))
            {
                logFile = options.OutputFileName;
            }

            if (logFile == null && assemblySource.Assemblies.Count == 0)
            {
                logFile = "FluentMigratorSql.log";
            }

            if (logFile == null)
            {
                var assembly = assemblySource.Assemblies.First();
                logFile = assembly.Location + ".sql";
            }

            var info = new FileInfo(logFile);

            if (info.DirectoryName != null)
            {
                var directoryInfo = new DirectoryInfo(info.DirectoryName);
                var subs = new List<string>();
                while (directoryInfo != null && directoryInfo.Exists == false)
                {
                    subs.Add(directoryInfo.Name);
                    directoryInfo = directoryInfo.Parent;
                }

                if (subs.Any())
                {
                    subs.Reverse();
                    subs.Aggregate(directoryInfo, (current, sub) => current?.CreateSubdirectory(sub));

                }
            }

            if (info.Exists == false)
            {
                return logFile;
            }

            if (postfix == null)
            {
                return logFile;
            }


            if (info.DirectoryName != null && Directory.GetCurrentDirectory().EqualsIgnoreCase(info.DirectoryName) == false)
            {
                logFile = Path.Combine(info.DirectoryName, $"{info.Name.SubstringTo(".")}-{postfix}{info.Extension}");
            }
            else
            {
                logFile = Path.Combine($"{info.Name.SubstringTo(".")}-{postfix}{info.Extension}");
            }
            

            return logFile;
        }

    }
}