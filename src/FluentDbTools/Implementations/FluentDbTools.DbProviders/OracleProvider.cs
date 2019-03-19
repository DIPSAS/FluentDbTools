using System.Runtime.CompilerServices;
using FluentDbTools.Common.Abstractions;

[assembly: InternalsVisibleTo("FluentDbTools.Extensions.DbProvider")]
[assembly: InternalsVisibleTo("Test.FluentDbTools.DbProvider")]
namespace FluentDbTools.DbProviders
{
    internal class OracleProvider : IDbConnectionProvider
    {
        //private bool ParseHostFromEzConnection = true;

        public SupportedDatabaseTypes DatabaseType => SupportedDatabaseTypes.Oracle;

        internal const string DefaultDataSourceTemplate = "(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1})))(CONNECT_DATA=(SERVICE_NAME={2})))";
        private const string DefaultConnectionStringTemplate = "User Id={0};" +
                                                               "Password={1};" +
                                                               "Pooling={2};" +
                                                               "Data Source={3}";


        public string GetConnectionString(IDbConfig dbConfig) =>
            string.Format(dbConfig.ConnectionStringTemplate ?? DefaultConnectionStringTemplate,
                dbConfig.User.ToUpper(),
                dbConfig.Password,
                dbConfig.Pooling,
                GetDataSourceTemplate(dbConfig));

        public string GetAdminConnectionString(IDbConfig dbConfig) =>
            string.Format(dbConfig.ConnectionStringTemplate ?? DefaultConnectionStringTemplate,
                dbConfig.AdminUser.ToUpper(),
                dbConfig.AdminPassword,
                dbConfig.Pooling,
                GetDataSourceTemplate(dbConfig));


        private string GetDataSourceTemplate(IDbConfig dbConfig)
        {
            var hostName = GetHostName(dbConfig);
            return string.IsNullOrEmpty(hostName)
                ? dbConfig.DatabaseConnectionName
                : string.Format(DefaultDataSourceTemplate, hostName, dbConfig.Port, GetServiceName(dbConfig));
        }


        private string GetHostName(IDbConfig dbConfig)
        {
            var hostnameFromServiceName = ParseHostNameFromServiceName(dbConfig);
            return !string.IsNullOrEmpty(hostnameFromServiceName) 
                ? hostnameFromServiceName
                : dbConfig.Hostname;

        }

        private string ParseHostNameFromServiceName(IDbConfig dbConfig)
        {
            var ezConnectionStrings = dbConfig.DatabaseConnectionName?.ToLower().Split('/');

            return ezConnectionStrings?.Length == 2 
                ? ezConnectionStrings[0] 
                : string.Empty;
        }

        private string GetServiceName(IDbConfig dbConfig)
        {
            var databaseConnectionName = dbConfig.DatabaseConnectionName?.ToLower();
            var ezConnectionStrings = databaseConnectionName?.Split('/');

            return ezConnectionStrings?.Length == 2 
                ? ezConnectionStrings[1].Trim() 
                : databaseConnectionName;
        }
    }
}