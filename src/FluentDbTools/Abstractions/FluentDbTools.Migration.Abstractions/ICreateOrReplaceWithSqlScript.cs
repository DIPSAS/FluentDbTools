using FluentMigrator.Infrastructure;

namespace FluentDbTools.Migration.Abstractions
{
    public interface ICreateOrReplaceWithSqlScript : IFluentSyntax
    {
        ICreateOrReplaceViewWithColumnSyntax InSchema(string schemaName);
        ICreateOrReplaceViewWithColumnSyntax WithSqlScript(string scriptName);
    }
}