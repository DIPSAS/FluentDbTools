using FluentDbTools.Common.Abstractions;
// ReSharper disable InconsistentNaming
#pragma warning disable CS1591

namespace FluentDbTools.SqlBuilder.Common
{
    public class SqlBuilderDbConfigSchemaTargets : IDbConfigSchemaTargets
    {
        private readonly string SchemaPrefixId;
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