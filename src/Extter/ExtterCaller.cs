using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data.Cobber;
using System.Data.Extter;
using System.Data.Impeller;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace System.Data.Cobber
{
    /// <summary>
    /// 扩展静态调用内容
    /// </summary>
    public static partial class CobberCaller
    {
        #region // 布尔判断 Boolean
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
        public static bool GetBoolean(this string value, bool defVal = false) => GetNBoolean(value, defVal) ?? defVal;
        /// <summary>
        /// 将字符串转换成布尔值
        /// </summary>
        /// <param name="value">待转换字符串,是否,对错,真假等</param>
        /// <param name="defVal">出现其他值后返回值,默认为False</param>
        /// <returns></returns>
        public static bool? GetNBoolean(this string value, bool? defVal = null)
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
        #endregion 布尔判断 Boolean

        #region // 字节编码 Byte Encoding
        /// <summary>
        /// 默认编码
        /// </summary>
        public static Encoding DefaultEncoding { get; set; } = Encoding.UTF8;
        /// <summary>
        /// 转换成字节数组
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this string content) => GetBytes(content, DefaultEncoding);
        /// <summary>
        /// 转换成字节数组
        /// </summary>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this String content, Encoding encoding)
        {
            return encoding.GetBytes(content);
        }
        /// <summary>
        /// 转换成字符串
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static String GetString(this byte[] content) => GetString(content, DefaultEncoding);
        /// <summary>
        /// 转换成字节数组
        /// </summary>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string GetString(this byte[] content, Encoding encoding)
        {
            return encoding.GetString(content);
        }
        /// <summary>
        /// 比较两个字节数组是否相同内容
        /// </summary>
        /// <param name="curr"></param>
        /// <param name="tag"></param>
        /// <param name="isNull">同时为NULL时是否相等,默认相等</param>
        /// <returns></returns>
        public static bool EqualBytes(this byte[] curr, byte[] tag, bool isNull = true)
        {
            if (curr == null && tag == null) { return isNull; }
            if (curr == null || tag == null) { return false; }
            if (curr.Length != tag.Length) { return false; }
            for (int i = 0; i < curr.Length; i++)
            {
                if (curr[i] != tag[i])
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 将字节数组转换成16进制字符串
        /// </summary>
        /// <param name="hashData">字节数组</param>
        /// <see cref="UserCrypto.GetHexString(byte[],bool)"/>
        /// <returns>16进制字符串(大写字母)</returns>
        public static string GetHexString(this byte[] hashData) => UserCrypto.GetHexString(hashData);
        /// <summary>
        /// 将字节数组转换成16进制字符串
        /// </summary>
        /// <param name="hashData">字节数组</param>
        /// <param name="isLower">是小写</param>
        /// <see cref="UserCrypto.GetHexString(byte[],bool)"/>
        /// <returns>16进制字符串</returns>
        public static string GetHexString(this byte[] hashData, bool isLower) => UserCrypto.GetHexString(hashData, isLower);
        #endregion 字节编码 Byte Encoding

        #region // 日期时间 DateTime
        /// <summary>
        /// 获取当前天是星期几
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static String GetDayOfWeek(this DateTime time)
        {
            switch (time.DayOfWeek)
            {
                case DayOfWeek.Sunday: return "星期日";
                case DayOfWeek.Monday: return "星期一";
                case DayOfWeek.Tuesday: return "星期二";
                case DayOfWeek.Wednesday: return "星期三";
                case DayOfWeek.Thursday: return "星期四";
                case DayOfWeek.Friday: return "星期五";
                case DayOfWeek.Saturday: return "星期六";
                default: return "周鑫";
            }
        }
        /// <summary>
        /// 日期格式
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static String GetDateString(this DateTime date) => date.ToString(Extter.ExtterCaller.DateFormatter);
        /// <summary>
        /// 时间格式
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static String GetTimeString(this DateTime time) => time.ToString(Extter.ExtterCaller.TimeFormatter);
        /// <summary>
        /// 日期时间格式
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static String GetDateTimeString(this DateTime dateTime) => dateTime.ToString(Extter.ExtterCaller.DateTimeFormatter);
        /// <summary>
        /// 日期格式
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static String GetDateString(this DateTimeOffset date) => date.ToString(Extter.ExtterCaller.DateFormatter);
        /// <summary>
        /// 时间格式
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static String GetTimeString(this DateTimeOffset time) => time.ToString(Extter.ExtterCaller.TimeFormatter);
        /// <summary>
        /// 日期时间格式
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static String GetDateTimeString(this DateTimeOffset dateTime) => dateTime.ToString(Extter.ExtterCaller.DateTimeFormatter);
        #endregion 日期时间 DateTime

        #region // 枚举调用 Enum NEnumerable
        /// <summary>
        /// 转换成枚举名称字符串
        /// </summary>
        /// <param name="enumValue">枚举值</param>
        /// <returns></returns>
        public static string GetEnumName<T>(this T enumValue) where T : struct, Enum
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
        public static String GetEDisplayName<T>(this T enumValue) where T : struct, Enum => GetEDisplayAttribute<T>(enumValue).Display;
        /// <summary>
        /// 获取EDisplayAttribute显示
        /// </summary>
        /// <see cref="EDisplayAttribute"/>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="t">枚举值</param>
        /// <returns></returns>
        public static EDisplayAttribute GetEDisplayAttribute<T>(T t) where T : struct, Enum
        {
            return NEnumerable<T>.GetFromEnum(t).EDisplay;
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
        public static T[] GetFlags<T>(this T enumValue) where T : struct, Enum
        {
            var enumVal = enumValue.GetIntValue();
            var result = new List<T>();
            foreach (var item in NEnumerable<T>.Attrs)
            {
                if (item.Value == enumVal)
                {
                    result.Add(item.Enum);
                    continue;
                }
                if ((item.Value & enumVal) > 0)
                {
                    result.Add(item.Enum);
                }
            }
            return result.ToArray();
        }
        /// <summary>
        /// 获取枚举的Int32值
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static int GetIntValue(this Enum e)
        {
            return e.GetValue<int>();
        }
        /// <summary>
        /// 获取枚举的Int32值
        /// </summary>
        /// <returns></returns>
        public static int GetValue(this Enum e)
        {
            return e.GetValue<int>();
        }
        /// <summary>
        /// 获取枚举的指定类型,如Int16/Int36/Byte/UInt32/Long等
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        public static T GetValue<T>(this Enum e) where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
        {
            try
            {
                return (T)(object)e;
            }
            catch
            {
                return (T)Convert.ChangeType(e, typeof(T));
            }
        }
        #endregion 枚举调用 Enum NEnumerable

        #region // 集合列表 List IEnumerable
        /// <summary>
        /// 转换成List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<T> AsList<T>(this IEnumerable<T> source) => source == null ? null : source is List<T> result ? result : source.ToList();

        /// <summary>
        /// 通知序列集合
        /// </summary>
        /// <returns></returns>
        public static List<T> Append<T>(this List<T> model, IEnumerable<T> list) => AppendRange(model, list);
        /// <summary>
        /// 通知序列集合
        /// </summary>
        /// <returns></returns>
        public static List<T> AppendRange<T>(this List<T> model, IEnumerable<T> list)
        {
            model.AddRange(list);
            return model;
        }
        /// <summary>
        /// 通知序列集合
        /// </summary>
        /// <returns></returns>
        public static List<T> Append<T>(this List<T> model, T item)
        {
            model.Add(item);
            return model;
        }
        /// <summary>
        /// 通知序列集合
        /// </summary>
        /// <returns></returns>
        public static List<T> Append<T>(this List<T> model, int index, T item)
        {
            model.Insert(index, item);
            return model;
        }
        /// <summary>
        /// 通知序列集合
        /// </summary>
        /// <returns></returns>
        public static List<T> Input<T>(this List<T> model, int index, T item)
        {
            model.Insert(index, item);
            return model;
        }
        /// <summary>
        /// 附加内容
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> Append<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, TValue value)
        {
            dic[key] = value;
            return dic;
        }
        /// <summary>
        /// 通知序列集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static ObservableCollection<T> ToObservable<T>(this IEnumerable<T> list)
        {
            return new ObservableCollection<T>(list);
        }
        /// <summary>
        /// 通知序列集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static ObservableCollection<T> AsObservable<T>(this IEnumerable<T> list)
        {
            return list == null ? null : (list is ObservableCollection<T> olist ? olist : new ObservableCollection<T>(list));
        }
        /// <summary>
        /// 通知序列集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static ObservableCollection<T> AsObservable<T>(this IEnumerable<object> list)
            where T : class
        {
            if (list == null) { return null; }
            if (list is ObservableCollection<T> olist) { return olist; }
            var res = new ObservableCollection<T>();
            foreach (var item in list)
            {
                res.Add(item as T);
            }
            return res;
        }
        /// <summary>
        /// 通知序列集合
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="list"></param>
        /// <param name="Convert"></param>
        /// <returns></returns>
        public static ObservableCollection<T2> AsObservable<T1, T2>(this IEnumerable<T1> list, Func<T1, T2> Convert)
        {
            if (list == null) { return null; }
            var res = new ObservableCollection<T2>();
            foreach (var item in list)
            {
                res.Add(Convert(item));
            }
            return res;
        }
        /// <summary>
        /// 通知序列集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static IEnumerable<T> ToEnumerable<T>(this Array array)
        {
            foreach (T item in array)
            {
                yield return item;
            }
        }
        /// <summary>
        /// 获取或默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="condition"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public static T GetOrDefault<T>(this IEnumerable<T> list, Func<T, bool> condition, T defVal = default(T))
        {
            foreach (var item in list)
            {
                if (condition(item)) { return item; }
            }
            return defVal;
        }
        /// <summary>
        /// 获取或默认值
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static object GetOrDefault(this IEnumerable list)
        {
            foreach (var item in list) { return item; }
            return null;
        }
        /// <summary>
        /// 获取或默认值
        /// </summary>
        /// <param name="list"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static object GetOrDefault(this IEnumerable list, Func<object, bool> condition)
        {
            foreach (var item in list)
            {
                if (condition(item)) { return item; }
            }
            return null;
        }
        /// <summary>
        /// 获取或默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static T GetDefault<T>(this IEnumerable<T> list, Func<T, bool> condition)
        {
            foreach (var item in list)
            {
                if (condition(item)) { return item; }
            }
            return (T)list.FirstOrDefault();
        }
        /// <summary>
        /// 获取或默认值
        /// </summary>
        /// <param name="list"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static object GetDefault(this IEnumerable list, Func<object, bool> condition)
        {
            foreach (var item in list)
            {
                if (condition(item)) { return item; }
            }
            foreach (var item in list) { return item; }
            return null;
        }
        /// <summary>
        /// 获取或默认值
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static object GetDefault(this IEnumerable list)
        {
            foreach (var item in list) { return item; }
            return null;
        }
        /// <summary>
        /// 转换成整型数组
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int[] ToArray(this Array array)
        {
            if (array == null || array.Length == 0) { return new int[] { }; }
            var result = new int[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = Convert.ToInt32(array.GetValue(i));
            }
            return result;
        }
        /// <summary>
        /// 转换成数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static T[] ToArray<T>(this Array array)
        {
            if (array == null || array.Length == 0) { return new T[] { }; }
            var result = new T[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = (T)array.GetValue(i);
            }
            return result;
        }
        /// <summary>
        /// 转换成列表
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<T> ToEnumerable<T>(this IEnumerable list)
        {
            foreach (T item in list)
            {
                yield return item;
            }
        }
        /// <summary>
        /// 转换成列表
        /// </summary>
        /// <param name="list"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEnumerable<T> ToEnumerable<T>(this IEnumerable list, Func<object, T> action)
        {
            foreach (var item in list)
            {
                yield return action(item);
            }
        }
        /// <summary>
        /// 循环处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
        }
        /// <summary>
        /// 循环处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="action"></param>
        public static void TryForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                try { action.Invoke(item); } catch { }
            }
        }
        /// <summary>
        /// 循环处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="action"></param>
        public static IEnumerable<T> ForEach<T>(this IEnumerable list, Func<object, T> action)
        {
            foreach (var item in list)
            {
                yield return action(item);
            }
        }
        /// <summary>
        /// 循环处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="action"></param>
        public static IEnumerable<T> ForEach<T>(this IEnumerable list, Action<T> action)
        {
            var ls = new List<T>();
            foreach (T item in list)
            {
                action(item);
                ls.Add(item);
            }
            return ls;
        }
        /// <summary>
        /// 循环处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> list, Action<T, int> action)
        {
            int index = 0;
            foreach (var item in list)
            {
                action(item, index++);
            }
        }
        /// <summary>
        /// 循环处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="action"></param>
        public static void TryForEach<T>(this IEnumerable<T> list, Action<T, int> action)
        {
            int index = 0;
            foreach (var item in list)
            {
                try { action(item, index++); } catch { }
            }
        }
        /// <summary>
        /// 通知序列集合
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<T> AddRange<T>(this ObservableCollection<T> model, IEnumerable<T> list)
        {
            if (list == null || list.Count() == 0) { return model; }
            foreach (var item in list)
            {
                model.Add(item);
            }
            return model;
        }
        /// <summary>
        /// 通知序列集合
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<T> Append<T>(this ObservableCollection<T> model, IEnumerable<T> list) => AppendRange(model, list);
        /// <summary>
        /// 通知序列集合
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<T> AppendRange<T>(this ObservableCollection<T> model, IEnumerable<T> list)
        {
            if (list == null || list.Count() == 0) { return model; }
            foreach (var item in list)
            {
                model.Add(item);
            }
            return model;
        }
        /// <summary>
        /// 通知序列集合
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<T> Append<T>(this ObservableCollection<T> model, T item)
        {
            model.Add(item);
            return model;
        }
        /// <summary>
        /// 通知序列集合
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<T> Append<T>(this ObservableCollection<T> model, int index, T item)
        {
            model.Insert(index, item);
            return model;
        }
        /// <summary>
        /// 通知序列集合
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<T> Input<T>(this ObservableCollection<T> model, int index, T item)
        {
            model.Insert(index, item);
            return model;
        }
        /// <summary>
        /// 通知序列集合
        /// </summary>
        public static ObservableCollection<TResult> ToObservable<TModel, TResult>(this IEnumerable<TModel> list, Func<TModel, TResult> func)
        {
            return new ObservableCollection<TResult>(list.Select(func));
        }
        /// <summary>
        /// 反转数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T[] Reverse<T>(this T[] list)
        {
            Array.Reverse(list);
            return list;
        }
        /// <summary>
        /// 数组相同
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="curr"></param>
        /// <param name="tag"></param>
        /// <param name="isNull">当同时为NULL时是否相等,默认相等</param>
        /// <returns></returns>
        public static bool EqualArray<T>(this T[] curr, T[] tag, bool isNull = true)
        {
            if (curr == null && tag == null) { return isNull; }
            if (curr == null || tag == null) { return false; }
            if (curr.Length != tag.Length) { return false; }
            for (int i = 0; i < curr.Length; i++)
            {
                var c = curr[i];
                var t = tag[i];
                if (c == null && t == null)
                {
                    continue;
                }
                if (c == null || t == null)
                {
                    return false;
                }
                if (!c.Equals(t))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 获取元素内容
        /// 无null判断
        /// </summary>
        /// <returns></returns>
        public static T2 GetElement<T2>(this IEnumerable<object> objs)
        {
            foreach (var item in objs)
            {
                if (item is T2 attr)
                {
                    return attr;
                }
            }
            return default;
        }
        /// <summary>
        /// 获取元素内容
        /// 无null判断
        /// </summary>
        /// <returns></returns>
        public static T2 GetElement<T, T2>(this T[] objs) where T2 : T
        {
            foreach (var item in objs)
            {
                if (item is T2 attr)
                {
                    return attr;
                }
            }
            return default;
        }
        /// <summary>
        /// 获取元素内容
        /// 无null判断
        /// </summary>
        /// <returns></returns>
        public static T GetElement<T>(this T[] objs, string name)
        {
            foreach (var item in objs)
            {
                if (item.GetType().Name.EqualIgnoreCase2(name))
                {
                    return item;
                }
            }
            return default;
        }
        /// <summary>
        /// 获取元素内容
        /// 无null判断
        /// </summary>
        /// <returns></returns>
        public static T GetElement<T>(this T[] objs, Type type)
        {
            foreach (var item in objs)
            {
                if (item.GetType().Equals(type))
                {
                    return item;
                }
            }
            return default;
        }
        /// <summary>
        /// 获取元素内容
        /// 无null判断
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<T2> GetElements<T2>(this IEnumerable<object> objs)
        {
            foreach (var item in objs)
            {
                if (item is T2 attr)
                {
                    yield return attr;
                }
            }
        }
        /// <summary>
        /// 获取元素内容
        /// 无null判断
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<T2> GetElements<T, T2>(this T[] objs) where T2 : T
        {
            foreach (var item in objs)
            {
                if (item is T2 attr)
                {
                    yield return attr;
                }
            }
        }
        /// <summary>
        /// 获取元素内容
        /// 无null判断
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<T> GetElements<T>(this T[] objs, string typeName)
        {
            foreach (var item in objs)
            {
                if (item.GetType().Name.EqualIgnoreCase2(typeName))
                {
                    yield return item;
                }
            }
        }
        /// <summary>
        /// 获取元素内容
        /// 无null判断
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<T> GetElements<T>(this T[] objs, Type type)
        {
            foreach (var item in objs)
            {
                if (item.GetType().Equals(type))
                {
                    yield return item;
                }
            }
        }
        /// <summary>
        /// 防止重复键导致的异常
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="models"></param>
        /// <param name="GetKey"></param>
        /// <param name="GetValue"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> GetDictionary<TItem, TKey, TValue>(this IEnumerable<TItem> models, Func<TItem, TKey> GetKey, Func<TItem, TValue> GetValue)
        {
            var res = new Dictionary<TKey, TValue>();
            if (models == null) { return res; }
            foreach (var model in models)
            {
                res[GetKey(model)] = GetValue(model);
            }
            return res;
        }
        #endregion 集合列表 List IEnumerable

        #region // IP地址 IPAddress
        /// <summary>
        /// 取本机主机ip
        /// </summary>
        /// <returns></returns>
        public static IPAddress GetCurrent()
        {
            try
            {
                IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in ipEntry.AddressList)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip;
                    }
                }
                foreach (var ip in ipEntry.AddressList)
                {
                    //从IP地址列表中筛选出IPv6类型的IP地址
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (ip.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        return ip;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
            return IPAddress.Parse("127.0.0.1");
        }
        /// <summary>
        /// 取本机主机ip
        /// </summary>
        /// <returns></returns>
        public static IPAddress[] GetList()
        {
            try
            {
                return Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
            return new IPAddress[] { IPAddress.Parse("127.0.0.1") };
        }
        /// <summary>
        /// 获取IPv4的值
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static Int32 GetIPv4Value(string ip)
        {
            string[] items = ip.Split(new char[] { '.' });
            return Int32.Parse(items[0]) << 24 | Int32.Parse(items[1]) << 16 | Int32.Parse(items[2]) << 8 | Int32.Parse(items[3]);
        }
        /// <summary>
        /// 获取IPv4的值
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static string GetIPv4Address(int ip)
        {
            return $"{(byte)(ip >> 24)}.{(byte)(ip >> 16)}.{(byte)(ip >> 8)}.{(byte)ip}";
        }
        #endregion IP地址 IPAddress

        #region // 数字调用 Number
        /// <summary>
        /// 四舍五入
        /// </summary>
        public static MidpointRounding DefaultMidPoint { get; set; } = MidpointRounding.ToEven;
        /// <summary>
        /// 获取数字
        /// </summary>
        /// <returns></returns>
        public static double GetDouble(this double value, int digit = 2)
        {
            return Math.Round(value, digit, DefaultMidPoint);
        }
        /// <summary>
        /// 获取数字
        /// </summary>
        /// <returns></returns>
        public static double GetDouble(this decimal value, int digit = 2)
        {
            return (double)Math.Round(value, digit, DefaultMidPoint);
        }
        /// <summary>
        /// 获取数字
        /// </summary>
        /// <returns></returns>
        public static decimal GetDecimal(this decimal value, int digit = 2)
        {
            return Math.Round(value, digit, DefaultMidPoint);
        }
        /// <summary>
        /// 获取数字
        /// </summary>
        /// <returns></returns>
        public static decimal GetDecimal(this double value, int digit = 2)
        {
            return Math.Round((decimal)value, digit, DefaultMidPoint);
        }
        /// <summary>
        /// 获取数字
        /// </summary>
        /// <returns></returns>
        public static double GetDouble(this float value, int digit = 2)
        {
            return Math.Round(value, digit, DefaultMidPoint);
        }
        /// <summary>
        /// 获取数字
        /// </summary>
        /// <returns></returns>
        public static decimal GetDecimal(this float value, int digit = 2)
        {
            return Math.Round((decimal)value, digit, DefaultMidPoint);
        }
        /// <summary>
        /// 获取数字
        /// </summary>
        /// <returns></returns>
        public static double? GetDouble(this double? value, int digit = 2)
        {
            if (value == null) { return null; }
            return Math.Round(value.Value, digit, DefaultMidPoint);
        }
        /// <summary>
        /// 获取数字
        /// </summary>
        /// <returns></returns>
        public static double? GetDouble(this decimal? value, int digit = 2)
        {
            if (value == null) { return null; }
            return (double)Math.Round(value.Value, digit, DefaultMidPoint);
        }
        /// <summary>
        /// 获取数字
        /// </summary>
        /// <returns></returns>
        public static decimal? GetDecimal(this decimal? value, int digit = 2)
        {
            if (value == null) { return null; }
            return Math.Round(value.Value, digit, DefaultMidPoint);
        }
        /// <summary>
        /// 获取数字
        /// </summary>
        /// <returns></returns>
        public static decimal? GetDecimal(this double? value, int digit = 2)
        {
            if (value == null) { return null; }
            return Math.Round((decimal)value.Value, digit, DefaultMidPoint);
        }
        /// <summary>
        /// 获取数字
        /// </summary>
        /// <returns></returns>
        public static double? GetDouble(this float? value, int digit = 2)
        {
            if (value == null) { return null; }
            return Math.Round(value.Value, digit, DefaultMidPoint);
        }
        /// <summary>
        /// 获取数字
        /// </summary>
        /// <returns></returns>
        public static decimal? GetDecimal(this float? value, int digit = 2)
        {
            if (value == null) { return null; }
            return Math.Round((decimal)value.Value, digit, DefaultMidPoint);
        }
        /// <summary>
        /// 逻辑且一个值
        /// </summary>
        /// <param name="firstVal"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static int AndValue(this int firstVal, int second)
        {
            return firstVal & second;
        }
        /// <summary>
        /// 逻辑且一个值
        /// </summary>
        /// <param name="firstVal"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool IsAndValue(this int firstVal, int second)
        {
            return (firstVal & second) > 0;
        }
        /// <summary>
        /// 逻辑或一个值
        /// </summary>
        /// <param name="firstVal"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool IsOrValue(this int firstVal, int second)
        {
            return (firstVal | second) > 0;
        }
        /// <summary>
        /// 逻辑或一个值
        /// </summary>
        /// <param name="firstVal"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static int OrValue(this int firstVal, int second)
        {
            return firstVal | second;
        }
        /// <summary>
        /// 尝试转换成整型
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryToInt32(this string key, out int value)
        {
            return int.TryParse(key, out value);
        }
        /// <summary>
        /// 尝试转换成整型
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryToInt64(this string key, out Int64 value)
        {
            return Int64.TryParse(key, out value);
        }
        /// <summary>
        /// 尝试转换成浮点型
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryToDouble(this string key, out double value)
        {
            return double.TryParse(key, out value);
        }
        /// <summary>
        /// 尝试转换成浮点型
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryToDecimal(this string key, out decimal value)
        {
            return decimal.TryParse(key, out value);
        }
        /// <summary>
        /// 转换成整型
        /// </summary>
        /// <returns></returns>
        public static int ToCInt32(this string value, int defVal = 0)
        {
            return string.IsNullOrEmpty(value) ? defVal : Convert.ToInt32(value);
        }
        /// <summary>
        /// 转换成整型
        /// </summary>
        /// <returns></returns>
        public static Int64 ToCInt64(this string value, Int64 defVal = 0)
        {
            return string.IsNullOrEmpty(value) ? defVal : Convert.ToInt64(value);
        }
        /// <summary>
        /// 转换成整型
        /// </summary>
        /// <returns></returns>
        public static int ToPInt32(this string value, int defVal = 0)
        {
            if (int.TryParse(value, out int result))
            {
                return result;
            }
            return defVal;
        }
        /// <summary>
        /// 转换成整型
        /// </summary>
        /// <returns></returns>
        public static Int64 ToPInt64(this string value, Int64 defVal = 0)
        {
            if (Int64.TryParse(value, out Int64 result))
            {
                return result;
            }
            return defVal;
        }
        /// <summary>
        /// 转换成浮点型
        /// </summary>
        /// <returns></returns>
        public static double ToCDouble(this string value, double defVal = 0)
        {
            return string.IsNullOrEmpty(value) ? defVal : Convert.ToDouble(value);
        }
        /// <summary>
        /// 转换成浮点型
        /// </summary>
        /// <returns></returns>
        public static double ToPDouble(this string value, double defVal = 0)
        {
            if (double.TryParse(value, out double result))
            {
                return result;
            }
            return defVal;
        }
        /// <summary>
        /// 转换成浮点型
        /// </summary>
        /// <returns></returns>
        public static double? ToNullableDouble(this string value, double? defVal = null)
        {
            if (double.TryParse(value, out double result))
            {
                return result;
            }
            return defVal;
        }
        /// <summary>
        /// 转换成浮点型
        /// </summary>
        /// <returns></returns>
        public static decimal ToCDecimal(this string value, decimal defVal = 0)
        {
            return string.IsNullOrEmpty(value) ? defVal : Convert.ToDecimal(value);
        }
        /// <summary>
        /// 转换成浮点型
        /// </summary>
        /// <returns></returns>
        public static decimal? ToNullableDecimal(this string value, decimal? defVal = null)
        {
            if (string.IsNullOrEmpty(value)) { return defVal; }
            if (decimal.TryParse(value, out decimal result))
            {
                return result;
            }
            return defVal;
        }
        /// <summary>
        /// 转换成浮点型
        /// </summary>
        /// <returns></returns>
        public static decimal ToPDecimal(this string value, decimal defVal = 0)
        {
            if (decimal.TryParse(value, out decimal result))
            {
                return result;
            }
            return defVal;
        }
        #endregion 数字调用 Number

        #region // 序列化 Serialize Newtonsoft Json
        /// <summary>
        /// 默认设置
        /// </summary>
        public static JsonSerializerSettings DefaultNewtonsoftSetting { get; } = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        };
        /// <summary>
        /// 当前默认设置
        /// </summary>
        public static JsonSerializerSettings CurrentNewtonsoftSetting { get; set; } = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        };
        /// <summary>
        /// 当前应用设置
        /// </summary>
        public static JsonSerializerSettings CurrentAppNewtonsoftSetting { get; set; } = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        };
        /// <summary>
        /// 当前桌面设置
        /// </summary>
        public static JsonSerializerSettings CurrentPcNewtonsoftSetting { get; set; } = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
        };
        /// <summary>
        /// 当前网站设置
        /// </summary>
        public static JsonSerializerSettings CurrentWebsiteNewtonsoftSetting { get; set; } = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new DefaultContractResolver(),
            DateFormatString = "yyyy-MM-dd HH:mm:ss",
            NullValueHandling = NullValueHandling.Ignore,
        };
        /// <summary>
        /// Web默认设置
        /// </summary>
        public static JsonSerializerSettings WebNewtonsoftSetting { get; } = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new DefaultContractResolver(),
            DateFormatString = "yyyy-MM-dd HH:mm:ss",
        };
        /// <summary>
        /// 帕斯卡(大驼峰)命名
        /// </summary>
        public static JsonSerializerSettings PascalCaseNewtonsoftSetting { get; } = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new DefaultContractResolver(),
            DateFormatString = "yyyy-MM-dd HH:mm:ss",
        };
        /// <summary>
        /// 小驼峰命名
        /// </summary>
        public static JsonSerializerSettings CamelCaseNewtonsoftSetting { get; } = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DateFormatString = "yyyy-MM-dd HH:mm:ss",
        };
        /// <summary>
        /// 小写属性命名
        /// </summary>
        public static JsonSerializerSettings LowerNewtonsoftSetting { get; } = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new LowercaseContractResolver(),
            DateFormatString = "yyyy-MM-dd HH:mm:ss",
        };
        /// <summary>
        /// 大写属性命名
        /// </summary>
        public static JsonSerializerSettings UpperNewtonsoftSetting { get; } = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new UppercaseContractResolver(),
            DateFormatString = "yyyy-MM-dd HH:mm:ss",
        };
        /// <summary>
        /// 蛇形属性命名
        /// </summary>
        public static JsonSerializerSettings SnakeNewtonsoftSetting { get; } = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new DefaultContractResolver() { NamingStrategy = new SnakeCaseNamingStrategy() },
            DateFormatString = "yyyy-MM-dd HH:mm:ss",
        };
        /// <summary>
        /// 获取对象的Json字符串
        /// Newtonsoft.Json.JsonConvert
        /// </summary>
        public static string GetJsonString<T>(this T value) => JsonConvert.SerializeObject(value, CurrentNewtonsoftSetting);
        /// <summary>
        /// 获取对象的Json字符串
        /// Newtonsoft.Json.JsonConvert
        /// </summary>
        public static bool TryGetJsonString<T>(this T value, out string jsonString, string defVal = "")
        {
            try
            {
                jsonString = JsonConvert.SerializeObject(value, CurrentNewtonsoftSetting);
                return true;
            }
            catch (Exception ex) { Console.WriteLine(ex); }
            jsonString = defVal;
            return false;
        }
        /// <summary>
        /// 获取对象的Json字符串
        /// Newtonsoft.Json.JsonConvert
        /// </summary>
        public static string GetJsonString<T>(this T value, JsonSerializerSettings settings) => JsonConvert.SerializeObject(value, settings);
        /// <summary>
        /// 获取对象的Json字符串
        /// Newtonsoft.Json.JsonConvert
        /// </summary>
        public static bool TryGetJsonString<T>(this T value, JsonSerializerSettings settings, out string jsonString, string defVal = "")
        {
            try
            {
                jsonString = JsonConvert.SerializeObject(value, settings);
                return true;
            }
            catch (Exception ex) { Console.WriteLine(ex); }
            jsonString = defVal;
            return false;
        }
        /// <summary>
        /// 获取对象的Json字符串
        /// Newtonsoft.Json.JsonConvert
        /// </summary>
        public static string TryGetJsonString<T>(this T value, JsonSerializerSettings settings, string defVal = "")
        {
            try { return JsonConvert.SerializeObject(value, settings); }
            catch (Exception ex) { Console.WriteLine(ex); }
            return defVal;
        }
        /// <summary>
        /// 获取格式化的Json代码
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String GetJsonFormatString<T>(this T value) => GetJsonFormatString(value, CurrentNewtonsoftSetting);
        /// <summary>
        /// 获取格式化的Json代码
        /// </summary>
        /// <returns></returns>
        public static bool TryGetJsonFormatString<T>(this T value, out string jsonString, string defVal = "")
        {
            try
            {
                jsonString = GetJsonFormatString(value, CurrentNewtonsoftSetting);
                return true;
            }
            catch (Exception ex) { Console.WriteLine(ex); }
            jsonString = defVal;
            return false;
        }
        /// <summary>
        /// 获取格式化的Json代码
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static String GetJsonFormatString<T>(this T value, JsonSerializerSettings settings)
        {
            StringWriter textWriter = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
            {
                Formatting = Formatting.Indented,
                Indentation = 4,
                IndentChar = ' '
            };
            //格式化json字符串
            JsonSerializer.Create(settings).Serialize(jsonWriter, value);
            return textWriter.ToString();
        }
        /// <summary>
        /// 获取格式化的Json代码
        /// </summary>
        /// <returns></returns>
        public static bool TryGetJsonFormatString<T>(this T value, JsonSerializerSettings settings, out string jsonString, string defVal = "")
        {
            try
            {
                jsonString = GetJsonFormatString(value, settings);
                return true;
            }
            catch (Exception ex) { Console.WriteLine(ex); }
            jsonString = defVal;
            return false;
        }
        /// <summary>
        /// 转换成对象
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static dynamic GetJsonObject(this string json) => JsonConvert.DeserializeObject(json, CurrentNewtonsoftSetting);
        /// <summary>
        /// 转换成对象
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static dynamic TryGetJsonObject(this string json) => TestTry.Try(JsonConvert.DeserializeObject, json, CurrentNewtonsoftSetting, default(object));
        /// <summary>
        /// 转换成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T GetJsonObject<T>(this string json) => JsonConvert.DeserializeObject<T>(json, CurrentNewtonsoftSetting);
        /// <summary>
        /// 转换成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T TryGetJsonObject<T>(this string json) => TestTry.Try(JsonConvert.DeserializeObject<T>, json, CurrentNewtonsoftSetting, default(T));
        /// <summary>
        /// 转换成对象
        /// </summary>
        /// <returns></returns>
        public static T TryGetJsonObject<T>(this string json, T model = default(T)) => TestTry.Try(JsonConvert.DeserializeObject<T>, json, CurrentNewtonsoftSetting, model);
        /// <summary>
        /// 转换成对象
        /// </summary>
        /// <param name="json"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static dynamic GetJsonObject(this string json, JsonSerializerSettings settings) => JsonConvert.DeserializeObject(json, settings);
        /// <summary>
        /// 转换成对象
        /// </summary>
        /// <returns></returns>
        public static dynamic TryGetJsonObject(this string json, JsonSerializerSettings settings, object model = default(object)) => TestTry.Try(JsonConvert.DeserializeObject, json, settings, model);
        /// <summary>
        /// 转换成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static T GetJsonObject<T>(this string json, JsonSerializerSettings settings) => JsonConvert.DeserializeObject<T>(json, settings);
        /// <summary>
        /// 转换成对象
        /// </summary>
        /// <returns></returns>
        public static T TryGetJsonObject<T>(this string json, JsonSerializerSettings settings, T defVal = default(T)) => TestTry.Try(JsonConvert.DeserializeObject<T>, json, settings, defVal);
        /// <summary>
        /// 转换成对象
        /// </summary>
        /// <returns></returns>
        public static object GetJsonObject(this string json, Type type) => JsonConvert.DeserializeObject(json, type, CurrentNewtonsoftSetting);
        /// <summary>
        /// 转换成对象
        /// </summary>
        /// <returns></returns>
        public static object TryGetJsonObject(this string json, Type type) => TestTry.Try(JsonConvert.DeserializeObject, json, type, CurrentNewtonsoftSetting, default(object));
        /// <summary>
        /// 转换成对象
        /// </summary>
        /// <returns></returns>
        public static object GetJsonObject(this string json, Type type, JsonSerializerSettings settings) => JsonConvert.DeserializeObject(json, type, settings);
        /// <summary>
        /// 转换成对象
        /// </summary>
        /// <returns></returns>
        public static object TryGetJsonObject(this string json, Type type, JsonSerializerSettings settings) => TestTry.Try(JsonConvert.DeserializeObject, json, type, settings, default(object));
        /// <summary>
        /// 获取对象的Json字符串
        /// Newtonsoft.Json.JsonConvert
        /// </summary>
        public static string GetJsonWebString<T>(this T value) => JsonConvert.SerializeObject(value, WebNewtonsoftSetting);
        /// <summary>
        /// 获取对象的Json字符串
        /// Newtonsoft.Json.JsonConvert
        /// </summary>
        public static string TryGetJsonWebString<T>(this T value, string defVal = "") => TryGetJsonString(value, WebNewtonsoftSetting, defVal);
        /// <summary>
        /// 获取对象的Json字符串(小写属性)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetJsonLowerString<T>(this T value) => JsonConvert.SerializeObject(value, LowerNewtonsoftSetting);
        /// <summary>
        /// 获取对象的Json字符串(小写属性)
        /// </summary>
        /// <returns></returns>
        public static string GetJsonLowerString<T>(this T value, string defVal = "") => TryGetJsonString(value, LowerNewtonsoftSetting, defVal);
        /// <summary>
        /// 获取对象的Json字符串(大写属性)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetJsonUpperString<T>(this T value) => JsonConvert.SerializeObject(value, UpperNewtonsoftSetting);
        /// <summary>
        /// 获取对象的Json字符串(大写属性)
        /// </summary>
        /// <returns></returns>
        public static string TryGetJsonUpperString<T>(this T value, string defVal = "") => TryGetJsonString(value, UpperNewtonsoftSetting, defVal);
        /// <summary>
        /// 获取对象的Json字符串(蛇形属性)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetJsonSnakeString<T>(this T value) => JsonConvert.SerializeObject(value, SnakeNewtonsoftSetting);
        /// <summary>
        /// 获取对象的Json字符串(蛇形属性)
        /// </summary>
        /// <returns></returns>
        public static string TryGetJsonSnakeString<T>(this T value, string defVal = "") => TryGetJsonString(value, SnakeNewtonsoftSetting, defVal);
        #endregion 序列化 Serialize Newtonsoft Json

        #region // 字符串调用 String
        /// <summary>
        /// 全为零的GUID(无连接符)
        /// </summary>
        public static String GuidEmpty { get; } = Guid.Empty.ToString("N");
        /// <summary>
        /// 全为零的GUID(默认ToString)
        /// </summary>
        public static String GuidDefault { get; } = Guid.Empty.ToString();
        /// <summary>
        /// 获取一个全新的Guid字符串,无连接符
        /// </summary>
        public static String GuidString { get => Guid.NewGuid().ToString("N"); }
        /// <summary>
        /// 获取一个全新的Guid字符串,无连接符
        /// </summary>
        public static String GetGuidString() => Guid.NewGuid().ToString("N");
        /// <summary>
        /// 创建一个全新的Guid字符串,无连接符
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_"></param>
        /// <returns></returns>
        public static String CreateGuidString<T>(this T _) => Guid.NewGuid().ToString("N");
        /// <summary>
        /// 创建一个全新的Guid字符串,无连接符
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_"></param>
        /// <param name="fmt"></param>
        /// <returns></returns>
        public static String GetGuidString<T>(this T _, string fmt = "N") => Guid.NewGuid().ToString(fmt);
        /// <summary>
        /// 转换成字符串唯一值
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="fmt"></param>
        /// <returns></returns>
        public static string GetString(this Guid guid, string fmt = "N") => guid.ToString(fmt);
        /// <summary>
        /// 转换成标题的大小写
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string GetTitleCase(this string val)
        {
            return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(val);
        }
        /// <summary>
        /// 获取空的默认值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public static String GetEmptyDefault(this string value, string defVal)
        {
            return string.IsNullOrWhiteSpace(value) ? defVal : value;
        }
        /// <summary>
        /// 获取空的默认值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public static String GetEmptyDefault(this string value, Func<string> defVal)
        {
            return string.IsNullOrWhiteSpace(value) ? defVal?.Invoke() : value;
        }
        /// <summary>
        /// 连接字符串
        /// </summary>
        /// <param name="list"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public static String JoinString<T>(this IEnumerable<T> list, string split = ",")
        {
            return string.Join(split, list);
        }
        /// <summary>
        /// 连接字符串
        /// </summary>
        /// <param name="list"></param>
        /// <param name="GetValue"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public static String JoinString<T>(this IEnumerable<T> list, Func<T, string> GetValue, string split = ",")
        {
            var vals = new List<string>();
            list.ForEach((m) => vals.Add(GetValue(m)));
            return string.Join(split, vals);
        }
        /// <summary>
        /// 连接字符串
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static String JoinLine<T>(this IEnumerable<T> list)
        {
            if (list == null || list.Count() == 0) { return string.Empty; }
            var sb = new StringBuilder(string.Format("{0}", list.First()));
            foreach (var item in list.Skip(1))
            {
                sb.AppendLine().AppendFormat("{0}", item);
            }
            return sb.ToString();
        }
        /// <summary>
        /// 转换成大写
        /// </summary>
        /// <param name="cha"></param>
        /// <returns></returns>
        public static Char ToUpper(this char cha)
        {
            return Char.ToUpper(cha);
        }
        /// <summary>
        /// 转换成小写
        /// </summary>
        /// <param name="cha"></param>
        /// <returns></returns>
        public static Char ToLower(this char cha)
        {
            return Char.ToLower(cha);
        }
        /// <summary>
        /// 反转字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Reverse(this string data)
        {
            var chars = data.ToCharArray();
            Array.Reverse(chars);
            return new String(chars);
        }
        /// <summary>
        /// 格式化字符串
        /// String.FormatString
        /// </summary>
        /// <param name="fmt"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static String FString(this string fmt, params object[] args)
        {
            return string.Format(fmt, args);
        }
        /// <summary>
        /// 格式化字符串
        /// String.FormatString
        /// </summary>
        /// <param name="fmt"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static String FormatString(this string fmt, params object[] args)
        {
            return string.Format(fmt, args);
        }
