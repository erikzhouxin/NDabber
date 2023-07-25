using System;
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
    ///     Configuration for TinyMapper
    /// </summary>
    public interface ITinyMapperConfig
    {
        /// <summary>
        ///     Custom name matching function used for auto bindings
        /// </summary>
        /// <param name="nameMatching">Function to match names</param>
        void NameMatching(Func<string, string, bool> nameMatching);

        /// <summary>
        ///     Reset settings to default
        /// </summary>
        void Reset();
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

        public bool IsEnumTypes => Helpers.IsEnum(Source) && Helpers.IsEnum(Target);

        public bool IsEnumerableTypes => Source.IsIEnumerable() && Target.IsIEnumerable();

        public bool IsNullableToNotNullable => Source.IsNullable() && Target.IsNullable() == false;

        public Type Source { get; }
        public Type Target { get; }

        private bool IsEqualTypes => Source == Target;

        private bool IsNullableTypes => Source.IsNullable() && Target.IsNullable();

        private bool IsPrimitiveTypes => Helpers.IsPrimitive(Source) && Helpers.IsPrimitive(Target);

        private bool IsValueTypes => Helpers.IsValueType(Source) && Helpers.IsValueType(Target);

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
    ///     https://github.com/Nelibur/Nelibur
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
            throw Error.Type<TException>();
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
}
