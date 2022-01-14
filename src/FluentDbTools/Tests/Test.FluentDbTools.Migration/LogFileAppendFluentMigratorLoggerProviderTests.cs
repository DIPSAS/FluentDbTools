using System.IO;
using Example.FluentDbTools.Config;
using FluentAssertions;
using FluentDbTools.Common.Abstractions;
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

            var options = new LogFileFluentMigratorLoggerOptions() { OutputFileName = Path.Combine("sub0", "sub1", "abcd.log") };
            var streamWriter1 = LogFileAppendFluentMigratorLoggerProvider.GetStreamWriter(null, options, out var logFile);
            streamWriter1.Should().NotBeNull();
            logFile.EndsWithIgnoreCase(options.OutputFileName).Should().BeTrue($"'{logFile}' should be same as '{options.OutputFileName}'"); ;
            logFile.Should().EndWith(options.OutputFileName);
            for (var i = 0; i < 10; i++)
            {
                var streamWriter2 = LogFileAppendFluentMigratorLoggerProvider.GetStreamWriter(null, options, out logFile);
                streamWriter2.Should().NotBeNull();
                logFile.EndsWithIgnoreCase(options.OutputFileName).Should().Be(BaseConfig.InContainer, $"'{logFile}' should {(BaseConfig.InContainer ? "be same as" : "differ from" )} '{options.OutputFileName}'");
            }

            streamWriter1.Close();

            var streamWriter3 = LogFileAppendFluentMigratorLoggerProvider.GetStreamWriter(null, options, out logFile);
            streamWriter3.Should().NotBeNull();

            logFile.EndsWithIgnoreCase(options.OutputFileName).Should().BeTrue($"'{logFile}' should be same as '{options.OutputFileName}'");
        }
    }
}