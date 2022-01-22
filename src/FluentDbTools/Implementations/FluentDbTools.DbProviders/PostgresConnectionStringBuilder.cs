using System.Linq;
using System.Runtime.CompilerServices;
using FluentDbTools.Common.Abstractions;

[assembly: InternalsVisibleTo("FluentDbTools.Extensions.DbProvider")]
namespace FluentDbTools.DbProviders
{
    internal class PostgresConnectionStringBuilder : IDbConnectionStringBuilder
    {
        public SupportedDatabaseTypes DatabaseType => SupportedDatabaseTypes.Postgres;

        internal const string ConnectionTimeoutTemplate = ";Timeout={0}";
        private const string DefaultConnectionStringTemplate = "Username={0};" +
                                                        "Password={1};" +
                                                        "Host={2};" +
                                                        "Port={3};" +
                                                        "Database={4};" +
                                                        "Pooling={5}" +
                                                        "{6}";

        public string BuildConnectionString(IDbConnectionStringBuilderConfig dbConfig)
        {
            return BuildConnectionString(dbConfig, false);
        }

        public string BuildAdminConnectionString(IDbConnectionStringBuilderConfig dbConfig)
        {
            return BuildConnectionString(dbConfig, true);
        }

        private static string BuildConnectionString(IDbConnectionStringBuilderConfig dbConfig, bool isAdminMode)
        {
            var adminUser = dbConfig.AdminUser;
            var adminPassword = dbConfig.AdminPassword;
            if (isAdminMode && dbConfig is IDbConfig dbConfigForValidation)
            {
                if (dbConfigForValidation.IsAdminValuesValid == false)
                {
                    adminUser = "";
                    adminPassword = "";
                };
            }

            var connectionString = string.Format(DefaultConnectionStringTemplate,
                (isAdminMode ? adminUser : dbConfig?.User)?.ToLower(),
                isAdminMode ? adminPassword : dbConfig?.Password,
                dbConfig?.Hostname,
                dbConfig?.Port,
                dbConfig?.DatabaseName.ToLower(),
                dbConfig?.Pooling,
                GetConnectionTimeout(dbConfig));

            return connectionString;
        }


        private static string GetConnectionTimeout(IDbConnectionStringBuilderConfig dbConfig)
        {
            return string.IsNullOrEmpty(dbConfig.ConnectionTimeoutInSecs)
                ? string.Empty
                : string.Format(ConnectionTimeoutTemplate, dbConfig.ConnectionTimeoutInSecs);
        }
    }
}