#if NET40 || NET45
        /// <summary>
        /// 字符串包含字符
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Contains(this string data, char key)
        {
            return !string.IsNullOrEmpty(data) && data.IndexOf(key) >= 0;
        }
        /// <summary>
        /// 字符串包含字符
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public static bool Contains(this string data, char key, StringComparison comparison)
        {
            return !string.IsNullOrEmpty(data) && data.IndexOf(key.ToString(), 0, data.Length, comparison) >= 0;
        }
#endif
        #endregion 字符串调用 String
    }
}
namespace System.Data.Extter
{
    /// <summary>
    /// 扩展静态调用内容
    /// </summary>
    public static partial class ExtterCaller
    {
        static ExtterCaller()
        {
#if NETFx
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); // 注册编码格式
#endif
        }

        #region // 序列化 Serialize Newtonsoft Json
        /// <summary>
        /// 获取桌面Json字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string GetJsonPcString<T>(this T model) => JsonConvert.SerializeObject(model, CobberCaller.CurrentPcNewtonsoftSetting);
        /// <summary>
        /// Json转换成桌面对象
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static dynamic GetJsonPcObject(this string json) => JsonConvert.DeserializeObject(json, CobberCaller.CurrentPcNewtonsoftSetting);
        /// <summary>
        /// Json转换成桌面对象
        /// </summary>
        /// <returns></returns>
        public static object GetJsonPcObject(this string json, Type type) => JsonConvert.DeserializeObject(json, type, CobberCaller.CurrentPcNewtonsoftSetting);
        /// <summary>
        /// Json转换成桌面对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T GetJsonPcObject<T>(this string json) => JsonConvert.DeserializeObject<T>(json, CobberCaller.CurrentPcNewtonsoftSetting);
        /// <summary>
        /// 获取应用Json字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static String GetJsonAppString<T>(this T model) => JsonConvert.SerializeObject(model, CobberCaller.CurrentAppNewtonsoftSetting);
        /// <summary>
        /// Json转换成应用对象
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static dynamic GetJsonAppObject(this string json) => JsonConvert.DeserializeObject(json, CobberCaller.CurrentAppNewtonsoftSetting);
        /// <summary>
        /// Json转换成应用对象
        /// </summary>
        /// <returns></returns>
        public static object GetJsonAppObject(this string json, Type type) => JsonConvert.DeserializeObject(json, type, CobberCaller.CurrentAppNewtonsoftSetting);
        /// <summary>
        /// Json转换成应用对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T GetJsonAppObject<T>(this string json) => JsonConvert.DeserializeObject<T>(json, CobberCaller.CurrentAppNewtonsoftSetting);
        /// <summary>
        /// 获取网站Json字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static String GetJsonWebsiteString<T>(this T model) => JsonConvert.SerializeObject(model, CobberCaller.CurrentWebsiteNewtonsoftSetting);
        /// <summary>
        /// Json转换成网站对象
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static dynamic GetJsonWebsiteObject(this string json) => JsonConvert.DeserializeObject(json, CobberCaller.CurrentWebsiteNewtonsoftSetting);
        /// <summary>
        /// Json转换成网站对象
        /// </summary>
        /// <returns></returns>
        public static object GetJsonWebsiteObject(this string json, Type type) => JsonConvert.DeserializeObject(json, type, CobberCaller.CurrentWebsiteNewtonsoftSetting);
        /// <summary>
        /// Json转换成网站对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T GetJsonWebsiteObject<T>(this string json) => JsonConvert.DeserializeObject<T>(json, CobberCaller.CurrentWebsiteNewtonsoftSetting);
        #endregion 序列化 Serialize Newtonsoft Json

        #region // 提示信息内容 AlertMsg
        /// <summary>
        /// 获取泛型实例提示信息
        /// </summary>
        public static AlertMsg<T> GetAlert<T>(this T data) => data;
        /// <summary>
        /// 获取泛型实例提示信息
        /// </summary>
        public static AlertMsg<T> GetAlert<T>(this T data, string msg) => new AlertMsg<T>(data != null, msg) { Data = data };
        /// <summary>
        /// 获取动态实例提示信息
        /// </summary>
        public static AlertMessage GetAlertMessage<T>(this T data, string msg = null) => new AlertMessage(data != null, msg) { Data = data };
#if NETFrame
        /// <summary>
        /// 获取动态接口提示信息
        /// </summary>
        public static AlertMsg GetAlert(this Tuple<bool, string> res) => new AlertMsg(res.Item1, res.Item2);
        /// <summary>
        /// 获取动态实例提示信息
        /// </summary>
        public static AlertMessage GetAlertMessage(this Tuple<bool, string> res) => new AlertMessage(res.Item1, res.Item2);
#endif
#if NETFx
        /// <summary>
        /// 获取动态接口提示信息
        /// </summary>
        public static AlertMsg GetAlert(this (bool IsSuccess, String Message) res) => new AlertMsg(res.IsSuccess, res.Message);
        /// <summary>
        /// 获取动态实例提示信息
        /// </summary>
        public static AlertMessage GetAlertMessage(this (bool IsSuccess, String Message) res) => new AlertMessage(res.IsSuccess, res.Message);
#endif
        /// <summary>
        /// 获取动态接口提示信息
        /// </summary>
        public static AlertMsg GetAlert(this Tuble<bool, string> res) => new AlertMsg(res.Item1, res.Item2);
        /// <summary>
        /// 获取动态实例提示信息
        /// </summary>
        public static AlertMessage GetAlertMessage(this Tuble<bool, string> res) => new AlertMessage(res.Item1, res.Item2);
        #endregion 提示信息内容 AlertMsg

        #region // 类型扩展 Type
        /// <summary>
        /// 获取导出类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isExport"></param>
        /// <returns></returns>
        public static Type[] GetTypes(this Type type, bool isExport = true)
        {
            return isExport ? type.Assembly.GetExportedTypes() : type.Assembly.GetTypes();
        }
        /// <summary>
        /// 获取导出类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="nSpace"></param>
        /// <param name="isExport"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetTypes(this Type type, string nSpace, bool isExport = true)
        {
            var types = isExport ? type.Assembly.GetExportedTypes() : type.Assembly.GetTypes();
            if (string.IsNullOrEmpty(nSpace)) { return types; }
            return types.Where(s => nSpace.Equals(s.Namespace, StringComparison.OrdinalIgnoreCase));
        }
        /// <summary>
        /// 获取导出类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isExport"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetNamespaceTypes(this Type type, bool isExport = true)
        {
            var types = isExport ? type.Assembly.GetExportedTypes() : type.Assembly.GetTypes();
            return types.Where(s => s.Namespace == type.Namespace);
        }
        /// <summary>
        /// 获取类型所在程序集的名称类型
        /// </summary>
        /// <param name="type">当前类型</param>
        /// <param name="name">全称时使用[Type.Assembly.GetType],非全称使用遍历</param>
        /// <returns></returns>
        public static Type GetSameAssemblyType(this Type type, string name)
        {
            var fType = type.Assembly.GetType(name);
            if (fType != null) { return fType; }
            foreach (var item in type.Assembly.GetTypes())
            {
                if (item.Name == name) { return item; }
            }
            return null;
        }
        #endregion 类型扩展 Type

