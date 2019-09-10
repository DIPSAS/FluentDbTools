using System;
using FluentDbTools.Migration.Abstractions;
using FluentMigrator.Infrastructure;

namespace FluentDbTools.Migration.Contracts
{
    /// <inheritdoc />
    public class CreateOrReplaceExpressionRoot : ICreateOrReplaceExpressionRoot
    {
        private readonly IMigrationContext _context;

        /// <inheritdoc />
        public CreateOrReplaceExpressionRoot(IMigrationContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public ICreateOrReplaceViewWithColumnSyntax View(string viewName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ICreateOrReplacePackageSyntax Package(string viewName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ICreateOrReplacePackageSyntax Script(string scriptPath)
        {
            throw new NotImplementedException();
        }
    }
}