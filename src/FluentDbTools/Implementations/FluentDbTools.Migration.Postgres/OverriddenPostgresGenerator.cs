using System.Linq;
using System.Text;
using FluentMigrator.Expressions;
using FluentMigrator.Runner.Generators;
using FluentMigrator.Runner.Generators.Base;
using FluentMigrator.Runner.Generators.Postgres;
using Microsoft.Extensions.Options;

namespace FluentDbTools.Migration.Postgres
{
    internal class OverriddenPostgresGenerator : PostgresGenerator
    {
        private new readonly OverriddenPostgresColumn Column;

        public OverriddenPostgresGenerator(
            PostgresQuoter quoter,
            OverriddenPostgresColumn column,
            IOptions<GeneratorOptions> generatorOptions)
            : base (quoter, generatorOptions)
        {
            Column = column;
            this.SetPrivateFieldValue<GeneratorBase>("_quoter", quoter);
            this.SetPrivateFieldValue<GeneratorBase>("_column",column);
        }

        public override string Generate(AlterColumnExpression expression)
        {
            var alterStatement = new StringBuilder();
            alterStatement.AppendFormat(
                "ALTER TABLE {0} {1};",
                Quoter.QuoteTableName(expression.TableName, expression.SchemaName),
                Column.GenerateAlterClauses(expression.Column));
            var descriptionStatement = DescriptionGenerator.GenerateDescriptionStatement(expression);
            if (!string.IsNullOrEmpty(descriptionStatement))
            {
                alterStatement.Append(";");
                alterStatement.Append(descriptionStatement);
            }
            return alterStatement.ToString();
        }

        public override string Generate(CreateTableExpression expression)
        {
            foreach (var expressionColumn in expression.Columns)
            {
                expressionColumn.Name = expressionColumn.Name.ToLower();
                expressionColumn.TableName = expressionColumn.TableName.ToLower();
            }

            var createStatement = new StringBuilder();
            createStatement.AppendFormat(
                "CREATE TABLE {0} ({1})",
                Quoter.QuoteTableName(expression.TableName, expression.SchemaName),
                Column.Generate(expression.Columns, Quoter.QuoteTableName(expression.TableName)));
            var descriptionStatement = DescriptionGenerator.GenerateDescriptionStatements(expression)
                ?.ToList();
            createStatement.Append(";");

            if (descriptionStatement != null && descriptionStatement.Count != 0)
            {
                createStatement.Append(string.Join(";", descriptionStatement.ToArray()));
                createStatement.Append(";");
            }
            return createStatement.ToString();
        }

        public override string Generate(RenameTableExpression expression)
        {
            var sql = $"ALTER TABLE {Quoter.QuoteTableName(expression.OldName, expression.SchemaName)} RENAME TO {Quoter.Quote(expression.NewName)};";
            return sql;
        }
    }
}