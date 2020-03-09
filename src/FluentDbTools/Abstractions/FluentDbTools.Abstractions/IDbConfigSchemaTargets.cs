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
        /// <para>Current Database type</para>
        /// <remarks>Default config-key: 'database:type' </remarks>
        /// </summary>
        SupportedDatabaseTypes DbType { get; }

        /// <summary>
        /// <para>Used to specify Schema for connected database</para>
        /// <remarks>Default config-key: 'database:schema' </remarks>
        /// </summary>
        string Schema { get; }

        /// <summary>
        /// <para>Can be used to specifying a short Prefix for the Schema.</para>
        /// <example>
        /// <para>i.e: EX => Tables should be prefixed with EX. </para>
        /// <para>          Entity Person will result in the EXPerson table in the database.</para>
        /// </example>
        /// <remarks>Default config-keys: ['database:schemaPrefix:id', 'database:migration:schemaPrefix:id'] </remarks>
        /// </summary>
        /// <returns></returns>
        string GetSchemaPrefixId();

    }
}