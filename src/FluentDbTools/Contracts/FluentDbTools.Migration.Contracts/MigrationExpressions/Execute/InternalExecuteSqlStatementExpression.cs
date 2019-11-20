using System;
using FluentDbTools.Common.Abstractions;
using FluentMigrator.Expressions;

namespace FluentDbTools.Migration.Contracts.MigrationExpressions.Execute
{
    /// <summary>
    /// Expression to execute an SQL statement
    /// </summary>
    internal class InternalExecuteSqlStatementExpression : ExecuteSqlStatementExpression
    {
        public Func<string,string> AdditionalSqlTitleConverterFunc { get; set; }
        /// <inheritdoc />
        public override string ToString()
        {
            return StaticToString(GetType(), SqlStatement, AdditionalSqlTitleConverterFunc);
        }

        internal static string StaticToString(Type type, string sql, Func<string,string> additionalTitleConverterFunc = null) =>
            $"{type.Name.Replace("Expression", "").Replace("Internal", "")} {sql.ConvertToSqlTitle(additionalTitleConverterFunc)}";
    }
}