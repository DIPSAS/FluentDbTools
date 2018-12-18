namespace FluentDbTools.Migration.Contracts
{
    public static class MigrationConsts
    {
        public const int DefaultStringLength = 30;
        public const int StatisticalDecimalPrecision = 5;
        public const int StatisticalDecimalSize = 15;

        public const string DateTimeTypeForOracle = "TIMESTAMP(3)";

        public const string BlobTypeForOracle = "BLOB";
        public const string BlobTypeForMsSql = "LONGTEXT";
        public const string BlobTypeForPostgres = "BYTEA";
    }
}