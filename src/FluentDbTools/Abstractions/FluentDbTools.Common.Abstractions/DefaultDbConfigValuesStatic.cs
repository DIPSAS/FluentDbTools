using System.Runtime.CompilerServices;
using FluentDbTools.Common.Abstractions;
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming

#pragma warning disable CS1591

[assembly: InternalsVisibleTo("FluentDbTools.Contracts")]
namespace FluentDbTools.Contracts
{
    public class DefaultDbConfigValuesStatic
    {
        public const string DefaultOraclePort = "1521";
        public const string DefaultPostgresPort = "5432";
        public const string DefaultPossibleInvalidDbUpgradeUser = "<Db upgrade user>";
        public const string DefaultPossibleInvalidDbUpgradePassword = "<Db upgrade password>";
        public static char[] InvalidDatabaseUserCharacter = { '*', '/', '\\', ',', '{', '}', '[', ']', '<', '>', '!', '?', '-' };

        public const SupportedDatabaseTypes LibraryDefaultDbType = SupportedDatabaseTypes.Postgres;

        public static (string AdminUser, string AdminPassword) LibraryDefaultPostgresAdminUserAndPassword => ("postgres", "postgres");
        public static (string AdminUser, string AdminPassword) LibraryDefaultOracleAdminUserAndPassword => ("oracle_system", "oracle_password");
        public static (string AdminUser, string AdminPassword) LibraryDefaultPossibleInvalidAdminUserAndPassword => (DefaultPossibleInvalidDbUpgradeUser, DefaultPossibleInvalidDbUpgradePassword);
        public static (string AdminUser, string AdminPassword) EmptyAdminUserAndPassword => (null, null);

        public static (string AdminUser, string AdminPassword) DefaultOracleAdminUserAndPassword = LibraryDefaultOracleAdminUserAndPassword;
        public static (string AdminUser, string AdminPassword) DefaultPostgresAdminUserAndPassword = LibraryDefaultPostgresAdminUserAndPassword;

        public static (string AdminUser, string AdminPassword) PossibleInvalidAdminUserAndPassword = LibraryDefaultPossibleInvalidAdminUserAndPassword;

        public const string LibraryDefaultServiceName = "xe";
        public static string DefaultServiceName = LibraryDefaultServiceName;
    }
}