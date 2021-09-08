using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace System.Data.Cobber
{
    /// <summary>
    /// 极懒加载
    /// </summary>
    [DebuggerTypeProxy(typeof(LazyBoneDebugView<>))]
    [DebuggerDisplay("IsValueCreated={IsValueCreated}, Value={Value}")]
    public class LazyBone<T>
    {
        private object _lockObj = new object();
        private Func<T> _factory;
        private T _value;
        private bool _isThreadSafe;
        private bool _isCreated;
        /// <summary>
        /// 构造
        /// </summary>
        public LazyBone(T value) : this(() => value)
        {
            _value = value;
            _isCreated = true;
        }
        /// <summary>
        /// 构造
        /// </summary>
        public LazyBone(Func<T> factory) : this(factory, false) { }
        /// <summary>
        /// 构造
        /// </summary>
        public LazyBone(Func<T> factory, bool isThreadSafe)
        {
            _factory = factory;
            _isThreadSafe = isThreadSafe;
        }
        /// <summary>
        /// 转换成字符串
        /// </summary>
        public override string ToString()
        {
            return Value?.ToString();
        }
        /// <summary>
        /// 是否已经创建
        /// </summary>
        public bool IsValueCreated => _isCreated;
        /// <summary>
        /// 是否已经创建
        /// </summary>
        public bool IsThreadSafe => _isThreadSafe;
        /// <summary>
        /// 加载值
        /// </summary>
        public T Value => _isCreated ? _value : CreateValue();
        private T CreateValue()
        {
            if (!_isThreadSafe)
            {
                return _value = _factory();
            }
            try
            {
                lock (_lockObj)
                {
                    if (!_isCreated)
                    {
                        _value = _factory();
                    }
                }
            }
            catch { }
            return _value;
        }
        /// <summary>
        /// 重新加载
        /// </summary>
        public void Reload()
        {
            _value = default(T);
            _isCreated = false;
        }
    }
    /// <summary>
    /// 触觉懒汉
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LazyBoneObservable<T> : LazyBone<ObservableCollection<T>>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public LazyBoneObservable(ObservableCollection<T> value) : base(value) { }
        /// <summary>
        /// 构造
        /// </summary>
        public LazyBoneObservable(Func<ObservableCollection<T>> factory) : base(factory) { }
        /// <summary>
        /// 构造
        /// </summary>
        public LazyBoneObservable(Func<ObservableCollection<T>> factory, bool isThreadSafe) : base(factory, isThreadSafe) { }
    }
    /// <summary>
    /// 序列懒汉
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LazyBones<T> : LazyBone<IEnumerable<T>>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public LazyBones(IEnumerable<T> value) : base(value) { }
        /// <summary>
        /// 构造
        /// </summary>
        public LazyBones(Func<IEnumerable<T>> factory) : base(factory) { }
        /// <summary>
        /// 构造
        /// </summary>
        public LazyBones(Func<IEnumerable<T>> factory, bool isThreadSafe) : base(factory, isThreadSafe) { }
    }
    /// <summary>
    /// 触觉懒汉
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LazyBoneList<T> : LazyBone<List<T>>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public LazyBoneList(List<T> value) : base(value) { }
        /// <summary>
        /// 构造
        /// </summary>
        public LazyBoneList(Func<List<T>> factory) : base(factory) { }
        /// <summary>
        /// 构造
        /// </summary>
        public LazyBoneList(Func<List<T>> factory, bool isThreadSafe) : base(factory, isThreadSafe) { }
    }
    /// <summary>
    /// 调试视图
    /// </summary>
    internal sealed class LazyBoneDebugView<T>
    {
        private readonly LazyBone<T> _lazy;
        /// <summary>
        /// 构造
        /// </summary>
        public LazyBoneDebugView(LazyBone<T> lazy)
        {
            _lazy = lazy;
        }
        /// <summary>
        /// 已创建
        /// </summary>
        public bool IsValueCreated => _lazy.IsValueCreated;
        /// <summary>
        /// 值
        /// </summary>
        public T Value => _lazy.Value;
        /// <summary>
        /// 线程安全
        /// </summary>
        public bool IsThreadSafe => _lazy.IsThreadSafe;
    }
    /// <summary>
    /// 懒加载调用
    /// </summary>
    public static partial class CobberCaller
    {
        /// <summary>
        /// 获取一个Lazy模型
        /// </summary>
        /// <returns></returns>
        public static Lazy<T> GetLazy<T>(this T model, bool isThreadSafe = true)
        {
            return new Lazy<T>(() => model, isThreadSafe);
        }
        /// <summary>
        /// 获取一个Lazy模型
        /// </summary>
        /// <returns></returns>
        public static Lazy<T> GetLazy<T>(this T model, Func<T> GetValue, bool isThreadSafe = true)
        {
            return model == null ? new Lazy<T>(GetValue, isThreadSafe) : new Lazy<T>(() => model, isThreadSafe);
        }
        /// <summary>
        /// 获取一个Lazy模型
        /// </summary>
        /// <returns></returns>
        public static Lazy<T> GetLazy<T>(this Func<T> GetValue, bool isThreadSafe = true)
        {
            return new Lazy<T>(GetValue, isThreadSafe);
        }
        /// <summary>
        /// 获取一个Lazy模型
        /// </summary>
        /// <returns></returns>
        public static LazyBone<T> GetLazyBone<T>(this T model, bool isThreadSafe = true)
        {
            return new LazyBone<T>(() => model, isThreadSafe);
        }
        /// <summary>
        /// 获取一个Lazy模型
        /// </summary>
        /// <returns></returns>
        public static LazyBone<T> GetLazyBone<T>(this T model, Func<T> GetValue, bool isThreadSafe = true)
        {
            return model == null ? new LazyBone<T>(GetValue, isThreadSafe) : new LazyBone<T>(() => model, isThreadSafe);
        }
        /// <summary>
        /// 获取一个Lazy模型
        /// </summary>
        /// <returns></returns>
        public static LazyBone<T> GetLazyBone<T>(this Func<T> GetValue, bool isThreadSafe = true)
        {
            return new LazyBone<T>(GetValue, isThreadSafe);
        }
    }
}
