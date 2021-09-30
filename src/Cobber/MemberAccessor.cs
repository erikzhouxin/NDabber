using System;
using System.Collections.Generic;
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
    /// 能效等级四级,不推荐使用
    /// </summary>
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
    /// 能效等级三级,不推荐使用
    /// </summary>
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
    /// 能效等级三级,不推荐使用
    /// </summary>
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
    /// 能效等级二级,推荐使用
    /// </summary>
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
                if (Get(source, item.Name) != Get(source, item.Name))
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
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
        /// 设置值(instance,memberName,newValue,propertyInfo)
        /// </summary>
        public static Func<T, string, object, PropertyInfo> SetProperty;
        static MemberExpressionAccessor()
        {
            GetValue = GenerateGetValue();
            SetValue = GenerateSetValue();
        }
        private static Func<T, string, object> GenerateGetValue()
        {
            var type = typeof(T);
            var instance = Expression.Parameter(type, "instance");
            var memberName = Expression.Parameter(typeof(string), "memberName");
            var nameHash = Expression.Variable(typeof(int), "nameHash");
            var calHash = Expression.Assign(nameHash, Expression.Call(memberName, typeof(object).GetMethod("GetHashCode")));
            var cases = new List<SwitchCase>();
            foreach (var propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var property = Expression.Property(Expression.Convert(instance, type), propertyInfo.Name);
                var propertyHash = Expression.Constant(propertyInfo.Name.GetHashCode(), typeof(int));

                cases.Add(Expression.SwitchCase(Expression.Convert(property, typeof(object)), propertyHash));
            }
            foreach (var propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            {
                var property = Expression.Property(null, type, propertyInfo.Name);
                var propertyHash = Expression.Constant(propertyInfo.Name.GetHashCode(), typeof(int));
                cases.Add(Expression.SwitchCase(Expression.Convert(property, typeof(object)), propertyHash));
            }
            var switchEx = Expression.Switch(nameHash, Expression.Constant(null), cases.ToArray());
            var methodBody = Expression.Block(typeof(object), new[] { nameHash }, calHash, switchEx);

            return Expression.Lambda<Func<T, string, object>>(methodBody, instance, memberName).Compile();
        }
        private static Action<T, string, object> GenerateSetValue()
        {
            var type = typeof(T);
            var instance = Expression.Parameter(type, "instance");
            var memberName = Expression.Parameter(typeof(string), "memberName");
            var newValue = Expression.Parameter(typeof(object), "newValue");
            var nameHash = Expression.Variable(typeof(int), "nameHash");
            var calHash = Expression.Assign(nameHash, Expression.Call(memberName, typeof(object).GetMethod("GetHashCode")));
            var cases = new List<SwitchCase>();
            foreach (var propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (!propertyInfo.CanWrite) { continue; }
                var property = Expression.Property(Expression.Convert(instance, type), propertyInfo.Name);
                var setValue = Expression.Assign(property, Expression.Convert(newValue, propertyInfo.PropertyType));
                var propertyHash = Expression.Constant(propertyInfo.Name.GetHashCode(), typeof(int));

                cases.Add(Expression.SwitchCase(Expression.Convert(setValue, typeof(object)), propertyHash));
            }
            foreach (var propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
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
}
