namespace FluentDbTools.Common.Abstractions
{
    public static class Extensions 
    {
        public static string GetPrefixedName(this IDbConfig dbConfig, string name)
        {
            return name.GetPrefixedName(dbConfig.GetSchemaPrefixId());
        }
    }
}