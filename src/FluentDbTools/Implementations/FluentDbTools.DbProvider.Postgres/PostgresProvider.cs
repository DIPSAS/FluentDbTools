using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using FluentDbTools.Common.Abstractions;
using Npgsql;

[assembly: InternalsVisibleTo("FluentDbTools.Extensions.DbProvider")]
namespace FluentDbTools.DbProvider.Postgres
{
    internal class PostgresProvider : IDbConnectionProvider
    {
        public SupportedDatabaseTypes DatabaseType => SupportedDatabaseTypes.Postgres;
        
        private const string ConnectionStringTemplate = "User ID={0};" +
                                                        "Password={1};" +
                                                        "Host={2};" +
                                                        "Port={3};" +
                                                        "Database={4};" +
                                                        "Pooling={5};";
        
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
            return new NpgsqlConnection(connectionString);
        }
    }
}