using FluentDbTools.Migration.Abstractions.ExtendedExpressions;

namespace FluentDbTools.Migration.Abstractions.DataDictionary
{
    public interface IDataDictionarySchemaPrefixAppender
    {
        void AppendToDatabase(string schemaName, string schemaPrefixId = null, string schemaPrefixUniqueId = null, DataDictionaryRequiredLevel? requiredLevel = DataDictionaryRequiredLevel.OnlyTablesVersion2);
        void AppendToDatabase(ICreateSchemaWithPrefixExpression expression, DataDictionaryRequiredLevel? requiredLevel = DataDictionaryRequiredLevel.OnlyTablesVersion2);
    }
}