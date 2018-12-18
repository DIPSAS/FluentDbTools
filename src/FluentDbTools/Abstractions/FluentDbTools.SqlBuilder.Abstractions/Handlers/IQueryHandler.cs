using FluentDbTools.SqlBuilder.Abstractions.Commands;

namespace FluentDbTools.SqlBuilder.Abstractions.Handlers
{
    public interface IQueryHandler
    {
        TResult Execute<TResult>(IPlainQueryCommand<TResult> command);
    }
}