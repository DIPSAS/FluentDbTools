namespace FluentDbTools.Migration.Abstractions
{
    public interface ICreateOrReplaceViewWithColumnSyntax : ICreateOrReplaceWithSqlScript
    {
        ICreateOrReplaceViewWithColumnSyntax WithColumn(string name);
        ICreateOrReplaceViewWithColumnSyntax WithColumns(params string[] names);
    }
}