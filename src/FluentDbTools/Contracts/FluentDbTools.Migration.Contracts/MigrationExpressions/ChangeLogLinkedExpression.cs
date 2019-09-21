using System;
using System.Collections.Generic;
using System.Linq;
using FluentDbTools.Migration.Abstractions;
using FluentDbTools.Migration.Abstractions.ExtendedExpressions;
using FluentMigrator.Builders.Alter.Table;
using FluentMigrator.Builders.Create.Column;
using FluentMigrator.Builders.Create.Table;
using FluentMigrator.Builders.Delete.Column;
using FluentMigrator.Builders.Delete.Table;
using FluentMigrator.Builders.Rename.Column;
using FluentMigrator.Builders.Rename.Table;
using FluentMigrator.Expressions;
using FluentMigrator.Model;

namespace FluentDbTools.Migration.Contracts.MigrationExpressions
{
    /// <summary>
    /// Expression <see cref="IChangeLogTabledExpression"/> to appending customized table logging
    /// Implement Interface <see cref="ILinkedExpression"/> to signal that this class <see cref="ChangeLogLinkedExpression"/> is linked to another Expression
    /// </summary>
    public class ChangeLogLinkedExpression : PerformDBOperationExpression, ILinkedExpression, IChangeLogTabledExpression
    {
        private Func<IList<ColumnDefinition>> GetColumns { get; }

        /// <inheritdoc />
        public IMigrationExpression Expression { get; }

        /// <inheritdoc />
        public string SchemaName { get; set; }

        /// <inheritdoc />
        public string DbOperation { get; set; }

        /// <inheritdoc />
        public string TableName { get; }

        /// <inheritdoc />
        public string OldRenamedName { get; }

        /// <inheritdoc />
        public string TableDescription { get; }

        /// <inheritdoc />
        public ChangeLogContext ChangeLogContext { get; }

        /// <inheritdoc />
        public IList<ColumnDefinition> Columns => GetColumns?.Invoke();

        /// <summary>
        /// Determine if the linked expression is equal
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is ChangeLogLinkedExpression ext && Equals(Expression, ext.Expression);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (TableName?.GetHashCode() ?? 0) |
                   (ChangeLogContext?.GetHashCode() ?? 0) |
                   (Expression?.GetHashCode() ?? 0);
        }

        /// <summary>
        /// Common constructor
        /// </summary>
        /// <param name="changeLog">The <see cref="ChangeLogContext"/> object</param>
        /// <param name="expression">The <see cref="IMigrationExpression"/> object</param>
        protected ChangeLogLinkedExpression(ChangeLogContext changeLog, IMigrationExpression expression)
        {
            Operation = (connection, transaction) => { };
            Expression = expression;
            ChangeLogContext = changeLog;
            DbOperation = Expression.GetDbOperation();
            GetColumns = () => new List<ColumnDefinition>();
        }

        /// <summary>
        /// Initialize object from the <see cref="DeleteColumnExpressionBuilder"/> object
        /// </summary>
        /// <param name="builder">The <see cref="DeleteColumnExpressionBuilder"/> object</param>
        /// <param name="changeLog">The <see cref="ChangeLogContext"/> object</param>
        public ChangeLogLinkedExpression(DeleteColumnExpressionBuilder builder, ChangeLogContext changeLog)
            : this(changeLog, builder.Expression)
        {
            GetColumns = () => builder.Expression.ColumnNames.Select(x => new ColumnDefinition { Name = x, TableName = TableName }).ToList();

            SchemaName = builder.Expression.SchemaName;
            TableName = builder.Expression.TableName;
        }

        /// <summary>
        /// Initialize object from the <see cref="DeleteTableExpressionBuilder"/> object
        /// </summary>
        /// <param name="builder">The <see cref="DeleteTableExpressionBuilder"/> object</param>
        /// <param name="changeLog">The <see cref="ChangeLogContext"/> object</param>
        public ChangeLogLinkedExpression(DeleteTableExpressionBuilder builder, ChangeLogContext changeLog)
            : this(changeLog, builder.Expression)
        {
            SchemaName = builder.Expression.SchemaName;
            TableName = builder.Expression.TableName;
        }

