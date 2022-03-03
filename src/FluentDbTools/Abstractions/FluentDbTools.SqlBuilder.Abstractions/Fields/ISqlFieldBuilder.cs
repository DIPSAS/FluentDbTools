using System;
using FluentDbTools.SqlBuilder.Abstractions.Common;
#pragma warning disable CS1591

namespace FluentDbTools.SqlBuilder.Abstractions.Fields
{
    public interface ISqlFieldBuilder<TClass> : ISqlBuildOnly
    {
        ISqlFieldBuilder<TClass> AddFields(Action<IFieldSetterSelector<TClass>> selector);
        ISqlFieldBuilder<TClass> AddDynamicFields(dynamic fields);
    }
}