using System;
using System.Data;
using Dapper;

namespace DIPS.FluentDbTools.SqlBuilder.TypeHandlers
{
    internal class CustomGuidTypeHandler : SqlMapper.ITypeHandler
    {
        public void SetValue(IDbDataParameter parameter, object value)
        {
            parameter.DbType = DbType.Guid;
            parameter.Value = ((Guid)value).ToByteArray();
        }

        public object Parse(Type destinationType, object value)
        {
        
            if (value == null)
            {
                return null;
            }
            if (value is Guid guid)
            {
                return guid;
            }

            if (value is byte[] bytes)
            {
                return new Guid(bytes);
            }

            return value;
        
        }
    }
}