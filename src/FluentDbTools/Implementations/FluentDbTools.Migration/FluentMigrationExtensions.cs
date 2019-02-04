using FluentMigrator.Expressions;
using FluentMigrator.Runner;
using FluentMigrator.Runner.VersionTableInfo;

namespace FluentDbTools.Migration
{
    public static class FluentMigrationExtensions
    {
        public static void DropSchema(this IMigrationRunner migrationRunner, IVersionTableMetaData versionTableMetaData)
        {
            var schemaName = versionTableMetaData.SchemaName;
            migrationRunner.Processor
                .Process(new DeleteSchemaExpression { SchemaName = schemaName });
        }
    }
}