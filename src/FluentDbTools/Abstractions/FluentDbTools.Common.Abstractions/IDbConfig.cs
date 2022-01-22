using System.Collections.Generic;
using FluentDbTools.Common.Abstractions.PrioritizedConfig;
// ReSharper disable UnusedMember.Global

namespace FluentDbTools.Common.Abstractions
{
    /// <summary>
    /// Database ConnectionStrings properties
    /// </summary>
    public interface IDbConfig : IDbConnectionStringBuilderConfig, IDbConfigAdminExtension
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

        /// <summary>
        /// The method to get config value directly from the configuration
        /// </summary>
        string GetConfigValue(params string[] keys);

        /// <summary>
        /// <para>Get Prioritized config keys</para>
        /// <para>Name 'Prioritized' is used to signal that these keys is prioritized before default-keys</para>
        /// </summary>
        /// <returns></returns>
        IPrioritizedConfigKeys[] GetPrioritizedConfigKeys();

        /// <summary>
        /// The delimiter/separator between config-sections and value
        /// </summary>
        string ConfigurationDelimiter { get; set; }
    }
}