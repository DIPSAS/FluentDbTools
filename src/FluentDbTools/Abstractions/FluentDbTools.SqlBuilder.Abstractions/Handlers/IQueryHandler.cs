using FluentDbTools.SqlBuilder.Abstractions.Commands;
#pragma warning disable CS1591

namespace FluentDbTools.SqlBuilder.Abstractions.Handlers
{
    public interface IQueryHandler
    {
        TResult Execute<TResult>(IPlainQueryCommand<TResult> command);
    }
}