using FluentDbTools.Migration.Abstractions;
using FluentDbTools.Migration.Abstractions.ExtendedExpressions;
using FluentMigrator.Expressions;

namespace FluentDbTools.Migration.Contracts.MigrationExpressions
{
    /// <summary>
    /// MigrationExpression for signalling current migration assembly
    /// </summary>
    public class MigrationMetadataChangedExpression : PerformDBOperationExpression, ILinkedExpression
    {
        /// <summary>
        /// Information about current MigrationAssembly
        /// </summary>
        public IMigrationMetadata MigrationMetadata { get; }

        /// <summary>
        /// Constructor initialized <paramref name="migrationMetadata"/> to <see cref="MigrationMetadata"/>
        /// </summary>
        /// <param name="migrationMetadata"></param>
        public MigrationMetadataChangedExpression(IMigrationMetadata migrationMetadata)
        {
            Operation = (connection, transaction) => { };
            MigrationMetadata = migrationMetadata;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"MigrationMetadataChangedExpression: {MigrationMetadata}";
        }
    }
}