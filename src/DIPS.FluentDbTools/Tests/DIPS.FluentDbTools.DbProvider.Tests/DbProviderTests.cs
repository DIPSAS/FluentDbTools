using System.Threading.Tasks;
using DIPS.FluentDbTools.Common.Abstractions;
using DIPS.FluentDbTools.Example.Database;
using DIPS.FluentDbTools.TestUtilities;
using Xunit;

namespace DIPS.FluentDbTools.DbProvider.Tests
{
    [Collection(TestCollectionFixtures.CollectionTag)]
    public class DbProviderTests
    {
        [Theory]
        [MemberData(nameof(TestParameters.DbParameters), MemberType = typeof(TestParameters))]
        public async Task DbProvider_ExampleRepository_Success(SupportedDatabaseTypes databaseType)
        {
            var overrideConfig = OverrideConfig.GetInMemoryOverrideConfig(databaseType);
            var jsonConfig = OverrideConfig.GetJsonOverrideConfig(databaseType);
            await DbExampleExecutor.ExecuteDbExample(overrideConfig, jsonConfig);
        }
    }
}