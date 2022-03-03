using FluentDbTools.Common.Abstractions;
#pragma warning disable CS1591

namespace FluentDbTools.SqlBuilder.Abstractions.Common
{
    public interface ISqlBuildOnly
    {
        SupportedDatabaseTypes DbType { get; }
        string Build();
    }
}