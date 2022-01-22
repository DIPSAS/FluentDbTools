using System;
using System.Linq;
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

        public string BuildConnectionString(IDbConnectionStringBuilderConfig dbConfig) => BuildConnectionString(dbConfig, GetDataSource(dbConfig), false);

        public string BuildAdminConnectionString(IDbConnectionStringBuilderConfig dbConfig) => BuildConnectionString(dbConfig, GetDataSource(dbConfig), true);


        internal static string BuildConnectionString(IDbConnectionStringBuilderConfig dbConfig, string datasource, bool isAdminMode)
        {
            var adminUser = dbConfig.AdminUser;
            var adminPassword = dbConfig.AdminPassword;
            if (isAdminMode && dbConfig is IDbConfig dbConfigFull)
            {
                if (dbConfigFull.IsAdminValuesValid == false)
                {
                    adminUser = "";
                    adminPassword = "";
                };
            }
            var poolingStr = $"{dbConfig.Pooling}";
            if (dbConfig.Pooling && dbConfig.PoolingKeyValues?.Any() == true)
            {
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var keyValue in dbConfig.PoolingKeyValues)
                {
                    poolingStr += $";{keyValue.Key} = {keyValue.Value}";
                }
            }

            var connectionString = string.Format(DefaultConnectionStringTemplate,
                (isAdminMode ? adminUser : dbConfig.User)?.ToUpper(),
                isAdminMode ? adminPassword : dbConfig.Password,
                datasource,
                poolingStr,
                GetConnectionTimeout(dbConfig));

            return connectionString;
        }

        internal static string GetDataSource(IDbConnectionStringBuilderConfig dbConfig)
        {
            var dataSource = dbConfig.Datasource;

            return !string.IsNullOrEmpty(dataSource)
                ? dataSource
                : GetDefaultDataSource(dbConfig.Hostname, dbConfig.Port, dbConfig.GetOracleServiceName()?.ToLower());
        }

        internal static string GetDefaultDataSource(string hostName, string port, string serviceName)
        {
            return string.Format(DefaultDataSourceTemplate, hostName, port, serviceName);
        }

        private static string GetConnectionTimeout(IDbConnectionStringBuilderConfig dbConfig)
        {
            return string.IsNullOrEmpty(dbConfig.ConnectionTimeoutInSecs)
                ? string.Empty
                : string.Format(ConnectionTimeoutTemplate, dbConfig.ConnectionTimeoutInSecs);
        }
    }
}