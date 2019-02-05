using System;
using System.Data.Common;

namespace FluentDbTools.Extensions.DbProvider
{
    internal class FluentDbProviderFactory : DbProviderFactory
    {
        private readonly DbProviderFactory DbProviderFactory;
        private readonly string ConnectionString;

        public FluentDbProviderFactory(
            DbProviderFactory dbProviderDbProviderFactory, 
            string connectionString)
        {
            DbProviderFactory = dbProviderDbProviderFactory;
            ConnectionString = connectionString;
        }
        
        public override DbCommand CreateCommand()
        {
            var command = DbProviderFactory.CreateCommand();
            AssertNullReference(command);
            command.Connection.ConnectionString = ConnectionString;
            return command;
        }

        public override DbCommandBuilder CreateCommandBuilder()
        {
            return DbProviderFactory.CreateCommandBuilder();
        }

        public override DbConnection CreateConnection()
        {
            var connection = DbProviderFactory.CreateConnection();
            AssertNullReference(connection);
            connection.ConnectionString = ConnectionString;
            return connection;
        }

        public override DbConnectionStringBuilder CreateConnectionStringBuilder()
        {
            var connectionBuilder = DbProviderFactory.CreateConnectionStringBuilder();
            AssertNullReference(connectionBuilder);
            connectionBuilder.ConnectionString = ConnectionString;
            return connectionBuilder;
        }

        public override DbDataAdapter CreateDataAdapter()
        {
            return DbProviderFactory.CreateDataAdapter();
        }

        public override DbDataSourceEnumerator CreateDataSourceEnumerator()
        {
            return DbProviderFactory.CreateDataSourceEnumerator();
        }

        public override DbParameter CreateParameter()
        {
            return DbProviderFactory.CreateParameter();
        }

        public override bool CanCreateDataSourceEnumerator => DbProviderFactory.CanCreateDataSourceEnumerator;

        private static void AssertNullReference<T>(T @object)
        {
            if (@object == null)
            {
                throw new NullReferenceException($"{typeof(T).Name} cannot be null!");
            }
        }
    }
}