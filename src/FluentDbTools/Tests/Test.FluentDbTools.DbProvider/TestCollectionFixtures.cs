using TestUtilities.FluentDbTools;
using Xunit;

namespace Test.FluentDbTools.DbProvider
{
    [CollectionDefinition(CollectionTag)]
    public class TestCollectionFixtures : ICollectionFixture<DatabaseFixture>
    {
        internal static readonly object LockObj = new();
        public const string CollectionTag = "Database collection";
    }
}