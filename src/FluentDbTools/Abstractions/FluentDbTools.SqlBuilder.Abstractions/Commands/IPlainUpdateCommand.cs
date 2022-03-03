using System.Data;
#pragma warning disable CS1591

namespace FluentDbTools.SqlBuilder.Abstractions.Commands
{
    public interface IPlainUpdateCommand
    {
        void Execute(IDbConnection connection, IDbTransaction transaction);
    }
}