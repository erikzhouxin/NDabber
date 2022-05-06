using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// Cobber创建者
    /// </summary>
    public static partial class CobberBuilder
    {
        /// <summary>
        /// 创建一个简单类实例
        /// </summary>
        /// <typeparam name="T">纯接口属性类型</typeparam>
        /// <returns></returns>
        public static T CreateSampleInstance<T>()
        {
            if (SampleClassBuilder<T>.IsValider)
            {
                return SampleClassBuilder<T>.CreateInstance();
            }
            return SampleClassBuilder<T>.Defaulter;
        }
        /// <summary>
        /// 创建一个简单数据库类实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateSampleDbInstance<T>()
        {
            if (SampleDbClassBuilder<T>.IsValider)
            {
                return SampleDbClassBuilder<T>.CreateInstance();
            }
            return SampleClassBuilder<T>.Defaulter;
        }

    }
}
