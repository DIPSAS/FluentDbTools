using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using FluentMigrator.Builders;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
[assembly: InternalsVisibleTo("FluentDbTools.Migration")]
[assembly: InternalsVisibleTo("FluentDbTools.Migration.Contracts")]
[assembly: InternalsVisibleTo("FluentDbTools.Migration.Oracle")]
[assembly: InternalsVisibleTo("FluentDbTools.Migration.Postgres")]

namespace FluentDbTools.Migration.Abstractions
{
    /// <summary>
    /// Methods using <see cref="Expression"/> to resolve <see cref="IMigrationContext"/> from object
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    internal static class LinqExpressions
    {
        //private static Func<object, IMigrationContext> MigrationContextFromBuilderGetter;

        private static readonly Dictionary<Type, Func<object, IMigrationContext>> MigrationContextGetterDictionary = new Dictionary<Type, Func<object, IMigrationContext>>();
        private static readonly Dictionary<string, Func<object, object>> ObjectGetterDictionary = new Dictionary<string, Func<object, object>>();

        /// <summary>
        /// Using <see cref="Expression"/>'s to Search object <paramref name="objectHavingPrivateMigrationContext"/> for <see cref="IMigrationContext"/> field
        /// </summary>
        /// <param name="objectHavingPrivateMigrationContext"></param>
        /// <param name="type"></param>
        /// <returns><see cref="IMigrationContext"/></returns>
        public static IMigrationContext GetMigrationContextFromObject(this object objectHavingPrivateMigrationContext, Type type = null)
        {
            return objectHavingPrivateMigrationContext.GetFieldValue<IMigrationContext>(type, "_context");
        }

        /// <summary>
        /// Using <see cref="Expression"/>'s to Search object <paramref name="builder"/> for <see cref="IMigrationContext"/> field
        /// </summary>
        /// <param name="builder"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns><see cref="IMigrationContext"/></returns>
        public static IMigrationContext GetMigrationContextFromBuilder<T>(this ExpressionBuilderBase<T> builder)
            where T : class, IMigrationExpression
        {
            return builder.GetMigrationContextFromObject();
        }

        /// <summary>
        /// Using <see cref="Expression"/>'s to Search object <paramref name="objectHavingField"/> for <typeparamref name="T"/>  field
        /// If <paramref name="fieldName"/> have a value, the search will try to find field with that name. If not found, field of <typeparamref name="T"/> will be searched for.
        /// Returns value of <typeparamref name="T"/>
        /// </summary>
        /// <param name="objectHavingField"></param>
        /// <param name="instanceType"></param>
        /// <param name="fieldName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetFieldValue<T>(this object objectHavingField, Type instanceType = null, string fieldName = null) where T : class
        {
            return objectHavingField.GetFieldValueObject(fieldType: typeof(T), instanceType: instanceType, fieldName: fieldName) as T;
        }


        public static object GetFieldValueObject(this object objectHavingField, Type fieldType = null, Type instanceType = null, string fieldName = null)
        {
            instanceType = instanceType ?? objectHavingField.GetType();
            var fieldTypeId = $"{fieldType?.Name ?? fieldName ?? ""}";
            if (string.IsNullOrEmpty(fieldTypeId))
            {
                return null;
            }

            var key = $"{instanceType.Name}-{fieldTypeId}";
            if (!ObjectGetterDictionary.TryGetValue(key, out var getter))
            {
                var field = instanceType.SearchForField(fieldType, fieldName);
                fieldType = fieldType ?? field?.FieldType;
                if (fieldType == null)
                {
                    return null;
                }

                var param = Expression.Parameter(typeof(object), "param");

                var fieldMemberExpression = GetFieldMemberLinqExpression<object>(field, param);

                if (fieldMemberExpression == null)
                {
                    return null;
                }

                var parameters = new List<ParameterExpression> {param};

                var funcType = Expression.GetFuncType(typeof(object), fieldType);
                var getFieldValueLambda  = Expression.Lambda(funcType, fieldMemberExpression, parameters);
                getter = getFieldValueLambda.Compile() as Func<object,object>;
                try
                {
                    ObjectGetterDictionary.Add(key, getter);
                }
                catch
                {
                    if (ObjectGetterDictionary.ContainsKey(key))
                    {
                        ObjectGetterDictionary[key] = getter;
                    }
                }
            }

            var fieldValue = getter?.Invoke(objectHavingField);
            return fieldValue;
        }


        private static MemberExpression GetFieldMemberLinqExpression<T>(FieldInfo field, ParameterExpression param = null)
        {
            param = param ?? Expression.Parameter(typeof(T), "param");
            MemberExpression fieldMemberExpression;
            try
            {
                fieldMemberExpression = Expression.Field(param, field);
            }
            catch
            {
                var methodInfo =
                    typeof(MemberExpression)
                        .GetMethod(
                            "Make",
                            bindingAttr: BindingFlags.NonPublic | BindingFlags.Static,
                            binder: Type.DefaultBinder,
                            new[] { typeof(Expression), typeof(FieldInfo) },
                            new ParameterModifier[0]
                        );

                fieldMemberExpression = methodInfo?.Invoke(null, new object[] { param, field }) as MemberExpression;
            }

            return fieldMemberExpression;
        }

        public static FieldInfo SearchForField(this Type instanceType, Type fieldType, string fieldName = null, int depth = 0)
        {
            if (fieldType == null && !string.IsNullOrEmpty(fieldName))
            {
                return instanceType.SearchForField(fieldName);
            }


            if (depth > 5 || instanceType == null)
            {
                return null;
            }
            var fieldInfo = string.IsNullOrEmpty(fieldName) ? null : instanceType.GetTheField(fieldName);
            if (!fieldInfo.IsFieldType(fieldType))
            {
                fieldInfo = instanceType.GetRuntimeFields().FirstOrDefault(x => x.FieldType == fieldType);
            }

            if (fieldInfo.IsFieldType(fieldType))
            {
                return fieldInfo;
            }

            fieldInfo = fieldInfo.IsFieldType(fieldType) ? fieldInfo : SearchForField(instanceType.BaseType, fieldType, fieldName, ++depth);

            return fieldInfo.IsFieldType(fieldType) ? fieldInfo : null;
        }

        [SuppressMessage("ReSharper", "TailRecursiveCall")]
        private static FieldInfo SearchForField(this Type instanceType, string fieldName, int depth = 0)
        {
            if (depth > 5 || instanceType == null)
            {
                return null;
            }

            var fieldInfo = instanceType.GetTheField(fieldName);
            return fieldInfo ?? SearchForField(instanceType.BaseType, fieldName, ++depth);
        }

        private static FieldInfo GetTheField(this Type instanceType, string fieldName)
        {
            var fieldInfo = instanceType.GetRuntimeField(fieldName) ?? instanceType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            return fieldInfo;
        }

    }
}