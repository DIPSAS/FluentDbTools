using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FluentDbTools.Common.Abstractions
{
    /// <summary>
    /// Datasource Connection String properties
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public interface IDbConnectionStringBuilderConfig : IDbConfigDatabaseTargets, IDbConfigCredentials
    {
        /// <summary>
        /// <para>Used by oracle typically when using tnsName or EzConnect.</para>
        /// <remarks>Oracle: Overrides HostName, Database and Port. Can override User and Password if specified</remarks>
        /// <remarks><para>Default config-key: 'database:dataSource'</para></remarks>
        /// </summary>
        string Datasource { get; }

        /// <summary>
        /// <para>Database hostname. If Oracle, this will be overridden if Datasource is specified</para>
        /// <remarks>Default config-key: 'database:hostname'</remarks>
        /// </summary>
        string Hostname { get; }

        /// <summary>
        /// <para>Database port. If Oracle, this will be overridden if Datasource is specified</para>
        /// <remarks>Default config-key: 'database:port'</remarks>
        /// </summary>
        string Port { get; }

        /// <summary>
        /// <para>Used to specify Connection Pooling or not</para>
        /// <remarks>Default config-key: 'database:pooling'</remarks>
        /// </summary>
        bool Pooling { get; }

        /// <summary>
        /// Additional Pooling properties
        /// </summary>
        IDictionary<string,string> PoolingKeyValues { get; }

        /// <summary>
        /// <para>Used to specify Connection Timeout or not</para>
        /// <remarks>Default config-key: 'database:connectionTimeoutInSecs'</remarks>
        /// </summary>
        string ConnectionTimeoutInSecs { get; }
    }
}