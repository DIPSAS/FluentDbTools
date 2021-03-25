namespace FluentDbTools.Migration.Abstractions.DataDictionary
{
    /// <summary>
    /// 
    /// </summary>
    public enum DataDictionaryRequiredLevel
    {
        /// <summary>
        /// Disable all DataDictionary functionality
        /// </summary>
        Disabled=0,

        /// <summary>
        /// Only enable to register Schema(and prefix) into the DataDictionary (Old SchemaPrefix register)
        /// </summary>
        OnlySchemaPrefix=1,

        /// <summary>
        /// Register both Schema(and prefix) and tables into the DataDictionary (Old SchemaPrefix register and Old DataDictionTable register)
        /// </summary>
        BothSchemaPrefixAndTables=2,

        /// <summary>
        /// Register only tables into the new DataDictionary version2 register
        /// </summary>
        OnlyTablesVersion2=3,

        /// Register both Schema(and prefix) and tables into the Old SchemaPrefix register and new DataDictionary version2 register
        BothSchemaPrefixAndTablesVersion2=4,

    }
}