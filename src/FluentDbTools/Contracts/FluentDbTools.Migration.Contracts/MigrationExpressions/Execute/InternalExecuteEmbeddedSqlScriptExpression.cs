using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentMigrator;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using JetBrains.Annotations;
// ReSharper disable InconsistentNaming
#pragma warning disable CS0169

namespace FluentDbTools.Migration.Contracts.MigrationExpressions.Execute
{
    /// <summary>
    /// Expression to execute an embedded SQL script
    /// </summary>
    internal class InternalExecuteEmbeddedSqlScriptExpression : ExecuteEmbeddedSqlScriptExpressionBase
    {
        public Func<string,string> AdditionalSqlTitleConverterFunc { get; set; }
        
        private readonly ExecuteEmbeddedSqlScriptExpression ExecuteEmbeddedSqlScriptExpression;
        
        /// <summary>
        /// Initializes a new instance of InternalExecuteEmbeddedSqlScriptExpression class
        /// </summary>
        [Obsolete]
        public InternalExecuteEmbeddedSqlScriptExpression()
        {
            ExecuteEmbeddedSqlScriptExpression = new ExecuteEmbeddedSqlScriptExpression();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteEmbeddedSqlScriptExpression"/> class.
        /// </summary>
        /// <param name="embeddedResourceProviders">The embedded resource providers</param>
        public InternalExecuteEmbeddedSqlScriptExpression([NotNull] IEnumerable<IEmbeddedResourceProvider> embeddedResourceProviders)
        {
            ExecuteEmbeddedSqlScriptExpression = new ExecuteEmbeddedSqlScriptExpression(embeddedResourceProviders);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteEmbeddedSqlScriptExpression"/> class.
        /// </summary>
        /// <param name="assemblyCollection">The collection of assemblies to be searched for the resources</param>
        [Obsolete]
        public InternalExecuteEmbeddedSqlScriptExpression([NotNull] IAssemblyCollection assemblyCollection)
        {
            ExecuteEmbeddedSqlScriptExpression = new ExecuteEmbeddedSqlScriptExpression(assemblyCollection);
        }

        /// <summary>
        /// Gets or sets the SQL script name
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(ErrorMessages),
            ErrorMessageResourceName = nameof(ErrorMessages.SqlScriptCannotBeNullOrEmpty))]
        public string SqlScript
        {
            get => ExecuteEmbeddedSqlScriptExpression.SqlScript;
            set => ExecuteEmbeddedSqlScriptExpression.SqlScript = value;
        }

        /// <summary>
        /// Gets or sets the migration assemblies
        /// </summary>
        [Obsolete()]
        [CanBeNull]
        public IAssemblyCollection MigrationAssemblies
        {
            get => ExecuteEmbeddedSqlScriptExpression.MigrationAssemblies;
            set => ExecuteEmbeddedSqlScriptExpression.MigrationAssemblies = value;
        }

        /// <inheritdoc />
        public override void ExecuteWith(IMigrationProcessor processor)
        {
            ExecuteEmbeddedSqlScriptExpression.ExecuteWith(processor);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return InternalExecuteSqlStatementExpression.StaticToString(GetType(), SqlScript, AdditionalSqlTitleConverterFunc);
        }
    }
}