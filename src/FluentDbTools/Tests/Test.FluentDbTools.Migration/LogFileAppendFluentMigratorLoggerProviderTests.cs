using System.IO;
using FluentAssertions;
using FluentDbTools.Migration;
using FluentMigrator.Runner.Logging;
using Xunit;

namespace Test.FluentDbTools.Migration
{
    public class LogFileAppendFluentMigratorLoggerProviderTests
    {
        [Fact]
        public void GetStreamWriter()
        {
            if (Directory.Exists(@"sub0"))
            {
                Directory.Delete(@"sub0", true);
            }

            var options = new LogFileFluentMigratorLoggerOptions() { OutputFileName = @"sub0\sub1\abcd.log"};
            var streamWriter1 = LogFileAppendFluentMigratorLoggerProvider.GetStreamWriter(null, options, out var logFile);
            streamWriter1.Should().NotBeNull();
            logFile.Should().EndWith(options.OutputFileName);
            for (var i = 0; i < 10; i++)
            {
                var streamWriter2 = LogFileAppendFluentMigratorLoggerProvider.GetStreamWriter(null, options, out logFile);
                streamWriter2.Should().NotBeNull();
                logFile.Should().NotEndWith(options.OutputFileName);
            }

            streamWriter1.Close();

            var streamWriter3 = LogFileAppendFluentMigratorLoggerProvider.GetStreamWriter(null, options, out logFile);
            streamWriter3.Should().NotBeNull();
            logFile.Should().EndWith(options.OutputFileName);

        }
    }
}