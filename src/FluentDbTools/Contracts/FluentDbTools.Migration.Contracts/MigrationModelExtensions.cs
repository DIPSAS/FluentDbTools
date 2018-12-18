using System.Data;
using FluentMigrator.Builders;
using FluentMigrator.Builders.Alter.Table;
using FluentMigrator.Builders.Create.Sequence;
using FluentMigrator.Builders.Create.Table;
using FluentMigrator.Infrastructure;

namespace FluentDbTools.Migration.Contracts
{
    public static class MigrationExtensions
    {
        public static IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax AddForeignKey(this IAlterTableAddColumnOrAlterColumnSyntax table, string primaryTableName, FluentMigrator.Migration migration)
        {
            var expression = ((AlterTableExpressionBuilder)table).Expression;
            var fromTableName = expression.TableName;

            var fkName = migration.GenerateFkName(primaryTableName, fromTableName);

            return table
                    .AddColumn(primaryTableName + ColumnName.Id).AsGuid()
                    .ForeignKey(fkName, primaryTableName, ColumnName.Id);
        }

        public static IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax AlterForeignKeyColumn(this IAlterTableAddColumnOrAlterColumnSyntax table, string primaryTableName)
        {
            return table.AlterColumn(primaryTableName + ColumnName.Id).AsGuid();
        }

        public static ICreateTableColumnAsTypeSyntax WithNullableColumn(this ICreateTableWithColumnSyntax tableWithColumnSyntax, string columnName)
        {
            var column = tableWithColumnSyntax.WithColumn(columnName);
            if (column is CreateTableExpressionBuilder builder)
            {
                builder.CurrentColumn.IsNullable = true;
            }
            

            return column;
        }

        public static IAlterTableColumnAsTypeSyntax AddNullableColumn(this IAlterTableAddColumnOrAlterColumnSyntax tableWithColumnSyntax, string columnName)
        {
            var column = tableWithColumnSyntax.AddColumn(columnName);

            if (column is AlterTableExpressionBuilder builder)
            {
                builder.CurrentColumn.IsNullable = true;
            }

            return column;
        }


        public static TNext AsDatabaseDateTime<TNext>(this IColumnTypeSyntax<TNext> column, MigrationModel model)
            where TNext : IFluentSyntax
        {
            return model.AsDatabaseDateTime(column);
        }


        public static TNext AsDatabaseBlob<TNext>(this IColumnTypeSyntax<TNext> column, MigrationModel model)
            where TNext : IFluentSyntax
        {
            return model.AsDatabaseBlob(column);
        }
        public static ICreateTableColumnAsTypeSyntax WithIdColumn(this ICreateTableWithColumnSyntax tableWithColumnSyntax, FluentMigrator.Migration migration)
        {
            var expression = ((CreateTableExpressionBuilder)tableWithColumnSyntax).Expression;
            var syntax = tableWithColumnSyntax
                .WithColumn(ColumnName.Id);

            migration.Create
                .PrimaryKey(migration?.GeneratePkName(expression.TableName, ColumnName.Id))
                .OnTable(expression.TableName)
                .WithSchema(expression.SchemaName)
                .Column(ColumnName.Id);
            return syntax;
        }

        public static ICreateTableColumnOptionOrWithColumnSyntax WithIdAsGuidColumn(this ICreateTableWithColumnSyntax tableWithColumnSyntax, FluentMigrator.Migration migration)
        {
            return tableWithColumnSyntax.WithIdColumn(migration).AsGuid().NotNullable();
        }
        public static ICreateTableColumnOptionOrWithColumnSyntax WithIdAsInt32Column(this ICreateTableWithColumnSyntax tableWithColumnSyntax, FluentMigrator.Migration migration)
        {
            var syntax = tableWithColumnSyntax.WithIdColumn(migration).AsInt32().NotNullable();

            return syntax;
        }

        public static ICreateSequenceSyntax WithTableSequence(
            this ICreateTableColumnOptionOrWithColumnSyntax tableSyntax, FluentMigrator.Migration migration)
        {
            var expression = ((CreateTableExpressionBuilder)tableSyntax).Expression;
            var fromTable = expression.TableName;
            var schemaName = expression.SchemaName;
            var sequenceName = $"{fromTable}_seq";

            return migration.Create
                .Sequence(sequenceName.ToLower())
                .InSchema(schemaName)
                .MaxValue(int.MaxValue).IncrementBy(1);
        }

        public static ICreateTableColumnAsTypeSyntax WithForeignKeyColumn(
            this ICreateTableWithColumnSyntax table, string primaryTableName, FluentMigrator.Migration migration)
        {
            var expression = ((CreateTableExpressionBuilder)table).Expression;
            var fromTable = expression.TableName;
            var schemaName = expression.SchemaName;

            var fkName = migration?.GenerateFkName(primaryTableName, fromTable);

            var foreignColumn = primaryTableName + ColumnName.Id;
            var syntax = table.WithColumn(foreignColumn);

            migration?.Create
                .ForeignKey(fkName)
                .FromTable(fromTable)
                .InSchema(schemaName)
                .ForeignColumn(foreignColumn)
                .ToTable(primaryTableName)
                .InSchema(schemaName)
                .PrimaryColumn(ColumnName.Id)
                .OnDelete(Rule.SetNull);

            return syntax;
        }

        public static string GenerateFkName(this FluentMigrator.Migration migrationModel, string primaryTableName, string fromTableName)
        {
            const string prefix = "FK_";
            const string postfix = "";


            var fkName = $"{prefix}{fromTableName}_{primaryTableName}{postfix}";

            if (fkName.Length > 30)
            {
                fkName = $"{prefix}{GetShortNameOfTableName(fromTableName)}_{GetShortNameOfTableName(primaryTableName)}{postfix}";
            }
            return fkName;
        }

        public static string GeneratePkName(this FluentMigrator.Migration migration, string primaryTableName, string idColumn)
        {
            const string prefix = "PK_";
            const string postfix = "";

            var pkName = $"{prefix}{primaryTableName}_{idColumn}{postfix}";

            if (pkName.Length > 30)
            {
                pkName = $"{prefix}{GetShortNameOfTableName(primaryTableName)}_{GetShortNameOfTableName(idColumn)}{postfix}";
            }
            return pkName;
        }

        private static string GetShortNameOfTableName(string currentTableName)
        {
            return currentTableName.Substring(0, currentTableName.Length > 13 ? 13 : currentTableName.Length);
        }

        public static string GetConfigurtedDatabaseType(this FluentMigrator.Migration migration)
        {
            var configuredDatabaseType = string.Empty;
            migration.IfDatabase(s =>
            {
                configuredDatabaseType = s;
                return true;
            });

            return configuredDatabaseType;
        }
    }
}