using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace System.Data.Mabber
{
    internal sealed class ClassMapperBuilder : MapperBuilder
    {
        private readonly MapperCache _mapperCache;
        private const string CreateTargetInstanceMethod = "CreateTargetInstance";
        private const string MapClassMethod = "MapClass";
        private readonly MappingMemberBuilder _mappingMemberBuilder;
        private readonly MemberMapper _memberMapper;

        public ClassMapperBuilder(MapperCache mapperCache, IMapperBuilderConfig config) : base(config)
        {
            _mapperCache = mapperCache;
            _memberMapper = new MemberMapper(mapperCache, config);
            _mappingMemberBuilder = new MappingMemberBuilder(config);
        }

        protected override string ScopeName => "ClassMappers";

        protected override Mapper BuildCore(TypePair typePair)
        {
            Type parentType = typeof(ClassMapper<,>).MakeGenericType(typePair.Source, typePair.Target);
            TypeBuilder typeBuilder = _assembly.DefineType(GetMapperFullName(), parentType);
            EmitCreateTargetInstance(typePair.Target, typeBuilder);

            MapperCacheItem rootMapperCacheItem = _mapperCache.AddStub(typePair);
            Option<MapperCache> mappers = EmitMapClass(typePair, typeBuilder);

            var rootMapper = (Mapper)Activator.CreateInstance(Helpers.CreateType(typeBuilder));

            UpdateMappers(mappers, rootMapperCacheItem.Id, rootMapper);

            _mapperCache.ReplaceStub(typePair, rootMapper);

            mappers.Do(x => rootMapper.AddMappers(x.Mappers));

            return rootMapper;
        }

        private static void UpdateMappers(Option<MapperCache> mappers, int rootMapperId, Mapper rootMapper)
        {
            if (mappers.HasValue)
            {
                var result = new List<Mapper>();
                foreach (var item in mappers.Value.MapperCacheItems)
                {
                    if (item.Id != rootMapperId)
                    {
                        result.Add(item.Mapper);
                    }
                    else
                    {
                        result.Add(null);
                    }
                }
                result[rootMapperId] = rootMapper;
                rootMapper.AddMappers(result);
                foreach (var item in mappers.Value.MapperCacheItems)
                {
                    if (item.Id == rootMapperId)
                    {
                        continue;
                    }
                    item.Mapper?.UpdateRootMapper(rootMapperId, rootMapper);
                }
            }
        }

        protected override Mapper BuildCore(TypePair parentTypePair, MappingMember mappingMember)
        {
            return BuildCore(mappingMember.TypePair);
        }

        protected override bool IsSupportedCore(TypePair typePair)
        {
            return true;
        }

        private static void EmitCreateTargetInstance(Type targetType, TypeBuilder typeBuilder)
        {
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(CreateTargetInstanceMethod, OverrideProtected, targetType, Type.EmptyTypes);
            var codeGenerator = new CodeGenerator(methodBuilder.GetILGenerator());

            IEmitterType result = Helpers.IsValueType(targetType) ? EmitValueType(targetType, codeGenerator) : EmitRefType(targetType);

            EmitReturn.Return(result, targetType).Emit(codeGenerator);
        }

        private static IEmitterType EmitRefType(Type type)
        {
            return type.HasDefaultCtor() ? EmitNewObj.NewObj(type) : EmitNull.Load();
        }

        private static IEmitterType EmitValueType(Type type, CodeGenerator codeGenerator)
        {
            LocalBuilder builder = codeGenerator.DeclareLocal(type);
            EmitLocalVariable.Declare(builder).Emit(codeGenerator);
            return EmitBox.Box(EmitLocal.Load(builder));
        }

        private Option<MapperCache> EmitMapClass(TypePair typePair, TypeBuilder typeBuilder)
        {
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(MapClassMethod,
                OverrideProtected,
                typePair.Target,
                new[] { typePair.Source, typePair.Target });
            var codeGenerator = new CodeGenerator(methodBuilder.GetILGenerator());

            var emitterComposite = new EmitComposite();

            MemberEmitterDescription emitterDescription = EmitMappingMembers(typePair);

            emitterComposite.Add(emitterDescription.Emitter);
            emitterComposite.Add(EmitReturn.Return(EmitArgument.Load(typePair.Target, 2)));
            emitterComposite.Emit(codeGenerator);
            return emitterDescription.MapperCache;
        }

        private MemberEmitterDescription EmitMappingMembers(TypePair typePair)
        {
            List<MappingMemberPath> members = _mappingMemberBuilder.Build(typePair);
            MemberEmitterDescription result = _memberMapper.Build(typePair, members);
            return result;
        }
    }
    internal abstract class ClassMapper<TSource, TTarget> : MapperOf<TSource, TTarget>
    {
        protected virtual TTarget CreateTargetInstance()
        {
            throw new NotImplementedException();
        }

        protected abstract TTarget MapClass(TSource source, TTarget target);

        protected override TTarget MapCore(TSource source, TTarget target)
        {
            if (target == null)
            {
                target = CreateTargetInstance();
            }
            TTarget result = MapClass(source, target);
            return result;
        }
    }
    internal sealed class CodeGenerator
    {
        private readonly ILGenerator _ilGenerator;

        public CodeGenerator(ILGenerator ilGenerator)
        {
            _ilGenerator = ilGenerator;
        }

        public CodeGenerator CastType(Type stackType, Type targetType)
        {
            if (stackType == targetType)
            {
                return this;
            }
            if (Helpers.IsValueType(stackType) == false && targetType == typeof(object))
            {
                return this;
            }
            if (Helpers.IsValueType(stackType) && !Helpers.IsValueType(targetType))
            {
                _ilGenerator.Emit(OpCodes.Box, stackType);
            }
            else if (!Helpers.IsValueType(stackType) && Helpers.IsValueType(targetType))
            {
                _ilGenerator.Emit(OpCodes.Unbox_Any, targetType);
            }
            else
            {
                _ilGenerator.Emit(OpCodes.Castclass, targetType);
            }
            return this;
        }

        public LocalBuilder DeclareLocal(Type type)
        {
            return _ilGenerator.DeclareLocal(type);
        }

        public void Emit(OpCode opCode)
        {
            _ilGenerator.Emit(opCode);
        }

        public void Emit(OpCode opCode, int value)
        {
            _ilGenerator.Emit(opCode, value);
        }

        public void Emit(OpCode opCode, Type value)
        {
            _ilGenerator.Emit(opCode, value);
        }

        public void Emit(OpCode opCode, FieldInfo value)
        {
            _ilGenerator.Emit(opCode, value);
        }

        public void EmitCall(MethodInfo method, IEmitterType invocationObject, params IEmitterType[] arguments)
        {
            ParameterInfo[] actualArguments = method.GetParameters();
            if (arguments.IsNull())
            {
                arguments = new IEmitterType[0];
            }
            if (arguments.Length != actualArguments.Length)
            {
                throw new ArgumentException();
            }

            if (invocationObject.IsNotNull())
            {
                invocationObject.Emit(this);
            }

            for (var i = 0; i < arguments.Length; i++)
            {
                arguments[i].Emit(this);
                CastType(arguments[i].ObjectType, actualArguments[i].ParameterType);
            }
            EmitCall(method);
        }

        public void EmitNewObject(ConstructorInfo ctor)
        {
            _ilGenerator.Emit(OpCodes.Newobj, ctor);
        }

        private void EmitCall(MethodInfo method)
        {
            _ilGenerator.Emit(method.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, method);
        }
    }
    internal sealed class CollectionMapperBuilder : MapperBuilder
    {
        private readonly MapperCache _mapperCache;
        private const string ConvertItemKeyMethod = "ConvertItemKey";
        private const string ConvertItemMethod = "ConvertItem";
        private const string DictionaryToDictionaryMethod = "DictionaryToDictionary";
        private const string DictionaryToDictionaryTemplateMethod = "DictionaryToDictionaryTemplate";
        private const string EnumerableToArrayMethod = "EnumerableToArray";
        private const string EnumerableToArrayTemplateMethod = "EnumerableToArrayTemplate";
        private const string EnumerableToListMethod = "EnumerableToList";
        private const string EnumerableToListTemplateMethod = "EnumerableToListTemplate";
        private const string EnumerableOfDeepCloneableToListTemplateMethod = "EnumerableOfDeepCloneableToListTemplate";

        public CollectionMapperBuilder(MapperCache mapperCache, IMapperBuilderConfig config) : base(config)
        {
            _mapperCache = mapperCache;
        }

        protected override string ScopeName => "CollectionMappers";

        protected override Mapper BuildCore(TypePair typePair)
        {
            Type parentType = typeof(CollectionMapper<,>).MakeGenericType(typePair.Source, typePair.Target);
            TypeBuilder typeBuilder = _assembly.DefineType(GetMapperFullName(), parentType);

            _mapperCache.AddStub(typePair);

            if (IsIEnumerableToList(typePair))
            {
                EmitEnumerableToList(parentType, typeBuilder, typePair);
            }
            else if (IsIEnumerableToArray(typePair))
            {
                EmitEnumerableToArray(parentType, typeBuilder, typePair);
            }
            else if (IsDictionaryToDictionary(typePair))
            {
                EmitDictionaryToDictionary(parentType, typeBuilder, typePair);
            }
            else if (IsEnumerableToEnumerable(typePair))
            {
                EmitEnumerableToEnumerable(parentType, typeBuilder, typePair);
            }

            var rootMapper = (Mapper)Activator.CreateInstance(Helpers.CreateType(typeBuilder));

            _mapperCache.ReplaceStub(typePair, rootMapper);
            rootMapper.AddMappers(_mapperCache.Mappers);

            return rootMapper;
        }

        protected override Mapper BuildCore(TypePair parentTypePair, MappingMember mappingMember)
        {
            return BuildCore(mappingMember.TypePair);
        }

        protected override bool IsSupportedCore(TypePair typePair)
        {
            return typePair.IsEnumerableTypes;
        }

        private static bool IsDictionaryToDictionary(TypePair typePair)
        {
            return typePair.Source.IsDictionaryOf() && typePair.Target.IsDictionaryOf();
        }

        private static bool IsIEnumerableToArray(TypePair typePair)
        {
            return typePair.Source.IsIEnumerable() && typePair.Target.IsArray;
        }

        private static bool IsIEnumerableToList(TypePair typePair)
        {
            return typePair.Source.IsIEnumerable() && typePair.Target.IsListOf();
        }

        private bool IsEnumerableToEnumerable(TypePair typePair)
        {
            return typePair.Source.IsIEnumerable() && typePair.Target.IsIEnumerable();
        }

        private MapperCacheItem CreateMapperCacheItem(TypePair typePair)
        {
            var mapperCacheItemOption = _mapperCache.Get(typePair);
            if (mapperCacheItemOption.HasValue)
            {
                return mapperCacheItemOption.Value;
            }

            MapperBuilder mapperBuilder = GetMapperBuilder(typePair);
            Mapper mapper = mapperBuilder.Build(typePair);
            MapperCacheItem mapperCacheItem = _mapperCache.Add(typePair, mapper);
            return mapperCacheItem;
        }

        private void EmitConvertItem(TypeBuilder typeBuilder, TypePair typePair, string methodName = ConvertItemMethod)
        {
            MapperCacheItem mapperCacheItem = CreateMapperCacheItem(typePair);

            MethodBuilder methodBuilder = typeBuilder.DefineMethod(methodName, OverrideProtected, typeof(object), new[] { typeof(object) });

            IEmitterType sourceMemeber = EmitArgument.Load(typeof(object), 1);
            IEmitterType targetMember = EmitNull.Load();

            IEmitterType callMapMethod = mapperCacheItem.EmitMapMethod(sourceMemeber, targetMember);

            EmitReturn.Return(callMapMethod).Emit(new CodeGenerator(methodBuilder.GetILGenerator()));
        }

        private void EmitDictionaryToDictionary(Type parentType, TypeBuilder typeBuilder, TypePair typePair)
        {
            EmitDictionaryToTarget(parentType, typeBuilder, typePair, DictionaryToDictionaryMethod, DictionaryToDictionaryTemplateMethod);
        }

        private void EmitDictionaryToTarget(
            Type parentType,
            TypeBuilder typeBuilder,
            TypePair typePair,
            string methodName,
            string templateMethodName)
        {
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(methodName, OverrideProtected, typePair.Target, new[] { typeof(IEnumerable) });

            KeyValuePair<Type, Type> sourceTypes = typePair.Source.GetDictionaryItemTypes();
            KeyValuePair<Type, Type> targetTypes = typePair.Target.GetDictionaryItemTypes();

            EmitConvertItem(typeBuilder, new TypePair(sourceTypes.Key, targetTypes.Key), ConvertItemKeyMethod);
            EmitConvertItem(typeBuilder, new TypePair(sourceTypes.Value, targetTypes.Value));

            var arguments = new[] { sourceTypes.Key, sourceTypes.Value, targetTypes.Key, targetTypes.Value };
            MethodInfo methodTemplate = parentType.GetGenericMethod(templateMethodName, arguments);

            IEmitterType returnValue = EmitMethod.Call(methodTemplate, EmitThis.Load(parentType), EmitArgument.Load(typeof(IEnumerable), 1));
            EmitReturn.Return(returnValue).Emit(new CodeGenerator(methodBuilder.GetILGenerator()));
        }

        private void EmitEnumerableToArray(Type parentType, TypeBuilder typeBuilder, TypePair typePair)
        {
            var collectionItemTypePair = GetCollectionItemTypePair(typePair);

            EmitEnumerableToTarget(parentType, typeBuilder, typePair, collectionItemTypePair, EnumerableToArrayMethod, EnumerableToArrayTemplateMethod);
        }

        private void EmitEnumerableToList(Type parentType, TypeBuilder typeBuilder, TypePair typePair)
        {
            var collectionItemTypePair = GetCollectionItemTypePair(typePair);
            var templateMethod = collectionItemTypePair.IsDeepCloneable ? EnumerableOfDeepCloneableToListTemplateMethod : EnumerableToListTemplateMethod;

            EmitEnumerableToTarget(parentType, typeBuilder, typePair, collectionItemTypePair, EnumerableToListMethod, templateMethod);
        }

        private void EmitEnumerableToEnumerable(Type parentType, TypeBuilder typeBuilder, TypePair typePair)
        {
            var collectionItemTypePair = GetCollectionItemTypePair(typePair);
            var templateMethod = collectionItemTypePair.IsDeepCloneable ? EnumerableOfDeepCloneableToListTemplateMethod : EnumerableToListTemplateMethod;

            EmitEnumerableToTarget(parentType, typeBuilder, typePair, collectionItemTypePair, EnumerableToListMethod, templateMethod);
        }

        private static TypePair GetCollectionItemTypePair(TypePair typePair)
        {
            Type sourceItemType = typePair.Source.GetCollectionItemType();
            Type targetItemType = typePair.Target.GetCollectionItemType();

            return new TypePair(sourceItemType, targetItemType);
        }

        private void EmitEnumerableToTarget(
            Type parentType,
            TypeBuilder typeBuilder,
            TypePair typePair,
            TypePair collectionItemTypePair,
            string methodName,
            string templateMethodName)
        {
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(methodName, OverrideProtected, typePair.Target, new[] { typeof(IEnumerable) });

            EmitConvertItem(typeBuilder, collectionItemTypePair);

            MethodInfo methodTemplate = parentType.GetGenericMethod(templateMethodName, collectionItemTypePair.Target);

            IEmitterType returnValue = EmitMethod.Call(methodTemplate, EmitThis.Load(parentType), EmitArgument.Load(typeof(IEnumerable), 1));
            EmitReturn.Return(returnValue).Emit(new CodeGenerator(methodBuilder.GetILGenerator()));
        }
    }
    internal abstract class CollectionMapper<TSource, TTarget> : MapperOf<TSource, TTarget>
        where TTarget : class
    {
        protected virtual object ConvertItem(object item)
        {
            throw new NotImplementedException();
        }

        protected virtual object ConvertItemKey(object item)
        {
            throw new NotImplementedException();
        }

        protected virtual TTarget DictionaryToDictionary(IEnumerable source)
        {
            throw new NotImplementedException();
        }

        protected Dictionary<TTargetKey, TTargetValue> DictionaryToDictionaryTemplate<TSourceKey, TSourceValue, TTargetKey, TTargetValue>(IEnumerable source)
        {
            var result = new Dictionary<TTargetKey, TTargetValue>();
            foreach (KeyValuePair<TSourceKey, TSourceValue> item in source)
            {
                var key = (TTargetKey)ConvertItemKey(item.Key);
                var value = (TTargetValue)ConvertItem(item.Value);
                result.Add(key, value);
            }
            return result;
        }

        protected virtual TTarget EnumerableToArray(IEnumerable source)
        {
            throw new NotImplementedException();
        }

        protected Array EnumerableToArrayTemplate<TTargetItem>(IEnumerable source)
        {
            var result = new TTargetItem[source.Count()];
            int index = 0;
            foreach (var item in source)
            {
                result[index++] = (TTargetItem)ConvertItem(item);
            }
            return result;
        }

        protected virtual TTarget EnumerableToList(IEnumerable source)
        {
            throw new NotImplementedException();
        }

        protected virtual TTarget EnumerableToArrayList(IEnumerable source)
        {
            var result = new ArrayList();

            foreach (var item in source)
            {
                result.Add(ConvertItem(item));
            }

            return result as TTarget;
        }

        protected List<TTargetItem> EnumerableToListTemplate<TTargetItem>(IEnumerable source)
        {
            var result = new List<TTargetItem>();
            foreach (var item in source)
            {
                result.Add((TTargetItem)ConvertItem(item));
            }
            return result;
        }

        protected List<TTargetItem> EnumerableOfDeepCloneableToListTemplate<TTargetItem>(IEnumerable source)
        {
            var result = new List<TTargetItem>();
            result.AddRange((IEnumerable<TTargetItem>)source);
            return result;
        }

        protected virtual TTarget EnumerableToEnumerable(IEnumerable source)
        {
            IList result = null;
            foreach (var item in source)
            {
                if (result == null)
                {
                    result = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(item.GetType()));
                }

                result.Add(ConvertItem(item));
            }
            return result as TTarget;
        }

        protected override TTarget MapCore(TSource source, TTarget target)
        {
            Type targetType = typeof(TTarget);
            var enumerable = (IEnumerable)source;

            if (targetType.IsListOf())
            {
                return EnumerableToList(enumerable);
            }
            if (targetType.IsArray)
            {
                return EnumerableToArray(enumerable);
            }
            if (typeof(TSource).IsDictionaryOf() && targetType.IsDictionaryOf())
            {
                return DictionaryToDictionary(enumerable);
            }
            if (targetType == typeof(ArrayList))
            {
                return EnumerableToArrayList(enumerable);
            }
            if (targetType.IsIEnumerable())
            {
                // Default Case
                return EnumerableToEnumerable(enumerable);
            }

            throw new NotSupportedException($"Not suppoerted From {typeof(TSource).Name} To {targetType.Name}");
        }
    }
    internal sealed class ConvertibleTypeMapperBuilder : MapperBuilder
    {
        private static readonly Func<object, object> _nothingConverter = x => x;

        public ConvertibleTypeMapperBuilder(IMapperBuilderConfig config) : base(config)
        {
        }

        protected override string ScopeName => "ConvertibleTypeMappers";

        protected override Mapper BuildCore(TypePair typePair)
        {
            Func<object, object> converter = GetConverter(typePair);
            return new ConvertibleTypeMapper(converter);
        }

        protected override Mapper BuildCore(TypePair parentTypePair, MappingMember mappingMember)
        {
            return BuildCore(mappingMember.TypePair);
        }

        protected override bool IsSupportedCore(TypePair typePair)
        {
            return IsSupportedType(typePair.Source) || typePair.HasTypeConverter();
        }

        private static Option<Func<object, object>> ConvertEnum(TypePair pair)
        {
            Func<object, object> result;
            if (pair.IsEnumTypes)
            {
                result = x => Convert.ChangeType(x, pair.Source);
                return result.ToOption();
            }

            if (Helpers.IsEnum(pair.Target))
            {
                if (Helpers.IsEnum(pair.Source) == false)
                {
                    if (pair.Source == typeof(string))
                    {
                        result = x => Enum.Parse(pair.Target, x.ToString());
                        return result.ToOption();
                    }
                }
                result = x => Enum.ToObject(pair.Target, Convert.ChangeType(x, Enum.GetUnderlyingType(pair.Target)));
                return result.ToOption();
            }

            if (Helpers.IsEnum(pair.Source))
            {
                result = x => Convert.ChangeType(x, pair.Target);
                return result.ToOption();
            }
            return Option<Func<object, object>>.Empty;
        }

        private static Func<object, object> GetConverter(TypePair pair)
        {
            if (pair.IsDeepCloneable)
            {
                return _nothingConverter;
            }

            TypeConverter fromConverter = TypeDescriptor.GetConverter(pair.Source);
            if (fromConverter.CanConvertTo(pair.Target))
            {
                return x => fromConverter.ConvertTo(x, pair.Target);
            }

            TypeConverter toConverter = TypeDescriptor.GetConverter(pair.Target);
            if (toConverter.CanConvertFrom(pair.Source))
            {
                return x => toConverter.ConvertFrom(x);
            }

            Option<Func<object, object>> enumConverter = ConvertEnum(pair);
            if (enumConverter.HasValue)
            {
                return enumConverter.Value;
            }

            if (pair.IsNullableToNotNullable)
            {
                return GetConverter(new TypePair(Nullable.GetUnderlyingType(pair.Source), pair.Target));
            }

            if (pair.Target.IsNullable())
            {
                return GetConverter(new TypePair(pair.Source, Nullable.GetUnderlyingType(pair.Target)));
            }

            return null;
        }

        private bool IsSupportedType(Type value)
        {
            return Helpers.IsPrimitive(value)
                   || value == typeof(string)
                   || value == typeof(Guid)
                   || Helpers.IsEnum(value)
                   || value == typeof(decimal)
                   || value.IsNullable() && IsSupportedType(Nullable.GetUnderlyingType(value));
        }
    }
    internal sealed class ConvertibleTypeMapper : Mapper
    {
        private readonly Func<object, object> _converter;

        public ConvertibleTypeMapper(Func<object, object> converter)
        {
            _converter = converter;
        }

        protected override object MapCore(object source, object target)
        {
            if (_converter == null)
            {
                return source;
            }
            if (source == null)
            {
                return target;
            }
            return _converter(source);
        }
    }
    internal sealed class CustomTypeMapperBuilder : MapperBuilder
    {
        public CustomTypeMapperBuilder(IMapperBuilderConfig config) : base(config)
        {
        }

        protected override string ScopeName => "CustomTypeMapper";

        public bool IsSupported(TypePair parentTypePair, MappingMember mappingMember)
        {
            Option<BindingConfig> bindingConfig = _config.GetBindingConfig(parentTypePair);
            if (bindingConfig.HasNoValue)
            {
                return false;
            }
            return bindingConfig.Value.HasCustomTypeConverter(mappingMember.Target.Name);
        }

        protected override Mapper BuildCore(TypePair typePair)
        {
            throw new NotSupportedException();
        }

        protected override Mapper BuildCore(TypePair parentTypePair, MappingMember mappingMember)
        {
            Option<BindingConfig> bindingConfig = _config.GetBindingConfig(parentTypePair);
            Func<object, object> converter = bindingConfig.Value.GetCustomTypeConverter(mappingMember.Target.Name).Value;
            return new CustomTypeMapper(converter);
        }

        protected override bool IsSupportedCore(TypePair typePair)
        {
            throw new NotSupportedException();
        }
    }
    internal sealed class CustomTypeMapper : Mapper
    {
        private readonly Func<object, object> _converter;

        public CustomTypeMapper(Func<object, object> converter)
        {
            _converter = converter;
        }

        protected override object MapCore(object source, object target)
        {
            if (_converter == null)
            {
                return source;
            }
            return _converter(source);
        }
    }
    internal class DynamicAssemblyBuilder
    {
        internal const string AssemblyName = "DynamicTinyMapper";
#if !NETFx
        //        private const string AssemblyNameFileName = AssemblyName + ".dll";
        //        private static AssemblyBuilder _assemblyBuilder;
#endif
        private static readonly DynamicAssembly _dynamicAssembly = new DynamicAssembly();

        public static IDynamicAssembly Get()
        {
            return _dynamicAssembly;
        }


        private sealed class DynamicAssembly : IDynamicAssembly
        {
            private readonly ModuleBuilder _moduleBuilder;

            public DynamicAssembly()
            {
                var assemblyName = new AssemblyName(AssemblyName);

#if NETFx
                AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
                _moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);

#else
                AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
                //                        _assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);

                _moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);
                //                        _moduleBuilder = _assemblyBuilder.DefineDynamicModule(assemblyName.Name, AssemblyNameFileName, true);
#endif

            }

            public TypeBuilder DefineType(string typeName, Type parentType)
            {
                return _moduleBuilder.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Sealed, parentType, null);
            }

            public void Save()
            {
#if !NETFx
                //                _assemblyBuilder.Save(AssemblyNameFileName);
#endif
            }
        }
    }
    internal static class EmitArgument
    {
        public static IEmitterType Load(Type type, int index)
        {
            var result = new EmitLoadArgument(type, index);
            return result;
        }


        private sealed class EmitLoadArgument : IEmitterType
        {
            private readonly int _index;

            public EmitLoadArgument(Type type, int index)
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
    }
    internal static class EmitArray
    {
        public static IEmitterType Load(IEmitterType array, int index)
        {
            return new EmitLoadArray(array, index);
        }


        private sealed class EmitLoadArray : IEmitterType
        {
            private readonly IEmitterType _array;
            private readonly int _index;

            public EmitLoadArray(IEmitterType array, int index)
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
    }
    internal sealed class EmitComposite : IEmitter
    {
        private readonly List<IEmitter> _nodes = new List<IEmitter>();

        public void Emit(CodeGenerator generator)
        {
            _nodes.ForEach(x => x.Emit(generator));
        }

        public EmitComposite Add(IEmitter node)
        {
            if (node.IsNotNull())
            {
                _nodes.Add(node);
            }
            return this;
        }
    }
    internal sealed class EmitBox : IEmitterType
    {
        private readonly IEmitterType _value;

        private EmitBox(IEmitterType value)
        {
            _value = value;
            ObjectType = value.ObjectType;
        }

        public Type ObjectType { get; }

        public void Emit(CodeGenerator generator)
        {
            _value.Emit(generator);

            if (Helpers.IsValueType(ObjectType))
            {
                generator.Emit(OpCodes.Box, ObjectType);
            }
        }

        public static IEmitterType Box(IEmitterType value)
        {
            return new EmitBox(value);
        }
    }
    internal static class EmitField
    {
        public static IEmitterType Load(IEmitterType source, FieldInfo field)
        {
            var result = new EmitLoadField(source, field);
            return result;
        }

        public static IEmitterType Store(FieldInfo field, IEmitterType targetObject, IEmitterType value)
        {
            return new EmitStoreField(field, targetObject, value);
        }


        private sealed class EmitLoadField : IEmitterType
        {
            private readonly FieldInfo _field;
            private readonly IEmitterType _source;

            public EmitLoadField(IEmitterType source, FieldInfo field)
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

            public EmitStoreField(FieldInfo field, IEmitterType targetObject, IEmitterType value)
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
    }
    internal static class EmitLocal
    {
        public static IEmitterType Load(LocalBuilder localBuilder)
        {
            var result = new EmitLoadLocal(localBuilder);
            return result;
        }


        private sealed class EmitLoadLocal : IEmitterType
        {
            private readonly LocalBuilder _localBuilder;

            public EmitLoadLocal(LocalBuilder localBuilder)
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
    }
    internal sealed class EmitLocalVariable : IEmitterType
    {
        private readonly Option<LocalBuilder> _localBuilder;

        private EmitLocalVariable(LocalBuilder localBuilder)
        {
            _localBuilder = localBuilder.ToOption();
            ObjectType = localBuilder.LocalType;
        }

        public Type ObjectType { get; }

        public void Emit(CodeGenerator generator)
        {
            _localBuilder.Where(x => Helpers.IsValueType(x.LocalType))
                         .Do(x => generator.Emit(OpCodes.Ldloca, x.LocalIndex))
                         .Do(x => generator.Emit(OpCodes.Initobj, x.LocalType));
        }

        public static IEmitterType Declare(LocalBuilder localBuilder)
        {
            return new EmitLocalVariable(localBuilder);
        }
    }
    internal static class EmitMethod
    {
        public static IEmitterType Call(MethodInfo method, IEmitterType invocationObject, params IEmitterType[] arguments)
        {
            return new EmitterCallMethod(method, invocationObject, arguments);
        }

        public static IEmitterType CallStatic(MethodInfo method, params IEmitterType[] arguments)
        {
            return new EmitterCallMethod(method, null, arguments);
        }


        private sealed class EmitterCallMethod : IEmitterType
        {
            private readonly IEmitterType[] _arguments;
            private readonly IEmitterType _invocationObject;
            private readonly MethodInfo _method;

            public EmitterCallMethod(MethodInfo method, IEmitterType invocationObject, params IEmitterType[] arguments)
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
    }
    internal sealed class EmitNull : IEmitterType
    {
        private EmitNull()
        {
            ObjectType = typeof(object);
        }

        public Type ObjectType { get; }

        public void Emit(CodeGenerator generator)
        {
            generator.Emit(OpCodes.Ldnull);
        }

        public static IEmitterType Load()
        {
            return new EmitNull();
        }
    }
    internal sealed class EmitNewObj : IEmitterType
    {
        private EmitNewObj(Type objectType)
        {
            ObjectType = objectType;
        }

        public Type ObjectType { get; }

        public void Emit(CodeGenerator generator)
        {
            ConstructorInfo ctor = ObjectType.GetDefaultCtor();
            generator.EmitNewObject(ctor);
        }

        public static IEmitterType NewObj(Type objectType)
        {
            return new EmitNewObj(objectType);
        }
    }
    internal sealed class EmitReturn : IEmitterType
    {
        private readonly IEmitterType _returnValue;

        private EmitReturn(IEmitterType returnValue, Type returnType)
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

        public static IEmitterType Return(IEmitterType returnValue, Type returnType = null)
        {
            return new EmitReturn(returnValue, returnType);
        }
    }
    internal sealed class EmitProperty
    {
        public static IEmitterType Load(IEmitterType source, PropertyInfo property)
        {
            return new EmitLoadProperty(source, property);
        }

        public static IEmitterType Store(PropertyInfo property, IEmitterType targetObject, IEmitterType value)
        {
            return new EmitStoreProperty(property, targetObject, value);
        }


        private sealed class EmitLoadProperty : IEmitterType
        {
            private readonly PropertyInfo _property;
            private readonly IEmitterType _source;

            public EmitLoadProperty(IEmitterType source, PropertyInfo property)
            {
                _source = source;
                _property = property;
                ObjectType = property.PropertyType;
            }

            public Type ObjectType { get; }

            public void Emit(CodeGenerator generator)
            {
                MethodInfo method = _property.GetGetMethod();
                EmitMethod.Call(method, _source, null).Emit(generator);
            }
        }


        private sealed class EmitStoreProperty : IEmitterType
        {
            private readonly IEmitterType _callMethod;

            public EmitStoreProperty(PropertyInfo property, IEmitterType targetObject, IEmitterType value)
            {
                MethodInfo method = property.GetSetMethod();
                _callMethod = EmitMethod.Call(method, targetObject, value);
                ObjectType = _callMethod.ObjectType;
            }

            public Type ObjectType { get; }

            public void Emit(CodeGenerator generator)
            {
                _callMethod.Emit(generator);
            }
        }
    }
    internal static class EmitThis
    {
        public static IEmitterType Load(Type thisType)
        {
            return EmitArgument.Load(thisType, 0);
        }
    }
}
