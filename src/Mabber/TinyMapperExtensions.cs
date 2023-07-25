using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Data.Mabber
{
    internal static class EnumerableExtensions
    {
        public static List<TResult> ConvertAll<TFrom, TResult>(
            this IEnumerable<TFrom> value,
            Func<TFrom, TResult> converter)
        {
            return value.Select(converter).ToList();
        }

        public static int Count(this IEnumerable source)
        {
            var collection = source as ICollection;
            if (collection != null)
            {
                return collection.Count;
            }

            var count = 0;
            foreach (object item in source)
            {
                count++;
            }
            return count;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> value)
        {
            return value.IsNull() || !value.Any();
        }

        /// <summary>
        ///     Apply the given function to each element of the collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">Input collection</param>
        /// <param name="action">Given function</param>
        public static void Iter<T>(this IEnumerable<T> value, Action<T> action)
        {
            foreach (T item in value)
            {
                action(item);
            }
        }

        /// <summary>
        ///     Apply the given function to each element of the collection.
        ///     The integer passed to the function indicates the index of element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">Input collection</param>
        /// <param name="action">Given function</param>
        public static void IterI<T>(this IEnumerable<T> value, Action<int, T> action)
        {
            var i = 0;
            foreach (T item in value)
            {
                action(i++, item);
            }
        }

        /// <summary>
        ///     Apply the given function to each element of the collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">Input collection</param>
        /// <param name="action">Given function</param>
        /// <param name="exceptionHandler">Exception handler action</param>
        public static void IterSafe<T>(
            this IEnumerable<T> value,
            Action<T> action,
            Action<Exception> exceptionHandler = null)
        {
            foreach (T item in value)
            {
                try
                {
                    action(item);
                }
                catch (Exception ex)
                {
                    exceptionHandler?.Invoke(ex);
                }
            }
        }

        public static IEnumerable<T> ToValue<T>(this IEnumerable<Option<T>> value)
        {
            return value.Where(x => x.HasValue)
                        .Select(x => x.Value);
        }
    }
    internal static class MemberInfoExtensions
    {
        public static Option<TAttribute> GetAttribute<TAttribute>(this MemberInfo value)
            where TAttribute : Attribute
        {
            return value.GetCustomAttributes(true)
                            .FirstOrDefault(x => x is TAttribute)
                            .ToType<TAttribute>();
        }

        public static List<TAttribute> GetAttributes<TAttribute>(this MemberInfo value)
            where TAttribute : Attribute
        {
            return value.GetCustomAttributes(true).OfType<TAttribute>().ToList();
        }

        public static Type GetMemberType(this MemberInfo value)
        {
            if (value.IsField())
            {
                return ((FieldInfo)value).FieldType;
            }
            if (value.IsProperty())
            {
                return ((PropertyInfo)value).PropertyType;
            }
            if (value.IsMethod())
            {
                return ((MethodInfo)value).ReturnType;
            }
            throw new NotSupportedException();
        }

        public static bool IsField(this MemberInfo value)
        {
#if NETFx
            return value is FieldInfo;
#else
            return value.MemberType == MemberTypes.Field;
#endif
        }

        public static bool IsProperty(this MemberInfo value)
        {
#if NETFx
            return value is PropertyInfo;
#else
            return value.MemberType == MemberTypes.Property;
#endif
        }

        private static bool IsMethod(this MemberInfo value)
        {
#if NETFx
            return value is MethodInfo;
#else
            return value.MemberType == MemberTypes.Method;
#endif

        }
    }
    internal static class DictionaryExtensions
    {
        public static Option<TValue> GetValue<TKey, TValue>(this IDictionary<TKey, TValue> value, TKey key)
        {
            TValue result;
            bool exists = value.TryGetValue(key, out result);
            return new Option<TValue>(result, exists);
        }
    }
    /// <summary>
    ///     https://github.com/Nelibur/Nelibur
    /// </summary>
    internal static class Error
    {
        public static Exception ArgumentNull(string paramName)
        {
            return new ArgumentNullException(paramName);
        }

        public static Exception InvalidOperation(string message)
        {
            return new InvalidOperationException(message);
        }

        public static Exception NotImplemented()
        {
            return new NotImplementedException();
        }

        public static Exception NotSupported()
        {
            return new NotSupportedException();
        }

        public static Exception Type<TException>()
            where TException : Exception, new()
        {
            return CreateCtor<TException>()();
        }

        private static Func<T> CreateCtor<T>()
            where T : new()
        {
            return () => new T();
        }
    }
    internal static class Helpers
    {
        internal static bool IsValueType(Type type)
        {
#if NETFx
            return type.GetTypeInfo().IsValueType;
#else
            return type.IsValueType;
#endif
        }

        internal static bool IsPrimitive(Type type)
        {
#if NETFx
            return type.GetTypeInfo().IsPrimitive;
#else
            return type.IsPrimitive;
#endif
        }

        internal static bool IsEnum(Type type)
        {
#if NETFx
            return type.GetTypeInfo().IsEnum;
#else
            return type.IsEnum;
#endif
        }

        internal static bool IsGenericType(Type type)
        {
#if NETFx
            return type.GetTypeInfo().IsGenericType;
#else
            return type.IsGenericType;
#endif
        }

        internal static Type CreateType(TypeBuilder typeBuilder)
        {
#if NETFx
            return typeBuilder.CreateTypeInfo().AsType();
#else
            return typeBuilder.CreateType();
#endif
        }

        internal static Type BaseType(Type type)
        {
#if NETFx
            return type.GetTypeInfo().BaseType;
#else
            return type.BaseType;
#endif
        }

    }
}