using System.ComponentModel.Design.Serialization;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Migration.Abstractions;
using FluentDbTools.Migration.Contracts.MigrationExpressions;
using FluentMigrator.Builders;
using FluentMigrator.Builders.Alter;
using FluentMigrator.Builders.Alter.Table;
using FluentMigrator.Builders.Create;
using FluentMigrator.Builders.Create.Column;
using FluentMigrator.Builders.Create.Sequence;
using FluentMigrator.Builders.Create.Table;
using FluentMigrator.Builders.Delete;
using FluentMigrator.Builders.Delete.Column;
using FluentMigrator.Builders.Delete.Table;
using FluentMigrator.Builders.Rename;
using FluentMigrator.Builders.Rename.Column;
using FluentMigrator.Builders.Rename.Table;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner.Processors.Oracle;

// ReSharper disable InvertIf

namespace FluentDbTools.Migration.Contracts
{
    /// <summary>
    /// FluentDbTools IMigrationSyntax and IFluentSyntax extensions
    /// </summary>
    public static class MigrationExtensions
    {
        /// <summary>
        /// Creates a table in Schema {<paramref name="migration"/>.SchemaName}<br/>
        /// If {migration.SchemaPrefix} have a value, the Syntax-TableName will be computed to {<paramref name="migration"/>.SchemaPrefix}{<paramref name="tableName"/>}<br/> 
        /// If NOT, the Syntax-TableName will be the same as {<paramref name="tableName"/>}
        /// </summary>
        /// <param name="root">Expression root to extend</param>
        /// <param name="tableName">
        /// The table name<br/>
        /// See summary-section how this parameter is used
        /// </param>
        /// <param name="migration"></param>
        /// <returns></returns>
        public static ICreateTableWithColumnOrSchemaOrDescriptionSyntax Table(this ICreateExpressionRoot root, string tableName, IMigrationModel migration)
        {
            var syntax = root.Table(migration.GetPrefixedName(tableName));
            syntax.InSchema(migration.SchemaName);
            return syntax;
        }


        /// <summary>
        /// Renames a table in Schema {<paramref name="migration"/>.SchemaName}<br/>
        /// If <paramref name="migration"/>.SchemaPrefix have a value, the Syntax-OldName will be computed to {<paramref name="migration"/>.SchemaPrefix}{<paramref name="oldName"/>}<br/> 
        /// If NOT, the Syntax-OldName will be the same as {<paramref name="oldName"/>}
        /// </summary>
        /// <param name="root">Expression root to extend</param>
        /// <param name="oldName">
        /// The current table name<br/>
        /// See summary-section how this parameter is used
        /// </param>
        /// <param name="migration">FluentDbTools <see cref="IMigrationModel"/></param>
        /// <returns>The next step</returns>
        public static IRenameTableToOrInSchemaSyntax Table(this IRenameExpressionRoot root, string oldName, IMigrationModel migration)
        {
            var syntax = root.Table(migration.GetPrefixedName(oldName));
            syntax.InSchema(migration.SchemaName);
            return syntax;
        }

        /// <summary>
        /// Alter the table or its columns/options<br/>
        /// If <paramref name="migration"/>.SchemaPrefix have a value, the Syntax-OldName will be computed to {<paramref name="migration"/>.SchemaPrefix}{<paramref name="tableName"/>}<br/> 
        /// If NOT, the Syntax-OldName will be the same as {<paramref name="tableName"/>}
        /// </summary>
        /// <param name="root">Expression root to extend</param>
        /// <param name="tableName">
        /// The table name to alter<br/>
        /// See summary-section how this parameter is used
        /// </param>
        /// <param name="migration"><see cref="IMigrationModel"/></param>
        /// <returns>The interface for the modifications - <see cref="IAlterTableAddColumnOrAlterColumnOrSchemaOrDescriptionSyntax"/></returns>
        public static IAlterTableAddColumnOrAlterColumnOrSchemaOrDescriptionSyntax Table(this IAlterExpressionRoot root, string tableName, IMigrationModel migration)
        {
            var syntax = root.Table(migration.GetPrefixedName(tableName));
            syntax.InSchema(migration.SchemaName);
            return syntax;
        }

