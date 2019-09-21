using FluentDbTools.Migration.Abstractions.ExtendedExpressions;
using FluentMigrator.Expressions;

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
        /// Constructor
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

        /// <inheritdoc />
        public override string ToString()
        {
            return $"DefaultColumn Expression is linked to [{Expression}] expression";
        }
    }
}

