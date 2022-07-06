using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Cobber;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Extter
{
    /// <summary>
    /// 注册容器
    /// </summary>
    public static class RegistryContainer
    {
    }
    /// <summary>
    /// 注册提供者
    /// 注意:
    /// 1.TagName下最好不要有相同的实现类,因为可能不知道使用哪一个
    /// 2.未进行性能优化,此加载为一次性加载,请自己配置按需加载
    /// 3.可以使用RegistryProviderProxy.Test进行按需加载获取代理
    /// </summary>
    public class RegistryProvider
    {
        /// <summary>
        /// 加载类
        /// </summary>
        internal static ConcurrentDictionary<string, InnerLoader> Loaders { get; } = new();
        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="loader"></param>
        public static void Set(TypeLoader loader)
        {
            var inner = Loaders.GetOrAdd(loader.Domain, (k) => new InnerLoader());
            inner.Append(loader.TagName, loader.Assembly);
        }
        /// <summary>
        /// 异步设置
        /// </summary>
        /// <param name="loader"></param>
        public static async Task SetAsync(TypeLoader loader)
        {
            await new Task(() => Set(loader));
        }
        /// <summary>
        /// 设置服务
        /// </summary>
        /// <param name="loader"></param>
        public static void SetService(TypeLoader loader) => Set(loader);
        /// <summary>
        /// 设置服务[单一实现]
        /// </summary>
        /// <param name="assembly"></param>
        public static void SetService(Assembly assembly) => SetService("Services", assembly);
        /// <summary>
        /// 设置服务
        /// </summary>
        /// <param name="domain">缺省TagName,Service所在命名空间</param>
        /// <param name="assembly"></param>
        public static void SetService(string domain, Assembly assembly)
        {
            Set(new TypeLoader(domain) { Assembly = assembly });
        }
        /// <summary>
        /// 设置服务(Services为域)[Services.{T}Impl]
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="svrType"></param>
        /// <param name="hasBase"></param>
        public static void SetServiceImpl<T>(Assembly assembly, T svrType, bool hasBase = true)
            where T : struct, Enum
            => SetServiceImpl("Services", assembly, svrType, hasBase);
        /// <summary>
        /// 设置服务[{domain}.{T}Impl]
        /// </summary>
        /// <param name="domain">缺省TagName,Service所在命名空间</param>
        /// <param name="assembly"></param>
        /// <param name="svrType"></param>
        /// <param name="hasBase"></param>
        public static void SetServiceImpl<T>(string domain, Assembly assembly, T svrType, bool hasBase = true)
            where T : struct, Enum
        {
            if (hasBase)
            {
                Set(new TypeLoader(domain) { TagName = $"{domain}.BaseImpl", Assembly = assembly });
            }
            var svrTypeAttr = NEnumerable<T>.GetFromEnum(svrType);
            Set(new TypeLoader(domain) { TagName = $"{domain}.{svrTypeAttr.EnumName}Impl", Assembly = assembly });
        }
        /// <summary>
        /// 设置服务
        /// </summary>
        /// <param name="domain">缺省TagName,Service所在命名空间</param>
        /// <param name="assembly"></param>
        /// <param name="tagNames">搜索命名空间</param>
        public static void SetService(string domain, Assembly assembly, params string[] tagNames)
        {
            if (tagNames == null || tagNames.Length == 0)
            {
                Set(new TypeLoader(domain) { Assembly = assembly });
                return;
            }
            foreach (var tagName in tagNames)
            {
                Set(new TypeLoader(domain) { TagName = tagName, Assembly = assembly });
            }
        }
        /// <summary>
        /// 设置服务
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="assemblies"></param>
        public static void SetService(string domain, params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                Set(new TypeLoader(domain) { Assembly = assembly });
            }
        }
        /// <summary>
        /// 设置数据访问
        /// </summary>
        /// <param name="loader"></param>
        public static void SetDataAccess(TypeLoader loader) => Set(loader);
        /// <summary>
        /// 设置数据访问
        /// </summary>
        /// <param name="storeType"></param>
        /// <param name="assembly"></param>
        public static void SetDataAccess(StoreType storeType, Assembly assembly) => Set(new TypeLoader(storeType) { Assembly = assembly });
        /// <summary>
        /// 设置数据访问
        /// </summary>
        /// <param name="storeType"></param>
        /// <param name="assemblies"></param>
        public static void SetDataAccess(StoreType storeType, params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                Set(new TypeLoader(storeType) { Assembly = assembly });
            }
        }
        /// <summary>
        /// 存储标记
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        public static string GetStoreDomain(StoreType store)
        {
            return store switch
            {
                StoreType.SqlServer => $"Db{nameof(StoreType.SqlServer)}",
                StoreType.MySQL => $"Db{nameof(StoreType.MySQL)}",
                StoreType.Oracle => $"Db{nameof(StoreType.Oracle)}",
                StoreType.PostgreSQL => $"Db{nameof(StoreType.PostgreSQL)}",
                StoreType.Redis => $"Db{nameof(StoreType.Redis)}",
                StoreType.Access => $"Db{nameof(StoreType.Access)}",
                StoreType.Excel => $"Db{nameof(StoreType.Excel)}",
                StoreType.Xml => $"Db{nameof(StoreType.Xml)}",
                StoreType.Memory => $"Db{nameof(StoreType.Memory)}",
                StoreType.SQLite => $"Db{nameof(StoreType.SQLite)}",
                _ => $"DataAccess",
            };
        }
        /// <summary>
        /// 获取接口实例
        /// </summary>
        public static T Get<T>(string domain, bool isNew = false) => Get<T>(domain, isNew, BuilderArray<object>.Empty);
        /// <summary>
        /// 获取数据访问接口实例
        /// </summary>
        public static T Get<T>(StoreType storeType, bool isNew, params object[] args) => Get<T>(GetStoreDomain(storeType), isNew, args);
        /// <summary>
        /// 获取数据访问接口实例
        /// </summary>
        public static T Get<T>(StoreModel store, bool isNew = false) => Get<T>(GetStoreDomain(store.DbType), isNew, store.ConnString);
        /// <summary>
        /// 获取通用实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="domain"></param>
        /// <param name="isNew"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T Get<T>(string domain, bool isNew, params object[] args)
        {
            if (!Loaders.TryGetValue(domain, out InnerLoader loader))
            {
                throw new NotImplementedException($"未初始化当前【{domain}】域，请使用【{nameof(RegistryProvider)}】中的SetXXX进行设定");
            }
            try
            {
                return loader.Get<T>(isNew, args ?? new object[0]);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException($"未找到实现【{typeof(T).Name}】接口类，【{nameof(RegistryProvider)}】无法创建【{domain}】下的实例", ex);
            }
        }
        #region // 内部类
        /// <summary>
        /// 类型加载
        /// </summary>
        public class TypeLoader
        {
            /// <summary>
            /// 域
            /// </summary>
            public string Domain { get; }
            /// <summary>
            /// 一般构造
            /// </summary>
            /// <param name="domain"></param>
            public TypeLoader(string domain)
            {
                Domain = domain;
                TagName = domain;
            }
            /// <summary>
            /// 存储构造
            /// </summary>
            /// <param name="storeType"></param>
            public TypeLoader(StoreType storeType) : this(GetStoreDomain(storeType))
            {
            }
            /// <summary>
            /// 全称特征
            /// </summary>
            public String TagName { get; set; }
            /// <summary>
            /// 程序集
            /// </summary>
            public Assembly Assembly { get; set; }
            private string _filePath;
            /// <summary>
            /// 程序集文件
            /// </summary>
            public string FilePath
            {
                get => _filePath;
                set
                {
                    _filePath = Path.GetFullPath(value);
                    if (File.Exists(_filePath))
                    {
                        try
                        {
                            Assembly = Assembly.LoadFrom(_filePath);
                        }
                        catch { }
                    }
                    else
                    {
                        Assembly = null;
                    }
                }
            }
            /// <summary>
            /// 程序集名称
            /// </summary>
            public AssemblyName AssemblyName
            {
                get => Assembly?.GetName();
                set
                {
                    try
                    {
                        Assembly = Assembly.Load(value);
                    }
                    catch { }
                }
            }
            /// <summary>
            /// 程序集短名称
            /// </summary>
            public string AssemblyShortName
            {
                get => AssemblyName?.Name;
                set
                {
                    try
                    {
                        AssemblyName = new AssemblyName(value);
                    }
                    catch { }
                }
            }
        }
        /// <summary>
        /// 类型加载器
        /// </summary>
        internal class InnerLoader
        {
            private object _locked = new object();
            /// <summary>
            /// 映射字典
            /// </summary>
            public ConcurrentDictionary<String, InnerMapper> Mapping { get; } = new();
            /// <summary>
            /// 内部类型
            /// </summary>
            public HashSet<Type> Types { get; } = new();
            /// <summary>
            /// 获取映射类实例
            /// </summary>
            /// <returns></returns>
            public T Get<T>(bool isNew, params object[] args)
            {
                var type = typeof(T);
                string key = $"{type.FullName}_{string.Join("_", args)}";
                var mapper = Mapping.GetOrAdd(key, (k) => GetMapper(type, args));
                return isNew || mapper.Instance == null ? (T)Activator.CreateInstance(mapper.Type, args) : (T)mapper.Instance;
            }

            InnerMapper GetMapper(Type type, params object[] args)
            {
                var findType = type;
                foreach (var item in Types)
                {
                    if (!item.IsClass) { continue; }
                    if (!type.IsAssignableFrom(item))
                    {
                        if (type.Name.Equals("I" + item.Name)) { findType = item; } // 名称类似
                        continue;
                    }
                    findType = item;
                    // 如果类名是接口命名的子字符串,直接结束
                    if (type.Name.Contains(item.Name)) { break; }
                }
                if (!type.Equals(findType))
                {
                    return new InnerMapper
                    {
                        Type = findType,
                        Interface = type,
                        Instance = Activator.CreateInstance(findType, args),
                    };
                }
                throw new Exception("多情自古空余恨，此恨绵绵无绝期！");
            }
            /// <summary>
            /// 添加程序集
            /// </summary>
            /// <param name="tag"></param>
            /// <param name="assembly"></param>
            public void Append(string tag, Assembly assembly)
            {
                if (assembly == null) { return; }
                if (Monitor.TryEnter(_locked, TimeSpan.FromSeconds(5))) // HashSet不加锁时并发会出现null值
                {
                    try
                    {
                        foreach (var type in assembly.GetTypes())
                        {
                            if (type.IsClass && type.FullName.IndexOf(tag, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                Types.Add(type);
                            }
                        }
                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); }
                    finally { Monitor.Exit(_locked); }
                }
            }
        }
        /// <summary>
        /// 提供者映射
        /// </summary>
        internal struct InnerMapper
        {
            /// <summary>
            /// 类型
            /// </summary>
            public Type Type;
            /// <summary>
            /// 接口
            /// </summary>
            public Type Interface;
            /// <summary>
            /// 实例
            /// </summary>
            public Object Instance;
        }
        #endregion
    }
    /// <summary>
    /// 注册提供者代理
    /// </summary>
    public class RegistryProviderProxy
    {
        internal static ConcurrentDictionary<StoreType, RegistryProviderProxy> TestStoreDic { get; } = new();
        /// <summary>
        /// 测试
        /// </summary>
        /// <param name="store"></param>
        /// <param name="Regist"></param>
        /// <returns></returns>
        public static RegistryProviderProxy Test(StoreType store, Action<StoreType> Regist)
        {
            return TestStoreDic.GetOrAdd(store, (k) =>
            {
                Regist(k);
                return new RegistryProviderProxy();
            });
        }
        /// <summary>
        /// 测试
        /// </summary>
        /// <param name="store"></param>
        /// <param name="Regist"></param>
        /// <returns></returns>
        public static RegistryProviderProxy Test(StoreType store, Func<RegistryProvider.TypeLoader> Regist)
        {
            return TestStoreDic.GetOrAdd(store, (k) =>
            {
                Regist();
                return new RegistryProviderProxy();
            });
        }
        /// <summary>
        /// 内部构造
        /// </summary>
        protected RegistryProviderProxy()
        {

        }
        #region // 获取DataAccess实现类
        /// <summary>
        /// 获取接口实例
        /// </summary>
        public T GetDataAccess<T>(StoreModel store, bool isNew = false) => GetDataAccess<T>(store.DbType, isNew, store.ConnString);
        /// <summary>
        /// 获取接口实例
        /// </summary>
        /// <returns></returns>
        public T GetDataAccess<T>(StoreType storeType, String connString, bool isNew = false) => GetDataAccess<T>(storeType, isNew, connString);
        /// <summary>
        /// 初始化参数获取类型
        /// </summary>
        public T GetDataAccess<T>(StoreType storeType, params object[] args) => GetDataAccess<T>(storeType, false, args);
        /// <summary>
        /// 初始化参数获取类型
        /// </summary>
        public T GetDataAccess<T>(StoreType storeType, bool isNew, params object[] args) => RegistryProvider.Get<T>(RegistryProvider.GetStoreDomain(storeType), isNew, args);
        #endregion
        #region // 获取Service实现类
        /// <summary>
        /// 获取接口实例
        /// </summary>
        public T GetService<T>(string domain, bool isNew = false) => RegistryProvider.Get<T>(domain, isNew, BuilderArray<object>.Empty);
        /// <summary>
        /// 初始化参数获取类型
        /// </summary>
        public T GetService<T>(string domain, params object[] args) => RegistryProvider.Get<T>(domain, false, args);
        /// <summary>
        /// 初始化参数获取类型
        /// </summary>
        public T GetService<T>(string domain, bool isNew, params object[] args) => RegistryProvider.Get<T>(domain, isNew, args);
        #endregion
    }
}
