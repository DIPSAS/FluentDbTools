using System.Data;
using FluentDbTools.Extensions.DbProvider;
using FluentDbTools.Common.Abstractions;

namespace Example.FluentDbTools.Database
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