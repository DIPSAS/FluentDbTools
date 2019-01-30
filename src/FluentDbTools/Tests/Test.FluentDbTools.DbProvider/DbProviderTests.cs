using System.Threading.Tasks;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Example.Database;
using TestUtilities.FluentDbTools;
using Xunit;

namespace Test.FluentDbTools.DbProvider
{
    [Collection(TestCollectionFixtures.CollectionTag)]
    public class DbProviderTests
    {
        [Theory]
        [MemberData(nameof(TestParameters.DbParameters), MemberType = typeof(TestParameters))]
        public async Task DbProvider_ExampleRepository_Success(SupportedDatabaseTypes databaseType)
        {
            var overrideConfig = OverrideConfig.GetInMemoryOverrideConfig(databaseType);
            await DbExampleExecutor.ExecuteDbExample(databaseType, overrideConfig);
        }
    }
}