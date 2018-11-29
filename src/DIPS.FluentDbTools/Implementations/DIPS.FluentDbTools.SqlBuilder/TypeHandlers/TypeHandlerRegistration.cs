using System;
using System.Runtime.CompilerServices;
using Dapper;

[assembly: InternalsVisibleTo("DIPS.Extensions.FluentDbTools.SqlBuilder")]
namespace DIPS.FluentDbTools.SqlBuilder.TypeHandlers
{
    internal static class TypeHandlerRegistration
    {
        public static void RegisterTypeHandlers()
        {
            SqlMapper.AddTypeHandler(typeof(Guid), new CustomGuidTypeHandler());
        }
    }
}