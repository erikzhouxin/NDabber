using System;
using System.Collections;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Data.Mabber
{
    /// <summary>
    /// 绑定配置
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    public interface IBindingConfig<TSource, TTarget>
    {
        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        void Bind(Expression<Func<TSource, object>> source, Expression<Func<TTarget, object>> target);
        // void Bind<TField>(Expression<Func<TTarget, TField>> target, TField value); not working yet
        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="target"></param>
        /// <param name="targetType"></param>
        void Bind(Expression<Func<TTarget, object>> target, Type targetType);
        /// <summary>
        /// 忽略
        /// </summary>
        /// <param name="expression"></param>
        void Ignore(Expression<Func<TSource, object>> expression);
    }
    /// <summary>
    /// TinyMapper配置
    /// </summary>
    public interface ITinyMapperConfig
    {
        /// <summary>
        /// Custom name matching function used for auto bindings
        /// </summary>
        /// <param name="nameMatching">Function to match names</param>
        void NameMatching(Func<string, string, bool> nameMatching);

        /// <summary>
        /// Reset settings to default
        /// </summary>
        void Reset();
    }
    /// <summary>
    /// 忽略特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class IgnoreAttribute : Attribute
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="targetType"></param>
        public IgnoreAttribute(Type targetType = null)
        {
            TargetType = targetType;
        }
        /// <summary>
        /// 标记类型
        /// </summary>
        public Type TargetType { get; }
    }
    /// <summary>
    /// 绑定特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class BindAttribute : Attribute
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="targetMemberName"></param>
        /// <param name="targetType"></param>
        public BindAttribute(string targetMemberName, Type targetType = null)
        {
            MemberName = targetMemberName;
            TargetType = targetType;
        }
        /// <summary>
        /// 标记类型
        /// </summary>
        public Type TargetType { get; }
        /// <summary>
        /// 属性名称
        /// </summary>
        public string MemberName { get; }
    }

    internal interface IMapperBuilderConfig
    {
        IDynamicAssembly Assembly { get; }
        Func<string, string, bool> NameMatching { get; }
        Option<BindingConfig> GetBindingConfig(TypePair typePair);
        MapperBuilder GetMapperBuilder(TypePair typePair);
        MapperBuilder GetMapperBuilder(TypePair parentTypePair, MappingMember mappingMember);
    }
    internal sealed class TinyMapperConfig : ITinyMapperConfig
    {
        private readonly TargetMapperBuilder _targetMapperBuilder;

        public TinyMapperConfig(TargetMapperBuilder targetMapperBuilder)
        {
            _targetMapperBuilder = targetMapperBuilder ?? throw new ArgumentNullException();
        }

        public void NameMatching(Func<string, string, bool> nameMatching)
        {
            if (nameMatching == null)
            {
                throw new ArgumentNullException();
            }
            _targetMapperBuilder.SetNameMatching(nameMatching);
        }

        public void Reset()
        {
            _targetMapperBuilder.SetNameMatching(TargetMapperBuilder.DefaultNameMatching);
        }
    }
    internal sealed class TargetMapperBuilder : IMapperBuilderConfig
    {
        public static readonly Func<string, string, bool> DefaultNameMatching = (source, target) => string.Equals(source, target, StringComparison.Ordinal);

        private readonly Dictionary<TypePair, BindingConfig> _bindingConfigs = new Dictionary<TypePair, BindingConfig>();
        private readonly ClassMapperBuilder _classMapperBuilder;
        private readonly CollectionMapperBuilder _collectionMapperBuilder;
        private readonly ConvertibleTypeMapperBuilder _convertibleTypeMapperBuilder;
        private readonly CustomTypeMapperBuilder _customTypeMapperBuilder;

        public TargetMapperBuilder(IDynamicAssembly assembly)
        {
            Assembly = assembly;

            var mapperCache = new MapperCache();
            _classMapperBuilder = new ClassMapperBuilder(mapperCache, this);
            _collectionMapperBuilder = new CollectionMapperBuilder(mapperCache, this);
            _convertibleTypeMapperBuilder = new ConvertibleTypeMapperBuilder(this);
            _customTypeMapperBuilder = new CustomTypeMapperBuilder(this);

            NameMatching = DefaultNameMatching;
        }

        public Func<string, string, bool> NameMatching { get; private set; }

        public IDynamicAssembly Assembly { get; }

        public Option<BindingConfig> GetBindingConfig(TypePair typePair)
        {
            Option<BindingConfig> result = _bindingConfigs.GetValue(typePair);
            return result;
        }

        public MapperBuilder GetMapperBuilder(TypePair parentTypePair, MappingMember mappingMember)
        {
            if (_customTypeMapperBuilder.IsSupported(parentTypePair, mappingMember))
            {
                return _customTypeMapperBuilder;
            }
            return GetTypeMapperBuilder(mappingMember.TypePair);
        }

        public MapperBuilder GetMapperBuilder(TypePair typePair)
        {
            return GetTypeMapperBuilder(typePair);
        }

        public void SetNameMatching(Func<string, string, bool> nameMatching)
        {
            NameMatching = nameMatching;
        }

        public Mapper Build(TypePair typePair, BindingConfig bindingConfig)
        {
            _bindingConfigs[typePair] = bindingConfig;
            return Build(typePair);
        }

        public Mapper Build(TypePair typePair)
        {
            MapperBuilder mapperBuilder = GetTypeMapperBuilder(typePair);
            Mapper mapper = mapperBuilder.Build(typePair);
            return mapper;
        }

        private MapperBuilder GetTypeMapperBuilder(TypePair typePair)
        {
            if (_convertibleTypeMapperBuilder.IsSupported(typePair))
            {
                return _convertibleTypeMapperBuilder;
            }
            else if (_collectionMapperBuilder.IsSupported(typePair))
            {
                return _collectionMapperBuilder;
            }
            return _classMapperBuilder;
        }
    }
    internal class BindingConfig
    {
        private readonly Dictionary<string, List<string>> _oneToOneBindFields = new Dictionary<string, List<string>>();
        private readonly Dictionary<string, List<BindingFieldPath>> _bindFieldsPath = new Dictionary<string, List<BindingFieldPath>>();
        private readonly Dictionary<string, Type> _bindTypes = new Dictionary<string, Type>();
        private readonly Dictionary<string, Func<object, object>> _customTypeConverters = new Dictionary<string, Func<object, object>>();
        private readonly HashSet<string> _ignoreFields = new HashSet<string>();

        internal void BindConverter(string targetName, Func<object, object> func)
        {
            _customTypeConverters[targetName] = func;
        }

        internal void BindFields(List<string> sourcePath, List<string> targetPath)
        {
            var bindingFieldPath = new BindingFieldPath(sourcePath, targetPath);

            if (!bindingFieldPath.HasPath)
            {
                if (_oneToOneBindFields.ContainsKey(bindingFieldPath.SourceHead))
                {
                    _oneToOneBindFields[bindingFieldPath.SourceHead].Add(bindingFieldPath.TargetHead);
                }
                else
                {
                    _oneToOneBindFields[bindingFieldPath.SourceHead] = new List<string> { bindingFieldPath.TargetHead };
                }
            }
            else
            {
                if (_bindFieldsPath.ContainsKey(bindingFieldPath.SourceHead))
                {
                    _bindFieldsPath[bindingFieldPath.SourceHead].Add(bindingFieldPath);
                }
                else
                {
                    _bindFieldsPath[bindingFieldPath.SourceHead] = new List<BindingFieldPath> { bindingFieldPath };
                }
            }
        }

        internal void BindType(string targetName, Type value)
        {
            _bindTypes[targetName] = value;
        }

        internal Option<List<string>> GetBindField(string sourceName)
        {
            List<string> result;
            bool exists = _oneToOneBindFields.TryGetValue(sourceName, out result);
            return new Option<List<string>>(result, exists);
        }

        internal Option<List<BindingFieldPath>> GetBindFieldPath(string fieldName)
        {
            List<BindingFieldPath> result;
            bool exists = _bindFieldsPath.TryGetValue(fieldName, out result);
            return new Option<List<BindingFieldPath>>(result, exists);
        }

        internal Option<Type> GetBindType(string targetName)
        {
            Type result;
            bool exists = _bindTypes.TryGetValue(targetName, out result);
            return new Option<Type>(result, exists);
        }

        internal Option<Func<object, object>> GetCustomTypeConverter(string targetName)
        {
            return _customTypeConverters.GetValue(targetName);
        }

        internal bool HasCustomTypeConverter(string targetName)
        {
            return _customTypeConverters.ContainsKey(targetName);
        }

        internal void IgnoreSourceField(string sourceName)
        {
            _ignoreFields.Add(sourceName);
        }

        internal bool IsIgnoreSourceField(string sourceName)
        {
            if (string.IsNullOrEmpty(sourceName))
            {
                return true;
            }
            return _ignoreFields.Contains(sourceName);
        }
    }
    internal sealed class BindingConfigOf<TSource, TTarget> : BindingConfig, IBindingConfig<TSource, TTarget>
    {
        public void Bind(Expression<Func<TSource, object>> source, Expression<Func<TTarget, object>> target)
        {
            List<string> sourcePath = GetMemberInfoPath(source);
            List<string> targetPath = GetMemberInfoPath(target);

            if (sourcePath.Count == 1 && targetPath.Count == 1 &&
                string.Equals(sourcePath[0], targetPath[0], StringComparison.Ordinal))
            {
                return;
            }

            BindFields(sourcePath, targetPath);
        }

        //        public void Bind<TField>(Expression<Func<TTarget, TField>> target, TField value)
        //        {
        //            Func<object, object> func = x => value;
        //            BindConverter(GetMemberInfo(target), func);
        //        }

        public void Bind(Expression<Func<TTarget, object>> target, Type targetType)
        {
            string targetName = GetMemberInfo(target);
            BindType(targetName, targetType);
        }

        public void Ignore(Expression<Func<TSource, object>> expression)
        {
            string memberName = GetMemberInfo(expression);
            IgnoreSourceField(memberName);
        }

        private static string GetMemberInfo<T, TField>(Expression<Func<T, TField>> expression)
        {
            var member = expression.Body as MemberExpression;
            if (member == null)
            {
                var unaryExpression = expression.Body as UnaryExpression;
                if (unaryExpression != null)
                {
                    member = unaryExpression.Operand as MemberExpression;
                }

                if (member == null)
                {
                    throw new ArgumentException("Expression is not a MemberExpression", "expression");
                }
            }
            return member.Member.Name;
        }

        private static List<string> GetMemberInfoPath<T, TField>(Expression<Func<T, TField>> expression)
        {
            var member = expression.Body as MemberExpression;
            if (member == null)
            {
                var unaryExpression = expression.Body as UnaryExpression;
                if (unaryExpression != null)
                {
                    member = unaryExpression.Operand as MemberExpression;
                }

                if (member == null)
                {
                    throw new ArgumentException("Expression is not a MemberExpression", nameof(expression));
                }
            }
            var result = new List<string>();
            do
            {
                var resultMember = member.Member;
                result.Add(resultMember.Name);
                member = member.Expression as MemberExpression;
            }
            while (member != null);
            result.Reverse();
            return result;
        }
    }
    internal sealed class BindingFieldPath
    {
        public BindingFieldPath(List<string> sourcePath, List<string> targetPath)
        {
            SourcePath = sourcePath;
            TargetPath = targetPath;
            HasPath = sourcePath.Count != 1 || targetPath.Count != 1;
            SourceHead = sourcePath[0];
            TargetHead = targetPath[0];
        }

        public List<string> SourcePath { get; }
        public List<string> TargetPath { get; }
        public string SourceHead { get; }
        public string TargetHead { get; }
        public bool HasPath { get; }
    }
    internal interface IDynamicAssembly
    {
        TypeBuilder DefineType(string typeName, Type parentType);
        void Save();
    }
    internal interface IEmitter
    {
        void Emit(CodeGenerator generator);
    }
    internal interface IEmitterType : IEmitter
    {
        Type ObjectType { get; }
    }
    internal struct TypePair : IEquatable<TypePair>
    {
        public TypePair(Type source, Type target) : this()
        {
            Target = target;
            Source = source;
        }

        public bool IsDeepCloneable
        {
            get
            {
                if (IsEqualTypes == false)
                {
                    return false;
                }
                else if (IsValueTypes && IsPrimitiveTypes)
                {
                    return true;
                }
                else if (Source == typeof(string) || Source == typeof(decimal) ||
                         Source == typeof(DateTime) || Source == typeof(DateTimeOffset) ||
                         Source == typeof(TimeSpan) || Source == typeof(Guid))
                {
                    return true;
                }
                else if (IsNullableTypes)
                {
                    var nullablePair = new TypePair(Nullable.GetUnderlyingType(Source), Nullable.GetUnderlyingType(Target));
                    return nullablePair.IsDeepCloneable;
                }
                return false;
            }
        }

        public bool IsEnumTypes => TinyMapper.IsEnum(Source) && TinyMapper.IsEnum(Target);

        public bool IsEnumerableTypes => Source.IsIEnumerable() && Target.IsIEnumerable();

        public bool IsNullableToNotNullable => Source.IsNullable() && Target.IsNullable() == false;

        public Type Source { get; }
        public Type Target { get; }

        private bool IsEqualTypes => Source == Target;

        private bool IsNullableTypes => Source.IsNullable() && Target.IsNullable();

        private bool IsPrimitiveTypes => TinyMapper.IsPrimitive(Source) && TinyMapper.IsPrimitive(Target);

        private bool IsValueTypes => TinyMapper.IsValueType(Source) && TinyMapper.IsValueType(Target);

        public static TypePair Create(Type source, Type target)
        {
            return new TypePair(source, target);
        }

        public static TypePair Create<TSource, TTarget>()
        {
            return new TypePair(typeof(TSource), typeof(TTarget));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is TypePair && Equals((TypePair)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Source != null ? Source.GetHashCode() : 0) * 397) ^ (Target != null ? Target.GetHashCode() : 0);
            }
        }

        // TODO Cache TypeConverters
        public bool HasTypeConverter()
        {
            TypeConverter fromConverter = TypeDescriptor.GetConverter(Source);
            if (fromConverter.CanConvertTo(Target))
            {
                return true;
            }

            TypeConverter toConverter = TypeDescriptor.GetConverter(Target);
            if (toConverter.CanConvertFrom(Source))
            {
                return true;
            }
            return false;
        }

        public bool Equals(TypePair other)
        {
            return Source == other.Source && Target == other.Target;
        }
    }
    /// <summary>
    /// https://github.com/Nelibur/Nelibur
    /// </summary>
    internal struct Option<T>
    {
        public Option(T value, bool hasValue = true)
        {
            HasValue = hasValue;
            Value = value;
        }

        public static Option<T> Empty { get; } = new Option<T>(default(T), false);

        public bool HasNoValue => !HasValue;

        public bool HasValue { get; }

        public T Value { get; }

        public Option<T> Match(Func<T, bool> predicate, Action<T> action)
        {
            if (HasNoValue)
            {
                return Empty;
            }
            if (predicate(Value))
            {
                action(Value);
            }
            return this;
        }

        public Option<T> MatchType<TTarget>(Action<TTarget> action)
            where TTarget : T
        {
            if (HasNoValue)
            {
                return Empty;
            }
            if (Value.GetType() == typeof(TTarget))
            {
                action((TTarget)Value);
            }
            return this;
        }

        public Option<T> ThrowOnEmpty<TException>()
            where TException : Exception, new()
        {
            if (HasValue)
            {
                return this;
            }
            throw TinyMapper.Type<TException>();
        }

        public Option<T> ThrowOnEmpty<TException>(Func<TException> func)
            where TException : Exception
        {
            if (HasValue)
            {
                return this;
            }
            throw func();
        }
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
            return TinyMapper.StoreField(field, targetObject, value);
        }

        private static IEmitterType StoreProperty(PropertyInfo property, IEmitterType targetObject, IEmitterType value)
        {
            return TinyMapper.StoreProperty(property, targetObject, value);
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
                var sourceObject = TinyMapper.LoadArgument(memberPath.TypePair.Source, 1);
                var targetObject = TinyMapper.LoadArgument(memberPath.TypePair.Target, 2);

                var sourceMember = LoadMember(memberPath.Source, sourceObject, memberPath.Source.Count);
                var targetMember = LoadMember(memberPath.Target, targetObject, memberPath.Target.Count);

                IEmitterType convertedMember = ConvertMember(parentTypePair, memberPath.Tail, sourceMember, targetMember);

                IEmitter result = StoreTargetObjectMember(memberPath.Tail, targetObject, convertedMember);
                return result;
            }
            else
            {
                var targetObject = TinyMapper.LoadArgument(memberPath.Head.TypePair.Target, 2);
                var targetMember = LoadMember(memberPath.Target, targetObject, memberPath.Target.Count - 1);

                var sourceObject = TinyMapper.LoadArgument(memberPath.Head.TypePair.Source, 1);
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
            return TinyMapper.LoadField(source, field);
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
            return TinyMapper.LoadProperty(source, property);
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
            IEmitterType mappers = TinyMapper.LoadField(TinyMapper.LoadThis(mapperType), mappersField);
            IEmitterType mapper = TinyMapper.LoadArray(mappers, Id);
            IEmitterType result = TinyMapper.Call(mapMethod, mapper, sourceMemeber, targetMember);
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

            var rootMapper = (Mapper)Activator.CreateInstance(TinyMapper.CreateType(typeBuilder));

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

            IEmitterType result = TinyMapper.IsValueType(targetType) ? EmitValueType(targetType, codeGenerator) : EmitRefType(targetType);

            TinyMapper.LoadReturn(result, targetType).Emit(codeGenerator);
        }

        private static IEmitterType EmitRefType(Type type)
        {
            return type.HasDefaultCtor() ? TinyMapper.LoadNewObj(type) : TinyMapper.LoadNull();
        }

        private static IEmitterType EmitValueType(Type type, CodeGenerator codeGenerator)
        {
            LocalBuilder builder = codeGenerator.DeclareLocal(type);
            TinyMapper.LoadDeclare(builder).Emit(codeGenerator);
            return TinyMapper.LoadBox(TinyMapper.LoadLocal(builder));
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
            emitterComposite.Add(TinyMapper.LoadReturn(TinyMapper.LoadArgument(typePair.Target, 2)));
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
            if (TinyMapper.IsValueType(stackType) == false && targetType == typeof(object))
            {
                return this;
            }
            if (TinyMapper.IsValueType(stackType) && !TinyMapper.IsValueType(targetType))
            {
                _ilGenerator.Emit(OpCodes.Box, stackType);
            }
            else if (!TinyMapper.IsValueType(stackType) && TinyMapper.IsValueType(targetType))
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

            var rootMapper = (Mapper)Activator.CreateInstance(TinyMapper.CreateType(typeBuilder));

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

            IEmitterType sourceMemeber = TinyMapper.LoadArgument(typeof(object), 1);
            IEmitterType targetMember = TinyMapper.LoadNull();

            IEmitterType callMapMethod = mapperCacheItem.EmitMapMethod(sourceMemeber, targetMember);

            TinyMapper.LoadReturn(callMapMethod).Emit(new CodeGenerator(methodBuilder.GetILGenerator()));
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

            IEmitterType returnValue = TinyMapper.Call(methodTemplate, TinyMapper.LoadThis(parentType), TinyMapper.LoadArgument(typeof(IEnumerable), 1));
            TinyMapper.LoadReturn(returnValue).Emit(new CodeGenerator(methodBuilder.GetILGenerator()));
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

            IEmitterType returnValue = TinyMapper.Call(methodTemplate, TinyMapper.LoadThis(parentType), TinyMapper.LoadArgument(typeof(IEnumerable), 1));
            TinyMapper.LoadReturn(returnValue).Emit(new CodeGenerator(methodBuilder.GetILGenerator()));
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

            if (TinyMapper.IsEnum(pair.Target))
            {
                if (TinyMapper.IsEnum(pair.Source) == false)
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

            if (TinyMapper.IsEnum(pair.Source))
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
            return TinyMapper.IsPrimitive(value)
                   || value == typeof(string)
                   || value == typeof(Guid)
                   || TinyMapper.IsEnum(value)
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
}
