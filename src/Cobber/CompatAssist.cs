using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// 兼容助手
    /// </summary>
    public static class CompatAssist
    {
#if NET40
        /// <summary>
        /// 获取自定义属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="member"></param>
        /// <returns></returns>
        public static T GetCustomAttribute<T>(this MemberInfo member)
        {
            foreach (var attr in member.GetCustomAttributes(typeof(T), false))
            {
                if (attr is T real) { return real; }
            }
            foreach (var attr in member.GetCustomAttributes(typeof(T), true))
            {
                if (attr is T real) { return real; }
            }
            return default;
        }
#endif
    }
}
