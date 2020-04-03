using FluentDbTools.Migration.Abstractions.ExtendedExpressions;

namespace FluentDbTools.Migration.Abstractions.DataDictionary
{
    public interface IDataDictionarySchemaPrefixAppender
    {
        void AppendToDatabase(string schemaName, string schemaPrefixId, string schemaPrefixUniqueId, DataDictionaryRequiredLevel? requiredLevel = DataDictionaryRequiredLevel.OnlySchemaPrefix);
        void AppendToDatabase(ICreateSchemaWithPrefixExpression expression, DataDictionaryRequiredLevel? requiredLevel = DataDictionaryRequiredLevel.OnlySchemaPrefix);
    }
}