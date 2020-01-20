namespace FluentDbTools.Common.Abstractions
{
    /// <summary>
    /// Database Connection targets<br/>
    /// Interface:<br/>
    /// • DatabaseName: <see cref="string"/> ──── <remarks>Current Database type</remarks> <br/>
    /// <br/>
    /// Inherits From Interface <see cref="IDbConfigSchemaTargets"/>:<br/>
    /// • DbType: <see cref="SupportedDatabaseTypes"/> ──── <remarks>Current Database type</remarks> <br/>
    /// • Schema: <see cref="string"/> ──── <remarks>Used to specify Schema for connected database</remarks> <br/>
    /// • GetSchemaPrefixId(): Return <returns><see cref="string"/></returns>  ──── <remarks>Can be used to specifying a short Prefix for the Schema.</remarks> <br/>
    /// </summary>
    public interface IDbConfigDatabaseTargets : IDbConfigSchemaTargets
    {
        /// <summary>
        /// Database instance on given host, e.g. Oracle SID or postgres database name.
        /// </summary>        
        string DatabaseName { get; }
    }
}