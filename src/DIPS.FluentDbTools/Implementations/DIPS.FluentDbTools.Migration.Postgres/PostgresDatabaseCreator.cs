using System;
using System.Data;
using System.IO;
using System.Threading;
using Dapper;
using DIPS.FluentDbTools.Common.Abstractions;
using Npgsql;

namespace DIPS.FluentDbTools.Migration.Postgres
{
    internal static class PostgresDatabaseCreator
    {
        private const string DatabaseExistsSqlTemplate = "SELECT 1 from pg_database WHERE lower(datname)=lower('{0}');";

        private const string DatabaseCreateSqlTemplate =
            "CREATE DATABASE \"{0}\" WITH OWNER = '{1}';" +
            "ALTER DEFAULT PRIVILEGES GRANT ALL ON TABLES TO PUBLIC;" +
            "ALTER DEFAULT PRIVILEGES GRANT SELECT, USAGE ON SEQUENCES TO PUBLIC;" +
            "ALTER DEFAULT PRIVILEGES GRANT EXECUTE ON FUNCTIONS TO PUBLIC;" +
            "ALTER DEFAULT PRIVILEGES GRANT USAGE ON TYPES TO PUBLIC;";

        private const string DatabaseKillAllConnections = "SELECT pg_terminate_backend(pg_stat_activity.pid) " +
                                                          "FROM pg_stat_activity WHERE pg_stat_activity.datname = '{0}' " +
                                                          "AND pid <> pg_backend_pid();";

        private const string DatabaseDropSqlTemplate = "DROP DATABASE {0};";

        private const string ConnectionStringWithoutDatabaseTemplate = "User ID={0};Password={1};Host={2};Port={3};Pooling={4};";

        public static void CreateDatabase(IDbConfig dbConfig)
        {
            using (var dbConnection = dbConfig.CreateDbConnection())
            {
                dbConnection.Open();
                try
                {
                    CreateDatabase(dbConfig, dbConnection);
                }
                finally 
                {
                    dbConnection.Close();
                }
            }
        }
        
        public static void DropDatabase(IDbConfig dbConfig)
        {
            using (var dbConnection = dbConfig.CreateDbConnection())
            {
                dbConnection.Open();
                try
                {
                    DropDatabase(dbConfig, dbConnection);
                }
                finally 
                {
                    dbConnection.Close();
                }
            }
        }

        private static void CreateDatabase(IDbConfig dbConfig, IDbConnection dbConnection)
        {
            if (!dbConnection.Exists(string.Format(DatabaseExistsSqlTemplate, dbConfig.DatabaseConnectionName.ToLower())))
            {
                var sqlCreateDatabase = string.Format(DatabaseCreateSqlTemplate, dbConfig.DatabaseConnectionName.ToLower(), dbConfig.AdminUser.ToLower());
                dbConnection.ExecuteWithCommand(sqlCreateDatabase);
            }
        }

        private static void DropDatabase(IDbConfig dbConfig, IDbConnection dbConnection)
        {
            var maxTries = 5;
            while (dbConnection.Exists(string.Format(DatabaseExistsSqlTemplate, dbConfig.DatabaseConnectionName.ToLower())))
            {
                KillAllDatabaseConnections(dbConfig, dbConnection);
                var sqlDropDatabase = string.Format(DatabaseDropSqlTemplate, dbConfig.DatabaseConnectionName.ToLower());
                dbConnection.ExecuteWithCommand(sqlDropDatabase);
                if (--maxTries < 0)
                {
                    break;
                }
            }
        }

        private static void KillAllDatabaseConnections(IDbConfig dbConfig, IDbConnection dbConnection)
        {
            if (dbConnection.Exists(string.Format(DatabaseExistsSqlTemplate, dbConfig.DatabaseConnectionName.ToLower())))
            {
                var sqlKillAllConnections = string.Format(DatabaseKillAllConnections, dbConfig.DatabaseConnectionName.ToLower());
                dbConnection.ExecuteWithCommand(sqlKillAllConnections);
            }
        }

        private static void ExecuteWithCommand(this IDbConnection dbConnection, string sql)
        {
            using (var command = dbConnection.CreateCommand())
            {
                command.CommandText = sql;
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    using (var message = new StringWriter())
                    {
                        message.WriteLine("An error occurred executing the following sql:");
                        message.WriteLine(sql);
                        message.WriteLine("The error was {0}", ex.Message);

                        throw new Exception(message.ToString(), ex);
                    }
                }
            }
        }

        private static IDbConnection CreateDbConnection(this IDbConfig dbConfig)
        {
            return new NpgsqlConnection(string.Format(ConnectionStringWithoutDatabaseTemplate, 
                dbConfig.AdminUser.ToLower(),
                dbConfig.AdminPassword,
                dbConfig.Hostname.ToLower(),
                dbConfig.Port,
                dbConfig.Pooling.ToString()));
        }

        private static bool Exists(this IDbConnection dbConnection, string template, params object[] args)
        {
            var dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandText = string.Format(template, args);
            using (var reader = dbCommand.ExecuteReader())
            {
                return reader.Read();
            }
        }

    }
}