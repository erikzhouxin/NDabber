using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Data.Mabber
{
    /// <summary>
    /// TinyMapper is an object to object mapper for .NET. The main advantage is performance.
    /// TinyMapper allows easily map object to object, i.e. properties or fields from one object to another.
    /// </summary>
    public static partial class TinyMapper
    {
        private static readonly Dictionary<TypePair, Mapper> _mappers = new Dictionary<TypePair, Mapper>();
        private static readonly TargetMapperBuilder _targetMapperBuilder;
        private static readonly TinyMapperConfig _config;
        private static readonly object _mappersLock = new object();

        static TinyMapper()
        {
            IDynamicAssembly assembly = DynamicAssemblyBuilder.Get();
            _targetMapperBuilder = new TargetMapperBuilder(assembly);
            _config = new TinyMapperConfig(_targetMapperBuilder);
        }

        /// <summary>
        /// Create a one-way mapping between Source and Target types.
        /// </summary>
        /// <typeparam name="TSource">Source type.</typeparam>
        /// <typeparam name="TTarget">Target type.</typeparam>
        /// <remarks>The method is thread safe.</remarks>
        public static void Bind<TSource, TTarget>()
        {
            TypePair typePair = TypePair.Create<TSource, TTarget>();
            lock (_mappersLock)
            {
                _mappers[typePair] = _targetMapperBuilder.Build(typePair);
            }
        }

        /// <summary>
        /// Create a one-way mapping between Source and Target types.
        /// </summary>
        /// <param name="sourceType">Source type.</param>
        /// <param name="targetType">Target type.</param>
        /// <remarks>The method is thread safe.</remarks>
        public static void Bind(Type sourceType, Type targetType)
        {
            if (sourceType == null)
            {
                throw new ArgumentNullException(nameof(sourceType));
            }
            if (targetType == null)
            {
                throw new ArgumentNullException(nameof(targetType));
            }
            TypePair typePair = TypePair.Create(sourceType, targetType);
            lock (_mappersLock)
            {
                _mappers[typePair] = _targetMapperBuilder.Build(typePair);
            }
        }

        /// <summary>
        /// Create a one-way mapping between Source and Target types.
        /// </summary>
        /// <typeparam name="TSource">Source type.</typeparam>
        /// <typeparam name="TTarget">Target type.</typeparam>
        /// <param name="config">BindingConfig for custom binding.</param>
        /// <remarks>The method is thread safe.</remarks>
        public static void Bind<TSource, TTarget>(Action<IBindingConfig<TSource, TTarget>> config)
        {
            TypePair typePair = TypePair.Create<TSource, TTarget>();

            var bindingConfig = new BindingConfigOf<TSource, TTarget>();
            config(bindingConfig);

            lock (_mappersLock)
            {
                _mappers[typePair] = _targetMapperBuilder.Build(typePair, bindingConfig);
            }
        }

        /// <summary>
        /// Find out if a binding exists from Source to Target.
        /// </summary>
        /// <typeparam name="TSource">Source type.</typeparam>
        /// <typeparam name="TTarget">Target type.</typeparam>
        /// <returns>True if exists, otherwise - False.</returns>
        /// <remarks>The method is thread safe.</remarks>
        public static bool BindingExists<TSource, TTarget>()
        {
            TypePair typePair = TypePair.Create<TSource, TTarget>();
            lock (_mappersLock)
            {
                return _mappers.ContainsKey(typePair);
            }
        }

        /// <summary>
        /// Maps the source to Target type.
        /// The method can be called in parallel to Map methods, but cannot be called in parallel to Bind method.
        /// </summary>
        /// <typeparam name="TSource">Source type.</typeparam>
        /// <typeparam name="TTarget">Target type.</typeparam>
        /// <param name="source">Source object.</param>
        /// <param name="target">Target object.</param>
        /// <returns>Mapped object.</returns>
        public static TTarget Map<TSource, TTarget>(TSource source, TTarget target = default(TTarget))
        {
            TypePair typePair = TypePair.Create<TSource, TTarget>();

            Mapper mapper = GetMapper(typePair);
            var result = (TTarget)mapper.Map(source, target);

            return result;
        }

        /// <summary>
        /// Maps the source to Target type.
        /// The method can be called in parallel to Map methods, but cannot be called in parallel to Bind method.
        /// </summary>
        /// <param name="sourceType">Source type.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="source">Source object.</param>
        /// <param name="target">Target object.</param>
        /// <returns>Mapped object.</returns>
        public static object Map(Type sourceType, Type targetType, object source, object target = null)
        {
            TypePair typePair = TypePair.Create(sourceType, targetType);

            Mapper mapper = GetMapper(typePair);
            var result = mapper.Map(source, target);

            return result;
        }

        /// <summary>
        /// Configure the Mapper.
        /// </summary>
        /// <param name="config">Lambda to provide config settings</param>
        public static void Config(Action<ITinyMapperConfig> config)
        {
            config(_config);
        }

        /// <summary>
        /// Maps the source to Target type.
        /// The method can be called in parallel to Map methods, but cannot be called in parallel to Bind method.
        /// </summary>
        /// <typeparam name="TTarget">Target type.</typeparam>
        /// <param name="source">Source object [Not null].</param>
        /// <returns>Mapped object. The method can be called in parallel to Map methods, but cannot be called in parallel to Bind method.</returns>
        public static TTarget Map<TTarget>(object source)
        {
            if (source.IsNull())
            {
                throw TinyMapper.ArgumentNull("Source cannot be null. Use TinyMapper.Map<TSource, TTarget> method instead.");
            }

            TypePair typePair = TypePair.Create(source.GetType(), typeof(TTarget));

            Mapper mapper = GetMapper(typePair);
            var result = (TTarget)mapper.Map(source);

            return result;
        }

        [SuppressMessage("ReSharper", "All")]
        private static Mapper GetMapper(TypePair typePair)
        {
            Mapper mapper;

            if (_mappers.TryGetValue(typePair, out mapper) == false)
            {
                throw new TinyMapperException($"No binding found for '{typePair.Source.Name}' to '{typePair.Target.Name}'. " +
                                              $"Call TinyMapper.Bind<{typePair.Source.Name}, {typePair.Target.Name}>()");
            }

            //            _mappersLock.EnterUpgradeableReadLock();
            //            try
            //            {
            //                if (_mappers.TryGetValue(typePair, out mapper) == false)
            //                {
            //                    if (_config.EnablePolymorphicMapping && (mapper = GetPolymorphicMapping(typePair)) != null)
            //                    {
            //                        return mapper;
            //                    }
            //                    else if (_config.EnableAutoBinding)
            //                    {
            //                        mapper = _targetMapperBuilder.Build(typePair);
            //                        _mappersLock.EnterWriteLock();
            //                        try
            //                        {
            //                            _mappers[typePair] = mapper;
            //                        }
            //                        finally
            //                        {
            //                            _mappersLock.ExitWriteLock();
            //                        }
            //                    }
            //                    else
            //                    {
            //                        throw new TinyMapperException($"Unable to find a binding for type '{typePair.Source?.Name}' to '{typePair.Target?.Name}'.");
            //                    }
            //                }
            //            }
            //            finally
            //            {
            //                _mappersLock.ExitUpgradeableReadLock();
            //            }

            return mapper;
        }

        //Note: Lock should already be acquired for the mapper
        //        private static Mapper GetPolymorphicMapping(TypePair types)
        //        {
        //            // Walk the polymorphic heirarchy until we find a mapping match
        //            Type source = types.Source;
        //
        //            do
        //            {
        //                Mapper result;
        //                foreach (Type iface in source.GetInterfaces())
        //                {
        //                    if (_mappers.TryGetValue(TypePair.Create(iface, types.Target), out result))
        //                        return result;
        //                }
        //
        //                if (_mappers.TryGetValue(TypePair.Create(source, types.Target), out result))
        //                    return result;
        //            }
        //            while ((source = Helpers.BaseType(source)) != null);
        //
        //            return null;
        //        }
    }
    public static partial class TinyMapper
    {
        #region // EnumerableExtensions
        internal static List<TResult> ConvertAll<TFrom, TResult>(
            this IEnumerable<TFrom> value,
            Func<TFrom, TResult> converter)
        {
            return value.Select(converter).ToList();
        }

        internal static int Count(this IEnumerable source)
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

        internal static bool IsNullOrEmpty<T>(this IEnumerable<T> value)
        {
            return value.IsNull() || !value.Any();
        }

        /// <summary>
        ///     Apply the given function to each element of the collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">Input collection</param>
        /// <param name="action">Given function</param>
        internal static void Iter<T>(this IEnumerable<T> value, Action<T> action)
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
        internal static void IterI<T>(this IEnumerable<T> value, Action<int, T> action)
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
        internal static void IterSafe<T>(
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

        internal static IEnumerable<T> ToValue<T>(this IEnumerable<Option<T>> value)
        {
            return value.Where(x => x.HasValue)
                        .Select(x => x.Value);
        }
        #endregion EnumerableExtensions
        #region // MemberInfoExtensions
        internal static Option<TAttribute> GetAttribute<TAttribute>(this MemberInfo value)
            where TAttribute : Attribute
        {
            return value.GetCustomAttributes(true)
                            .FirstOrDefault(x => x is TAttribute)
                            .ToType<TAttribute>();
        }

        internal static List<TAttribute> GetAttributes<TAttribute>(this MemberInfo value)
            where TAttribute : Attribute
        {
            return value.GetCustomAttributes(true).OfType<TAttribute>().ToList();
        }

        internal static Type GetMemberType(this MemberInfo value)
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

        internal static bool IsField(this MemberInfo value)
        {
#if NETFx
            return value is FieldInfo;
#else
            return value.MemberType == MemberTypes.Field;
#endif
        }

        internal static bool IsProperty(this MemberInfo value)
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
        #endregion MemberInfoExtensions
        #region // DictionaryExtensions
        internal static Option<TValue> GetValue<TKey, TValue>(this IDictionary<TKey, TValue> value, TKey key)
        {
            TValue result;
            bool exists = value.TryGetValue(key, out result);
            return new Option<TValue>(result, exists);
        }
        #endregion DictionaryExtensions
        #region // Error https://github.com/Nelibur/Nelibur
        internal static Exception ArgumentNull(string paramName)
        {
            return new ArgumentNullException(paramName);
        }

        internal static Exception InvalidOperation(string message)
        {
            return new InvalidOperationException(message);
        }

        internal static Exception NotImplemented()
        {
            return new NotImplementedException();
        }

        internal static Exception NotSupported()
        {
            return new NotSupportedException();
        }

        internal static Exception Type<TException>()
            where TException : Exception, new()
        {
            return CreateCtor<TException>()();
        }

        private static Func<T> CreateCtor<T>()
            where T : new()
        {
            return () => new T();
        }
        #endregion Error
        #region // Helpers
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
        #endregion Helpers
    }
    public static partial class TinyMapper
    {
        #region // EmitNewObj
        internal static IEmitterType LoadNewObj(Type objectType)
        {
            return new EmitNewObj(objectType);
        }
        private sealed class EmitNewObj : IEmitterType
        {
            internal EmitNewObj(Type objectType)
            {
                ObjectType = objectType;
            }

            public Type ObjectType { get; }

            public void Emit(CodeGenerator generator)
            {
                ConstructorInfo ctor = ObjectType.GetDefaultCtor();
                generator.EmitNewObject(ctor);
            }
        }
        #endregion EmitNewObj
        #region // EmitNull
        internal static IEmitterType LoadNull()
        {
            return new EmitNull();
        }
        private sealed class EmitNull : IEmitterType
        {
            internal EmitNull()
            {
                ObjectType = typeof(object);
            }
            public Type ObjectType { get; }
            public void Emit(CodeGenerator generator)
            {
                generator.Emit(OpCodes.Ldnull);
            }
        }
        #endregion EmitNull
        #region // EmitLocalVariable
        internal static IEmitterType LoadDeclare(LocalBuilder localBuilder)
        {
            return new EmitLocalVariable(localBuilder);
        }
        private sealed class EmitLocalVariable : IEmitterType
        {
            private readonly Option<LocalBuilder> _localBuilder;

            internal EmitLocalVariable(LocalBuilder localBuilder)
            {
                _localBuilder = localBuilder.ToOption();
                ObjectType = localBuilder.LocalType;
            }

            public Type ObjectType { get; }

            public void Emit(CodeGenerator generator)
            {
                _localBuilder.Where(x => TinyMapper.IsValueType(x.LocalType))
                             .Do(x => generator.Emit(OpCodes.Ldloca, x.LocalIndex))
                             .Do(x => generator.Emit(OpCodes.Initobj, x.LocalType));
            }
        }
        #endregion EmitLocalVariable
        #region // EmitBox
        internal static IEmitterType LoadBox(IEmitterType value)
        {
            return new EmitBox(value);
        }
        private sealed class EmitBox : IEmitterType
        {
            private readonly IEmitterType _value;

            internal EmitBox(IEmitterType value)
            {
                _value = value;
                ObjectType = value.ObjectType;
            }

            public Type ObjectType { get; }

            public void Emit(CodeGenerator generator)
            {
                _value.Emit(generator);

                if (TinyMapper.IsValueType(ObjectType))
                {
                    generator.Emit(OpCodes.Box, ObjectType);
                }
            }
        }
        #endregion EmitBox
        #region // EmitReturn
        internal static IEmitterType LoadReturn(IEmitterType returnValue, Type returnType = null)
        {
            return new EmitReturn(returnValue, returnType);
        }
        private sealed class EmitReturn : IEmitterType
        {
            private readonly IEmitterType _returnValue;

            internal EmitReturn(IEmitterType returnValue, Type returnType)
            {
                ObjectType = returnType ?? returnValue.ObjectType;
                _returnValue = returnValue;
            }

            public Type ObjectType { get; }

            public void Emit(CodeGenerator generator)
            {
                _returnValue.Emit(generator);
                if (ObjectType == _returnValue.ObjectType)
                {
                    generator.Emit(OpCodes.Ret);
                }
                else
                {
                    generator.CastType(_returnValue.ObjectType, ObjectType)
                             .Emit(OpCodes.Ret);
                }
            }
        }
        #endregion EmitReturn
        #region // EmitProperty
        internal static IEmitterType LoadProperty(IEmitterType source, PropertyInfo property)
        {
            return new EmitLoadProperty(source, property);
        }
        internal static IEmitterType StoreProperty(PropertyInfo property, IEmitterType targetObject, IEmitterType value)
        {
            return new EmitStoreProperty(property, targetObject, value);
        }
        private sealed class EmitLoadProperty : IEmitterType
        {
            private readonly PropertyInfo _property;
            private readonly IEmitterType _source;

            internal EmitLoadProperty(IEmitterType source, PropertyInfo property)
            {
                _source = source;
                _property = property;
                ObjectType = property.PropertyType;
            }

            public Type ObjectType { get; }

            public void Emit(CodeGenerator generator)
            {
                MethodInfo method = _property.GetGetMethod();
                TinyMapper.Call(method, _source, null).Emit(generator);
            }
        }
        private sealed class EmitStoreProperty : IEmitterType
        {
            private readonly IEmitterType _callMethod;

            internal EmitStoreProperty(PropertyInfo property, IEmitterType targetObject, IEmitterType value)
            {
                MethodInfo method = property.GetSetMethod();
                _callMethod = TinyMapper.Call(method, targetObject, value);
                ObjectType = _callMethod.ObjectType;
            }

            public Type ObjectType { get; }

            public void Emit(CodeGenerator generator)
            {
                _callMethod.Emit(generator);
            }
        }
        #endregion EmitProperty
        #region // EmitThis
        internal static IEmitterType LoadThis(Type thisType)
        {
            return TinyMapper.LoadArgument(thisType, 0);
        }
        #endregion EmitThis
        #region // EmitArgument
        internal static IEmitterType LoadArgument(Type type, int index)
        {
            return new EmitLoadArgument(type, index);
        }
        private sealed class EmitLoadArgument : IEmitterType
        {
            private readonly int _index;

            internal EmitLoadArgument(Type type, int index)
            {
                ObjectType = type;
                _index = index;
            }

            public Type ObjectType { get; }

            public void Emit(CodeGenerator generator)
            {
                switch (_index)
                {
                    case 0:
                        generator.Emit(OpCodes.Ldarg_0);
                        break;
                    case 1:
                        generator.Emit(OpCodes.Ldarg_1);
                        break;
                    case 2:
                        generator.Emit(OpCodes.Ldarg_2);
                        break;
                    case 3:
                        generator.Emit(OpCodes.Ldarg_3);
                        break;
                    default:
                        generator.Emit(OpCodes.Ldarg, _index);
                        break;
                }
            }
        }
        #endregion EmitArgument
        #region // EmitArray
        internal static IEmitterType LoadArray(IEmitterType array, int index)
        {
            return new EmitLoadArray(array, index);
        }

        private sealed class EmitLoadArray : IEmitterType
        {
            private readonly IEmitterType _array;
            private readonly int _index;

            internal EmitLoadArray(IEmitterType array, int index)
            {
                _array = array;
                _index = index;
                ObjectType = array.ObjectType.GetElementType();
            }

            public Type ObjectType { get; }

            public void Emit(CodeGenerator generator)
            {
                _array.Emit(generator);
                switch (_index)
                {
                    case 0:
                        generator.Emit(OpCodes.Ldc_I4_0);
                        break;
                    case 1:
                        generator.Emit(OpCodes.Ldc_I4_1);
                        break;
                    case 2:
                        generator.Emit(OpCodes.Ldc_I4_2);
                        break;
                    case 3:
                        generator.Emit(OpCodes.Ldc_I4_3);
                        break;
                    default:
                        generator.Emit(OpCodes.Ldc_I4, _index);
                        break;
                }
                generator.Emit(OpCodes.Ldelem, ObjectType);
            }
        }
        #endregion EmitArray
        #region // EmitField
        internal static IEmitterType LoadField(IEmitterType source, FieldInfo field)
        {
            return new EmitLoadField(source, field);
        }
        internal static IEmitterType StoreField(FieldInfo field, IEmitterType targetObject, IEmitterType value)
        {
            return new EmitStoreField(field, targetObject, value);
        }
        private sealed class EmitLoadField : IEmitterType
        {
            private readonly FieldInfo _field;
            private readonly IEmitterType _source;

            internal EmitLoadField(IEmitterType source, FieldInfo field)
            {
                _source = source;
                _field = field;
                ObjectType = field.FieldType;
            }

            public Type ObjectType { get; }

            public void Emit(CodeGenerator generator)
            {
                _source.Emit(generator);
                generator.Emit(OpCodes.Ldfld, _field);
            }
        }
        private sealed class EmitStoreField : IEmitterType
        {
            private readonly FieldInfo _field;
            private readonly IEmitterType _targetObject;
            private readonly IEmitterType _value;

            internal EmitStoreField(FieldInfo field, IEmitterType targetObject, IEmitterType value)
            {
                _field = field;
                _targetObject = targetObject;
                _value = value;
                ObjectType = _field.FieldType;
            }

            public Type ObjectType { get; }

            public void Emit(CodeGenerator generator)
            {
                _targetObject.Emit(generator);
                _value.Emit(generator);
                generator.CastType(_value.ObjectType, _field.FieldType);
                generator.Emit(OpCodes.Stfld, _field);
            }
        }
        #endregion EmitField
        #region // EmitLocal
        internal static IEmitterType LoadLocal(LocalBuilder localBuilder)
        {
            return new EmitLoadLocal(localBuilder);
        }
        private sealed class EmitLoadLocal : IEmitterType
        {
            private readonly LocalBuilder _localBuilder;

            internal EmitLoadLocal(LocalBuilder localBuilder)
            {
                _localBuilder = localBuilder;
                ObjectType = localBuilder.LocalType;
            }

            public Type ObjectType { get; }

            public void Emit(CodeGenerator generator)
            {
                switch (_localBuilder.LocalIndex)
                {
                    case 0:
                        generator.Emit(OpCodes.Ldloc_0);
                        break;
                    case 1:
                        generator.Emit(OpCodes.Ldloc_1);
                        break;
                    case 2:
                        generator.Emit(OpCodes.Ldloc_2);
                        break;
                    case 3:
                        generator.Emit(OpCodes.Ldloc_3);
                        break;
                    default:
                        generator.Emit(OpCodes.Ldloc, _localBuilder.LocalIndex);
                        break;
                }
            }
        }
        #endregion EmitLocal
        #region // EmitMethod
        internal static IEmitterType Call(MethodInfo method, IEmitterType invocationObject, params IEmitterType[] arguments)
        {
            return new EmitterCallMethod(method, invocationObject, arguments);
        }
        internal static IEmitterType CallStatic(MethodInfo method, params IEmitterType[] arguments)
        {
            return new EmitterCallMethod(method, null, arguments);
        }
        private sealed class EmitterCallMethod : IEmitterType
        {
            private readonly IEmitterType[] _arguments;
            private readonly IEmitterType _invocationObject;
            private readonly MethodInfo _method;

            internal EmitterCallMethod(MethodInfo method, IEmitterType invocationObject, params IEmitterType[] arguments)
            {
                _method = method;
                _invocationObject = invocationObject;
                _arguments = arguments;
                ObjectType = _method.ReturnType;
            }

            public Type ObjectType { get; }

            public void Emit(CodeGenerator generator)
            {
                generator.EmitCall(_method, _invocationObject, _arguments);
            }
        }
        #endregion EmitMethod
    }
}