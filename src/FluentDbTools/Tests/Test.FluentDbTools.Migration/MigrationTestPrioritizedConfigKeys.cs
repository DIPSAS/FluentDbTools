using FluentDbTools.Common.Abstractions.PrioritizedConfig;

namespace Test.FluentDbTools.Migration
{
    public class MigrationTestPrioritizedConfigKeys : PrioritizedConfigKeys
    {
        public override string[] GetDbSchemaKeys()
        {
            return new[] {"exampleSchema2"};
        }
    }
}