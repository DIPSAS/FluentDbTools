using System.Diagnostics.CodeAnalysis;
using FluentDbTools.Common.Abstractions.PrioritizedConfig;

namespace Example.FluentDbTools.Migration
{
    /// <inheritdoc />
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class TestPrioritizedConfigKeys : PrioritizedConfigKeys
    {
        /// <inheritdoc />
        public override string[] GetDbSchemaKeys()
        {
            return new[] {"exampleSchema"};
        }

        /// <inheritdoc />
        public override string[] GetDbSchemaPasswordKeys()
        {
            return new[] {"database:migration:exampleSchema:password"};
        }

        /// <inheritdoc />
        public override string[] GetDbSchemaPrefixIdStringKeys()
        {
            return new[] {"database:schemaPrefix:exampleSchemaPrefixId"};
        }

        /// <inheritdoc />
        public override string[] GetDbSchemaUniquePrefixIdStringKeys()
        {
            return new[] {"database:migration:schemaPrefix:exampleSchemaPrefixUniqueId"};
        }
    }
}