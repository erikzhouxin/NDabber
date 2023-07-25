using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace System.Data.Mabber
{
    /// <summary>
    /// TinyMapper is an object to object mapper for .NET. The main advantage is performance.
    /// TinyMapper allows easily map object to object, i.e. properties or fields from one object to another.
    /// </summary>
    public static class TinyMapper
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
                throw Error.ArgumentNull("Source cannot be null. Use TinyMapper.Map<TSource, TTarget> method instead.");
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
    internal sealed class MemberMapper
    {
        private readonly IMapperBuilderConfig _config;
        private readonly MapperCache _mapperCache;


        public MemberMapper(MapperCache mapperCache, IMapperBuilderConfig config)
        {
            _mapperCache = mapperCache;
            _config = config;
        }

        public MemberEmitterDescription Build(TypePair parentTypePair, List<MappingMemberPath> members)
        {
            var emitComposite = new EmitComposite();
            foreach (var path in members)
            {
                IEmitter emitter = Build(parentTypePair, path);
                emitComposite.Add(emitter);
            }
            var result = new MemberEmitterDescription(emitComposite, _mapperCache);
            result.AddMapper(_mapperCache);
            return result;
        }

        private static IEmitterType StoreFiled(FieldInfo field, IEmitterType targetObject, IEmitterType value)
        {
            return EmitField.Store(field, targetObject, value);
        }

        private static IEmitterType StoreProperty(PropertyInfo property, IEmitterType targetObject, IEmitterType value)
        {
            return EmitProperty.Store(property, targetObject, value);
        }

        private static IEmitterType StoreTargetObjectMember(MappingMember member, IEmitterType targetObject, IEmitterType convertedMember)
        {
            IEmitterType result = null;
            member.Target
                  .ToOption()
                  .Match(x => x.IsField(), x => result = StoreFiled((FieldInfo)x, targetObject, convertedMember))
                  .Match(x => x.IsProperty(), x => result = StoreProperty((PropertyInfo)x, targetObject, convertedMember));
            return result;
        }

        private IEmitter Build(TypePair parentTypePair, MappingMemberPath memberPath)
        {

            if (memberPath.OneLevelTarget)
            {
                var sourceObject = EmitArgument.Load(memberPath.TypePair.Source, 1);
                var targetObject = EmitArgument.Load(memberPath.TypePair.Target, 2);

                var sourceMember = LoadMember(memberPath.Source, sourceObject, memberPath.Source.Count);
                var targetMember = LoadMember(memberPath.Target, targetObject, memberPath.Target.Count);

                IEmitterType convertedMember = ConvertMember(parentTypePair, memberPath.Tail, sourceMember, targetMember);

                IEmitter result = StoreTargetObjectMember(memberPath.Tail, targetObject, convertedMember);
                return result;
            }
            else
            {
                var targetObject = EmitArgument.Load(memberPath.Head.TypePair.Target, 2);
                var targetMember = LoadMember(memberPath.Target, targetObject, memberPath.Target.Count - 1);

                var sourceObject = EmitArgument.Load(memberPath.Head.TypePair.Source, 1);
                var sourceMember = LoadMember(memberPath.Source, sourceObject, memberPath.Source.Count);

                IEmitterType convertedMember = ConvertMember(parentTypePair, memberPath.Tail, sourceMember, targetMember);

                IEmitter result = StoreTargetObjectMember(memberPath.Tail, targetMember, convertedMember);
                return result;
            }
        }

        private IEmitterType ConvertMember(TypePair parentTypePair, MappingMember member, IEmitterType sourceMemeber, IEmitterType targetMember)
        {
            //            if (member.TypePair.IsDeepCloneable && _config.GetBindingConfig(parentTypePair).HasNoValue)
            if (member.TypePair.IsDeepCloneable)
            {
                return sourceMemeber;
            }

            MapperCacheItem mapperCacheItem = CreateMapperCacheItem(parentTypePair, member);

            IEmitterType result = mapperCacheItem.EmitMapMethod(sourceMemeber, targetMember);
            return result;
        }

        private MapperCacheItem CreateMapperCacheItem(TypePair parentTypePair, MappingMember mappingMember)
        {
            var mapperCacheItemOption = _mapperCache.Get(mappingMember.TypePair);
            if (mapperCacheItemOption.HasValue)
            {
                return mapperCacheItemOption.Value;
            }

            MapperBuilder mapperBuilder = _config.GetMapperBuilder(parentTypePair, mappingMember);
            Mapper mapper = mapperBuilder.Build(parentTypePair, mappingMember);
            MapperCacheItem mapperCacheItem = _mapperCache.Add(mappingMember.TypePair, mapper);
            return mapperCacheItem;
        }

        private IEmitterType LoadField(IEmitterType source, FieldInfo field)
        {
            return EmitField.Load(source, field);
        }

        private IEmitterType LoadMember(List<MemberInfo> members, IEmitterType sourceObject, int loadLevel)
        {
            IEmitterType dummySource = sourceObject;
            if (members.Count == 1)
            {
                return LoadMember(members[0], dummySource);
            }
            for (int i = 0; i < loadLevel; i++)
            {
                dummySource = LoadMember(members[i], dummySource);
            }
            return dummySource;
        }

        private IEmitterType LoadMember(MemberInfo member, IEmitterType sourceObject)
        {
            IEmitterType result = null;
            member.ToOption()
                  .Match(x => x.IsField(), x => result = LoadField(sourceObject, (FieldInfo)x))
                  .Match(x => x.IsProperty(), x => result = LoadProperty(sourceObject, (PropertyInfo)x));
            return result;
        }

        private IEmitterType LoadProperty(IEmitterType source, PropertyInfo property)
        {
            return EmitProperty.Load(source, property);
        }
    }
    internal sealed class MappingMemberBuilder
    {
        private readonly IMapperBuilderConfig _config;

        public MappingMemberBuilder(IMapperBuilderConfig config)
        {
            _config = config;
        }

        public List<MappingMemberPath> Build(TypePair typePair)
        {
            return ParseMappingTypes(typePair);
        }

        private static MemberInfo[] GetPublicMembers(Type type)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
            PropertyInfo[] properties = type.GetProperties(flags);
            FieldInfo[] fields = type.GetFields(flags);
            MemberInfo[] members = new MemberInfo[properties.Length + fields.Length];
            properties.CopyTo(members, 0);
            fields.CopyTo(members, properties.Length);
            return members;
        }

        private static List<MemberInfo> GetSourceMembers(Type sourceType)
        {
            var result = new List<MemberInfo>();

            MemberInfo[] members = GetPublicMembers(sourceType);
            foreach (MemberInfo member in members)
            {
                if (member.IsProperty())
                {
                    MethodInfo method = ((PropertyInfo)member).GetGetMethod();
                    if (method.IsNull())
                    {
                        continue;
                    }
                }
                result.Add(member);
            }
            return result;
        }

        private static List<MemberInfo> GetTargetMembers(Type targetType)
        {
            var result = new List<MemberInfo>();

            MemberInfo[] members = GetPublicMembers(targetType);
            foreach (MemberInfo member in members)
            {
                if (member.IsProperty())
                {
                    MethodInfo method = ((PropertyInfo)member).GetSetMethod();
                    if (method.IsNull() || method.GetParameters().Length != 1)
                    {
                        continue;
                    }
                }
                result.Add(member);
            }
            return result;
        }

        private List<string> GetTargetName(
            Option<BindingConfig> bindingConfig,
            TypePair typePair,
            MemberInfo sourceMember,
            Dictionary<string, string> targetBindings)
        {
            Option<List<string>> targetName;
            List<BindAttribute> binds = sourceMember.GetAttributes<BindAttribute>();
            BindAttribute bind = binds.FirstOrDefault(x => x.TargetType.IsNull());
            if (bind.IsNull())
            {
                bind = binds.FirstOrDefault(x => typePair.Target.IsAssignableFrom(x.TargetType));
            }
            if (bind.IsNotNull())
            {
                targetName = new Option<List<string>>(new List<string> { bind.MemberName });
            }
            else
            {
                targetName = bindingConfig.Map(x => x.GetBindField(sourceMember.Name));
                if (targetName.HasNoValue)
                {
                    if (targetBindings.TryGetValue(sourceMember.Name, out var targetMemberName))
                    {
                        targetName = new Option<List<string>>(new List<string> { targetMemberName });
                    }
                    else
                    {
                        targetName = new Option<List<string>>(new List<string> { sourceMember.Name });
                    }
                }
            }
            return targetName.Value;
        }

        private Dictionary<string, string> GetTest(TypePair typePair, List<MemberInfo> targetMembers)
        {
            var result = new Dictionary<string, string>();
            foreach (MemberInfo member in targetMembers)
            {
                Option<BindAttribute> bindAttribute = member.GetAttribute<BindAttribute>();
                if (bindAttribute.HasNoValue)
                {
                    continue;
                }

                if (bindAttribute.Value.TargetType.IsNull() || typePair.Source.IsAssignableFrom(bindAttribute.Value.TargetType))
                {
                    result[bindAttribute.Value.MemberName] = member.Name;
                }
            }
            return result;
        }

        private bool IsIgnore(Option<BindingConfig> bindingConfig, TypePair typePair, MemberInfo sourceMember)
        {
            List<IgnoreAttribute> ignores = sourceMember.GetAttributes<IgnoreAttribute>();
            if (ignores.Any(x => x.TargetType.IsNull()))
            {
                return true;
            }
            if (ignores.FirstOrDefault(x => typePair.Target.IsAssignableFrom(x.TargetType)).IsNotNull())
            {
                return true;
            }
            return bindingConfig.Map(x => x.IsIgnoreSourceField(sourceMember.Name)).Value;
        }

        private List<MemberInfo> GetSourceMemberPath(List<string> fieldPath, Type sourceType)
        {
            var result = new List<MemberInfo>();
            var dummyType = sourceType;
            foreach (var path in fieldPath)
            {
                var member = GetSourceMembers(dummyType).Single(x => string.Equals(x.Name, path, StringComparison.Ordinal));
                result.Add(member);
                dummyType = member.GetMemberType();
            }
            return result;
        }

        private List<MappingMemberPath> ParseMappingTypes(TypePair typePair)
        {
            var result = new List<MappingMemberPath>();

            List<MemberInfo> sourceMembers = GetSourceMembers(typePair.Source);
            List<MemberInfo> targetMembers = GetTargetMembers(typePair.Target);

            Dictionary<string, string> targetBindings = GetTest(typePair, targetMembers);

            Option<BindingConfig> bindingConfig = _config.GetBindingConfig(typePair);

            foreach (MemberInfo sourceMember in sourceMembers)
            {
                if (IsIgnore(bindingConfig, typePair, sourceMember))
                {
                    continue;
                }

                List<string> targetNames = GetTargetName(bindingConfig, typePair, sourceMember, targetBindings);

                foreach (var targetName in targetNames)
                {
                    MemberInfo targetMember = targetMembers.FirstOrDefault(x => _config.NameMatching(targetName, x.Name));
                    if (targetMember.IsNull())
                    {
                        result.AddRange(GetBindMappingMemberPath(typePair, bindingConfig, sourceMember));
                        continue;
                    }
                    Option<Type> concreteBindingType = bindingConfig.Map(x => x.GetBindType(targetName));
                    if (concreteBindingType.HasValue)
                    {
                        var mappingTypePair = new TypePair(sourceMember.GetMemberType(), concreteBindingType.Value);
                        result.Add(new MappingMemberPath(sourceMember, targetMember, mappingTypePair));
                    }
                    else
                    {
                        result.Add(new MappingMemberPath(sourceMember, targetMember));
                    }

                    result.AddRange(GetBindMappingMemberPath(typePair, bindingConfig, sourceMember));
                }

            }
            return result;
        }

        private List<MappingMemberPath> GetBindMappingMemberPath(TypePair typePair, Option<BindingConfig> bindingConfig, MemberInfo sourceMember)
        {
            var result = new List<MappingMemberPath>();

            var bindFieldPath = bindingConfig.Map(x => x.GetBindFieldPath(sourceMember.Name));

            if (bindFieldPath.HasValue)
            {
                foreach (BindingFieldPath item in bindFieldPath.Value)
                {
                    var sourceMemberPath = GetSourceMemberPath(item.SourcePath, typePair.Source);
                    var targetMemberPath = GetSourceMemberPath(item.TargetPath, typePair.Target);
                    result.Add(new MappingMemberPath(sourceMemberPath, targetMemberPath));

                }
            }
            return result;
        }
    }
    internal sealed class MappingMember
    {
        public MappingMember(MemberInfo source, MemberInfo target, TypePair typePair)
        {
            Source = source;
            Target = target;
            TypePair = typePair;
        }

        public MemberInfo Source { get; }
        public MemberInfo Target { get; }
        public TypePair TypePair { get; }
    }
    internal sealed class MemberEmitterDescription
    {
        public MemberEmitterDescription(IEmitter emitter, MapperCache mappers)
        {
            Emitter = emitter;
            MapperCache = new Option<MapperCache>(mappers, mappers.IsEmpty);
        }

        public IEmitter Emitter { get; }
        public Option<MapperCache> MapperCache { get; private set; }

        public void AddMapper(MapperCache value)
        {
            MapperCache = value.ToOption();
        }
    }
    internal sealed class MappingMemberPath
    {
        public MappingMemberPath(List<MemberInfo> source, List<MemberInfo> target)
            : this(source, target, new TypePair(source[source.Count - 1].GetMemberType(), target[target.Count - 1].GetMemberType()))
        {
        }

        public MappingMemberPath(MemberInfo source, MemberInfo target)
            : this(new List<MemberInfo> { source }, new List<MemberInfo> { target }, new TypePair(source.GetMemberType(), target.GetMemberType()))
        {
        }

        public MappingMemberPath(MemberInfo source, MemberInfo target, TypePair typePair)
            : this(new List<MemberInfo> { source }, new List<MemberInfo> { target }, typePair)
        {
        }

        public MappingMemberPath(List<MemberInfo> source, List<MemberInfo> target, TypePair typePair)
        {
            Source = source;
            OneLevelSource = source.Count == 1;
            OneLevelTarget = target.Count == 1;
            Target = target;
            TypePair = typePair;
            Tail = new MappingMember(source[source.Count - 1], target[target.Count - 1], typePair);
            Head = new MappingMember(source[0], target[0], new TypePair(source[0].GetMemberType(), target[0].GetMemberType()));
        }

        public bool OneLevelSource { get; }
        public bool OneLevelTarget { get; }
        public List<MemberInfo> Source { get; }
        public List<MemberInfo> Target { get; }
        public TypePair TypePair { get; }
        public MappingMember Tail { get; }
        public MappingMember Head { get; }
    }
    internal abstract class MapperOf<TSource, TTarget> : Mapper
    {
        protected override object MapCore(object source, object target)
        {
            if (source == null)
            {
                return default(TTarget);
            }
            return MapCore((TSource)source, (TTarget)target);
        }

        protected abstract TTarget MapCore(TSource source, TTarget target);
    }
    internal sealed class MapperCacheItem
    {
        public int Id { get; set; }
        public Mapper Mapper { get; set; }

        public IEmitterType EmitMapMethod(IEmitterType sourceMemeber, IEmitterType targetMember)
        {
            Type mapperType = typeof(Mapper);
            MethodInfo mapMethod = mapperType.GetMethod(Mapper.MapMethodName, BindingFlags.Instance | BindingFlags.Public);
            FieldInfo mappersField = mapperType.GetField(Mapper.MappersFieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            IEmitterType mappers = EmitField.Load(EmitThis.Load(mapperType), mappersField);
            IEmitterType mapper = EmitArray.Load(mappers, Id);
            IEmitterType result = EmitMethod.Call(mapMethod, mapper, sourceMemeber, targetMember);
            return result;
        }
    }
    internal sealed class MapperCache
    {
        private readonly Dictionary<TypePair, MapperCacheItem> _cache = new Dictionary<TypePair, MapperCacheItem>();

        public bool IsEmpty => _cache.Count == 0;

        public List<Mapper> Mappers
        {
            get
            {
                return _cache.Values
                             .OrderBy(x => x.Id)
                             .ConvertAll(x => x.Mapper);
            }
        }

        public List<MapperCacheItem> MapperCacheItems => _cache.Values.ToList();

        public MapperCacheItem AddStub(TypePair key)
        {
            if (_cache.ContainsKey(key))
            {
                return _cache[key];
            }

            var mapperCacheItem = new MapperCacheItem { Id = GetId() };
            _cache[key] = mapperCacheItem;
            return mapperCacheItem;
        }

        public void ReplaceStub(TypePair key, Mapper mapper)
        {
            _cache[key].Mapper = mapper;
        }

        public MapperCacheItem Add(TypePair key, Mapper mapper)
        {
            MapperCacheItem result;
            if (_cache.TryGetValue(key, out result))
            {
                return result;
            }
            result = new MapperCacheItem
            {
                Id = GetId(),
                Mapper = mapper
            };
            _cache[key] = result;
            return result;
        }

        public Option<MapperCacheItem> Get(TypePair key)
        {
            MapperCacheItem result;
            if (_cache.TryGetValue(key, out result))
            {
                return new Option<MapperCacheItem>(result);
            }
            return Option<MapperCacheItem>.Empty;
        }

        private int GetId()
        {
            return _cache.Count;
        }
    }
    internal abstract class MapperBuilder
    {
        protected const MethodAttributes OverrideProtected = MethodAttributes.Family | MethodAttributes.Virtual;
        private const string AssemblyName = "DynamicTinyMapper";
        protected readonly IDynamicAssembly _assembly;
        protected readonly IMapperBuilderConfig _config;

        protected MapperBuilder(IMapperBuilderConfig config)
        {
            _config = config;
            _assembly = config.Assembly;
        }

        protected abstract string ScopeName { get; }

        public Mapper Build(TypePair typePair)
        {
            return BuildCore(typePair);
        }

        public Mapper Build(TypePair parentTypePair, MappingMember mappingMember)
        {
            return BuildCore(parentTypePair, mappingMember);
        }

        public bool IsSupported(TypePair typePair)
        {
            return IsSupportedCore(typePair);
        }

        protected abstract Mapper BuildCore(TypePair typePair);
        protected abstract Mapper BuildCore(TypePair parentTypePair, MappingMember mappingMember);

        protected MapperBuilder GetMapperBuilder(TypePair typePair)
        {
            return _config.GetMapperBuilder(typePair);
        }

        protected string GetMapperFullName()
        {
            string random = Guid.NewGuid().ToString("N");
            return $"{AssemblyName}.{ScopeName}.Mapper{random}";
        }

        protected abstract bool IsSupportedCore(TypePair typePair);
    }
    internal abstract class Mapper
    {
        public const string MapMethodName = "Map";
        public const string MappersFieldName = "_mappers";
        protected Mapper[] _mappers;

        public void AddMappers(IEnumerable<Mapper> mappers)
        {
            _mappers = mappers.ToArray();
        }

        public void UpdateRootMapper(int mapperId, Mapper mapper)
        {
            if (_mappers == null)
            {
                return;
            }

            for (int i = 0; i < _mappers.Length; i++)
            {
                if (i == mapperId)
                {
                    if (_mappers[i] == null)
                    {
                        _mappers[i] = mapper;
                    }
                    return;
                }
            }
        }

        public object Map(object source, object target = null)
        {
            return MapCore(source, target);
        }

        protected abstract object MapCore(object source, object target);
    }
}
