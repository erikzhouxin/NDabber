using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Cobber;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace System.Data.Extter
{
    /// <summary>
    /// 成员调用
    /// </summary>
    public static class MemberCaller
    {
        /// <summary>
        /// 字段信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static FieldInfo GetFieldInfo<T>(this Expression<Func<T>> expression)
        {
            MemberExpression body = (MemberExpression)expression.Body;
            return (FieldInfo)body.Member;
        }
        /// <summary>
        /// 属性信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo<T>(this Expression<Func<T>> expression)
        {
            MemberExpression body = (MemberExpression)expression.Body;
            return (PropertyInfo)body.Member;
        }
        /// <summary>
        /// 属性信息
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo<TM, TP>(this Expression<Func<TM, TP>> expression)
        {
            MemberExpression body = (MemberExpression)expression.Body;
            return (PropertyInfo)body.Member;
        }
        /// <summary>
        /// 属性信息
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo<TM>(this Expression<Func<TM, object>> expression)
        {
            var body = expression.Body;
            if (body is MemberExpression member)
            {
                return (PropertyInfo)member.Member;
            }
            if (body is UnaryExpression unary)
            {
                return (PropertyInfo)((MemberExpression)unary.Operand).Member;
            }
            return null;
        }
        /// <summary>
        /// 成员信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MemberInfo GetMemberInfo<T>(this Expression<Func<T>> expression)
        {
            MemberExpression body = (MemberExpression)expression.Body;
            return body.Member;
        }
        /// <summary>
        /// 成员信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static String GetFullName<T>(this Expression<Func<T>> expression)
        {
            MemberExpression body = (MemberExpression)expression.Body;
            var member = body.Member;
            return $"{member.DeclaringType.FullName}.{member.Name}";
        }
        /// <summary>
        /// 获取成员全称
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static String GetFullName(this MemberInfo method)
        {
            return $"{method.DeclaringType?.FullName}.{method.Name}";
        }
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="model"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SearchModel<T>(this T model, string value)
        {
            if (model == null) { return false; }
            if (string.IsNullOrWhiteSpace(value)) { return true; }
            return SearchContains(model, new Dictionary<string, bool> { { value.Trim(), false } }, PropertyAccess.Get(model).FuncGetDic.Values);
        }
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="model"></param>
        /// <param name="value"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public static bool SearchModel<T>(this T model, string value, char split)
        {
            if (model == null || value == null) { return false; }
            if (string.IsNullOrWhiteSpace(value)) { return true; }
            return SearchModel(model, value.Split(split));
        }
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="model"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool SearchModel<T>(this T model, params string[] values)
        {
            if (model == null || values == null) { return false; }
            var valItems = values.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).Distinct().ToList();
            if (valItems.Count == 0) { return true; }
            return SearchContains(model, valItems.ToDictionary(s => s, s => false), PropertyAccess.Get(model).FuncGetDic.Values);
        }
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="models"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static IEnumerable<T> SearchModel<T>(this IEnumerable<T> models, params string[] values)
        {
            if (models.IsEmpty() || values == null) { return models; }
            var valItems = values.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
            if (valItems.Length == 0) { return models; }
            var valDic = valItems.ToDictionary(s => s, s => false);
            var first = models.First();
            var funcGetList = PropertyAccess.Get(first).FuncGetDic.Values;
            return models.Where(m => SearchContains<T>(m, valDic, funcGetList));
        }

        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="models"></param>
        /// <param name="properties"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static IEnumerable<T> SearchModel<T>(this IEnumerable<T> models, string[] properties, string[] values)
        {
            if (models.IsEmpty() || values == null || properties.IsEmpty()) { return models; }
            var valItems = values.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
            if (valItems.Length == 0) { return models; }
            var valDic = valItems.ToDictionary(s => s, s => false);
            var first = models.First();
            var funcGetList = PropertyAccess.Get(first).FuncGetDic.Where(s => properties.Contains(s.Key)).Select(s => s.Value);
            return models.Where(m => SearchContains<T>(m, valDic, funcGetList));
        }

        private static bool SearchContains<T>(T model, Dictionary<string, bool> valItems, IEnumerable<Func<object, object>> getFuncList)
        {
            var dic = valItems.ToDictionary(s => s.Key, s => false);
            foreach (var item in getFuncList)
            {
                var val = item.Invoke(model);
                if (val == null) { continue; }
                var valString = val.ToString();
                if (string.IsNullOrWhiteSpace(valString)) { continue; }
                foreach (var vi in valItems.Keys)
                {
                    if (valString.Contains(vi))
                    {
                        dic[vi] = true;
                    }
                }
            }
            return dic.All(s => s.Value);
        }
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="model"></param>
        /// <param name="value"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public static int SearchOrModel<T>(this T model, string value, char split)
        {
            if (model == null || value == null) { return 0; }
            if (string.IsNullOrEmpty(value)) { return 1; }
            return SearchOrModel<T>(model, value.Split(split));
        }
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="model"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static int SearchOrModel<T>(this T model, params string[] values)
        {
            if (model == null || values == null) { return 0; }
            var valItems = values.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
            if (valItems.Length == 0) { return 1; }
            return SearchContainsCount(model, valItems);
        }
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="models"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static IEnumerable<T> SearchOrModel<T>(this IEnumerable<T> models, params string[] values)
        {
            if (models.IsEmpty() || values == null) { return models; }
            var valItems = values.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
            if (valItems.Length == 0) { return models; }
            var dic = new Dictionary<int, List<T>>();
            for (int i = 0; i < valItems.Length; i++)
            {
                dic.Add(i + 1, new List<T>());
            }
            foreach (var model in models)
            {
                var count = SearchContainsCount<T>(model, valItems);
                if (count > 0)
                {
                    dic[count].Add(model);
                }
            }
            var result = new List<T>();
            foreach (var item in dic.OrderByDescending(s => s.Key))
            {
                result.AddRange(item.Value);
            }
            return result;
        }

        private static int SearchContainsCount<T>(T model, string[] valItems)
        {
            var count = 0;
            foreach (var item in PropertyAccess.Get(model).FuncGetDic)
            {
                var val = item.Value.Invoke(model);
                if (val == null) { continue; }
                var valString = val.ToString();
                if (string.IsNullOrWhiteSpace(valString)) { continue; }
                foreach (var vi in valItems)
                {
                    if (valString.Contains(vi)) { count++; }
                }
            }
            return count;
        }

        /// <summary>
        /// 获取属性访问
        /// </summary>
        /// <see cref="PropertyAccess{T}"/>
        /// <returns></returns>
        public static IPropertyAccess GetPropertyAccess<T>() => new PropertyAccess<T>();
        /// <summary>
        /// 获取属性访问
        /// </summary>
        /// <see cref="PropertyAccess.Get"/>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IPropertyAccess GetPropertyAccess(Type type) => PropertyAccess.Get(type);
        /// <summary>
        /// 获取静态的成员属性值
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="memberName">成员名称</param>
        /// <returns>成员值</returns>
        public static object GetPropertyValue(this Type type, string memberName)
        {
            return PropertyAccess.Get(type).FuncGetValue.Invoke(null, memberName);
        }
        /// <summary>
        /// 设置静态的成员属性值
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="memberName">成员名称</param>
        /// <param name="newValue">新值</param>
        /// <returns>成员值</returns>
        public static void SetPropertyValue(this Type type, string memberName, object newValue)
        {
            PropertyAccess.Get(type).FuncSetValue.Invoke(null, memberName, newValue);
        }
        /// <summary>
        /// 获取静态的成员属性值
        /// <see cref="PropertyAccess{T}.InternalGetValue"/>
        /// </summary>
        /// <param name="memberName">成员名称</param>
        /// <returns>成员值</returns>
        public static object GetPropertyValue<T>(string memberName)
        {
            return PropertyAccess<T>.InternalGetValue(default(T), memberName);
        }
        /// <summary>
        /// 设置静态的成员属性值
        /// <see cref="PropertyAccess{T}.InternalSetValue"/>
        /// </summary>
        /// <param name="memberName">成员名称</param>
        /// <param name="newValue">新值</param>
        /// <returns>成员值</returns>
        public static void SetPropertyValue<T>(string memberName, object newValue)
        {
            PropertyAccess<T>.InternalSetValue(default(T), memberName, newValue);
        }
        /// <summary>
        /// 转换成属性字典
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Dictionary<string, object> ToParameters<T>(this T model)
        {
            return PropertyAccess<T>.InternalGetDic.ToDictionary(s => s.Key, s => s.Value(model));
        }
        /// <summary>
        /// 获取静态的成员属性值
        /// 如果Type和对象能匹配请使用<see cref="PropertyAccess{T}.InternalGetValue"/>
        /// </summary>
        /// <param name="instance">实例对象,为null时使用泛型类的静态内容</param>
        /// <param name="memberName">成员名称</param>
        /// <returns>成员值</returns>
        public static object GetPropertyValue<T>(this T instance, string memberName) where T : class
        {
            if (instance == null || instance.GetType() == typeof(T)) { return PropertyAccess<T>.InternalGetValue(instance, memberName); }
            return PropertyAccess.Get(instance.GetType()).FuncGetValue(instance, memberName);
        }
        /// <summary>
        /// 设置静态的成员属性值
        /// 如果Type和对象能匹配请使用<see cref="PropertyAccess{T}.InternalSetValue"/>
        /// </summary>
        /// <param name="instance">实例对象,为null时使用泛型类的静态内容</param>
        /// <param name="memberName">成员名称</param>
        /// <param name="newValue">新值</param>
        /// <returns>成员值</returns>
        public static void SetPropertyValue<T>(this T instance, string memberName, object newValue) where T : class
        {
            if (instance == null || instance.GetType() == typeof(T)) { PropertyAccess<T>.InternalSetValue(instance, memberName, newValue); return; }
            PropertyAccess.Get(instance.GetType()).FuncSetValue(instance, memberName, newValue);
        }
        #region // 获取Action/Func表达式的方法全称或方法信息
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName(this Action expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo(Expression<Action> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0>(this Func<T0> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0>(this Action<T0> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0>(Expression<Func<T0>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0>(Expression<Action<T0>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1>(this Func<T0, T1> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1>(this Action<T0, T1> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1>(Expression<Func<T0, T1>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1>(Expression<Action<T0, T1>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2>(this Func<T0, T1, T2> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2>(this Action<T0, T1, T2> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2>(Expression<Func<T0, T1, T2>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2>(Expression<Action<T0, T1, T2>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3>(this Func<T0, T1, T2, T3> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3>(this Action<T0, T1, T2, T3> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3>(Expression<Func<T0, T1, T2, T3>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3>(Expression<Action<T0, T1, T2, T3>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4>(this Func<T0, T1, T2, T3, T4> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4>(this Action<T0, T1, T2, T3, T4> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4>(Expression<Func<T0, T1, T2, T3, T4>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4>(Expression<Action<T0, T1, T2, T3, T4>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5>(this Func<T0, T1, T2, T3, T4, T5> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5>(this Action<T0, T1, T2, T3, T4, T5> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5>(Expression<Func<T0, T1, T2, T3, T4, T5>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5>(Expression<Action<T0, T1, T2, T3, T4, T5>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6>(this Func<T0, T1, T2, T3, T4, T5, T6> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6>(this Action<T0, T1, T2, T3, T4, T5, T6> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6>(Expression<Func<T0, T1, T2, T3, T4, T5, T6>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6>(Expression<Action<T0, T1, T2, T3, T4, T5, T6>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7>(this Func<T0, T1, T2, T3, T4, T5, T6, T7> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7>(this Action<T0, T1, T2, T3, T4, T5, T6, T7> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7>(Expression<Action<T0, T1, T2, T3, T4, T5, T6, T7>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T0, T1, T2, T3, T4, T5, T6, T7, T8> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T0, T1, T2, T3, T4, T5, T6, T7, T8> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7, T8>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8>(Expression<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(this Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(Expression<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(this Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(Expression<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(this Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(Expression<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(this Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(Expression<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(this Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(Expression<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static String GetMethodFullName<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(this Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF> expression)
        {
            return expression.Method.GetFullName();
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(Expression<Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        /// <summary>
        /// 获取方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(Expression<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        #endregion
    }
}
