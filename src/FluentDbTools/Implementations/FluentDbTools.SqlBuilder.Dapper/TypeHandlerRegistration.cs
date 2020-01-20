using System;
using System.Runtime.CompilerServices;
using Dapper;

[assembly: InternalsVisibleTo("FluentDbTools.Extensions.SqlBuilder")]
namespace FluentDbTools.SqlBuilder
{
    internal static class TypeHandlerRegistration
    {
        public static void RegisterTypeHandlers()
        {
            SqlMapper.AddTypeHandler(typeof(Guid), new CustomGuidTypeHandler());
        }
    }
}