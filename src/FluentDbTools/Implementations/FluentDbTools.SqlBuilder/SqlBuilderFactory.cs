using System.Diagnostics.CodeAnalysis;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.SqlBuilder.Abstractions;
using FluentDbTools.SqlBuilder.Abstractions.Parameters;
using FluentDbTools.SqlBuilder.Common;
using FluentDbTools.SqlBuilder.Parameters;

// FluentDbTools.SqlBuilder Assembly
namespace FluentDbTools.SqlBuilder
{
    /// <summary>
    /// Partial Static Factory class for FluentDbTools.SqlBuilder project<br/>
    /// -> THIS static constructor is empty<br/>
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "PartialTypeWithSinglePart")]
    public static class SqlBuilderFactory
    {
        /// <summary>
        /// Create a <see cref="ISqlBuilder"/> instance.<br/>
        /// See <see cref="IDbConfigSchemaTargets"/> for parameter details<br/>
        /// <inheritdoc cref="IDbConfigSchemaTargets"/>
        /// </summary>
        /// <param name="dbConfigConfig">See <see cref="IDbConfigSchemaTargets.Schema"/></param>
        /// <returns></returns>
        public static ISqlBuilder CreateSqlBuilder(this IDbConfigSchemaTargets dbConfigConfig)
        {
            return new SqlBuilder(dbConfigConfig);
        }

        /// <summary>
        /// Create a <see cref="ISqlBuilder"/> instance by parameters
        /// </summary>
        /// <param name="schema"><inheritdoc cref="IDbConfigSchemaTargets.Schema"/></param>
        /// <param name="schemaPrefixId"><inheritdoc cref="IDbConfigSchemaTargets.GetSchemaPrefixId"/></param>
        /// <param name="dbType"><inheritdoc cref="IDbConfigSchemaTargets.DbType"/></param>
        /// <returns></returns>
        public static ISqlBuilder CreateSqlBuilder(
            string schema,
            string schemaPrefixId,
            SupportedDatabaseTypes dbType = SupportedDatabaseTypes.Oracle)
        {
            return new SqlBuilder(schema, schemaPrefixId, dbType);
        }

        /// <summary>
        /// Create a <see cref="IDatabaseParameterResolver"/> instance. <br/>
        /// See <see cref="IDbConfigSchemaTargets"/> for parameter details<br/>
        /// <inheritdoc cref="IDbConfigSchemaTargets"/>
        /// </summary>
        /// <param name="dbConfigConfig"></param>
        /// <returns></returns>
        public static IDatabaseParameterResolver CreateParameterResolver(this IDbConfigSchemaTargets dbConfigConfig)
        {
            return new DatabaseParameterResolver(dbConfigConfig);
        }


        /// <summary>
        /// Create a <see cref="IDatabaseParameterResolver"/> instance by parameters
        /// </summary>
        /// <param name="schema"><inheritdoc cref="IDbConfigSchemaTargets.Schema"/></param>
        /// <param name="schemaPrefixId"><inheritdoc cref="IDbConfigSchemaTargets.GetSchemaPrefixId"/></param>
        /// <param name="dbType"><inheritdoc cref="IDbConfigSchemaTargets.DbType"/></param>
        /// <returns></returns>
        public static IDatabaseParameterResolver CreateParameterResolver(
            string schema, 
            string schemaPrefixId,
            SupportedDatabaseTypes dbType = SupportedDatabaseTypes.Oracle)
        {
            return new DatabaseParameterResolver(schema, schemaPrefixId, dbType);
        }

        /// <summary>
        /// Create a instance of Database Schema targets<br/>
        /// Interface:<br/>
        /// • DbType: <see cref="SupportedDatabaseTypes"/> ──── <remarks>Current Database type</remarks> <br/>
        /// • Schema: <see cref="string"/> ──── <remarks>Used to specify Schema for connected database</remarks> <br/>
        /// • GetSchemaPrefixId(): Return <returns><see cref="string"/></returns>  ──── <remarks>Can be used to specifying a short Prefix for the Schema.</remarks> <br/>
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="schemaPrefixId"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static IDbConfigSchemaTargets CreateDbConfigSchemaTargets(
            string schema,
            string schemaPrefixId,
            SupportedDatabaseTypes dbType = SupportedDatabaseTypes.Oracle)
        {
            return new SqlBuilderDbConfigSchemaTargets(schema,schemaPrefixId,dbType);
        }

    }
}