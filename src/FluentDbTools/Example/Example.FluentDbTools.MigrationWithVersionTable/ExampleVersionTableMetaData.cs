using FluentDbTools.Migration.Abstractions;
using FluentMigrator.Runner.VersionTableInfo;

#pragma warning disable 618

namespace Example.FluentDbTools.MigrationWithVersionTable
{
    public class ExampleVersionTableMetaData : DefaultVersionTableMetaData
    {
        private readonly IDbMigrationConfig DbMigrationConfig;

        public ExampleVersionTableMetaData(IDbMigrationConfig dbMigrationConfig) 
        {
            DbMigrationConfig = dbMigrationConfig;
        }

        public override string UniqueIndexName => "UC_" + nameof(ExampleVersionTable);
        public override string TableName => nameof(ExampleVersionTable);
        public override string SchemaName => DbMigrationConfig.Schema;
    }
}