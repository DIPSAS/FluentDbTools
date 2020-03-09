using System.Collections.Generic;

namespace FluentDbTools.Common.Abstractions
{
    /// <summary>
    /// Database ConnectionStrings properties
    /// </summary>
    public interface IDbConfig : IDbConnectionStringBuilderConfig
    {
        /// <summary>
        /// <para>If set, overrides all other config values related to the connection string.</para>
        /// <remarks>Default config-key: 'database:connectionString'</remarks>
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// <para>If set, overrides all other config values related to the admin connection string.</para>
        /// <remarks>Default config-key: 'database:adminConnectionString'</remarks>
        /// </summary>
        string AdminConnectionString { get; }

        /// <summary>
        /// <para>GetAllDatabaseConfigValues() : Get all values and subValues from configuration "database".</para>
        /// </summary>
        IDictionary<string, string> GetAllDatabaseConfigValues(bool reload = false, string sectionName = null);

    }
}