using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Dabber;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;

namespace System.Data.Cobber
{
    /// <summary>
    /// 缓存模型
    /// </summary>
    public static class CacheModel
    {
        /// <summary>
        /// Concurrent字典(线程安全)
        /// </summary>
        public static ICacheModel Concurrent { get; } = new CacheConcurrentModel();
        /// <summary>
        /// HashTable字典(线程安全)
        /// </summary>
        public static ICacheModel HashTable { get; } = new CacheHashTableModel();
        /// <summary>
        /// Dictionary字典(非线程安全)
        /// </summary>
        public static ICacheModel Dictionary { get; } = new CacheDictionaryModel();
    }
    /// <summary>
    /// 哈希缓存模型(多线程)
    /// </summary>
    public sealed class CacheHashTableModel : ACacheHashTable { }
    /// <summary>
    /// 字典缓存模型(多线程)
    /// </summary>
    public sealed class CacheConcurrentModel : ACacheConcurrent { }
    /// <summary>
    /// 字典缓存模型(非多线程)
    /// </summary>
    public sealed class CacheDictionaryModel : ACacheDictionary { }
    /// <summary>
    /// 缓存接口
    /// </summary>
    public interface ICacheModel
    {
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Exists(string key);
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get<T>(string key);
        /// <summary>
        /// 获取键的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object Get(string key);
        /// <summary>
        /// 获取缓存,不存在时执行func存后返回
        /// </summary>
        /// <returns></returns>
        T GetOrAdd<T>(string key, Func<T> func);
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        bool Set<T>(string key, T value);
        /// <summary>
        /// 设置缓存
        /// 设置绝对时间过期
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expire"></param>
        /// <returns></returns>
        bool Set<T>(string key, T value, DateTimeOffset expire);
        /// <summary>
        /// 设置缓存
        /// 设置滑动时间过期
        /// </summary>
        bool Set<T>(string key, T value, TimeSpan sliding);
        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key"></param>
        object Remove(string key);
        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key"></param>
        bool Delete(string key);
        #region // 字符串String
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <returns></returns>
        string StringGet(string key);
        /// <summary>
        /// 获取缓存,不存在时执行func存后返回
        /// </summary>
        /// <returns></returns>
        string StringGet(string key, Func<string> func);
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        string StringSet(string key, string value);
        /// <summary>
        /// 设置缓存
        /// 设置绝对时间过期
        /// </summary>
        /// <returns></returns>
        string StringSet(string key, string value, DateTimeOffset expire);
        /// <summary>
        /// 设置缓存
        /// 设置滑动时间过期
        /// </summary>
        /// <returns></returns>
        string StringSet(string key, string value, TimeSpan sliding);
        #endregion
        #region // 内部操作
        /// <summary>
        /// 获取键列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetKeys();
        /// <summary>
        /// 获取键列表
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        IEnumerable<string> GetKeys(PagingResult paging);
        /// <summary>
        /// 获取键列表
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        IEnumerable<string> GetKeys(string pattern, PagingResult paging);
        #endregion
        #region // 列表List
        #endregion
    }
    /// <summary>
    /// 缓存值类
    /// </summary>
    public interface ICacheDicValue
    {
        /// <summary>
        /// 过期时间(Tick)
        /// </summary>
        long Expire { get; set; }
        /// <summary>
        /// 滑动时间(Tick)
        /// </summary>
        long? Sliding { get; set; }
        /// <summary>
        /// 存储键
        /// </summary>
        string Key { get; set; }
        /// <summary>
        /// 存储值
        /// </summary>
        object Value { get; set; }
        /// <summary>
        /// 是有效值
        /// </summary>
        bool IsValid();
    }
    /// <summary>
    /// 缓存值类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ECacheDicValue<T> : ICacheDicValue
    {
        /// <summary>
        /// 构造
        /// </summary>
        public ECacheDicValue(string key)
        {
            Key = key;
        }
        /// <summary>
        /// 存储键
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 存储值
        /// </summary>
        public T Value { get; set; }
        /// <summary>
        /// 过期时间(Tick)
        /// </summary>
        public long Expire { get; set; }
        /// <summary>
        /// 滑动时间(Tick)
        /// </summary>
        public long? Sliding { get; set; }
        /// <summary>
        /// 存储值
        /// </summary>
        object ICacheDicValue.Value { get => Value; set { Value = (T)value; } }
        /// <summary>
        /// 是有效值
        /// </summary>
        public bool IsValid()
        {
            if (Expire >= DateTime.Now.Ticks)
            {
                if (Sliding.HasValue) { Expire += Sliding.Value; }
                return true;
            }
            return false;
        }
    }
    /// <summary>
    /// HashTable的缓存表
    /// </summary>
    public abstract class ACacheHashTable : ICacheModel
    {
        #region // 静态定义
        private static Hashtable InternalDb = Hashtable.Synchronized(new Hashtable());
        private static Timer ClearTimer { get; } = new Timer
        {
            Enabled = true, // 是否执行System.Timers.Timer.Elapsed事件
            Interval = 43200000, // 执行间隔时间,单位为毫秒;此时时间间隔为0.5天
            AutoReset = true, // 设置是执行一次（false）还是一直执行(true)
        };
        static ACacheHashTable()
        {
            var tagNow = DateTime.Now;
            var timer = new Timer
            {
                Enabled = true, // 是否执行System.Timers.Timer.Elapsed事件
                Interval = (tagNow.Date.AddDays(1).AddHours(1.5) - tagNow).TotalMilliseconds, // 执行间隔时间,到1点半执行一次
                AutoReset = false, // 设置是执行一次（false）还是一直执行(true)
            };
            timer.Start();
            timer.Elapsed += new ElapsedEventHandler((se, ev) =>
            {
                ClearTimer.Elapsed += new ElapsedEventHandler((s, e) => InternalClear());
                ClearTimer.Start();
            });
        }
        internal static bool InternalClear()
        {
            var kvList = new List<DictionaryEntry>();
            foreach (DictionaryEntry item in InternalDb)
            {
                kvList.Add(item);
            }
            var tagTicks = DateTime.Now.Ticks;
            foreach (DictionaryEntry item in kvList)
            {
                var entity = item.Value as ICacheDicValue;
                if (entity.Expire < tagTicks)
                {
                    InternalDb.Remove(entity.Key);
                }
            }
            return true;
        }
        #endregion
        /// <summary>
        /// 空构造
        /// </summary>
        public ACacheHashTable() : this(string.Empty)
        {
        }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="connString"></param>
        public ACacheHashTable(string connString)
        {
        }
        #region // 通用操作
        /// <summary>
        /// 存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            return InternalDb.ContainsKey(key);
        }
        /// <summary>
        /// 获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            if (InternalDb.ContainsKey(key))
            {
                var item = InternalDb[key] as ICacheDicValue;
                // 判断过期
                if (item.IsValid())
                {
                    if (item is ECacheDicValue<T>)
                    {
                        return (item as ECacheDicValue<T>).Value;
                    }
                    return (T)item.Value;
                }
                else
                {
                    InternalDb.Remove(key);
                }
            }
            return default(T);
        }
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get(string key)
        {
            if (InternalDb.ContainsKey(key))
            {
                var item = InternalDb[key] as ICacheDicValue;
                // 判断过期
                if (item.IsValid())
                {
                    return item.Value;
                }
                else
                {
                    InternalDb.Remove(key);
                }
            }
            return default;
        }
        /// <summary>
        /// 获取或添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public T GetOrAdd<T>(string key, Func<T> func)
        {
            if (InternalDb.ContainsKey(key))
            {
                ICacheDicValue item = InternalDb[key] as ICacheDicValue;
                // 判断过期
                if (item.IsValid())
                {
                    if (item is ECacheDicValue<T>)
                    {
                        return (item as ECacheDicValue<T>).Value;
                    }
                    if (item.Value is T)
                    {
                        return (T)item.Value;
                    }
                }
            }
            T result = func();
            Task.Factory.StartNew(() => { Set(key, result); });
            return result;
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Remove(string key)
        {
            object result = null;
            if (InternalDb.ContainsKey(key))
            {
                result = InternalDb[key];
            }
            InternalDb.Remove(key);
            return result;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Delete(string key)
        {
            InternalDb.Remove(key);
            return true;
        }
        /// <summary>
        /// 设置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Set<T>(string key, T value)
        {
            InternalDb[key] = new ECacheDicValue<T>(key)
            {
                Expire = DateTime.Now.AddDays(1).Ticks,
                Value = value,
            };
            return true;
        }
        /// <summary>
        /// 设置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expire"></param>
        /// <returns></returns>
        public bool Set<T>(string key, T value, DateTimeOffset expire)
        {
            InternalDb[key] = new ECacheDicValue<T>(key)
            {
                Expire = expire.Ticks,
                Value = value,
            };
            return true;
        }
        /// <summary>
        /// 设置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="sliding"></param>
        /// <returns></returns>
        public bool Set<T>(string key, T value, TimeSpan sliding)
        {
            InternalDb[key] = new ECacheDicValue<T>(key)
            {
                Expire = DateTime.Now.Ticks + sliding.Ticks,
                Sliding = sliding.Ticks,
                Value = value,
            };
            return true;
        }
        /// <summary>
        /// 清除
        /// </summary>
        /// <returns></returns>
        public bool Clear()
        {
            return InternalClear();
        }
        #endregion
        #region // String操作
        /// <summary>
        /// 字符串获取
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string StringGet(string key) => Get<string>(key);
        /// <summary>
        /// 字符串获取
        /// </summary>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public string StringGet(string key, Func<string> func) => GetOrAdd(key, func);
        /// <summary>
        /// 字符串获取
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string StringSet(string key, string value) => Set(key, value) ? value : default;
        /// <summary>
        /// 字符串设置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expire"></param>
        /// <returns></returns>
        public string StringSet(string key, string value, DateTimeOffset expire) => Set(key, value, expire) ? value : default;
        /// <summary>
        /// 字符串设置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="sliding"></param>
        /// <returns></returns>
        public string StringSet(string key, string value, TimeSpan sliding) => Set(key, value, sliding) ? value : default;
        #endregion
        #region // 内部操作
        /// <summary>
        /// 获取键列
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetKeys()
        {
            foreach (string item in InternalDb.Keys)
            {
                yield return item;
            }
        }
        /// <summary>
        /// 获取键列
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        public IEnumerable<string> GetKeys(PagingResult paging)
        {
            var keys = GetKeys();
            paging.TotalCount = keys.Count();
            var items = keys.Skip(paging.Skip).Take(paging.Take);
            paging.Items = items;
            return items;
        }
        /// <summary>
        /// 获取键列
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        public IEnumerable<string> GetKeys(string pattern, PagingResult paging)
        {
            var regex = new Regex(pattern);
            var keys = GetKeys().Where(s => regex.IsMatch(s));
            paging.TotalCount = keys.Count();
            var items = keys.Skip(paging.Skip).Take(paging.Take);
            paging.Items = items;
            return items;
        }
        #endregion
    }
    /// <summary>
    /// ConcurrentDictionary的缓存表
    /// </summary>
    public abstract class ACacheConcurrent : ICacheModel
    {
        #region // 静态定义
        private static ConcurrentDictionary<string, ICacheDicValue> InternalDb = new ConcurrentDictionary<string, ICacheDicValue>(StringComparer.OrdinalIgnoreCase);
        private static Timer ClearTimer { get; } = new Timer
        {
            Enabled = true, // 是否执行System.Timers.Timer.Elapsed事件
            Interval = 43200000, // 执行间隔时间,单位为毫秒;此时时间间隔为0.5天
            AutoReset = true, // 设置是执行一次（false）还是一直执行(true)
        };
        static ACacheConcurrent()
        {
            var tagNow = DateTime.Now;
            var timer = new Timer
            {
                Enabled = true, // 是否执行System.Timers.Timer.Elapsed事件
                Interval = (tagNow.Date.AddDays(1).AddHours(1.5) - tagNow).TotalMilliseconds, // 执行间隔时间,到1点半执行一次
                AutoReset = false, // 设置是执行一次（false）还是一直执行(true)
            };
            timer.Start();
            timer.Elapsed += new ElapsedEventHandler((se, ev) =>
            {
                ClearTimer.Elapsed += new ElapsedEventHandler((s, e) => InternalClear());
                ClearTimer.Start();
            });
        }
        internal static bool InternalClear()
        {
            var result = true;
            var tagTicks = DateTime.Now.Ticks;
            foreach (var item in InternalDb.ToList())
            {
                if (item.Value.Expire < tagTicks)
                {
                    result &= InternalDb.TryRemove(item.Key, out _);
                }
            }
            return result;
        }
        #endregion
        /// <summary>
        /// 默认构造
        /// </summary>
        public ACacheConcurrent() : this(string.Empty)
        {
        }
        /// <summary>
        /// 连接字符串构造
        /// </summary>
        /// <param name="connString"></param>
        public ACacheConcurrent(string connString)
        {
        }
        #region // 通用操作
        /// <summary>
        /// 判断存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            return InternalDb.ContainsKey(key);
        }
        /// <summary>
        /// 获取内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            if (InternalDb.TryGetValue(key, out ICacheDicValue item))
            {
                // 判断过期
                if (item.IsValid())
                {
                    if (item is ECacheDicValue<T>)
                    {
                        return (item as ECacheDicValue<T>).Value;
                    }
                    return (T)item.Value;
                }
                else
                {
                    InternalDb.TryRemove(key, out _);
                }
            }
            return default(T);
        }
        /// <summary>
        /// 获取内容
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get(string key)
        {
            if (InternalDb.TryGetValue(key, out ICacheDicValue item))
            {
                // 判断过期
                if (item.IsValid())
                {
                    return item.Value;
                }
                else
                {
                    InternalDb.TryRemove(key, out _);
                }
            }
            return null;
        }
        /// <summary>
        /// 获取或添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public T GetOrAdd<T>(string key, Func<T> func)
        {
            if (InternalDb.TryGetValue(key, out ICacheDicValue item))
            {
                // 判断过期
                if (item.IsValid())
                {
                    if (item is ECacheDicValue<T>)
                    {
                        return (item as ECacheDicValue<T>).Value;
                    }
                    if (item.Value is T)
                    {
                        return (T)item.Value;
                    }
                }
                else
                {
                    InternalDb.TryRemove(key, out _);
                }
            }
            T result = func();
            Task.Factory.StartNew(() => { Set(key, result); });
            return result;
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Remove(string key)
        {
            if (InternalDb.TryRemove(key, out ICacheDicValue item))
            {
                return item.Value;
            }
            return null;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Delete(string key)
        {
            return InternalDb.TryRemove(key, out _);
        }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Set<T>(string key, T value)
        {
            InternalDb[key] = new ECacheDicValue<T>(key)
            {
                Expire = DateTime.Now.AddDays(1).Ticks,
                Value = value,
            };
            return true;
        }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expire"></param>
        /// <returns></returns>
        public bool Set<T>(string key, T value, DateTimeOffset expire)
        {
            InternalDb[key] = new ECacheDicValue<T>(key)
            {
                Expire = expire.Ticks,
                Value = value,
            };
            return true;
        }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="sliding"></param>
        /// <returns></returns>
        public bool Set<T>(string key, T value, TimeSpan sliding)
        {
            if (key == null) { return false; }
            InternalDb[key] = new ECacheDicValue<T>(key)
            {
                Expire = DateTime.Now.Ticks + sliding.Ticks,
                Sliding = sliding.Ticks,
                Value = value,
            };
            return true;
        }
        /// <summary>
        /// 清空
        /// </summary>
        /// <returns></returns>
        public bool Clear()
        {
            return InternalClear();
        }
        #endregion
        #region // String操作
        /// <summary>
        /// 字符串获取
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string StringGet(string key) => Get<string>(key);
        /// <summary>
        /// 字符串获取
        /// </summary>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public string StringGet(string key, Func<string> func) => GetOrAdd(key, func);
        /// <summary>
        /// 字符串获取
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string StringSet(string key, string value) => Set(key, value) ? value : default;
        /// <summary>
        /// 字符串获取
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expire"></param>
        /// <returns></returns>
        public string StringSet(string key, string value, DateTimeOffset expire) => Set(key, value, expire) ? value : default;
        /// <summary>
        /// 字符串获取
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="sliding"></param>
        /// <returns></returns>
        public string StringSet(string key, string value, TimeSpan sliding) => Set(key, value, sliding) ? value : default;
        #endregion
        #region // 内部操作
        /// <summary>
        /// 获取全部值
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetKeys()
        {
            return InternalDb.Keys;
        }
        /// <summary>
        /// 获取分页结果
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        public IEnumerable<string> GetKeys(PagingResult paging)
        {
            var keys = GetKeys();
            paging.TotalCount = keys.Count();
            var items = keys.Skip(paging.Skip).Take(paging.Take);
            paging.Items = items;
            return items;
        }
        /// <summary>
        /// 获取分页结果
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        public IEnumerable<string> GetKeys(string pattern, PagingResult paging)
        {
            var regex = new Regex(pattern);
            var keys = GetKeys().Where(s => regex.IsMatch(s));
            paging.TotalCount = keys.Count();
            var items = keys.Skip(paging.Skip).Take(paging.Take);
            paging.Items = items;
            return items;
        }
        #endregion
    }
    /// <summary>
    /// Dictionary的缓存表
    /// </summary>
    public abstract class ACacheDictionary : ICacheModel
    {
        #region // 静态定义
        private static Dictionary<string, ICacheDicValue> InternalDb = new Dictionary<string, ICacheDicValue>(StringComparer.OrdinalIgnoreCase);
        private static Timer ClearTimer { get; } = new Timer
        {
            Enabled = true, // 是否执行System.Timers.Timer.Elapsed事件
            Interval = 43200000, // 执行间隔时间,单位为毫秒;此时时间间隔为0.5天
            AutoReset = true, // 设置是执行一次（false）还是一直执行(true)
        };
        static ACacheDictionary()
        {
            var tagNow = DateTime.Now;
            var timer = new Timer
            {
                Enabled = true, // 是否执行System.Timers.Timer.Elapsed事件
                Interval = (tagNow.Date.AddDays(1).AddHours(1.5) - tagNow).TotalMilliseconds, // 执行间隔时间,到1点半执行一次
                AutoReset = false, // 设置是执行一次（false）还是一直执行(true)
            };
            timer.Start();
            timer.Elapsed += new ElapsedEventHandler((se, ev) =>
            {
                ClearTimer.Elapsed += new ElapsedEventHandler((s, e) => InternalClear());
                ClearTimer.Start();
            });
        }
        internal static bool InternalClear()
        {
            var result = true;
            var tagTicks = DateTime.Now.Ticks;
            foreach (var item in InternalDb.ToList())
            {
                if (item.Value.Expire < tagTicks)
                {
#if NET40 || NET45
                    result &= InternalDb.Remove(item.Key);
#else
                    result &= InternalDb.Remove(item.Key, out _);
#endif
                }
            }
            return result;
        }
        #endregion
        /// <summary>
        /// 构造
        /// </summary>
        public ACacheDictionary() : this(string.Empty)
        {
        }
        /// <summary>
        /// 连接字符串构造
        /// </summary>
        /// <param name="connString"></param>
        public ACacheDictionary(string connString)
        {
        }
        #region // 通用操作
        /// <summary>
        /// 存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            return InternalDb.ContainsKey(key);
        }
        /// <summary>
        /// 获取内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            if (InternalDb.TryGetValue(key, out ICacheDicValue item))
            {
                // 判断过期
                if (item.IsValid())
                {
                    if (item is ECacheDicValue<T>)
                    {
                        return (item as ECacheDicValue<T>).Value;
                    }
                    return (T)item.Value;
                }
                else
                {
#if NET40 || NET45
                    InternalDb.Remove(key);
#else
                    InternalDb.Remove(key, out _);
#endif
                }
            }
            return default(T);
        }
        /// <summary>
        /// 获取内容
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get(string key)
        {
            if (InternalDb.TryGetValue(key, out ICacheDicValue item))
            {
                // 判断过期
                if (item.IsValid())
                {
                    return item.Value;
                }
                else
                {
#if NET40 || NET45
                    InternalDb.Remove(key);
#else
                    InternalDb.Remove(key, out _);
#endif
                }
            }
            return null;
        }
        /// <summary>
        /// 获取内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public T GetOrAdd<T>(string key, Func<T> func)
        {
            if (InternalDb.TryGetValue(key, out ICacheDicValue item))
            {
                // 判断过期
                if (item.IsValid())
                {
                    if (item is ECacheDicValue<T>)
                    {
                        return (item as ECacheDicValue<T>).Value;
                    }
                    if (item.Value is T)
                    {
                        return (T)item.Value;
                    }
                }
                else
                {
#if NET40 || NET45
                    InternalDb.Remove(key);
#else
                    InternalDb.Remove(key, out _);
#endif
                }
            }
            T result = func();
            Task.Factory.StartNew(() => { Set(key, result); });
            return result;
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Remove(string key)
        {
#if NET40 || NET45
            InternalDb.Remove(key);
#else
            if (InternalDb.Remove(key, out ICacheDicValue item))
            {
                return item.Value;
            }
#endif
            return null;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Delete(string key)
        {
#if NET40 || NET45
            return InternalDb.Remove(key);
#else
            return InternalDb.Remove(key, out _);
#endif
        }
        /// <summary>
        /// 设置内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Set<T>(string key, T value)
        {
            InternalDb[key] = new ECacheDicValue<T>(key)
            {
                Expire = DateTime.Now.AddDays(1).Ticks,
                Value = value,
            };
            return true;
        }
        /// <summary>
        /// 设置内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expire"></param>
        /// <returns></returns>
        public bool Set<T>(string key, T value, DateTimeOffset expire)
        {
            InternalDb[key] = new ECacheDicValue<T>(key)
            {
                Expire = expire.Ticks,
                Value = value,
            };
            return true;
        }
        /// <summary>
        /// 设置内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="sliding"></param>
        /// <returns></returns>
        public bool Set<T>(string key, T value, TimeSpan sliding)
        {
            if (key == null) { return false; }
            InternalDb[key] = new ECacheDicValue<T>(key)
            {
                Expire = DateTime.Now.Ticks + sliding.Ticks,
                Sliding = sliding.Ticks,
                Value = value,
            };
            return true;
        }
        /// <summary>
        /// 清空
        /// </summary>
        /// <returns></returns>
        public bool Clear()
        {
            return InternalClear();
        }
        #endregion
        #region // String操作
        /// <summary>
        /// 字符串获取
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string StringGet(string key) => Get<string>(key);
        /// <summary>
        /// 字符串获取
        /// </summary>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public string StringGet(string key, Func<string> func) => GetOrAdd(key, func);
        /// <summary>
        /// 字符串获取
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string StringSet(string key, string value) => Set(key, value) ? value : default;
        /// <summary>
        /// 字符串获取
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expire"></param>
        /// <returns></returns>
        public string StringSet(string key, string value, DateTimeOffset expire) => Set(key, value, expire) ? value : default;
        /// <summary>
        /// 字符串获取
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="sliding"></param>
        /// <returns></returns>
        public string StringSet(string key, string value, TimeSpan sliding) => Set(key, value, sliding) ? value : default;
        #endregion
        #region // 内部操作
        /// <summary>
        /// 获取所有键
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetKeys()
        {
            return InternalDb.Keys;
        }
        /// <summary>
        /// 获取键
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        public IEnumerable<string> GetKeys(PagingResult paging)
        {
            var keys = GetKeys();
            paging.TotalCount = keys.Count();
            var items = keys.Skip(paging.Skip).Take(paging.Take);
            paging.Items = items;
            return items;
        }
        /// <summary>
        /// 获取键
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        public IEnumerable<string> GetKeys(string pattern, PagingResult paging)
        {
            var regex = new Regex(pattern);
            var keys = GetKeys().Where(s => regex.IsMatch(s));
            paging.TotalCount = keys.Count();
            var items = keys.Skip(paging.Skip).Take(paging.Take);
            paging.Items = items;
            return items;
        }
        #endregion
    }
}
