using FluentDbTools.Common.Abstractions.PrioritizedConfig;

namespace Example.FluentDbTools.Migration
{
    public class TestPrioritizedConfigKeys : PrioritizedConfigKeys
    {
        public override string[] GetDbSchemaKeys()
        {
            return new[] {"exampleSchema"};
        }

        public override string[] GetDbSchemaPasswordKeys()
        {
            return new[] {"database:migration:exampleSchema:password"};
        }

        public override string[] GetDbSchemaPrefixIdStringKeys()
        {
            return new[] {"database:schemaPrefix:exampleSchemaPrefixId"};
        }

        public override string[] GetDbSchemaUniquePrefixIdStringKeys()
        {
            return new[] {"database:migration:schemaPrefix:exampleSchemaPrefixUniqueId"};
        }
    }
}