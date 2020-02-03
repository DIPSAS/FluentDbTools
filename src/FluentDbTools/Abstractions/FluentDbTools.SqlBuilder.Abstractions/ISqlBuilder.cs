using FluentDbTools.Common.Abstractions;
using FluentDbTools.SqlBuilder.Abstractions.Parameters;

namespace FluentDbTools.SqlBuilder.Abstractions
{
    public interface ISqlBuilder
    {
        SupportedDatabaseTypes DatabaseType { get; }

        IDeleteSqlBuilder<TClass> Delete<TClass>(string tableName = null);

        IUpdateSqlBuilder<TClass> Update<TClass>(string tableName = null);

        IInsertSqlBuilder<TClass> Insert<TClass>(string tableName = null);

        ISelectSqlBuilder Select();
    }
}