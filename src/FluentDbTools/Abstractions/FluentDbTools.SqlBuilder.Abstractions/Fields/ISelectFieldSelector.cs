﻿using System;
using System.Linq.Expressions;
#pragma warning disable CS1591

namespace FluentDbTools.SqlBuilder.Abstractions.Fields
{
    public interface ISelectFieldSelector<TClass>
    {
        ISelectFieldSelector<TClass> F<T>(Expression<Func<TClass, T>> field, string alias = null, string tablealias = null);
        ISelectFieldSelector<TClass> F(string field, string alias = null, string tablealias = null);
        ISelectFieldSelector<TClass> All(string tablealias = null);

        ISelectFieldSelector<TClass> Count();
    }
}