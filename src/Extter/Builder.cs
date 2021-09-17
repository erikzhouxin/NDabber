using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Extter
{
    /// <summary>
    /// 创建者
    /// </summary>
    public static class Builder
    {
        /// <summary>
        /// 创建空数组(利用引用,相当于Array.Empty)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T[] EmptyArray<T>()
        {
            return BuilderArray<T>.Empty;
        }
    }
    internal class BuilderArray<T>
    {
        public static T[] Empty { get; }

        static BuilderArray()
        {
            Empty = new T[0];
        }
    }
}
