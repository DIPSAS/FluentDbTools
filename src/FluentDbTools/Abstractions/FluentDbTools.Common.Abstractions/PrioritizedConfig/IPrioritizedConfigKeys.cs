﻿namespace FluentDbTools.Common.Abstractions.PrioritizedConfig
{
    public interface IPrioritizedConfigKeys
    {
        string[] GetDbTypeKeys();
        string[] GetDbSchemaKeys();
        string[] GetDbSchemaPrefixIdStringKeys();
        string[] GetDbDatabaseNameKeys();
        string[] GetDbUserKeys();
        string[] GetDbPasswordKeys();
        string[] GetDbAdminUserKeys();
        string[] GetDbAdminPasswordKeys();

        string[] GetDbHostnameKeys();
        string[] GetDbPortKeys();
        string[] GetDbDataSourceKeys();
        string[] GetDbConnectionTimeoutKeys();
        string[] GetDbPoolingKeys();

        string[] GetDbConnectionStringKeys();
        string[] GetDbAdminConnectionStringKeys();
    }
}