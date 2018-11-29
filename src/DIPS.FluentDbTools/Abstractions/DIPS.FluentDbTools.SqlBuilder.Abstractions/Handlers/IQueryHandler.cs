using DIPS.FluentDbTools.SqlBuilder.Abstractions.Commands;

namespace DIPS.FluentDbTools.SqlBuilder.Abstractions.Handlers
{
    public interface IQueryHandler
    {
        TResult Execute<TResult>(IPlainQueryCommand<TResult> command);
    }
}