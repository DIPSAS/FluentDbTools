using System;
using System.Collections.Generic;
using FluentDbTools.SqlBuilder.Abstractions.Commands;
#pragma warning disable CS1591

namespace FluentDbTools.SqlBuilder.Abstractions.Handlers
{
    public interface IUpdateHandler
    {
        void Execute(IPlainUpdateCommand command);

        IEnumerable<Tuple<long, long>> ExecuteBatch(IBatchUpdateCommand command);
    }
}