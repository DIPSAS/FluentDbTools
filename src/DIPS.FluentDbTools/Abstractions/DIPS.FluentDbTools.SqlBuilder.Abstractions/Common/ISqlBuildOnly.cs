using DIPS.FluentDbTools.Common.Abstractions;

namespace DIPS.FluentDbTools.SqlBuilder.Abstractions.Common
{
    public interface ISqlBuildOnly
    {
        SupportedDatabaseTypes DbType { get; }
        string Build();
    }
}