using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    }
}
namespace System.Data.Extter
{
    /// <summary>
    /// 集合和数组操作
    /// </summary>
    public static class ListArrayCaller
    {
    }
}
