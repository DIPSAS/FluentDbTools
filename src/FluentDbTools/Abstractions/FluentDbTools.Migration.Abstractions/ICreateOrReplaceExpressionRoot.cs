using FluentMigrator.Infrastructure;

namespace FluentDbTools.Migration.Abstractions
{
    /// <summary>
    /// ExpressionRoot for View, Package and Scripts
    /// </summary>
    public interface ICreateOrReplaceExpressionRoot : IFluentSyntax
    {
        /// <summary>
        /// Database View operations
        /// </summary>
        /// <param name="viewName">The name of the view</param>
        /// <returns></returns>
        ICreateOrReplaceViewWithColumnSyntax View(string viewName);

        /// <summary>
        /// Database Package operation
        /// </summary>
        /// <param name="packageName">The name of the package</param>
        /// <returns></returns>
        ICreateOrReplacePackageSyntax Package(string packageName);

        /// <summary>
        /// Database Script operation
        /// </summary>
        /// <param name="scriptPath">Then location for the script</param>
        /// <returns></returns>
        ICreateOrReplacePackageSyntax Script(string scriptPath);
    }
}