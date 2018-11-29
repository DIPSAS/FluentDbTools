using Microsoft.Extensions.Configuration;

namespace DIPS.FluentDbTools.Example.Config
{
    internal static class MigrationConfigExtensions
    {
        private const int DefaultMigrationVersion = 0;
        
        private static IConfigurationSection GetMigrationSection(this IConfiguration configuration)
        {
            return configuration.GetSection("migration");
        }
        
        public static int GetMigrationVersion(this IConfiguration configuration)
        {
            var section = configuration.GetMigrationSection();
            if (!int.TryParse(section["version"], out var version))
            {
                version = DefaultMigrationVersion;
            }
            return version;
        }
    }
}