        /// <summary>
        /// Specify the table to delete<br/>
        /// If <paramref name="migration"/>.SchemaPrefix have a value, the Syntax-OldName will be computed to {<paramref name="migration"/>.SchemaPrefix}{<paramref name="tableName"/>}<br/> 
        /// If NOT, the Syntax-OldName will be the same as {<paramref name="tableName"/>}
        /// </summary>
        /// <param name="root">Expression root to extend</param>
        /// <param name="tableName">
        /// The table name to delete<br/>
        /// See summary-section how this parameter is used
        /// </param>
        /// <param name="migration"></param>
        /// <returns>The next step - <see cref="IInSchemaSyntax"/></returns>
        public static IInSchemaSyntax Table(this IDeleteExpressionRoot root, string tableName, IMigrationModel migration)
        {
            var syntax = root.Table(tableName.GetPrefixedName(migration?.SchemaPrefixId));
            syntax.InSchema(migration?.SchemaName);
            return syntax;
        }

        /// <summary>
        /// Specify the table for the new column<br/>
        /// If <paramref name="migration"/>.SchemaPrefix have a value, the Syntax-TableName will be computed to {<paramref name="migration"/>.SchemaPrefix}{<paramref name="tableName"/>}<br/> 
        /// If NOT, the Syntax-TableName will be the same as {<paramref name="tableName"/>}
        /// </summary>
        /// <param name="syntax">Syntax to extend</param>
        /// <param name="tableName">
        /// The table name for the new column <br/>
        /// See summary-section how this parameter is used
        /// </param>
        /// <param name="migration"></param>
        /// <returns>The interface to specify the table schema or column information</returns>
        public static ICreateColumnAsTypeOrInSchemaSyntax OnTable(this ICreateColumnOnTableSyntax syntax, string tableName, IMigrationModel migration)
        {
            var tableSyntax = syntax.OnTable(migration.GetPrefixedName(tableName));
            tableSyntax.InSchema(migration.SchemaName);
            return tableSyntax;
        }


        /// <summary>
        /// Specify the table for the renaming column<br/>
        /// If <paramref name="migration"/>.SchemaPrefix have a value, the Syntax-TableName will be computed to {<paramref name="migration"/>.SchemaPrefix}{<paramref name="tableName"/>}<br/> 
        /// If NOT, the Syntax-TableName will be the same as {<paramref name="tableName"/>}
        /// </summary>
        /// <param name="syntax">Syntax to extend</param>
        /// <param name="tableName">
        /// The table name for the renaming column <br/>
        /// See summary-section how this parameter is used
        /// </param>
        /// <param name="migration"></param>
        /// <returns>The interface to specify the table schema or column information</returns>
        public static IRenameColumnToOrInSchemaSyntax OnTable(this IRenameColumnTableSyntax syntax, string tableName, IMigrationModel migration)
        {
            var tableSyntax = syntax.OnTable(migration.GetPrefixedName(tableName));
            tableSyntax.InSchema(migration.SchemaName);
            return tableSyntax;
        }

        /// <summary>
        /// Specify the new name of the table<br/>
        /// If <paramref name="migration"/>.SchemaPrefix have a value, the Syntax-TableName will be computed to {<paramref name="migration"/>.SchemaPrefix}{<paramref name="tableName"/>}<br/> 
        /// If NOT, the Syntax-TableName will be the same as {<paramref name="tableName"/>}
        /// </summary>
        /// <param name="syntax">Syntax to extend</param>
        /// <param name="tableName">
        /// The new table name<br/>
        /// See summary-section how this parameter is used
        /// </param>
        /// <param name="migration"></param>
        /// <returns>return <see cref="IInSchemaSyntax"/></returns>
        public static IInSchemaSyntax ToTable(this IRenameTableToOrInSchemaSyntax syntax, string tableName, IMigrationModel migration)
        {
            var tableSyntax = syntax.To(migration.GetPrefixedName(tableName));
            tableSyntax.InSchema(migration.SchemaName);
            return tableSyntax;
        }

        /// <summary>
        /// Specify the new column name. 
        /// </summary>
        /// <param name="syntax">Rename Column Syntax to extend</param>
        /// <param name="newColumnName">The new column name</param>
        /// <param name="migration">null can be used</param>
        /// <returns>Return the <paramref name="syntax"/></returns>
        public static IRenameColumnToSyntax To(this IRenameColumnToSyntax syntax, string newColumnName, IMigrationModel migration)
        {
            syntax.To(newColumnName);
            return syntax;
        }


