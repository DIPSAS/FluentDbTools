namespace FluentDbTools.Common.Abstractions
{
    /// <summary>
    /// Database ConnectionStrings properties
    /// </summary>
    public interface IDbConfig : IDbConnectionStringBuilderConfig
    {
        /// <summary>
        /// If set, overrides all other config values related to the connection string.
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// If set, overrides all other config values related to the admin connection string.
        /// </summary>
        string AdminConnectionString { get; }
    }
}