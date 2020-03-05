namespace FluentDbTools.Common.Abstractions.PrioritizedConfig
{
    public interface IPrioritizedConfigValues
    {
        SupportedDatabaseTypes? GetDbType();
        string GetDbSchema();
        string GetDbSchemaPassword();
        string GetDbSchemaPrefixIdString();
        string GetDbSchemaUniquePrefixIdString();
        string GetDbDatabaseName();
        string GetDbUser();
        string GetDbPassword();
        string GetDbAdminUser();
        string GetDbAdminPassword();

        string GetDbHostname();
        string GetDbPort();
        string GetDbDataSource();
        string GetDbConnectionTimeout();
        bool? GetDbPooling();

        string GetDbConnectionString();
        string GetDbAdminConnectionString();
    }
}