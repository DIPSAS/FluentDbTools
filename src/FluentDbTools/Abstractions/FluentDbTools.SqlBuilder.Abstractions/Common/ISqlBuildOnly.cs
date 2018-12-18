using FluentDbTools.Common.Abstractions;

namespace FluentDbTools.SqlBuilder.Abstractions.Common
{
    public interface ISqlBuildOnly
    {
        SupportedDatabaseTypes DbType { get; }
        string Build();
    }
}