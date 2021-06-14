using System.Diagnostics.CodeAnalysis;

namespace FluentDbTools.Common.Abstractions.PrioritizedConfig
{
    /// <summary>
    /// <para>Contains methods for overriding config-keys in 'database' and 'database:migration' section.</para>
    /// <para>Name 'Prioritized' is used to signal that these keys is prioritized before default-keys</para>
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "CommentTypo")]
    public interface IPrioritizedConfigKeys
    {
        /// <summary>
        /// <para>Can be used to override default-key: 'database:type'</para>
        /// <remarks>PS! Keys from this function is prioritized before the above default-key</remarks>
        /// <para>See <see cref="IDbConfigSchemaTargets.DbType">IDbConfig.Schema</see> (<see cref="IDbConfigSchemaTargets.DbType"/>)</para>
        /// </summary>
        /// <returns></returns>
        string[] GetDbTypeKeys();

        /// <summary>
        /// <para>Can be used to override default-key: 'database:schema'</para>
        /// <remarks>PS! Keys from this function is prioritized before the above default-key</remarks>
        /// <para>See <see cref="IDbConfigSchemaTargets.Schema">IDbConfig.Schema</see> (<see cref="IDbConfigSchemaTargets.Schema"/>)</para>
        /// </summary>
        /// <returns></returns>
        string[] GetDbSchemaKeys();

        /// <summary>
        /// <para>Can be used to override default-key: 'database:migration:schemaPassword'</para>
        /// <remarks>PS! Keys from this function is prioritized before the above default-key</remarks>
        /// <para>See <a href="https://dipsas.github.io/FluentDbTools/docs/doc/api/FluentDbTools.Migration.Abstractions.IDbMigrationConfig.html#SchemaPassword">IDbMigrationConfig.SchemaPassword</a></para>
        /// </summary>
        /// <returns></returns>
        string[] GetDbSchemaPasswordKeys();

        /// <summary>
        /// <para>Can be used to override default-keys: ['database:schemaPrefix:id','database:migration:schemaPrefix:id']</para>
        /// <remarks>PS! Keys from this function is prioritized before the above default-keys</remarks>
        /// <para>See <see cref="IDbConfigSchemaTargets.GetSchemaPrefixId">IDbConfig.GetSchemaPrefixId()</see> (<see cref="IDbConfigSchemaTargets.GetSchemaPrefixId"/>)</para>
        /// </summary>
        /// <returns></returns>
        string[] GetDbSchemaPrefixIdStringKeys();

        /// <summary>
        /// <para>Can be used to override default-keys: ['database:migration:schemaPrefix:uniqueId','database:schemaPrefix:uniqueId']</para>
        /// <remarks>PS! Keys from this function is prioritized before the above default-keys</remarks>
        /// <para>See <a href="https://dipsas.github.io/FluentDbTools/docs/doc/api/FluentDbTools.Migration.Abstractions.IDbMigrationConfig.html#GetSchemaPrefixUniqueId">IDbMigrationConfig.GetSchemaPrefixUniqueId()</a></para>
        /// </summary>
        /// <returns></returns>
        string[] GetDbSchemaUniquePrefixIdStringKeys();
        /// <summary>
        /// <para>Can be used to override default-keys: ['database:databaseName','database:databaseConnectionName', 'database:serviceName', (postgres only) 'database:schema']</para>
        /// <remarks>PS! Keys from this function is prioritized before the above default-keys</remarks>
        /// <para>See <see cref="IDbConfigDatabaseTargets.DatabaseName">IDbConfig.DatabaseName</see> (<see cref="IDbConfigDatabaseTargets.DatabaseName"/>)</para>
        /// </summary>
        /// <returns></returns>
        string[] GetDbDatabaseNameKeys();

        /// <summary>
        /// <para>Can be used to override default-key: 'database:user'</para>
        /// <remarks>PS! Keys from this function is prioritized before the above default-key</remarks>
        /// <para>See <see cref="IDbConfigCredentials.User">IDbConfig.User</see> (<see cref="IDbConfigCredentials.User"/>)</para>
        /// </summary>
        /// <returns></returns>
        string[] GetDbUserKeys();

        /// <summary>
        /// <para>Can be used to override default-keys ['database:password', 'database:user']</para>
        /// <remarks>PS! Keys from this function is prioritized before the above default-keys</remarks>
        /// <para>See <see cref="IDbConfigCredentials.Password">IDbConfig.Password</see> (<see cref="IDbConfigCredentials.Password"/>)</para>
        /// </summary>
        /// <returns></returns>
        string[] GetDbPasswordKeys();

        /// <summary>
        /// <para>Can be used to override default-key: 'database:adminUser'</para>
        /// <remarks>PS! Keys from this function is prioritized before the above default-key</remarks>
        /// <para>See <see cref="IDbConfigCredentials.AdminUser">IDbConfig.AdminUser</see> (<see cref="IDbConfigCredentials.AdminUser"/>)</para>
        /// </summary>
        /// <returns></returns>
        string[] GetDbAdminUserKeys();

        /// <summary>
        /// <para>Can be used to override default-keys: ['database:adminPassword', 'database:adminUser']</para>
        /// <remarks>PS! Keys from this function is prioritized before the above default-keys</remarks>
        /// <para>See <see cref="IDbConfigCredentials.AdminPassword">IDbConfig.AdminPassword</see> (<see cref="IDbConfigCredentials.AdminPassword"/>)</para>
        /// </summary>
        /// <returns></returns>
        string[] GetDbAdminPasswordKeys();

        /// <summary>
        /// <para>Can be used to override default-key: 'database:hostName' </para>
        /// <remarks>PS! Keys from this function is prioritized before the above default-key</remarks>
        /// <para>See <see cref="IDbConnectionStringBuilderConfig.Hostname">IDbConfig.Hostname</see> (<see cref="IDbConnectionStringBuilderConfig.Hostname"/>)</para>
        /// </summary>
        /// <returns></returns>
        string[] GetDbHostnameKeys();
        
        /// <summary>
        /// <para>Can be used to override default-key: 'database:port' </para>
        /// <remarks>PS! Keys from this function is prioritized before the above default-keys</remarks>
        /// <para>See <see cref="IDbConnectionStringBuilderConfig.Port">IDbConfig.Port</see> (<see cref="IDbConnectionStringBuilderConfig.Port"/>)</para>
        /// </summary>
        /// <returns></returns>
        string[] GetDbPortKeys();

        /// <summary>
        /// <para>Can be used to override default-key: 'database:dataSource' </para>
        /// <remarks>PS! Keys from this function is prioritized before the above default-key</remarks>
        /// <para>See <see cref="IDbConnectionStringBuilderConfig.Datasource">IDbConfig.Datasource</see> (<see cref="IDbConnectionStringBuilderConfig.Datasource"/>)</para>
        /// </summary>
        /// <returns></returns>
        string[] GetDbDataSourceKeys();

        /// <summary>
        /// <para>Can be used to override default-key: 'database:connectionTimeoutInSecs' </para>
        /// <remarks>PS! Keys from this function is prioritized before the above default-key</remarks>
        /// <para>See <see cref="IDbConnectionStringBuilderConfig.ConnectionTimeoutInSecs">IDbConfig.ConnectionTimeoutInSecs</see> (<see cref="IDbConnectionStringBuilderConfig.ConnectionTimeoutInSecs"/>)</para>
        /// </summary>
        /// <returns></returns>
        string[] GetDbConnectionTimeoutKeys();

        /// <summary>
        /// <para>Can be used to override default-key: 'database:pooling' </para>
        /// <remarks>PS! Keys from this function is prioritized before the above default-key</remarks>
        /// <para>See <see cref="IDbConnectionStringBuilderConfig.Pooling">IDbConfig.Pooling</see> (<see cref="IDbConnectionStringBuilderConfig.Pooling"/>)</para>
        /// </summary>
        /// <returns></returns>
        string[] GetDbPoolingKeys();

        /// <summary>
        /// <para>Can be used to override default-key: 'database:poolingKeyValues' </para>
        /// <remarks>PS! Keys from this function is prioritized before the above default-key</remarks>
        /// <para>See <see cref="IDbConnectionStringBuilderConfig.PoolingKeyValues">IDbConfig.PoolingKeyValues</see> (<see cref="IDbConnectionStringBuilderConfig.PoolingKeyValues"/>)</para>
        /// </summary>
        /// <returns></returns>
        string[] GetDbPoolingKeyValuesKeys();

        /// <summary>
        /// <para>Can be used to override default-key: 'database:connectionString' </para>
        /// <remarks>PS! Keys from this function is prioritized before the above default-key</remarks>
        /// <para>See <see cref="IDbConfig.ConnectionString"/></para>
        /// </summary>
        /// <returns></returns>
        string[] GetDbConnectionStringKeys();

        /// <summary>
        /// <para>Can be used to override default-key: 'database:adminConnectionString' </para>
        /// <remarks>PS! Keys from this function is prioritized before the above default-key</remarks>
        /// <para>See <see cref="IDbConfig.AdminConnectionString"/></para>
        /// </summary>
        /// <returns></returns>
        string[] GetDbAdminConnectionStringKeys();

        /// <summary>
        /// When parsing array of <see cref="IPrioritizedConfigKeys"/>-objects, <see cref="Order"/> is used to sort the array.
        /// </summary>
        int Order { get; }
    }
}