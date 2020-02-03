using System.Runtime.CompilerServices;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.SqlBuilder.Abstractions;
using FluentDbTools.SqlBuilder.Common;

[assembly: InternalsVisibleTo("Test.FluentDbTools.SqlBuilder")]
namespace FluentDbTools.SqlBuilder
{
    public class SqlBuilder : ISqlBuilder
    {
        private readonly IDbConfigSchemaTargets DbConfigConfig;


        public SupportedDatabaseTypes DatabaseType => DbConfigConfig.DbType;

        public SqlBuilder(IDbConfigSchemaTargets dbConfigConfig)
        {
            DbConfigConfig = dbConfigConfig;
        }

        public SqlBuilder(
            string schema,
            string schemaPrefixId,
            SupportedDatabaseTypes dbType = SupportedDatabaseTypes.Oracle)
            : this(new SqlBuilderDbConfigSchemaTargets(schema, schemaPrefixId, dbType))
        {
        }

        public ISelectSqlBuilder Select()
        {
            return new SelectSqlBuilder(DbConfigConfig);
        }

        public IDeleteSqlBuilder<TClass> Delete<TClass>(string tableName = null)
        {
            return new DeleteSqlBuilder<TClass>(DbConfigConfig, tableName);
        }

        public IUpdateSqlBuilder<TClass> Update<TClass>(string tableName = null)
        {
            return new UpdateSqlBuilder<TClass>(DbConfigConfig, tableName);
        }

        public IInsertSqlBuilder<TClass> Insert<TClass>(string tableName = null)
        {
            return new InsertSqlBuilder<TClass>(DbConfigConfig, tableName);
        }
    }
}

