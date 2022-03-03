using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
#pragma warning disable CS1591

namespace System.Data
{
    public static class HiddenPropertyResolverExtensions
    {
        private static readonly ConcurrentDictionary<string, Func<object, object>> ExpressionGetDelegateLambdaDict = new ConcurrentDictionary<string, Func<object, object>>();
        private static readonly ConcurrentDictionary<string, Action<object, object>> ExpressionSetDelegateLambdaDict = new ConcurrentDictionary<string, Action<object, object>>();

        internal static ConcurrentDictionary<Type, Action<Exception, string>[]> StackTraceExpressionStringLambdaDictionary = new ConcurrentDictionary<Type, Action<Exception, string>[]>();
        internal static ConcurrentDictionary<Type, Action<Exception, object>[]> StackTraceExpressionObjectLambdaDictionary = new ConcurrentDictionary<Type, Action<Exception, object>[]>();

        /// <summary>
        /// Get value from a "hidden" property/field in <paramref name="object"></paramref><br/>
        /// <br/>
        /// The code behind this method use "LambdaExpression over Reflection" to optimize the call. <br/>
        /// A local key-delegate cache-dictionary is used to avoid expression/reflection-resolving each time.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="object"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Global
        public static T GetHiddenPropertyValue<T>(this object @object, string propertyName)
        {
            if (@object == null)
            {
                return default;
            }

            var type = @object.GetType();
            var key = type.FullName + "__" + propertyName;

            // ReSharper disable once InvertIf
            if (!ExpressionGetDelegateLambdaDict.TryGetValue(key, out var delegateMethod))
            {
                delegateMethod = CreateGetValueDelegate(type, propertyName);
                ExpressionGetDelegateLambdaDict.TryAdd(key, delegateMethod);
            }

            return delegateMethod?.Invoke(@object) is T value ? value : default;
        }

        /// <summary>
        /// Set <paramref name="value"/> from a "hidden" property/field in <paramref name="object"></paramref><br/>
        /// <br/>
        /// The code behind this method use "LambdaExpression over Reflection" to optimize the call. <br/>
        /// A local key-delegate cache-dictionary is used to avoid expression/reflection-resolving each time.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="object"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Global
        public static void SetHiddenPropertyValue<T, TValue>(this T @object, string propertyName, TValue value)
        {
            if (@object == null)
            {
                return;
            }

            var type = typeof(T);
            var key = type.FullName + "__" + propertyName;

            // ReSharper disable once InvertIf
            if (!ExpressionSetDelegateLambdaDict.TryGetValue(key, out var delegateMethod))
            {
                delegateMethod = CreateSetValueDelegate<T, TValue>(propertyName);
                ExpressionSetDelegateLambdaDict.TryAdd(key, delegateMethod);
            }

            delegateMethod?.Invoke(@object, value);
        }


        private static Func<object, object> CreateGetValueDelegate(Type declareType, string propertyOrFieldName)
        {
            var paramInstance = Expression.Parameter(typeof(object));
            var bodyObjToType = Expression.Convert(paramInstance, declareType);
            try
            {
                var bodyGetTypeProperty = Expression.PropertyOrField(bodyObjToType, propertyOrFieldName);
                if (bodyGetTypeProperty.Expression == null)
                {
                    return null;
                }
                var bodyReturn = Expression.Convert(bodyGetTypeProperty, typeof(object));
                return Expression.Lambda<Func<object, object>>(bodyReturn, paramInstance).Compile();
            }
            catch
            {
                return null;
            }
        }

