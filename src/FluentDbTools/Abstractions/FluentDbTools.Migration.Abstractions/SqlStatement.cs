namespace FluentDbTools.Migration.Abstractions
{
    internal class SqlStatement
    {
        public string Sql { get; set; }
        public bool IsExternal { get; set; }
    }
}