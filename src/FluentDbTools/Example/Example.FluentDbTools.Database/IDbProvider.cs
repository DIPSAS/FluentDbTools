using System;
using System.Data;

namespace Example.FluentDbTools.Database
{
    public interface IDbProvider : IDisposable
    {
        IDbConnection DbConnection { get; }
        IDbTransaction DbTransaction { get; }
    }
}