using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.SqlBuilder.Abstractions.Parameters;
using FluentDbTools.SqlBuilder.Common;

namespace FluentDbTools.SqlBuilder.Parameters
{
    public class DatabaseParameterResolver : IDatabaseParameterResolver
    {
        private readonly IDbConfigDatabaseTargets DbConfig;
        private readonly IDatabaseParameterHelper DatabaseParameterHelperField;

        public DatabaseParameterResolver(
            IDbConfigDatabaseTargets dbConfig)
        {
            DbConfig = dbConfig;
            DatabaseParameterHelperField = new DatabaseParameterHelper(dbConfig);
        }

        public SupportedDatabaseTypes DatabaseType => DatabaseParameterHelperField?.DatabaseType ?? SupportedDatabaseTypes.Postgres;
        public IDbTypeTranslator DbTypeTranslator => DatabaseParameterHelperField?.DbTypeTranslator;

        public IDataParameterCollectionExt CreateParameters()
        {
            return new DataParameterCollectionExt(this);
        }

        public IDbDataParameter CreateParameter(IDbConnection dbConnection)
        {
            return CreateCommand(dbConnection).CreateParameter();
        }

        public IDbCommand CreateCommand(IDbConnection dbConnection, string sql = null)
        {
            return dbConnection.CreateCommand(sql);
        }

        public object ToDynamicParameters(IDataParameterCollection parameterCollection)
        {
            var dynamic = new DynamicParameters();
            foreach (var param in parameterCollection)
            {

                if (param is IDbDataParameter dbParam)
                {
                    dynamic.AddDbDataParameter(dbParam);
                }
                else
                {
                    dynamic.AddDynamicParams(param);
                }

            }

            return dynamic;
        }

        public int AddParameter(IDbConnection dbConnection, IDataParameterCollection parameterCollection, string parameterName, object value,
            DbType? dbType = null, ParameterDirection? direction = null, int? size = null, byte? precision = null,
            byte? scale = null)
        {

            var param = CreateParameter(dbConnection);
            param.ParameterName = parameterName;
            if (dbType != null) param.DbType = dbType.Value;
            if (direction != null) param.Direction = direction.Value;
            if (size != null) param.Size = size.Value;
            if (precision != null) param.Precision = precision.Value;
            if (scale != null) param.Scale = scale.Value;
            param.Value = value;
            return parameterCollection.Add(param);
        }

        public int AddParameterValue(IDbConnection dbConnection, IDataParameterCollection parameterCollection, string parameterName, object value)
        {
            var param = CreateParameter(dbConnection);
            param.ParameterName = parameterName;
            param.Value = value;
            return parameterCollection.Add(param);
        }

        public string[] AddArrayParameter<T>(DynamicParameters @params, string paramName, IEnumerable<T> enumerable)
        {
            return DatabaseParameterHelperField.AddArrayParameter(@params, paramName, enumerable);
        }

        public string[] AddArrayParameter<T>(DynamicParameters @params, IEnumerable<T> enumerable)
        {
            return AddArrayParameter(@params, typeof(T).Name, enumerable);
        }

        public int GetNextSequenceValue(IDbConnection dbConnection, string sequenceName = null)
        {
            var sequence = DatabaseParameterHelperField.WithNextSequence(sequenceName);
            var sql = DatabaseParameterHelperField.DatabaseType == SupportedDatabaseTypes.Oracle
                ? $"select {sequence} from dual"
                : $"select {sequence}";

            return dbConnection.ExecuteScalar<int>(sql);
        }

        public int GetNextSequenceValueForTable(IDbConnection dbConnection, string tableName = null)
        {
            return GetNextSequenceValue(dbConnection, $"{tableName}_seq".ToLower());
        }

        public int GetNextSequenceValueForTable<T>(IDbConnection dbConnection)
        {
            return GetNextSequenceValueForTable(dbConnection, $"{typeof(T).Name}");
        }

        public string WithParameters(params string[] parameters)
        {
            var prefix = GetParameterPrefix();

            parameters = parameters.Select(x => $"{prefix}{x.Replace(":", "").Replace("@", "")}").ToArray();

            return string.Join(", ", parameters);
        }

        public string WithNextSequence(string sequenceName = null)
        {
            return DatabaseParameterHelperField?.WithNextSequence(sequenceName);
        }

        public string WithNextTableSequence<T>(string postfix = "_seq")
        {
            return DatabaseParameterHelperField?.WithNextTableSequence<T>(postfix);
        }

        public string AsForeignKey<T>(string postfix = "Id")
        {
            return DatabaseParameterHelperField?.AsForeignKey<T>(postfix);
        }

        public object WithGuidParameterValue(Guid guid)
        {
            return DatabaseParameterHelperField?.WithGuidParameterValue(guid);
        }

        public object WithBooleanParameterValue(bool boolean)
        {
            return DatabaseParameterHelperField?.WithBooleanParameterValue(boolean);
        }

        public string GetParameterPrefix()
        {
            return SqlBuilderHelper.GetParameterPrefixIfNull(DatabaseParameterHelperField?.GetParameterPrefix());
        }
    }
}