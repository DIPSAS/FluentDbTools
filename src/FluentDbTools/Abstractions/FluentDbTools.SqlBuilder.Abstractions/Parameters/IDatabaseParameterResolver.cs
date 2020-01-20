using System.Data;

namespace FluentDbTools.SqlBuilder.Abstractions.Parameters
{
    public interface IDatabaseParameterResolver : IDatabaseParameterHelper
    {
        IDataParameterCollectionExt CreateParameters<TDynamicParameters>() where TDynamicParameters : new();
        IDbDataParameter CreateParameter(IDbConnection dbConnection);

        IDbCommand CreateCommand(IDbConnection dbConnection, string sql = null);

        TDynamicParameters ToDynamicParameters<TDynamicParameters>(IDataParameterCollection parameterCollection) where TDynamicParameters : new();

        int AddParameter(IDbConnection dbConnection, IDataParameterCollection parameterCollection, string parameterName, object value,
            DbType? dbType = null, ParameterDirection? direction = null, int? size = null, byte? precision = null,
            byte? scale = null);
        int AddParameterValue(IDbConnection dbConnection, IDataParameterCollection parameterCollection, string parameterName, object value);

        int GetNextSequenceValue(IDbConnection dbConnection, string sequenceName = null);
        int GetNextSequenceValueForTable(IDbConnection dbConnection, string tableName = null);
        int GetNextSequenceValueForTable<T>(IDbConnection dbConnection);
    }
}