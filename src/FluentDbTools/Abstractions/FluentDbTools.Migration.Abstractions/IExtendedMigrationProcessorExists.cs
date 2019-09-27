
// ReSharper disable All

namespace FluentDbTools.Migration.Abstractions
{
    /// <summary>
    /// Important database Exists methods
    /// </summary>
    public interface IExtendedMigrationProcessorExists
    {
        /// <summary>
        /// Generic Exists function <br/>
        /// i.e: SELECT 1 FROM ALL_USERS WHERE USERNAME = '{0}'
        /// </summary>
        /// <param name="template"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        bool IsExists(string template, params object[] args);

        /// <summary>
        /// Generic Exists function <br/>
        /// i.e: SELECT 1 FROM ALL_USERS WHERE USERNAME = '{0}'
        /// </summary>
        /// <param name="template"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        bool Exists(string template, params object[] args);

        /// <summary>
        /// Return true if schema specified in '<paramref name="schemaName"/>' exists in the database
        /// </summary>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        bool SchemaExists(string schemaName);

        /// <summary>
        /// Return true if table specified in '<paramref name="schemaName"/>.<paramref name="tableName"/>' exists in the database
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        bool TableExists(string schemaName, string tableName);

        /// <summary>
        /// Return true if column specified in '<paramref name="schemaName"/>.<paramref name="tableName"/>.<paramref name="columnName"/>' exists in the database
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        bool ColumnExists(string schemaName, string tableName, string columnName);

        /// <summary>
        /// Return true if constrain specified in '<paramref name="schemaName"/>.<paramref name="tableName"/>.<paramref name="constraintName"/>' exists in the database
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="tableName"></param>
        /// <param name="constraintName"></param>
        /// <returns></returns>
        bool ConstraintExists(string schemaName, string tableName, string constraintName);

        /// <summary>
        /// Return true if index specified in '<paramref name="schemaName"/>.<paramref name="tableName"/>.<paramref name="indexName"/>' exists in the database
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="tableName"></param>
        /// <param name="indexName"></param>
        /// <returns></returns>
        bool IndexExists(string schemaName, string tableName, string indexName);

        /// <summary>
        /// Return true if sequence specified in '<paramref name="schemaName"/>.<paramref name="sequenceName"/>' exists in the database
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="sequenceName"></param>
        /// <returns></returns>
        bool SequenceExists(string schemaName, string sequenceName);

        /// <summary>
        /// Return true if defaultValue specified in '<paramref name="schemaName"/>.<paramref name="tableName"/>.<paramref name="columnName"/>.<paramref name="defaultValue"/>' exists in the database
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        bool DefaultValueExists(string schemaName, string tableName, string columnName, object defaultValue);
    }
}