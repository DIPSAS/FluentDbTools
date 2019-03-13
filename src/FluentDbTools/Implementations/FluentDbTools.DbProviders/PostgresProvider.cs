using System.Runtime.CompilerServices;
using FluentDbTools.Common.Abstractions;

[assembly: InternalsVisibleTo("FluentDbTools.Extensions.DbProvider")]
namespace FluentDbTools.DbProviders
{
    internal class PostgresProvider : IDbConnectionProvider
    {
        public SupportedDatabaseTypes DatabaseType => SupportedDatabaseTypes.Postgres;
        
        private const string DefaultConnectionStringTemplate = "User ID={0};" +
                                                        "Password={1};" +
                                                        "Host={2};" +
                                                        "Port={3};" +
                                                        "Database={4};" +
                                                        "Pooling={5};";
        
        public string GetConnectionString(IDbConfig dbConfig) =>
            string.Format(dbConfig.ConnectionStringTemplate ?? DefaultConnectionStringTemplate,
                dbConfig.User.ToLower(),
                dbConfig.Password,
                dbConfig.Hostname,
                dbConfig.Port,
                dbConfig.DatabaseConnectionName.ToLower(),
                dbConfig.Pooling.ToString());
        
        public string GetAdminConnectionString(IDbConfig dbConfig) =>
            string.Format(dbConfig.ConnectionStringTemplate ?? DefaultConnectionStringTemplate,
                dbConfig.AdminUser.ToLower(),
                dbConfig.AdminPassword,
                dbConfig.Hostname,
                dbConfig.Port,
                dbConfig.DatabaseConnectionName.ToLower(),
                dbConfig.Pooling.ToString());
    }
}