using System.Diagnostics.CodeAnalysis;
using FluentMigrator.Expressions;

namespace FluentDbTools.Migration.Abstractions
{
    /// <summary>
    /// Extended Database sql-statement generator
    /// </summary>
    public interface IExtendedMigrationGenerator
    {
        /// <summary>
        /// Generate a Delete Schema sql-statement
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        string Generate(DeleteSchemaExpression expression);

        /// <summary>
        /// Generate a Delete User sql-statement
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        string Generate(DeleteUserExpression expression);

        /// <summary>
        /// Generate a Create User sql-statement
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        string Generate(CreateUserExpression expression);

        /// <summary>
        /// Generate a Delete Schema sql-statement
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        string Generate(CreateSchemaExpression expression);

        /// <summary>
        /// Generate a User-Exists sql-statement
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        string GenerateUserExistsSql(string user);

        /// <summary>
        /// Generate a Schema-Exists sql-statement
        /// </summary>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        string GenerateSchemaExistsSql(string schemaName);

        /// <summary>
        /// Generate a Table-Exists sql-statement
        /// </summary>
        /// <param name="tableSpaceType"></param>
        /// <returns></returns>
        string GenerateTableSpaceExistsSql(TableSpaceType tableSpaceType);

        /// <summary>
        /// Generate a Create Tablespace sql-statement
        /// </summary>
        /// <param name="tableSpaceType"></param>
        /// <returns></returns>
        string GenerateCreateTableSpaceSql(TableSpaceType tableSpaceType);

        /// <summary>
        /// Generate a Get Tablespace sql-statement
        /// </summary>
        /// <param name="tableSpaceType"></param>
        /// <returns></returns>
        string GetTableSpaceName(TableSpaceType tableSpaceType);

        /// <summary>
        /// Generate a Create Index sql-statement
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        string Generate(CreateIndexExpression expression);

        /// <summary>
        /// Generate a Create Constraint sql-statement
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        string Generate(CreateConstraintExpression expression);

        /// <summary>
        /// Generate a Create Default Privileges sql-statement
        /// </summary>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        string GenerateDefaultPrivilegesSql(string schemaName);

        /// <summary>
        /// Generate a Sequence-Exists sql-statement
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="sequenceName"></param>
        /// <returns></returns>
        string GenerateSequenceExistsSql(string schemaName, string sequenceName);
    }

    /// <summary>
    /// Represent <see cref="IExtendedMigrationGenerator"/> of <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SuppressMessage("ReSharper", "UnusedTypeParameter")]
    public interface IExtendedMigrationGenerator<T> : IExtendedMigrationGenerator
    {
    }
}