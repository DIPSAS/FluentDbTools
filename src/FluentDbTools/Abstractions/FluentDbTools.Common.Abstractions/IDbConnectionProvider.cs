namespace FluentDbTools.Common.Abstractions
{
    public interface IDbConnectionProvider
    {
        SupportedDatabaseTypes DatabaseType { get; }
        string GetConnectionString(IDbConfig dbConfig);
        string GetAdminConnectionString(IDbConfig dbConfig);
    }
}