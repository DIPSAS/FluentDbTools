using FluentMigrator.Expressions;

namespace FluentDbTools.Migration.Abstractions
{
    /// <summary>
    /// Option object for Create User expression
    /// </summary>
    public class CreateUserExpression : ISchemaExpression
    {
        /// <summary>
        /// Initialize <see cref="CreateUserExpression"/> by <see cref="CreateSchemaExpression"/>
        /// </summary>
        /// <param name="expression"></param>
        public CreateUserExpression(CreateSchemaExpression expression)
        {
            SchemaName = expression.SchemaName;
        }

        /// <inheritdoc />
        public string SchemaName { get; set; }
    }
}