using System.Diagnostics.CodeAnalysis;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.SqlBuilder;
using FluentDbTools.SqlBuilder.Abstractions;
using FluentDbTools.SqlBuilder.Abstractions.Parameters;

// FluentDbTools.Extensions.SqlBuilder
namespace FluentDbTools.Extensions.SqlBuilder
{
    /// <summary>
    /// Static Factory class for FluentDbTools.SqlBuilder project<br/>
    /// -> THIS static constructor, Will call <see cref="RegisterDapperTypeHandlers"/> to register Dapper TypeHandlers<br/>
    /// <br/>
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class SqlBuilderFactory
    {
        static SqlBuilderFactory()
        {
            RegisterDapperTypeHandlers();
        }

        /// <summary>
        /// Will register Dapper TypeHandlers
        /// </summary>
        public static void RegisterDapperTypeHandlers()
        {
            TypeHandlerRegistration.RegisterTypeHandlers();
        }


        /// <summary>
        /// Create a <see cref="ISqlBuilder"/> instance.<br/>
        /// See <see cref="IDbConfigSchemaTargets"/> for parameter details<br/>
        /// <inheritdoc cref="IDbConfigSchemaTargets"/>
        /// </summary>
        /// <param name="dbConfigConfig">See <see cref="IDbConfigSchemaTargets.Schema"/></param>
        /// <returns></returns>
        public static ISqlBuilder CreateSqlBuilder(this IDbConfigSchemaTargets dbConfigConfig)
        {
            return dbConfigConfig.SqlBuilder();
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
            return FluentDbTools.SqlBuilder.SqlBuilderFactory.SqlBuilder(schema, schemaPrefixId, dbType);
        }

        /// <summary>
        /// Create a <see cref="IDatabaseParameterResolver"/> instance. <br/>
        /// See <see cref="IDbConfigSchemaTargets"/> for parameter details<br/>
        /// <inheritdoc cref="IDbConfigSchemaTargets"/>
        /// </summary>
        /// <param name="dbConfigConfig"></param>
        /// <returns></returns>
        public static IDatabaseParameterResolver CreateDatabaseParameterResolver(this IDbConfigSchemaTargets dbConfigConfig)
        {
            return dbConfigConfig.DatabaseParameterResolver();
        }

        /// <summary>
        /// Create a <see cref="IDatabaseParameterResolver"/> instance by parameters
        /// </summary>
        /// <param name="schema"><inheritdoc cref="IDbConfigSchemaTargets.Schema"/></param>
        /// <param name="schemaPrefixId"><inheritdoc cref="IDbConfigSchemaTargets.GetSchemaPrefixId"/></param>
        /// <param name="dbType"><inheritdoc cref="IDbConfigSchemaTargets.DbType"/></param>
        /// <returns></returns>
        public static IDatabaseParameterResolver CreateDatabaseParameterResolver(
            string schema, 
            string schemaPrefixId,
            SupportedDatabaseTypes dbType = SupportedDatabaseTypes.Oracle)
        {
            return FluentDbTools.SqlBuilder.SqlBuilderFactory.DatabaseParameterResolver(schema, schemaPrefixId, dbType);
        }
    }

}

namespace FluentDbTools.Extensions.SqlBuilder
{
    internal class _
    {
        static _()
        {
            SqlBuilderFactory.RegisterDapperTypeHandlers();
        }
    }
}