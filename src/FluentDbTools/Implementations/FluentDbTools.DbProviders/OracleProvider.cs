using System.Runtime.CompilerServices;
using FluentDbTools.Common.Abstractions;

[assembly: InternalsVisibleTo("FluentDbTools.Extensions.DbProvider")]
namespace FluentDbTools.DbProviders
{
    internal class OracleProvider : IDbConnectionProvider
    {
        public SupportedDatabaseTypes DatabaseType => SupportedDatabaseTypes.Oracle;

        private const string DefaultConnectionStringTemplate = "User Id={0};" +
                                                                 "Password={1};" +
                                                                 "Pooling={5};" +
                                                                 "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={2})(PORT={3})))(CONNECT_DATA=(SERVICE_NAME={4})))";

        public string GetConnectionString(IDbConfig dbConfig) =>
            string.Format(dbConfig.ConnectionStringTemplate ?? DefaultConnectionStringTemplate,
                dbConfig.User.ToUpper(),
                dbConfig.Password,
                dbConfig.Hostname,
                dbConfig.Port,
                dbConfig.DatabaseConnectionName.ToLower(),
                dbConfig.Pooling);

        public string GetAdminConnectionString(IDbConfig dbConfig) =>
            string.Format(dbConfig.ConnectionStringTemplate ?? DefaultConnectionStringTemplate,
                dbConfig.AdminUser.ToUpper(),
                dbConfig.AdminPassword,
                dbConfig.Hostname,
                dbConfig.Port,
                dbConfig.DatabaseConnectionName.ToLower(),
                dbConfig.Pooling);
    }
}