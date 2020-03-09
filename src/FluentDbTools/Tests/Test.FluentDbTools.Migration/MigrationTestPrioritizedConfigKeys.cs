using System.Diagnostics.CodeAnalysis;
using FluentDbTools.Common.Abstractions.PrioritizedConfig;

namespace Test.FluentDbTools.Migration
{
    /// <inheritdoc />
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class MigrationTestPrioritizedConfigKeys : PrioritizedConfigKeys
    {
        /// <inheritdoc />
        public override string[] GetDbSchemaKeys()
        {
            return new[] {"exampleSchema2"};
        }
    }
}