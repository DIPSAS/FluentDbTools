using FluentAssertions;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Extensions.DbProvider;
using System;
using System.IO;
using Xunit;

namespace Test.FluentDbTools.DbProvider
{
    public class DbProviderExtensionsTest
    {
        [Fact]
        public void ConfigureOracleTnsAdminPath_ValidPaths_ShouldReturnResolvedPath()
        {
            var str = Path.Combine("oracle", "product", "client", "network", "admin");
            DbProviderExtensions.ConfigureOracleTnsAdminPath(Path.Combine("oracle", "product", "client", "bin")).Should().EndWithEquivalentOf(str);
            DbProviderExtensions.ConfigureOracleTnsAdminPath(str).Should().EndWithEquivalentOf(str);
        }

        [Fact]
        public void ConfigureOracleTnsAdminPath_InvalidPaths_ShouldReturnEmptyPath()
        {
            DbProviderExtensions.ConfigureOracleTnsAdminPath(Path.Combine("oracle", "product")).Should().BeNullOrEmpty();
            DbProviderExtensions.ConfigureOracleTnsAdminPath(Path.Combine("network", "admin")).Should().BeNullOrEmpty();
            DbProviderExtensions.ConfigureOracleTnsAdminPath(Path.Combine("main1", "level1", "level2")).Should().BeNullOrEmpty();
            DbProviderExtensions.ConfigureOracleTnsAdminPath(Path.Combine("main2", "level1", "level2")).Should().BeNullOrEmpty();
        }

        [Fact]
        public void ConfigureOracleTnsAdminPath_EnvironmentResolved_ShouldReturnResolvedPath()
        {
            var expected = Path.Combine("oracle", "product", "client", "network", "admin");

            var environmentPath1 = Path.Combine("main1", "level1", "level2") +
                                   $"{Path.PathSeparator}{Path.Combine("network", "admin")}" +
                                   $"{Path.PathSeparator}{Path.Combine("oracle", "product", "client", "bin")}" +
                                   $"{Path.PathSeparator}{Path.Combine("main2", "level1", "level2")}";
            DbProviderExtensions.ConfigureOracleTnsAdminPath(string.Empty, environmentPath1).Should().EndWithEquivalentOf(expected);
            
            var environmentPath2 = Path.Combine("main1", "level1", "level2") +
                                   $"{Path.PathSeparator}{Path.Combine("network", "admin")}" +
                                   $"{Path.PathSeparator}{expected}" +
                                   $"{Path.PathSeparator}{Path.Combine("main2", "level1", "level2")}";
            DbProviderExtensions.ConfigureOracleTnsAdminPath(string.Empty, environmentPath2).Should().EndWithEquivalentOf(expected);
        }

        [Fact]
        public void ConfigureOracleTnsAdminPath_EnvironmentResolved_ShouldReturnEmptyPath()
        {
            var environmentPath1 = Path.Combine("main1", "level1", "level2") +
                                   $"{Path.PathSeparator}{Path.Combine("network", "admin")}" +
                                   $"{Path.PathSeparator}{Path.Combine("oracle", "product", "client", "bin", "no")}" +
                                   $"{Path.PathSeparator}{Path.Combine("main2", "level1", "level2")}";
            DbProviderExtensions.ConfigureOracleTnsAdminPath(string.Empty, environmentPath1).Should().BeNullOrEmpty();
            
            var environmentPath2 = Path.Combine("main1", "level1", "level2") +
                                   $"{Path.PathSeparator}{Path.Combine("network", "admin")}" +
                                   $"{Path.PathSeparator}{Path.Combine("oracle", "product", "client", "network")}" +
                                   $"{Path.PathSeparator}{Path.Combine("main2", "level1", "level2")}";
            DbProviderExtensions.ConfigureOracleTnsAdminPath(string.Empty, environmentPath2).Should().BeNullOrEmpty();
        }

        [Fact]
        public void ConfigureOracleTnsAdminPath_EnvironmentResolvedOnAhoPC_ShouldReturnEmptyPath()
        {
            if (!Environment.MachineName.ContainsIgnoreCase("aho-p1"))
            {
                return;
            }

            DbProviderExtensions.ConfigureOracleTnsAdminPath(null).Should().BeEquivalentTo("C:\\Oracle\\product\\12.1.0\\client_1\\network\\admin");
        }
    }
}
