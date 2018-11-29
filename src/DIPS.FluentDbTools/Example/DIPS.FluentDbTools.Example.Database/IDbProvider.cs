using System;
using System.Data;

namespace DIPS.FluentDbTools.Example.Database
{
    public interface IDbProvider : IDisposable
    {
        IDbConnection DbConnection { get; }
        IDbTransaction DbTransaction { get; }
    }
}