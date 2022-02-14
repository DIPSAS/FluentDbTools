namespace FluentDbTools.Migration.Abstractions
{
    internal class SqlStatement
    {
        public SqlStatement()
        {
            ParseTitle = false;
        }

        public string Sql { get; set; }
        public bool IsExternal { get; set; }
        public bool ParseTitle { get; set; }
    }
}