        /// <summary>
        /// Initialize object from the <see cref="CreateColumnExpressionBuilder"/> object
        /// </summary>
        /// <param name="builder">The <see cref="CreateColumnExpressionBuilder"/> object</param>
        /// <param name="changeLog">The <see cref="ChangeLogContext"/> object</param>
        public ChangeLogLinkedExpression(CreateColumnExpressionBuilder builder, ChangeLogContext changeLog)
            : this(changeLog, builder.Expression)
        {
            GetColumns = () => new List<ColumnDefinition> { builder.Expression.Column };
            SchemaName = builder.Expression.SchemaName;
            TableName = builder.Expression.TableName;
        }

        /// <summary>
        /// Initialize object from the <see cref="RenameColumnExpressionBuilder"/> object
        /// </summary>
        /// <param name="builder">The <see cref="RenameColumnExpressionBuilder"/> object</param>
        /// <param name="changeLog">The <see cref="ChangeLogContext"/> object</param>
        public ChangeLogLinkedExpression(RenameColumnExpressionBuilder builder, ChangeLogContext changeLog)
        : this(changeLog, builder.Expression)
        {
            OldRenamedName = builder.Expression.OldName;
            GetColumns = () => new List<ColumnDefinition>
            {
                new ColumnDefinition() {Name = builder.Expression.NewName, TableName = builder.Expression.TableName}
            };
            SchemaName = builder.Expression.SchemaName;
            TableName = builder.Expression.NewName;
        }

        /// <summary>
        /// Initialize object from the <see cref="RenameTableExpressionBuilder"/> object
        /// </summary>
        /// <param name="builder">The <see cref="RenameTableExpressionBuilder"/> object</param>
        /// <param name="changeLog">The <see cref="ChangeLogContext"/> object</param>
        public ChangeLogLinkedExpression(RenameTableExpressionBuilder builder, ChangeLogContext changeLog)
            : this(changeLog, builder.Expression)
        {
            SchemaName = builder.Expression.SchemaName;
            TableName = builder.Expression.NewName;
            OldRenamedName = builder.Expression.OldName;
        }

        /// <summary>
        /// Initialize object from the <see cref="CreateTableExpressionBuilder"/> object
        /// </summary>
        /// <param name="builder">The <see cref="CreateTableExpressionBuilder"/> object</param>
        /// <param name="changeLog">The <see cref="ChangeLogContext"/> object</param>
        public ChangeLogLinkedExpression(CreateTableExpressionBuilder builder, ChangeLogContext changeLog)
            : this(changeLog, builder.Expression)
        {
            GetColumns = () => builder.Expression.Columns;
            SchemaName = builder.Expression.SchemaName;
            TableName = builder.Expression.TableName;
            TableDescription = builder.Expression.TableDescription;
        }


        /// <summary>
        /// Initialize object from the <see cref="AlterTableExpressionBuilder"/> object
        /// </summary>
        /// <param name="builder">The <see cref="AlterTableExpressionBuilder"/> object</param>
        /// <param name="changeLog">The <see cref="ChangeLogContext"/> object</param>
        public ChangeLogLinkedExpression(AlterTableExpressionBuilder builder, ChangeLogContext changeLog)
            : this(changeLog, builder.Expression)
        {
            GetColumns = () => builder.GetMigrationExpressions().GetAlterColumns(builder.Expression.TableName);
            SchemaName = builder.Expression.SchemaName;
            TableName = builder.Expression.TableName;
            TableDescription = builder.Expression.TableDescription;
        }


        /// <inheritdoc />
        public override string ToString()
        {
            return $"ChangeLog Expression is linked to [{Expression}] expression";
        }
    }
}