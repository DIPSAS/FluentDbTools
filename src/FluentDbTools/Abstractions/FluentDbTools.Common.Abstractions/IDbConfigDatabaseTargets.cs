namespace FluentDbTools.Common.Abstractions
{
    /// <summary>
    /// Database Connection targets
    /// </summary>
    public interface IDbConfigDatabaseTargets 
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
        /// Database instance on given host, e.g. Oracle SID or postgres database name.
        /// </summary>        
        string DatabaseName { get; }

        /// <summary>
        /// Can be used to specifying a short Prefix for the Schema. <br/>
        /// i.e: EX => Tables should be prefixed with EX. <br/>
        ///      Entity Person will result in the EXPerson table in the database.
        /// </summary>
        /// <returns></returns>
        string GetSchemaPrefixId();

        /// <summary>
        /// Can be used to specifying a unique Id for the <see cref="IDbConfigDatabaseTargets.GetSchemaPrefixId"/>
        /// </summary>
        /// <returns></returns>
        string GetSchemaPrefixUniqueId();

    }
}