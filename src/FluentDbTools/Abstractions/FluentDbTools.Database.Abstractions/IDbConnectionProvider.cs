using System.Collections.Generic;
using System.Data;
using FluentDbTools.Common.Abstractions;

namespace FluentDbTools.Database.Abstractions
{
    public interface IDbConnectionProvider
    {
        SupportedDatabaseTypes DatabaseType { get; }
        string GetConnectionString(IDbConfig dbConfig);
        string GetAdminConnectionString(IDbConfig dbConfig);
        IDbConnection CreateDbConnection(IDbConfig dbConfig, bool withAdminPrivileges = false);
    }
}