using System.Collections.Generic;

namespace FluentDbTools.Common.Abstractions.PrioritizedConfig
{
    /// <summary>
    /// <para>Contains methods for override config-values from 'database' and 'database:migration' section.</para>
    /// <para>Name 'Prioritized' is used to signal that these values is prioritized before default-values</para>
    /// </summary>
    public interface IPrioritizedConfigValues
    {
        /// <summary>
        /// <para>Can be used to override the value from default-key: 'database:type'</para>
        /// <remarks>PS! Value from this function is prioritized before the above value by default-key</remarks>
        /// <para>See <see cref="IDbConfigSchemaTargets.DbType">IDbConfig.Schema</see> (<see cref="IDbConfigSchemaTargets.DbType"/>)</para>
        /// </summary>
        /// <returns></returns>
        SupportedDatabaseTypes? GetDbType();

        /// <summary>
        /// <para>Can be used to override the value from default-key: 'database:schema'</para>
        /// <remarks>PS! Value from this function is prioritized before the above value by default-key</remarks>
        /// <para>See <see cref="IDbConfigSchemaTargets.Schema">IDbConfig.Schema</see> (<see cref="IDbConfigSchemaTargets.Schema"/>)</para>
        /// </summary>
        /// <returns></returns>
        string GetDbSchema();

        /// <summary>
        /// <para>Can be used to override the value from default-key: 'database:migration:schemaPassword'</para>
        /// <remarks>PS! Value from this function is prioritized before the above value from default-key</remarks>
        /// <para>See <a href="https://dipsas.github.io/FluentDbTools/docs/doc/api/FluentDbTools.Migration.Abstractions.IDbMigrationConfig.html#SchemaPassword">IDbMigrationConfig.SchemaPassword</a></para>
        /// </summary>
        /// <returns></returns>
        string GetDbSchemaPassword();

        /// <summary>
        /// <para>Can be used to override the value from default-keys: ['database:schemaPrefix:id','database:migration:schemaPrefix:id']</para>
        /// <remarks>PS! Value from this function is prioritized before the above value from default-keys</remarks>
        /// <para>See <see cref="IDbConfigSchemaTargets.GetSchemaPrefixId">IDbConfig.GetSchemaPrefixId()</see> (<see cref="IDbConfigSchemaTargets.GetSchemaPrefixId"/>)</para>
        /// </summary>
        /// <returns></returns>
        string GetDbSchemaPrefixIdString();

        /// <summary>
        /// <para>Can be used to override the value from default-keys: ['database:migration:schemaPrefix:uniqueId','database:schemaPrefix:uniqueId']</para>
        /// <remarks>PS! Value from this function is prioritized before the above value from default-keys</remarks>
        /// <para>See <a href="https://dipsas.github.io/FluentDbTools/docs/doc/api/FluentDbTools.Migration.Abstractions.IDbMigrationConfig.html#GetSchemaPrefixUniqueId">IDbMigrationConfig.GetSchemaPrefixUniqueId()</a></para>
        /// </summary>
        /// <returns></returns>
        string GetDbSchemaUniquePrefixIdString();

        /// <summary>
        /// <para>Can be used to override the value from default-keys: ['database:databaseName','database:databaseConnectionName', 'database:serviceName', (postgres only) 'database:schema']</para>
        /// <remarks>PS! Value from this function is prioritized before the above value from default-keys</remarks>
        /// <para>See <see cref="IDbConfigDatabaseTargets.DatabaseName">IDbConfig.DatabaseName</see> (<see cref="IDbConfigDatabaseTargets.DatabaseName"/>)</para>
        /// </summary>
        /// <returns></returns>
        string GetDbDatabaseName();

        /// <summary>
        /// <para>Can be used to override the value from default-key: 'database:user'</para>
        /// <remarks>PS! Value from this function is prioritized before the above value from default-key</remarks>
        /// <para>See <see cref="IDbConfigCredentials.User">IDbConfig.User</see> (<see cref="IDbConfigCredentials.User"/>)</para>
        /// </summary>
        /// <returns></returns>
        string GetDbUser();

        /// <summary>
        /// <para>Can be used to override the value from default-keys ['database:password', 'database:user']</para>
        /// <remarks>PS! Value from this function is prioritized before the above value from default-keys</remarks>
        /// <para>See <see cref="IDbConfigCredentials.Password">IDbConfig.Password</see> (<see cref="IDbConfigCredentials.Password"/>)</para>
        /// </summary>
        /// <returns></returns>
        string GetDbPassword();
        
