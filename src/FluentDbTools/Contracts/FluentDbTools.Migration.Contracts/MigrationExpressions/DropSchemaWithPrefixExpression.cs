using FluentDbTools.Migration.Abstractions.ExtendedExpressions;

namespace FluentDbTools.Migration.Contracts.MigrationExpressions
{
    /// <summary>
    /// <inheritdoc cref="SchemaWithPrefixExpression"/><br/>
    /// <inheritdoc cref="IDropSchemaWithPrefixExpression"/>
    /// </summary>
    public class DropSchemaWithPrefixExpression : SchemaWithPrefixExpression, IDropSchemaWithPrefixExpression
    {
    }
}