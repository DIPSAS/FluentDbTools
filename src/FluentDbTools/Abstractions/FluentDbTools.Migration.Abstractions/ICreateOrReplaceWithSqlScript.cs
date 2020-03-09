using System.Diagnostics.CodeAnalysis;
using FluentMigrator.Infrastructure;

namespace FluentDbTools.Migration.Abstractions
{
    /// <summary>
    /// Interface to implement for Sql-Script support 
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public interface ICreateOrReplaceWithSqlScript : IFluentSyntax
    {
        /// <summary>
        /// The <paramref name="schemaName"/> script should be executed on
        /// </summary>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        /// 
        ICreateOrReplaceViewWithColumnSyntax InSchema(string schemaName);
        /// <summary>
        /// The <paramref name="scriptName"/> script to rune
        /// </summary>
        /// <param name="scriptName"></param>
        /// <returns></returns>
        ICreateOrReplaceViewWithColumnSyntax WithSqlScript(string scriptName);
    }
}