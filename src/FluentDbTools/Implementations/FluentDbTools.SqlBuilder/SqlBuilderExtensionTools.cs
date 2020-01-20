using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FluentDbTools.Common.Abstractions;

namespace FluentDbTools.SqlBuilder
{
    internal static class SqlBuilderExtensionTools
    {
        private static readonly Dictionary<string, Action<string, object, DbType?, ParameterDirection?, int?, byte?, byte?>> MethodAddDictionary = new Dictionary<string, Action<string, object, DbType?, ParameterDirection?, int?, byte?, byte?>>();

        private static readonly Dictionary<string, Action<object, object>> MethodOneObjectParameterDictionary = new Dictionary<string, Action<object, object>>();
        
        private static object MethodAddDictionaryObj;
        private static object MethodAddDictionaryObjLock = new object();

        private static object MethodOneObjectParameterDictionaryObj;
        private static object MethodOneObjectParameterDictionaryObjLock = new object();

        public static string GetParameterPrefix(this IDbConfigSchemaTargets dbConfigConfig)
        {
            return dbConfigConfig.DbType == SupportedDatabaseTypes.Oracle ? ":" : "@";
        }

        public static string WithParameters(this IDbConfigSchemaTargets dbConfigConfig, params string[] parameters)
        {
            var prefix = dbConfigConfig.GetParameterPrefix();

            parameters = parameters.Select(x => $"{prefix}{x.Replace(":", "").Replace("@", "")}").ToArray();

            return string.Join(", ", parameters);
        }


        public static IDbCommand CreateCommand(this IDbConnection dbConnection, string commandText)
        {
            var command = dbConnection.CreateCommand();
            command.Connection = dbConnection;
            return command;
        }

        public static bool IsListType(this Type type)
        {
            return type.GetInterfaces().Contains(typeof(IList));
        }

        public static TDynamicParameters ToDynamicParameters<TDynamicParameters>(this IDataParameterCollection parameterCollection) where TDynamicParameters : new()
        {

            var dynamic = new TDynamicParameters();
            foreach (var param in parameterCollection)
            {
                if (param is IDbDataParameter dbParam)
                {
                    dynamic.AddDbDataParameter(dbParam);
                }
                else
                {
                    dynamic.AddDynamicParams(param);
                }
            }

            return dynamic;
        }

        public static TDynamicParameters AddDbDataParameter<TDynamicParameters>(this TDynamicParameters dynamic, IDbDataParameter dbParam) where TDynamicParameters : new()
        {
            var paramInfo = new ParamInfo
            {
                Name = dbParam.ParameterName,
                Value = dbParam.Value
            };

            var dbTypeHasValue = (dbParam.DbType != default(DbType));
            var directionHasValue = (dbParam.Direction != default(ParameterDirection));
            var sizeHasValue = (dbParam.Size != default(int));
            var precisionHasValue = (dbParam.Precision != default(byte));
            var scaleHasValue = (dbParam.Scale != default(byte));

            if (dbTypeHasValue)
            {
                paramInfo.DbType = dbParam.DbType;
            }

            if (directionHasValue)
            {
                paramInfo.ParameterDirection = dbParam.Direction;
            }

            if (sizeHasValue)
            {
                paramInfo.Size = dbParam.Size;
            }

            if (precisionHasValue)
            {
                paramInfo.Precision = dbParam.Precision;
            }

            if (scaleHasValue)
            {
                paramInfo.Scale = dbParam.Scale;
            }

            dynamic.Add(paramInfo.Name, paramInfo.Value, paramInfo.DbType, paramInfo.ParameterDirection, paramInfo.Size, paramInfo.Precision, paramInfo.Scale);

            return dynamic;
        }


        public static void AddParameter(
            this IDbCommand command,
            string parameterName,
            object value,
            DbType? dbType = null,
            ParameterDirection? direction = null,
            int? size = null,
            byte? precision = null,
            byte? scale = null)
        {
            var param = command.CreateParameter();
            param.ParameterName = parameterName;
            param.Value = value;
            if (dbType != null) param.DbType = dbType.Value;
            if (direction != null) param.Direction = direction.Value;
            if (size != null) param.Size = size.Value;
            if (precision != null) param.Precision = precision.Value;
            if (scale != null) param.Scale = scale.Value;
            command.Parameters.Add(param);
        }

        private sealed class ParamInfo
        {
            public string Name { get; set; }
            public object Value { get; set; }
            public ParameterDirection ParameterDirection { get; set; }
            public DbType? DbType { get; set; }
            public int? Size { get; set; }

            public byte? Precision { get; set; }

