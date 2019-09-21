using FluentMigrator.Expressions;

namespace FluentDbTools.Migration.Abstractions.ExtendedExpressions
{
    /// <summary>
    /// Extended schema expression convention(<see cref="ISchemaExpression"/>) <see cref="ISchemaExpression.SchemaName"/> <br/>
    /// with <see cref="SchemaPrefixId"/> and <see cref="SchemaPrefixUniqueId"/>
    /// </summary>
    public interface ISchemaWithPrefixExpression : ISchemaExpression
    {
        /// <summary>
        /// Get SchemaPrefixId
        /// </summary>
        string SchemaPrefixId { get; }

        /// <summary>
        /// Get SchemaPrefixUniqueId
        /// </summary>
        string SchemaPrefixUniqueId { get; }
    }
}