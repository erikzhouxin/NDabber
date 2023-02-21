using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 极懒加载
    /// <see cref="LazyBone{T}"/>
    /// </summary>
    [DebuggerTypeProxy(typeof(LazyBoneDebugView<>))]
    [DebuggerDisplay("IsValueCreated={IsValueCreated}, Value={Value}")]
    public class Lazying<T> : LazyBone<T>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public Lazying(T value) : this(value, false) { }
        /// <summary>
        /// 构造
        /// </summary>
        public Lazying(T value, bool isThreadSafe) : base(value, isThreadSafe) { }
        /// <summary>
        /// 构造
        /// </summary>
        public Lazying(Func<T> factory) : this(factory, false) { }
        /// <summary>
        /// 构造
        /// </summary>
        public Lazying(Func<T> factory, bool isThreadSafe) : base(factory, isThreadSafe) { }
    }
    /// <summary>
    /// 极懒加载
    /// <see cref="LazyBones{T}"/>
    /// </summary>
    [DebuggerTypeProxy(typeof(LazyBoneDebugView<>))]
    [DebuggerDisplay("IsValueCreated={IsValueCreated}, Value={Value}")]
    public class Lazyes<T> : LazyBones<T>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public Lazyes(IEnumerable<T> value) : this(value, false) { }
        /// <summary>
        /// 构造
        /// </summary>
        public Lazyes(IEnumerable<T> value, bool isThreadSafe) : base(value, isThreadSafe) { }
        /// <summary>
        /// 构造
        /// </summary>
        public Lazyes(Func<IEnumerable<T>> factory) : this(factory, false) { }
        /// <summary>
        /// 构造
        /// </summary>
        public Lazyes(Func<IEnumerable<T>> factory, bool isThreadSafe) : base(factory, isThreadSafe) { }
    }
}