        /// <summary>
        /// Enable DefaultColumns functionality.<br/>
        /// It will be possible to add custom columns when creating table.
        /// The interface <see cref="ICustomMigrationProcessor{T}"/> must be implemented and registered in the IoC container<br/>
        /// Method <see cref="ICustomMigrationProcessor.GetDefaultColumns"/> must be implemented to return all columns <br/>
        /// i.e:<br/>
        /// ServiceProvider.AddSingleton&lt;<see cref="ICustomMigrationProcessor"/>&lt;<see cref="OracleProcessor"/>>,MyCustomMigrationUtilities>()
        /// </summary>
        /// <param name="syntax">CreateTable syntax</param>
        /// <typeparam name="T">Available only for all CreateTable syntax's</typeparam>
        /// <returns>return <paramref name="syntax"/></returns>
        public static T WithDefaultColumns<T>(this T syntax) where T : ICreateTableWithColumnSyntax
        {
            if (syntax is CreateTableExpressionBuilder createTableBuilder)
            {
                var expressions = createTableBuilder.GetMigrationExpressions();
                expressions.Insert(createTableBuilder.Expression, new DefaultColumnsLinkedExpression(createTableBuilder.Expression));
                return syntax;
            }

            return syntax;
        }

        /// <summary>
        /// Enable ChangeLog functionality.<br/>
        /// It will be possible to do custom stuff (i.e Write to a custom log table) for all table and columns operations
        /// </summary>
        /// <param name="syntax">RenameColumn syntax</param>
        /// <param name="changeLog">ChangeLog context object</param>
        /// <param name="migration"><see cref="IMigrationModel"/></param>
        /// <returns></returns>
        public static IRenameColumnToSyntax WithChangeLog(this IRenameColumnToSyntax syntax, ChangeLogContext changeLog, IMigrationModel migration)
        {
            return AddChangeLogDynamicSyntax(syntax, changeLog, migration);
        }

        public static ICreateColumnAsTypeOrInSchemaSyntax WithChangeLog(
            this ICreateColumnAsTypeOrInSchemaSyntax syntax, ChangeLogContext changeLog)
        {
            return AddChangeLogCreateColumnSyntax(syntax, changeLog);
        }
        public static ICreateColumnOptionSyntax WithChangeLog(
            this ICreateColumnOptionSyntax syntax, ChangeLogContext changeLog)
        {
            return AddChangeLogCreateColumnSyntax(syntax, changeLog);
        }

        public static IColumnTypeSyntax<ICreateColumnOptionSyntax> WithChangeLog(
            this IColumnTypeSyntax<ICreateColumnOptionSyntax> syntax,
            ChangeLogContext changeLog)
        {
            return AddChangeLogCreateColumnSyntax(syntax, changeLog);
        }

        public static T WithChangeLog<T>(this T syntax, ChangeLogContext changeLog) where T : ICreateTableWithColumnSyntax
        {
            return AddChangeLogCreateTableSyntax(syntax, changeLog);
        }

        public static IRenameTableToSyntax WithChangeLog(this IRenameTableToSyntax syntax, ChangeLogContext changeLog, IMigrationModel migration)
        {
            return AddChangeLogDynamicSyntax(syntax, changeLog, migration);
        }

        public static IInSchemaSyntax WithChangeLog(this IInSchemaSyntax syntax, ChangeLogContext changeLog, IMigrationModel migration)
        {
            return AddChangeLogDynamicSyntax(syntax, changeLog, migration);
        }

        public static IAlterTableAddColumnOrAlterColumnSyntax WithChangeLog(this IAlterTableAddColumnOrAlterColumnSyntax syntax, ChangeLogContext changeLog)
        {
            return AddChangeLogAlterTableSyntax(syntax, changeLog);
        }

        public static IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax AddForeignKey(this IAlterTableAddColumnOrAlterColumnSyntax table, string primaryTableName, MigrationModel migration)
        {
            return table.AddForeignKey(primaryTableName, migration, migration.SchemaPrefixId);
        }

        public static IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax AddForeignKey(this IAlterTableAddColumnOrAlterColumnSyntax table, string primaryTableName, FluentMigrator.Migration migration, string schemaPrefix = null)
        {
            var expression = ((AlterTableExpressionBuilder)table).Expression;
            var fromTableName = expression.TableName;

            var fkName = migration.GenerateFkName(primaryTableName, fromTableName, schemaPrefix);

            return table
                    .AddColumn(primaryTableName + ColumnName.Id).AsGuid()
                    .ForeignKey(fkName, primaryTableName, ColumnName.Id);
        }

        public static IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax AlterForeignKeyColumn(this IAlterTableAddColumnOrAlterColumnSyntax syntax, string primaryTableName)
        {
            return syntax.AlterColumn(primaryTableName + ColumnName.Id).AsGuid();
        }

