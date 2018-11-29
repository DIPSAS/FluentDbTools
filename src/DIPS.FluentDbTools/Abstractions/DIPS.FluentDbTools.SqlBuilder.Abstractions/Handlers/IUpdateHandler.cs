using System;
using System.Collections.Generic;
using DIPS.FluentDbTools.SqlBuilder.Abstractions.Commands;

namespace DIPS.FluentDbTools.SqlBuilder.Abstractions.Handlers
{
    public interface IUpdateHandler
    {
        void Execute(IPlainUpdateCommand command);

        IEnumerable<Tuple<long, long>> ExecuteBatch(IBatchUpdateCommand command);
    }
}