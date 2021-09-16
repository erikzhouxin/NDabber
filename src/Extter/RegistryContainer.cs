using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Cobber;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Data.Extter
{
    class RegistryContainer
    {
    }
    /// <summary>
    /// 注册提供者
    /// </summary>
    public static class RegistryProvider
    {
        /// <summary>
        /// 加载类
        /// </summary>
        public static ConcurrentDictionary<string, InnerLoader> Loaders { get; } = new ConcurrentDictionary<string, InnerLoader>();
        /// <summary>
        /// 静态构造
        /// </summary>
        static RegistryProvider()
        {
            //var st = new StackTrace();
            //var frame = st.GetFrame(1); // 获取导致它初始化的帧
            //if (frame != null)
            //{
            //    var method = frame.GetMethod(); // 获取方法
            //    if (method != null)
            //    {
            //        // 自动注册
            //        //SetService(new TypeLoader
            //        //{
            //        //    Assembly = method.Module.Assembly,
            //        //});
            //    }
            //}
        }
        /// <summary>
        /// 设置服务
        /// </summary>
        /// <param name="loader"></param>
        public static InnerLoader SetService(TypeLoader loader)
        {
            var inner = Loaders.GetOrAdd(loader.Domain, (k) => new InnerLoader(k));
            inner.Append(loader.Assembly);
            return inner;
        }
        /// <summary>
        /// 设置数据访问
        /// </summary>
        /// <param name="loader"></param>
        public static InnerLoader SetDataAccess(TypeLoader loader)
        {
            var tag = GetStoreTag(loader.DomainStore);
            var inner = Loaders.GetOrAdd(tag, (k) => new InnerLoader(k));
            inner.Append(loader.Assembly);
            return inner;
        }
        /// <summary>
        /// 存储标记
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        public static string GetStoreTag(StoreType store)
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
        #region // 获取DataAccess实现类
        /// <summary>
        /// 获取接口实例
        /// </summary>
        public static T GetDataAccess<T>(StoreModel store, bool isNew = false) => GetDataAccess<T>(store.DbType, store.ConnString, isNew);
        /// <summary>
        /// 获取接口实例
        /// </summary>
        /// <returns></returns>
        public static T GetDataAccess<T>(StoreType storeType, String connString, bool isNew = false) => GetDataAccess<T>(storeType, isNew, connString);
        /// <summary>
        /// 初始化参数获取类型
        /// </summary>
        public static T GetDataAccess<T>(StoreType storeType, params object[] args) => GetDataAccess<T>(storeType, false, args);
        /// <summary>
        /// 初始化参数获取类型
        /// </summary>
        public static T GetDataAccess<T>(StoreType storeType, bool isNew, params object[] args) => Get<T>(GetStoreTag(storeType), isNew, args);
        #endregion
        #region // 获取Service实现类
        /// <summary>
        /// 获取接口实例
        /// </summary>
        public static T GetService<T>(string domain, bool isNew = false) => GetService<T>(domain, isNew, null);
        /// <summary>
        /// 初始化参数获取类型
        /// </summary>
        public static T GetService<T>(string domain, params object[] args) => GetService<T>(domain, false, args);
        /// <summary>
        /// 初始化参数获取类型
        /// </summary>
        public static T GetService<T>(string domain, bool isNew, params object[] args) => Get<T>(domain, isNew, args);
        /// <summary>
        /// 获取通用实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="domain"></param>
        /// <param name="isNew"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T Get<T>(string domain, bool isNew, object[] args)
        {
            if (!Loaders.TryGetValue(domain, out InnerLoader loader))
            {
                throw new NotImplementedException($"未初始化当前【{domain}】域，请使用【{nameof(RegistryProvider)}】中的SetXXX进行设定");
            }
            try
            {
                return loader.Get<T>(isNew, args);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException($"未找到实现【{typeof(T).Name}】接口类，【{nameof(RegistryProvider)}】无法创建【{domain}】下的实例", ex);
            }
        }
        #endregion
        #region // 内部类
        /// <summary>
        /// 类型加载
        /// </summary>
        public class TypeLoader
        {
            /// <summary>
            /// 域
            /// </summary>
            public string Domain { get; set; }
            /// <summary>
            /// 存储域
            /// </summary>
            public StoreType DomainStore { get; set; }
            /// <summary>
            /// 程序集
            /// </summary>
            public Assembly Assembly { get; set; }
            private string _filePath;
            /// <summary>
            /// 程序集目录
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
            private AssemblyName _assemblyName;
            /// <summary>
            /// 程序集名称
            /// </summary>
            public AssemblyName AssemblyName
            {
                get => _assemblyName;
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
                get => _assemblyName.Name;
                set
                {
                    try
                    {
                        AssemblyName = new AssemblyName(value);
                    }
                    catch { }
                }
            }
            /// <summary>
            /// 全称特征
            /// </summary>
            public String Regex { get; set; }
        }
        /// <summary>
        /// 类型加载器
        /// </summary>
        public class InnerLoader
        {
            /// <summary>
            /// 映射字典
            /// </summary>
            public ConcurrentDictionary<String, InnerMapper> Mapping { get; } = new ConcurrentDictionary<String, InnerMapper>();
            /// <summary>
            /// 内部类型
            /// </summary>
            public List<Type> Types { get; } = new List<Type>();
            /// <summary>
            /// 数据库类型
            /// </summary>
            private string _dbName;
            /// <summary>
            /// 构造
            /// </summary>
            /// <param name="dbName"></param>
            public InnerLoader(string dbName)
            {
                _dbName = dbName;
            }
            /// <summary>
            /// 获取映射类实例
            /// </summary>
            /// <returns></returns>
            public T Get<T>(bool isNew, params object[] args)
            {
                var type = typeof(T);
                string key = $"{type.FullName}_{string.Join("_", args)}";
                return Get<T>(type, key, isNew, args);
            }

            private T Get<T>(Type type, string key, bool isNew, params object[] args)
            {
                var mapper = Mapping.GetOrAdd(key, (k) => GetMapper(Types, type, _dbName, args));
                return isNew || mapper.Instance == null ? (T)Activator.CreateInstance(mapper.Type, args) : (T)mapper.Instance;
            }

            static InnerMapper GetMapper(IEnumerable<Type> types, Type type, string dbName, params object[] args)
            {
                if (TryGetType(types, type, out Type findType))
                {
                    return new InnerMapper
                    {
                        Type = findType,
                        Interface = type,
                        Instance = Activator.CreateInstance(findType, args),
                    };
                }
                var nameTag = $"{dbName}.";
                foreach (var dbFile in GetDllFiles($"{dbName}*.dll"))
                {
                    try
                    {
                        var assName = AssemblyName.GetAssemblyName(dbFile);
                        if (TryGetType(assName, type, nameTag, out findType))
                        {
                            return GetMapper(type, findType, args);
                        }
                    }
                    catch { }
                }
                foreach (var assName in GetRefAssemblies($"({dbName})[\\w_\\.]*$"))
                {
                    try
                    {
                        if (TryGetType(assName, type, nameTag, out findType))
                        {
                            return GetMapper(type, findType, args);
                        }
                    }
                    catch { }
                }
                throw new Exception("多情自古空余恨，此恨绵绵无绝期！");
            }

            private static InnerMapper GetMapper(Type type, Type findType, params object[] args)
            {
                return new InnerMapper
                {
                    Type = findType,
                    Interface = type,
                    Instance = Activator.CreateInstance(findType, args),
                };
            }

            private static IEnumerable<string> GetDllFiles(string dbTag)
            {
                var currentAss = Assembly.GetExecutingAssembly().GetName().Name.Split('.');
                var files = new List<String>();
                if (currentAss.Length > 1)
                {
                    if (currentAss.Length > 2)
                    {
                        files.AddRange(GetFiles(currentAss[0] + "." + currentAss[1] + "." + dbTag));
                    }
                    files.AddRange(GetFiles(currentAss[0] + "." + dbTag));
                }
                else
                {
                    files.AddRange(GetFiles(dbTag));
                }
                return files.Distinct();
            }

            private static IEnumerable<AssemblyName> GetRefAssemblies(string dbTag)
            {
                var currentAss = Assembly.GetExecutingAssembly().GetName().Name.Split('.');
                string startText;
                if (currentAss.Length > 2)
                {
                    startText = "(?i)^((" + currentAss[0] + @"\." + currentAss[1] + @"\.)|(" + currentAss[0] + @"\.))";
                }
                else if (currentAss.Length > 1)
                {
                    startText = "(?i)^(" + currentAss[0] + @"\.)";
                }
                else
                {
                    startText = "(?i)^";
                }
                var regex = new Regex(startText + dbTag);
                var libs = AppDomain.CurrentDomain.GetAssemblies();
                return libs.Select(s => s.GetName()).Where(s => regex.IsMatch(s.Name));
            }
            private static bool TryGetType(AssemblyName assName, Type type, string nameTag, out Type findType)
            {
                var ass = Assembly.Load(assName);
                var fillterTypes = ass.GetTypes().Where(s => Contains(s.FullName, nameTag));
                if (fillterTypes.Count() > 0)
                {
                    return TryGetType(fillterTypes, type, out findType);
                }
                findType = type;
                return false;
            }

            private static bool TryGetType(IEnumerable<Type> types, Type type, out Type findType)
            {
                findType = type;
                foreach (var item in types)
                {
                    if (!item.IsClass) { continue; }
                    if (!type.IsAssignableFrom(item))
                    {
                        if (type.Name.Equals("I" + item.Name)) { findType = item; } // 名称类似
                        continue;
                    }
                    findType = item;
                    // 如果类名是接口命名的子字符串,直接结束
                    if (type.Name.Contains(item.Name)) { return true; }
                }
                return !type.Equals(findType);
            }
            private static IEnumerable<string> GetFiles(string searchPattern)
            {
                foreach (var item in Directory.GetFiles(Environment.CurrentDirectory, searchPattern, SearchOption.AllDirectories))
                {
                    yield return item;
                }
            }

            static bool Contains(string source, string toCheck, StringComparison comp = StringComparison.OrdinalIgnoreCase)
            {
                return source.IndexOf(toCheck, comp) >= 0;
            }
            /// <summary>
            /// 添加程序集
            /// </summary>
            /// <param name="assembly"></param>
            public void Append(Assembly assembly)
            {
                try
                {
                    if (assembly != null)
                    {
                        foreach (var item in assembly.GetTypes())
                        {
                            if (item.IsClass)
                            {
                                Types.Add(item);
                            }
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
        }
        /// <summary>
        /// 提供者映射
        /// </summary>
        public struct InnerMapper
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
}
