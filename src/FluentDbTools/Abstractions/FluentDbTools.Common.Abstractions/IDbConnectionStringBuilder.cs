namespace FluentDbTools.Common.Abstractions
{
    public interface IDbConnectionStringBuilder
    {
        SupportedDatabaseTypes DatabaseType { get; }
        string BuildConnectionString(IDbConnectionStringBuilderConfig dbConfig);
        string BuildAdminConnectionString(IDbConnectionStringBuilderConfig dbConfig);
    }
}