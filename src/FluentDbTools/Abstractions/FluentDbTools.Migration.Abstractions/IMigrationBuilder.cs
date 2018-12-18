using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("TestLab.Infrastructure.Factory")]
[assembly:InternalsVisibleTo("Lab.Infrastructure.Migration")]
[assembly:InternalsVisibleTo("Lab.Infrastructure.Migration.Builder")]
namespace FluentDbTools.Migration.Abstractions
{
    internal interface IMigrationBuilder
    {
        IMigrationExecutor BuildMigrator();
    }
}
