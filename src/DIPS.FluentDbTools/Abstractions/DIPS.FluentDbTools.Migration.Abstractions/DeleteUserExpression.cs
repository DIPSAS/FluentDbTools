using FluentMigrator.Expressions;

namespace DIPS.FluentDbTools.Migration.Abstractions
{
    public class DeleteUserExpression : ISchemaExpression
    {
        public DeleteUserExpression(DeleteSchemaExpression expression)
        {
            SchemaName = expression.SchemaName;
        }

        public string SchemaName { get; set; }
    }
}