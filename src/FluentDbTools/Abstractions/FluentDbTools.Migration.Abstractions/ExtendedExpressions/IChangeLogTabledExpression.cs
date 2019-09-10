using System.Collections.Generic;
using FluentMigrator.Expressions;
using FluentMigrator.Model;

namespace FluentDbTools.Migration.Abstractions.ExtendedExpressions
{
    /// <summary>
    /// Expression to appending customized table logging
    /// i.e: 
    /// </summary>
    public interface IChangeLogTabledExpression : ISchemaExpression
    {
        /// <summary>
        /// Describe the database operation behind this Table-Change-Log operation
        /// Can be:<br/>
        /// CreateTable, AlterTable, RenameTable, DeleteTable, <br/>
        /// CreateColumn, AlterColumn, RenameColumn, DeleteColumn
        /// </summary>
        string DbOperation { get; set; }

        /// <summary>
        /// The TableName for this Table-Change-Log operation
        /// </summary>
        string TableName { get; }

        /// <summary>
        /// The TableDescription for this Table-Change-Log operation
        /// </summary>
        string TableDescription { get; }

        /// <summary>
        /// The Table columns (if any) for this Table-Change-Log operation
        /// </summary>
        IList<ColumnDefinition> Columns { get; }

        /// <summary>
        /// The Table columns (if any) for this Table-Change-Log operation
        /// </summary>
        ChangeLogContext ChangeLogContext { get; }

        /// <summary>
        /// Old name of renamed object<br/>
        /// Can be the old TableName or old ColumnName<br/>
        /// PS! Only in user when <see cref="DbOperation"/> is RenameTable or RenameColumn
        /// </summary>
        string OldRenamedName { get; }

        /// <summary>
        /// The underlying MigrationExpression
        /// </summary>
        /// <example>
        ///     When <see cref="DbOperation"/> is "CreateTable", this will be <see cref="CreateTableExpression"/><br/>
        ///     When <see cref="DbOperation"/> is "AlterTable", this will be <see cref="AlterTableExpression"/><br/>
        ///     When <see cref="DbOperation"/> is "RenameTable", this will be <see cref="RenameTableExpression"/><br/>
        ///     When <see cref="DbOperation"/> is "DeleteTable", this will be <see cref="DeleteTableExpression"/><br/>
        ///     When <see cref="DbOperation"/> is "CreateColumn", this will be <see cref="CreateColumnExpression"/><br/>
        ///     When <see cref="DbOperation"/> is "AlterColumn", this will be <see cref="AlterColumnExpression"/><br/>
        ///     When <see cref="DbOperation"/> is "RenameColumn", this will be <see cref="RenameColumnExpression"/><br/>
        ///     When <see cref="DbOperation"/> is "DeleteColumn", this will be <see cref="DeleteColumnExpression"/><br/>
        /// </example>
        IMigrationExpression Expression { get; }
    }
}