            public byte? Scale { get; set; }

        }



        internal static void AddDynamicParams<TDynamicParameters>(this TDynamicParameters dynamicParameter, object param)
        {
            var type = dynamicParameter.GetType();
            var methodName = nameof(AddDynamicParams);
            var key = $"{type.Name}-{methodName}";
            if (!MethodOneObjectParameterDictionary.TryGetValue(key, out var action))
            {
                Expression<Func<TDynamicParameters>> getInstanceValue = () => (TDynamicParameters)MethodOneObjectParameterDictionaryObj;

                var paramsTypes = MethodBase.GetCurrentMethod().GetParameters().Where(x => x.Name != nameof(dynamicParameter)).ToDictionary(x => x.Name, x => x.ParameterType);

                var methodInfo = SearchForMethod(type, methodName, paramsTypes.Values.ToArray());
                var methodCallExpression = GetMethodMemberLinqExpression(type, methodInfo, paramsTypes, getInstanceValue);
                var parametersExpression = methodCallExpression.Arguments.OfType<ParameterExpression>().ToList();

                var lambdaMethod = Expression.Lambda<Action<object, object>>(methodCallExpression, parametersExpression);
                action = Expression.Lambda<Action<object, object>>(lambdaMethod.Body, parametersExpression).Compile();

                MethodAddDictionary.Add(key, action);
            }

            lock (MethodOneObjectParameterDictionaryObjLock)
            {
                MethodOneObjectParameterDictionaryObj = dynamicParameter;
                action?.Invoke(dynamicParameter, param);
            }


        }

        internal static void Add<TDynamicParameters>(this TDynamicParameters dynamicParameter,
            string name,
            object value = null,
            DbType? dbType = null,
            ParameterDirection? direction = null,
            int? size = null, byte? precision = null, byte? scale = null)
        {
            var type = dynamicParameter.GetType();
            var methodName = nameof(Add);
            var key = $"{type.Name}-{methodName}";
            if (!MethodAddDictionary.TryGetValue(key, out var action))
            {
                Expression<Func<TDynamicParameters>> getInstanceValue = () => (TDynamicParameters)MethodAddDictionaryObj;

                var paramsTypes = MethodBase.GetCurrentMethod().GetParameters().Where(x => x.Name != nameof(dynamicParameter)).ToDictionary(x => x.Name, x => x.ParameterType);

                var methodInfo = SearchForMethod(type, methodName, paramsTypes.Values.ToArray());
                var methodCallExpression = GetMethodMemberLinqExpression(type, methodInfo, paramsTypes, getInstanceValue);

                var parametersExpression = methodCallExpression.Arguments.OfType<ParameterExpression>().ToList();

                var lambdaMethod = Expression.Lambda<Action<string, object, DbType?, ParameterDirection?, int?, byte?, byte?>>(methodCallExpression, parametersExpression);
                action = Expression.Lambda<Action<string, object, DbType?, ParameterDirection?, int?, byte?, byte?>>(lambdaMethod.Body, parametersExpression).Compile();

                MethodAddDictionary.Add(key, action);
            }

            lock (MethodAddDictionaryObjLock)
            {
                MethodAddDictionaryObj = dynamicParameter;

                action?.Invoke(name, value, dbType, direction, size, precision, scale);
            }
        }

        public static MethodInfo SearchForMethod(this Type instanceType, string methodName, Type[] methodTypes, int depth = 0)
        {
            if (depth > 5 || instanceType == null)
            {
                return null;
            }
            var methodInfo = instanceType.GetTheMethod(methodName, methodTypes);

            methodInfo = methodInfo != null ? methodInfo : SearchForMethod(instanceType.BaseType, methodName, methodTypes, ++depth);

            return methodInfo;
        }

        private static MethodInfo GetTheMethod(this Type instanceType, string fieldName, Type[] methodTypes)
        {
            var fieldInfo = instanceType.GetRuntimeMethod(fieldName, methodTypes) ?? instanceType.GetMethod(fieldName, methodTypes);
            return fieldInfo;
        }

        private static MethodCallExpression GetMethodMemberLinqExpression<TDynamicParameters>(Type instanceType, MethodInfo methodInfo, IDictionary<string, Type> methodTypes, Expression<Func<TDynamicParameters>> getInstanceValue)
        {
            try
            {
                var args = methodTypes.Select(x => Expression.Parameter(x.Value, x.Key)).ToArray();
                return Expression.Call(Expression.Invoke(getInstanceValue), methodInfo, args);
            }
            catch
            {
            }
            return null;

        }

    }

}