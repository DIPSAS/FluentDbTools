using System;
using System.Collections.Generic;
using System.Data;
#pragma warning disable CS1591

namespace FluentDbTools.SqlBuilder.Abstractions.Commands
{
    public interface IBatchUpdateCommand
    {
        IEnumerable<Tuple<long, long>> Execute(IDbConnection connection, IDbTransaction transaction);
    }
}