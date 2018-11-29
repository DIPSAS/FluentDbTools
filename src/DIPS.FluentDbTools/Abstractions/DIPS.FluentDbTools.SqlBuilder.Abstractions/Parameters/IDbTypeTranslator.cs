using System.Collections.Generic;
using System.Data;
using DIPS.FluentDbTools.Common.Abstractions;

namespace DIPS.FluentDbTools.SqlBuilder.Abstractions.Parameters
{
    public interface IDbTypeTranslator
    {
        SupportedDatabaseTypes DatabaseType { get; }
        Dictionary<DbType, string> DbTypeMappings { get; }
    }
}