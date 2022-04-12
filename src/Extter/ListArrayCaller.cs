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
                if (condition(item))
                {
                    return item;
                }
            }
            return defVal;
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
                if (condition(item))
                {
                    return item;
                }
            }
            return list.FirstOrDefault();
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
    }
}
namespace System.Data.Extter
{
    /// <summary>
    /// 集合和数组操作
    /// </summary>
    public static class ListArrayCaller
    {
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
    }
}
