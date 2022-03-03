using System;
using System.Data;
using System.Linq;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.SqlBuilder.Abstractions.Parameters;
using FluentDbTools.SqlBuilder.Common;
// ReSharper disable NotAccessedField.Local
#pragma warning disable CS1591

namespace FluentDbTools.SqlBuilder.Parameters
{
    public class DatabaseParameterResolver : IDatabaseParameterResolver
    {
        private readonly IDbConfigSchemaTargets DbConfigConfig;
        private readonly IDatabaseParameterHelper DatabaseParameterHelperField;

        public DatabaseParameterResolver(
            IDbConfigSchemaTargets dbConfigConfig)
        {
            DbConfigConfig = dbConfigConfig;
            DatabaseParameterHelperField = new DatabaseParameterHelper(dbConfigConfig);
        }

        public DatabaseParameterResolver(
            string schema,
            string schemaPrefixId,
            SupportedDatabaseTypes dbType = SupportedDatabaseTypes.Oracle)
            : this(new SqlBuilderDbConfigSchemaTargets(schema, schemaPrefixId, dbType))
        {
        }

        public SupportedDatabaseTypes DatabaseType => DatabaseParameterHelperField?.DatabaseType ?? SupportedDatabaseTypes.Postgres;
        public IDbTypeTranslator DbTypeTranslator => DatabaseParameterHelperField?.DbTypeTranslator;

        public IDataParameterCollectionExt CreateParameters<TDynamicParameters>() where TDynamicParameters : new()
        {
            return new DataParameterCollectionExt<TDynamicParameters>(this);
        }

        public IDbDataParameter CreateParameter(IDbConnection dbConnection)
        {
            return CreateCommand(dbConnection).CreateParameter();
        }

        public IDbCommand CreateCommand(IDbConnection dbConnection, string sql = null)
        {
            return dbConnection.CreateCommand(sql);
        }

        public TDynamicParameters ToDynamicParameters<TDynamicParameters>(IDataParameterCollection parameterCollection) where TDynamicParameters : new()
        {
            var dynamic = new TDynamicParameters();
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

        public int GetNextSequenceValue(IDbConnection dbConnection, string sequenceName = null)
        {
            var sequence = DatabaseParameterHelperField.WithNextSequence(sequenceName);
            var sql = DatabaseParameterHelperField.DatabaseType == SupportedDatabaseTypes.Oracle
                ? $"select {sequence} from dual"
                : $"select {sequence}";
            var result = dbConnection.CreateCommand(sql).ExecuteScalar();
            if (result != null && result is int intResult)
            {
                return intResult;
            }

            return default(int);
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