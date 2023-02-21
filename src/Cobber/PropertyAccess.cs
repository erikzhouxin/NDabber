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
    /// 动态生成方法成员访问
    /// 支持object<see cref="PropertyAccess"/>
    /// 能效等级二级,推荐使用
    /// </summary>
    public class MemberAccessor : IMemberAccessor
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
            return PropertyAccess.GetAccess(instance).FuncGetValue(instance, memberName);
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
            PropertyAccess.GetAccess(instance).FuncSetValue(instance, memberName, newValue);
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
            Func<T, string, object> Get = PropertyAccess<T>.InternalGetValue;
            foreach (var item in property)
            {
                if (ignores.Any(s => item.Name.Equals(s, StringComparison.OrdinalIgnoreCase))) { continue; }
                if (Get(source, item.Name) != Get(target, item.Name)) { return false; }
            }
            return true;
        }
    }
    /// <summary>
    /// 表达式树生成case字段泛型获取
    /// 能效等级二级,推荐使用
    /// 泛型不支持object
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
    /// 支持object
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
            var getCases = new List<SwitchCase>();
            var getTypeCases = new List<SwitchCase>();
            var setCases = new List<SwitchCase>();
            var getDic = new Dictionary<string, Func<T, object>>();
            var setDic = new Dictionary<string, Action<T, object>>();
            var infoDic = new Dictionary<string, InfoModel>();
            foreach (var propertyInfo in Type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var resCase = Expression.Constant(propertyInfo.Name, typeof(string));
                var resVal = Expression.Property(instance, propertyInfo);
                var resValue = Expression.Convert(resVal, typeof(object));
                getCases.Add(Expression.SwitchCase(resValue, resCase));
                getTypeCases.Add(Expression.SwitchCase(Expression.Constant(propertyInfo.PropertyType), resCase));
                var getLambda = Expression.Lambda<Func<T, object>>(resValue, instance).Compile();
                getDic.Add(propertyInfo.Name, getLambda);
                Action<T, object> setLambda = null;
                if (propertyInfo.CanWrite)
                {
                    var setValue = Expression.Assign(resVal, Expression.Convert(setNewValue, propertyInfo.PropertyType));
                    setCases.Add(Expression.SwitchCase(setValue, resCase));
                    setLambda = Expression.Lambda<Action<T, object>>(setValue, instance, setNewValue).Compile();
                    setDic.Add(propertyInfo.Name, setLambda);
                }
                infoDic.Add(propertyInfo.Name, new InfoModel(propertyInfo, getLambda, setLambda));
            }
            foreach (var propertyInfo in Type.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            {
                var resCase = Expression.Constant(propertyInfo.Name, typeof(string));

                var resVal = Expression.Property(null, propertyInfo);
                var resValue = Expression.Convert(resVal, typeof(object));
                getCases.Add(Expression.SwitchCase(resValue, resCase));
                getTypeCases.Add(Expression.SwitchCase(Expression.Constant(propertyInfo.PropertyType), resCase));
                var getLambda = Expression.Lambda<Func<T, object>>(resValue, instance).Compile();
                getDic.Add(propertyInfo.Name, getLambda);
                Action<T, object> setLambda = null;
                if (propertyInfo.CanWrite)
                {
                    var setValue = Expression.Assign(resVal, Expression.Convert(setNewValue, propertyInfo.PropertyType));
                    setCases.Add(Expression.SwitchCase(setValue, resCase));
                    setLambda = Expression.Lambda<Action<T, object>>(setValue, instance, setNewValue).Compile();
                    setDic.Add(propertyInfo.Name, setLambda);
                }
                infoDic.Add(propertyInfo.Name, new InfoModel(propertyInfo, getLambda, setLambda));
            }
            foreach (var item in Type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {

            }

            InternalGetValue = Expression.Lambda<Func<T, string, object>>(Expression.Switch(memberName, Expression.Constant(null), getCases.ToArray()), instance, memberName).Compile();

            InternalGetType = Expression.Lambda<Func<string, Type>>(Expression.Switch(memberName, Expression.Constant(typeof(object)), getTypeCases.ToArray()), memberName).Compile();

            InternalSetValue = Expression.Lambda<Action<T, string, object>>(Expression.Switch(typeof(void), memberName, Expression.Constant(null), null, setCases.ToArray()), instance, memberName, setNewValue).Compile();

            InternalGetDic = new(getDic);
            InternalSetDic = new(setDic);
            InternalInfoDic = new(infoDic);
            PropertyAccess.MemberDic[Type] = Proxy = new PropertyAccess<T>();
            Instance = default(T);
        }
        #endregion
        Func<object, string, object> IPropertyAccess.FuncGetValue { get; } = (instance, memberName) => InternalGetValue((T)instance, memberName);
        Func<string, Type> IPropertyAccess.FuncGetType { get; } = InternalGetType;

        Action<object, string, object> IPropertyAccess.FuncSetValue { get; } = (instance, memberName, newValue) => InternalSetValue((T)instance, memberName, newValue);

        ReadOnlyDictionary<string, Func<object, object>> IPropertyAccess.FuncGetDic { get; } = new(InternalGetDic.ToDictionary(s => s.Key, s => new Func<object, object>((m) => s.Value((T)m))));

        ReadOnlyDictionary<string, Action<object, object>> IPropertyAccess.FuncSetDic { get; } = new(InternalSetDic.ToDictionary(s => s.Key, s => new Action<object, object>((m, v) => s.Value((T)m, v))));

        ReadOnlyDictionary<string, PropertyAccess.IInfoModel> IPropertyAccess.FuncInfoDic { get; } = new(InternalInfoDic.ToDictionary(s => s.Key, s => (PropertyAccess.IInfoModel)s.Value));

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
        public PropertyAccess() { }
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
            readonly Func<object, object> _getValue;
            readonly Action<object, object> _setValue;
            /// <summary>
            /// 属性信息
            /// </summary>
            public PropertyInfo PropertyInfo { get; }
            /// <summary>
            /// 获取值
            /// </summary>
            public Func<T, object> GetValue { get; }
            /// <summary>
            /// 设置值
            /// </summary>
            public Action<T, object> SetValue { get; }
            Func<object, object> PropertyAccess.IInfoModel.GetValue { get => _getValue; }
            Action<object, object> PropertyAccess.IInfoModel.SetValue { get => _setValue; }
            /// <summary>
            /// 构造
            /// </summary>
            internal InfoModel(PropertyInfo propertyInfo, Func<T, object> getValue, Action<T, object> setValue)
            {
                PropertyInfo = propertyInfo;
                GetValue = getValue;
                SetValue = setValue;
                _getValue = (m) => getValue((T)m);
                _setValue = (m, v) => setValue((T)m, v);
            }
        }
    }
}
