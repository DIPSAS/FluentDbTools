using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Logging;
using Microsoft.Extensions.Options;

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
            : base(GetStreamWriter(assemblySource, options), options.Value, disposeWriter)
        {
        }

        private static StreamWriter GetStreamWriter(IAssemblySource assemblySource, IOptions<LogFileFluentMigratorLoggerOptions> options)
        {
            const int retryInitStreamWriter = 10;
            Exception fileException = null;
            var absolutePath = GetOutputFileName(assemblySource, options.Value);

            for (var retries = 0; retries < retryInitStreamWriter; retries++)
            {
                try
                {
                    return new StreamWriter(absolutePath, true, Encoding.UTF8);
                }
                catch (IOException exception)
                {
                    fileException = exception;
                    Task.Delay(TimeSpan.FromMilliseconds(250)).Wait();
                }
            }

            if (fileException != null)
            {
                throw fileException;
            }

            return new StreamWriter(absolutePath, true, Encoding.UTF8);
        }

        private static string GetOutputFileName(
            IAssemblySource assemblySource,
            LogFileFluentMigratorLoggerOptions options)
        {
            if (!string.IsNullOrEmpty(options.OutputFileName))
                return options.OutputFileName;

            if (assemblySource.Assemblies.Count == 0)
                return "fluentmigrator.sql";

            var assembly = assemblySource.Assemblies.First();
            return assembly.Location + ".sql";
        }

    }
}