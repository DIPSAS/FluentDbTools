namespace DIPS.FluentDbTools.Common.Abstractions
{
    public interface IMigrationConfig
    {
        int Version { get; }
    }
}