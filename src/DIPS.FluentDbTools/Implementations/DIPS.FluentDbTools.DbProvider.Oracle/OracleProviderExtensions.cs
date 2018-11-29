using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using DIPS.FluentDbTools.Common.Abstractions;
using DIPS.FluentDbTools.Database.Abstractions;
using Oracle.ManagedDataAccess.Client;

[assembly: InternalsVisibleTo("DIPS.Extensions.FluentDbTools.DbProvider")]
namespace DIPS.FluentDbTools.DbProvider.Oracle
{
    internal class OracleProvider : IDbConnectionProvider
    {
        public SupportedDatabaseTypes DatabaseType => SupportedDatabaseTypes.Oracle;
        
        private static string ConnectionStringTemplate => "User Id={0};" +
                                                   "Password={1};" +
                                                   "Pooling={5};" +
                                                   "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={2})(PORT={3})))(CONNECT_DATA=(SERVICE_NAME={4})))";
        
        public string GetConnectionString(IDbConfig dbConfig) =>
            string.Format(ConnectionStringTemplate,
                dbConfig.User.ToLower(),
                dbConfig.Password,
                dbConfig.Hostname,
                dbConfig.Port,
                dbConfig.DatabaseConnectionName.ToLower(),
                dbConfig.Pooling.ToString());
        
        public string GetAdminConnectionString(IDbConfig dbConfig) =>
            string.Format(ConnectionStringTemplate,
                dbConfig.AdminUser.ToLower(),
                dbConfig.AdminPassword,
                dbConfig.Hostname,
                dbConfig.Port,
                dbConfig.DatabaseConnectionName.ToLower(),
                dbConfig.Pooling.ToString());

        public IDbConnection CreateDbConnection(IDbConfig dbConfig, bool withAdminPrivileges = false)
        {
            var connectionString = withAdminPrivileges ? GetAdminConnectionString(dbConfig) : GetConnectionString(dbConfig);
            return new OracleConnection(connectionString);
        }
    }
}