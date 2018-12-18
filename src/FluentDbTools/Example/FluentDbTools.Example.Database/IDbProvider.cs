using System;
using System.Data;

namespace FluentDbTools.Example.Database
{
    public interface IDbProvider : IDisposable
    {
        IDbConnection DbConnection { get; }
        IDbTransaction DbTransaction { get; }
    }
}