        #region // 布尔判断 Boolean
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
            return FuncIf(model) ? trueValue : model;
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
            return FuncIf(model) ? GetTrue(model) : model;
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
        #endregion 布尔判断 Boolean

        #region // 字节编码 Byte Encoding
        /// <summary>
        /// 获取MD5加密值
        /// </summary>
        /// <param name="bytes"></param>
        /// <see cref="UserCrypto.GetMd5Bytes"/>
        /// <returns></returns>
        public static byte[] GetMd5(this byte[] bytes) => UserCrypto.GetMd5Bytes(bytes);
        /// <summary>
        /// 获取MD5加密值
        /// </summary>
        /// <param name="bytes"></param>
        /// <see cref="UserCrypto.GetMd5HexString(byte[],bool)"/>
        /// <returns></returns>
        public static string GetMd5String(this byte[] bytes) => UserCrypto.GetMd5HexString(bytes, false);
        /// <summary>
        /// 将16进制字符串转换成字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] GetHexByte(this string hexString)
        {
            if ((hexString.Length % 2) != 0) { hexString += " "; }
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return returnBytes;
        }
        /// <summary>
        /// 获取字节数组的句柄
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static IntPtr GetIntPtr(this byte[] bytes)
        {
            var ptr = Marshal.AllocHGlobal(bytes.Length);
            Marshal.Copy(bytes, 0, ptr, bytes.Length);
            return ptr;
        }
        /// <summary>
        /// 将16进制字符串转换成字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] GetHexBytes(this string hexString)
        {
            if ((hexString.Length % 2) != 0) { hexString += " "; }
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return returnBytes;
        }
        /// <summary>
        /// 将16进制字符串转换成字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <param name="fix"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public static byte[] GetHexBytes(this string hexString, char fix, char replace = ' ')
        {
            hexString.Replace(replace.ToString(), "");
            if ((hexString.Length % 2) != 0) { hexString += fix; }
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return returnBytes;
        }
        /// <summary>
        /// 将16进制字符串转换成字节数组
        /// </summary>
        /// <returns></returns>
        public static byte[] GetHexBytes(this string hexString, char fix = ' ', string replace = " ")
        {
            hexString.Replace(replace, "");
            if ((hexString.Length % 2) != 0) { hexString += fix; }
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return returnBytes;
        }
        /// <summary>
        /// 将16进制字符串转换成字节数组
        /// </summary>
        /// <returns></returns>
        public static byte[] GetHexBytes(this string hexString, char fix, params char[] replaces)
        {
            foreach (var item in replaces)
            {
                hexString.Replace(item.ToString(), "");
            }
            if ((hexString.Length % 2) != 0) { hexString += fix; }
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return returnBytes;
        }
        #region // 转数字
        /// <summary>
        /// 读短整型数字
        /// </summary>
        /// <returns></returns>
        unsafe public static Int16 GetInt16(this byte[] bytes, bool isBigEndian = true)
        {
            if (bytes == null) { return 0; }
            if (isBigEndian) { Array.Reverse(bytes); }
            fixed (byte* ptr = bytes)
            {
                return *(short*)ptr;
            }
        }
        /// <summary>
        /// 读无符号短整型数字
        /// </summary>
        /// <returns></returns>
        unsafe public static UInt16 GetUInt16(this byte[] bytes, bool isBigEndian = true)
        {
            if (bytes == null) { return 0; }
            if (isBigEndian) { Array.Reverse(bytes); }
            fixed (byte* ptr = bytes)
            {
                return *(ushort*)ptr;
            }
        }
        /// <summary>
        /// 读整型数字
        /// </summary>
        /// <returns></returns>
        unsafe public static Int32 GetInt32(this byte[] bytes, bool isBigEndian = true)
        {
            if (bytes == null) { return 0; }
            if (isBigEndian) { Array.Reverse(bytes); }
            fixed (byte* ptr = bytes)
            {
                return *(int*)ptr;
            }
        }
        /// <summary>
        /// 读无符号整型数字
        /// </summary>
        /// <returns></returns>
        unsafe public static UInt32 GetUInt32(this byte[] bytes, bool isBigEndian = true)
        {
            if (bytes == null) { return 0; }
            if (isBigEndian) { Array.Reverse(bytes); }
            fixed (byte* ptr = bytes)
            {
                return *(uint*)ptr;
            }
        }
        /// <summary>
        /// 读长整型数字
        /// </summary>
        /// <returns></returns>
        unsafe public static Int64 GetInt64(this byte[] bytes, bool isBigEndian = true)
        {
            if (bytes == null) { return 0; }
            if (isBigEndian) { Array.Reverse(bytes); }
            fixed (byte* ptr = bytes)
            {
                return *(long*)ptr;
            }
        }
        /// <summary>
        /// 读无符号长整型数字
        /// </summary>
        /// <returns></returns>
        unsafe public static UInt64 GetUInt64(this byte[] bytes, bool isBigEndian = true)
        {
            if (bytes == null) { return 0; }
            if (isBigEndian) { Array.Reverse(bytes); }
            fixed (byte* ptr = bytes)
            {
                return *(ulong*)ptr;
            }
        }
        /// <summary>
        /// 读短整型数字
        /// </summary>
        /// <returns></returns>
        unsafe public static Int16 GetInt16(this sbyte[] bytes, bool isBigEndian = true)
        {
            if (bytes == null) { return 0; }
            if (isBigEndian) { Array.Reverse(bytes); }
            fixed (sbyte* ptr = bytes)
            {
                return *(short*)ptr;
            }
        }
        /// <summary>
        /// 读无符号短整型数字
        /// </summary>
        /// <returns></returns>
        unsafe public static UInt16 GetUInt16(this sbyte[] bytes, bool isBigEndian = true)
        {
            if (bytes == null) { return 0; }
            if (isBigEndian) { Array.Reverse(bytes); }
            fixed (sbyte* ptr = bytes)
            {
                return *(ushort*)ptr;
            }
        }
        /// <summary>
        /// 读整型数字
        /// </summary>
        /// <returns></returns>
        unsafe public static Int32 GetInt32(this sbyte[] bytes, bool isBigEndian = true)
        {
            if (bytes == null) { return 0; }
            if (isBigEndian) { Array.Reverse(bytes); }
            fixed (sbyte* ptr = bytes)
            {
                return *(int*)ptr;
            }
        }
        /// <summary>
        /// 读无符号整型数字
        /// </summary>
        /// <returns></returns>
        unsafe public static UInt32 GetUInt32(this sbyte[] bytes, bool isBigEndian = true)
        {
            if (bytes == null) { return 0; }
            if (isBigEndian) { Array.Reverse(bytes); }
            fixed (sbyte* ptr = bytes)
            {
                return *(uint*)ptr;
            }
        }
        /// <summary>
        /// 读长整型数字
        /// </summary>
        /// <returns></returns>
        unsafe public static Int64 GetInt64(this sbyte[] bytes, bool isBigEndian = true)
        {
            if (bytes == null) { return 0; }
            if (isBigEndian) { Array.Reverse(bytes); }
            fixed (sbyte* ptr = bytes)
            {
                return *(long*)ptr;
            }
        }
        /// <summary>
        /// 读无符号长整型数字
        /// </summary>
        /// <returns></returns>
        unsafe public static UInt64 GetUInt64(this sbyte[] bytes, bool isBigEndian = true)
        {
            if (bytes == null) { return 0; }
            if (isBigEndian) { Array.Reverse(bytes); }
            fixed (sbyte* ptr = bytes)
            {
                return *(ulong*)ptr;
            }
        }
        #endregion
        #region // 压缩
        /// <summary>
        /// 压缩字节
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Compress(this byte[] data)
        {
            var ms = new MemoryStream();
            var zip = new GZipStream(ms, CompressionMode.Compress, true);
            zip.Write(data, 0, data.Length);
            zip.Close();
            var buffer = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(buffer, 0, buffer.Length);
            ms.Close();
            return buffer;
        }

