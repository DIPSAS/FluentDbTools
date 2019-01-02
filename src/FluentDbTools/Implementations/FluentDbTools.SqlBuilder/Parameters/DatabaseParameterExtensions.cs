using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.SqlBuilder.Abstractions.Parameters;

namespace FluentDbTools.SqlBuilder.Parameters
{
    internal static class DatabaseParameterExtensions
    {
        public static string GetParameterWithIdPostfix<T>(this IDatabaseParameterHelper @param, string postfix = "Id")
        {
            return GetParameterWithIdPostfix<T>(postfix);
        }

        public static string GetParameterWithIdPostfix<T>(string postfix = "Id")
        {
            return typeof(T).Name + postfix;
        }


        public static string[] AddArrayParameter<T>(this IDatabaseParameterHelper databaseParameterHelper, DynamicParameters parameters, string paramName, IEnumerable<T> enumerable)
        {
            var array = databaseParameterHelper.ToParameterArrayValue(enumerable);
            if (databaseParameterHelper.DatabaseType != SupportedDatabaseTypes.Oracle)
            {
                var parameterNames = new List<string>();
                for (var i = 0; i < array.Length; i++)
                {
                    var parameterName = $"{paramName}{i}";
                    parameterNames.Add($"{databaseParameterHelper.GetParameterPrefix()}{parameterName}");
                    parameters.Add(parameterName, array.GetValue(i));
                }

                return parameterNames.ToArray();
            }
            parameters.Add(paramName, array);

            return new []{$"{databaseParameterHelper.GetParameterPrefix()}{paramName}"};
        }

        public static void AddArrayParameter<T>(this IDatabaseParameterHelper databaseParameterHelper, IDataParameterCollectionExt parameters, string paramName, IEnumerable<T> enumerable, ref string sql)
        {
            parameters.AddArrayParameter(paramName, enumerable, ref sql);
        }


        public static Array ToParameterArrayValue<T>(this IDatabaseParameterHelper databaseParameterHelper, IEnumerable<T> collection)
        {
            if (databaseParameterHelper.DatabaseType == SupportedDatabaseTypes.Oracle)
            {
                if (typeof(T) == typeof(Guid))
                {
                    return collection.Cast<Guid>().Select(databaseParameterHelper.WithGuidParameterValue).ToArray();
                }
            }

            return collection.ToArray();

        }
    }
}