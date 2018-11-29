using System.Collections.Generic;
using System.Data;
using DIPS.FluentDbTools.Common.Abstractions;

namespace DIPS.FluentDbTools.Database.Abstractions
{
    public interface IDbConnectionProvider
    {
        SupportedDatabaseTypes DatabaseType { get; }
        string GetConnectionString(IDbConfig dbConfig);
        string GetAdminConnectionString(IDbConfig dbConfig);
        IDbConnection CreateDbConnection(IDbConfig dbConfig, bool withAdminPrivileges = false);
    }
}