using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Extter;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// 对象成员访问接口
    /// </summary>
    public interface IMemberAccessor
    {
        /// <summary>
        /// 获取对象的成员属性值
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="instance">实例对象(静态成员可为空)</param>
        /// <param name="memberName">成员名称</param>
        /// <returns>成员值</returns>
        object GetValue<T>(T instance, string memberName);

        /// <summary>
        /// 设置对象的成员属性值
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="instance">实例对象(静态成员可为空)</param>
        /// <param name="memberName">成员名称</param>
        /// <param name="newValue">成员值</param>
        void SetValue<T>(T instance, string memberName, object newValue);

        /// <summary>
        /// 比较
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="ignores"></param>
        /// <returns></returns>
        bool Compare<T>(T source, T target, params string[] ignores);
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
            GetValue = GenerateGetValue();
            SetValue = GenerateSetValue();
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
    }
    /// <summary>
    /// 表达式树生成case字段泛型获取
    /// 能效等级二级,推荐使用
    /// 泛型支持object
    /// </summary>
    public interface IPropertyAccess
    {
        /// <summary>
        /// 泛型类型
        /// </summary>
        Type FuncGenericType { get; }
        /// <summary>
        /// 获取值(instance,memberName,return)
        /// </summary>
        Func<object, string, object> FuncGetValue { get; }
        /// <summary>
        /// 获取值类型(memberName,return)
        /// </summary>
        Func<string, Type> FuncGetType { get; }
        /// <summary>
        /// 设置值(instance,memberName,newValue)
        /// </summary>
        Action<object, string, object> FuncSetValue { get; }
        /// <summary>
        /// 获取字典
        /// </summary>
        ReadOnlyDictionary<string, Func<object, object>> FuncGetDic { get; }
        /// <summary>
        /// 设置字典
        /// </summary>
        ReadOnlyDictionary<string, Action<object, object>> FuncSetDic { get; }
        /// <summary>
        /// 信息字典
        /// </summary>
        ReadOnlyDictionary<string, PropertyAccess.IInfoModel> FuncInfoDic { get; }
    }
    /// <summary>
    /// 表达式树生成case字段获取
    /// 能效等级二级,推荐使用
    /// 泛型不支持object
    /// </summary>
    public class PropertyAccess : IPropertyAccess
    {
        internal readonly static Dictionary<Type, IPropertyAccess> MemberDic = new Dictionary<Type, IPropertyAccess>();
        private IPropertyAccess _access;
        /// <summary>
        /// 泛型类型
        /// </summary>
        public Type FuncGenericType { get => _access.FuncGenericType; }
        /// <summary>
        /// 获取值(instance,memberName,return)
        /// </summary>
        public Func<object, string, object> FuncGetValue { get => _access.FuncGetValue; }
        /// <summary>
        /// 获取值类型(memberName,return)
        /// </summary>
        public Func<string, Type> FuncGetType { get => _access.FuncGetType; }
        /// <summary>
        /// 设置值(instance,memberName,newValue)
        /// </summary>
        public Action<object, string, object> FuncSetValue { get => _access.FuncSetValue; }
        /// <summary>
        /// 获取字典
        /// </summary>
        public ReadOnlyDictionary<string, Func<object, object>> FuncGetDic { get => _access.FuncGetDic; }
        /// <summary>
        /// 设置字典
        /// </summary>
        public ReadOnlyDictionary<string, Action<object, object>> FuncSetDic { get => _access.FuncSetDic; }
        /// <summary>
        /// 信息字典
        /// </summary>
        public ReadOnlyDictionary<string, PropertyAccess.IInfoModel> FuncInfoDic { get => _access.FuncInfoDic; }
        /// <summary>
        /// 构造
        /// </summary>
        public PropertyAccess(Type type)
        {
            _access = Get(type);
        }
        /// <summary>
        /// 获取访问类接口实例
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IPropertyAccess Get(Type type)
        {
            if (MemberDic.TryGetValue(type, out var value))
            {
                return value;
            }
            return (IPropertyAccess)Activator.CreateInstance(typeof(PropertyAccess<>).MakeGenericType(type));
        }
        /// <summary>
        /// 获取访问类接口实例
        /// </summary>
        /// <returns></returns>
        public static IPropertyAccess GetAccess<T>(T model)
        {
            return model == null ? Get(typeof(T)) : Get(model.GetType());
        }
        /// <summary>
        /// 获取对象的成员属性值
        /// </summary>
        /// <param name="instance">实例对象</param>
        /// <param name="memberName">成员名称</param>
        /// <returns>成员值</returns>
        public object GetValue(object instance, string memberName)
        {
            return _access.FuncGetValue(instance, memberName);
        }

        /// <summary>
        /// 设置对象的成员属性值
        /// </summary>
        /// <param name="instance">实例对象</param>
        /// <param name="memberName">成员名称</param>
        /// <param name="newValue">成员值</param>
        public void SetValue(object instance, string memberName, object newValue)
        {
            _access.FuncSetValue(instance, memberName, newValue);
        }
        /// <summary>
        /// 级联获取值
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="path"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryCascadeGetValue(object instance, string path, out object value)
        {
            value = instance;
            if (string.IsNullOrWhiteSpace(path)) { return true; }
            try
            {
                IPropertyAccess access = _access;
                var paths = path.Trim().Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in paths)
                {
                    value = access.FuncGetValue(value, item);
                    if (value == null) { return true; }
                    access = PropertyAccess.Get(value.GetType());
                }
                return true;
            }
            catch (Exception ex)
            {
                value = ex;
                return false;
            }
        }
        /// <summary>
        /// 级联设置值
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="path"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryCascadeSetValue(object instance, string path, object value)
        {
            if (string.IsNullOrWhiteSpace(path)) { return true; }
            try
            {
                IPropertyAccess access = _access;
                var paths = path.Trim().Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                var inObject = instance;
                for (int i = 0; i < paths.Length - 1; i++)
                {
                    var item = paths[i];
                    inObject = access.FuncGetValue(value, item);
                    if (value == null) { break; }
                    access = PropertyAccess.Get(inObject.GetType());
                }
                var setVal = paths.Last();
                var setType = access.FuncGetType(setVal);
                access.FuncSetValue(inObject, setVal, Convert.ChangeType(value, setType));
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
        /// <summary>
        /// 级联设置值
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="path"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryCascadeSetValue(object instance, string path, string value)
        {
            if (string.IsNullOrWhiteSpace(path)) { return true; }
            try
            {
                IPropertyAccess access = _access;
                var paths = path.Trim().Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                var inObject = instance;
                for (int i = 0; i < paths.Length - 1; i++)
                {
                    var item = paths[i];
                    inObject = access.FuncGetValue(value, item);
                    if (value == null) { break; }
                    access = PropertyAccess.Get(inObject.GetType());
                }
                var setVal = paths.Last();
                var setType = access.FuncGetType(setVal);
                object setObj = null;
                if (value.StartsWith("[") | value.StartsWith("{")) // 是Json
                {
                    setObj = CobberCaller.GetJsonObject(value, setType);
                }
                else
                {
                    setObj = Convert.ChangeType(value, setType);
                }
                access.FuncSetValue(inObject, setVal, setObj);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
        /// <summary>
        /// 设置真实类型值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static T SetInstance<T>(T model)
        {
            try
            {
                GetAccess(GetAccess(model)).FuncSetValue(null, nameof(PropertyAccess<T>.Instance), model);
            }
            catch { }
            return model;
        }
        /// <summary>
        /// 设置指定类型值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static T SetSingleton<T>(T model)
        {
            return PropertyAccess<T>.Instance = model;
        }
        /// <summary>
        /// 属性访问信息模型
        /// </summary>
        public interface IInfoModel
        {
            /// <summary>
            /// 属性信息
            /// </summary>
            PropertyInfo PropertyInfo { get; }
            /// <summary>
            /// 获取值
            /// </summary>
            Func<object, object> GetValue { get; }
            /// <summary>
            /// 设置值
            /// </summary>
            Action<object, object> SetValue { get; }
        }
    }
    /// <summary>
    /// 表达式树生成case字段泛型获取
    /// 能效等级二级,推荐使用
    /// 泛型不支持object
    /// </summary>
    public class PropertyAccess<T> : IPropertyAccess
    {
        #region // 静态内容
        /// <summary>
        /// 代理类
        /// </summary>
        public static PropertyAccess<T> Proxy { get; }
        /// <summary>
        /// 获取值(instance,memberName,return)
        /// </summary>
        public static Func<T, string, object> InternalGetValue { get; }
        /// <summary>
        /// 获取值类型(instance,memberName,return)
        /// </summary>
        public static Func<string, Type> InternalGetType { get; }
        /// <summary>
        /// 设置值(instance,memberName,newValue)
        /// </summary>
        public static Action<T, string, object> InternalSetValue { get; }
        /// <summary>
        /// 内部获取字典
        /// </summary>
        public static ReadOnlyDictionary<string, Func<T, object>> InternalGetDic { get; }
        /// <summary>
        /// 内部设置字典
        /// </summary>
        public static ReadOnlyDictionary<string, Action<T, object>> InternalSetDic { get; }
        /// <summary>
        /// 内部属性信息字典
        /// </summary>
        public static ReadOnlyDictionary<string, InfoModel> InternalInfoDic { get; }
        /// <summary>
        /// 类型
        /// </summary>
        public static Type Type { get; }
        static PropertyAccess()
        {
            Type = typeof(T);

            var instance = Expression.Parameter(Type, "instance");
            var memberName = Expression.Parameter(typeof(string), "memberName");
            var setNewValue = Expression.Parameter(typeof(object), "newValue");
            var nameHash = Expression.Variable(typeof(int), "nameHash");
            var calHash = Expression.Assign(nameHash, Expression.Call(memberName, typeof(object).GetMethod("GetHashCode")));
            var getCases = new List<SwitchCase>();
            var getTypeCases = new List<SwitchCase>();
            var setCases = new List<SwitchCase>();
            var getDic = new Dictionary<string, Func<T, object>>();
            var setDic = new Dictionary<string, Action<T, object>>();
            var infoDic = new Dictionary<string, InfoModel>();
            foreach (var propertyInfo in Type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var property = Expression.Property(Expression.Convert(instance, Type), propertyInfo);
                var propertyHash = Expression.Constant(propertyInfo.Name.GetHashCode(), typeof(int));

                getCases.Add(Expression.SwitchCase(Expression.Convert(property, typeof(object)), propertyHash));

                getTypeCases.Add(Expression.SwitchCase(Expression.Constant(propertyInfo.PropertyType), propertyHash));
                var getLambda = (Func<T, object>)Expression.Lambda(typeof(Func<T, object>), Expression.Convert(property, typeof(object)), instance).Compile();
                getDic.Add(propertyInfo.Name, getLambda);
                var propInfo = new InfoModel
                {
                    PropertyInfo = propertyInfo,
                    GetValue = getLambda
                };
                infoDic.Add(propertyInfo.Name, propInfo);
                if (!propertyInfo.CanWrite) { continue; }
                var setValue = Expression.Assign(property, Expression.Convert(setNewValue, propertyInfo.PropertyType));
                setCases.Add(Expression.SwitchCase(Expression.Convert(setValue, typeof(object)), propertyHash));
                var setLambda = (Action<T, object>)Expression.Lambda(typeof(Action<T, object>), setValue, instance, setNewValue).Compile();
                setDic.Add(propertyInfo.Name, setLambda);
                propInfo.SetValue = setLambda;
            }
            foreach (var propertyInfo in Type.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            {
                var property = Expression.Property(null, propertyInfo);
                var propertyHash = Expression.Constant(propertyInfo.Name.GetHashCode(), typeof(int));

                getCases.Add(Expression.SwitchCase(Expression.Convert(property, typeof(object)), propertyHash));

                getTypeCases.Add(Expression.SwitchCase(Expression.Constant(propertyInfo.PropertyType), propertyHash));
                var getLambda = (Func<T, object>)Expression.Lambda(typeof(Func<T, object>), Expression.Convert(property, typeof(object)), instance).Compile();
                getDic.Add(propertyInfo.Name, getLambda);
                var propInfo = new InfoModel
                {
                    PropertyInfo = propertyInfo,
                    GetValue = getLambda
                };
                infoDic.Add(propertyInfo.Name, propInfo);
                if (!propertyInfo.CanWrite) { continue; }
                var setValue = Expression.Assign(property, Expression.Convert(setNewValue, propertyInfo.PropertyType));

                setCases.Add(Expression.SwitchCase(Expression.Convert(setValue, typeof(object)), propertyHash));
                var setLambda = (Action<T, object>)Expression.Lambda(typeof(Action<T, object>), setValue, instance, setNewValue).Compile();
                setDic.Add(propertyInfo.Name, setLambda);
                propInfo.SetValue = setLambda;
            }
            var getMethodBody = Expression.Block(typeof(object), new[] { nameHash }, calHash, Expression.Switch(nameHash, Expression.Constant(null), getCases.ToArray()));
            InternalGetValue = Expression.Lambda<Func<T, string, object>>(getMethodBody, instance, memberName).Compile();

            var getTypeMethodBody = Expression.Block(typeof(Type), new[] { nameHash }, calHash, Expression.Switch(nameHash, Expression.Constant(typeof(object)), getTypeCases.ToArray()));
            InternalGetType = Expression.Lambda<Func<string, Type>>(getTypeMethodBody, memberName).Compile();

            var setMethodBody = Expression.Block(typeof(object), new[] { nameHash }, calHash, Expression.Switch(nameHash, Expression.Constant(null), setCases.ToArray()));
            InternalSetValue = Expression.Lambda<Action<T, string, object>>(setMethodBody, instance, memberName, setNewValue).Compile();

            InternalGetDic = new(getDic);
            InternalSetDic = new(setDic);
            InternalInfoDic = new(infoDic);
            PropertyAccess.MemberDic[Type] = Proxy = new PropertyAccess<T>();
            Instance = default(T);
        }
        #endregion
        Func<object, string, object> IPropertyAccess.FuncGetValue => (instance, memberName) => InternalGetValue((T)instance, memberName);
        Func<string, Type> IPropertyAccess.FuncGetType => (memberName) => InternalGetType(memberName);

        Action<object, string, object> IPropertyAccess.FuncSetValue => (instance, memberName, newValue) => InternalSetValue((T)instance, memberName, newValue);

        ReadOnlyDictionary<string, Func<object, object>> IPropertyAccess.FuncGetDic => new(InternalGetDic.ToDictionary(s => s.Key, s => new Func<object, object>((m) => s.Value((T)m))));

        ReadOnlyDictionary<string, Action<object, object>> IPropertyAccess.FuncSetDic => new(InternalSetDic.ToDictionary(s => s.Key, s => new Action<object, object>((m, v) => s.Value((T)m, v))));

        ReadOnlyDictionary<string, PropertyAccess.IInfoModel> IPropertyAccess.FuncInfoDic => new(InternalInfoDic.ToDictionary(s => s.Key, s => (PropertyAccess.IInfoModel)s.Value));

        /// <summary>
        /// 获取值(instance,memberName,return)
        /// </summary>
        public object GetValue(T instance, string memberName) => InternalGetValue(instance, memberName);
        /// <summary>
        /// 获取值(instance,memberName,return)
        /// </summary>
        public P GetValue<P>(T instance, string memberName) => (P)InternalGetValue(instance, memberName);
        /// <summary>
        /// 设置值(instance,memberName,newValue)
        /// </summary>
        public void SetValue(T instance, string memberName, object newValue) => InternalSetValue(instance, memberName, newValue);
        /// <summary>
        /// 构造
        /// </summary>
        public PropertyAccess()
        {
        }
        /// <summary>
        /// 静态单例
        /// </summary>
        public static T Instance { get; set; }
        /// <summary>
        /// 单例
        /// </summary>
        public T Singleton { get => Instance; set => Instance = value; }
        /// <summary>
        /// 泛型类型
        /// </summary>
        public Type FuncGenericType { get => Type; }
        /// <summary>
        /// 属性访问信息模型
        /// </summary>
        public sealed class InfoModel : PropertyAccess.IInfoModel
        {
            /// <summary>
            /// 属性信息
            /// </summary>
            public PropertyInfo PropertyInfo { get; internal set; }
            /// <summary>
            /// 获取值
            /// </summary>
            public Func<T, object> GetValue { get; internal set; }
            /// <summary>
            /// 设置值
            /// </summary>
            public Action<T, object> SetValue { get; internal set; }

            Func<object, object> PropertyAccess.IInfoModel.GetValue { get => (obj) => GetValue((T)obj); }
            Action<object, object> PropertyAccess.IInfoModel.SetValue { get => (obj, args) => SetValue((T)obj, args); }
        }
    }
}
