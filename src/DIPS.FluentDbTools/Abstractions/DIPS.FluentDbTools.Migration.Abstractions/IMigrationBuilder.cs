using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("TestDIPS.Lab.Infrastructure.Factory")]
[assembly:InternalsVisibleTo("DIPS.Lab.Infrastructure.Migration")]
[assembly:InternalsVisibleTo("DIPS.Lab.Infrastructure.Migration.Builder")]
namespace DIPS.FluentDbTools.Migration.Abstractions
{
    internal interface IMigrationBuilder
    {
        IMigrationExecutor BuildMigrator();
    }
}