        public static ICreateTableColumnAsTypeSyntax WithNullableColumn(this ICreateTableWithColumnSyntax syntax, string columnName)
        {
            var column = syntax.WithColumn(columnName);
            if (column is CreateTableExpressionBuilder builder)
            {
                builder.CurrentColumn.IsNullable = true;
            }

            return column;
        }

        public static IAlterTableColumnAsTypeSyntax AddNullableColumn(this IAlterTableAddColumnOrAlterColumnSyntax syntax, string columnName)
        {
            var column = syntax.AddColumn(columnName);

            if (column is AlterTableExpressionBuilder builder)
            {
                builder.CurrentColumn.IsNullable = true;
            }

            return column;
        }


        public static TNext AsDatabaseDateTime<TNext>(this IColumnTypeSyntax<TNext> syntax, MigrationModel model)
            where TNext : IFluentSyntax
        {
            return model.AsDatabaseDateTime(syntax);
        }


        public static TNext AsDatabaseBlob<TNext>(this IColumnTypeSyntax<TNext> syntax, MigrationModel model)
            where TNext : IFluentSyntax
        {
            return model.AsDatabaseBlob(syntax);
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

        public static ICreateTableColumnOptionOrWithColumnSyntax WithIdAsGuidColumn(this ICreateTableWithColumnSyntax syntax, FluentMigrator.Migration migration)
        {
            return syntax.WithIdColumn(migration).AsGuid().NotNullable();
        }
        public static ICreateTableColumnOptionOrWithColumnSyntax WithIdAsInt32Column(this ICreateTableWithColumnSyntax syntax, FluentMigrator.Migration migration)
        {
            return syntax.WithIdColumn(migration).AsInt32().NotNullable();
        }

        public static ICreateSequenceSyntax WithTableSequence(
            this ICreateTableColumnOptionOrWithColumnSyntax syntax, MigrationModel migration)
        {
            return WithTableSequence(syntax, migration, migration.SchemaPrefixId);
        }


        public static ICreateSequenceSyntax WithTableSequence(
            this ICreateTableColumnOptionOrWithColumnSyntax syntax,
            FluentMigrator.Migration migration,
            string schemaPrefix = null)
        {
            var expression = ((CreateTableExpressionBuilder)syntax).Expression;
            var fromTable = expression.TableName;
            var schemaName = expression.SchemaName;
            var sequenceName = $"{fromTable.GetPrefixedName(schemaPrefix)}_seq";

            return migration.Create
                .Sequence(sequenceName.ToLower())
                .InSchema(schemaName)
                .MaxValue(int.MaxValue).IncrementBy(1);
        }

        public static ICreateTableColumnAsTypeSyntax WithForeignKeyColumn(
            this ICreateTableWithColumnSyntax syntax,
            string primaryTableName,
            MigrationModel migration,
            string columnName = null,
            string primaryColumnName = null)
        {
            return syntax.WithForeignKeyColumn(primaryTableName, migration, migration.SchemaPrefixId, columnName, primaryColumnName);

        }

        public static ICreateTableColumnAsTypeSyntax WithForeignKeyColumn(
            this ICreateTableWithColumnSyntax syntax,
            string primaryTableName,
            FluentMigrator.Migration migration,
            string schemaPrefix = null,
            string columnName = null,
            string primaryColumnName = null)
        {
            var expression = ((CreateTableExpressionBuilder)syntax).Expression;
            var fromTable = expression.TableName.GetPrefixedName(schemaPrefix);
            var fromSchemaName = expression.SchemaName;
            var toSchemaName = fromSchemaName;
            var split = primaryTableName.Split('.');
            if (split.Length == 2)
            {
                primaryTableName = split[1];
                toSchemaName = split[0];
            }

            var fkName = migration?.GenerateFkName(primaryTableName, fromTable.TrimPrefixName(schemaPrefix), schemaPrefix);

            columnName = columnName ?? primaryTableName + ColumnName.Id;
            primaryColumnName = primaryColumnName ?? ColumnName.Id;
            var syntaxWithColumn = syntax.WithColumn(columnName);

            migration?.Create
                .ForeignKey(fkName)
                .FromTable(fromTable).InSchema(fromSchemaName)
                .ForeignColumn(columnName)
                .ToTable(primaryTableName).InSchema(toSchemaName)
                .PrimaryColumn(primaryColumnName)
                .OnDelete(Rule.SetNull);

            return syntaxWithColumn;
        }

        public static string GenerateFkName(this FluentMigrator.Migration migrationModel, string primaryTableName, string fromTableName, string schemaPrefix = null)
        {
            const string prefix = "FK_";
            const string postfix = "";

            var fkName = $"{prefix}{fromTableName.GetPrefixedName(schemaPrefix)}_{primaryTableName}{postfix}";

            if (fkName.Length > 30)
            {
                var length = 13 - (schemaPrefix ?? "").Length / 2;
                fkName = $"{prefix}{GetShortNameOfTableName(fromTableName, length).GetPrefixedName(schemaPrefix)}_{GetShortNameOfTableName(primaryTableName, length)}{postfix}";
            }
            return fkName;
        }

        public static string GeneratePkName(this MigrationModel migration, string primaryTableName,
            string idColumn)
        {
            return migration.GeneratePkName(primaryTableName, idColumn, migration.SchemaPrefixId);
        }

        public static string GeneratePkName(this FluentMigrator.Migration migration, string primaryTableName, string idColumn, string schemaPrefix = null)
        {
            const string prefix = "";
            const string postfix = "_PK";

            var pkName = $"{prefix}{primaryTableName.GetPrefixedName(schemaPrefix)}_{idColumn}{postfix}";

            if (pkName.Length > 30)
            {
                var length = 13 - (schemaPrefix ?? "").Length / 2;
                pkName = $"{prefix}{GetShortNameOfTableName(primaryTableName, length).GetPrefixedName(schemaPrefix)}_{GetShortNameOfTableName(idColumn, length)}{postfix}";
            }
            return pkName;
        }

        private static string GetShortNameOfTableName(string currentTableName, int length = 13)
        {
            return currentTableName.Substring(0, currentTableName.Length > length ? length : currentTableName.Length);
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
        private static T AddChangeLogCreateTableSyntax<T>(T syntax, ChangeLogContext changeLog) where T : ICreateTableWithColumnSyntax
        {
            if (syntax is CreateTableExpressionBuilder createTableBuilder)
            {
                var expressions = createTableBuilder.GetMigrationExpressions();
                expressions.Insert(createTableBuilder.Expression, new ChangeLogLinkedExpression(createTableBuilder, changeLog), true);
            }

            return syntax;
        }

        private static T AddChangeLogAlterTableSyntax<T>(T syntax, ChangeLogContext changeLog) where T : IAlterTableAddColumnOrAlterColumnSyntax
        {
            if (syntax is AlterTableExpressionBuilder alterTableBuilder)
            {
                var expressions = alterTableBuilder.GetMigrationExpressions();
                expressions.Insert(alterTableBuilder.Expression, new ChangeLogLinkedExpression(alterTableBuilder, changeLog), true);
            }

            return syntax;
        }

        private static T AddChangeLogCreateColumnSyntax<T>(T syntax, ChangeLogContext changeLog)
        {
            if (syntax is CreateColumnExpressionBuilder createColumnBuilder)
            {
                var expressions = createColumnBuilder.GetMigrationExpressions();
                expressions.Insert(createColumnBuilder.Expression, new ChangeLogLinkedExpression(createColumnBuilder, changeLog), true);
                return syntax;
            }

            return syntax;
        }

        private static T AddChangeLogDynamicSyntax<T>(T syntax, ChangeLogContext changeLog, IMigrationModel migration)
        {
            var expressions = migration?.GetExpressions();
            if (syntax is RenameTableExpressionBuilder renameTableBuilder)
            {
                expressions?.Insert(renameTableBuilder.Expression, new ChangeLogLinkedExpression(renameTableBuilder, changeLog), true);
                return syntax;
            }
            if (syntax is RenameColumnExpressionBuilder renameColumnBuilder)
            {
                expressions?.Insert(renameColumnBuilder.Expression, new ChangeLogLinkedExpression(renameColumnBuilder, changeLog), true);
                return syntax;
            }

            if (syntax is DeleteTableExpressionBuilder deleteTableBuilder)
            {
                expressions.Insert(deleteTableBuilder.Expression, new ChangeLogLinkedExpression(deleteTableBuilder, changeLog), true);
                return syntax;
            }

            if (syntax is DeleteColumnExpressionBuilder deleteColumnBuilder)
            {
                expressions?.Insert(deleteColumnBuilder.Expression, new ChangeLogLinkedExpression(deleteColumnBuilder, changeLog), true);
                return syntax;
            }

            return syntax;
        }

        internal static bool IsMigrationContextChanged(this IMigrationModel migration, object objectHavingContext)
        {
            var context = objectHavingContext.GetMigrationContextFromObject();
            if (context != null && !Equals(context, migration?.GetMigrationContext()))
            {
                migration?.Reset(context);
                return true;
            }

            return false;
        }


    }
}