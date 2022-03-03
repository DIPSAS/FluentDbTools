using System.Data;
#pragma warning disable CS1591

namespace FluentDbTools.SqlBuilder.Abstractions.Commands
{
    public interface IPlainQueryCommand<out TResult>
    {
        TResult Execute(IDbConnection connection, IDbTransaction transaction);
    }
}