using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FluentMigrator.Builders;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;

namespace FluentDbTools.Migration.Abstractions
{
    /// <summary>
    /// Methods using <see cref="Expression"/> to resolve <see cref="IMigrationContext"/> from object
    /// </summary>
    public static class LinqExpressions
    {
        //private static Func<object, IMigrationContext> MigrationContextFromBuilderGetter;

        private static readonly Dictionary<Type, Func<object, IMigrationContext>> MigrationContextGetterDictionary = new Dictionary<Type, Func<object, IMigrationContext>>();

        /// <summary>
        /// Using <see cref="Expression"/>'s to Search object <paramref name="objectHavingPrivateMigrationContext"/> for <see cref="IMigrationContext"/> field
        /// </summary>
        /// <param name="objectHavingPrivateMigrationContext"></param>
        /// <param name="type"></param>
        /// <returns><see cref="IMigrationContext"/></returns>
        public static IMigrationContext GetMigrationContextFromObject(this object objectHavingPrivateMigrationContext, Type type = null)
        {
            type = type ?? objectHavingPrivateMigrationContext.GetType();
            if (!MigrationContextGetterDictionary.TryGetValue(type, out var migrationContextFromGetter)) 
            {
                var field = type.SearchForField(typeof(IMigrationContext), "_context");
                var param = Expression.Parameter(typeof(object), "param");
                
                var fieldMemberExpression = GetFieldMemberLinqExpression<object>(field, param);

                if (fieldMemberExpression != null)
                {
                    var getFieldValueExp = Expression.Lambda(fieldMemberExpression, param);

                    var getFieldValueLambda = (Expression<Func<object, IMigrationContext>>) getFieldValueExp;

                    migrationContextFromGetter = getFieldValueLambda.Compile();
                    MigrationContextGetterDictionary.Add(type, migrationContextFromGetter);
                }
            }

            var context = migrationContextFromGetter?.Invoke(objectHavingPrivateMigrationContext);
            return context;
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
                            new[] {typeof(Expression), typeof(FieldInfo)},
                            new ParameterModifier[0]
                        );

                fieldMemberExpression = methodInfo?.Invoke(null, new object[] {param, field}) as MemberExpression;
            }

            return fieldMemberExpression;
        }

        private static FieldInfo SearchForField(this Type instanceType, Type fieldType, string fieldName = null, int depth = 0)
        {
            if (depth > 5 || instanceType == null)
            {
                return null;
            }
            var fieldInfo = (string.IsNullOrEmpty(fieldName) ? null : instanceType.GetRuntimeField(fieldName));
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
    }
}