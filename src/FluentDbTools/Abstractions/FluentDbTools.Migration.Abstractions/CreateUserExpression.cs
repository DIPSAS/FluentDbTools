using FluentMigrator.Expressions;

namespace FluentDbTools.Migration.Abstractions
{
    public class CreateUserExpression : ISchemaExpression
    {
        public CreateUserExpression(CreateSchemaExpression expression)
        {
            SchemaName = expression.SchemaName;
        }

        public string SchemaName { get; set; }
    }
}