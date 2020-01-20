namespace FluentDbTools.Common.Abstractions
{
    /// <summary>
    /// Database Schema targets<br/>
    /// Interface:<br/>
    /// • DbType: <see cref="DbType"/> ──── <remarks>Current Database type</remarks> <br/>
    /// • Schema: <see cref="string"/> ──── <remarks>Used to specify Schema for connected database</remarks> <br/>
    /// • GetSchemaPrefixId(): Return <returns><see cref="string"/></returns>  ──── <remarks>Can be used to specifying a short Prefix for the Schema.</remarks> <br/>
    /// </summary>
    public interface IDbConfigSchemaTargets
    {
        /// <summary>
        /// Current Database type
        /// </summary>
        SupportedDatabaseTypes DbType { get; }

        /// <summary>
        /// Used to specify Schema for connected database
        /// </summary>
        string Schema { get; }

        /// <summary>
        /// Can be used to specifying a short Prefix for the Schema. <br/>
        /// i.e: EX => Tables should be prefixed with EX. <br/>
        ///      Entity Person will result in the EXPerson table in the database.
        /// </summary>
        /// <returns></returns>
        string GetSchemaPrefixId();

    }
}