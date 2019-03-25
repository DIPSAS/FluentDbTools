using System;
using FluentDbTools.Common.Abstractions;

namespace FluentDbTools.Contracts
{
    public class DefaultDbConfigValues
    {
        public const SupportedDatabaseTypes DefaultDbType = SupportedDatabaseTypes.Postgres;
        
        // DbConfigDatabaseTargets defaults
        public Func<SupportedDatabaseTypes> GetDefaultDbType = () => DefaultDbType;
        public Func<string> GetDefaultSchema = null;
        public Func<string> GetDefaulDatabaseName = null;

        // DbConfigCredentials defaults
        public Func<string> GetDefaultUser = () => "user";
        public Func<string> GetDefaultPassword = () => "password";
        public Func<string> GetDefaultAdminUser = () => "postgres";
        public Func<string> GetDefaultAdminPassword = () => "postgres";

        // DbConnectionStringBuilderConfig defaults
        public Func<string> GetDefaultHostName = () => "localhost";
        public Func<string> GetDefaultPort = () => "5432";
        public Func<string> GetDefaultDatasource = () => null;
        public Func<string> GetDefaultConnectionTimeoutInSecs = () => null;
        public Func<bool>   GetDefaultPooling = () => true;

        // DbConfig defaults
        public Func<string> GetDefaultConnectionString = () => null;
        public Func<string> GetDefaultAdminConnectionString = () => null;

    }
}