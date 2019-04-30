using FluentDbTools.Migration.Abstractions;
using FluentMigrator.Runner.VersionTableInfo;

#pragma warning disable 618

namespace Example.FluentDbTools.Migration
{
    public class ExampleVersionTable : DefaultVersionTableMetaData
    {
        private readonly IDbMigrationConfig DbMigrationConfig;
        
        public ExampleVersionTable(IDbMigrationConfig dbMigrationConfig) 
        {
            DbMigrationConfig = dbMigrationConfig;
        }

        public override string UniqueIndexName => "UC_" + TableName;
        public override string TableName => nameof(ExampleVersionTable);
        public override string SchemaName => DbMigrationConfig.Schema;
    }
}