using System.Collections.Generic;
using FluentDbTools.Common.Abstractions;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner.VersionTableInfo;

namespace FluentDbTools.Migration.Abstractions
{
    /// <summary>
    /// Extended props and methods to <see cref="FluentMigrator.Migration"/>
    /// </summary>
    public interface IMigrationModel
    {
        /// <summary>
        /// SchemaName extracted from <see cref="IVersionTableMetaData.SchemaName"/>
        /// </summary>
        string SchemaName { get; }

        /// <summary>
        /// SchemaPrefixId - Fetch value from configuration "database:migration:schemaprefix:id" or "database:database:schemaprefix:id"
        /// </summary>
        string SchemaPrefixId { get; }

        /// <summary>
        /// SchemaPrefixUniqueId - Fetch value from configuration "database:migration:schemaprefix:uniqueid" or "database:database:schemaprefix:uniqueid"
        /// </summary>
        string SchemaPrefixUniqueId { get; }


        /// <summary>
        /// Return the injected MigrationContext or it will be resolved by reflection
        /// </summary>
        /// <returns></returns>
        IMigrationContext GetMigrationContext();

        /// <summary>
        /// return DbbMigrationConfig from <see cref="IMigrationContext.ServiceProvider"/>
        /// </summary>
        /// <returns></returns>
        IDbMigrationConfig GetMigrationConfig();

        /// <summary>
        /// return MigrationExpressions from <see cref="IMigrationContext.Expressions"/>
        /// </summary>
        /// <returns></returns>
        IList<IMigrationExpression> GetExpressions();

        /// <summary>
        /// Gets the starting point for creating or replacing View, Package or Scripts
        /// </summary>
        ICreateOrReplaceExpressionRoot CreateOrReplace { get; }

        /// <summary>
        /// return SchemaPrefix == null ? name : $"{SchemaPrefix}{name}";<br/>
        /// Calls the <see cref="StringExtensions.GetPrefixedName"/> method
        /// </summary>
        /// <param name="name">
        /// name of the object to add prefix to.<br/>
        /// If name start with the value of {<see cref="SchemaPrefixId"/>}, the value of <paramref name="name"/> will be returned
        /// </param>
        /// <returns></returns>
        string GetPrefixedName(string name);

        /// <summary>
        /// Reset fields 
        /// </summary>
        /// <param name="context"></param>
        IMigrationContext Reset(IMigrationContext context);
    }
}
