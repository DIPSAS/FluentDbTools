using FluentMigrator.Expressions;

namespace FluentDbTools.Migration.Abstractions
{
    /// <summary>
    /// Option object for Delete User expression
    /// </summary>
    public class DeleteUserExpression : ISchemaExpression
    {
        /// <summary>
        /// Initialize <see cref="DeleteUserExpression"/> by <see cref="DeleteSchemaExpression"/>
        /// </summary>
        /// <param name="expression"></param>
        public DeleteUserExpression(DeleteSchemaExpression expression)
        {
            SchemaName = expression.SchemaName;
        }

        /// <inheritdoc />
        public string SchemaName { get; set; }
    }
}