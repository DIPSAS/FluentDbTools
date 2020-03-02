using FluentDbTools.Common.Abstractions.PrioritizedConfig;

namespace Example.FluentDbTools.Migration
{
    public class TestPrioritizedConfigKeys : PrioritizedConfigKeys
    {
        public override string[] GetDbSchemaKeys()
        {
            return new[] {"exampleSchema"};
        }
    }
}