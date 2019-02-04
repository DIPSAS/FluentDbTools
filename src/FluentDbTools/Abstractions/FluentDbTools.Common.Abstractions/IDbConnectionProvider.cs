using System.Data;

namespace FluentDbTools.Common.Abstractions
{
    public interface IDbConnectionProvider
    {
        SupportedDatabaseTypes DatabaseType { get; }
        string GetConnectionString(IDbConfig dbConfig);
        string GetAdminConnectionString(IDbConfig dbConfig);
        IDbConnection CreateDbConnection(IDbConfig dbConfig, bool withAdminPrivileges = false);
    }
}