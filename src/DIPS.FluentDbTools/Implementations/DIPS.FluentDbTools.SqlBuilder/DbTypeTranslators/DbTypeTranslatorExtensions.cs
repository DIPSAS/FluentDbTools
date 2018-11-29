using System.Collections.Generic;
using DIPS.FluentDbTools.Common.Abstractions;
using DIPS.FluentDbTools.SqlBuilder.Abstractions.Parameters;

namespace DIPS.FluentDbTools.SqlBuilder.DbTypeTranslators
{
    internal static class DbTypeTranslatorExtensions
    {
        private static readonly IDictionary<SupportedDatabaseTypes, IDbTypeTranslator> DbTypeTranslators =
            new Dictionary<SupportedDatabaseTypes, IDbTypeTranslator>
            {
                {SupportedDatabaseTypes.Oracle, new KnownOracleDbTypeTranslator()},
                {SupportedDatabaseTypes.Postgres, new KnownPostgresDbTypeTranslator()}
            };
        
        public static IDbTypeTranslator GetDbTypeTranslator(this SupportedDatabaseTypes databaseType)
        {
            return DbTypeTranslators[databaseType];
        }
    }
}