        /// <summary>
        /// 解压字节
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Decompress(this byte[] data)
        {
            var ms = new MemoryStream(data);
            var zip = new GZipStream(ms, CompressionMode.Decompress, true);
            var msreader = new MemoryStream();
            var buffer = new byte[0x1000];
            while (true)
            {
                var reader = zip.Read(buffer, 0, buffer.Length);
                if (reader <= 0)
                {
                    break;
                }
                msreader.Write(buffer, 0, reader);
            }
            zip.Close();
            ms.Close();
            msreader.Position = 0;
            buffer = msreader.ToArray();
            msreader.Close();
            return buffer;
        }
        #endregion
        #region // 转编码
        private static LazyBone<Encoding> _asciiEncoding = new LazyBone<Encoding>(() => Encoding.ASCII, true);
        private static LazyBone<Encoding> _gb2312Encoding = new LazyBone<Encoding>(() => Encoding.GetEncoding("GB2312"), true);
        private static LazyBone<Encoding> _gbkEncoding = new LazyBone<Encoding>(() => Encoding.GetEncoding("GBK"), true);
        private static LazyBone<Encoding> _utf8Encoding = new LazyBone<Encoding>(() => Encoding.UTF8, true);
        private static LazyBone<Encoding> _unicodeEncoding = new LazyBone<Encoding>(() => Encoding.Unicode, true);
        /// <summary>
        /// ASCII编码
        /// </summary>
        public static Encoding ASCIIEncoding { get => _asciiEncoding.Value; }
        /// <summary>
        /// GB2312编码
        /// </summary>
        public static Encoding GB2312Encoding { get => _gb2312Encoding.Value; }
        /// <summary>
        /// GBK编码
        /// </summary>
        public static Encoding GBKEncoding { get => _gbkEncoding.Value; }
        /// <summary>
        /// UTF8编码
        /// </summary>
        public static Encoding UTF8Encoding { get => _utf8Encoding.Value; }
        /// <summary>
        /// Unicode编码
        /// </summary>
        public static Encoding UnicodeEncoding { get => _unicodeEncoding.Value; }
        /// <summary>
        /// 默认编码
        /// </summary>
        public static Encoding DefaultEncoding { get => CobberCaller.DefaultEncoding; }
        /// <summary>
        /// 获取ASCII编码的字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String GetASCIIString(this byte[] value) => _asciiEncoding.Value.GetString(value);
        /// <summary>
        /// 获取ASCII进制字符串
        /// </summary>
        /// <param name="content"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        public static String GetASCIIHexString(this string content, bool isLower = false) => GetASCIIBytes(content).GetHexString(isLower);
        /// <summary>
        /// 获取16进制字符串的ASCII字符串
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static String GetHexASCIIString(this string hexString) => GetASCIIString(GetHexByte(hexString));
        /// <summary>
        /// 转换ASCII成字节数组
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] GetASCIIBytes(this string content) => _asciiEncoding.Value.GetBytes(content);
        /// <summary>
        /// 获取GB2312编码的字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String GetGB2312String(this byte[] value) => _gb2312Encoding.Value.GetString(value);
        /// <summary>
        /// 转换GBK成字节数组
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] GetGB2312Bytes(this string content) => _gb2312Encoding.Value.GetBytes(content);
        /// <summary>
        /// 获取GB2312进制字符串
        /// </summary>
        /// <param name="content"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        public static String GetGB2312HexString(this string content, bool isLower = false) => GetGB2312Bytes(content).GetHexString(isLower);
        /// <summary>
        /// 获取16进制字符串的GB2312字符串
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static String GetHexGB2312String(this string hexString) => GetGB2312String(GetHexByte(hexString));
        /// <summary>
        /// 获取GBK编码的字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String GetGBKString(this byte[] value) => _gbkEncoding.Value.GetString(value);
        /// <summary>
        /// 转换GBK成字节数组
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] GetGBKBytes(this string content) => _gbkEncoding.Value.GetBytes(content);
        /// <summary>
        /// 获取GBK16进制字符串
        /// </summary>
        /// <param name="content"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        public static String GetGBKHexString(this string content, bool isLower = false) => GetGBKBytes(content).GetHexString(isLower);
        /// <summary>
        /// 获取16进制字符串的GBK字符串
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static String GetHexGBKString(this string hexString) => GetGBKString(GetHexByte(hexString));
        /// <summary>
        /// 获取Utf8编码的字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String GetUTF8String(this byte[] value) => _utf8Encoding.Value.GetString(value);
        /// <summary>
        /// 转换UTF8成字节数组
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] GetUTF8Bytes(this string content) => _utf8Encoding.Value.GetBytes(content);
        /// <summary>
        /// 获取UTF8进制字符串
        /// </summary>
        /// <param name="content"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        public static String GetUTF8HexString(this string content, bool isLower = false) => GetUTF8Bytes(content).GetHexString(isLower);
        /// <summary>
        /// 获取16进制字符串的UTF8字符串
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static String GetHexUTF8String(this string hexString) => GetUTF8String(GetHexByte(hexString));
        /// <summary>
        /// 获取Unicode编码的字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String GetUnicodeString(this byte[] value) => _unicodeEncoding.Value.GetString(value);
        /// <summary>
        /// 转换Unicode成字节数组
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] GetUnicodeBytes(this string content) => _unicodeEncoding.Value.GetBytes(content);
        /// <summary>
        /// 获取Unicode进制字符串
        /// </summary>
        /// <param name="content"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        public static String GetUnicodeHexString(this string content, bool isLower = false) => GetUnicodeBytes(content).GetHexString(isLower);
        /// <summary>
        /// 获取16进制字符串的Unicode字符串
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static String GetHexUnicodeString(this string hexString) => GetUnicodeString(GetHexByte(hexString));
        /// <summary>
        /// 获取编码的字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String GetEncodingString(this byte[] value) => CobberCaller.GetString(value, DefaultEncoding);
        /// <summary>
        /// 获取编码的字符串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static String GetEncodingString(this byte[] value, Encoding encoding) => encoding.GetString(value);
        /// <summary>
        /// 转换成字节数组
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] GetEncodingBytes(this string content) => CobberCaller.GetBytes(content, DefaultEncoding);
        /// <summary>
        /// 转换成字节数组
        /// </summary>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] GetEncodingBytes(this string content, Encoding encoding) => CobberCaller.GetBytes(content, encoding);
        /// <summary>
        /// 获取Encoding16进制字符串
        /// </summary>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        public static String GetEncodingHexString(this string content, Encoding encoding, bool isLower = false) => GetEncodingBytes(content, encoding).GetHexString(isLower);
        /// <summary>
        /// 获取16进制字符串Encoding字符串
        /// </summary>
        /// <param name="hexString"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static String GetHexEncodingString(this string hexString, Encoding encoding) => GetEncodingString(GetHexByte(hexString), encoding);
        #endregion
        #endregion

        #region // 日期时间 DateTime
        /// <summary>
        /// Unix时间起始 1970-1-1,未知
        /// </summary>
        public static DateTime UnixTimeStart { get; } = new DateTime(1970, 1, 1);
        /// <summary>
        /// 国庆节,本地时间
        /// </summary>
        public static DateTime PRCNationalDay { get; } = new DateTime(1949, 10, 1, 0, 0, 0, DateTimeKind.Local);
        /// <summary>
        /// Unix时间起始 1970-1-1,UTC时间
        /// </summary>
        public static DateTime UnixTimeStartUtc { get; } = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        /// <summary>
        /// Unix时间起始 1970-1-1,UTC时间
        /// </summary>
        public static DateTimeOffset UnixTimeStartOffset { get; } = new DateTimeOffset(UnixTimeStartUtc);
        /// <summary>
        /// Unix时间起始 1970-1-1
        /// </summary>
        public static Int64 UnixTimestamp { get; } = UnixTimeStart.Ticks;
        /// <summary>
        /// 1970-1-1 00:00:00
        /// </summary>
        public static DateTime DateTime1970 { get => UnixTimeStart; }
        /// <summary>
        /// 1970-1-1 00:00:00
        /// </summary>
        public static DateTimeOffset DateTimeOffset1970 { get => UnixTimeStart; }
        /// <summary>
        /// 从1970-1-1的秒数,UTC时间
        /// </summary>
        public static Int64 SecondsFrom1970 => 62135596800L;
        /// <summary>
        /// 从1970-1-1的秒数,UTC时间
        /// </summary>
        public static Int64 TicksFrom1970 => 621355968000000000L;
        /// <summary>
        /// 距离1970年的秒数
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static Int64 DistanceFrom1970Seconds(this DateTime dateTime) => (Int64)(dateTime - DateTime1970).TotalSeconds;
        /// <summary>
        /// 距离1970年的秒数
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static Int64 DistanceFrom1970Seconds(this DateTimeOffset dateTime) => (Int64)(dateTime - DateTimeOffset1970).TotalSeconds;
        /// <summary>
        /// 从距离1970的秒数还原时间
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static DateTime FromDistance1970Seconds(this Int64 seconds) => new DateTime((seconds + SecondsFrom1970) * TimeSpan.TicksPerSecond);
        /// <summary>
        /// 通用日期格式
        /// </summary>
        public static String DateFormatter { get; set; } = "yyyy-MM-dd";
        /// <summary>
        /// 通用时间格式
        /// </summary>
        public static String TimeFormatter { get; set; } = "HH:mm:ss";
        /// <summary>
        /// 通用日期时间格式
        /// </summary>
        public static String DateTimeFormatter { get; set; } = "yyyy-MM-dd HH:mm:ss";
        /// <summary>
        /// 日期格式
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static String ToDateString(this DateTime date) => date.ToString(DateFormatter);
        /// <summary>
        /// 时间格式
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static String ToTimeString(this DateTime time) => time.ToString(TimeFormatter);
        /// <summary>
        /// 日期时间格式
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static String ToDateTimeString(this DateTime dateTime) => dateTime.ToString(DateTimeFormatter);
        /// <summary>
        /// 返回距离当前时间的区间
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static TimeSpan GetNowTimeSpan(this DateTime dateTime) => GetTimeSpan(DateTime.Now, dateTime);
        /// <summary>
        /// 获取一个时间区间
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="dist"></param>
        /// <returns></returns>
        public static TimeSpan GetTimeSpan(this DateTime dateTime, DateTime dist) => dateTime - dist;
        /// <summary>
        /// 返回距离当前时间的区间
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static TimeSpan GetNowDuration(this DateTime dateTime) => GetDuration(DateTime.Now, dateTime);
        /// <summary>
        /// 获取一个时间区间
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="dist"></param>
        /// <returns></returns>
        public static TimeSpan GetDuration(this DateTime dateTime, DateTime dist) => (dateTime - dist).Duration();
        /// <summary>
        /// 获取时间偏移量
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static DateTimeOffset GetDateTimeOffset(this DateTime dateTime, TimeSpan timeSpan) => new DateTimeOffset(dateTime, timeSpan);
        /// <summary>
        /// 合并一个时分秒到当前日期
        /// </summary>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime CombineTime(this DateTime date, DateTime time) => new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
        #endregion 日期时间 DateTime

        #region // 枚举调用 Enum NEnumerable
        /// <summary>
        /// 有标记
        /// </summary>
        /// <param name="value"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool HasFlag(this int value, int tag)
        {
            var temp = value & tag;
            return temp != 0 && tag == temp;
        }
        /// <summary>
        /// 有标记
        /// </summary>
        /// <param name="value"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool HasFlag(this uint value, uint tag)
        {
            var temp = value & tag;
            return temp != 0 && tag == temp;
        }
        /// <summary>
        /// 有标记
        /// </summary>
        /// <param name="value"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool HasFlag(this long value, long tag)
        {
            var temp = value & tag;
            return temp != 0 && tag == temp;
        }
        /// <summary>
        /// 有标记
        /// </summary>
        /// <param name="value"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool HasFlag(this ulong value, ulong tag)
        {
            var temp = value & tag;
            return temp != 0 && tag == temp;
        }
        /// <summary>
        /// 有标记
        /// </summary>
        /// <param name="value"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool HasFlag<T>(this T value, int tag) where T : struct, Enum
        {
            var vale = (int)Convert.ChangeType(value, typeof(int));
            return vale.HasFlag(tag);
        }
        /// <summary>
        /// 有标记
        /// </summary>
        /// <param name="value"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool HasFlag<T>(this T value, uint tag) where T : struct, Enum
        {
            var vale = (uint)Convert.ChangeType(value, typeof(uint));
            return vale.HasFlag(tag);
        }
        /// <summary>
        /// 有标记
        /// </summary>
        /// <param name="value"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool HasFlag<T>(this T value, long tag) where T : struct, Enum
        {
            var vale = (long)Convert.ChangeType(value, typeof(long));
            return vale.HasFlag(tag);
        }
        /// <summary>
        /// 有标记
        /// </summary>
        /// <param name="value"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool HasFlag<T>(this T value, ulong tag) where T : struct, Enum
        {
            var vale = (ulong)Convert.ChangeType(value, typeof(ulong));
            return vale.HasFlag(tag);
        }
        /// <summary>
        /// 获取逻辑序号值
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int[] GetFlagNos(this uint val)
        {
            var outList = new List<int>();
            for (int i = 0; i < 32; i++)
            {
                if (((val >> i) & 1) == 1) { outList.Add(i); }
            }
            return outList.ToArray();
        }
        /// <summary>
        /// 获取逻辑序号值
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int[] GetFlagNos(this int val)
        {
            var outList = new List<int>();
            for (int i = 0; i < 32; i++)
            {
                if (((val >> i) & 1) == 1) { outList.Add(i); }
            }
            return outList.ToArray();
        }
        /// <summary>
        /// 获取逻辑序号值
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int[] GetFlagNos(this long val)
        {
            var outList = new List<int>();
            for (int i = 0; i < 64; i++)
            {
                if (((val >> i) & 1) == 1) { outList.Add(i); }
            }
            return outList.ToArray();
        }
        /// <summary>
        /// 获取逻辑序号值
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int[] GetFlagNos(this ulong val)
        {
            var outList = new List<int>();
            for (int i = 0; i < 64; i++)
            {
                if (((val >> i) & 1) == 1) { outList.Add(i); }
            }
            return outList.ToArray();
        }
        /// <summary>
        /// 获取逻辑值
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static uint[] GetFlags(this uint val)
        {
            var outList = new List<uint>();
            for (int i = 0; i < 32; i++)
            {
                uint temp = 1U << i;
                if ((val & temp) != 0) { outList.Add(temp); }
            }
            return outList.ToArray();
        }
        /// <summary>
        /// 获取逻辑值
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int[] GetFlags(this int val)
        {
            var outList = new List<int>();
            for (int i = 0; i < 32; i++)
            {
                int temp = 1 << i;
                if ((val & temp) != 0) { outList.Add(temp); }
            }
            return outList.ToArray();
        }
        /// <summary>
        /// 获取逻辑值
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static long[] GetFlags(this long val)
        {
            var outList = new List<long>();
            for (int i = 0; i < 64; i++)
            {
                long temp = 1L << i;
                if ((val & temp) != 0) { outList.Add(temp); }
            }
            return outList.ToArray();
        }
        /// <summary>
        /// 获取逻辑值
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static ulong[] GetFlags(this ulong val)
        {
            var outList = new List<ulong>();
            for (int i = 0; i < 64; i++)
            {
                ulong temp = 1U << i;
                if ((val & temp) != 0) { outList.Add(temp); }
            }
            return outList.ToArray();
        }
        /// <summary>
        /// 获取逻辑值
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static T[] GetFlags<T>(this T val) where T : struct, Enum
        {
            var vale = (long)Convert.ChangeType(val, typeof(long));
            if (vale == 0) { return new T[0]; }
            var outList = new List<T>();
            foreach (var item in NEnumerable<T>.AllAttrs)
            {
                if (vale.HasFlag(item.Val)) { outList.Add(item.Enum); }
            }
            return outList.ToArray();
        }
        /// <summary>
        /// 获取逻辑值
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static NEnumerable<T>[] GetFlagsNEnumerable<T>(this T val) where T : struct, Enum
        {
            var vale = (long)Convert.ChangeType(val, typeof(long));
            if (vale == 0) { return new NEnumerable<T>[0]; }
            var outList = new List<NEnumerable<T>>();
            foreach (var item in NEnumerable<T>.AllAttrs)
            {
                if (vale.HasFlag(item.Val)) { outList.Add(item); }
            }
            return outList.ToArray();
        }
        /// <summary>
        /// 获取逻辑值
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string[] GetFlagsName<T>(this T val) where T : struct, Enum
        {
            var vale = (long)Convert.ChangeType(val, typeof(long));
            if (vale == 0) { return new string[0]; }
            var outList = new List<string>();
            foreach (var item in NEnumerable<T>.AllAttrs)
            {
                if (vale.HasFlag(item.Val)) { outList.Add(item.Name); }
            }
            return outList.ToArray();
        }
        /// <summary>
        /// 获取逻辑值
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string GetFlagString<T>(this T val) where T : struct, Enum
        {
            return GetFlagsName(val).JoinString();
        }
        /// <summary>
        /// 获取逻辑值
        /// </summary>
        /// <param name="val"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public static string GetFlagString<T>(this T val, string split) where T : struct, Enum
        {
            return GetFlagsName(val).JoinString(split);
        }
        #endregion 枚举调用 Enum NEnumerable

        #region // 集合列表 List IEnumerable
        /// <summary>
        /// 集合左边添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="length"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public static List<T> PadLeft<T>(this IEnumerable<T> list, int length, T defVal = default(T))
        {
            var result = new List<T>();
            if (list != null) { result.AddRange(list); }
            if (result.Count >= length) { return result; }
            var count = length - result.Count;
            for (int i = 0; i < count; i++)
            {
                result.Insert(0, defVal);
            }
            return result;
        }
        /// <summary>
        /// 集合右边添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="length"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public static List<T> PadRight<T>(this IEnumerable<T> list, int length, T defVal = default(T))
        {
            var result = new List<T>();
            if (list != null)
            {
                result.AddRange(list);
            }
            if (result.Count >= length)
            {
                return result;
            }
            var count = length - result.Count;
            for (int i = 0; i < count; i++)
            {
                result.Add(defVal);
            }
            return result;
        }
        /// <summary>
        /// 集合左边添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="length"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public static T[] PadLeft<T>(this T[] list, int length, T defVal = default(T))
        {
            if (list.Length >= length)
            {
                var toarr = new T[list.Length];
                Array.Copy(list, toarr, toarr.Length);
                return toarr;
            }
            var count = length - list.Length;
            var tarr = new T[length];
            Array.Copy(list, 0, tarr, count - 1, list.Length);
            for (int i = 0; i < count; i++)
            {
                tarr[i] = defVal;
            }
            return tarr;
        }
        /// <summary>
        /// 集合右边添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="length"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public static T[] PadRight<T>(this T[] list, int length, T defVal = default(T))
        {
            if (list.Length >= length)
            {
                var toarr = new T[list.Length];
                Array.Copy(list, toarr, toarr.Length);
                return toarr;
            }
            var count = length - list.Length;
            var tarr = new T[length];
            Array.Copy(list, tarr, list.Length);
            for (int i = 0; i < count; i++)
            {
                tarr[list.Length + i] = defVal;
            }
            return tarr;
        }
        /// <summary>
        /// 集合填充截断左边添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="length"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public static T[] PadInterceptLeft<T>(this T[] list, int length, T defVal = default(T))
        {
            if (list.Length >= length)
            {
                var toarr = new T[length];
                Array.Copy(list, toarr, toarr.Length);
                return toarr;
            }
            var count = length - list.Length;
            var tarr = new T[length];
            Array.Copy(list, 0, tarr, count, list.Length);
            for (int i = 0; i < count; i++)
            { tarr[i] = defVal; }
            return tarr;
        }
        /// <summary>
        /// 集合填充截断右边添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="length"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public static T[] PadInterceptRight<T>(this T[] list, int length, T defVal = default(T))
        {
            if (list.Length >= length)
            {
                var toarr = new T[length];
                Array.Copy(list, list.Length - length, toarr, 0, toarr.Length);
                return toarr;
            }
            var count = length - list.Length;
            var tarr = new T[length];
            Array.Copy(list, tarr, list.Length);
            for (int i = 0; i < count; i++)
            { tarr[list.Length + i] = defVal; }
            return tarr;
        }
        /// <summary>
        /// 设置ASCII码开头的内容
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static byte[] SetAsciiCharStart(this byte[] tag, string val)
        {
            var len = tag.Length > val.Length ? val.Length : tag.Length;
            for (int i = 0; i < len; i++) { tag[i] = (byte)val[i]; }
            return tag;
        }
        /// <summary>
        /// 判断数组是否以内容开头
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool StartsWith(this byte[] tag, byte[] val)
        {
            if (tag == null) { return false; }
            if (val == null) { return true; }
            if (tag.Length < val.Length) { return false; }
            for (int i = 0; i < val.Length; i++)
            {
                if (tag[i] != val[i]) { return false; }
            }
            return true;
        }
        /// <summary>
        /// 判断数组是否以内容开头
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool StartsWithAscii(this byte[] tag, string val)
        {
            if (tag == null) { return false; }
            if (val == null) { return true; }
            if (tag.Length < val.Length) { return false; }
            for (int i = 0; i < val.Length; i++)
            {
                if (tag[i] != val[i]) { return false; }
            }
            return true;
        }
        /// <summary>
        /// 查找及其所有子项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="EqualItem"></param>
        /// <param name="GetSubItems"></param>
        /// <returns></returns>
        public static T[] FindAndSubItems<T>(this IEnumerable<T> list, Func<T, bool> EqualItem, Func<T, IEnumerable<T>> GetSubItems)
        {
            if (list == null || !list.Any()) { return new T[0]; }
            var res = new List<T>();
            foreach (var item in list)
            {
                if (EqualItem(item))
                {
                    res.Add(item);
                    res.AddRange(FindAndSubItems(GetSubItems(item), GetSubItems));
                }
            }
            return res.ToArray();
        }
        /// <summary>
        /// 查找及其所有子项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="tagModel"></param>
        /// <param name="FindItems"></param>
        /// <returns></returns>
        public static T[] FindAndSubItems<T>(this IEnumerable<T> list, T tagModel, Func<T, T, bool> FindItems)
        {
            if (list == null || !list.Any()) { return new T[0]; }
            var res = new List<T>();
            foreach (var item in list)
            {
                if (FindItems(tagModel, item))
                {
                    res.Add(item);
                    res.AddRange(FindAndSubItems(list, item, FindItems));
                }
            }
            return res.ToArray();
        }
        /// <summary>
        /// 查找及其所有子项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="EqualItem"></param>
        /// <param name="FindItems"></param>
        /// <returns></returns>
        public static T[] FindAndSubItems<T>(this IEnumerable<T> list, Func<T, bool> EqualItem, Func<T, T, bool> FindItems)
        {
            if (list == null || !list.Any()) { return new T[0]; }
            var res = new List<T>();
            foreach (var item in list)
            {
                if (EqualItem(item))
                {
                    res.Add(item);
                    res.AddRange(FindAndSubItems(list, item, FindItems));
                }
            }
            return res.ToArray();
        }
        /// <summary>
        /// 查找所有子项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="GetSubItems"></param>
        /// <returns></returns>
        public static T[] FindAndSubItems<T>(this IEnumerable<T> list, Func<T, IEnumerable<T>> GetSubItems)
        {
            if (list == null || !list.Any()) { return new T[0]; }
            var res = new List<T>();
            foreach (var item in list)
            {
                res.Add(item);
                res.AddRange(FindAndSubItems(GetSubItems(item), GetSubItems));
            }
            return res.ToArray();
        }
        /// <summary>
        /// 计数
        /// </summary>
        /// <param name="ie"></param>
        /// <returns></returns>
        public static Int32 GetCount(this IEnumerable ie)
        {
            var index = 0;
            foreach (var _ in ie)
            {
                index++;
            }
            return index;
        }
        /// <summary>
        /// 有元素
        /// </summary>
        /// <param name="ie"></param>
        /// <returns></returns>
        public static bool HasElement(this IEnumerable ie)
        {
            return ie.GetEnumerator().MoveNext();
        }
        /// <summary>
        /// 转换成过滤数组,简写Where(fillter).ToArray()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="fillter"></param>
        /// <returns></returns>
        public static T[] ToArray<T>(this IEnumerable<T> list, Func<T, bool> fillter)
            => list?.Where(fillter).ToArray();
        /// <summary>
        /// 转换成过滤数组,简写Select(getter).ToArray()
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="list"></param>
        /// <param name="getter"></param>
        /// <returns></returns>
        public static T2[] ToArray<T1, T2>(this IEnumerable<T1> list, Func<T1, T2> getter)
            => list?.Select(getter).ToArray();
        /// <summary>
        /// 一次性调用where和select,减少调用
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="list"></param>
        /// <param name="where"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public static IEnumerable<T2> WhereSelect<T1, T2>(this IEnumerable<T1> list, Func<T1, bool> where, Func<T1, T2> select)
            => list?.Where(where).Select(select);
        /// <summary>
        /// 添加区域内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashSet"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static HashSet<T> AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> list)
        {
            if (hashSet == null) { return hashSet; }
            if (list == null) { return hashSet; }
            foreach (var item in list)
            {
                hashSet.Add(item);
            }
            return hashSet;
        }
        #endregion 集合列表 List IEnumerable

        #region // 异常类型 Exception
        /// <summary>
        /// 获取异常的信息及跟踪信息文本
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string GetMessageStackTraceString(this Exception exception)
        {
            var sb = new StringBuilder();
            sb.Append(exception.Message);
            if (exception.StackTrace != null)
            {
                sb.AppendLine().Append(exception.StackTrace);
            }
            return sb.ToString();
        }
        #endregion 异常类型 Exception

        #region // 文件目录 File Directory
        private static readonly object _pathLocker = new object();
        private static readonly Dictionary<string, string> _pathDic = new Dictionary<string, string>();
        #region // 文件空间及大小
        /// <summary>
        /// 获取文件目录
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static FolderSizeInfo GetFolderSize(this DirectoryInfo directory)
        {
            FolderSizeInfo result = new FolderSizeInfo();
            if (!directory.Exists) { return result; }
            var files = directory.GetFiles("*", SearchOption.AllDirectories);
            long fileSize = 0;
            long spaceSize = 0;
            var diskInfo = directory.GetDiskInfo();
            foreach (var item in files)
            {
                fileSize += item.Length;
                spaceSize += CalcSpaceSize(item.Length, diskInfo.ClusterSize);
            }
            var dirs = directory.GetDirectories("*", SearchOption.AllDirectories);
            result.FolderCount = dirs.Length;
            result.FolderSize = spaceSize;
            result.FileCount = files.Length;
            result.FileSize = fileSize;
            return result;
        }
        /// <summary>
        /// 获取磁盘信息
        /// </summary>
        /// <returns></returns>
        public static DiskSizeInfo GetDiskInfo(this DirectoryInfo dir) => GetDiskInfo(dir.Root.FullName);
        /// <summary>
        /// 获取磁盘信息
        /// </summary>
        /// <returns></returns>
        public static DiskSizeInfo GetDiskInfo(this FileInfo file) => GetDiskInfo(file.Directory.Root.FullName);
        /// <summary>
        /// 获取磁盘信息
        /// </summary>
        /// <param name="rootPathName"></param>
        /// <returns></returns>
        public static DiskSizeInfo GetDiskInfo(string rootPathName)
        {
            uint sectorsPerCluster = 0, bytesPerSector = 0, numberOfFreeClusters = 0, totalNumberOfClusters = 0;
            Impeller.KERNEL32.GetDiskFreeSpace(rootPathName, ref sectorsPerCluster, ref bytesPerSector, ref numberOfFreeClusters, ref totalNumberOfClusters);
            return new DiskSizeInfo()
            {
                SectorsPerCluster = sectorsPerCluster,
                BytesPerSector = bytesPerSector
            };
        }
        private static long CalcSpaceSize(long fileSize, long clusterSize)
        {
            if (fileSize % clusterSize == 0) { return fileSize; }
            decimal res = fileSize / clusterSize;
            int clu = Convert.ToInt32(Math.Ceiling(res)) + 1;
            return clusterSize * clu;
        }
        #endregion
        #region // 判断文件夹
        /// <summary>
        /// 尝试打开文件夹
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static bool TryOpenDirectory(string dir)
        {
            try
            {
                if (File.Exists(dir))
                {
                    Process.Start("explorer.exe", $"/e,/select,{dir}");
                }
                else
                {
                    Process.Start("explorer.exe", $"/e,{dir}");
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
        /// <summary>
        /// 最后一个盘符
        /// </summary>
        public static String LastLocalDisk { get; } = GetLastLocalDisk();
        /// <summary>
        /// 最后一个盘符
        /// 如:F:\
        /// </summary>
        /// <returns></returns>
        public static string GetLastLocalDisk()
        {
            return DriveInfo.GetDrives().LastOrDefault(s => s.DriveType == DriveType.Fixed)?.Name;
        }
        /// <summary>
        /// 获取已存在的保存目录
        /// 反人类方法规则:
        /// 1.如果saveDir不为空,返回saveDir
        /// 2.如果saveDir不为空,将拼接parent和subDir为saveDir返回
        /// 3.如果出现异常将返回Directory.GetCurrentDirectory()下的Temp文件夹
        /// </summary>
        /// <param name="saveDir"></param>
        /// <param name="parent"></param>
        /// <param name="subDir"></param>
        /// <param name="isRecursive"></param>
        /// <returns></returns>
        public static string GetExistSaveDir(string saveDir, string parent, string subDir, bool isRecursive)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(saveDir))
                {
                    if (!Directory.Exists(saveDir))
                    {
                        Directory.CreateDirectory(saveDir);
                    }
                    return saveDir;
                }
                saveDir = Path.GetFullPath(Path.Combine(parent, subDir));
                CreateDir(new DirectoryInfo(saveDir), isRecursive);
                return saveDir;
            }
            catch
            {
                return Path.Combine(Directory.GetCurrentDirectory(), "Temp");
            }
        }
        /// <summary>
        /// 获取已存在的保存目录
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="subDir"></param>
        /// <returns></returns>
        public static string GetExistSaveDir(string parent, string subDir)
        {
            try
            {
                var saveDir = Path.GetFullPath(Path.Combine(parent, subDir));
                CreateDir(new DirectoryInfo(saveDir), true);
                return saveDir;
            }
            catch
            {
                return Path.Combine(Directory.GetCurrentDirectory(), "Temp");
            }
        }
        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="isRecursive">循环递归创建</param>
        /// <returns></returns>
        public static DirectoryInfo CreateDir(this DirectoryInfo dir, bool isRecursive = false)
        {
            if (isRecursive) { return CreateRecursiveDir(dir); }
            if (!dir.Exists) { dir.Create(); }
            return dir;
        }
        /// <summary>
        /// 级联创建目录
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static DirectoryInfo CreateRecursiveDir(this DirectoryInfo dir)
        {
            if (dir.Exists) { return dir; }
            CreateRecursiveDir(dir.Parent);
            if (!dir.Exists) { dir.Create(); }
            return dir;
        }
        /// <summary>
        /// 级联检查目录是否存在
        /// </summary>
        /// <param name="dir"></param>
        public static void CheckRecursiveDir(this DirectoryInfo dir)
        {
            if (!dir.Exists)
            {
                CheckRecursiveDir(dir.Parent);
                dir.Create();
            }
        }
        /// <summary>
        /// 存在内容
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static bool HasContent(this DirectoryInfo dir)
        {
            if (dir == null) { return false; }
            return dir.Exists && (dir.GetFiles().Length > 0 || dir.GetDirectories().Length > 0);
        }
        /// <summary>
        /// 注册路径
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static void RegistPath(string key, string value)
        {
            lock (_pathLocker)
            {
                _pathDic[key] = value;
            }
        }
        /// <summary>
        /// 注册路径
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetOrRegistPath(string key, string value)
        {
            if (_pathDic.TryGetValue(key, out var path))
            {
                return path;
            }
            lock (_pathLocker)
            {
                if (_pathDic.TryGetValue(key, out path))
                {
                    return path;
                }
                return _pathDic[key] = value;
            }
        }
        /// <summary>
        /// 注册路径
        /// </summary>
        /// <param name="key"></param>
        /// <param name="GetPath"></param>
        /// <returns></returns>
        public static string GetOrRegistPath(string key, Func<string> GetPath)
        {
            if (_pathDic.TryGetValue(key, out var path)) { return path; }
            lock (_pathLocker)
            {
                if (_pathDic.TryGetValue(key, out path)) { return path; }
                return _pathDic[key] = GetPath();
            }
        }
        /// <summary>
        /// 注册路径
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetPath(string key)
        {
            if (_pathDic.TryGetValue(key, out var path)) { return path; }
            return Path.GetFullPath(string.Empty);
        }
        /// <summary>
        /// 获取一个目录信息
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="subDir"></param>
        /// <param name="defSubDir"></param>
        /// <returns></returns>
        public static DirectoryInfo GetDirectory(string parent, string subDir, string defSubDir)
        {
            if (string.IsNullOrWhiteSpace(subDir)) { subDir = string.IsNullOrWhiteSpace(defSubDir) ? "Temp" : defSubDir; }
            var path = new DirectoryInfo(Path.GetFullPath(Path.Combine(parent, subDir)));
            if (!path.Exists) { path.Create(); }
            return path;
        }
        /// <summary>
        /// 获取一个目录信息
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="subDir"></param>
        /// <param name="defSubDir"></param>
        /// <returns></returns>
        public static LazyBone<DirectoryInfo> GetLazyDirectory(string parent, string subDir, string defSubDir = null)
        {
            return new LazyBone<DirectoryInfo>(() => GetDirectory(parent, subDir, defSubDir), true);
        }
        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="fileName"></param>
        public static void StartFile(string fileName) => StartFile(new FileInfo(fileName));
        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="fileName"></param>
        public static void StartFile(this FileInfo fileName)
        {
            Process.Start(new ProcessStartInfo(fileName.FullName) { UseShellExecute = true });
        }
        /// <summary>
        /// 打开目录
        /// </summary>
        /// <param name="dir"></param>
        public static void StartFolder(string dir) => StartFolder(new DirectoryInfo(dir));
        /// <summary>
        /// 打开目录
        /// </summary>
        /// <param name="fileName"></param>
        public static void StartFolder(this DirectoryInfo fileName)
        {
            ExecHidden("explorer.exe", fileName.FullName, fileName.FullName);
        }
        #region // SdkFileComponent
        /// <summary>
        /// 写资源文件
        /// </summary>
        /// <param name="dllFile"></param>
        /// <param name="fullName"></param>
        public static void WriteResourceFile(byte[] dllFile, string fullName)
        {
            try
            {
                if (File.Exists(fullName)) { File.Delete(fullName); }
                if (!Directory.Exists(Path.GetDirectoryName(fullName))) { Directory.CreateDirectory(Path.GetDirectoryName(fullName)); }
                File.WriteAllBytes(fullName, dllFile);
            }
            catch (Exception ex) { Console.WriteLine(ex); }
        }
        /// <summary>
        /// 写资源文件
        /// </summary>
        /// <param name="dllFile"></param>
        /// <param name="fullName"></param>
        public static void WriteResourceFile(Stream dllFile, string fullName)
        {
            try
            {
                if (File.Exists(fullName)) { File.Delete(fullName); }
                if (!Directory.Exists(Path.GetDirectoryName(fullName))) { Directory.CreateDirectory(Path.GetDirectoryName(fullName)); }
                using (var fs = File.Create(fullName))
                {
                    dllFile.Seek(0, SeekOrigin.Begin);
                    dllFile.CopyTo(fs);
                    fs.Flush();
                }
            }
            catch (Exception ex) { Console.WriteLine(ex); }
        }
        /// <summary>
        /// 比较两个文件
        /// </summary>
        /// <param name="srcFile"></param>
        /// <param name="tagFile"></param>
        public static bool CompareFile(string srcFile, string tagFile)
        {
            var srcInfo = new FileInfo(srcFile);
            var tagInfo = new FileInfo(tagFile);
            if (!srcInfo.Exists || !tagInfo.Exists) { return false; }
            using (var srcFs = srcInfo.OpenRead())
            {
                using (var tagFs = tagInfo.OpenRead())
                {
                    using var sha = SHA1.Create();
                    var srcSha = sha.ComputeHash(srcFs);
                    var tagSha = sha.ComputeHash(tagFs);
                    if (srcSha.Length != tagSha.Length) { return false; }
                    for (int i = 0; i < srcSha.Length; i++)
                    {
                        if (srcSha[i] != tagSha[i]) { return false; }
                    }
                    return true;
                }
            }
        }
        /// <summary>
        /// 比较资源文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="res"></param>
        /// <returns></returns>
        public static bool CompareResourceFile(string file, byte[] res)
        {
            if (!File.Exists(file)) { return false; }
            using (var hash = SHA1.Create())
            {
                using (var distFile = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var resHash = hash.ComputeHash(res);
                    var distHash = hash.ComputeHash(distFile);
                    if (resHash.Length != distHash.Length) { return false; }
                    for (int i = 0; i < resHash.Length; i++)
                    {
                        if (resHash[i] != distHash[i]) { return false; }
                    }
                    return true;
                }
            }
        }
        /// <summary>
        /// 比较资源文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="res"></param>
        /// <returns></returns>
        public static bool CompareResourceFile(string file, Stream res)
        {
            if (!File.Exists(file)) { return false; }
            using (var hash = SHA1.Create())
            {
                using (var distFile = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var resHash = hash.ComputeHash(res);
                    var distHash = hash.ComputeHash(distFile);
                    if (resHash.Length != distHash.Length) { return false; }
                    for (int i = 0; i < resHash.Length; i++)
                    {
                        if (resHash[i] != distHash[i]) { return false; }
                    }
                    return true;
                }
            }
        }
        /// <summary>
        /// 复制目录
        /// </summary>
        /// <param name="src"></param>
        /// <param name="tag"></param>
        public static void CopyDirectory(string src, string tag)
        {
            foreach (var item in new DirectoryInfo(src).GetFileSystemInfos())
            {
                if (item is DirectoryInfo dir)
                {
                    var tagDir = Path.Combine(tag, dir.Name);
                    if (!Directory.Exists(tagDir)) { Directory.CreateDirectory(tagDir); }
                    CopyDirectory(dir.FullName, tagDir);
                    continue;
                }
                File.Copy(item.FullName, Path.Combine(tag, item.Name), false);
            }
        }
        /// <summary>
        /// 尝试复制文件夹
        /// </summary>
        /// <param name="src"></param>
        /// <param name="tag"></param>
        public static void TryCopyDirectory(string src, string tag)
        {
            try
            {
                CopyDirectory(src, tag);
            }
            catch (Exception ex)
            {
                Console.WriteLine(new { src, tag, method = nameof(TryCopyDirectory), ex });
            }
        }
        /// <summary>
        /// 获取一个资源字节
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static byte[] GetResourceBytes(Assembly assembly, string name)
        {
            var res = assembly.GetManifestResourceNames().WhereSelect(s => s.EndsWith(name), s => s);
            var count = res.Count();
            if (count == 0) { throw new ArgumentNullException($"未找到'{name}'的资源文件"); }
            if (count != 1) { throw new ArgumentOutOfRangeException($"找到不唯一的'{name}'的资源文件"); }
            using (var ms = new MemoryStream())
            {
                assembly.GetManifestResourceStream(res.First()).CopyTo(ms);
                return ms.ToArray();
            }
        }
        /// <summary>
        /// 获取一个资源流
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Stream GetResourceStream(Assembly assembly, string name)
        {
            var res = assembly.GetManifestResourceNames().WhereSelect(s => s.EndsWith(name), s => s);
            var count = res.Count();
            if (count == 0) { throw new ArgumentNullException($"未找到'{name}'的资源文件"); }
            if (count != 1) { throw new ArgumentOutOfRangeException($"找到不唯一的'{name}'的资源文件"); }
            var ms = new MemoryStream();
            assembly.GetManifestResourceStream(res.First()).CopyTo(ms);
            ms.Position = 0;
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }
        /// <summary>
        /// 获取一个资源文件
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static FileInfo GetResource(Assembly assembly, string name)
        {
            return GetResource(assembly, name, Directory.GetCurrentDirectory());
        }
        /// <summary>
        /// 获取一个资源文件
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="name"></param>
        /// <param name="savePath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static FileInfo GetResource(Assembly assembly, string name, string savePath)
        {
            var res = assembly.GetManifestResourceNames().WhereSelect(s => s.EndsWith(name), s => s);
            var count = res.Count();
            if (count == 0) { throw new ArgumentNullException($"未找到'{name}'的资源文件"); }
            if (count != 1) { throw new ArgumentOutOfRangeException($"找到不唯一的'{name}'的资源文件"); }
            var stream = assembly.GetManifestResourceStream(res.First());
            string fileFullName;
            if (Directory.Exists(savePath))
            {
                string fileName;
                using (var sha1 = SHA1.Create())
                {
                    var hash = sha1.ComputeHash(stream);
                    fileName = hash.GetSha1HexString(true);
                }
                fileFullName = Path.Combine(savePath, fileName);
            }
            else { fileFullName = savePath; }
            using (var fs = File.Create(fileFullName))
            {
                stream.Position = 0;
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fs);
                fs.Flush();
            }
            return new FileInfo(fileFullName);
        }
        #region // 文件自定义
        /// <summary>
        /// 单文件压缩（生成的压缩包和第三方的解压软件兼容）
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <returns></returns>
        public static string FileGZipCompress(string sourceFilePath)
        {
            string zipFileName = sourceFilePath + ".gz";
            if (!File.Exists(sourceFilePath)) { return zipFileName; }
            using (FileStream zipFileStream = File.Create(zipFileName))
            {
                using (GZipStream zipStream = new GZipStream(zipFileStream, CompressionMode.Compress))
                {
                    using (FileStream sourceFileStream = File.OpenRead(sourceFilePath))
                    {
                        sourceFileStream.CopyTo(zipStream);
                    }
                }
            }
            return zipFileName;
        }
        /// <summary>
        /// 单文件解压缩（生成的压缩包和第三方的解压软件兼容）
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="savePath"></param>
        /// <returns></returns>
        public static string FileGZipDecompress(string sourceFilePath, string savePath)
        {
            if (!File.Exists(sourceFilePath)) { return savePath; }
            using (FileStream sourceFileStream = File.OpenRead(sourceFilePath))
            {
                using (GZipStream zipStream = new GZipStream(sourceFileStream, CompressionMode.Decompress))
                {
                    using (FileStream zipFileStream = File.Create(savePath))
                    {
                        zipStream.CopyTo(zipFileStream);
                    }
                }
            }
            return savePath;
        }
        /// <summary>
        /// 自定义压缩SDK(不兼容解压工具)
        /// [名称长度4][名称][文件长度8][文件]
        /// </summary>
        /// <param name="map">文件压缩路径为Key,对应的物理路径为Value</param>
        /// <param name="savePath"></param>
        public static IAlertMsg FileGZipCompress(Dictionary<string, string> map, string savePath)
        {
            try
            {
                using (var zipFileStream = File.Create(savePath))
                {
                    using (var zipStream = new GZipStream(zipFileStream, CompressionMode.Compress))
                    {
                        byte[] readBytes = new byte[4096];
                        foreach (var kv in map)
                        {
                            var filePath = kv.Value;
                            var fileName = kv.Key;
                            if (File.Exists(filePath))
                            {
                                byte[] fileNameBytes = System.Text.Encoding.UTF8.GetBytes(fileName);
                                byte[] sizeBytes = BitConverter.GetBytes(fileNameBytes.Length);
                                zipStream.Write(sizeBytes, 0, sizeBytes.Length);
                                zipStream.Write(fileNameBytes, 0, fileNameBytes.Length);
                                using (var fsRead = File.OpenRead(filePath))
                                {
                                    zipStream.Write(BitConverter.GetBytes(fsRead.Length), 0, 8);
                                    int len = 0;
                                    //4.通过读取文件流读取数据
                                    while ((len = fsRead.Read(readBytes, 0, readBytes.Length)) > 0)
                                    {
                                        //通过压缩流写入数据
                                        zipStream.Write(readBytes, 0, len);
                                    }
                                }
                            }
                            zipStream.Flush();
                        }
                    }
                }
                return (AlertMsg<string>)(true, "操作成功", savePath);
            }
            catch (Exception ex)
            {
                return new AlertException(ex);
            }
        }
        /// <summary>
        /// 自定义解压SDK(不兼容解压工具)
        /// </summary>
        /// <param name="zipFile"></param>
        /// <param name="tagDir"></param>
        /// <param name="Fillter"></param>
        public static IAlertMsg FileGZipDecompress(string zipFile, string tagDir, Func<string, bool> Fillter)
        {
            if (!File.Exists(zipFile)) { return AlertMsg.NotFound; }
            Fillter ??= (s) => true;
            try
            {
                using (FileStream fStream = File.OpenRead(zipFile))
                {
                    using (GZipStream zipStream = new GZipStream(fStream, CompressionMode.Decompress))
                    {
                        var readBytes = new byte[4096];
                        while (true)
                        {
                            byte[] fileNameSize = new byte[4];
                            var readRes = zipStream.Read(fileNameSize, 0, fileNameSize.Length);
                            if (readRes == 0) { break; }
                            int fileNameLength = BitConverter.ToInt32(fileNameSize, 0);
                            byte[] fileNameBytes = new byte[fileNameLength];
                            zipStream.Read(fileNameBytes, 0, fileNameBytes.Length);
                            string fileName = System.Text.Encoding.UTF8.GetString(fileNameBytes);
                            var fileSize = new byte[8];
                            zipStream.Read(fileSize, 0, fileSize.Length);
                            var fileContentLength = BitConverter.ToInt64(fileSize, 0);
                            if (!Fillter.Invoke(fileName))
                            {
                                long readLen = 0;
                                while (readLen < fileContentLength)
                                {
                                    var takeLen = fileContentLength - readLen;
                                    if (takeLen > readBytes.Length) { takeLen = readBytes.Length; }
                                    readLen += takeLen;
                                    var res = zipStream.Read(readBytes, 0, (int)takeLen);
                                    if (res <= 0) { break; }
                                }
                                continue;
                            }
                            string fileFullName = System.IO.Path.Combine(tagDir, fileName);
                            var currDir = Path.GetDirectoryName(fileFullName);
                            if (!Directory.Exists(currDir)) { Directory.CreateDirectory(currDir); }
                            using (FileStream childFileStream = File.Create(fileFullName))
                            {
                                long readLen = 0;
                                while (readLen < fileContentLength)
                                {
                                    var takeLen = fileContentLength - readLen;
                                    if (takeLen > readBytes.Length) { takeLen = readBytes.Length; }
                                    readLen += takeLen;
                                    var res = zipStream.Read(readBytes, 0, (int)takeLen);
                                    if (res > 0)
                                    {
                                        //通过文件流写入文件
                                        childFileStream.Write(readBytes, 0, res);//读取的长度为len，这样不会造成数据的错误
                                    }
                                    else { break; }
                                }
                                childFileStream.Flush();
                            }
                        }
                    }
                }
                return (AlertMsg<String>)(true, "操作成功", tagDir);
            }
            catch (Exception ex)
            {
                return new AlertException(ex);
            }
        }
        /// <summary>
        /// 自定义解压SDK(不兼容解压工具)
        /// </summary>
        /// <param name="zipFile"></param>
        /// <param name="Converter"></param>
        public static IAlertMsg FileGZipDecompress(string zipFile, Action<string, byte[]> Converter)
        {
            if (!File.Exists(zipFile)) { return AlertMsg.NotFound; }
            try
            {
                using (FileStream fStream = File.OpenRead(zipFile))
                {
                    using (GZipStream zipStream = new GZipStream(fStream, CompressionMode.Decompress))
                    {
                        while (true)
                        {
                            byte[] fileNameSize = new byte[4];
                            var readRes = zipStream.Read(fileNameSize, 0, fileNameSize.Length);
                            if (readRes == 0) { break; }
                            int fileNameLength = BitConverter.ToInt32(fileNameSize, 0);
                            byte[] fileNameBytes = new byte[fileNameLength];
                            zipStream.Read(fileNameBytes, 0, fileNameBytes.Length);
                            string fileName = System.Text.Encoding.UTF8.GetString(fileNameBytes);
                            var fileSize = new byte[8];
                            zipStream.Read(fileSize, 0, fileSize.Length);
                            var fileContentLength = BitConverter.ToInt64(fileSize, 0);
                            var readBytes = new byte[fileContentLength];
                            var res = zipStream.Read(readBytes, 0, readBytes.Length);
                            Converter?.Invoke(fileName, readBytes);
                        }
                    }
                }
                return (AlertMsg)(true, "操作成功");
            }
            catch (Exception ex)
            {
                return new AlertException(ex);
            }
        }
        #endregion 文件自定义
        #endregion SdkFileComponent
        #endregion 文件目录 File Directory

        #region // IP地址 IPAddress
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static long GetValue(this IPAddress ip)
        {
            byte[] byt = ip.GetAddressBytes();
            if (byt.Length == 4)
            {
                return System.BitConverter.ToUInt32(byt, 0);
            }
            return System.BitConverter.ToInt64(byt, 0);
        }
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static IPAddress GetIPAddress(this Int64 ip)
        {
            return new IPAddress(ip);
        }
        #endregion IP地址 IPAddress

        #region // 流字节操作 Stream Bytes
        /// <summary>
        /// 转换成字节数组
        /// </summary>
        /// <returns>MemoryStream已经释放</returns>
        public static byte[] GetBytes(this MemoryStream ms)
        {
            ms.Seek(0, SeekOrigin.Begin); //一定不要忘记将流的初始位置重置
            byte[] bytes = new byte[ms.Length];
            ms.Read(bytes, 0, bytes.Length); //如果上面流没有seek 则这里读取的数据全会为0
            ms.Dispose();
            return bytes;
        }
        /// <summary>
        /// 比较两个字节数组
        /// </summary>
        /// <param name="src"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool CompareEach(this byte[] src, byte[] tag)
        {
            if (src == null || tag == null) { return false; }
            if (src.Length != tag.Length) { return false; }
            for (int i = 0; i < src.Length; i++)
            {
                if (src[i] != tag[i]) { return false; }
            }
            return true;
        }
        #endregion 流字节操作 Stream Bytes

        #region // 成员调用 MemberInfo PropertyInfo MethodInfo
        /// <summary>
        /// 跟踪方法键
        /// </summary>
        public static String TraceMathodKey { get { return new StackTrace().GetFrame(1).GetMethod().GetMemberFullName(); } }
        /// <summary>
        /// 跟踪方法小时键
        /// </summary>
        public static String TraceMathodHourKey
        {
            get
            {
                var frame = new StackTrace().GetFrame(1);
                return $"{frame.GetMethod().GetMemberFullName()}.{DateTime.Now:yyMMddHH}";
            }
        }
        /// <summary>
        /// 跟踪方法小时键
        /// </summary>
        public static String TraceMathodDayKey
        {
            get
            {
                var frame = new StackTrace().GetFrame(1);
                return $"{frame.GetMethod().GetMemberFullName()}.{DateTime.Now:yyMMdd}";
            }
        }
        /// <summary>
        /// 获取跟踪Frame内容
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static StackFrame GetTraceFrame(int index = 1)
        {
            return new StackTrace().GetFrame(index);
        }
        /// <summary>
        /// 字段信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static FieldInfo GetFieldInfo<T>(this Expression<Func<T>> expression)
        {
            MemberExpression body = (MemberExpression)expression.Body;
            return (FieldInfo)body.Member;
        }
        /// <summary>
        /// 属性信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo<T>(this Expression<Func<T>> expression)
        {
            MemberExpression body = (MemberExpression)expression.Body;
            return (PropertyInfo)body.Member;
        }
        /// <summary>
        /// 属性信息
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo<TM, TP>(this Expression<Func<TM, TP>> expression)
        {
            MemberExpression body = (MemberExpression)expression.Body;
            return (PropertyInfo)body.Member;
        }
        /// <summary>
        /// 属性信息
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo<TM>(this Expression<Func<TM, object>> expression)
        {
            var body = expression.Body;
            if (body is MemberExpression member)
            {
                return (PropertyInfo)member.Member;
            }
            if (body is UnaryExpression unary)
            {
                return (PropertyInfo)((MemberExpression)unary.Operand).Member;
            }
            return null;
        }
        /// <summary>
        /// 成员信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MemberInfo GetMemberInfo<T>(this Expression<Func<T>> expression)
        {
            MemberExpression body = (MemberExpression)expression.Body;
            return body.Member;
        }
        /// <summary>
        /// 成员信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static String GetFullName<T>(this Expression<Func<T>> expression)
        {
            MemberExpression body = (MemberExpression)expression.Body;
            var member = body.Member;
            return $"{member.DeclaringType.FullName}.{member.Name}";
        }
        /// <summary>
        /// 获取成员全称
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static String GetFullName(this MemberInfo method)
        {
            return $"{method.DeclaringType?.FullName}.{method.Name}";
        }
        #region // 查询模型内容
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="model">模型,为null时,false</param>
        /// <param name="value">查询字符串,为空时,true</param>
        /// <param name="split">分隔符</param>
        /// <returns></returns>
        public static bool SearchModel<T>(this T model, string value, char split = default)
        {
            if (model == null) { return false; }
            if (string.IsNullOrWhiteSpace(value)) { return true; }
            if (split == default)
            {
                return SearchContains(model, new string[] { value.Trim() }, PropertyAccess.GetAccess(model).FuncGetDic.Values);
            }
            return SearchModel(model, value.Split(split));
        }
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="model">模型,为null时,0</param>
        /// <param name="value">查找字符串,为空时,1</param>
        /// <param name="split">查找分隔符</param>
        /// <returns></returns>
        public static int SearchOrModel<T>(this T model, string value, char split = default)
        {
            if (model == null) { return 0; }
            if (string.IsNullOrEmpty(value)) { return 1; }
            if (split == default)
            {
                return SearchContainsCount<T>(model, new string[] { value.Trim() }, PropertyAccess.GetAccess(model).FuncGetDic.Values);
            }
            return SearchOrModel<T>(model, value.Split(split));
        }
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="model">模型,为null时,false</param>
        /// <param name="values">查询值,为空时,true</param>
        /// <returns></returns>
        public static bool SearchModel<T>(this T model, params string[] values)
        {
            if (model == null) { return false; }
            if (values.IsEmpty()) { return true; }
            var valItems = values.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).Distinct().ToArray();
            if (valItems.Length == 0) { return true; }
            return SearchContains(model, valItems, PropertyAccess.GetAccess(model).FuncGetDic.Values);
        }
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="model"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static int SearchOrModel<T>(this T model, params string[] values)
        {
            if (model == null || values == null) { return 0; }
            var valItems = values.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
            if (valItems.Length == 0) { return 1; }
            return SearchContainsCount(model, valItems, PropertyAccess.GetAccess(model).FuncGetDic.Values);
        }
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="model">模型,为null时,false</param>
        /// <param name="properties">查询属性,为空时,false</param>
        /// <param name="values">查询值,为空时,true</param>
        /// <returns></returns>
        public static bool SearchModel<T>(this T model, string[] properties, string[] values)
        {
            if (model == null || properties.IsEmpty()) { return false; }
            if (values.IsEmpty()) { return true; }
            var valItems = values.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
            if (valItems.Length == 0) { return true; }
            var funcGetList = PropertyAccess.GetAccess(model).FuncGetDic.Where(s => properties.Contains(s.Key)).Select(s => s.Value);
            return SearchContains<T>(model, valItems, funcGetList);
        }
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="model"></param>
        /// <param name="properties"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static int SearchOrModel<T>(this T model, string[] properties, string[] values)
        {
            if (model == null || values == null) { return 0; }
            var valItems = values.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
            if (valItems.Length == 0) { return 1; }
            var funcGetList = PropertyAccess.GetAccess(model).FuncGetDic.Where(s => properties.Contains(s.Key)).Select(s => s.Value);
            return SearchContainsCount(model, valItems, funcGetList);
        }
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="models">模型,为空时,空列表</param>
        /// <param name="values">查询值,为空时,源列表</param>
        /// <returns></returns>
        public static IEnumerable<T> SearchModels<T>(this IEnumerable<T> models, params string[] values)
        {
            if (models.IsEmpty()) { return new List<T>(); }
            if (values.IsEmpty()) { return models; }
            var valItems = values.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
            if (valItems.Length == 0) { return models; }
            var funcGetList = PropertyAccess.GetAccess(models.First()).FuncGetDic.Values;
            return models.Where(m => SearchContains<T>(m, valItems, funcGetList));
        }
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="page">模型,为空时,空列表</param>
        /// <param name="values">查询值,为空时,源列表</param>
        /// <returns></returns>
        public static IPageResult<T> SearchModels<T>(this IPageResult<T> page, params string[] values)
        {
            if (page.Items.IsEmpty() || values.IsEmpty()) { return page; }
            var valItems = values.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
            if (valItems.Length == 0) { return page; }
            var funcGetList = PropertyAccess.GetAccess(page.Items.First()).FuncGetDic.Values;
            return page.Reset(page.Items.Where(m => SearchContains<T>(m, valItems, funcGetList)));
        }
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="models"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static IEnumerable<T> SearchOrModels<T>(this IEnumerable<T> models, params string[] values)
        {
            if (models.IsEmpty() || values == null) { return models; }
            var valItems = values.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
            if (valItems.Length == 0) { return models; }
            var dic = new Dictionary<int, List<T>>();
            for (int i = 0; i < valItems.Length; i++)
            {
                dic.Add(i + 1, new List<T>());
            }
            var getFuncList = PropertyAccess.GetAccess(models.First()).FuncGetDic.Values;
            foreach (var model in models)
            {
                var count = SearchContainsCount<T>(model, valItems, getFuncList);
                if (count > 0)
                {
                    dic[count].Add(model);
                }
            }
            var result = new List<T>();
            foreach (var item in dic.OrderByDescending(s => s.Key))
            {
                result.AddRange(item.Value);
            }
            return result;
        }
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="models">模型,为空时,空列表</param>
        /// <param name="searchContent">查询值,为空时,源列表</param>
        /// <param name="properties">查询属性,为空时,false</param>
        /// <returns></returns>
        public static IEnumerable<T> SearchModels<T>(this IEnumerable<T> models, string searchContent, string[] properties) => SearchModels(models, GetSearchKeys(searchContent), properties);
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="models">模型,为空时,空列表</param>
        /// <param name="searchContent">查询值,为空时,源列表</param>
        /// <param name="properties">查询属性,为空时,false</param>
        /// <returns></returns>
        public static IPageResult<T> SearchModels<T>(this IPageResult<T> models, string searchContent, string[] properties) => SearchModels(models, GetSearchKeys(searchContent), properties);
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="models">模型,为空时,空列表</param>
        /// <param name="searchContent">查询值,为空时,源列表</param>
        /// <param name="properties">查询属性,为空时,false</param>
        /// <returns></returns>
        public static IEnumerable<T> SearchOrModels<T>(this IEnumerable<T> models, string searchContent, string[] properties) => SearchOrModels(models, GetSearchKeys(searchContent), properties);
        /// <summary>
        /// 查询模型(指定字段,指定查找值)
        /// </summary>
        /// <param name="models">模型,为空时,空列表</param>
        /// <param name="properties">查找属性,为空时,空列表</param>
        /// <param name="values">查找值,为空时,源列表</param>
        /// <returns></returns>
        public static IEnumerable<T> SearchModels<T>(this IEnumerable<T> models, string[] values, string[] properties)
        {
            if (models.IsEmpty()) { return new List<T>(); }
            if (values.IsEmpty()) { return models; }
            var valItems = values.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
            if (valItems.Length == 0) { return models; }
            var funcGetList = properties.IsEmpty() ? PropertyAccess.GetAccess(models.First()).FuncGetDic.Values : PropertyAccess.GetAccess(models.First()).FuncGetDic.Where(s => properties.Contains(s.Key)).Select(s => s.Value);
            return models.Where(m => SearchContains<T>(m, valItems, funcGetList));
        }
        /// <summary>
        /// 查询模型(指定字段,指定查找值)
        /// </summary>
        /// <param name="page">模型,为空时,空列表</param>
        /// <param name="properties">查找属性,为空时,空列表</param>
        /// <param name="values">查找值,为空时,源列表</param>
        /// <returns></returns>
        public static IPageResult<T> SearchModels<T>(this IPageResult<T> page, string[] values, string[] properties)
        {
            if (page.Items.IsEmpty() || values.IsEmpty()) { return page; }
            var valItems = values.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
            if (valItems.Length == 0) { return page; }
            var funcGetList = properties.IsEmpty() ? PropertyAccess.GetAccess(page.Items.First()).FuncGetDic.Values : PropertyAccess.GetAccess(page.Items.First()).FuncGetDic.Where(s => properties.Contains(s.Key)).Select(s => s.Value);
            return page.Reset(page.Items.Where(m => SearchContains<T>(m, valItems, funcGetList)));
        }
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="models"></param>
        /// <param name="properties"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static IEnumerable<T> SearchOrModels<T>(this IEnumerable<T> models, string[] values, string[] properties)
        {
            if (models.IsEmpty() || values == null) { return models; }
            var valItems = values.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
            if (valItems.Length == 0) { return models; }
            var dic = new Dictionary<int, List<T>>();
            for (int i = 0; i < valItems.Length; i++)
            {
                dic.Add(i + 1, new List<T>());
            }
            var getFuncList = PropertyAccess.GetAccess(models.First()).FuncGetDic.Where(s => properties.Contains(s.Key)).Select(s => s.Value);
            foreach (var model in models)
            {
                var count = SearchContainsCount<T>(model, valItems, getFuncList);
                if (count > 0)
                {
                    dic[count].Add(model);
                }
            }
            var result = new List<T>();
            foreach (var item in dic.OrderByDescending(s => s.Key))
            {
                result.AddRange(item.Value);
            }
            return result;
        }
        private static bool SearchContains<T>(T model, string[] valItems, IEnumerable<Func<object, object>> getFuncList)
        {
            var dic = valItems.ToDictionary(s => s, s => false);
            foreach (var item in getFuncList)
            {
                var val = item.Invoke(model);
                if (val == null) { continue; }
                var valString = val.ToString();
                if (string.IsNullOrWhiteSpace(valString)) { continue; }
                foreach (var vi in valItems)
                {
                    if (valString.Contains(vi))
                    {
                        dic[vi] = true;
                    }
                }
            }
            return dic.All(s => s.Value);
        }
        private static int SearchContainsCount<T>(T model, string[] valItems, IEnumerable<Func<object, object>> getFuncList)
        {
            var count = 0;
            foreach (var item in getFuncList)
            {
                var val = item.Invoke(model);
                if (val == null) { continue; }
                var valString = val.ToString();
                if (string.IsNullOrWhiteSpace(valString)) { continue; }
                foreach (var vi in valItems)
                {
                    if (valString.Contains(vi)) { count++; }
                }
            }
            return count;
        }
        /// <summary>
        /// 获取查询值,分隔符包括两种逗号/空格/竖线
        /// </summary>
        /// <param name="content"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public static string[] GetSearchKeys(string content, char[] split = null)
        {
            split ??= new char[] { ',', '，', '|', ' ' };
            return string.IsNullOrWhiteSpace(content) ? (new string[0]) : content.Trim().Split(split, StringSplitOptions.RemoveEmptyEntries);
        }
        #endregion
        /// <summary>
        /// 获取属性访问
        /// </summary>
        /// <see cref="PropertyAccess{T}"/>
        /// <returns></returns>
        public static IPropertyAccess GetPropertyAccess<T>() => new PropertyAccess<T>();
        /// <summary>
        /// 获取属性访问
        /// </summary>
        /// <see cref="PropertyAccess.Get"/>
        /// <param name="model"></param>
        /// <returns></returns>
        public static IPropertyAccess GetPropertyAccess<T>(this T model) => PropertyAccess.GetAccess(model);
        /// <summary>
        /// 获取属性访问
        /// </summary>
        /// <see cref="PropertyAccess.Get"/>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IPropertyAccess GetPropertyAccess(this Type type) => PropertyAccess.Get(type);
        /// <summary>
        /// 获取静态的成员属性值
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="memberName">成员名称</param>
        /// <returns>成员值</returns>
        public static object GetPropertyValue(this Type type, string memberName)
        {
            return PropertyAccess.Get(type).FuncGetValue.Invoke(null, memberName);
        }
        /// <summary>
        /// 设置静态的成员属性值
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="memberName">成员名称</param>
        /// <param name="newValue">新值</param>
        /// <returns>成员值</returns>
        public static void SetPropertyValue(this Type type, string memberName, object newValue)
        {
            PropertyAccess.Get(type).FuncSetValue.Invoke(null, memberName, newValue);
        }
        /// <summary>
        /// 获取静态的成员属性值
        /// <see cref="PropertyAccess{T}.InternalGetValue"/>
        /// </summary>
        /// <param name="memberName">成员名称</param>
        /// <returns>成员值</returns>
        public static object GetPropertyValue<T>(string memberName)
        {
            return PropertyAccess<T>.InternalGetValue(default(T), memberName);
        }
        /// <summary>
        /// 设置静态的成员属性值
        /// <see cref="PropertyAccess{T}.InternalSetValue"/>
        /// </summary>
        /// <param name="memberName">成员名称</param>
        /// <param name="newValue">新值</param>
        /// <returns>成员值</returns>
        public static void SetPropertyValue<T>(string memberName, object newValue)
        {
            PropertyAccess<T>.InternalSetValue(default(T), memberName, newValue);
        }
        /// <summary>
        /// 转换成属性字典
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Dictionary<string, object> ToParameters<T>(this T model)
        {
            return PropertyAccess<T>.InternalGetDic.ToDictionary(s => s.Key, s => s.Value(model));
        }
        /// <summary>
        /// 获取静态的成员属性值
        /// 如果Type和对象能匹配请使用<see cref="PropertyAccess{T}.InternalGetValue"/>
        /// </summary>
        /// <param name="instance">实例对象,为null时使用泛型类的静态内容</param>
        /// <param name="memberName">成员名称</param>
        /// <returns>成员值</returns>
        public static object GetPropertyValue<T>(this T instance, string memberName) where T : class
        {
            if (instance == null || instance.GetType() == typeof(T)) { return PropertyAccess<T>.InternalGetValue(instance, memberName); }
            return PropertyAccess.Get(instance.GetType()).FuncGetValue(instance, memberName);
        }
        /// <summary>
        /// 设置静态的成员属性值
        /// 如果Type和对象能匹配请使用<see cref="PropertyAccess{T}.InternalSetValue"/>
        /// </summary>
        /// <param name="instance">实例对象,为null时使用泛型类的静态内容</param>
        /// <param name="memberName">成员名称</param>
        /// <param name="newValue">新值</param>
        /// <returns>成员值</returns>
        public static void SetPropertyValue<T>(this T instance, string memberName, object newValue) where T : class
        {
            if (instance == null || instance.GetType() == typeof(T)) { PropertyAccess<T>.InternalSetValue(instance, memberName, newValue); return; }
            PropertyAccess.Get(instance.GetType()).FuncSetValue(instance, memberName, newValue);
        }
        /// <summary>
        /// 设置静态的成员属性值
        /// 如果Type和对象能匹配请使用<see cref="PropertyAccess{T}.InternalInfoDic"/>
        /// </summary>
        /// <param name="instance">实例对象</param>
        /// <param name="model">新值</param>
        /// <returns>成员值</returns>
        public static void SetPropertyValues<T>(this T instance, T model) where T : class
        {
            if (instance == null || model == null) { return; }
            var type = instance.GetType();
            // 按照父类进行数据处理
            var otherType = model.GetType();
            IPropertyAccess access;
            if (type == otherType)
            {
                access = PropertyAccess.Get(type);
            }
            else
            {
                access = otherType.IsAssignableFrom(type) ? PropertyAccess.Get(otherType) : PropertyAccess.Get(type);
            }
            foreach (var item in access.FuncInfoDic)
            {
                var info = item.Value;
                try
                {
                    info.SetValue(instance, info.GetValue.Invoke(model));
                }
                catch { }
            }
        }
        #region // 获取Action/Func表达式的方法全称或方法信息
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName(this Action expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo(Expression<Action> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0>(this Func<T0> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0>(this Action<T0> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0>(Expression<Func<T0>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0>(Expression<Action<T0>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1>(this Func<T0, T1> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1>(this Action<T0, T1> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1>(Expression<Func<T0, T1>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1>(Expression<Action<T0, T1>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2>(this Func<T0, T1, T2> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2>(this Action<T0, T1, T2> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2>(Expression<Func<T0, T1, T2>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2>(Expression<Action<T0, T1, T2>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3>(this Func<T0, T1, T2, T3> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3>(this Action<T0, T1, T2, T3> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3>(Expression<Func<T0, T1, T2, T3>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3>(Expression<Action<T0, T1, T2, T3>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4>(this Func<T0, T1, T2, T3, T4> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4>(this Action<T0, T1, T2, T3, T4> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4>(Expression<Func<T0, T1, T2, T3, T4>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4>(Expression<Action<T0, T1, T2, T3, T4>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5>(this Func<T0, T1, T2, T3, T4, T5> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5>(this Action<T0, T1, T2, T3, T4, T5> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5>(Expression<Func<T0, T1, T2, T3, T4, T5>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5>(Expression<Action<T0, T1, T2, T3, T4, T5>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6>(this Func<T0, T1, T2, T3, T4, T5, T6> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6>(this Action<T0, T1, T2, T3, T4, T5, T6> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6>(Expression<Func<T0, T1, T2, T3, T4, T5, T6>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6>(Expression<Action<T0, T1, T2, T3, T4, T5, T6>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7>(this Func<T0, T1, T2, T3, T4, T5, T6, T7> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7>(this Action<T0, T1, T2, T3, T4, T5, T6, T7> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7>(Expression<Action<T0, T1, T2, T3, T4, T5, T6, T7>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T0, T1, T2, T3, T4, T5, T6, T7, T8> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T0, T1, T2, T3, T4, T5, T6, T7, T8> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7, T8>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8>(Expression<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(Expression<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(Expression<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(Expression<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(Expression<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(Expression<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(Expression<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        #endregion

        #region // CopyClone
        /// <summary>
        /// 深度表达式树复制
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="original">Object to copy.</param>
        /// <param name="copiedReferencesDict">Dictionary of already copied objects (Keys: original objects, Values: their copies).</param>
        /// <returns></returns>
        public static T DeepExpressionCopy<T>(this T original, Dictionary<object, object> copiedReferencesDict = null)
        {
            return (T)DeepExpressionTreeObjCopy(original, false, copiedReferencesDict ?? new Dictionary<object, object>(new ReferenceEqualityComparer()));
        }
        /// <summary>
        /// 深度Json的Serialize复制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DeepJsonCopy<T>(T obj)
        {
            return obj.GetJsonString().GetJsonObject<T>();
        }
        /// <summary>
        /// 深度反射复制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original"></param>
        /// <returns></returns>
        public static T DeepReflectionCopy<T>(this T original)
        {
            return (T)ReflectionInternalCopy((object)original, new Dictionary<Object, object>(new ReferenceEqualityComparer()));
        }
        #region // 内部方法及类
        internal class ReferenceEqualityComparer : EqualityComparer<Object>
        {
            public override bool Equals(object x, object y)
            {
                return ReferenceEquals(x, y);
            }
            public override int GetHashCode(object obj)
            {
                if (obj == null) return 0;
                return obj.GetHashCode();
            }
        }
        internal static void ForEach(this Array array, Action<Array, int[]> action)
        {
            if (array.LongLength == 0) return;
            ArrayTraverse walker = new ArrayTraverse(array);
            do action(array, walker.Position);
            while (walker.Step());
        }
        internal class ArrayTraverse
        {
            public int[] Position;
            private int[] maxLengths;

            public ArrayTraverse(Array array)
            {
                maxLengths = new int[array.Rank];
                for (int i = 0; i < array.Rank; ++i)
                {
                    maxLengths[i] = array.GetLength(i) - 1;
                }
                Position = new int[array.Rank];
            }

            public bool Step()
            {
                for (int i = 0; i < Position.Length; ++i)
                {
                    if (Position[i] < maxLengths[i])
                    {
                        Position[i]++;
                        for (int j = 0; j < i; j++)
                        {
                            Position[j] = 0;
                        }
                        return true;
                    }
                }
                return false;
            }
        }
        #region // 表达式树
        private static readonly object ExpressionTreeStructTypeDeepCopyLocker = new object();
        private static Dictionary<Type, bool> ExpressionTreeStructTypeToDeepCopyDic = new Dictionary<Type, bool>();

        private static readonly object ExpressionTreeCompiledCopyFuncDicLocker = new object();
        private static Dictionary<Type, Func<object, Dictionary<object, object>, object>> ExpressionTreeCompiledCopyFuncDic = new Dictionary<Type, Func<object, Dictionary<object, object>, object>>();

        private static readonly Type ExpressionTreeObjectType = typeof(Object);
        private static readonly Type ExpressionTreeObjectDictionaryType = typeof(Dictionary<object, object>);
        private static readonly Type ExpressionTreeFieldInfoType = typeof(FieldInfo);
        private static readonly MethodInfo SetValueMethod = ExpressionTreeFieldInfoType.GetMethod("SetValue", new[] { ExpressionTreeObjectType, ExpressionTreeObjectType });
        private static readonly Type ThisType = typeof(ExtterCaller);
        private static readonly MethodInfo DeepCopyByExpressionTreeObjMethod = ThisType.GetMethod(nameof(DeepExpressionTreeObjCopy), BindingFlags.NonPublic | BindingFlags.Static);

        internal static object DeepExpressionTreeObjCopy(object original, bool forceDeepCopy, Dictionary<object, object> copiedReferencesDict)
        {
            if (original == null)
            {
                return null;
            }

            var type = original.GetType();

            if (ExpressionTreeIsDelegate(type))
            {
                return null;
            }

            if (!forceDeepCopy && !ExpressionTreeIsTypeToDeepCopy(type))
            {
                return original;
            }

            object alreadyCopiedObject;

            if (copiedReferencesDict.TryGetValue(original, out alreadyCopiedObject))
            {
                return alreadyCopiedObject;
            }

            if (type == ExpressionTreeObjectType)
            {
                return new object();
            }

            var compiledCopyFunction = ExpressionTreeGetOrCreateCompiledLambdaCopyFunc(type);

            object copy = compiledCopyFunction(original, copiedReferencesDict);

            return copy;
        }

        private static Func<object, Dictionary<object, object>, object> ExpressionTreeGetOrCreateCompiledLambdaCopyFunc(Type type)
        {
            // The following structure ensures that multiple threads can use the dictionary
            // even while dictionary is locked and being updated by other thread.
            // That is why we do not modify the old dictionary instance but
            // we replace it with a new instance everytime.

            Func<object, Dictionary<object, object>, object> compiledCopyFunction;

            if (!ExpressionTreeCompiledCopyFuncDic.TryGetValue(type, out compiledCopyFunction))
            {
                lock (ExpressionTreeCompiledCopyFuncDicLocker)
                {
                    if (!ExpressionTreeCompiledCopyFuncDic.TryGetValue(type, out compiledCopyFunction))
                    {
                        var uncompiledCopyFunction = ExpressionTreeCreateCompiledLambdaCopyFuncForType(type);

                        compiledCopyFunction = uncompiledCopyFunction.Compile();

                        var dictionaryCopy = ExpressionTreeCompiledCopyFuncDic.ToDictionary(pair => pair.Key, pair => pair.Value);

                        dictionaryCopy.Add(type, compiledCopyFunction);

                        ExpressionTreeCompiledCopyFuncDic = dictionaryCopy;
                    }
                }
            }

            return compiledCopyFunction;
        }

        private static Expression<Func<object, Dictionary<object, object>, object>> ExpressionTreeCreateCompiledLambdaCopyFuncForType(Type type)
        {
            ParameterExpression inputParameter;
            ParameterExpression inputDictionary;
            ParameterExpression outputVariable;
            ParameterExpression boxingVariable;
            LabelTarget endLabel;
            List<ParameterExpression> variables;
            List<Expression> expressions;

            ///// INITIALIZATION OF EXPRESSIONS AND VARIABLES

            ExpressionTreeInitializeExpressions(type,
                                  out inputParameter,
                                  out inputDictionary,
                                  out outputVariable,
                                  out boxingVariable,
                                  out endLabel,
                                  out variables,
                                  out expressions);

            ///// RETURN NULL IF ORIGINAL IS NULL

            ExpressionTreeIfNullThenReturnNull(inputParameter, endLabel, expressions);

            ///// MEMBERWISE CLONE ORIGINAL OBJECT

            ExpressionTreeMemberwiseCloneInputToOutput(type, inputParameter, outputVariable, expressions);

            ///// STORE COPIED OBJECT TO REFERENCES DICTIONARY

            if (ExpressionTreeIsClassOtherThanString(type))
            {
                ExpressionTreeStoreReferencesIntoDictionary(inputParameter, inputDictionary, outputVariable, expressions);
            }

            ///// COPY ALL NONVALUE OR NONPRIMITIVE FIELDS

            ExpressionTreeFieldsCopy(type,
                                  inputParameter,
                                  inputDictionary,
                                  outputVariable,
                                  boxingVariable,
                                  expressions);

            ///// COPY ELEMENTS OF ARRAY

            if (type.IsArray && ExpressionTreeIsTypeToDeepCopy(type.GetElementType()))
            {
                ExpressionTreeCreateArrayCopyLoop(type,
                                              inputParameter,
                                              inputDictionary,
                                              outputVariable,
                                              variables,
                                              expressions);
            }

            ///// COMBINE ALL EXPRESSIONS INTO LAMBDA FUNCTION

            var lambda = ExpressionTreeCombineAllIntoLambdaFunc(inputParameter, inputDictionary, outputVariable, endLabel, variables, expressions);

            return lambda;
        }

        private static void ExpressionTreeInitializeExpressions(Type type,
                                                  out ParameterExpression inputParameter,
                                                  out ParameterExpression inputDictionary,
                                                  out ParameterExpression outputVariable,
                                                  out ParameterExpression boxingVariable,
                                                  out LabelTarget endLabel,
                                                  out List<ParameterExpression> variables,
                                                  out List<Expression> expressions)
        {

            inputParameter = Expression.Parameter(ExpressionTreeObjectType);

            inputDictionary = Expression.Parameter(ExpressionTreeObjectDictionaryType);

            outputVariable = Expression.Variable(type);

            boxingVariable = Expression.Variable(ExpressionTreeObjectType);

            endLabel = Expression.Label();

            variables = new List<ParameterExpression>();

            expressions = new List<Expression>();

            variables.Add(outputVariable);
            variables.Add(boxingVariable);
        }

        private static void ExpressionTreeIfNullThenReturnNull(ParameterExpression inputParameter, LabelTarget endLabel, List<Expression> expressions)
        {
            ///// Intended code:
            /////
            ///// if (input == null)
            ///// {
            /////     return null;
            ///// }

            var ifNullThenReturnNullExpression =
                Expression.IfThen(
                    Expression.Equal(
                        inputParameter,
                        Expression.Constant(null, ExpressionTreeObjectType)),
                    Expression.Return(endLabel));

            expressions.Add(ifNullThenReturnNullExpression);
        }

        private static void ExpressionTreeMemberwiseCloneInputToOutput(
            Type type,
            ParameterExpression inputParameter,
            ParameterExpression outputVariable,
            List<Expression> expressions)
        {
            ///// Intended code:
            /////
            ///// var output = (<type>)input.MemberwiseClone();

            var memberwiseCloneMethod = ExpressionTreeObjectType.GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);

            var memberwiseCloneInputExpression =
                Expression.Assign(
                    outputVariable,
                    Expression.Convert(
                        Expression.Call(
                            inputParameter,
                            memberwiseCloneMethod),
                        type));

            expressions.Add(memberwiseCloneInputExpression);
        }

        private static void ExpressionTreeStoreReferencesIntoDictionary(ParameterExpression inputParameter,
                                                                          ParameterExpression inputDictionary,
                                                                          ParameterExpression outputVariable,
                                                                          List<Expression> expressions)
        {
            ///// Intended code:
            /////
            ///// inputDictionary[(Object)input] = (Object)output;

            var storeReferencesExpression =
                Expression.Assign(
                    Expression.Property(
                        inputDictionary,
                        ExpressionTreeObjectDictionaryType.GetProperty("Item"),
                        inputParameter),
                    Expression.Convert(outputVariable, ExpressionTreeObjectType));

            expressions.Add(storeReferencesExpression);
        }

        private static Expression<Func<object, Dictionary<object, object>, object>> ExpressionTreeCombineAllIntoLambdaFunc(
            ParameterExpression inputParameter,
            ParameterExpression inputDictionary,
            ParameterExpression outputVariable,
            LabelTarget endLabel,
            List<ParameterExpression> variables,
            List<Expression> expressions)
        {
            expressions.Add(Expression.Label(endLabel));

            expressions.Add(Expression.Convert(outputVariable, ExpressionTreeObjectType));

            var finalBody = Expression.Block(variables, expressions);

            var lambda = Expression.Lambda<Func<object, Dictionary<object, object>, object>>(finalBody, inputParameter, inputDictionary);

            return lambda;
        }

        private static void ExpressionTreeCreateArrayCopyLoop(Type type,
                                                          ParameterExpression inputParameter,
                                                          ParameterExpression inputDictionary,
                                                          ParameterExpression outputVariable,
                                                          List<ParameterExpression> variables,
                                                          List<Expression> expressions)
        {
            ///// Intended code:
            /////
            ///// int i1, i2, ..., in; 
            ///// 
            ///// int length1 = inputarray.GetLength(0); 
            ///// i1 = 0; 
            ///// while (true)
            ///// {
            /////     if (i1 >= length1)
            /////     {
            /////         goto ENDLABELFORLOOP1;
            /////     }
            /////     int length2 = inputarray.GetLength(1); 
            /////     i2 = 0; 
            /////     while (true)
            /////     {
            /////         if (i2 >= length2)
            /////         {
            /////             goto ENDLABELFORLOOP2;
            /////         }
            /////         ...
            /////         ...
            /////         ...
            /////         int lengthn = inputarray.GetLength(n); 
            /////         in = 0; 
            /////         while (true)
            /////         {
            /////             if (in >= lengthn)
            /////             {
            /////                 goto ENDLABELFORLOOPn;
            /////             }
            /////             outputarray[i1, i2, ..., in] 
            /////                 = (<elementType>)DeepCopyByExpressionTreeObj(
            /////                        (Object)inputarray[i1, i2, ..., in])
            /////             in++; 
            /////         }
            /////         ENDLABELFORLOOPn:
            /////         ...
            /////         ...  
            /////         ...
            /////         i2++; 
            /////     }
            /////     ENDLABELFORLOOP2:
            /////     i1++; 
            ///// }
            ///// ENDLABELFORLOOP1:

            var rank = type.GetArrayRank();

            var indices = ExpressionTreeGenerateIndices(rank);

            variables.AddRange(indices);

            var elementType = type.GetElementType();

            var assignExpression = ExpressionTreeArrayFieldToArrayFieldAssign(inputParameter, inputDictionary, outputVariable, elementType, type, indices);

            Expression forExpression = assignExpression;

            for (int dimension = 0; dimension < rank; dimension++)
            {
                var indexVariable = indices[dimension];

                forExpression = ExpressionTreeLoopIntoLoop(inputParameter, indexVariable, forExpression, dimension);
            }

            expressions.Add(forExpression);
        }

        private static List<ParameterExpression> ExpressionTreeGenerateIndices(int arrayRank)
        {
            ///// Intended code:
            /////
            ///// int i1, i2, ..., in; 

            var indices = new List<ParameterExpression>();

            for (int i = 0; i < arrayRank; i++)
            {
                var indexVariable = Expression.Variable(typeof(Int32));

                indices.Add(indexVariable);
            }

            return indices;
        }

        private static BinaryExpression ExpressionTreeArrayFieldToArrayFieldAssign(
            ParameterExpression inputParameter,
            ParameterExpression inputDictionary,
            ParameterExpression outputVariable,
            Type elementType,
            Type arrayType,
            List<ParameterExpression> indices)
        {
            ///// Intended code:
            /////
            ///// outputarray[i1, i2, ..., in] 
            /////     = (<elementType>)DeepCopyByExpressionTreeObj(
            /////            (Object)inputarray[i1, i2, ..., in]);

            var indexTo = Expression.ArrayAccess(outputVariable, indices);

            var indexFrom = Expression.ArrayIndex(Expression.Convert(inputParameter, arrayType), indices);

            var forceDeepCopy = elementType != ExpressionTreeObjectType;

            var rightSide =
                Expression.Convert(
                    Expression.Call(
                        DeepCopyByExpressionTreeObjMethod,
                        Expression.Convert(indexFrom, ExpressionTreeObjectType),
                        Expression.Constant(forceDeepCopy, typeof(Boolean)),
                        inputDictionary),
                    elementType);

            var assignExpression = Expression.Assign(indexTo, rightSide);

            return assignExpression;
        }

        private static BlockExpression ExpressionTreeLoopIntoLoop(
            ParameterExpression inputParameter,
            ParameterExpression indexVariable,
            Expression loopToEncapsulate,
            int dimension)
        {
            ///// Intended code:
            /////
            ///// int length = inputarray.GetLength(dimension); 
            ///// i = 0; 
            ///// while (true)
            ///// {
            /////     if (i >= length)
            /////     {
            /////         goto ENDLABELFORLOOP;
            /////     }
            /////     loopToEncapsulate;
            /////     i++; 
            ///// }
            ///// ENDLABELFORLOOP:

            var lengthVariable = Expression.Variable(typeof(Int32));

            var endLabelForThisLoop = Expression.Label();

            var newLoop =
                Expression.Loop(
                    Expression.Block(
                        new ParameterExpression[0],
                        Expression.IfThen(
                            Expression.GreaterThanOrEqual(indexVariable, lengthVariable),
                            Expression.Break(endLabelForThisLoop)),
                        loopToEncapsulate,
                        Expression.PostIncrementAssign(indexVariable)),
                    endLabelForThisLoop);

            var lengthAssignment = ExpressionTreeGetLengthForDimension(lengthVariable, inputParameter, dimension);

            var indexAssignment = Expression.Assign(indexVariable, Expression.Constant(0));

            return Expression.Block(
                new[] { lengthVariable },
                lengthAssignment,
                indexAssignment,
                newLoop);
        }

        private static BinaryExpression ExpressionTreeGetLengthForDimension(
            ParameterExpression lengthVariable,
            ParameterExpression inputParameter,
            int i)
        {
            ///// Intended code:
            /////
            ///// length = ((Array)input).GetLength(i); 

            var getLengthMethod = typeof(Array).GetMethod("GetLength", BindingFlags.Public | BindingFlags.Instance);

            var dimensionConstant = Expression.Constant(i);

            return Expression.Assign(
                lengthVariable,
                Expression.Call(
                    Expression.Convert(inputParameter, typeof(Array)),
                    getLengthMethod,
                    new[] { dimensionConstant }));
        }

        private static void ExpressionTreeFieldsCopy(Type type,
                                                  ParameterExpression inputParameter,
                                                  ParameterExpression inputDictionary,
                                                  ParameterExpression outputVariable,
                                                  ParameterExpression boxingVariable,
                                                  List<Expression> expressions)
        {
            var fields = ExpressionTreeGetAllRelevantFields(type);

            var readonlyFields = fields.Where(f => f.IsInitOnly).ToList();
            var writableFields = fields.Where(f => !f.IsInitOnly).ToList();

            ///// READONLY FIELDS COPY (with boxing)

            bool shouldUseBoxing = readonlyFields.Any();

            if (shouldUseBoxing)
            {
                var boxingExpression = Expression.Assign(boxingVariable, Expression.Convert(outputVariable, ExpressionTreeObjectType));

                expressions.Add(boxingExpression);
            }

            foreach (var field in readonlyFields)
            {
                if (ExpressionTreeIsDelegate(field.FieldType))
                {
                    ExpressionTreeReadonlyFieldToNull(field, boxingVariable, expressions);
                }
                else
                {
                    ExpressionTreeReadonlyFieldCopy(type,
                                                field,
                                                inputParameter,
                                                inputDictionary,
                                                boxingVariable,
                                                expressions);
                }
            }

            if (shouldUseBoxing)
            {
                var unboxingExpression = Expression.Assign(outputVariable, Expression.Convert(boxingVariable, type));

                expressions.Add(unboxingExpression);
            }

            ///// NOT-READONLY FIELDS COPY

            foreach (var field in writableFields)
            {
                if (ExpressionTreeIsDelegate(field.FieldType))
                {
                    ExpressionTreeWritableFieldToNull(field, outputVariable, expressions);
                }
                else
                {
                    ExpressionTreeWritableFieldCopy(type,
                                                field,
                                                inputParameter,
                                                inputDictionary,
                                                outputVariable,
                                                expressions);
                }
            }
        }

        private static FieldInfo[] ExpressionTreeGetAllRelevantFields(Type type, bool forceAllFields = false)
        {
            var fieldsList = new List<FieldInfo>();

            var typeCache = type;

            while (typeCache != null)
            {
                fieldsList.AddRange(
                    typeCache
                        .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
                        .Where(field => forceAllFields || ExpressionTreeIsTypeToDeepCopy(field.FieldType)));

                typeCache = typeCache.BaseType;
            }

            return fieldsList.ToArray();
        }

        private static void ExpressionTreeReadonlyFieldToNull(FieldInfo field, ParameterExpression boxingVariable, List<Expression> expressions)
        {
            // This option must be implemented by Reflection because of the following:
            // https://visualstudio.uservoice.com/forums/121579-visual-studio-2015/suggestions/2727812-allow-expression-assign-to-set-readonly-struct-f

            ///// Intended code:
            /////
            ///// fieldInfo.SetValue(boxing, <fieldtype>null);

            var fieldToNullExpression =
                    Expression.Call(
                        Expression.Constant(field),
                        SetValueMethod,
                        boxingVariable,
                        Expression.Constant(null, field.FieldType));

            expressions.Add(fieldToNullExpression);
        }

        private static void ExpressionTreeReadonlyFieldCopy(Type type,
                                                        FieldInfo field,
                                                        ParameterExpression inputParameter,
                                                        ParameterExpression inputDictionary,
                                                        ParameterExpression boxingVariable,
                                                        List<Expression> expressions)
        {
            // This option must be implemented by Reflection (SetValueMethod) because of the following:
            // https://visualstudio.uservoice.com/forums/121579-visual-studio-2015/suggestions/2727812-allow-expression-assign-to-set-readonly-struct-f

            ///// Intended code:
            /////
            ///// fieldInfo.SetValue(boxing, DeepCopyByExpressionTreeObj((Object)((<type>)input).<field>))

            var fieldFrom = Expression.Field(Expression.Convert(inputParameter, type), field);

            var forceDeepCopy = field.FieldType != ExpressionTreeObjectType;

            var fieldDeepCopyExpression =
                Expression.Call(
                    Expression.Constant(field, ExpressionTreeFieldInfoType),
                    SetValueMethod,
                    boxingVariable,
                    Expression.Call(
                        DeepCopyByExpressionTreeObjMethod,
                        Expression.Convert(fieldFrom, ExpressionTreeObjectType),
                        Expression.Constant(forceDeepCopy, typeof(Boolean)),
                        inputDictionary));

            expressions.Add(fieldDeepCopyExpression);
        }

        private static void ExpressionTreeWritableFieldToNull(FieldInfo field, ParameterExpression outputVariable, List<Expression> expressions)
        {
            ///// Intended code:
            /////
            ///// output.<field> = (<type>)null;

            var fieldTo = Expression.Field(outputVariable, field);

            var fieldToNullExpression =
                Expression.Assign(
                    fieldTo,
                    Expression.Constant(null, field.FieldType));

            expressions.Add(fieldToNullExpression);
        }

        private static void ExpressionTreeWritableFieldCopy(Type type,
                                                        FieldInfo field,
                                                        ParameterExpression inputParameter,
                                                        ParameterExpression inputDictionary,
                                                        ParameterExpression outputVariable,
                                                        List<Expression> expressions)
        {
            ///// Intended code:
            /////
            ///// output.<field> = (<fieldType>)DeepCopyByExpressionTreeObj((Object)((<type>)input).<field>);

            var fieldFrom = Expression.Field(Expression.Convert(inputParameter, type), field);

            var fieldType = field.FieldType;

            var fieldTo = Expression.Field(outputVariable, field);

            var forceDeepCopy = field.FieldType != ExpressionTreeObjectType;

            var fieldDeepCopyExpression =
                Expression.Assign(
                    fieldTo,
                    Expression.Convert(
                        Expression.Call(
                            DeepCopyByExpressionTreeObjMethod,
                            Expression.Convert(fieldFrom, ExpressionTreeObjectType),
                            Expression.Constant(forceDeepCopy, typeof(Boolean)),
                            inputDictionary),
                        fieldType));

            expressions.Add(fieldDeepCopyExpression);
        }

        private static bool ExpressionTreeIsDelegate(Type type)
        {
            return typeof(Delegate).IsAssignableFrom(type);
        }

        private static bool ExpressionTreeIsTypeToDeepCopy(Type type)
        {
            return ExpressionTreeIsClassOtherThanString(type) || ExpressionTreeIsStructWhichNeedsDeepCopy(type);
        }

        private static bool ExpressionTreeIsClassOtherThanString(Type type)
        {
            return !type.IsValueType && type != typeof(String);
        }

        private static bool ExpressionTreeIsStructWhichNeedsDeepCopy(Type type)
        {
            // The following structure ensures that multiple threads can use the dictionary
            // even while dictionary is locked and being updated by other thread.
            // That is why we do not modify the old dictionary instance but
            // we replace it with a new instance everytime.

            bool isStructTypeToDeepCopy;

            if (!ExpressionTreeStructTypeToDeepCopyDic.TryGetValue(type, out isStructTypeToDeepCopy))
            {
                lock (ExpressionTreeStructTypeDeepCopyLocker)
                {
                    if (!ExpressionTreeStructTypeToDeepCopyDic.TryGetValue(type, out isStructTypeToDeepCopy))
                    {
                        isStructTypeToDeepCopy = ExpressionTreeIsStructOtherThanBasicValueTypes(type) && ExpressionTreeHasInItsHierarchyFieldsWithClasses(type);

                        var newDictionary = ExpressionTreeStructTypeToDeepCopyDic.ToDictionary(pair => pair.Key, pair => pair.Value);

                        newDictionary[type] = isStructTypeToDeepCopy;

                        ExpressionTreeStructTypeToDeepCopyDic = newDictionary;
                    }
                }
            }

            return isStructTypeToDeepCopy;
        }

        private static bool ExpressionTreeIsStructOtherThanBasicValueTypes(Type type)
        {
            return type.IsValueType && !type.IsPrimitive && !type.IsEnum && type != typeof(Decimal);
        }

        private static bool ExpressionTreeHasInItsHierarchyFieldsWithClasses(Type type, HashSet<Type> alreadyCheckedTypes = null)
        {
            alreadyCheckedTypes = alreadyCheckedTypes ?? new HashSet<Type>();

            alreadyCheckedTypes.Add(type);

            var allFields = ExpressionTreeGetAllRelevantFields(type, forceAllFields: true);

            var allFieldTypes = allFields.Select(f => f.FieldType).Distinct().ToList();

            var hasFieldsWithClasses = allFieldTypes.Any(ExpressionTreeIsClassOtherThanString);

            if (hasFieldsWithClasses)
            {
                return true;
            }

            var notBasicStructsTypes = allFieldTypes.Where(ExpressionTreeIsStructOtherThanBasicValueTypes).ToList();

            var typesToCheck = notBasicStructsTypes.Where(t => !alreadyCheckedTypes.Contains(t)).ToList();

            foreach (var typeToCheck in typesToCheck)
            {
                if (ExpressionTreeHasInItsHierarchyFieldsWithClasses(typeToCheck, alreadyCheckedTypes))
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
        #region // 反射方法
        private static readonly MethodInfo CloneMethod = typeof(Object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);
        internal static bool IsPrimitive(Type type)
        {
            if (type == typeof(String)) return true;
            return (type.IsValueType & type.IsPrimitive);
        }

        internal static Object ReflectionInternalCopy(Object originalObject, IDictionary<Object, Object> visited)
        {
            if (originalObject == null) return null;
            var typeToReflect = originalObject.GetType();
            if (IsPrimitive(typeToReflect)) return originalObject;
            if (visited.ContainsKey(originalObject)) return visited[originalObject];
            if (typeof(Delegate).IsAssignableFrom(typeToReflect)) return null;
            var cloneObject = CloneMethod.Invoke(originalObject, null);
            if (typeToReflect.IsArray)
            {
                var arrayType = typeToReflect.GetElementType();
                if (IsPrimitive(arrayType) == false)
                {
                    Array clonedArray = (Array)cloneObject;
                    clonedArray.ForEach((array, indices) => array.SetValue(ReflectionInternalCopy(clonedArray.GetValue(indices), visited), indices));
                }

            }
            visited.Add(originalObject, cloneObject);
            ReflectionCopyFields(originalObject, visited, cloneObject, typeToReflect);
            ReflectionRecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);
            return cloneObject;
        }

        private static void ReflectionRecursiveCopyBaseTypePrivateFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect)
        {
            if (typeToReflect.BaseType != null)
            {
                ReflectionRecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.BaseType);
                ReflectionCopyFields(originalObject, visited, cloneObject, typeToReflect.BaseType, BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate);
            }
        }

        private static void ReflectionCopyFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy, Func<FieldInfo, bool> filter = null)
        {
            foreach (FieldInfo fieldInfo in typeToReflect.GetFields(bindingFlags))
            {
                if (typeToReflect.Name == "Entry" && fieldInfo.Name == "value")
                { }

                if (filter != null && filter(fieldInfo) == false) continue;
                if (IsPrimitive(fieldInfo.FieldType)) continue;
                var originalFieldValue = fieldInfo.GetValue(originalObject);
                var clonedFieldValue = ReflectionInternalCopy(originalFieldValue, visited);
                fieldInfo.SetValue(cloneObject, clonedFieldValue);
            }
        }
        #endregion
        #endregion
        #endregion

        #region // 类型内容
        /// <summary>
        /// 从程序集中搜索类型(搜到就返回)
        /// 完全匹配 不区分大小写
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public static Type SearchTypeFromAssembies(string fullName)
        {
            Type type = null;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var intype = assembly.GetType(fullName, false, true);
                if (intype != null)
                {
                    return intype;
                }
            }
            return type;
        }
        #endregion

        #endregion 成员调用 MemberInfo PropertyInfo MethodInfo

        #region // 数字调用 Number
        #region // 读取字节
        /// <summary>
        /// 读取短整型字节
        /// </summary>
        /// <returns></returns>
        unsafe public static byte[] GetBytes(this Int16 number, bool isBigEndian = true)
        {
            byte[] array = new byte[2];
            fixed (byte* ptr = array)
            {
                *(short*)ptr = number;
            }
            return isBigEndian ? new byte[2] { array[1], array[0] } : array;
        }
        /// <summary>
        /// 读取短整型字节
        /// </summary>
        /// <returns></returns>
        [Obsolete(nameof(GetBytes))]
        public static byte[] ReadBytes(this Int16 number)
        {
            return new byte[2]
            {
                (byte)(number>>8),
                (byte)number,
            };
        }
        /// <summary>
        /// 读取无符号短整型字节
        /// </summary>
        /// <returns></returns>
        unsafe public static byte[] GetBytes(this UInt16 number, bool isBigEndian = true)
        {
            byte[] array = new byte[2];
            fixed (byte* ptr = array)
            {
                *(ushort*)ptr = number;
            }
            return isBigEndian ? new byte[2] { array[1], array[0] } : array;
        }
        /// <summary>
        /// 读取无符号短整型字节
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetBytes))]
        public static byte[] ReadBytes(this UInt16 number)
        {
            return new byte[2]
            {
                (byte)(number>>8),
                (byte)number,
            };
        }
        /// <summary>
        /// 读取短整型字节
        /// </summary>
        /// <returns></returns>
        unsafe public static sbyte[] GetSBytes(this Int16 number, bool isBigEndian = true)
        {
            sbyte[] array = new sbyte[2];
            fixed (sbyte* ptr = array)
            {
                *(short*)ptr = number;
            }
            return isBigEndian ? new sbyte[2] { array[1], array[0] } : array;
        }
        /// <summary>
        /// 读取短整型字节
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetSBytes))]
        public static sbyte[] ReadSBytes(this Int16 number)
        {
            return new sbyte[2]
            {
                (sbyte)(number>>8),
                (sbyte)number,
            };
        }
        /// <summary>
        /// 读取无符号短整型字节
        /// </summary>
        /// <returns></returns>
        unsafe public static sbyte[] GetSBytes(this UInt16 number, bool isBigEndian = true)
        {
            sbyte[] array = new sbyte[2];
            fixed (sbyte* ptr = array)
            {
                *(ushort*)ptr = number;
            }
            return isBigEndian ? new sbyte[2] { array[1], array[0] } : array;
        }
        /// <summary>
        /// 读取无符号短整型字节
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetSBytes))]
        public static sbyte[] ReadSBytes(this UInt16 number)
        {
            return new sbyte[2]
            {
                (sbyte)(number>>8),
                (sbyte)number,
            };
        }
        /// <summary>
        /// 读取整型字节
        /// </summary>
        /// <returns></returns>
        unsafe public static byte[] GetBytes(this Int32 number, bool isBigEndian = true)
        {
            byte[] array = new byte[4];
            fixed (byte* ptr = array)
            {
                *(int*)ptr = number;
            }
            return isBigEndian ? new byte[4] { array[3], array[2], array[1], array[0] } : array;
        }
        /// <summary>
        /// 读取整型字节(高位开始)
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetBytes))]
        public static byte[] ReadBytes(this Int32 number)
        {
            return new Byte[4]
            {
                (byte)(number>>24),
                (byte)(number>>16),
                (byte)(number>>8),
                (byte)(number>>0),
            };
        }
        /// <summary>
        /// 读取整型字节
        /// </summary>
        /// <returns></returns>
        unsafe public static sbyte[] GetSBytes(this Int32 number, bool isBigEndian = true)
        {
            sbyte[] array = new sbyte[4];
            fixed (sbyte* ptr = array)
            {
                *(int*)ptr = number;
            }
            return isBigEndian ? new sbyte[4] { array[3], array[2], array[1], array[0] } : array;
        }
        /// <summary>
        /// 读取整型字节
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetSBytes))]
        public static sbyte[] ReadSBytes(this Int32 number)
        {
            return new SByte[4]
            {
                (sbyte)(number>>24),
                (sbyte)(number>>16),
                (sbyte)(number>>8),
                (sbyte)(number>>0),
            };
        }
        /// <summary>
        /// 读取无符号整型字节
        /// </summary>
        /// <returns></returns>
        unsafe public static byte[] GetBytes(this UInt32 number, bool isBigEndian = true)
        {
            byte[] array = new byte[4];
            fixed (byte* ptr = array)
            {
                *(uint*)ptr = number;
            }
            return isBigEndian ? new byte[4] { array[3], array[2], array[1], array[0] } : array;
        }
        /// <summary>
        /// 读取无符号整型字节
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetBytes))]
        public static byte[] ReadBytes(this UInt32 number)
        {
            return new Byte[4]
            {
                (byte)(number>>24),
                (byte)(number>>16),
                (byte)(number>>8),
                (byte)(number>>0),
            };
        }
        /// <summary>
        /// 读取无符号整型字节
        /// </summary>
        /// <returns></returns>
        unsafe public static sbyte[] GetSBytes(this UInt32 number, bool isBigEndian = true)
        {
            sbyte[] array = new sbyte[4];
            fixed (sbyte* ptr = array)
            {
                *(uint*)ptr = number;
            }
            return isBigEndian ? new sbyte[4] { array[3], array[2], array[1], array[0] } : array;
        }
        /// <summary>
        /// 读取无符号整型字节
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetSBytes))]
        public static sbyte[] ReadSBytes(this UInt32 number)
        {
            return new SByte[4]
            {
                (sbyte)(number>>24),
                (sbyte)(number>>16),
                (sbyte)(number>>8),
                (sbyte)(number>>0),
            };
        }
        /// <summary>
        /// 读取长整型字节
        /// </summary>
        /// <returns></returns>
        unsafe public static byte[] GetBytes(this Int64 number, bool isBigEndian = true)
        {
            byte[] array = new byte[8];
            fixed (byte* ptr = array)
            {
                *(long*)ptr = number;
            }
            return isBigEndian ? new byte[8] { array[7], array[6], array[5], array[4], array[3], array[2], array[1], array[0] } : array;
        }
        /// <summary>
        /// 读取长整型字节
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetBytes))]
        public static byte[] ReadBytes(this Int64 number)
        {
            var num1 = number >> 32;
            var num2 = (int)number;
            return new byte[8]
            {
                (byte)(num1>>24),
                (byte)(num1>>16),
                (byte)(num1>>8),
                (byte)num1,
                (byte)(num2>>24),
                (byte)(num2>>16),
                (byte)(num2>>8),
                (byte)num2,
            };
        }
        /// <summary>
        /// 读取长整型字节
        /// </summary>
        /// <returns></returns>
        unsafe public static sbyte[] GetSBytes(this Int64 number, bool isBigEndian = true)
        {
            sbyte[] array = new sbyte[8];
            fixed (sbyte* ptr = array)
            {
                *(long*)ptr = number;
            }
            return isBigEndian ? new sbyte[8] { array[7], array[6], array[5], array[4], array[3], array[2], array[1], array[0] } : array;
        }
        /// <summary>
        /// 读取长整型字节
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetSBytes))]
        public static sbyte[] ReadSBytes(this Int64 number)
        {
            var num1 = number >> 32;
            var num2 = (int)number;
            return new sbyte[8]
            {
                (sbyte)(num1>>24),
                (sbyte)(num1>>16),
                (sbyte)(num1>>8),
                (sbyte)num1,
                (sbyte)(num2>>24),
                (sbyte)(num2>>16),
                (sbyte)(num2>>8),
                (sbyte)num2,
            };
        }
        /// <summary>
        /// 读取无符号长整型字节
        /// </summary>
        /// <returns></returns>
        unsafe public static byte[] GetBytes(this UInt64 number, bool isBigEndian = true)
        {
            byte[] array = new byte[8];
            fixed (byte* ptr = array)
            {
                *(ulong*)ptr = number;
            }
            return isBigEndian ? new byte[8] { array[7], array[6], array[5], array[4], array[3], array[2], array[1], array[0] } : array;
        }
        /// <summary>
        /// 读取无符号长整型字节
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetBytes))]
        public static byte[] ReadBytes(this UInt64 number)
        {
            UInt32 num1 = (UInt32)(number >> 32);
            UInt32 num2 = (UInt32)number;
            return new byte[8]
            {
                (byte)(num1>>24),
                (byte)(num1>>16),
                (byte)(num1>>8),
                (byte)num1,
                (byte)(num2>>24),
                (byte)(num2>>16),
                (byte)(num2>>8),
                (byte)num2,
            };
        }
        /// <summary>
        /// 读取无符号长整型字节
        /// </summary>
        /// <returns></returns>
        unsafe public static sbyte[] GetSBytes(this UInt64 number, bool isBigEndian = true)
        {
            sbyte[] array = new sbyte[8];
            fixed (sbyte* ptr = array)
            {
                *(ulong*)ptr = number;
            }
            return isBigEndian ? new sbyte[8] { array[7], array[6], array[5], array[4], array[3], array[2], array[1], array[0] } : array;
        }
        /// <summary>
        /// 读取无符号长整型字节
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [Obsolete(nameof(GetSBytes))]
        public static sbyte[] ReadSBytes(this UInt64 number)
        {
            UInt32 num1 = (UInt32)(number >> 32);
            UInt32 num2 = (UInt32)number;
            return new sbyte[8]
            {
                (sbyte)(num1>>24),
                (sbyte)(num1>>16),
                (sbyte)(num1>>8),
                (sbyte)num1,
                (sbyte)(num2>>24),
                (sbyte)(num2>>16),
                (sbyte)(num2>>8),
                (sbyte)num2,
            };
        }
        #endregion 读取字节
        #region // 转换类型
        /// <summary>
        /// 转换类型
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<TOut> CastAs<TIn, TOut>(this IEnumerable<TIn> list)
            where TIn : class
            where TOut : class
        {
            foreach (var item in list)
            {
                yield return item as TOut;
            }
        }
        /// <summary>
        /// 转换类型
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="list"></param>
        /// <param name="GetValue"></param>
        /// <returns></returns>
        public static IEnumerable<TOut> CastAs<TIn, TOut>(this IEnumerable<TIn> list, Func<TIn, TOut> GetValue)
        {
            foreach (var item in list)
            {
                yield return GetValue(item);
            }
        }
        /// <summary>
        /// 强制转换(单字节转换)
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<Char> CastTo(this IEnumerable<Byte> list)
        {
            foreach (var item in list)
            {
                yield return (char)item;
            }
        }
        /// <summary>
        /// 强制转换(双字节转换)
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<Char> CastTo2(this IEnumerable<Byte> list)
        {
            var array = list.ToArray();
            if (array.Length % 2 == 1)
            {
                var arrList = list.ToList();
                arrList.Insert(0, (byte)0);
                array = arrList.ToArray();
            }
            for (int i = 1; i < array.Length; i++)
            {
                if (i % 2 == 0)
                {
                    yield return (char)(array[i - 1] * 256 + array[i]);
                }
            }
        }
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<Byte> CastTo(this IEnumerable<Char> list)
        {
            foreach (var item in list)
            {
                yield return (byte)item;
            }
        }
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<Byte> CastTo2(this IEnumerable<Char> list)
        {
            foreach (var item in list)
            {
                yield return (byte)(item >> 8);
                yield return (byte)item;
            }
        }
        #endregion 转换类型
        #region // 16进制
        /// <summary>
        /// 读十六进制数
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static String ReadHex(this Int32 val)
        {
            return val.ToString("X8");
        }
        /// <summary>
        /// 读十六进制数
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static String ReadHex(this UInt32 val)
        {
            return val.ToString("X8");
        }
        /// <summary>
        /// 读十六进制数
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static String ReadHex(this Int16 val)
        {
            return val.ToString("X4");
        }
        /// <summary>
        /// 读十六进制数
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static String ReadHex(this UInt16 val)
        {
            return val.ToString("X4");
        }
        /// <summary>
        /// 读十六进制数
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static String ReadHex(this Int64 val)
        {
            return val.ToString("X16");
        }
        /// <summary>
        /// 读十六进制数
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static String ReadHex(this UInt64 val)
        {
            return val.ToString("X16");
        }
        #endregion // 16进制
        /// <summary>
        /// 半进位1.2=>1.5或1.8=>2.0
        /// </summary>
        /// <param name="value"></param>
        /// <param name="digit"></param>
        /// <returns></returns>
        public static double GetHalfBitDouble(this double value, int digit)
        {
            return Cobber.CobberCaller.GetDouble((Math.Ceiling(value * Math.Pow(10, digit - 1) * 2) / 2.0 * Math.Pow(10, -digit + 1)), digit);
        }
        /// <summary>
        /// 判断版本范围,不合适直接抛异常
        /// </summary>
        /// <param name="version">在【1949-10-01 00:00:00】至【9999-12-31 23:59:59】的数字之间</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static long GetThrowVersionRange(this long version) => GetVersionRange(version);
        /// <summary>
        /// 判断版本范围,不合适根据配置抛异常
        /// </summary>
        /// <param name="version">在【1949-10-01 00:00:00】至【9999-12-31 23:59:59】的数字之间</param>
        /// <param name="isFixed">是否修正成当天版本【yyyy-MM-dd 00:00:00】</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static long GetVersionRange(this long version, bool isFixed = false)
        {
            if (version >= 19491001000000L && version <= 99991231235959L) { return version; }
            return isFixed ? Int64.Parse(DateTime.Now.ToString("yyyyMMdd") + "000000")
            : throw new ArgumentOutOfRangeException("迁移版本号【Version】必须在【1949-10-01 00:00:00】至【9999-12-31 23:59:59】的数字之间");
        }
        private static Random _random = new Random();
        /// <summary>
        /// 获取1024随机数的之后的随机数类
        /// </summary>
        /// <returns></returns>
        public static Random GetRandom() => GetRandom(1024);
        /// <summary>
        /// 获取一个种子随机数的之后的随机数类
        /// </summary>
        /// <returns></returns>
        public static Random GetRandom(this int seed)
        {
            _ = _random.Next(seed);
            return _random;
        }
        /// <summary>
        /// 全局无指定随机对象
        /// </summary>
        public static Random GRandom { get; } = GetRandom();
        /// <summary>
        /// 获取一个指定种子的随机数
        /// </summary>
        /// <returns></returns>
        public static int GetRandomInt32() => GRandom.Next(_random.Next());
        /// <summary>
        /// 获取一个指定种子的随机数
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int GetRandomInt32(this int max) => GRandom.Next(max);
        /// <summary>
        /// 获取一个指定种子的随机数
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int GetRandomInt32(this int min, int max) => GRandom.Next(min, max);
        /// <summary>
        /// 获取一个指定种子的随机数
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int GetRandomInt32(this int seed, int min, int max) => GetRandom(seed).Next(min, max);
        /// <summary>
        /// 获取一个指定种子的最大值随机数
        /// </summary>
        /// <returns></returns>
        public static double GetRandomDouble() => GRandom.NextDouble();
        /// <summary>
        /// 获取一个指定种子的最大值随机数
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static double GetRandomDouble(this int seed) => GetRandom(seed).NextDouble();
        #endregion 数字调用 Number

        #region // 字符串调用 String
        /// <summary>
        /// 转换成蛇形
        /// ATest=>a_test
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string ToSnakeCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) { return str; }
            List<char> list = new List<char>(str.Length * 2);
            list.Add(char.ToLower(str[0]));
            char curr;
            for (int i = 1; i < str.Length; i++)
            {
                curr = str[i];
                if (Char.IsUpper(curr))
                {
                    list.Add('_');
                    list.Add(Char.ToLower(curr));
                }
                else
                {
                    list.Add(curr);
                }
            }
            return new String(list.ToArray());
        }
        /// <summary>
        /// 驼峰转换成蛇形
        /// aTest=>a_test
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string CamelToSnakeCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) { return str; }
            List<char> list = new List<char>(str.Length * 2);
            char curr;
            for (int i = 0; i < str.Length; i++)
            {
                curr = str[i];
                if (Char.IsUpper(curr))
                {
                    list.Add('_');
                    list.Add(Char.ToLower(curr));
                }
                else
                {
                    list.Add(curr);
                }
            }
            return new String(list.ToArray());
        }
        /// <summary>
        /// 帕斯卡(大驼峰)转换成蛇形
        /// ATest=>a_test
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string PascalToSnakeCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) { return str; }
            List<char> list = new List<char>(str.Length * 2);
            list.Add(char.ToLower(str[0]));
            char curr;
            for (int i = 1; i < str.Length; i++)
            {
                curr = str[i];
                if (char.IsUpper(curr))
                {
                    list.Add('_');
                    list.Add(char.ToLower(curr));
                }
                else
                {
                    list.Add(curr);
                }
            }
            return new String(list.ToArray());
        }
        /// <summary>
        /// 帕斯卡(大驼峰)转换成驼峰
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string PascalToCamelCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) { return str; }
            char curr = Char.ToLower(str[0]);
            StringBuilder sb = new StringBuilder().Append(curr);
            for (int i = 0; i < str.Length; i++)
            {
                sb.Append(str[i]);
            }
            return sb.ToString();
        }
        /// <summary>
        /// 驼峰转换成帕斯卡(大驼峰)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string CamelToPascalCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) { return str; }
            char curr = Char.ToUpper(str[0]);
            StringBuilder sb = new StringBuilder().Append(curr);
            for (int i = 0; i < str.Length; i++)
            {
                sb.Append(str[i]);
            }
            return sb.ToString();
        }
        /// <summary>
        /// 蛇形转换成驼峰
        /// a_test=>aTest
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string SnakeToCamelCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) { return str; }
            List<char> list = new List<char>(str.Length);
            char curr;
            for (int i = 0; i < str.Length; i++)
            {
                curr = str[i];
                if (curr == '_')
                {
                    i++;
                    if (i >= str.Length) { break; }
                    curr = Char.ToUpper(str[i]);
                }
                list.Add(curr);
            }
            return new String(list.ToArray());
        }
        /// <summary>
        /// 蛇形转换成帕斯卡(大驼峰)
        /// a_test=>ATest
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string SnakeToPascalCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) { return str; }
            List<char> list = new List<char>(str.Length);
            char curr = str[0];
            int start;
            if (curr == '_')
            {
                if (str.Length == 1) { return str; }
                start = 2;
                list.Add(Char.ToUpper(curr));
            }
            else
            {
                start = 1;
                list.Add(char.ToUpper(curr));
            }
            for (int i = start; i < str.Length; i++)
            {
                curr = str[i];
                if (curr == '_')
                {
                    i++;
                    if (i >= str.Length) { break; }
                    curr = Char.ToUpper(str[i]);
                }
                list.Add(curr);
            }
            return new String(list.ToArray());
        }
        /// <summary>
        /// 相等忽略大小写
        /// </summary>
        /// <param name="src"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(this string src, string tag)
        {
            if (src == null) { return src == tag; }
            return src.Equals(tag, StringComparison.OrdinalIgnoreCase);
        }
        /// <summary>
        /// 相等忽略大小写
        /// </summary>
        /// <param name="src"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool EqualIgnoreCase(this string src, string tag)
        {
            if (src == null) { return src == tag; }
            return src.Equals(tag, StringComparison.OrdinalIgnoreCase);
        }
        /// <summary>
        /// 相等忽略大小写
        /// 无null判断
        /// </summary>
        /// <param name="src"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase2(this string src, string tag)
        {
            return src.Equals(tag, StringComparison.OrdinalIgnoreCase);
        }
        /// <summary>
        /// 相等忽略大小写
        /// 无null判断
        /// </summary>
        /// <param name="src"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool EqualIgnoreCase2(this string src, string tag)
        {
            return src.Equals(tag, StringComparison.OrdinalIgnoreCase);
        }
        /// <summary>
        /// 获取文本的指定长度
        /// </summary>
        /// <param name="text"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string TakeString(this string text, int len)
        {
            if (text == null || len <= 0) { return text; }
            if (text.Length > len) { return text.Substring(0, len); }
            return text;
        }
        #region // Base64
        /// <summary>
        /// 获取Base64编码
        /// </summary>
        /// <returns></returns>
        public static String GetBase64(this string json, Encoding encoding)
        {
            return Convert.ToBase64String(encoding.GetBytes(json));
        }
        /// <summary>
        /// 获取Base64编码
        /// </summary>
        /// <see cref="SerialCaller.GetModelBytes{T}(T)"/>
        /// <returns></returns>
        public static String GetBase64<T>(this T model)
        {
            return Convert.ToBase64String(model.GetModelBytes());
        }
        /// <summary>
        /// 获取Base64解码
        /// </summary>
        /// <see cref="SerialCaller.GetBytesModel{T}(byte[])"/>
        /// <returns></returns>
        public static T GetDebase64<T>(this string base64)
        {
            return Convert.FromBase64String(base64).GetBytesModel<T>();
        }
        /// <summary>
        /// 获取Base64编码
        /// </summary>
        /// <returns></returns>
        public static String GetBase64(this string json) => GetBase64(json, Encoding.UTF8);
        /// <summary>
        /// 获取Base64解码
        /// </summary>
        /// <returns></returns>
        public static String GetDebase64(this string base64, Encoding encoding)
        {
            return encoding.GetString(Convert.FromBase64String(base64));
        }
        /// <summary>
        /// 获取Base64解码
        /// </summary>
        /// <returns></returns>
        public static String GetDebase64(this string base64) => GetDebase64(base64, Encoding.UTF8);
        /// <summary>
        /// 将对象转换为byte数组
        /// 注意:此方法之前未使用Json进行转换，只支持Struct
        /// Marshal.StructureToPtr(model, Marshal.UnsafeAddrOfPinnedArrayElement(new byte[Marshal.SizeOf(model)], 0), true)
        /// </summary>
        /// <param name="model">被转换对象</param>
        /// <returns>转换后byte数组</returns>
        public static byte[] GetModelBytes<T>(this T model) => model.GetJsonString().GetBytes();
        /// <summary>
        /// 将byte数组转换成对象
        /// 注意:此方法之前未使用Json进行转换，只支持Struct
        /// Marshal.PtrToStructure(Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0), typeof(T))
        /// </summary>
        /// <param name="bytes">被转换byte数组</param>
        /// <returns>转换完成后的对象</returns>
        public static T GetBytesModel<T>(this byte[] bytes) => bytes.GetString().GetJsonObject<T>();
        /// <summary>
        /// 将byte数组转换成对象
        /// 注意:此方法之前未使用Json进行转换，只支持Struct
        /// Marshal.PtrToStructure(Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0), type);
        /// </summary>
        /// <param name="bytes">被转换byte数组</param>
        /// <param name="type">转换成的类名</param>
        /// <returns>转换完成后的对象</returns>
        public static object GetBytesModel(this byte[] bytes, Type type) => bytes.GetString().GetJsonObject(type);
        #endregion
        #region // 压缩
        /// <summary>
        /// 压缩字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Compress(this string str) => Compress(str, Encoding.UTF8);
        /// <summary>
        /// 根据编码压缩字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Compress(this string str, Encoding encoding)
        {
            var bytes = encoding.GetBytes(str);

            var ms = new MemoryStream();
            var zip = new GZipStream(ms, CompressionMode.Compress, true);
            zip.Write(bytes, 0, bytes.Length);
            zip.Close();
            var buffer = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(buffer, 0, buffer.Length);
            ms.Close();

            return Convert.ToBase64String(buffer);
        }
        /// <summary>
        /// 解压字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Decompress(this string str) => Decompress(str, DefaultEncoding);
        /// <summary>
        /// 根据编码解压字符串
        /// </summary>
        /// <returns></returns>
        public static string Decompress(this string str, Encoding encoding)
        {
            var bytes = Convert.FromBase64String(str);

            var ms = new MemoryStream(bytes);
            var zip = new GZipStream(ms, CompressionMode.Decompress, true);
            var msreader = new MemoryStream();
            var buffer = new byte[0x1000];
            while (true)
            {
                var reader = zip.Read(buffer, 0, buffer.Length);
                if (reader <= 0)
                {
                    break;
                }
                msreader.Write(buffer, 0, reader);
            }
            zip.Close();
            ms.Close();
            msreader.Position = 0;
            buffer = msreader.ToArray();
            msreader.Close();

            return encoding.GetString(buffer);
        }
        #endregion
        #endregion 字符串调用 String

        #region // 单元测试调用 Test
        /// <summary>
        /// 测试方法
        /// </summary>
        public static void TestAction(string consoleFmt, Action action, int times = 10000)
        {
            action.Invoke();
            var now = DateTime.Now;
            for (int i = 0; i < times; i++)
            {
                action.Invoke();
            }
            Console.WriteLine(consoleFmt, DateTime.Now - now, times);
        }
        /// <summary>
        /// 测试方法
        /// </summary>
        public static void TestParallelAction(string consoleFmt, Action action, int times = 10000, int degree = 5)
        {
            action.Invoke();
            var now = DateTime.Now;
            Parallel.For(0, times, new ParallelOptions { MaxDegreeOfParallelism = degree }, (i) => action.Invoke());
            Console.WriteLine(consoleFmt, DateTime.Now - now, times, degree);
        }
        /// <summary>
        /// 测试方法
        /// </summary>
        public static void TestFunc<T>(string consoleFmt, Func<T> action, int times = 10000)
        {
            action.Invoke();
            var now = DateTime.Now;
            for (int i = 0; i < times; i++)
            {
                action.Invoke();
            }
            Console.WriteLine(consoleFmt, DateTime.Now - now);
        }
        /// <summary>
        /// 测试方法
        /// </summary>
        public static async Task TestActionAsync(string consoleFmt, Action action, int times = 10000)
        {
            await Task.Factory.StartNew(() => TestAction(consoleFmt, action, times));
        }
        #endregion 单元测试调用 Test

        #region // Windows内容操作
        /// <summary>
        /// C:\Windows
        /// </summary>
        public static String WindowDir => Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.Windows));
        /// <summary>
        /// 获取特殊目录全路径
        /// </summary>
        /// <param name="special"></param>
        /// <returns></returns>
        public static String GetFullPath(this Environment.SpecialFolder special)
        {
            return Path.GetFullPath(Environment.GetFolderPath(special));
        }
        /// <summary>
        /// 获取特殊目录全路径
        /// </summary>
        /// <param name="special"></param>
        /// <returns></returns>
        public static String GetFullPath(this WindowSpecialFolder special)
        {
            return Path.GetFullPath(Environment.GetFolderPath((Environment.SpecialFolder)special));
        }
        /// <summary>
        /// 执行命令行
        /// </summary>
        /// <param name="exeFile"></param>
        /// <param name="startDir"></param>
        /// <param name="args"></param>
        public static IAlertMsg ExecHidden(string exeFile, string startDir, string args)
        {
            var result = new AlertMsg(true, "");
            var p = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    FileName = exeFile,
                    Arguments = args,
                    WorkingDirectory = Path.GetFullPath(startDir),
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                }
            };
            p.OutputDataReceived += (sender, e) =>
            {
                if (e.Data == null) { return; }
                result.AddMsg(e.Data);
            };
            p.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data == null) { return; }
                result.AddMsg(e.Data);
            };
            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            p.WaitForExit();

            return result;
        }
        /// <summary>
        /// 执行命令行
        /// </summary>
        public static IAlertMsg ExecHidden(string command) => ExecHidden("cmd.exe", WindowDir, $" /c {command}");
        /// <summary>
        /// 执行命令行
        /// </summary>
        public static IAlertMsg ExecHidden(string exeFile, string command) => ExecHidden(exeFile, WindowDir, command);
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static IAlertMsg NetStart(string serviceName) => ExecHidden("net", WindowDir, $" start {serviceName}");
        /// <summary>
        /// 关闭服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static IAlertMsg NetStop(string serviceName) => ExecHidden("net", WindowDir, $" stop {serviceName}");
        /// <summary>
        /// 资源管理器打开目录(不重复打开)
        /// </summary>
        /// <param name="dir"></param>
        public static void StartExplorer(this DirectoryInfo dir) => ExecHidden("cmd", dir.FullName, $" /c start \"\" \"{dir.FullName}\"");
        /// <summary>
        /// 启动链接
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static IAlertMsg StartUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) { return AlertMsg.OperSuccess; }
            url = url.Trim().Replace("&", "^&");
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            return AlertMsg.OperSuccess;
        }
        /// <summary>
        /// 启动链接
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static IAlertMsg TryStartUrl(string url)
        {
            try { return StartUrl(url); }
            catch { return AlertMsg.OperError; }
        }
        #region // 快捷方式调用者
        /// <summary>
        /// 创建快捷方式
        /// </summary>
        /// <param name="directory">快捷方式所处的文件夹</param>
        /// <param name="shortcutName">快捷方式名称</param>
        /// <param name="targetPath">目标路径</param>
        /// <param name="description">描述</param>
        /// <param name="iconLocation">图标路径，格式为"可执行文件或DLL路径, 图标编号"，例如System.Environment.SystemDirectory + "\\" + "shell32.dll, 165"</param>
        /// <param name="args">参数</param>
        /// <remarks></remarks>
        public static void CreateShortcut(string directory, string shortcutName, string targetPath, string description = null, string iconLocation = null, string args = null)
        {
            using var shellLink = CreateShortcut2(Path.Combine(directory, shortcutName), targetPath, description, iconLocation, args, null);
        }
        /// <summary>
        /// 创建桌面快捷方式
        /// </summary>
        /// <param name="shortcutName">快捷方式名称,不包括扩展名</param>
        /// <param name="targetPath">目标路径</param>
        /// <param name="description">描述</param>
        /// <param name="iconLocation">图标路径，格式为"可执行文件或DLL路径, 图标编号"，例如System.Environment.SystemDirectory + "\\" + "shell32.dll, 165"</param>
        /// <param name="args">参数</param>
        /// <remarks>参数</remarks>
        public static void CreateShortcutDesktop(string shortcutName, string targetPath, string description = null, string iconLocation = null, string args = null)
        {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory); //获取桌面文件夹路径
            using var shellLink = CreateShortcut2(Path.Combine(desktop, shortcutName), targetPath, description, iconLocation, args, null);
        }
        /// <summary>
        /// 创建桌面快捷方式
        /// </summary>
        /// <param name="shortcutName">快捷方式名称,不包括扩展名</param>
        /// <param name="targetPath">目标路径</param>
        /// <param name="description">描述</param>
        /// <param name="icon">图标路径，格式为"可执行文件或DLL路径, 图标编号"，例如System.Environment.SystemDirectory + "\\" + "shell32.dll, 165"</param>
        /// <param name="args">参数</param>
        /// <remarks>参数</remarks>
        public static void CreateShortcutStartup(string shortcutName, string targetPath, string description = null, string icon = null, string args = null)
        {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Startup); //获取启动文件夹路径
            using var shellLink = CreateShortcut2(Path.Combine(desktop, shortcutName), targetPath, description, icon, args, null);
        }
        /// <summary>
        /// 创建桌面快捷方式
        /// </summary>
        /// <param name="model"></param>
        /// <remarks>参数</remarks>
        public static void Create(this WindowShortcutInfoModel model)
        {
            using var shellLink = CreateShortcut2(Path.Combine(model.SavePath, model.SaveName), model.SourceName, model.Description, model.Icon, model.Args, model.WorkDir);
        }
        /// <summary>
        /// 创建快捷方式
        /// </summary>
        /// <param name="shortcutPath"></param>
        /// <param name="targetPath"></param>
        /// <param name="description"></param>
        /// <param name="icon"></param>
        /// <param name="workDir"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static ShellLink CreateShortcut2(string shortcutPath, string targetPath, string description = null, string icon = null, string args = null, string workDir = null)
        {
            shortcutPath = ShellLink.GetFullPath(shortcutPath);
            targetPath = ShellLink.GetFullPath(targetPath);
            if (!System.IO.Path.HasExtension(shortcutPath)) { shortcutPath += ShellLink.Extension; }
            // if (File.Exists(shortcutPath)) { File.Delete(shortcutPath); }
            var link = new ShellLink(shortcutPath);
            link.Path = targetPath;
            link.WorkingDirectory = workDir == null ? System.IO.Path.GetDirectoryName(targetPath) : Path.GetFullPath(workDir);
            link.IconLocation = GetIconLocation(icon);
            link.Description = description ?? Path.GetFileNameWithoutExtension(shortcutPath);
            link.Arguments = args ?? String.Empty;
            link.Save(shortcutPath);
            return link;
            static Tuble2StringInt GetIconLocation(string iconLocation)
            {
                if (string.IsNullOrWhiteSpace(iconLocation)) { return null; }
                try
                {
                    if (File.Exists(Path.GetFullPath(iconLocation)))
                    {
                        return new Tuble2StringInt(iconLocation, 0);
                    }
                }
                catch { }
                var iconTag = iconLocation.LastIndexOf(',');
                int index = 0;
                var fileName = iconLocation;
                if (iconTag > 0)
                {
                    index = iconLocation.Substring(iconTag + 1).ToPInt32();
                    fileName = iconLocation.Substring(0, iconTag);
                }
                return new Tuble2StringInt(fileName, index);
            }
        }
        /// <summary>
        /// 是快捷方式路径
        /// </summary>
        /// <param name="shortcutPath"></param>
        /// <returns></returns>
        public static bool IsShortcutFile(string shortcutPath)
        {
            shortcutPath = ShellLink.GetFullPath(shortcutPath);
            if (File.Exists(shortcutPath))
            {
                try
                {
                    using var shellLink = new ShellLink(shortcutPath);
                    return true;
                }
                catch { }
            }
            return false;
        }
        /// <summary>
        /// 是快捷方式指向路径
        /// </summary>
        /// <param name="shortcutPath"></param>
        /// <param name="targetPath"></param>
        /// <returns></returns>
        public static bool IsShortcutPointTo(string shortcutPath, string targetPath)
        {
            try
            {
                using var shellLink = new ShellLink(shortcutPath);
                return shellLink.IsPointTo(targetPath);
            }
            catch { }
            return false;
        }
        /// <summary>
        /// 创建任务
        /// </summary>
        /// <param name="file"></param>
        /// <param name="name"></param>
        /// <param name="dirs"></param>
        public static void CreateStartTask(string file, string name, params string[] dirs)
        {
            //新建任务
            var scheduler = new TaskSchedulerClass();
            //连接
            scheduler.Connect(null, null, null, null);
            //获取创建任务的目录
            var folder = scheduler.GetFolder("\\");
            if(dirs != null && dirs.Length > 0)
            {
                foreach (var dir in dirs)
                {
                    folder = TestTry.Try(folder.CreateFolder, dir, (object)null, (ex) => folder.GetFolder(dir));
                }
            }
            var tesks = folder.GetTasks(0);
            IRegisteredTask tesk = null;
            foreach (IRegisteredTask item in tesks)
            {
                if (item.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) { tesk = item; break; }
            }
            ITaskDefinition task;
            if (tesk == null)
            {
                //设置参数
                task = scheduler.NewTask(0);
            }
            else
            {
                task = tesk.Definition;
                var hasTrigger = false;
                foreach (ITrigger item in task.Triggers)
                {
                    if (item.Type == _TASK_TRIGGER_TYPE2.TASK_TRIGGER_LOGON)
                    {
                        hasTrigger = true;
                        break;
                    }
                }
                if (hasTrigger) { return; }
            }
            //task.RegistrationInfo.Author = author;//创建者
            //task.RegistrationInfo.Description = desc;//描述
            task.Principal.RunLevel = _TASK_RUNLEVEL.TASK_RUNLEVEL_HIGHEST;
            //设置触发机制（此处是 登陆后）
            task.Triggers.Create(_TASK_TRIGGER_TYPE2.TASK_TRIGGER_LOGON);
            //设置动作（此处为运行exe程序）
            var action = (IExecAction)task.Actions.Create(_TASK_ACTION_TYPE.TASK_ACTION_EXEC);
            action.Path = file;//设置文件目录
            task.Settings.ExecutionTimeLimit = "PT0S"; //运行任务时间超时停止任务吗? PTOS 不开启超时
            task.Settings.DisallowStartIfOnBatteries = false;//只有在交流电源下才执行
            task.Settings.RunOnlyIfIdle = false;//仅当计算机空闲下才执行

            var regTask =
                folder.RegisterTaskDefinition(name, task,//此处需要设置任务的名称（name）
                (int)_TASK_CREATION.TASK_CREATE, null, //user
                null, // password
                _TASK_LOGON_TYPE.TASK_LOGON_INTERACTIVE_TOKEN,
                "");
            _ = regTask.Run(null);
        }
        #endregion 快捷方式调用者
        #endregion Windows内容操作
    }
}
