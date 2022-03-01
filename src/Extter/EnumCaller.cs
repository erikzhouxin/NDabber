﻿using System;
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
        /// <summary>
        /// 获取所有枚举类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T[] GetAllEnums<T>() where T : Enum
        {
            return (T[])Enum.GetValues(typeof(T));
        }
        /// <summary>
        /// 获取所有枚举类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ignore"></param>
        /// <returns></returns>
        public static T[] GetAllEnums<T>(this T ignore) where T : Enum
        {
            return (T[])Enum.GetValues(typeof(T));
        }
        /// <summary>
        /// 获取除此之外的所有枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T[] GetEnums<T>(this T ignore) where T : Enum
        {
            var result = ((T[])Enum.GetValues(typeof(T))).ToList();
            result.Remove(ignore);
            return result.ToArray();
        }
        /// <summary>
        /// 获取此值后面的枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T[] GetRightEnums<T>(this T ignore) where T : Enum
        {
            var result = ((T[])Enum.GetValues(typeof(T))).ToList();
            var index = result.IndexOf(ignore);
            if (index >= 0)
            {
                return result.Skip(index + 1).ToArray();
            }
            return result.ToArray();
        }
        /// <summary>
        /// 获取此值包含的所有枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static T[] GetFlags<T>(this T enumValue) where T : Enum
        {
            var eval = (EDisplayAttr<T>)enumValue;
            if(eval.Value <= 0) { return new T[] { eval.Enum }; }
            var result = new List<T>();
            foreach (var item in EDisplayAttr<T>.Attrs)
            {
                if((item.Value & eval.Value) > 0)
                {
                    result.Add(item.Enum);
                }
            }
            return result.ToArray();
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
