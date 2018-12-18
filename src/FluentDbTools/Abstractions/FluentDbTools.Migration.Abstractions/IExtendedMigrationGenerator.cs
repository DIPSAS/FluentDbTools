using FluentMigrator.Expressions;

namespace FluentDbTools.Migration.Abstractions
{
    public interface IExtendedMigrationGenerator
    {
        string Generate(DeleteSchemaExpression expression);
        string Generate(DeleteUserExpression expression);
        string Generate(CreateUserExpression expression);
        string Generate(CreateSchemaExpression expression);
        string GenerateUserExistsSql(string user);
        string GenerateSchemaExistsSql(string schemaName);
        string GenerateTableSpaceExistsSql(TableSpaceType tableSpaceType);
        string GenerateCreateTableSpaceSql(TableSpaceType tableSpaceType);
        string GetTableSpaceName(TableSpaceType tableSpaceType);
        string Generate(CreateIndexExpression expression);
        string Generate(CreateConstraintExpression expression);
        string GenerateDefaultPrivilegesSql(string schemaName);
        string GenerateSequenceExistsSql(string schemaName, string sequenceName);
    }

    public interface IExtendedMigrationGenerator<T> : IExtendedMigrationGenerator
    {
    }
}