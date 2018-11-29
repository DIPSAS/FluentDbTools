using System.Collections.Generic;
using System.Data;
using Dapper;

namespace DIPS.FluentDbTools.SqlBuilder.Abstractions.Parameters
{
    public interface IDatabaseParameterResolver : IDatabaseParameterHelper
    {
        IDataParameterCollectionExt CreateParameters();
        IDbDataParameter CreateParameter(IDbConnection dbConnection);

        IDbCommand CreateCommand(IDbConnection dbConnection, string sql = null);

        object ToDynamicParameters(IDataParameterCollection parameterCollection);

        int AddParameter(IDbConnection dbConnection, IDataParameterCollection parameterCollection, string parameterName, object value,
            DbType? dbType = null, ParameterDirection? direction = null, int? size = null, byte? precision = null,
            byte? scale = null);
        int AddParameterValue(IDbConnection dbConnection, IDataParameterCollection parameterCollection, string parameterName, object value);

        string[] AddArrayParameter<T>(DynamicParameters @params, string paramName, IEnumerable<T> enumerable);
        string[] AddArrayParameter<T>(DynamicParameters @params, IEnumerable<T> enumerable);

        int GetNextSequenceValue(IDbConnection dbConnection, string sequenceName = null);
        int GetNextSequenceValueForTable(IDbConnection dbConnection, string tableName = null);
        int GetNextSequenceValueForTable<T>(IDbConnection dbConnection);
    }
}