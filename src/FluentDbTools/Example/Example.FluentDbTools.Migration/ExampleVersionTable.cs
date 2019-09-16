using System.Runtime.CompilerServices;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Migration.Abstractions;
using FluentDbTools.Migration.Common;

#pragma warning disable 618
[assembly:InternalsVisibleTo("Test.FluentDbTools.Migration")]
namespace Example.FluentDbTools.Migration
{
    /// <inheritdoc />
    internal class ExampleVersionTableMetaData : VersionTableMetaData
    {
        /// <inheritdoc />
        public ExampleVersionTableMetaData(IDbMigrationConfig dbMigrationConfig) 
            : base(dbMigrationConfig)
        {
        }

        /// <inheritdoc />
        public override string TableName => nameof(ExampleVersionTableMetaData).GetPrefixedName(DbMigrationConfig.GetSchemaPrefixId());
    }
}