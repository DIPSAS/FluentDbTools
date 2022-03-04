﻿using System.Collections.Generic;
using System.Data;
using FluentDbTools.Common.Abstractions;
#pragma warning disable CS1591

namespace FluentDbTools.SqlBuilder.Abstractions.Parameters
{
    public interface IDbTypeTranslator
    {
        SupportedDatabaseTypes DatabaseType { get; }
        Dictionary<DbType, string> DbTypeMappings { get; }
    }
}