using FluentDbTools.Migration.Abstractions.ExtendedExpressions;

namespace FluentDbTools.Migration.Contracts.MigrationExpressions
{
    /// <inheritdoc />
    public class SchemaWithPrefixExpression : ISchemaWithPrefixExpression
    {
        /// <inheritdoc />
        public string SchemaName { get; set; }

        /// <inheritdoc />
        public string SchemaPrefixId { get; set; }

        /// <inheritdoc />
        public string SchemaPrefixUniqueId { get; set; }
    }
}