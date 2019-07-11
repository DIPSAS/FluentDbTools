namespace FluentDbTools.Common.Abstractions
{
    /// <summary>
    /// Connections String Builder for a supported Database
    /// </summary>
    public interface IDbConnectionStringBuilder
    {
        /// <summary>
        /// Current DatabaseType
        /// </summary>
        SupportedDatabaseTypes DatabaseType { get; }
        /// <summary>
        /// Build a ConnectionsString from dbConfig settings and for then specific DatabaseType
        /// </summary>
        /// <param name="dbConfig"></param>
        /// <returns></returns>
        string BuildConnectionString(IDbConnectionStringBuilderConfig dbConfig);

        /// <summary>
        /// Build a AdminConnectionsString from dbConfig settings and for then specific DatabaseType
        /// </summary>
        /// <param name="dbConfig"></param>
        /// <returns></returns>
        string BuildAdminConnectionString(IDbConnectionStringBuilderConfig dbConfig);
    }
}