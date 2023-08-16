using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
#if !NETFx
using System.Runtime.Serialization;
#endif

namespace System.Data.Mabber
{
    /// <summary>
    ///     Exception during mapping or binding
    /// </summary>
#if !NETFx
    [Serializable]
#endif
    public class TinyMapperException : Exception
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TinyMapperException()
        {
        }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="message"></param>
        public TinyMapperException(string message) : base(message)
        {
        }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public TinyMapperException(string message, Exception innerException) : base(message, innerException)
        {
        }
#if !NETFx
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected TinyMapperException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
#endif
    }
    /// <summary>
    /// 类型异常
    /// </summary>
    internal static class TypeExtensions
    {
        public static Type GetCollectionItemType(this Type type)
        {
            if (type.IsArray)
            {
                return type.GetElementType();
            }
            if (TinyMapper.IsGenericType(type) && type.IsIEnumerableOf())
            {
                return type.GetGenericArguments().First();
            }

            return typeof(object);
        }

        public static ConstructorInfo GetDefaultCtor(this Type type)
        {
            return type.GetConstructor(Type.EmptyTypes);
        }

        public static KeyValuePair<Type, Type> GetDictionaryItemTypes(this Type type)
        {
            if (type.IsDictionaryOf())
            {
                Type[] types = type.GetGenericArguments();
                return new KeyValuePair<Type, Type>(types[0], types[1]);
            }
            throw new NotSupportedException();
        }

        public static MethodInfo GetGenericMethod(this Type type, string methodName, params Type[] arguments)
        {
            return type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)
                       .MakeGenericMethod(arguments);
        }

        public static bool HasDefaultCtor(this Type type)
        {
            return type.GetConstructor(Type.EmptyTypes) != null;
        }

        public static bool IsDictionaryOf(this Type type)
        {
            return TinyMapper.IsGenericType(type) &&
                   (type.GetGenericTypeDefinition() == typeof(Dictionary<,>) ||
                    type.GetGenericTypeDefinition() == typeof(IDictionary<,>));
        }

        public static bool IsIEnumerable(this Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        public static bool IsIEnumerableOf(this Type type)
        {
            return type.GetInterfaces()
                       .Any(x => TinyMapper.IsGenericType(x) &&
                                 x.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                                 !TinyMapper.IsGenericType(x) && x == typeof(IEnumerable));
        }

        public static bool IsListOf(this Type type)
        {
            return
                TinyMapper.IsGenericType(type) &&
                (type.GetGenericTypeDefinition() == typeof(List<>) ||
                 type.GetGenericTypeDefinition() == typeof(IList<>) ||
                 type.GetGenericTypeDefinition() == typeof(ICollection<>));
        }

        public static bool IsNullable(this Type type)
        {
            return TinyMapper.IsGenericType(type) && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }

    /// <summary>
    ///     https://github.com/Nelibur/Nelibur
    /// </summary>
    internal static class OptionExtensions
    {
        public static Option<T> Do<T>(this Option<T> value, Action<T> action)
        {
            if (value.HasValue)
            {
                action(value.Value);
            }
            return value;
        }

        public static Option<T> DoOnEmpty<T>(this Option<T> value, Action action)
        {
            if (value.HasNoValue)
            {
                action();
            }
            return value;
        }

        public static Option<T> Finally<T>(this Option<T> value, Action<T> action)
        {
            action(value.Value);
            return value;
        }

        public static Option<TResult> Map<TInput, TResult>(this Option<TInput> value, Func<TInput, Option<TResult>> func)
        {
            if (value.HasNoValue)
            {
                return Option<TResult>.Empty;
            }
            return func(value.Value);
        }

        public static Option<TResult> Map<TInput, TResult>(this Option<TInput> value, Func<TInput, TResult> func)
        {
            if (value.HasNoValue)
            {
                return Option<TResult>.Empty;
            }
            return func(value.Value).ToOption();
        }

        public static Option<TResult> Map<TInput, TResult>(this Option<TInput> value, Func<TInput, bool> predicate, Func<TInput, TResult> func)
        {
            if (value.HasNoValue)
            {
                return Option<TResult>.Empty;
            }
            if (!predicate(value.Value))
            {
                return Option<TResult>.Empty;
            }
            return func(value.Value).ToOption();
        }

        public static Option<T> MapOnEmpty<T>(this Option<T> value, Func<T> func)
        {
            if (value.HasNoValue)
            {
                return func().ToOption();
            }
            return value;
        }

        public static Option<V> SelectMany<T, U, V>(this Option<T> value, Func<T, Option<U>> func, Func<T, U, V> selector)
        {
            return value.Map(x => func(x).Map(y => selector(x, y).ToOption()));
        }

        public static Option<T> Where<T>(this Option<T> value, Func<T, bool> predicate)
        {
            if (value.HasNoValue)
            {
                return Option<T>.Empty;
            }
            return predicate(value.Value) ? value : Option<T>.Empty;
        }
    }
    /// <summary>
    ///     https://github.com/Nelibur/Nelibur
    /// </summary>
    internal static class ObjectExtensions
    {
        public static bool IsNotNull(this object obj)
        {
            return obj != null;
        }

        public static bool IsNull(this object obj)
        {
            return obj == null;
        }

        public static Option<T> ToOption<T>(this T value)
        {
            if (TinyMapper.IsValueType(typeof(T)) == false && ReferenceEquals(value, null))
            {
                return Option<T>.Empty;
            }
            return new Option<T>(value);
        }

        public static Option<TResult> ToType<TResult>(this object obj)
        {
            if (obj is TResult)
            {
                return new Option<TResult>((TResult)obj);
            }
            return Option<TResult>.Empty;
        }
    }
}
