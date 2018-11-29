using System.Data;
using DIPS.Extensions.FluentDbTools.DbProvider;
using DIPS.FluentDbTools.Common.Abstractions;

namespace DIPS.FluentDbTools.Example.Database
{
    public class DbProvider : IDbProvider
    {
        private readonly IDbConfig DbConfig;

        private IDbConnection DbConnectionField;
        private IDbTransaction DbTransactionField;

        public DbProvider(IDbConfig dbConfig)
        {
            DbConfig = dbConfig;
        }

        public IDbConnection DbConnection => DbConnectionField = DbConnectionField ?? CreateAndOpenDbConnection();
        public IDbTransaction DbTransaction => DbTransactionField = DbTransactionField ?? DbConnection.BeginTransaction();

        private IDbConnection CreateAndOpenDbConnection()
        {
            var dbConnection = DbConfig.CreateDbConnection();
            dbConnection.Open();
            return dbConnection;
        }

        public void Dispose()
        {
            DbConnectionField?.Dispose();
            DbTransactionField?.Dispose();
        }
    }
}