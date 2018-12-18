using System;
using FluentDbTools.Common.Abstractions;

namespace FluentDbTools.SqlBuilder.Abstractions.Parameters
{
    public interface IDatabaseParameterHelper
    {
        SupportedDatabaseTypes DatabaseType { get; }

        IDbTypeTranslator DbTypeTranslator { get; }
        string WithParameters(params string[] parameters);

        string WithNextSequence(string sequenceName = null);
        string WithNextTableSequence<T>(string postfix = "_seq");
        
        string AsForeignKey<T>(string postfix = "Id");

        object WithGuidParameterValue(Guid guid);

        string GetParameterPrefix();
    }
}