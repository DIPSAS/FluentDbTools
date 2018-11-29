using DIPS.FluentDbTools.Common.Abstractions;
using DIPS.FluentDbTools.SqlBuilder.Abstractions.Parameters;

namespace DIPS.FluentDbTools.SqlBuilder.Abstractions
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