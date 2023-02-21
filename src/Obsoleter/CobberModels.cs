using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// 自动Microsoft SQL Server创建者
    /// 依赖于[DbMarkable/DbAutoable]
    /// 推荐使用[AutoSqlServerBuilder]
    /// </summary>
    [Obsolete("替代方案:AutoSqlServerBuilder")]
    public class AutoMssqlBuilder
    {
        /// <summary>
        /// 创建SQL Builder
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static AutoSqlBuilder Builder(Type type) => AutoSqlServerBuilder.EntitySqlDic.GetOrAdd(type, (k) => AutoSqlServerBuilder.CreateSqlModel(k));
        /// <summary>
        /// 创建SQL Builder
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static AutoSqlBuilder Builder<T>() => AutoSqlServerBuilder.EntitySqlDic.GetOrAdd(typeof(T), (k) => AutoSqlServerBuilder.CreateSqlModel(k));
    }
    /// <summary>
    /// 枚举显示泛型类
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    [Obsolete("替代方案:NEnumerable<TEnum>")]
    public class EDisplayAttr<TEnum> : NEnumerable<TEnum>
        where TEnum : struct, Enum
    {
        private EDisplayAttr() : base(default(TEnum)) { }
    }
    public partial class UserPassword
    {
        /// <summary>
        /// 将字节数组转换成16进制字符串
        /// </summary>
        /// <param name="hashData">字节数组</param>
        /// <returns>16进制字符串(大写字母)</returns>
        [Obsolete("替代方案:UserCrypto.GetHexString")]
        public static string GetHexString(byte[] hashData) => UserCrypto.GetHexString(hashData, DefaultCase);
        /// <summary>
        /// 将字节数组转换成16进制字符串
        /// </summary>
        /// <param name="hashData">字节数组</param>
        /// <param name="isLower">是小写</param>
        /// <returns>16进制字符串(大写字母)</returns>
        [Obsolete("替代方案:UserCrypto.GetHexString")]
        public static string GetHexString(byte[] hashData, bool isLower) => UserCrypto.GetHexString(hashData, isLower);
        /// <summary>
        /// 将字节数组转换成16进制字符串
        /// </summary>
        /// <param name="hashData">字节数组</param>
        /// <returns>16进制字符串(大写字母)</returns>
        [Obsolete("替代方案:UserCrypto.GetHexString")]
        public static string GetByte16String(byte[] hashData) => UserCrypto.GetHexString(hashData, false);
        /// <summary>
        /// 原码字符
        /// </summary>
        [Obsolete("替代方案:Origin")]
        public string OPass { get => Origin; }
        /// <summary>
        /// 加密字符
        /// </summary>
        [Obsolete("替代方案:Hash")]
        public string HPass { get => Hash; }
    }

    /// <summary>
    /// 反射对象成员访问
    /// 支持object访问
    /// 能效等级四级,不推荐使用
    /// </summary>
    [Obsolete("替代方案:PropertyAccess")]
    public class MemberReflectionAccessor : IMemberAccessor
    {
        /// <summary>
        /// 获取对象的成员属性值
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="instance">实例对象</param>
        /// <param name="memberName">成员名称</param>
        /// <returns>成员值</returns>
        public object GetValue<T>(T instance, string memberName)
        {
            var type = instance == null ? typeof(T) : instance.GetType();
            var propertyInfo = type.GetProperty(memberName);
            return propertyInfo?.GetValue(instance, null);
        }
        /// <summary>
        /// 设置对象的成员属性值
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="instance">实例对象</param>
        /// <param name="memberName">成员名称</param>
        /// <param name="newValue">成员值</param>
        public void SetValue<T>(T instance, string memberName, object newValue)
        {
            var type = instance == null ? typeof(T) : instance.GetType();
            var propertyInfo = type.GetProperty(memberName);
            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                propertyInfo.SetValue(instance, newValue, null);
            }
        }
        /// <summary>
        /// 比较
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="ignores"></param>
        /// <returns></returns>
        public bool Compare<T>(T source, T target, params string[] ignores)
        {
            var property = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var item in property)
            {
                if (ignores.Any(s => item.Name.Equals(s, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }
                if (item.GetValue(source, null) != item.GetValue(target, null))
                {
                    return false;
                }
            }
            return true;
        }
    }
    /// <summary>
    /// 委托表达式树成员访问
    /// 支持object访问
    /// 能效等级三级,不推荐使用
    /// </summary>
    [Obsolete("替代方案:PropertyAccess")]
    public class MemberDelegatedExpressionAccessor : IMemberAccessor
    {
        /// <summary>
        /// 获取值字典
        /// </summary>
        protected Dictionary<string, Func<object, object>> GetValueDic = new();
        /// <summary>
        /// 设置值字典
        /// </summary>
        protected Dictionary<string, Action<object, object>> SetValueDic = new();
        /// <summary>
        /// 获取对象的成员属性值
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="instance">实例对象</param>
        /// <param name="memberName">成员名称</param>
        /// <returns>成员值</returns>
        public object GetValue<T>(T instance, string memberName)
        {
            var type = instance == null ? typeof(T) : instance.GetType();
            var GetMember = GetValue(type, memberName);
            return GetMember(instance);
        }
        /// <summary>
        /// 获取类型的属性
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public Func<object, object> GetValue(Type type, string memberName)
        {
            var key = type.FullName + memberName;
            Func<object, object> getValueDelegate;
            GetValueDic.TryGetValue(key, out getValueDelegate);
            if (getValueDelegate == null)
            {
                var info = type.GetProperty(memberName);
                var target = Expression.Parameter(typeof(object), "target");
                var getMethod = info.GetGetMethod();
                Expression body;
                if (getMethod.IsStatic)
                {
                    body = Expression.Convert(Expression.Property(null, info), typeof(object));
                }
                else
                {
                    body = Expression.Convert(Expression.Property(Expression.Convert(target, type), info), typeof(object));
                }
                var getter = Expression.Lambda(typeof(Func<object, object>), body, target);
                getValueDelegate = (Func<object, object>)getter.Compile();
                GetValueDic.Add(key, getValueDelegate);
            }

            return getValueDelegate;
        }
        /// <summary>
        /// 比较
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="ignores"></param>
        /// <returns></returns>
        public bool Compare<T>(T source, T target, params string[] ignores)
        {
            var type = typeof(T);
            var property = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var item in property)
            {
                if (ignores.Any(s => item.Name.Equals(s, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }
                var func = GetValue(type, item.Name);
                if (func(source) != func(target))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 设置对象的成员属性值
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="instance">实例对象</param>
        /// <param name="memberName">成员名称</param>
        /// <param name="newValue">成员值</param>
        public void SetValue<T>(T instance, string memberName, object newValue)
        {
            var type = instance == null ? typeof(T) : instance.GetType();
            var setValue = SetValue<T>(type, memberName);
            setValue(instance, newValue);
        }
        /// <summary>
        /// 设置对象的成员方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public Action<object, object> SetValue<T>(Type type, string memberName)
        {
            var key = type.FullName + memberName;
            Action<object, object> setValueDelegate;
            SetValueDic.TryGetValue(key, out setValueDelegate);
            if (setValueDelegate == null)
            {
                var info = type.GetProperty(memberName);
                var target = Expression.Parameter(typeof(object), "target");
                var value = Expression.Parameter(typeof(object), "value");
                Expression body;
                if (info.CanWrite)
                {
                    var setMethod = info.GetSetMethod();
                    if (setMethod.IsStatic)
                    {
                        body = Expression.Assign(Expression.Property(null, info), Expression.Convert(value, info.PropertyType));
                    }
                    else
                    {
                        body = Expression.Assign(Expression.Property(Expression.Convert(target, type), info), Expression.Convert(value, info.PropertyType));
                    }
                }
                else
                {
                    body = Expression.Empty();
                }
                var setter = Expression.Lambda(typeof(Action<object, object>), body, target, value);
                setValueDelegate = (Action<object, object>)setter.Compile();
                SetValueDic.Add(key, setValueDelegate);
            }
            return setValueDelegate;
        }
    }
    /// <summary>
    /// 委托反射成员访问
    /// 支持object访问
    /// 能效等级三级,不推荐使用
    /// </summary>
    [Obsolete("替代方案:PropertyAccess")]
    public class MemberDelegatedReflectionAccessor : IMemberAccessor
    {
        private static Dictionary<string, INamedMemberAccessor> AccessorCache = new();
        /// <summary>
        /// 获取对象的成员属性值
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="instance">实例对象</param>
        /// <param name="memberName">成员名称</param>
        /// <returns>成员值</returns>
        public object GetValue<T>(T instance, string memberName)
        {
            var type = instance == null ? typeof(T) : instance.GetType();
            return FindAccessor(type, memberName).GetValue(instance);
        }
        /// <summary>
        /// 设置对象的成员属性值
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="instance">实例对象</param>
        /// <param name="memberName">成员名称</param>
        /// <param name="newValue">成员值</param>
        public void SetValue<T>(T instance, string memberName, object newValue)
        {
            var type = instance == null ? typeof(T) : instance.GetType();
            FindAccessor(type, memberName).SetValue(instance, newValue);
        }

        private INamedMemberAccessor FindAccessor(Type type, string memberName)
        {
            var key = type.FullName + memberName;
            INamedMemberAccessor accessor;
            AccessorCache.TryGetValue(key, out accessor);
            if (accessor == null)
            {
                var propertyInfo = type.GetProperty(memberName);
                accessor = Activator.CreateInstance(typeof(PropertyAccessor<,>).MakeGenericType(type, propertyInfo.PropertyType), type, memberName) as INamedMemberAccessor;
                AccessorCache.Add(key, accessor);
            }
            return accessor;
        }
        /// <summary>
        /// 比较
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="ignores"></param>
        /// <returns></returns>
        public bool Compare<T>(T source, T target, params string[] ignores)
        {
            var type = typeof(T);
            var property = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var item in property)
            {
                if (ignores.Any(s => item.Name.Equals(s, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }
                var func = FindAccessor(type, item.Name);
                if (func.GetValue(source) != func.GetValue(target))
                {
                    return false;
                }
            }
            return true;
        }
        internal interface INamedMemberAccessor
        {
            object GetValue(object instance);

            void SetValue(object instance, object newValue);
        }
        internal class PropertyAccessor<T, P> : INamedMemberAccessor
        {
            private Func<T, P> GetValueDelegate;
            private Action<T, P> SetValueDelegate;

            public PropertyAccessor(Type type, string propertyName)
            {
                var propertyInfo = type.GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    GetValueDelegate = (Func<T, P>)Delegate.CreateDelegate(typeof(Func<T, P>), propertyInfo.GetGetMethod());
                    SetValueDelegate = (Action<T, P>)Delegate.CreateDelegate(typeof(Action<T, P>), propertyInfo.GetSetMethod());
                }
            }

            public object GetValue(object instance)
            {
                return GetValueDelegate((T)instance);
            }

            public void SetValue(object instance, object newValue)
            {
                SetValueDelegate((T)instance, (P)newValue);
            }
        }
    }
    /// <summary>
    /// 动态生成方法成员访问
    /// 泛型不支持object
    /// 能效等级二级,推荐使用
    /// </summary>
    [Obsolete("替代方案:PropertyAccess")]
    public class MemberExpressionAccessor : IMemberAccessor
    {
        /// <summary>
        /// 获取对象的成员属性值
        /// </summary>
        /// <seealso cref="MemberExpressionAccessor{T}"/>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="instance">实例对象</param>
        /// <param name="memberName">成员名称</param>
        /// <returns>成员值</returns>
        public object GetValue<T>(T instance, string memberName)
        {
            return MemberExpressionAccessor<T>.GetValue(instance, memberName);
        }

        /// <summary>
        /// 设置对象的成员属性值
        /// </summary>
        /// <seealso cref="MemberExpressionAccessor{T}"/>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="instance">实例对象</param>
        /// <param name="memberName">成员名称</param>
        /// <param name="newValue">成员值</param>
        public void SetValue<T>(T instance, string memberName, object newValue)
        {
            MemberExpressionAccessor<T>.SetValue(instance, memberName, newValue);
        }
        /// <summary>
        /// 比较
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="ignores"></param>
        /// <returns></returns>
        public bool Compare<T>(T source, T target, params string[] ignores)
        {
            var type = typeof(T);
            var property = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            Func<T, string, object> Get = MemberExpressionAccessor<T>.GetValue;
            foreach (var item in property)
            {
                if (ignores.Any(s => item.Name.Equals(s, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }
                if (Get(source, item.Name) != Get(target, item.Name))
                {
                    return false;
                }
            }
            return true;
        }
    }
    /// <summary>
    /// 表达式树生成case字段泛型获取
    /// 能效等级二级,推荐使用
    /// 泛型不支持object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Obsolete("替代方案:PropertyAccess<T>")]
    public static class MemberExpressionAccessor<T>
    {
        /// <summary>
        /// 获取值(instance,memberName,return)
        /// </summary>
        public static Func<T, string, object> GetValue;
        /// <summary>
        /// 设置值(instance,memberName,newValue)
        /// </summary>
        public static Action<T, string, object> SetValue;
        /// <summary>
        /// 类型
        /// </summary>
        public static Type Type;
        static MemberExpressionAccessor()
        {
            Type = typeof(T);

            //GetValue = GenerateGetValue();
            //SetValue = GenerateSetValue();

            GetValue = GenerateGetValue2();
            SetValue = GenerateSetValue2();
        }
        private static Func<T, string, object> GenerateGetValue()
        {
            var instance = Expression.Parameter(Type, "instance");
            var memberName = Expression.Parameter(typeof(string), "memberName");
            var nameHash = Expression.Variable(typeof(int), "nameHash");
            var calHash = Expression.Assign(nameHash, Expression.Call(memberName, typeof(object).GetMethod("GetHashCode")));
            var cases = new List<SwitchCase>();
            foreach (var propertyInfo in Type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var property = Expression.Property(Expression.Convert(instance, Type), propertyInfo.Name);
                var propertyHash = Expression.Constant(propertyInfo.Name.GetHashCode(), typeof(int));

                cases.Add(Expression.SwitchCase(Expression.Convert(property, typeof(object)), propertyHash));
            }
            foreach (var propertyInfo in Type.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            {
                var property = Expression.Property(null, Type, propertyInfo.Name);
                var propertyHash = Expression.Constant(propertyInfo.Name.GetHashCode(), typeof(int));
                cases.Add(Expression.SwitchCase(Expression.Convert(property, typeof(object)), propertyHash));
            }
            var switchEx = Expression.Switch(nameHash, Expression.Constant(null), cases.ToArray());
            var methodBody = Expression.Block(typeof(object), new[] { nameHash }, calHash, switchEx);

            return Expression.Lambda<Func<T, string, object>>(methodBody, instance, memberName).Compile();
        }
        private static Action<T, string, object> GenerateSetValue()
        {
            var instance = Expression.Parameter(Type, "instance");
            var memberName = Expression.Parameter(typeof(string), "memberName");
            var newValue = Expression.Parameter(typeof(object), "newValue");
            var nameHash = Expression.Variable(typeof(int), "nameHash");
            var calHash = Expression.Assign(nameHash, Expression.Call(memberName, typeof(object).GetMethod("GetHashCode")));
            var cases = new List<SwitchCase>();
            foreach (var propertyInfo in Type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (!propertyInfo.CanWrite) { continue; }
                var property = Expression.Property(Expression.Convert(instance, Type), propertyInfo.Name);
                var setValue = Expression.Assign(property, Expression.Convert(newValue, propertyInfo.PropertyType));
                var propertyHash = Expression.Constant(propertyInfo.Name.GetHashCode(), typeof(int));

                cases.Add(Expression.SwitchCase(Expression.Convert(setValue, typeof(object)), propertyHash));
            }
            foreach (var propertyInfo in Type.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            {
                if (!propertyInfo.CanWrite) { continue; }
                var property = Expression.Property(null, propertyInfo);
                var setValue = Expression.Assign(property, Expression.Convert(newValue, propertyInfo.PropertyType));
                var propertyHash = Expression.Constant(propertyInfo.Name.GetHashCode(), typeof(int));

                cases.Add(Expression.SwitchCase(Expression.Convert(setValue, typeof(object)), propertyHash));
            }
            var switchEx = Expression.Switch(nameHash, Expression.Constant(null), cases.ToArray());
            var methodBody = Expression.Block(typeof(object), new[] { nameHash }, calHash, switchEx);

            return Expression.Lambda<Action<T, string, object>>(methodBody, instance, memberName, newValue).Compile();
        }

        private static Func<T, string, object> GenerateGetValue2()
        {
            var instance = Expression.Parameter(Type, "instance");
            var memberName = Expression.Parameter(typeof(string), "memberName");
            var cases = new List<SwitchCase>();
            foreach (var propertyInfo in Type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var property = Expression.Property(Expression.Convert(instance, Type), propertyInfo.Name);
                var propertyHash = Expression.Constant(propertyInfo.Name, typeof(string));
                cases.Add(Expression.SwitchCase(Expression.Convert(property, typeof(object)), propertyHash));
            }
            foreach (var propertyInfo in Type.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            {
                var property = Expression.Property(null, Type, propertyInfo.Name);
                var propertyHash = Expression.Constant(propertyInfo.Name, typeof(string));
                cases.Add(Expression.SwitchCase(Expression.Convert(property, typeof(object)), propertyHash));
            }
            return Expression.Lambda<Func<T, string, object>>(Expression.Switch(memberName, Expression.Constant(null), cases.ToArray()), instance, memberName).Compile();
        }
        private static Action<T, string, object> GenerateSetValue2()
        {
            var instance = Expression.Parameter(Type, "instance");
            var memberName = Expression.Parameter(typeof(string), "memberName");
            var newValue = Expression.Parameter(typeof(object), "newValue");
            var cases = new List<SwitchCase>();
            foreach (var propertyInfo in Type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (!propertyInfo.CanWrite) { continue; }
                var property = Expression.Property(Expression.Convert(instance, Type), propertyInfo.Name);
                var setValue = Expression.Assign(property, Expression.Convert(newValue, propertyInfo.PropertyType));
                var propertyHash = Expression.Constant(propertyInfo.Name, typeof(string));

                cases.Add(Expression.SwitchCase(Expression.Convert(setValue, typeof(object)), propertyHash));
            }
            foreach (var propertyInfo in Type.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            {
                if (!propertyInfo.CanWrite) { continue; }
                var property = Expression.Property(null, propertyInfo);
                var setValue = Expression.Assign(property, Expression.Convert(newValue, propertyInfo.PropertyType));
                var propertyHash = Expression.Constant(propertyInfo.Name, typeof(string));

                cases.Add(Expression.SwitchCase(Expression.Convert(setValue, typeof(object)), propertyHash));
            }
            return Expression.Lambda<Action<T, string, object>>(Expression.Switch(memberName, Expression.Constant(null), cases.ToArray()), instance, memberName, newValue).Compile();
        }
    }

    /// <summary>
    /// 应用文件配置
    /// </summary>
    [Obsolete("替代方案:SettingJsonFile")]
    public abstract class AppConfigJsonFile
    {
        /// <summary>
        /// 资源字典
        /// </summary>
        public ConcurrentDictionary<string, string> ResDic { get; } = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// 文件监听
        /// </summary>
        protected FileSystemWatcher FileWatcher { get; }
        /// <summary>
        /// 文件路径
        /// </summary>
        protected string FilePath { get; }
        /// <summary>
        /// 安全码
        /// </summary>
        protected string SecurityKey { get; }
        /// <summary>
        /// 不加密构造
        /// </summary>
        /// <param name="filePath"></param>
        protected AppConfigJsonFile(string filePath) : this(filePath, string.Empty) { }
        /// <summary>
        /// 构造
        /// </summary>
        protected AppConfigJsonFile(string filePath, string securityKey)
        {
            FilePath = filePath;
            SecurityKey = securityKey;
            var fileWatcher = new FileSystemWatcher();
            try
            {
                //初始化监听
                fileWatcher.BeginInit();
                //设置监听的路径
                fileWatcher.Path = System.IO.Path.GetDirectoryName(filePath);
                //设置监听文件类型
                fileWatcher.Filter = System.IO.Path.GetFileName(filePath);
                //设置是否监听子目录
                fileWatcher.IncludeSubdirectories = false;
                //设置是否启用监听?
                fileWatcher.EnableRaisingEvents = true;
                //设置需要监听的更改类型(如:文件或者文件夹的属性,文件或者文件夹的创建时间;NotifyFilters枚举的内容)
                fileWatcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size;
                //注册创建文件或目录时的监听事件
                //watcher.Created += new FileSystemEventHandler(watch_created);
                //注册当指定目录的文件或者目录发生改变的时候的监听事件
                fileWatcher.Changed += new FileSystemEventHandler((sender, e) => LoadConfigFile());
                //注册当删除目录的文件或者目录的时候的监听事件
                fileWatcher.Deleted += new FileSystemEventHandler((sender, e) => TrySave());
                //当指定目录的文件或者目录发生重命名的时候的监听事件
                //fileWatcher.Renamed += new RenamedEventHandler((sender, e) =>
                //{
                //    if (!File.Exists(FilePath)) { Save(); }
                //});
                //结束初始化
                fileWatcher.EndInit();
            }
            catch (Exception ex) { Console.WriteLine(ex); }
            FileWatcher = fileWatcher;
            LoadingDefault();
            LoadConfigFile();
        }
        /// <summary>
        /// 加载默认
        /// </summary>
        protected virtual void LoadingDefault()
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Static);
            foreach (var prop in properties)
            {
                try
                {
                    ResDic[prop.Name] = prop.GetValue(null, null).ToString();
                }
                catch { }
            }
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        protected virtual void LoadConfigFile()
        {
            if (!System.IO.File.Exists(FilePath))
            {
                TrySave();
                return;
            }
            try
            {
                var fcontent = File.ReadAllText(FilePath);
                var dic = fcontent.GetJsonObject<Dictionary<string, string>>();
                var isEncrypt = !string.IsNullOrEmpty(SecurityKey);
                var type = GetType();
                foreach (var item in dic)
                {
                    try
                    {
                        var prop = type.GetProperty(item.Key);
                        if (prop != null && prop.CanWrite)
                        {
                            var value = item.Value;
                            if (isEncrypt)
                            {
                                value = UserCrypto.GetAesDecrypt(item.Value, SecurityKey);
                            }
                            prop.SetValue(null, value, null);
                            ResDic[item.Key] = value;
                        }
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex); }
        }
        /// <summary>
        /// 保存到文件
        /// </summary>
        public virtual void TrySave()
        {
            try
            {
                FileWatcher.EnableRaisingEvents = false;
                File.WriteAllText(FilePath, ResDic.GetJsonFormatString());
                FileWatcher.EnableRaisingEvents = true;
            }
            catch (Exception ex) { Console.WriteLine(ex); }
        }
        /// <summary>
        /// 获取字典值
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        protected virtual string Get(string property)
        {
            if (ResDic.TryGetValue(property, out string value))
            {
                return value;
            }
            return property ?? string.Empty;
        }
        /// <summary>
        /// 获取字典值
        /// </summary>
        /// <param name="content"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual String Get(string content, params string[] args)
        {
            return Get(string.Format(content, args));
        }
    }
}
