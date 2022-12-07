using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Extter;
using System.Linq;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// 集合及数组调用
    /// </summary>
    public static partial class CobberCaller
    {
        #region // 枚举调用
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
            return (T)(object)e;
        }
        #endregion
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
    }
}
namespace System.Data.Extter
{
    /// <summary>
    /// 集合和数组操作
    /// </summary>
    public static partial class ExtterCaller
    {
        #region // 枚举调用
        /// <summary>
        /// 有标记
        /// </summary>
        /// <param name="value"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool HasFlag(this int value, int tag)
        {
            return (value & tag) != 0;
        }
        /// <summary>
        /// 有标记
        /// </summary>
        /// <param name="value"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool HasFlag<T>(this T value, int tag) where T : struct, Enum
        {
            return value.HasFlag((T)(object)tag);
        }
        #endregion
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
            {
                tarr[i] = defVal;
            }
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
            {
                tarr[list.Length + i] = defVal;
            }
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
    }
}
