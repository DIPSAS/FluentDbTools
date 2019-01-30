using TestUtilities.FluentDbTools;
using Xunit;

namespace Test.FluentDbTools.DbProvider
{
    [CollectionDefinition(CollectionTag)]
    public class TestCollectionFixtures : ICollectionFixture<DatabaseFixture>
    {
        public const string CollectionTag = "Database collection";
    }
}