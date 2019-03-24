namespace FluentDbTools.Common.Abstractions
{
    public interface IDbConfigDatabaseTargets 
    {
        SupportedDatabaseTypes DbType { get; }

        /// <summary>
        /// Used to specify Schema for connected database
        /// </summary>
        string Schema { get; }

        /// <summary>
        /// Database instance on given host, e.g. Oracle SID or postgresl database name.
        /// </summary>        
        string DatabaseName { get; }
    }
}