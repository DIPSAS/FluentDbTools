using System;
using System.Collections.Generic;
using System.Data;

namespace FluentDbTools.SqlBuilder.Abstractions.Commands
{
    public interface IBatchUpdateCommand
    {
        IEnumerable<Tuple<long, long>> Execute(IDbConnection connection, IDbTransaction transaction);
    }
}