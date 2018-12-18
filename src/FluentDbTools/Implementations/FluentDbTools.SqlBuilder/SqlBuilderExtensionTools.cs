using System;
using System.Collections;
using System.Data;
using System.Linq;
using Dapper;
using FluentDbTools.Common.Abstractions;

namespace FluentDbTools.SqlBuilder
{
    internal static class SqlBuilderExtensionTools
    {
        public static string GetParameterPrefix(this IDbConfig dbConfig)
        {
            return dbConfig.DbType == SupportedDatabaseTypes.Oracle ? ":" : "@";
        }

        public static string WithParameters(this IDbConfig dbConfig, params string[] parameters)
        {
            var prefix = dbConfig.GetParameterPrefix();

            parameters = parameters.Select(x => $"{prefix}{x.Replace(":", "").Replace("@", "")}").ToArray();

            return string.Join(", ", parameters);
        }


        public static IDbCommand CreateCommand(this IDbConnection dbConnection, string commandText)
        {
            var command = dbConnection.CreateCommand();
            command.Connection = dbConnection;
            return command;
        }
        
        public static bool IsListType(this Type type)
        {
            return type.GetInterfaces().Contains(typeof(IList));
        }

        public static DynamicParameters ToDynamicParameters(this IDataParameterCollection parameterCollection)
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

        public static DynamicParameters AddDbDataParameter(this DynamicParameters dynamic, IDbDataParameter dbParam)
        {
            var paramInfo = new ParamInfo
            {
                Name = dbParam.ParameterName,
                Value = dbParam.Value
            };

            var dbTypeHasValue = (dbParam.DbType != default(DbType));
            var directionHasValue = (dbParam.Direction != default(ParameterDirection));
            var sizeHasValue = (dbParam.Size != default(int));
            var precisionHasValue = (dbParam.Precision != default(byte));
            var scaleHasValue = (dbParam.Scale != default(byte));

            if (dbTypeHasValue)
            {
                paramInfo.DbType = dbParam.DbType;
            }

            if (directionHasValue)
            {
                paramInfo.ParameterDirection = dbParam.Direction;
            }

            if (sizeHasValue)
            {
                paramInfo.Size = dbParam.Size;
            }

            if (precisionHasValue)
            {
                paramInfo.Precision = dbParam.Precision;
            }

            if (scaleHasValue)
            {
                paramInfo.Scale = dbParam.Scale;
            }

            dynamic.Add(paramInfo.Name,paramInfo.Value,paramInfo.DbType, paramInfo.ParameterDirection, paramInfo.Size,paramInfo.Precision, paramInfo.Scale);

            return dynamic;
        }


        public static void AddParameter(
            this IDbCommand command,
            string parameterName,
            object value,
            DbType? dbType = null,
            ParameterDirection? direction = null,
            int? size = null,
            byte? precision = null,
            byte? scale = null)
        {
            var param = command.CreateParameter();
            param.ParameterName = parameterName;
            param.Value = value;
            if (dbType != null) param.DbType = dbType.Value;
            if (direction != null) param.Direction = direction.Value;
            if (size != null) param.Size = size.Value;
            if (precision != null) param.Precision = precision.Value;
            if (scale != null) param.Scale = scale.Value;
            command.Parameters.Add(param);
        }

        private sealed class ParamInfo
        {
            public string Name { get; set; }
            public object Value { get; set; }
            public ParameterDirection ParameterDirection { get; set; }
            public DbType? DbType { get; set; }
            public int? Size { get; set; }

            public byte? Precision { get; set; }

            public byte? Scale { get; set; }

        }
    }
}