using System;
using System.Runtime.CompilerServices;
using FluentDbTools.Common.Abstractions;

[assembly: InternalsVisibleTo("FluentDbTools.Extensions.DbProvider")]
[assembly: InternalsVisibleTo("Test.FluentDbTools.DbProvider")]

namespace FluentDbTools.DbProviders
{
    internal class OracleConnectionStringBuilder : IDbConnectionStringBuilder
    {

        internal const string ConnectionTimeoutTemplate = ";Connection Timeout={0}";
        internal const string DefaultDataSourceTemplate = "(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1})))(CONNECT_DATA=(SERVICE_NAME={2})))";
        internal const string DefaultConnectionStringTemplate = "User Id={0};" +
                                                               "Password={1};" +
                                                               "Data Source={2};" +
                                                               "Pooling={3}" +
                                                               "{4}";
        

        public SupportedDatabaseTypes DatabaseType => SupportedDatabaseTypes.Oracle;

        public string BuildConnectionString(IDbConnectionStringBuilderConfig dbConfig) => BuildConnectionString(dbConfig, GetDataSourceTemplate(dbConfig), false);

        public string BuildAdminConnectionString(IDbConnectionStringBuilderConfig dbConfig) => BuildConnectionString(dbConfig, GetDataSourceTemplate(dbConfig), true);


        internal static string BuildConnectionString(IDbConnectionStringBuilderConfig dbConfig, string datasource, bool isAdminMode)
        {
            var connstr = string.Format(DefaultConnectionStringTemplate,
                (isAdminMode ? dbConfig.AdminUser : dbConfig.User).ToUpper(),
                isAdminMode ? dbConfig.AdminPassword : dbConfig.Password,
                datasource,
                dbConfig.Pooling,
                GetConnectionTimeout(dbConfig));

            return connstr;
        }

        internal static string GetDataSourceTemplate(IDbConnectionStringBuilderConfig dbConfig)
        {
            return !string.IsNullOrEmpty(dbConfig.Datasource)
                ? dbConfig.Datasource
                : GetDefaultDataSource(dbConfig.Hostname, dbConfig.Port, dbConfig.DatabaseName?.ToLower());
        }

        internal static string GetDefaultDataSource(string hostName, string port, string servicename)
        {
            return string.Format(DefaultDataSourceTemplate, hostName, port, servicename);
        }

        private static string GetConnectionTimeout(IDbConnectionStringBuilderConfig dbConfig)
        {
            return string.IsNullOrEmpty(dbConfig.ConnectionTimeoutInSecs)
                ? string.Empty
                : string.Format(ConnectionTimeoutTemplate, dbConfig.ConnectionTimeoutInSecs);
        }
    }
}