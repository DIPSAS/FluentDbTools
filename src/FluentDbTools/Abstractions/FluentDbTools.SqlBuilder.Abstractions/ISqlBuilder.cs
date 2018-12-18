using FluentDbTools.Common.Abstractions;
using FluentDbTools.SqlBuilder.Abstractions.Parameters;

namespace FluentDbTools.SqlBuilder.Abstractions
{
    public interface ISqlBuilder
    {
        SupportedDatabaseTypes DatabaseType { get; }

        IDeleteSqlBuilder<TClass> Delete<TClass>();

        IUpdateSqlBuilder<TClass> Update<TClass>();

        IInsertSqlBuilder<TClass> Insert<TClass>();

        ISelectSqlBuilder Select();
    }
}