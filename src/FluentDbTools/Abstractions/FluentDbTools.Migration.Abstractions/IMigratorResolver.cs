namespace FluentDbTools.Migration.Abstractions
{
    public interface IMigrationResolver : IMigrationExecutor
    {
        bool CanMigrate();
    }
}
