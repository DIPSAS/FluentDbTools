using FluentMigrator.Expressions;

namespace FluentDbTools.Migration.Abstractions.ExtendedExpressions
{
    public interface ISchemaWithPrefixExpression : ISchemaExpression
    {
        string SchemaPrefix { get; }
        string SchemaPrefixUniqueId { get; }
    }
}