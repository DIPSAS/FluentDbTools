namespace FluentDbTools.Common.Abstractions
{
    public interface IDbConnectionStringBuilderConfig : IDbConfigDatabaseTargets, IDbConfigCredentials
    {
        
        /// <summary>
        /// Used by oracle typically when using tnsname or ezconnect.
        /// Oracle: Overrides HostName, Database and Port. Can override User and Password if spscified 
        /// </summary>
        string Datasource { get; }

        /// <summary>
        /// Database hostname. If Oracle, this will be overridden if Datasource is specified
        /// </summary>
        string Hostname { get; }

        /// <summary>
        /// Database port. If Oracle, this will be overridden if Datasource is specified
        /// </summary>
        string Port { get; }

        /// <summary>
        /// Used to specify Connection Pooling or not
        /// </summary>
        bool Pooling { get; }

        /// <summary>
        /// Used to specify Connection Timeout or not
        /// </summary>
        string ConnectionTimeoutInSecs { get; }
    }
}