        /// <summary>
        /// <para>Can be used to override the value from default-key: 'database:adminUser'</para>
        /// <remarks>PS! Value from this function is prioritized before the above value from default-key</remarks>
        /// <para>See <see cref="IDbConfigCredentials.AdminUser">IDbConfig.AdminUser</see> (<see cref="IDbConfigCredentials.AdminUser"/>)</para>
        /// </summary>
        /// <returns></returns>
        string GetDbAdminUser();

        /// <summary>
        /// <para>Can be used to override the value fromdefault-keys: ['database:adminPassword', 'database:adminUser']</para>
        /// <remarks>PS! Value from this function is prioritized before the above value from default-keys</remarks>
        /// <para>See <see cref="IDbConfigCredentials.AdminPassword">IDbConfig.AdminPassword</see> (<see cref="IDbConfigCredentials.AdminPassword"/>)</para>
        /// </summary>
        /// <returns></returns>
        string GetDbAdminPassword();

        /// <summary>
        /// <para>Can be used to override the value from default-key: 'database:hostName' </para>
        /// <remarks>PS! Value from this function is prioritized before the above value from default-key</remarks>
        /// <para>See <see cref="IDbConnectionStringBuilderConfig.Hostname">IDbConfig.Hostname</see> (<see cref="IDbConnectionStringBuilderConfig.Hostname"/>)</para>
        /// </summary>
        /// <returns></returns>
        string GetDbHostname();

        /// <summary>
        /// <para>Can be used to override the value from default-key: 'database:port' </para>
        /// <remarks>PS! Value from this function is prioritized before the above value from default-keys</remarks>
        /// <para>See <see cref="IDbConnectionStringBuilderConfig.Port">IDbConfig.Port</see> (<see cref="IDbConnectionStringBuilderConfig.Port"/>)</para>
        /// </summary>
        /// <returns></returns>
        string GetDbPort();

        /// <summary>
        /// <para>Can be used to override the value from default-key: 'database:dataSource' </para>
        /// <remarks>PS! Value from this function is prioritized before the above value from default-key</remarks>
        /// <para>See <see cref="IDbConnectionStringBuilderConfig.Datasource">IDbConfig.Datasource</see> (<see cref="IDbConnectionStringBuilderConfig.Datasource"/>)</para>
        /// </summary>
        /// <returns></returns>
        string GetDbDataSource();

        /// <summary>
        /// <para>Can be used to override the value from default-key: 'database:connectionTimeoutInSecs' </para>
        /// <remarks>PS! Value from this function is prioritized before the above value from default-key</remarks>
        /// <para>See <see cref="IDbConnectionStringBuilderConfig.ConnectionTimeoutInSecs">IDbConfig.ConnectionTimeoutInSecs</see> (<see cref="IDbConnectionStringBuilderConfig.ConnectionTimeoutInSecs"/>)</para>
        /// </summary>
        /// <returns></returns>
        string GetDbConnectionTimeout();

        /// <summary>
        /// <para>Can be used to override the value from default-key: 'database:pooling' </para>
        /// <remarks>PS! Value from this function is prioritized before the above value from default-key</remarks>
        /// <para>See <see cref="IDbConnectionStringBuilderConfig.Pooling">IDbConfig.Pooling</see> (<see cref="IDbConnectionStringBuilderConfig.Pooling"/>)</para>
        /// </summary>
        /// <returns></returns>
        bool? GetDbPooling();

        /// <summary>
        /// <para>Can be used to override the value from default-key: 'database:poolingKeyValues' </para>
        /// <remarks>PS! Value from this function is prioritized before the above value from default-key</remarks>
        /// <para>See <see cref="IDbConnectionStringBuilderConfig.PoolingKeyValues">IDbConfig.Pooling</see> (<see cref="IDbConnectionStringBuilderConfig.PoolingKeyValues"/>)</para>
        /// </summary>
        /// <returns></returns>
        IDictionary<string,string> GetDbPoolingKeyValues();


        /// <summary>
        /// <para>Can be used to override the value from default-key: 'database:connectionString' </para>
        /// <remarks>PS! Value from this function is prioritized before the above value from default-key</remarks>
        /// <para>See <see cref="IDbConfig.ConnectionString"/></para>
        /// </summary>
        /// <returns></returns>
        string GetDbConnectionString();

        /// <summary>
        /// <para>Can be used to override the value from default-key: 'database:adminConnectionString' </para>
        /// <remarks>PS! Value from this function is prioritized before the above value from default-key</remarks>
        /// <para>See <see cref="IDbConfig.AdminConnectionString"/></para>
        /// </summary>
        /// <returns></returns>
        string GetDbAdminConnectionString();
    }
}