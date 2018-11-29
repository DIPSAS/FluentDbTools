using FluentMigrator.Runner.Generators.Postgres;

namespace DIPS.FluentDbTools.Migration.Postgres
{
    internal class OverriddenPostgresQouter : PostgresQuoter
    {
        public override string QuoteColumnName(string columnName)
        {
            columnName = columnName.ToLower();
            return IsQuoted(columnName) ? UnQuote(columnName) : columnName;
        }

        public override string QuoteConstraintName(string constraintName, string schemaName = null)
        {
            constraintName = constraintName.ToLower();
            return IsQuoted(constraintName) ? UnQuote(constraintName) : constraintName;
        }

        public override string QuoteIndexName(string indexName, string schemaName)
        {
            indexName = indexName.ToLower();
            return IsQuoted(indexName) ? UnQuote(indexName) : indexName;
        }

        public override string QuoteSequenceName(string sequenceName, string schemaName)
        {
            sequenceName = sequenceName.ToLower();
            return CreateSchemaPrefixedQuotedIdentifier(
                QuoteSchemaName(schemaName),
                IsQuoted(sequenceName) ? UnQuote(sequenceName) : sequenceName);
        }

        public override string QuoteTableName(string tableName, string schemaName = null)
        {
            //if (tableName == "VersionInfo")
            //{
            //    return base.QuoteTableName(tableName, schemaName);
            //}

            tableName = tableName.ToLower();
            return CreateSchemaPrefixedQuotedIdentifier(
                QuoteSchemaName(schemaName),
                IsQuoted(tableName) ? UnQuote(tableName) : tableName);
        }
    }
}