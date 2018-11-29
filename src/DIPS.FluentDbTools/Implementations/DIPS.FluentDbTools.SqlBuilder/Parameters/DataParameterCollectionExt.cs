using System.Collections.Generic;
using System.Data;
using Dapper;
using DIPS.FluentDbTools.Common.Abstractions;
using DIPS.FluentDbTools.SqlBuilder.Abstractions.Parameters;

namespace DIPS.FluentDbTools.SqlBuilder.Parameters
{
    internal class DataParameterCollectionExt : IDataParameterCollectionExt
    {
        private readonly IDatabaseParameterHelper DataParameterHelperField;
        private DynamicParameters DynamicParameters { get; }

        public DataParameterCollectionExt(IDatabaseParameterHelper dataParameterHelper)
        {
            DataParameterHelperField = dataParameterHelper;
            DynamicParameters = new DynamicParameters();
        }

        public void Add(string parameterName, object value, DbType? dbType = null, ParameterDirection? direction = null,
            int? size = null, byte? precision = null, byte? scale = null)
        {
            DynamicParameters.Add(parameterName, value, dbType, direction, size, precision, scale);

        }

        public void Add(object @params)
        {
            DynamicParameters.AddDynamicParams(@params);
        }

        public void AddArrayParameter<T>(string paramName, IEnumerable<T> enumerable, ref string sql)
        {
            var array = DataParameterHelperField.ToParameterArrayValue(enumerable);
            if (DataParameterHelperField.DatabaseType == SupportedDatabaseTypes.Postgres)
            {
                var parameterNames = new List<string>();
                for (var i = 0; i < array.Length; i++)
                {
                    var parameterName = $"{paramName}{i}";
                    parameterNames.Add(parameterName);
                    
                    Add(parameterName, array.GetValue(i));
                }

                sql = sql.Replace(DataParameterHelperField.WithParameters(paramName), $"({string.Join(",", DataParameterHelperField.WithParameters(parameterNames.ToArray()))})");
            }
            else
            {
                DynamicParameters.Add(paramName, array);
            }
        }

        public DynamicParameters ToDynamicParameters()
        {
            return DynamicParameters;
        }

    }
}