using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Extter
{
    /// <summary>
    /// 缓存计数器(线程安全)
    /// </summary>
    [Obsolete("替代方案:CacheCountModel")]
    public sealed class CacheConcurrentCountModel
    {
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="kvs"></param>
        public void Set<T>(Dictionary<string, int> kvs) => CacheConcurrentCountModel<T>.Set(kvs);
        /// <summary>
        /// 加载
        /// </summary>
        public void Set<T>(string key, int value) => CacheConcurrentCountModel<T>.Set(key, value);
        /// <summary>
        /// 获取计数(值+1处理)
        /// </summary>
        /// <param name="key"></param>
        public int Count<T>(string key) => CacheConcurrentCountModel<T>.Count(key);
        /// <summary>
        /// 获取当前(不改变值)
        /// </summary>
        /// <returns></returns>
        public int Get<T>(string key) => CacheConcurrentCountModel<T>.Get(key);
        /// <summary>
        /// 检查键并返回
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public void Check<T>(string key) => CacheConcurrentCountModel<T>.Check(key);
        /// <summary>
        /// 重置键并返回
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public void Reset<T>(string key) => CacheConcurrentCountModel<T>.Reset(key);
        /// <summary>
        /// 重置键
        /// </summary>
        /// <returns></returns>
        public void Reset<T>() => CacheConcurrentCountModel<T>.Reset();
        /// <summary>
        /// 清空
        /// </summary>
        public void Clear<T>() => CacheConcurrentCountModel<T>.Clear();
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key"></param>
        public void Remove<T>(string key) => CacheConcurrentCountModel<T>.Remove(key);
    }
    /// <summary>
    /// 缓存计数器(泛型/线程安全)
    /// </summary>
    [Obsolete("替代方案:CacheCountModel<T>")]
    public class CacheConcurrentCountModel<T>
    {
        private readonly static ConcurrentDictionary<string, int> counterDic = new ConcurrentDictionary<string, int>();
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="kvs"></param>
        public static void Set(Dictionary<string, int> kvs)
        {
            foreach (var item in kvs)
            {
                counterDic[item.Key] = item.Value;
            }
        }
        /// <summary>
        /// 加载
        /// </summary>
        public static void Set(string key, int value)
        {
            counterDic[key] = value;
        }
        /// <summary>
        /// 获取计数(值+1处理)
        /// </summary>
        /// <param name="key"></param>
        public static int Count(string key)
        {
            return counterDic.AddOrUpdate(key, 1, (k, v) => ++v);
        }
        /// <summary>
        /// 获取当前(不改变值)
        /// </summary>
        /// <returns></returns>
        public static int Get(string key)
        {
            return counterDic.GetOrAdd(key, 0);
        }
        /// <summary>
        /// 检查键并返回
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static void Check(string key)
        {
            counterDic.GetOrAdd(key, 0);
        }
        /// <summary>
        /// 重置键并返回
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static void Reset(string key)
        {
            counterDic[key] = 0;
        }
        /// <summary>
        /// 重置键
        /// </summary>
        /// <returns></returns>
        public static void Reset()
        {
            counterDic.Keys.ToList().ForEach(key => counterDic[key] = 0);
        }
        /// <summary>
        /// 清空
        /// </summary>
        public static void Clear()
        {
            counterDic.Clear();
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            counterDic.TryRemove(key, out _);
        }
        /// <summary>
        /// 构造
        /// </summary>
        protected CacheConcurrentCountModel() { }
    }
    /// <summary>
    /// 缓存锁器(泛型/线程安全)
    /// </summary>
    [Obsolete("替代方案:CacheLockModel")]
    public sealed class CacheConcurrentLockModel
    {
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="kvs"></param>
        public void Set<T>(Dictionary<string, object> kvs) => CacheConcurrentLockModel<T>.Set(kvs);
        /// <summary>
        /// 加载
        /// </summary>
        public void Set<T>(string key, object value) => CacheConcurrentLockModel<T>.Set(key, value);
        /// <summary>
        /// 获取当前(不改变值)
        /// </summary>
        /// <returns></returns>
        public object Get<T>(string key) => CacheConcurrentLockModel<T>.Get(key);
        /// <summary>
        /// 检查键并返回
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Check<T>(string key) => CacheConcurrentLockModel<T>.Check(key);
        /// <summary>
        /// 重置键并返回
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Reset<T>(string key) => CacheConcurrentLockModel<T>.Reset(key);
        /// <summary>
        /// 重置
        /// </summary>
        /// <returns></returns>
        public void Reset<T>() => CacheConcurrentLockModel<T>.Reset();
        /// <summary>
        /// 移除
        /// </summary>
        /// <returns></returns>
        public void Clear<T>() => CacheConcurrentLockModel<T>.Clear();
        /// <summary>
        /// 移除
        /// </summary>
        /// <returns></returns>
        public void Remove<T>(string key) => CacheConcurrentLockModel<T>.Remove(key);
    }
    /// <summary>
    /// 缓存锁器(泛型/线程安全)
    /// </summary>
    [Obsolete("替代方案:CacheLockModel<T>")]
    public class CacheConcurrentLockModel<T>
    {
        private readonly static ConcurrentDictionary<string, object> counterDic = new ConcurrentDictionary<string, object>();
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="kvs"></param>
        public static void Set(Dictionary<string, object> kvs)
        {
            foreach (var item in kvs)
            {
                counterDic[item.Key] = item.Value;
            }
        }
        /// <summary>
        /// 加载
        /// </summary>
        public static void Set(string key, object value)
        {
            counterDic[key] = value;
        }
        /// <summary>
        /// 获取当前(不改变值)
        /// </summary>
        /// <returns></returns>
        public static object Get(string key)
        {
            return counterDic.GetOrAdd(key, (k) => new object());
        }
        /// <summary>
        /// 检查键并返回
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object Check(string key)
        {
            return counterDic.GetOrAdd(key, (k) => new object());
        }
        /// <summary>
        /// 重置键并返回
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object Reset(string key)
        {
            return counterDic[key] = new object();
        }
        /// <summary>
        /// 重置
        /// </summary>
        /// <returns></returns>
        public static void Reset()
        {
            counterDic.Keys.ToList().ForEach(k => counterDic[k] = new object());
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <returns></returns>
        public static void Clear()
        {
            counterDic.Clear();
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <returns></returns>
        public static void Remove(string key)
        {
            counterDic.TryRemove(key, out _);
        }
        /// <summary>
        /// 构造
        /// </summary>
        protected CacheConcurrentLockModel() { }
    }
}
