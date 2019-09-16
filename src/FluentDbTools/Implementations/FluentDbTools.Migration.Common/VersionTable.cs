using System.Runtime.CompilerServices;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Migration.Abstractions;
using FluentMigrator.Runner.Conventions;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.Options;
#pragma warning disable 618
[assembly:InternalsVisibleTo("Example.FluentDbTools.Migration")]
namespace FluentDbTools.Migration.Common
{
    /// <summary>
    /// Default implementation of VersionTable
    /// </summary>
    internal class VersionTableMetaData : DefaultVersionTableMetaData, IFluentDbToolsVersionTableMetaData
    {
        /// <summary>
        /// The migration config
        /// </summary>
        protected readonly IDbMigrationConfig DbMigrationConfig;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbMigrationConfig"></param>
        public VersionTableMetaData(IDbMigrationConfig dbMigrationConfig)
        {
            DbMigrationConfig = dbMigrationConfig;
        }
        /// <summary>
        /// Then name of UniqueIndexName <br/>
        /// Default if no schemaPrefix will be UC_Version.<br/>
        /// With schemaPrefix specified, the UniqueIndexName will be {schemaPrefix}UC_Version<br/>
        /// <br/>
        /// i.e:<br/>
        /// When schemaPrefix is null, the UniqueIndexName will be UC_Version<br/>
        /// When schemaPrefix = "EX", the UniqueIndexName will be EXUC_Version<br/>
        /// </summary>
        public override string UniqueIndexName => base.UniqueIndexName.GetPrefixedName(DbMigrationConfig.GetSchemaPrefixId());
        
        /// <summary>
        /// Then name of Version TableName <br/>
        /// Default if no schemaPrefix will be VersionInfo.<br/>
        /// With schemaPrefix specified, the UniqueIndexName will be {schemaPrefix}VersionInfo<br/>
        /// <br/>
        /// i.e:<br/>
        /// When schemaPrefix is null, the UniqueIndexName will be VersionInfo<br/>
        /// When schemaPrefix = "EX", the UniqueIndexName will be EXVersionInfo<br/>
        /// </summary>
        public override string TableName => base.TableName.GetPrefixedName(DbMigrationConfig.GetSchemaPrefixId());

        /// <summary>
        /// SchemaPrefix will be fetched from <see cref="DbMigrationConfig"/>.Schema
        /// </summary>
        public override string SchemaName => DbMigrationConfig.Schema;

    }
}