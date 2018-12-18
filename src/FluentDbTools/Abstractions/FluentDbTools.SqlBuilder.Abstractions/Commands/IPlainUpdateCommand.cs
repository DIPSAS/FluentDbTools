using System.Data;

namespace FluentDbTools.SqlBuilder.Abstractions.Commands
{
    public interface IPlainUpdateCommand
    {
        void Execute(IDbConnection connection, IDbTransaction transaction);
    }
}