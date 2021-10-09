using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// 枚举调用
    /// </summary>
    public static partial class CobberCaller
    {
        /// <summary>
        /// 转换成枚举名称字符串
        /// </summary>
        /// <param name="enumValue">枚举值</param>
        /// <returns></returns>
        public static string GetEnumName<T>(this T enumValue) where T : Enum
        {
            return Enum.GetName(typeof(T), enumValue);
        }
        /// <summary>
        /// 获取枚举名称
        /// </summary>
        /// <see cref="EDisplayAttribute"/>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="enumValue">枚举值</param>
        /// <returns></returns>
        public static String GetEDisplayName<T>(this T enumValue) where T : Enum => GetEDisplayAttribute<T>(enumValue).Display;
        /// <summary>
        /// 获取EDisplayAttribute显示
        /// </summary>
        /// <see cref="EDisplayAttribute"/>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="t">枚举值</param>
        /// <returns></returns>
        public static EDisplayAttribute GetEDisplayAttribute<T>(T t) where T : Enum
        {
            var type = typeof(T);
            var prop = type.GetField(Enum.GetName(type, t));
            if (prop == null) { return new EDisplayAttribute("未知"); }
            var attr = prop.GetCustomAttribute<EDisplayAttribute>();
            if (attr != null) { return attr; }
            return new EDisplayAttribute("未知");
        }
    }
}

namespace System.Data.Extter
{
    /// <summary>
    /// 枚举调用
    /// </summary>
    public static class EnumCaller
    {
    }
}
