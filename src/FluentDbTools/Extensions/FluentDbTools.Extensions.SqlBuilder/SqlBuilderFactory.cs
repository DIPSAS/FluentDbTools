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
        public static ISqlBuilder SqlBuilder(this IDbConfigSchemaTargets dbConfigConfig)
        {
            return dbConfigConfig.CreateSqlBuilder();
        }

        /// <summary>
        /// Create a <see cref="ISqlBuilder"/> instance by parameters
        /// </summary>
        /// <param name="schema"><inheritdoc cref="IDbConfigSchemaTargets.Schema"/></param>
        /// <param name="schemaPrefixId"><inheritdoc cref="IDbConfigSchemaTargets.GetSchemaPrefixId"/></param>
        /// <param name="dbType"><inheritdoc cref="IDbConfigSchemaTargets.DbType"/></param>
        /// <returns></returns>
        public static ISqlBuilder SqlBuilder(
            string schema,
            string schemaPrefixId,
            SupportedDatabaseTypes dbType = SupportedDatabaseTypes.Oracle)
        {
            return FluentDbTools.SqlBuilder.SqlBuilderFactory.CreateSqlBuilder(schema, schemaPrefixId, dbType);
        }

        /// <summary>
        /// Create a <see cref="IDatabaseParameterResolver"/> instance. <br/>
        /// See <see cref="IDbConfigSchemaTargets"/> for parameter details<br/>
        /// <inheritdoc cref="IDbConfigSchemaTargets"/>
        /// </summary>
        /// <param name="dbConfigConfig"></param>
        /// <returns></returns>
        public static IDatabaseParameterResolver DatabaseParameterResolver(this IDbConfigSchemaTargets dbConfigConfig)
        {
            return dbConfigConfig.CreateParameterResolver();
        }


        /// <summary>
        /// Create a <see cref="IDatabaseParameterResolver"/> instance by parameters
        /// </summary>
        /// <param name="schema"><inheritdoc cref="IDbConfigSchemaTargets.Schema"/></param>
        /// <param name="schemaPrefixId"><inheritdoc cref="IDbConfigSchemaTargets.GetSchemaPrefixId"/></param>
        /// <param name="dbType"><inheritdoc cref="IDbConfigSchemaTargets.DbType"/></param>
        /// <returns></returns>
        public static IDatabaseParameterResolver DatabaseParameterResolver(
            string schema, 
            string schemaPrefixId,
            SupportedDatabaseTypes dbType = SupportedDatabaseTypes.Oracle)
        {
            return FluentDbTools.SqlBuilder.SqlBuilderFactory.CreateParameterResolver(schema, schemaPrefixId, dbType);
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