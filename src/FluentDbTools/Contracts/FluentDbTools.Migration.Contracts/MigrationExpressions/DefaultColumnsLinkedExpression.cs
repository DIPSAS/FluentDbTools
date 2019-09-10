using FluentDbTools.Migration.Abstractions.ExtendedExpressions;
using FluentMigrator.Builders.Execute;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;

namespace FluentDbTools.Migration.Contracts.MigrationExpressions
{
    /// <summary>
    /// Expression <see cref="IChangeLogTabledExpression"/> to appending customized table logging
    /// </summary>
    public class DefaultColumnsLinkedExpression : PerformDBOperationExpression, ILinkedExpression
    {
        /// <summary>
        /// The Linked Expression.
        /// </summary>
        public CreateTableExpression Expression { get;}

        /// <summary>
        /// Constru
        /// </summary>
        /// <param name="dependedExpression"></param>
        public DefaultColumnsLinkedExpression(CreateTableExpression dependedExpression)
        {
            Expression = dependedExpression;
            Operation = (connection, transaction) => { };
        }

        /// <summary>
        /// Determine if the linked expression is equal
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is DefaultColumnsLinkedExpression ext && Equals(Expression, ext.Expression);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Expression.GetHashCode();
        }
        public override string ToString()
        {
            return $"DefaultColumn Expression is linked to [{Expression}] expression";
        }
    }

    public class SchemaWithPrefixExpression : ISchemaWithPrefixExpression
    {
        public string SchemaName { get; set; }
        public string SchemaPrefix { get; set; }
        public string SchemaPrefixUniqueId { get; set; }
    }

    public class DropSchemaWithPrefixExpression : SchemaWithPrefixExpression, IDropSchemaWithPrefixExpression
    {
    }

    public class CreateSchemaWithPrefixExpression : SchemaWithPrefixExpression, ICreateSchemaWithPrefixExpression
    {

    }
}

