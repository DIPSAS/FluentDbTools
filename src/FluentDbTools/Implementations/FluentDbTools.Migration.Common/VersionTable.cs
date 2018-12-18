using FluentDbTools.Common.Abstractions;
using FluentMigrator.Runner.Conventions;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.Options;

namespace FluentDbTools.Migration.Common
{
    internal class VersionTable : DefaultVersionTableMetaData
    {
        private readonly IDbConfig DbConfig;

        public VersionTable(IDbConfig dbConfig)
        {
            DbConfig = dbConfig;
        }
        
        public override string SchemaName => DbConfig.Schema;
    }
}