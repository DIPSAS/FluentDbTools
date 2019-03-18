using FluentMigrator.Runner.Generators;
using FluentMigrator.Runner.Generators.Generic;
using FluentMigrator.Runner.Generators.Postgres;

namespace FluentDbTools.Migration.Postgres
{
    internal class OverridenPostgresDescriptionGenerator : GenericDescriptionGenerator
    {
        private readonly IQuoter _quoter;

        public OverridenPostgresDescriptionGenerator(PostgresQuoter quoter)
        {
            _quoter = quoter;
        }

        protected IQuoter Quoter
        {
            get { return _quoter; }
        }

        #region Constants

        private const string TableDescriptionTemplate = "COMMENT ON TABLE {0} IS '{1}';";
        private const string ColumnDescriptionTemplate = "COMMENT ON COLUMN {0}.{1} IS '{2}';";

        #endregion

        private string GetFullTableName(string schemaName, string tableName)
        {
            return Quoter.QuoteTableName(tableName, schemaName);
        }

        protected override string GenerateTableDescription(
            string schemaName, string tableName, string tableDescription)
        {
            if (string.IsNullOrEmpty(tableDescription))
                return string.Empty;

            return string.Format(TableDescriptionTemplate, GetFullTableName(schemaName, tableName), tableDescription.Replace("'", "''"));
        }

        protected override string GenerateColumnDescription(
            string schemaName, string tableName, string columnName, string columnDescription)
        {
            if (string.IsNullOrEmpty(columnDescription))
                return string.Empty;

            return string.Format(
                ColumnDescriptionTemplate,
                GetFullTableName(schemaName, tableName),
                Quoter.QuoteColumnName(columnName),
                columnDescription.Replace("'", "''"));
        }
    }
}