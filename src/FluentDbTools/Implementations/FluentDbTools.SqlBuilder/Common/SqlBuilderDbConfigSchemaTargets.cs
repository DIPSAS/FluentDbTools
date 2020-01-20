using FluentDbTools.Common.Abstractions;

namespace FluentDbTools.SqlBuilder.Common
{
    public class SqlBuilderDbConfigSchemaTargets : IDbConfigSchemaTargets
    {
        private string SchemaPrefixId;
        public SupportedDatabaseTypes DbType { get; }
        public string Schema { get; }
        public string GetSchemaPrefixId() => SchemaPrefixId;

        public SqlBuilderDbConfigSchemaTargets(string schema, string schemaPrefixId, SupportedDatabaseTypes dbType = SupportedDatabaseTypes.Oracle)
        {
            SchemaPrefixId = schemaPrefixId;
            Schema = schema;
            DbType = dbType;
        }
    }

    
}