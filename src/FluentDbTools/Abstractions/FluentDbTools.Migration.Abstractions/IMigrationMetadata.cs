using System.Reflection;

namespace FluentDbTools.Migration.Abstractions
{
    /// <summary>
    /// Information about current MigrationAssembly
    /// </summary>
    public interface IMigrationMetadata
    {
        /// <summary>
        /// Can be used to specify the Assembly where this migration is hosted<br/>
        /// By default will <see cref="MigrationName"/> be initialized from MigrationAssembly.GetName().Name
        /// </summary>
        Assembly MigrationAssembly { get; }

        /// <summary>
        /// Can be used to specify the custom MigrationName <br/>
        /// If config database:migration:migrationName OR database:migration:name exists, <see cref="MigrationName"/> will be initialized from that value 
        /// </summary>
        string MigrationName { get; }
    }
}