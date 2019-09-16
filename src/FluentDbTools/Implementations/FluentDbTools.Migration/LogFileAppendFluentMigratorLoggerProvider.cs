using System.IO;
using System.Linq;
using System.Text;
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
        /// Initializes a new instance of the <see cref="LogFileAppendFluentMigratorLoggerProvider"/> class.
        /// </summary>
        /// <param name="assemblySource">The assembly source</param>
        /// <param name="options">The log file logger options</param>
        /// <param name="disposeWriter">A value indicating whether the internal writer should be disposed by this logger provider</param>
        public LogFileAppendFluentMigratorLoggerProvider(
                IAssemblySource assemblySource,
                IOptions<LogFileFluentMigratorLoggerOptions> options,
                bool disposeWriter = true)
            : base(new StreamWriter(GetOutputFileName(assemblySource, options.Value), true, Encoding.UTF8), options.Value, disposeWriter)
        {
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