        private static Action<object, object> CreateSetValueDelegate<T, TValue>(string propertyOrFieldName)
        {
            var declareType = typeof(T);
            var targetExp = Expression.Parameter(declareType, "target");
            var valueExp = Expression.Parameter(typeof(TValue), "value");

            var targetInstance = Expression.Convert(targetExp, declareType);
            var targetValue = Expression.Convert(valueExp, typeof(TValue));
            try
            {
                var property = declareType.GetProperty(propertyOrFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var setterMethod = property?.GetSetMethod();
                if (setterMethod != null)
                {
                    var bodyCall = Expression.Call(targetInstance, setterMethod, targetValue);
                    var strongPropSetter = Expression.Lambda<Action<T, TValue>>(bodyCall, targetExp, valueExp).Compile();
                    return (owner, value) => strongPropSetter.Invoke((T)owner, (TValue)value);
                }

                var field = declareType.GetField(propertyOrFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                MemberExpression memberExpression = null;
                if (field != null)
                {
                    memberExpression = Expression.Field(targetExp, field);
                }
                else if (property != null)
                {
                    memberExpression = Expression.Property(targetExp, property);
                }

                if (memberExpression == null)
                {
                    return null;
                }

                var assignExp = Expression.Assign(memberExpression, valueExp);
                var strongSetter = Expression.Lambda<Action<T, TValue>>(assignExp, targetExp, valueExp).Compile();
                return (owner, value) => strongSetter.Invoke((T)owner, (TValue)value);
            }
            catch
            {
                return null;
            }
        }

        internal static bool TrySetStacktracePropertyByLambdaExpression<T>(this T @object, string value) where T : Exception
        {
            try
            {
                if (string.IsNullOrEmpty(@object?.StackTrace))
                {
                    return true;
                }

                if (string.Equals(@object.StackTrace, value))
                {
                    return true;
                }


                var type = typeof(T);
                if (!StackTraceExpressionStringLambdaDictionary.TryGetValue(type, out var setters))
                {
                    var setter1 = GetFieldSetterAction<string>(type, "_stackTraceString");
                    var setter2 = GetFieldSetterAction<string>(type, "_remoteStackTraceString");

                    setters = new[] { setter1, setter2 }.Where(x => x != null).ToArray();
                    StackTraceExpressionStringLambdaDictionary.TryAdd(type, setters);

                }

                if (!StackTraceExpressionObjectLambdaDictionary.TryGetValue(type, out var setters2))
                {
                    var setter3 = GetFieldSetterAction<object>(type, "_stackTrace");
                    if (setter3 != null)
                    {
                        StackTraceExpressionObjectLambdaDictionary.TryAdd(type, new[] { setter3 });
                    }
                }

                if (setters == null && setters2 == null)
                {
                    return false;
                }

                if (setters != null)
                {
                    foreach (var action in setters)
                    {
                        action?.Invoke(@object, value);
                        if (IsChanged())
                        {
                            break;
                        }
                    }
                }

                if (setters2 != null && !IsChanged())
                {
                    foreach (var action in setters2)
                    {
                        action?.Invoke(@object, null);
                        if (IsChanged())
                        {
                            break;
                        }
                    }
                }

                return IsChanged();
            }
            catch
            {
                //
            }

            return false;

            bool IsChanged()
            {
                var isChanged = (@object.StackTrace ?? string.Empty) == (value ?? string.Empty);
                return isChanged;
            }
        }

        private static Action<Exception, TValue> GetFieldSetterAction<TValue>(Type type, string setterFieldName)
        {
            var field = type.GetField(setterFieldName, BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.NonPublic);

            if (field == null)
            {
                return null;
            }

            var targetExp = Expression.Parameter(type, "target");
            var valueExp = Expression.Parameter(typeof(TValue), "value");

            var fieldExp = Expression.Field(targetExp, field);
            var assignExp = Expression.Assign(fieldExp, valueExp);
            return Expression.Lambda<Action<Exception, TValue>>(assignExp, targetExp, valueExp).Compile();
        }

        public static string StripAllStackTraces(string message, ref Exception exception, string newStacktrace = null)
        {
            if (string.IsNullOrEmpty(message) == false)
            {
                var pos = message.IndexOf("\n   at ", StringComparison.OrdinalIgnoreCase);
                if (pos == -1) pos = message.IndexOf("\n   ved ", StringComparison.OrdinalIgnoreCase);
                if (pos > -1)
                {
                    message = message.Substring(0, pos - 1);
                }
            }

            var setStackTrace = string.IsNullOrEmpty(newStacktrace) == false;
            setStackTrace = setStackTrace || string.IsNullOrEmpty(newStacktrace) &&
                string.IsNullOrEmpty(exception?.StackTrace) == false;

            if (setStackTrace && exception?.TrySetStacktracePropertyByLambdaExpression(newStacktrace) == false)
            {
                exception = null;
            }


            return message;
        }

    }
}