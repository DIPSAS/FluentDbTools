using FluentDbTools.Common.Abstractions;
using FluentDbTools.Migration.Abstractions;
using FluentMigrator.Runner.Conventions;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.Options;
#pragma warning disable 618

namespace FluentDbTools.Migration.Common
{
    internal class VersionTable : DefaultVersionTableMetaData
    {
        private readonly IDbMigrationConfig DbConfig;

        public VersionTable(IDbMigrationConfig dbConfig)
        {
            DbConfig = dbConfig;
        }
        
        public override string SchemaName => DbConfig.Schema;
    }
}