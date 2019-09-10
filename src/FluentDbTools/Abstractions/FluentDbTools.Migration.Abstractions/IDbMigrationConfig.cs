using System;
using System.Collections.Generic;
using FluentDbTools.Common.Abstractions;

namespace FluentDbTools.Migration.Abstractions
{
    public interface IDbMigrationConfig : IDbConfigDatabaseTargets
    {
        /// <summary>
        /// Used to select other database specific configurations
        /// </summary>
        Func<IDbConfig> GetDbConfig { get; }

        /// <summary>
        /// SchemaPassword. Will follow configuration parameter "database:migration:schemaPassword". 
        /// - Only used when creating a new Schema.
        /// - If empty, it will fallback to GetDbConfig().Password
        /// </summary>
        string SchemaPassword { get; }

        /// <summary>
        /// Default tablespace for Oracle. Will follow configuration parameter "database:migration:defaultTablespace". 
        /// - If empty, it will fallback to configuration parameter "database:defaultTablespace"
        /// </summary>
        string DefaultTablespace { get; }

        /// <summary>
        /// Temp tablespace for Oracle. Will follow configuration parameter "database:migration:tableSpace". 
        /// - If empty, it will fallback to configuration parameter "database:tableSpace"
        /// </summary>
        string TempTablespace { get; }

        /// <summary>
        /// Database owner: Will follow configuration parameter "database:migration:databaseOwner". 
        /// - Only used when creating a new Postgres database
        /// - If empty, it will fallback to GetDbConfig().AdminUser
        /// </summary>
        string DatabaseOwner { get; }

        /// <summary>
        /// Migration ProcessorId:
        /// - Oracle: Value of FluentDbTools.Migration.Common.ProcessorIds.OracleProcessorId
        /// - Postgres: Value of FluentDbTools.Migration.Common.ProcessorIds.PostgresProcessorId
        /// </summary>
        string ProcessorId { get; }

        /// <summary>
        /// Admin ConnectionString string to the database. 
        /// - Value is always selected from GetDbConfig().GetAdminConnectionString()
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// GetAllMigrationConfigValues() : Get al values and subValues from configuration "database:migration". 
        /// </summary>
        IDictionary<string, string> GetAllMigrationConfigValues(bool reload = false);

        /// <summary>
        /// Can be used to specifying a unique Id for the <see cref="IDbConfigDatabaseTargets.GetSchemaPrefixId"/>
        /// </summary>
        /// <returns></returns>
        string GetSchemaPrefixUniqueId();
    }
}