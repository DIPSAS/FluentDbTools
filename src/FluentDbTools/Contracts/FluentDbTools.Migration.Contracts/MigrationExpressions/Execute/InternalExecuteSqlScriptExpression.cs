using System;
using FluentDbTools.Common.Abstractions;
using FluentMigrator.Expressions;

namespace FluentDbTools.Migration.Contracts.MigrationExpressions.Execute
{
    /// <summary>
    /// Expression to execute SQL scripts
    /// </summary>
    internal class InternalExecuteSqlScriptExpression : ExecuteSqlScriptExpression
    {
        public Func<string,string> AdditionalSqlTitleConverterFunc { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return InternalExecuteSqlStatementExpression.StaticToString(GetType(), SqlScript, AdditionalSqlTitleConverterFunc);
        }
    }
}