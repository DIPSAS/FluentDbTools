using System.Diagnostics.CodeAnalysis;

namespace FluentDbTools.Migration.Abstractions
{
    /// <summary>
    /// Interface for Views support 
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public interface ICreateOrReplaceViewWithColumnSyntax : ICreateOrReplaceWithSqlScript
    {
        /// <summary>
        /// Create or Replace View with column <paramref name="name"/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ICreateOrReplaceViewWithColumnSyntax WithColumn(string name);

        /// <summary>
        /// Create or Replace View with columns <paramref name="names"/>
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        ICreateOrReplaceViewWithColumnSyntax WithColumns(params string[] names);
    }
}