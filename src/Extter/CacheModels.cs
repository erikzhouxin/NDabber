using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Data.Dabber;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Extter
{
    /// <summary>
    /// 缓存计数器(线程安全)
    /// </summary>
    public sealed class CacheCountModel
    {
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="kvs"></param>
        public void Set<T>(Dictionary<string, int> kvs) => CacheCountModel<T>.Set(kvs);
        /// <summary>
        /// 加载
        /// </summary>
        public void Set<T>(string key, int value) => CacheCountModel<T>.Set(key, value);
        /// <summary>
        /// 获取计数(值+1处理)
        /// </summary>
        /// <param name="key"></param>
        public int Count<T>(string key) => CacheCountModel<T>.Count(key);
        /// <summary>
        /// 获取当前(不改变值)
        /// </summary>
        /// <returns></returns>
        public int Get<T>(string key) => CacheCountModel<T>.Get(key);
        /// <summary>
        /// 检查键并返回
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public void Check<T>(string key) => CacheCountModel<T>.Check(key);
        /// <summary>
        /// 重置键并返回
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public void Reset<T>(string key) => CacheCountModel<T>.Reset(key);
        /// <summary>
        /// 重置键
        /// </summary>
        /// <returns></returns>
        public void Reset<T>() => CacheCountModel<T>.Reset();
        /// <summary>
        /// 清空
        /// </summary>
        public void Clear<T>() => CacheCountModel<T>.Clear();
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key"></param>
        public void Remove<T>(string key) => CacheCountModel<T>.Remove(key);
    }
    /// <summary>
    /// 缓存计数器(线程安全)
    /// </summary>
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
    public class CacheCountModel<T>
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
        protected CacheCountModel() { }
    }
    /// <summary>
    /// 缓存计数器(泛型/线程安全)
    /// </summary>
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
    public sealed class CacheLockModel
    {
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="kvs"></param>
        public void Set<T>(Dictionary<string, object> kvs) => CacheLockModel<T>.Set(kvs);
        /// <summary>
        /// 加载
        /// </summary>
        public void Set<T>(string key, object value) => CacheLockModel<T>.Set(key, value);
        /// <summary>
        /// 获取当前(不改变值)
        /// </summary>
        /// <returns></returns>
        public object Get<T>(string key) => CacheLockModel<T>.Get(key);
        /// <summary>
        /// 检查键并返回
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Check<T>(string key) => CacheLockModel<T>.Check(key);
        /// <summary>
        /// 重置键并返回
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Reset<T>(string key) => CacheLockModel<T>.Reset(key);
        /// <summary>
        /// 重置
        /// </summary>
        /// <returns></returns>
        public void Reset<T>() => CacheLockModel<T>.Reset();
        /// <summary>
        /// 移除
        /// </summary>
        /// <returns></returns>
        public void Clear<T>() => CacheLockModel<T>.Clear();
        /// <summary>
        /// 移除
        /// </summary>
        /// <returns></returns>
        public void Remove<T>(string key) => CacheLockModel<T>.Remove(key);
    }
    /// <summary>
    /// 缓存锁器(泛型/线程安全)
    /// </summary>
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
    public class CacheLockModel<T>
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
        protected CacheLockModel() { }
    }
    /// <summary>
    /// 缓存锁器(泛型/线程安全)
    /// </summary>
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
    /// <summary>
    /// 缓存设置模型类
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public interface ICacheSettingModel<T1, T2>
    {
        /// <summary>
        /// 值内容
        /// </summary>
        T2 Value { get; }
        /// <summary>
        /// 获取内容
        /// </summary>
        /// <returns></returns>
        T2 Get();
        /// <summary>
        /// 获取内容
        /// </summary>
        /// <returns></returns>
        T2 GetDefault(Lazy<T2> defVal);
        /// <summary>
        /// 获取内容
        /// </summary>
        /// <returns></returns>
        T2 GetDefault(Func<T2> defVal);
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        ICacheSettingModel<T1, T2> Set(T2 model);
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        ICacheSettingModel<T1, T2> Save();
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        ICacheSettingModel<T1, T2> SaveSet(T2 model);
        /// <summary>
        /// 重置
        /// </summary>
        /// <returns></returns>
        ICacheSettingModel<T1, T2> Reset();
        /// <summary>
        /// 重新加载
        /// </summary>
        /// <returns></returns>
        ICacheSettingModel<T1, T2> Reload();
        /// <summary>
        /// 重置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ICacheSettingModel<T1, T2> Reset(T2 model);
    }
    /// <summary>
    /// 缓存设置模型类
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public interface ICacheSetDictModel<T1, T2>
    {
        /// <summary>
        /// 值内容
        /// </summary>
        Dictionary<string, T2> Value { get; }
        /// <summary>
        /// 获取值(不验证key)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        T2 Get(string key, T2 defVal = default(T2));
        /// <summary>
        /// 获取值(验证key)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        T2 TryGet(string key, T2 defVal = default(T2));
        /// <summary>
        /// 获取值(验证key)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        T2 GetOrAdd(string key, T2 defVal = default(T2));
        /// <summary>
        /// 获取值(不验证key)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        T2 Get(string key, Func<T2> defVal);
        /// <summary>
        /// 获取值(不验证key)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        T2 GetOrAdd(string key, Func<T2> defVal);
        /// <summary>
        /// 获取值(不验证key)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        T2 GetOrAdd(string key, Lazy<T2> defVal);
        /// <summary>
        /// 获取值(验证key)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        T2 TryGet(string key, Func<T2> defVal);
        /// <summary>
        /// 获取值(验证key)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        T2 TryGetOrAdd(string key, Func<T2> defVal);
        /// <summary>
        /// 获取值(验证key)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        T2 TryGetOrAdd(string key, Lazy<T2> defVal);
        /// <summary>
        /// 设置内容
        /// </summary>
        /// <param name="key"></param>
        /// <param name="model"></param>
        ICacheSetDictModel<T1, T2> Set(string key, T2 model);
        /// <summary>
        /// 重新加载
        /// </summary>
        /// <returns></returns>
        ICacheSetDictModel<T1, T2> Reload();
        /// <summary>
        /// 重置
        /// </summary>
        /// <returns></returns>
        ICacheSetDictModel<T1, T2> Reset();
        /// <summary>
        /// 重置内容
        /// </summary>
        /// <param name="key"></param>
        /// <param name="model"></param>
        ICacheSetDictModel<T1, T2> Reset(string key, T2 model = default(T2));
        /// <summary>
        /// 设置内容
        /// </summary>
        /// <param name="key"></param>
        /// <param name="model"></param>
        ICacheSetDictModel<T1, T2> TrySet(string key, T2 model);
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        ICacheSetDictModel<T1, T2> Save();
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        ICacheSetDictModel<T1, T2> SaveSet(string key, T2 model);
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        Task<ICacheSetDictModel<T1, T2>> SaveSetAsync(string key, T2 model);
        /// <summary>
        /// 清空内容
        /// </summary>
        /// <returns></returns>
        ICacheSetDictModel<T1, T2> Clear();
    }
    /// <summary>
    /// 页面缓存设置接口
    /// </summary>
    public interface ICacheSettingModel
    {
        #region // 模型内容
        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="defVal"></param>
        /// <returns></returns>
        TValue Get<TValue>(TValue defVal = default(TValue)) where TValue : class;
        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="defVal"></param>
        /// <returns></returns>
        TValue Get<TValue>(Func<TValue> defVal) where TValue : class;
        /// <summary>
        /// 保存对象
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        IAlertMsg<TValue> Save<TValue>() where TValue : class;
        /// <summary>
        /// 保存对象
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="defVal"></param>
        /// <returns></returns>
        IAlertMsg<TValue> Save<TValue>(TValue defVal) where TValue : class;
        /// <summary>
        /// 重置对象
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="defVal"></param>
        /// <returns></returns>
        IAlertMsg<TValue> Reset<TValue>(TValue defVal = default(TValue)) where TValue : class;
        #endregion 模型内容
        #region // 字典内容
        /// <summary>
        /// 获取字典
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        Dictionary<string, TValue> GetDic<TValue>() where TValue : class;
        /// <summary>
        /// 获取字典值
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        TValue GetDic<TValue>(string key, TValue value = null) where TValue : class;
        /// <summary>
        /// 获取字典值
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        TValue GetDic<TValue>(string key, Func<TValue> value) where TValue : class;
        /// <summary>
        /// 获取或添加字典值
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        TValue GetOrAdd<TValue>(string key, Lazy<TValue> value) where TValue : class;
        /// <summary>
        /// 保存字典值
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        IAlertMsg<Dictionary<string, TValue>> SaveDic<TValue>() where TValue : class;
        /// <summary>
        /// 保存字典值
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IAlertMsg<TValue> SaveDic<TValue>(string key, TValue value) where TValue : class;
        /// <summary>
        /// 保存字典
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="defVal"></param>
        /// <returns></returns>
        IAlertMsg<Dictionary<string, TValue>> SaveDic<TValue>(Dictionary<string, TValue> defVal) where TValue : class;
        /// <summary>
        /// 重置字典
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="defVal"></param>
        /// <returns></returns>
        IAlertMsg<Dictionary<string, TValue>> ResetDic<TValue>(Dictionary<string, TValue> defVal = null) where TValue : class;
        #endregion 字典内容
    }
    /// <summary>
    /// 页面缓存设置接口
    /// </summary>
    /// <typeparam name="TPage"></typeparam>
    public interface ICacheSettingModel<TPage>
    {
        #region // 模型内容
        /// <summary>
        /// 获取模型
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        ICacheSettingModel<TPage, TValue> GetModule<TValue>() where TValue : class;
        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="defVal"></param>
        /// <returns></returns>
        TValue Get<TValue>(TValue defVal = default(TValue)) where TValue : class;
        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="defVal"></param>
        /// <returns></returns>
        TValue Get<TValue>(Func<TValue> defVal) where TValue : class;
        /// <summary>
        /// 保存对象
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        IAlertMsg<TValue> Save<TValue>() where TValue : class;
        /// <summary>
        /// 保存对象
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="defVal"></param>
        /// <returns></returns>
        IAlertMsg<TValue> Save<TValue>(TValue defVal) where TValue : class;
        /// <summary>
        /// 重置对象
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="defVal"></param>
        /// <returns></returns>
        IAlertMsg<TValue> Reset<TValue>(TValue defVal = default(TValue)) where TValue : class;
        #endregion 模型内容
        #region // 字典内容
        /// <summary>
        /// 获取字典
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        ICacheSetDictModel<TPage, TValue> GetDict<TValue>() where TValue : class;
        /// <summary>
        /// 获取字典
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        Dictionary<string, TValue> GetDic<TValue>() where TValue : class;
        /// <summary>
        /// 获取字典值
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        TValue GetDic<TValue>(string key, TValue value = null) where TValue : class;
        /// <summary>
        /// 获取字典值
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        TValue GetDic<TValue>(string key, Func<TValue> value) where TValue : class;
        /// <summary>
        /// 获取或添加字典值
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        TValue GetOrAdd<TValue>(string key, Lazy<TValue> value) where TValue : class;
        /// <summary>
        /// 保存字典值
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        IAlertMsg<Dictionary<string, TValue>> SaveDic<TValue>() where TValue : class;
        /// <summary>
        /// 保存字典值
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IAlertMsg<TValue> SaveDic<TValue>(string key, TValue value) where TValue : class;
        /// <summary>
        /// 保存字典
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="defVal"></param>
        /// <returns></returns>
        IAlertMsg<Dictionary<string, TValue>> SaveDic<TValue>(Dictionary<string, TValue> defVal) where TValue : class;
        /// <summary>
        /// 重置字典
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="defVal"></param>
        /// <returns></returns>
        IAlertMsg<Dictionary<string, TValue>> ResetDic<TValue>(Dictionary<string, TValue> defVal = null) where TValue : class;
        #endregion 字典内容
    }
    /// <summary>
    /// 设置缓存
    /// </summary>
    /// <typeparam name="TPage"></typeparam>
    public class CacheSettingModel<TPage> : ICacheSettingModel<TPage>, ICacheSettingModel
    {
        static readonly Dictionary<String, String> TypeDic = new Dictionary<string, string>();
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="kvs"></param>
        public static void Set(Dictionary<string, string> kvs)
        {
            foreach (var item in kvs)
            {
                TypeDic[item.Key] = item.Value;
            }
        }
        /// <summary>
        /// 加载
        /// </summary>
        public static void Set(string key, string value)
        {
            TypeDic[key] = value;
        }
        /// <summary>
        /// 注册一下
        /// </summary>
        public static void Regist(Func<string> get, Func<Dictionary<String, string>, IAlertMsg> update)
        {
            try
            {
                Set(get()?.GetJsonObject<Dictionary<String, string>>());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            UpdateResource = update ?? UpdateResource;
        }
        #region // 模型内容
        /// <summary>
        /// 获取模型
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static ICacheSettingModel<TPage, TValue> GetModule<TValue>() where TValue : class
        {
            return Model<TValue>.Instance;
        }
        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public static TValue Get<TValue>(TValue defVal = default(TValue)) where TValue : class
        {
            return Model<TValue>.InnerValue ?? defVal;
        }
        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public static TValue Get<TValue>(Func<TValue> defVal) where TValue : class
        {
            return Model<TValue>.Instance.GetOrAdd(defVal);
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static IAlertMsg<TValue> Save<TValue>() where TValue : class
        {
            return Save(Model<TValue>.InnerValue);
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public static IAlertMsg<TValue> Save<TValue>(TValue defVal) where TValue : class
        {
            bool isUpdate = false;
            // 保存内容
            if (TypeDic.ContainsKey(Model<TValue>.Key))
            {
                if (defVal == null)
                {
                    TypeDic.Remove(Model<TValue>.Key);
                    isUpdate = true;
                }
                else
                {
                    var content = defVal.GetJsonString();
                    var text = Model<TValue>.InnerText;
                    if (content != text)
                    {
                        TypeDic[Model<TValue>.Key] = content;
                        Model<TValue>.Instance.Reload();
                        isUpdate = true;
                    }
                }
            }
            else
            {
                if (defVal != null)
                {
                    TypeDic[Model<TValue>.Key] = defVal.GetJsonString();
                    Model<TValue>.Instance.Reload();
                    isUpdate = true;
                }
            }
            if (isUpdate && UpdateResource != null)
            {
                var res = UpdateResource.Invoke(TypeDic);
                return new AlertMsg<TValue>(res.IsSuccess, res.Message) { Data = Model<TValue>.InnerValue };
            }
            return new AlertMsg<TValue>(true) { Data = defVal };
        }
        /// <summary>
        /// 重置
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public static IAlertMsg<TValue> Reset<TValue>(TValue defVal = default(TValue)) where TValue : class
        {
            bool isUpdate = false;
            if (TypeDic.ContainsKey(Model<TValue>.Key))
            {
                if (defVal == null)
                {
                    Model<TValue>.Instance.Reset();
                }
                else
                {
                    Model<TValue>.Instance.Set(defVal);
                    Model<TValue>.Instance.Reload();
                }
                isUpdate = true;
            }
            else
            {
                if (defVal != null)
                {
                    isUpdate = true;
                }
            }
            if (isUpdate && UpdateResource != null)
            {
                var res = UpdateResource.Invoke(TypeDic);
                return new AlertMsg<TValue>(res.IsSuccess, res.Message) { Data = Model<TValue>.InnerValue };
            }
            return new AlertMsg<TValue>(true) { Data = defVal };
        }
        /// <summary>
        /// 内部模型
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        internal sealed class Model<TValue> : ICacheSettingModel<TPage, TValue>
            where TValue : class
        {
            /// <summary>
            /// 实例
            /// </summary>
            public static Model<TValue> Instance { get; }
            /// <summary>
            /// 键
            /// </summary>
            public static String Key { get; }
            /// <summary>
            /// 内容值
            /// </summary>
            public static TValue InnerValue { get; private set; }
            /// <summary>
            /// 内容Json
            /// </summary>
            public static String InnerText { get; private set; }
            static Model()
            {
                Key = typeof(TValue).Name;
                if (TypeDic.TryGetValue(Key, out string value))
                {
                    try
                    {
                        InnerValue = value?.GetJsonObject<TValue>();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    InnerText = value ?? String.Empty;
                }
                else
                {
                    InnerValue = default(TValue);
                    InnerText = InnerValue?.GetJsonString() ?? String.Empty;
                }
                Instance = new Model<TValue>();
            }
            private Model() { }
            /// <summary>
            /// 值内容
            /// </summary>
            public TValue Value { get => InnerValue; }
            /// <summary>
            /// 获取内容
            /// </summary>
            /// <returns></returns>
            public TValue Get() => InnerValue;
            /// <summary>
            /// 获取内容
            /// </summary>
            /// <returns></returns>
            public TValue GetOrAdd(Lazy<TValue> defVal)
            {
                if (InnerValue == null)
                {
                    return defVal.Value;
                }
                return InnerValue;
            }
            /// <summary>
            /// 获取内容
            /// </summary>
            /// <returns></returns>
            public TValue GetOrAdd(Func<TValue> defVal)
            {
                if (InnerValue == null)
                {
                    return defVal();
                }
                return InnerValue;
            }
            /// <summary>
            /// 获取内容
            /// </summary>
            /// <returns></returns>
            public TValue GetDefault(Lazy<TValue> defVal)
            {
                if (InnerValue == null)
                {
                    return defVal.Value;
                }
                return InnerValue;
            }
            /// <summary>
            /// 获取内容
            /// </summary>
            /// <returns></returns>
            public TValue GetDefault(Func<TValue> defVal)
            {
                if (InnerValue == null)
                {
                    return defVal();
                }
                return InnerValue;
            }
            /// <summary>
            /// 保存
            /// </summary>
            /// <returns></returns>
            public ICacheSettingModel<TPage, TValue> Set(TValue model)
            {
                InnerValue = model;
                Update();
                return this;
            }
            /// <summary>
            /// 保存
            /// </summary>
            /// <returns></returns>
            public ICacheSettingModel<TPage, TValue> Save()
            {
                Save<TValue>();
                return this;
            }
            /// <summary>
            /// 设置并保存
            /// </summary>
            /// <returns></returns>
            public ICacheSettingModel<TPage, TValue> SaveSet(TValue model)
            {
                InnerValue = model;
                Update();
                Save<TValue>();
                return this;
            }
            /// <summary>
            /// 重置
            /// </summary>
            /// <returns></returns>
            public ICacheSettingModel<TPage, TValue> Reset()
            {
                try
                {
                    InnerValue = InnerText?.GetJsonObject<TValue>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    InnerValue = default(TValue);
                }
                Update();
                return this;
            }
            /// <summary>
            /// 重新加载
            /// </summary>
            /// <returns></returns>
            public ICacheSettingModel<TPage, TValue> Reload()
            {
                try
                {
                    InnerText = TypeDic.TryGetValue(Key, out string value) ? value : null;
                    InnerValue = InnerText?.GetJsonObject<TValue>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    InnerValue = default(TValue);
                }
                return this;
            }
            /// <summary>
            /// 重置
            /// </summary>
            /// <param name="model"></param>
            /// <returns></returns>
            public ICacheSettingModel<TPage, TValue> Reset(TValue model)
            {
                InnerValue = model;
                Update();
                return this;
            }
            static void Update()
            {
                if (InnerValue == null)
                {
                    TypeDic.Remove(Key);
                }
                else
                {
                    TypeDic[Key] = InnerValue.GetJsonString();
                }
            }
        }
        #endregion
        #region // 字典内容
        /// <summary>
        /// 获取模型
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static ICacheSetDictModel<TPage, TValue> GetDict<TValue>() where TValue : class
        {
            return Dic<TValue>.Instance;
        }
        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static Dictionary<string, TValue> GetDic<TValue>() where TValue : class
        {
            return Dic<TValue>.InnerValue;
        }
        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static TValue GetDic<TValue>(string key, TValue value = null) where TValue : class
        {
            return Dic<TValue>.Instance.TryGet(key, value);
        }
        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static TValue GetDic<TValue>(string key, Func<TValue> value) where TValue : class
        {
            return Dic<TValue>.Instance.TryGet(key, value);
        }
        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static TValue GetOrAdd<TValue>(string key, Lazy<TValue> value) where TValue : class
        {
            return Dic<TValue>.Instance.TryGetOrAdd(key, value);
        }
        /// <summary>
        /// 保存字典
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static IAlertMsg<Dictionary<string, TValue>> SaveDic<TValue>() where TValue : class
        {
            return SaveDic(Dic<TValue>.InnerValue);
        }
        /// <summary>
        /// 保存字典
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static IAlertMsg<TValue> SaveDic<TValue>(string key, TValue value) where TValue : class
        {
            Dic<TValue>.Instance.SaveSet(key, value);
            return new AlertMsg<TValue>(true) { Data = value };
        }
        /// <summary>
        /// 保存字典
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public static IAlertMsg<Dictionary<string, TValue>> SaveDic<TValue>(Dictionary<string, TValue> defVal) where TValue : class
        {
            bool isUpdate = false;
            if (TypeDic.ContainsKey(Dic<TValue>.Key))
            {
                // 保存内容
                if (defVal.IsEmpty())
                {
                    Dic<TValue>.Instance.Clear();
                    isUpdate = true;
                }
                else
                {
                    var content = defVal.GetJsonString();
                    var text = Dic<TValue>.InnerText;
                    if (content != text)
                    {
                        TypeDic[Dic<TValue>.Key] = content;
                        Dic<TValue>.Instance.Reload();
                        isUpdate = true;
                    }
                }
            }
            else
            {
                if (defVal != null)
                {
                    TypeDic[Dic<TValue>.Key] = defVal.GetJsonString();
                    isUpdate = true;
                }
            }
            if (isUpdate && UpdateResource != null)
            {
                var res = UpdateResource.Invoke(TypeDic);
                return new AlertMsg<Dictionary<string, TValue>>(res.IsSuccess, res.Message) { Data = Dic<TValue>.InnerValue };
            }
            return new AlertMsg<Dictionary<string, TValue>>(true) { Data = defVal };
        }
        /// <summary>
        /// 重置字典
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public static IAlertMsg<Dictionary<string, TValue>> ResetDic<TValue>(Dictionary<string, TValue> defVal = null) where TValue : class
        {
            bool isUpdate = false;
            if (TypeDic.ContainsKey(Dic<TValue>.Key))
            {
                if (defVal == null)
                {
                    Dic<TValue>.Instance.Clear();
                }
                else
                {
                    TypeDic[Dic<TValue>.Key] = defVal.GetJsonString();
                    Dic<TValue>.Instance.Reload();
                }
                isUpdate = true;
            }
            else
            {
                if (defVal != null)
                {
                    TypeDic[Dic<TValue>.Key] = defVal.GetJsonString();
                    Dic<TValue>.Instance.Reload();
                    isUpdate = true;
                }
            }
            if (isUpdate && UpdateResource != null)
            {
                var res = UpdateResource.Invoke(TypeDic);
                return new AlertMsg<Dictionary<string, TValue>>(res.IsSuccess, res.Message) { Data = Dic<TValue>.InnerValue };
            }
            return new AlertMsg<Dictionary<string, TValue>>(true) { Data = defVal };
        }
        /// <summary>
        /// 字典类
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        internal sealed class Dic<TValue> : ICacheSetDictModel<TPage, TValue>
            where TValue : class
        {
            /// <summary>
            /// 实例
            /// </summary>
            public static Dic<TValue> Instance { get; }
            /// <summary>
            /// 键
            /// </summary>
            public static String Key { get; }
            private Dic() { }
            /// <summary>
            /// 内容值
            /// </summary>
            public static Dictionary<string, TValue> InnerValue { get; private set; }
            /// <summary>
            /// 内容Json
            /// </summary>
            public static String InnerText { get; private set; }
            static Dic()
            {
                Key = $"Dic{typeof(TValue).Name}";
                if (TypeDic.TryGetValue(Key, out string value))
                {
                    try
                    {
                        InnerValue = value.GetJsonObject<Dictionary<string, TValue>>();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        InnerValue = new Dictionary<string, TValue>();
                    }
                    InnerText = value ?? String.Empty;
                }
                else
                {
                    InnerValue = new Dictionary<string, TValue>();
                    InnerText = InnerValue?.GetJsonString() ?? String.Empty;
                }
                Instance = new Dic<TValue>();
            }
            /// <summary>
            /// 值内容
            /// </summary>
            public Dictionary<string, TValue> Value { get => InnerValue; }
            /// <summary>
            /// 获取值(不验证key)
            /// </summary>
            /// <param name="key"></param>
            /// <param name="defVal"></param>
            /// <returns></returns>
            public TValue Get(string key, TValue defVal = default(TValue))
            {
                if (InnerValue.TryGetValue(key, out TValue value))
                {
                    return value;
                }
                return defVal;
            }
            /// <summary>
            /// 获取值(验证key)
            /// </summary>
            /// <param name="key"></param>
            /// <param name="defVal"></param>
            /// <returns></returns>
            public TValue TryGet(string key, TValue defVal = default(TValue))
            {
                if (string.IsNullOrEmpty(key)) { return defVal; }
                return Get(key, defVal);
            }
            /// <summary>
            /// 获取值(验证key)
            /// </summary>
            /// <param name="key"></param>
            /// <param name="defVal"></param>
            /// <returns></returns>
            public TValue GetOrAdd(string key, TValue defVal = default(TValue))
            {
                if (string.IsNullOrEmpty(key)) { return defVal; }
                return GetOrAdd(key, () => defVal);
            }
            /// <summary>
            /// 获取值(不验证key)
            /// </summary>
            /// <param name="key"></param>
            /// <param name="defVal"></param>
            /// <returns></returns>
            public TValue Get(string key, Func<TValue> defVal)
            {
                if (InnerValue.TryGetValue(key, out TValue value))
                {
                    return value;
                }
                return defVal == null ? default(TValue) : defVal.Invoke();
            }
            /// <summary>
            /// 获取值(不验证key)
            /// </summary>
            /// <param name="key"></param>
            /// <param name="defVal"></param>
            /// <returns></returns>
            public TValue GetOrAdd(string key, Func<TValue> defVal)
            {
                if (InnerValue.TryGetValue(key, out TValue value))
                {
                    return value;
                }
                if (defVal == null)
                {
                    InnerValue[key] = default(TValue);
                    Update();
                    return default(TValue);
                }
                var res = InnerValue[key] = defVal.Invoke();
                Update();
                return res;
            }
            /// <summary>
            /// 获取值(不验证key)
            /// </summary>
            /// <param name="key"></param>
            /// <param name="defVal"></param>
            /// <returns></returns>
            public TValue GetOrAdd(string key, Lazy<TValue> defVal)
            {
                if (InnerValue.TryGetValue(key, out TValue value))
                {
                    return value;
                }
                if (defVal == null)
                {
                    InnerValue[key] = default(TValue);
                    Update();
                    return default(TValue);
                }
                var res = InnerValue[key] = defVal.Value;
                Update();
                return res;
            }
            /// <summary>
            /// 获取值(验证key)
            /// </summary>
            /// <param name="key"></param>
            /// <param name="defVal"></param>
            /// <returns></returns>
            public TValue TryGet(string key, Func<TValue> defVal)
            {
                if (string.IsNullOrEmpty(key))
                {
                    return defVal == null ? default(TValue) : defVal.Invoke();
                }
                return Get(key, defVal);
            }
            /// <summary>
            /// 获取值(验证key)
            /// </summary>
            /// <param name="key"></param>
            /// <param name="defVal"></param>
            /// <returns></returns>
            public TValue TryGetOrAdd(string key, Func<TValue> defVal)
            {
                if (string.IsNullOrEmpty(key))
                {
                    return defVal == null ? default(TValue) : defVal.Invoke();
                }
                return GetOrAdd(key, defVal);
            }
            /// <summary>
            /// 获取值(验证key)
            /// </summary>
            /// <param name="key"></param>
            /// <param name="defVal"></param>
            /// <returns></returns>
            public TValue TryGetOrAdd(string key, Lazy<TValue> defVal)
            {
                if (string.IsNullOrEmpty(key))
                {
                    return defVal == null ? default(TValue) : defVal.Value;
                }
                return GetOrAdd(key, defVal);
            }
            /// <summary>
            /// 设置内容
            /// </summary>
            /// <param name="key"></param>
            /// <param name="model"></param>
            public ICacheSetDictModel<TPage, TValue> Set(string key, TValue model)
            {
                if (model == null)
                {
                    InnerValue.Remove(key);
                }
                else
                {
                    InnerValue[key] = model;
                }
                Update();
                return this;
            }
            /// <summary>
            /// 重新加载
            /// </summary>
            /// <returns></returns>
            public ICacheSetDictModel<TPage, TValue> Reload()
            {
                try
                {
                    InnerText = TypeDic.TryGetValue(Key, out string value) ? value : "{}";
                    InnerValue = InnerText.GetJsonObject<Dictionary<string, TValue>>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    InnerValue = new Dictionary<string, TValue>();
                }
                return this;
            }
            /// <summary>
            /// 重置
            /// </summary>
            /// <returns></returns>
            public ICacheSetDictModel<TPage, TValue> Reset()
            {
                try
                {
                    InnerValue = InnerText.GetJsonObject<Dictionary<string, TValue>>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    InnerValue = new Dictionary<string, TValue>();
                }
                Update();
                return this;
            }
            /// <summary>
            /// 重置内容
            /// </summary>
            /// <param name="key"></param>
            /// <param name="model"></param>
            public ICacheSetDictModel<TPage, TValue> Reset(string key, TValue model = default(TValue))
            {
                TrySet(key, model);
                return this;
            }
            /// <summary>
            /// 设置内容
            /// </summary>
            /// <param name="key"></param>
            /// <param name="model"></param>
            public ICacheSetDictModel<TPage, TValue> TrySet(string key, TValue model)
            {
                if (string.IsNullOrEmpty(key)) { return this; }
                if (model == null)
                {
                    InnerValue.Remove(key);
                }
                else
                {
                    InnerValue[key] = model;
                }
                Update();
                return this;
            }
            /// <summary>
            /// 保存
            /// </summary>
            /// <returns></returns>
            public ICacheSetDictModel<TPage, TValue> Save()
            {
                SaveDic<TValue>();
                return this;
            }
            /// <summary>
            /// 保存
            /// </summary>
            /// <returns></returns>
            public ICacheSetDictModel<TPage, TValue> SaveSet(string key, TValue model)
            {
                TrySet(key, model);
                SaveDic<TValue>();
                return this;
            }
            /// <summary>
            /// 保存
            /// </summary>
            /// <returns></returns>
            public async Task<ICacheSetDictModel<TPage, TValue>> SaveSetAsync(string key, TValue model)
            {
                return await Task.Factory.StartNew(() =>
                {
                    TrySet(key, model);
                    SaveDic<TValue>();
                    return this;
                });
            }
            /// <summary>
            /// 清空内容
            /// </summary>
            /// <returns></returns>
            public ICacheSetDictModel<TPage, TValue> Clear()
            {
                InnerValue.Clear();
                TypeDic.Remove(Key);
                return this;
            }
            static void Update()
            {
                TypeDic[Key] = InnerValue.GetJsonString();
            }
        }
        #endregion
        /// <summary>
        /// 更新资源
        /// </summary>
        public static Func<Dictionary<String, string>, IAlertMsg> UpdateResource { get; set; }
        /// <summary>
        /// 构造
        /// </summary>
        protected CacheSettingModel() { }
        #region // 显示实现 ICacheSettingModel<TPage>
        TValue ICacheSettingModel<TPage>.Get<TValue>(TValue defVal) => Get<TValue>(defVal);
        TValue ICacheSettingModel<TPage>.Get<TValue>(Func<TValue> defVal) => Get<TValue>(defVal);
        Dictionary<string, TValue> ICacheSettingModel<TPage>.GetDic<TValue>() => GetDic<TValue>();
        TValue ICacheSettingModel<TPage>.GetDic<TValue>(string key, TValue value) => GetDic<TValue>(key, value);
        TValue ICacheSettingModel<TPage>.GetDic<TValue>(string key, Func<TValue> value) => GetDic<TValue>(key, value);
        ICacheSetDictModel<TPage, TValue> ICacheSettingModel<TPage>.GetDict<TValue>() => GetDict<TValue>();
        ICacheSettingModel<TPage, TValue> ICacheSettingModel<TPage>.GetModule<TValue>() => GetModule<TValue>();
        TValue ICacheSettingModel<TPage>.GetOrAdd<TValue>(string key, Lazy<TValue> value) => GetOrAdd<TValue>(key, value);
        IAlertMsg<TValue> ICacheSettingModel<TPage>.Reset<TValue>(TValue defVal) => Reset<TValue>(defVal);
        IAlertMsg<Dictionary<string, TValue>> ICacheSettingModel<TPage>.ResetDic<TValue>(Dictionary<string, TValue> defVal) => ResetDic<TValue>(defVal);
        IAlertMsg<TValue> ICacheSettingModel<TPage>.Save<TValue>() => Save<TValue>();
        IAlertMsg<TValue> ICacheSettingModel<TPage>.Save<TValue>(TValue defVal) => Save<TValue>(defVal);
        IAlertMsg<Dictionary<string, TValue>> ICacheSettingModel<TPage>.SaveDic<TValue>() => SaveDic<TValue>();
        IAlertMsg<TValue> ICacheSettingModel<TPage>.SaveDic<TValue>(string key, TValue value) => SaveDic<TValue>(key, value);
        IAlertMsg<Dictionary<string, TValue>> ICacheSettingModel<TPage>.SaveDic<TValue>(Dictionary<string, TValue> defVal) => SaveDic<TValue>(defVal);
        #endregion 显示实现 ICacheSettingModel<TPage>
        #region // 显示实现 ICacheSettingModel
        TValue ICacheSettingModel.Get<TValue>(TValue defVal) => Get<TValue>(defVal);
        TValue ICacheSettingModel.Get<TValue>(Func<TValue> defVal) => Get<TValue>(defVal);
        Dictionary<string, TValue> ICacheSettingModel.GetDic<TValue>() => GetDic<TValue>();
        TValue ICacheSettingModel.GetDic<TValue>(string key, TValue value) => GetDic<TValue>(key, value);
        TValue ICacheSettingModel.GetDic<TValue>(string key, Func<TValue> value) => GetDic<TValue>(key, value);
        TValue ICacheSettingModel.GetOrAdd<TValue>(string key, Lazy<TValue> value) => GetOrAdd<TValue>(key, value);
        IAlertMsg<TValue> ICacheSettingModel.Reset<TValue>(TValue defVal) => Reset<TValue>(defVal);
        IAlertMsg<Dictionary<string, TValue>> ICacheSettingModel.ResetDic<TValue>(Dictionary<string, TValue> defVal) => ResetDic<TValue>(defVal);
        IAlertMsg<TValue> ICacheSettingModel.Save<TValue>() => Save<TValue>();
        IAlertMsg<TValue> ICacheSettingModel.Save<TValue>(TValue defVal) => Save<TValue>(defVal);
        IAlertMsg<Dictionary<string, TValue>> ICacheSettingModel.SaveDic<TValue>() => SaveDic<TValue>();
        IAlertMsg<TValue> ICacheSettingModel.SaveDic<TValue>(string key, TValue value) => SaveDic<TValue>(key, value);
        IAlertMsg<Dictionary<string, TValue>> ICacheSettingModel.SaveDic<TValue>(Dictionary<string, TValue> defVal) => SaveDic<TValue>(defVal);
        #endregion 显示实现 ICacheSettingModel
    }
    /// <summary>
    /// 设置缓存
    /// </summary>
    /// <typeparam name="TPage"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public sealed class CacheSettingModel<TPage, TValue> : CacheSettingModel<TPage>
        where TValue : class
    {
        /// <summary>
        /// 模型
        /// </summary>
        public ICacheSettingModel<TPage, TValue> Module { get => Model<TValue>.Instance; }
        /// <summary>
        /// 模型
        /// </summary>
        public ICacheSetDictModel<TPage, TValue> Dict { get => Dic<TValue>.Instance; }
        #region // 模型内容
        /// <summary>
        /// 获取模型
        /// </summary>
        /// <returns></returns>
        public ICacheSettingModel<TPage, TValue> GetModel() => Module;
        /// <summary>
        /// 获取值
        /// </summary>
        /// <returns></returns>
        public TValue Get() => CacheSettingModel<TPage>.Get<TValue>();
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public IAlertMsg<TValue> Save() => CacheSettingModel<TPage>.Save<TValue>();
        /// <summary>
        /// 重置
        /// </summary>
        /// <returns></returns>
        public IAlertMsg<TValue> Reset() => CacheSettingModel<TPage>.Reset<TValue>();
        #endregion
        #region // 字典内容
        /// <summary>
        /// 获取模型
        /// </summary>
        /// <returns></returns>
        public ICacheSetDictModel<TPage, TValue> GetDict() => CacheSettingModel<TPage>.GetDict<TValue>();
        /// <summary>
        /// 获取值
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, TValue> GetDic() => CacheSettingModel<TPage>.GetDic<TValue>();
        /// <summary>
        /// 保存字典
        /// </summary>
        /// <returns></returns>
        public IAlertMsg<Dictionary<string, TValue>> SaveDic() => CacheSettingModel<TPage>.SaveDic<TValue>();
        /// <summary>
        /// 重置字典
        /// </summary>
        /// <returns></returns>
        public IAlertMsg<Dictionary<string, TValue>> ResetDic() => CacheSettingModel<TPage>.ResetDic<TValue>();
        #endregion
    }
}
