using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// 判断调用
    /// </summary>
    public static partial class CobberCaller
    {
        /// <summary>
        /// 转换成布尔值
        /// </summary>
        /// <returns></returns>
        public static bool ToCBoolean(this string value, bool defVal = false)
        {
            return string.IsNullOrEmpty(value) ? defVal : Convert.ToBoolean(value);
        }
        /// <summary>
        /// 转换成布尔值
        /// </summary>
        /// <returns></returns>
        public static bool ToPBoolean(this string value, bool defVal = false)
        {
            if (bool.TryParse(value, out bool result))
            {
                return result;
            }
            return defVal;
        }
        /// <summary>
        /// 将字符串转换成布尔值
        /// </summary>
        /// <param name="value">待转换字符串,是否,对错,真假等</param>
        /// <param name="defVal">出现其他值后返回值,默认为False</param>
        /// <returns></returns>
        public static bool GetBoolean(this string value, bool defVal = false)
        {
            if (value == null) { return defVal; }
            switch (value.ToUpper())
            {
                case "Y":
                case "T":
                case "TRUE":
                case "YES":
                case "是":
                case "对":
                case "真":
                case "合格":
                case "同意":
                case "行":
                case "可以":
                    return true;
                case "N":
                case "F":
                case "FALSE":
                case "NO":
                case "否":
                case "错":
                case "假":
                case "不":
                case "不合格":
                case "不同意":
                case "不行":
                case "不可以":
                    return false;
                default:
                    return defVal;
            }
        }
        /// <summary>
        /// 可为空的是True
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsTrue(this bool? value)
        {
            return value.HasValue && value.Value;
        }
        /// <summary>
        /// 可为空的是False
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsFalse(this bool? value)
        {
            return value.HasValue && !value.Value;
        }
        /// <summary>
        /// 是为空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool IsNull<T>(this T model)
        {
            return model == null;
        }
        /// <summary>
        /// 不是为空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool IsNotNull<T>(this T model)
        {
            return model != null;
        }
        /// <summary>
        /// 为Null或空
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }
        /// <summary>
        /// 为Null或空
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> value)
        {
#if NETFrame
            return value == null || value.Count() == 0;
#else
            return value == null || !value.Any();
#endif
        }
        /// <summary>
        /// 不为Null或空
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNotNullOrEmpty(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }
        /// <summary>
        /// 为Null或空
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> value)
        {
#if NETFrame
            return value != null && value.Count() > 0;
#else
            return value != null && value.Any();
#endif
        }
        /// <summary>
        /// 为NULL或空白字符
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
        /// <summary>
        /// 不为NULL或空白字符
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNotNullOrWhiteSpace(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }
        /// <summary>
        /// 为NULL/空/空白
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
        /// <summary>
        /// 不为NULL/空/空白
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNotEmpty(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }
        /// <summary>
        /// 为空串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEmpty<T>(this IEnumerable<T> value)
        {
#if NETFrame
            return value == null || value.Count() == 0;
#else
            return value == null || !value.Any();
#endif
        }
        /// <summary>
        /// 不为空串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNotEmpty<T>(this IEnumerable<T> value)
        {
#if NETFrame
            return value != null && value.Count() > 0;
#else
            return value != null && value.Any();
#endif
        }
        /// <summary>
        /// 为空数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEmpty<T>(this T[] value)
        {
            return value == null || value.Length == 0;
        }
        /// <summary>
        /// 不为空数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNotEmpty<T>(this T[] value)
        {
            return value != null && value.Length > 0;
        }
        /// <summary>
        /// 是否为默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool IsDefault<T>(this T model)
        {
            return model == null ? default(T) == null : model.Equals(default(T));
        }
        /// <summary>
        /// 是否为默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool IsNotDefault<T>(this T model)
        {
            return model == null ? default(T) != null : !model.Equals(default(T));
        }
    }
}

namespace System.Data.Extter
{
    public static partial class ExtterCaller
    {
        /// <summary>
        /// IF语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="FuncIf"></param>
        /// <param name="GetTrue"></param>
        /// <param name="GetFalse"></param>
        /// <returns></returns>
        public static T If<T>(this T model, Func<T, bool> FuncIf, Func<T, T> GetTrue, Func<T, T> GetFalse)
        {
            return FuncIf(model) ? GetTrue(model) : GetFalse(model);
        }
        /// <summary>
        /// IF语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="FuncIf"></param>
        /// <param name="trueValue"></param>
        /// <param name="GetFalse"></param>
        /// <returns></returns>
        public static T If<T>(this T model, Func<T, bool> FuncIf, T trueValue, Func<T, T> GetFalse)
        {
            return FuncIf(model) ? trueValue : GetFalse(model);
        }
        /// <summary>
        /// IF语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="FuncIf"></param>
        /// <param name="GetTrue"></param>
        /// <param name="falseValue"></param>
        /// <returns></returns>
        public static T If<T>(this T model, Func<T, bool> FuncIf, Func<T, T> GetTrue, T falseValue)
        {
            return FuncIf(model) ? GetTrue(model) : falseValue;
        }
        /// <summary>
        /// IF语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="FuncIf"></param>
        /// <param name="falseValue"></param>
        /// <returns></returns>
        public static T IfTrue<T>(this T model, Func<T, bool> FuncIf, T falseValue)
        {
            return FuncIf(model) ? model : falseValue;
        }
        /// <summary>
        /// IF语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="FuncIf"></param>
        /// <param name="GetFalse"></param>
        /// <returns></returns>
        public static T IfTrue<T>(this T model, Func<T, bool> FuncIf, Func<T, T> GetFalse)
        {
            return FuncIf(model) ? model : GetFalse(model);
        }
        /// <summary>
        /// IF语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="FuncIf"></param>
        /// <param name="trueValue"></param>
        /// <returns></returns>
        public static T IfFalse<T>(this T model, Func<T, bool> FuncIf, T trueValue)
        {
            return FuncIf(model) ? model : trueValue;
        }
        /// <summary>
        /// IF语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="FuncIf"></param>
        /// <param name="GetTrue"></param>
        /// <returns></returns>
        public static T IfFalse<T>(this T model, Func<T, bool> FuncIf, Func<T, T> GetTrue)
        {
            return FuncIf(model) ? model : GetTrue(model);
        }
        /// <summary>
        /// 当为空时继续查找后续内容
        /// </summary>
        /// <param name="value"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string IfEmpty(this string value, params string[] args)
        {
            if (!string.IsNullOrEmpty(value)) { return value; }
            if (args == null || args.Length == 0) { return value; }
            foreach (var item in args)
            {
                if (!string.IsNullOrEmpty(item)) { return item; }
            }
            return string.Empty;
        }
    }
}