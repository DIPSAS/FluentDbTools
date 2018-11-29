using FluentMigrator.Expressions;
using FluentMigrator.Runner;
using FluentMigrator.Runner.VersionTableInfo;

namespace DIPS.FluentDbTools.Migration
{
    public static class FluentMigrationExtensions
    {
        public static void DropData(this IMigrationRunner migrationRunner, IVersionTableMetaData versionTableMetaData)
        {
            var schemaName = versionTableMetaData.SchemaName;
            migrationRunner.Processor
                .Process(new DeleteSchemaExpression { SchemaName = schemaName });
        }
    }
}