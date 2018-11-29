using System;
using DIPS.FluentDbTools.SqlBuilder.Abstractions.Common;

namespace DIPS.FluentDbTools.SqlBuilder.Abstractions.Fields
{
    public interface ISqlFieldBuilder<TClass> : ISqlBuildOnly
    {
        ISqlFieldBuilder<TClass> AddFields(Action<IFieldSetterSelector<TClass>> selector);
        ISqlFieldBuilder<TClass> AddDynamicFields(dynamic fields);
    }
}