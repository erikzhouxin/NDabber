using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Extter
{
    /// <summary>
    /// 默认值
    /// </summary>
    public sealed class DefaultValue : IDefaultValue
    {
        internal static Dictionary<Type, IDefaultValue> _defaultValues = new Dictionary<Type, IDefaultValue>();
        /// <summary>
        /// 得到默认值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Get(Type type)
        {
            return GetModel(type).Value;
        }
        /// <summary>
        /// 获取模型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IDefaultValue GetModel(Type type)
        {
            if (_defaultValues.TryGetValue(type, out var value))
            {
                return value;
            }
            return (IDefaultValue)Activator.CreateInstance(typeof(DefaultValue<>).MakeGenericType(type));
        }
        private IDefaultValue _model;
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="type"></param>
        public DefaultValue(Type type)
        {
            _model = GetModel(type);
        }
        /// <summary>
        /// 默认值
        /// </summary>
        public object Value { get => _model.Value; }
        /// <summary>
        /// 类型
        /// </summary>
        public Type Type { get => _model.Type; }
    }
    /// <summary>
    /// 默认值接口
    /// </summary>
    public interface IDefaultValue
    {
        /// <summary>
        /// 默认值
        /// </summary>
        object Value { get; }
        /// <summary>
        /// 类型
        /// </summary>
        Type Type { get; }
    }
    /// <summary>
    /// 默认值泛型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class DefaultValue<T> : IDefaultValue
    {
        /// <summary>
        /// 默认值
        /// </summary>
        public static object DefValue { get; }
        /// <summary>
        /// 类型
        /// </summary>
        public static Type DefType { get; }
        static DefaultValue()
        {
            DefValue = default(T);
            DefaultValue._defaultValues[DefType = typeof(T)] = new DefaultValue<T>();
        }
        /// <summary>
        /// 默认值
        /// </summary>
        public object Value => DefValue;
        /// <summary>
        /// 类型
        /// </summary>
        public Type Type => DefType;
    }
}
