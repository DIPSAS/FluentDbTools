namespace FluentDbTools.Common.Abstractions
{
    /// <summary>
    /// Connections String Builder for a supported Database
    /// </summary>
    public interface IDbConnectionStringBuilder
    {
        /// <summary>
        /// <para>Current DatabaseType</para>
        /// <remarks>Default config-key: 'database:type' </remarks>
        /// </summary>
        SupportedDatabaseTypes DatabaseType { get; }

        /// <summary>
        /// <para>Build a ConnectionsString from dbConfig settings and for then specific DatabaseType</para>
        /// </summary>
        /// <param name="dbConfig"></param>
        /// <returns></returns>
        string BuildConnectionString(IDbConnectionStringBuilderConfig dbConfig);

        /// <summary>
        /// <para>Build a AdminConnectionsString from dbConfig settings and for then specific DatabaseType</para>
        /// </summary>
        /// <param name="dbConfig"></param>
        /// <returns></returns>
        string BuildAdminConnectionString(IDbConnectionStringBuilderConfig dbConfig);
    }
}