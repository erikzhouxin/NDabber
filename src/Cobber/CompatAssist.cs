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
        /// <param name="isHerit"></param>
        /// <returns></returns>
        public static T GetCustomAttribute<T>(this MemberInfo member, bool isHerit = false) where T : Attribute
        {
            foreach (var attr in member.GetCustomAttributes(typeof(T), isHerit))
            {
                if (attr is T real) { return real; }
            }
            return default;
        }
        /// <summary>
        /// 获取用户属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static T GetCustomAttribute<T>(this PropertyInfo prop, bool inherit = false) where T : Attribute
        {
            foreach (var item in prop.GetCustomAttributes(inherit))
            {
                if (item is T) { return item as T; }
            }
            return default(T);
        }
        /// <summary>
        /// 获取用户属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static T GetCustomAttribute<T>(this Type type, bool inherit = false) where T : Attribute
        {
            foreach (var item in type.GetCustomAttributes(inherit))
            {
                if (item is T) { return item as T; }
            }
            return default(T);
        }
#endif
    }
}
