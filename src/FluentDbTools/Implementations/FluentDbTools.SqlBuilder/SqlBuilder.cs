using System.Runtime.CompilerServices;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.SqlBuilder.Abstractions;

[assembly: InternalsVisibleTo("Test.FluentDbTools.SqlBuilder")]
namespace FluentDbTools.SqlBuilder
{
    public class SqlBuilder : ISqlBuilder
    {
        private readonly IDbConfig DbConfig;

       
        public SupportedDatabaseTypes DatabaseType => DbConfig.DbType;

        public SqlBuilder(IDbConfig dbConfig)
        {
            DbConfig = dbConfig;
        }
        
        public ISelectSqlBuilder Select()
        {
            return new SelectSqlBuilder(DbConfig);
        }
        
        public IDeleteSqlBuilder<TClass> Delete<TClass>()
        {
            return new DeleteSqlBuilder<TClass>(DbConfig);
        }

        public IUpdateSqlBuilder<TClass> Update<TClass>()
        {
            return new UpdateSqlBuilder<TClass>(DbConfig);
        }

        public IInsertSqlBuilder<TClass> Insert<TClass>()
        {
            return new InsertSqlBuilder<TClass>(DbConfig);
        }
    }
}