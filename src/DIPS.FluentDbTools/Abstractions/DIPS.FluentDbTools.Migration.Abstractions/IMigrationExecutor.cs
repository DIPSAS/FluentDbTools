namespace DIPS.FluentDbTools.Migration.Abstractions
{
    public interface IMigrationExecutor
    {
        /// <summary>
        /// Executes all found (and unapplied) migrations
        /// </summary>
        IMigrationExecutor MigrateUp();

        /// <summary>
        /// Executes all found (and unapplied) migrations up to the given version
        /// </summary>
        /// <param name="version">The target version to migrate to (inclusive)</param>
        IMigrationExecutor MigrateUp(long version);

        /// <summary>
        /// Rollback the given number of steps
        /// </summary>
        /// <param name="steps">The number of steps to rollback</param>
        IMigrationExecutor Rollback(int steps);

        /// <summary>
        /// Rollback to a given version
        /// </summary>
        /// <param name="version">The target version to rollback to</param>
        IMigrationExecutor RollbackToVersion(long version);

        /// <summary>
        /// Reverting all migration steps
        /// </summary>
        IMigrationExecutor MigrateDown();

        /// <summary>
        /// Migrate down to the given version
        /// </summary>
        /// <param name="version">The version to migrate down to</param>
        IMigrationExecutor MigrateDown(long version);

        /// <summary>
        /// List all migrations to the logger
        /// </summary>
        IMigrationExecutor ListMigrations();
    }
}
