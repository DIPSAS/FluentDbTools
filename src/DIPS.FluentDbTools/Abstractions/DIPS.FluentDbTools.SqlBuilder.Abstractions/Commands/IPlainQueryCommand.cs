using System.Data;

namespace DIPS.FluentDbTools.SqlBuilder.Abstractions.Commands
{
    public interface IPlainQueryCommand<out TResult>
    {
        TResult Execute(IDbConnection connection, IDbTransaction transaction);
    }
}