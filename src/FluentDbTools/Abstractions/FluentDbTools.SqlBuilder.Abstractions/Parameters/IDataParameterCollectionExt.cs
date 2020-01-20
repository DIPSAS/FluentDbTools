using System.Collections.Generic;
using System.Data;

namespace FluentDbTools.SqlBuilder.Abstractions.Parameters
{
    public interface IDataParameterCollectionExt
    {
        void Add(string parameterName, object value,
            DbType? dbType = null, ParameterDirection? direction = null, int? size = null, byte? precision = null,
            byte? scale = null);

        void Add(object @params);

        void AddArrayParameter<T>(string paramName, IEnumerable<T> enumerable, ref string sql);
    }
}