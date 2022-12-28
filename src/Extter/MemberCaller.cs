using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Cobber;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace System.Data.Extter
{
    /// <summary>
    /// 成员调用
    /// </summary>
    public static partial class ExtterCaller
    {
        /// <summary>
        /// 跟踪方法键
        /// </summary>
        public static String TraceMathodKey { get { return new StackTrace().GetFrame(1).GetMethod().GetMemberFullName(); } }
        /// <summary>
        /// 跟踪方法小时键
        /// </summary>
        public static String TraceMathodHourKey
        {
            get
            {
                var frame = new StackTrace().GetFrame(1);
                return $"{frame.GetMethod().GetMemberFullName()}.{DateTime.Now:yyMMddHH}";
            }
        }
        /// <summary>
        /// 跟踪方法小时键
        /// </summary>
        public static String TraceMathodDayKey
        {
            get
            {
                var frame = new StackTrace().GetFrame(1);
                return $"{frame.GetMethod().GetMemberFullName()}.{DateTime.Now:yyMMdd}";
            }
        }
        /// <summary>
        /// 获取跟踪Frame内容
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static StackFrame GetTraceFrame(int index = 1)
        {
            return new StackTrace().GetFrame(index);
        }
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
        #region // 查询模型内容
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="model">模型,为null时,false</param>
        /// <param name="value">查询字符串,为空时,true</param>
        /// <param name="split">分隔符</param>
        /// <returns></returns>
        public static bool SearchModel<T>(this T model, string value, char split = default)
        {
            if (model == null) { return false; }
            if (string.IsNullOrWhiteSpace(value)) { return true; }
            if (split == default)
            {
                return SearchContains(model, new string[] { value.Trim() }, PropertyAccess.GetAccess(model).FuncGetDic.Values);
            }
            return SearchModel(model, value.Split(split));
        }
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="model">模型,为null时,0</param>
        /// <param name="value">查找字符串,为空时,1</param>
        /// <param name="split">查找分隔符</param>
        /// <returns></returns>
        public static int SearchOrModel<T>(this T model, string value, char split = default)
        {
            if (model == null) { return 0; }
            if (string.IsNullOrEmpty(value)) { return 1; }
            if (split == default)
            {
                return SearchContainsCount<T>(model, new string[] { value.Trim() }, PropertyAccess.GetAccess(model).FuncGetDic.Values);
            }
            return SearchOrModel<T>(model, value.Split(split));
        }
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="model">模型,为null时,false</param>
        /// <param name="values">查询值,为空时,true</param>
        /// <returns></returns>
        public static bool SearchModel<T>(this T model, params string[] values)
        {
            if (model == null) { return false; }
            if (values.IsEmpty()) { return true; }
            var valItems = values.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).Distinct().ToArray();
            if (valItems.Length == 0) { return true; }
            return SearchContains(model, valItems, PropertyAccess.GetAccess(model).FuncGetDic.Values);
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
            return SearchContainsCount(model, valItems, PropertyAccess.GetAccess(model).FuncGetDic.Values);
        }
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="model">模型,为null时,false</param>
        /// <param name="properties">查询属性,为空时,false</param>
        /// <param name="values">查询值,为空时,true</param>
        /// <returns></returns>
        public static bool SearchModel<T>(this T model, string[] properties, string[] values)
        {
            if (model == null || properties.IsEmpty()) { return false; }
            if (values.IsEmpty()) { return true; }
            var valItems = values.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
            if (valItems.Length == 0) { return true; }
            var funcGetList = PropertyAccess.GetAccess(model).FuncGetDic.Where(s => properties.Contains(s.Key)).Select(s => s.Value);
            return SearchContains<T>(model, valItems, funcGetList);
        }
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="model"></param>
        /// <param name="properties"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static int SearchOrModel<T>(this T model, string[] properties, string[] values)
        {
            if (model == null || values == null) { return 0; }
            var valItems = values.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
            if (valItems.Length == 0) { return 1; }
            var funcGetList = PropertyAccess.GetAccess(model).FuncGetDic.Where(s => properties.Contains(s.Key)).Select(s => s.Value);
            return SearchContainsCount(model, valItems, funcGetList);
        }
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="models">模型,为空时,空列表</param>
        /// <param name="values">查询值,为空时,源列表</param>
        /// <returns></returns>
        public static IEnumerable<T> SearchModels<T>(this IEnumerable<T> models, params string[] values)
        {
            if (models.IsEmpty()) { return new List<T>(); }
            if (values.IsEmpty()) { return models; }
            var valItems = values.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
            if (valItems.Length == 0) { return models; }
            var funcGetList = PropertyAccess.GetAccess(models.First()).FuncGetDic.Values;
            return models.Where(m => SearchContains<T>(m, valItems, funcGetList));
        }
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="page">模型,为空时,空列表</param>
        /// <param name="values">查询值,为空时,源列表</param>
        /// <returns></returns>
        public static IPageResult<T> SearchModels<T>(this IPageResult<T> page, params string[] values)
        {
            if (page.Items.IsEmpty() || values.IsEmpty()) { return page; }
            var valItems = values.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
            if (valItems.Length == 0) { return page; }
            var funcGetList = PropertyAccess.GetAccess(page.Items.First()).FuncGetDic.Values;
            return page.Reset(page.Items.Where(m => SearchContains<T>(m, valItems, funcGetList)));
        }
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="models"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static IEnumerable<T> SearchOrModels<T>(this IEnumerable<T> models, params string[] values)
        {
            if (models.IsEmpty() || values == null) { return models; }
            var valItems = values.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
            if (valItems.Length == 0) { return models; }
            var dic = new Dictionary<int, List<T>>();
            for (int i = 0; i < valItems.Length; i++)
            {
                dic.Add(i + 1, new List<T>());
            }
            var getFuncList = PropertyAccess.GetAccess(models.First()).FuncGetDic.Values;
            foreach (var model in models)
            {
                var count = SearchContainsCount<T>(model, valItems, getFuncList);
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
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="models">模型,为空时,空列表</param>
        /// <param name="searchContent">查询值,为空时,源列表</param>
        /// <param name="properties">查询属性,为空时,false</param>
        /// <returns></returns>
        public static IEnumerable<T> SearchModels<T>(this IEnumerable<T> models, string searchContent, string[] properties) => SearchModels(models, GetSearchKeys(searchContent), properties);
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="models">模型,为空时,空列表</param>
        /// <param name="searchContent">查询值,为空时,源列表</param>
        /// <param name="properties">查询属性,为空时,false</param>
        /// <returns></returns>
        public static IPageResult<T> SearchModels<T>(this IPageResult<T> models, string searchContent, string[] properties) => SearchModels(models, GetSearchKeys(searchContent), properties);
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="models">模型,为空时,空列表</param>
        /// <param name="searchContent">查询值,为空时,源列表</param>
        /// <param name="properties">查询属性,为空时,false</param>
        /// <returns></returns>
        public static IEnumerable<T> SearchOrModels<T>(this IEnumerable<T> models, string searchContent, string[] properties) => SearchOrModels(models, GetSearchKeys(searchContent), properties);
        /// <summary>
        /// 查询模型(指定字段,指定查找值)
        /// </summary>
        /// <param name="models">模型,为空时,空列表</param>
        /// <param name="properties">查找属性,为空时,空列表</param>
        /// <param name="values">查找值,为空时,源列表</param>
        /// <returns></returns>
        public static IEnumerable<T> SearchModels<T>(this IEnumerable<T> models, string[] values, string[] properties)
        {
            if (models.IsEmpty()) { return new List<T>(); }
            if (values.IsEmpty()) { return models; }
            var valItems = values.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
            if (valItems.Length == 0) { return models; }
            var funcGetList = properties.IsEmpty() ? PropertyAccess.GetAccess(models.First()).FuncGetDic.Values : PropertyAccess.GetAccess(models.First()).FuncGetDic.Where(s => properties.Contains(s.Key)).Select(s => s.Value);
            return models.Where(m => SearchContains<T>(m, valItems, funcGetList));
        }
        /// <summary>
        /// 查询模型(指定字段,指定查找值)
        /// </summary>
        /// <param name="page">模型,为空时,空列表</param>
        /// <param name="properties">查找属性,为空时,空列表</param>
        /// <param name="values">查找值,为空时,源列表</param>
        /// <returns></returns>
        public static IPageResult<T> SearchModels<T>(this IPageResult<T> page, string[] values, string[] properties)
        {
            if (page.Items.IsEmpty() || values.IsEmpty()) { return page; }
            var valItems = values.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
            if (valItems.Length == 0) { return page; }
            var funcGetList = properties.IsEmpty() ? PropertyAccess.GetAccess(page.Items.First()).FuncGetDic.Values : PropertyAccess.GetAccess(page.Items.First()).FuncGetDic.Where(s => properties.Contains(s.Key)).Select(s => s.Value);
            return page.Reset(page.Items.Where(m => SearchContains<T>(m, valItems, funcGetList)));
        }
        /// <summary>
        /// 查询模型
        /// </summary>
        /// <param name="models"></param>
        /// <param name="properties"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static IEnumerable<T> SearchOrModels<T>(this IEnumerable<T> models, string[] values, string[] properties)
        {
            if (models.IsEmpty() || values == null) { return models; }
            var valItems = values.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
            if (valItems.Length == 0) { return models; }
            var dic = new Dictionary<int, List<T>>();
            for (int i = 0; i < valItems.Length; i++)
            {
                dic.Add(i + 1, new List<T>());
            }
            var getFuncList = PropertyAccess.GetAccess(models.First()).FuncGetDic.Where(s => properties.Contains(s.Key)).Select(s => s.Value);
            foreach (var model in models)
            {
                var count = SearchContainsCount<T>(model, valItems, getFuncList);
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
        private static bool SearchContains<T>(T model, string[] valItems, IEnumerable<Func<object, object>> getFuncList)
        {
            var dic = valItems.ToDictionary(s => s, s => false);
            foreach (var item in getFuncList)
            {
                var val = item.Invoke(model);
                if (val == null) { continue; }
                var valString = val.ToString();
                if (string.IsNullOrWhiteSpace(valString)) { continue; }
                foreach (var vi in valItems)
                {
                    if (valString.Contains(vi))
                    {
                        dic[vi] = true;
                    }
                }
            }
            return dic.All(s => s.Value);
        }
        private static int SearchContainsCount<T>(T model, string[] valItems, IEnumerable<Func<object, object>> getFuncList)
        {
            var count = 0;
            foreach (var item in getFuncList)
            {
                var val = item.Invoke(model);
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
        /// 获取查询值,分隔符包括两种逗号/空格/竖线
        /// </summary>
        /// <param name="content"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public static string[] GetSearchKeys(string content, char[] split = null)
        {
            split ??= new char[] { ',', '，', '|', ' ' };
            return string.IsNullOrWhiteSpace(content) ? (new string[0]) : content.Trim().Split(split, StringSplitOptions.RemoveEmptyEntries);
        }
        #endregion
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
        /// <param name="model"></param>
        /// <returns></returns>
        public static IPropertyAccess GetPropertyAccess<T>(this T model) => PropertyAccess.GetAccess(model);
        /// <summary>
        /// 获取属性访问
        /// </summary>
        /// <see cref="PropertyAccess.Get"/>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IPropertyAccess GetPropertyAccess(this Type type) => PropertyAccess.Get(type);
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
        /// <summary>
        /// 设置静态的成员属性值
        /// 如果Type和对象能匹配请使用<see cref="PropertyAccess{T}.InternalInfoDic"/>
        /// </summary>
        /// <param name="instance">实例对象</param>
        /// <param name="model">新值</param>
        /// <returns>成员值</returns>
        public static void SetPropertyValues<T>(this T instance, T model) where T : class
        {
            if (instance == null || model == null) { return; }
            var type = instance.GetType();
            // 按照父类进行数据处理
            var otherType = model.GetType();
            IPropertyAccess access;
            if (type == otherType)
            {
                access = PropertyAccess.Get(type);
            }
            else
            {
                access = otherType.IsAssignableFrom(type) ? PropertyAccess.Get(otherType) : PropertyAccess.Get(type);
            }
            foreach (var item in access.FuncInfoDic)
            {
                var info = item.Value;
                try
                {
                    info.SetValue(instance, info.GetValue.Invoke(model));
                }
                catch { }
            }
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

        #region // CopyClone
        /// <summary>
        /// 深度表达式树复制
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="original">Object to copy.</param>
        /// <param name="copiedReferencesDict">Dictionary of already copied objects (Keys: original objects, Values: their copies).</param>
        /// <returns></returns>
        public static T DeepExpressionCopy<T>(this T original, Dictionary<object, object> copiedReferencesDict = null)
        {
            return (T)DeepExpressionTreeObjCopy(original, false, copiedReferencesDict ?? new Dictionary<object, object>(new ReferenceEqualityComparer()));
        }
        /// <summary>
        /// 深度Json的Serialize复制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DeepJsonCopy<T>(T obj)
        {
            return obj.GetJsonString().GetJsonObject<T>();
        }
        /// <summary>
        /// 深度反射复制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original"></param>
        /// <returns></returns>
        public static T DeepReflectionCopy<T>(this T original)
        {
            return (T)ReflectionInternalCopy((object)original, new Dictionary<Object, object>(new ReferenceEqualityComparer()));
        }
        #region // 内部方法及类
        internal class ReferenceEqualityComparer : EqualityComparer<Object>
        {
            public override bool Equals(object x, object y)
            {
                return ReferenceEquals(x, y);
            }
            public override int GetHashCode(object obj)
            {
                if (obj == null) return 0;
                return obj.GetHashCode();
            }
        }
        internal static void ForEach(this Array array, Action<Array, int[]> action)
        {
            if (array.LongLength == 0) return;
            ArrayTraverse walker = new ArrayTraverse(array);
            do action(array, walker.Position);
            while (walker.Step());
        }
        internal class ArrayTraverse
        {
            public int[] Position;
            private int[] maxLengths;

            public ArrayTraverse(Array array)
            {
                maxLengths = new int[array.Rank];
                for (int i = 0; i < array.Rank; ++i)
                {
                    maxLengths[i] = array.GetLength(i) - 1;
                }
                Position = new int[array.Rank];
            }

            public bool Step()
            {
                for (int i = 0; i < Position.Length; ++i)
                {
                    if (Position[i] < maxLengths[i])
                    {
                        Position[i]++;
                        for (int j = 0; j < i; j++)
                        {
                            Position[j] = 0;
                        }
                        return true;
                    }
                }
                return false;
            }
        }
        #region // 表达式树
        private static readonly object ExpressionTreeStructTypeDeepCopyLocker = new object();
        private static Dictionary<Type, bool> ExpressionTreeStructTypeToDeepCopyDic = new Dictionary<Type, bool>();

        private static readonly object ExpressionTreeCompiledCopyFuncDicLocker = new object();
        private static Dictionary<Type, Func<object, Dictionary<object, object>, object>> ExpressionTreeCompiledCopyFuncDic = new Dictionary<Type, Func<object, Dictionary<object, object>, object>>();

        private static readonly Type ExpressionTreeObjectType = typeof(Object);
        private static readonly Type ExpressionTreeObjectDictionaryType = typeof(Dictionary<object, object>);
        private static readonly Type ExpressionTreeFieldInfoType = typeof(FieldInfo);
        private static readonly MethodInfo SetValueMethod = ExpressionTreeFieldInfoType.GetMethod("SetValue", new[] { ExpressionTreeObjectType, ExpressionTreeObjectType });
        private static readonly Type ThisType = typeof(ExtterCaller);
        private static readonly MethodInfo DeepCopyByExpressionTreeObjMethod = ThisType.GetMethod(nameof(DeepExpressionTreeObjCopy), BindingFlags.NonPublic | BindingFlags.Static);

        internal static object DeepExpressionTreeObjCopy(object original, bool forceDeepCopy, Dictionary<object, object> copiedReferencesDict)
        {
            if (original == null)
            {
                return null;
            }

            var type = original.GetType();

            if (ExpressionTreeIsDelegate(type))
            {
                return null;
            }

            if (!forceDeepCopy && !ExpressionTreeIsTypeToDeepCopy(type))
            {
                return original;
            }

            object alreadyCopiedObject;

            if (copiedReferencesDict.TryGetValue(original, out alreadyCopiedObject))
            {
                return alreadyCopiedObject;
            }

            if (type == ExpressionTreeObjectType)
            {
                return new object();
            }

            var compiledCopyFunction = ExpressionTreeGetOrCreateCompiledLambdaCopyFunc(type);

            object copy = compiledCopyFunction(original, copiedReferencesDict);

            return copy;
        }

        private static Func<object, Dictionary<object, object>, object> ExpressionTreeGetOrCreateCompiledLambdaCopyFunc(Type type)
        {
            // The following structure ensures that multiple threads can use the dictionary
            // even while dictionary is locked and being updated by other thread.
            // That is why we do not modify the old dictionary instance but
            // we replace it with a new instance everytime.

            Func<object, Dictionary<object, object>, object> compiledCopyFunction;

            if (!ExpressionTreeCompiledCopyFuncDic.TryGetValue(type, out compiledCopyFunction))
            {
                lock (ExpressionTreeCompiledCopyFuncDicLocker)
                {
                    if (!ExpressionTreeCompiledCopyFuncDic.TryGetValue(type, out compiledCopyFunction))
                    {
                        var uncompiledCopyFunction = ExpressionTreeCreateCompiledLambdaCopyFuncForType(type);

                        compiledCopyFunction = uncompiledCopyFunction.Compile();

                        var dictionaryCopy = ExpressionTreeCompiledCopyFuncDic.ToDictionary(pair => pair.Key, pair => pair.Value);

                        dictionaryCopy.Add(type, compiledCopyFunction);

                        ExpressionTreeCompiledCopyFuncDic = dictionaryCopy;
                    }
                }
            }

            return compiledCopyFunction;
        }

        private static Expression<Func<object, Dictionary<object, object>, object>> ExpressionTreeCreateCompiledLambdaCopyFuncForType(Type type)
        {
            ParameterExpression inputParameter;
            ParameterExpression inputDictionary;
            ParameterExpression outputVariable;
            ParameterExpression boxingVariable;
            LabelTarget endLabel;
            List<ParameterExpression> variables;
            List<Expression> expressions;

            ///// INITIALIZATION OF EXPRESSIONS AND VARIABLES

            ExpressionTreeInitializeExpressions(type,
                                  out inputParameter,
                                  out inputDictionary,
                                  out outputVariable,
                                  out boxingVariable,
                                  out endLabel,
                                  out variables,
                                  out expressions);

            ///// RETURN NULL IF ORIGINAL IS NULL

            ExpressionTreeIfNullThenReturnNull(inputParameter, endLabel, expressions);

            ///// MEMBERWISE CLONE ORIGINAL OBJECT

            ExpressionTreeMemberwiseCloneInputToOutput(type, inputParameter, outputVariable, expressions);

            ///// STORE COPIED OBJECT TO REFERENCES DICTIONARY

            if (ExpressionTreeIsClassOtherThanString(type))
            {
                ExpressionTreeStoreReferencesIntoDictionary(inputParameter, inputDictionary, outputVariable, expressions);
            }

            ///// COPY ALL NONVALUE OR NONPRIMITIVE FIELDS

            ExpressionTreeFieldsCopy(type,
                                  inputParameter,
                                  inputDictionary,
                                  outputVariable,
                                  boxingVariable,
                                  expressions);

            ///// COPY ELEMENTS OF ARRAY

            if (type.IsArray && ExpressionTreeIsTypeToDeepCopy(type.GetElementType()))
            {
                ExpressionTreeCreateArrayCopyLoop(type,
                                              inputParameter,
                                              inputDictionary,
                                              outputVariable,
                                              variables,
                                              expressions);
            }

            ///// COMBINE ALL EXPRESSIONS INTO LAMBDA FUNCTION

            var lambda = ExpressionTreeCombineAllIntoLambdaFunc(inputParameter, inputDictionary, outputVariable, endLabel, variables, expressions);

            return lambda;
        }

        private static void ExpressionTreeInitializeExpressions(Type type,
                                                  out ParameterExpression inputParameter,
                                                  out ParameterExpression inputDictionary,
                                                  out ParameterExpression outputVariable,
                                                  out ParameterExpression boxingVariable,
                                                  out LabelTarget endLabel,
                                                  out List<ParameterExpression> variables,
                                                  out List<Expression> expressions)
        {

            inputParameter = Expression.Parameter(ExpressionTreeObjectType);

            inputDictionary = Expression.Parameter(ExpressionTreeObjectDictionaryType);

            outputVariable = Expression.Variable(type);

            boxingVariable = Expression.Variable(ExpressionTreeObjectType);

            endLabel = Expression.Label();

            variables = new List<ParameterExpression>();

            expressions = new List<Expression>();

            variables.Add(outputVariable);
            variables.Add(boxingVariable);
        }

        private static void ExpressionTreeIfNullThenReturnNull(ParameterExpression inputParameter, LabelTarget endLabel, List<Expression> expressions)
        {
            ///// Intended code:
            /////
            ///// if (input == null)
            ///// {
            /////     return null;
            ///// }

            var ifNullThenReturnNullExpression =
                Expression.IfThen(
                    Expression.Equal(
                        inputParameter,
                        Expression.Constant(null, ExpressionTreeObjectType)),
                    Expression.Return(endLabel));

            expressions.Add(ifNullThenReturnNullExpression);
        }

        private static void ExpressionTreeMemberwiseCloneInputToOutput(
            Type type,
            ParameterExpression inputParameter,
            ParameterExpression outputVariable,
            List<Expression> expressions)
        {
            ///// Intended code:
            /////
            ///// var output = (<type>)input.MemberwiseClone();

            var memberwiseCloneMethod = ExpressionTreeObjectType.GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);

            var memberwiseCloneInputExpression =
                Expression.Assign(
                    outputVariable,
                    Expression.Convert(
                        Expression.Call(
                            inputParameter,
                            memberwiseCloneMethod),
                        type));

            expressions.Add(memberwiseCloneInputExpression);
        }

        private static void ExpressionTreeStoreReferencesIntoDictionary(ParameterExpression inputParameter,
                                                                          ParameterExpression inputDictionary,
                                                                          ParameterExpression outputVariable,
                                                                          List<Expression> expressions)
        {
            ///// Intended code:
            /////
            ///// inputDictionary[(Object)input] = (Object)output;

            var storeReferencesExpression =
                Expression.Assign(
                    Expression.Property(
                        inputDictionary,
                        ExpressionTreeObjectDictionaryType.GetProperty("Item"),
                        inputParameter),
                    Expression.Convert(outputVariable, ExpressionTreeObjectType));

            expressions.Add(storeReferencesExpression);
        }

        private static Expression<Func<object, Dictionary<object, object>, object>> ExpressionTreeCombineAllIntoLambdaFunc(
            ParameterExpression inputParameter,
            ParameterExpression inputDictionary,
            ParameterExpression outputVariable,
            LabelTarget endLabel,
            List<ParameterExpression> variables,
            List<Expression> expressions)
        {
            expressions.Add(Expression.Label(endLabel));

            expressions.Add(Expression.Convert(outputVariable, ExpressionTreeObjectType));

            var finalBody = Expression.Block(variables, expressions);

            var lambda = Expression.Lambda<Func<object, Dictionary<object, object>, object>>(finalBody, inputParameter, inputDictionary);

            return lambda;
        }

        private static void ExpressionTreeCreateArrayCopyLoop(Type type,
                                                          ParameterExpression inputParameter,
                                                          ParameterExpression inputDictionary,
                                                          ParameterExpression outputVariable,
                                                          List<ParameterExpression> variables,
                                                          List<Expression> expressions)
        {
            ///// Intended code:
            /////
            ///// int i1, i2, ..., in; 
            ///// 
            ///// int length1 = inputarray.GetLength(0); 
            ///// i1 = 0; 
            ///// while (true)
            ///// {
            /////     if (i1 >= length1)
            /////     {
            /////         goto ENDLABELFORLOOP1;
            /////     }
            /////     int length2 = inputarray.GetLength(1); 
            /////     i2 = 0; 
            /////     while (true)
            /////     {
            /////         if (i2 >= length2)
            /////         {
            /////             goto ENDLABELFORLOOP2;
            /////         }
            /////         ...
            /////         ...
            /////         ...
            /////         int lengthn = inputarray.GetLength(n); 
            /////         in = 0; 
            /////         while (true)
            /////         {
            /////             if (in >= lengthn)
            /////             {
            /////                 goto ENDLABELFORLOOPn;
            /////             }
            /////             outputarray[i1, i2, ..., in] 
            /////                 = (<elementType>)DeepCopyByExpressionTreeObj(
            /////                        (Object)inputarray[i1, i2, ..., in])
            /////             in++; 
            /////         }
            /////         ENDLABELFORLOOPn:
            /////         ...
            /////         ...  
            /////         ...
            /////         i2++; 
            /////     }
            /////     ENDLABELFORLOOP2:
            /////     i1++; 
            ///// }
            ///// ENDLABELFORLOOP1:

            var rank = type.GetArrayRank();

            var indices = ExpressionTreeGenerateIndices(rank);

            variables.AddRange(indices);

            var elementType = type.GetElementType();

            var assignExpression = ExpressionTreeArrayFieldToArrayFieldAssign(inputParameter, inputDictionary, outputVariable, elementType, type, indices);

            Expression forExpression = assignExpression;

            for (int dimension = 0; dimension < rank; dimension++)
            {
                var indexVariable = indices[dimension];

                forExpression = ExpressionTreeLoopIntoLoop(inputParameter, indexVariable, forExpression, dimension);
            }

            expressions.Add(forExpression);
        }

        private static List<ParameterExpression> ExpressionTreeGenerateIndices(int arrayRank)
        {
            ///// Intended code:
            /////
            ///// int i1, i2, ..., in; 

            var indices = new List<ParameterExpression>();

            for (int i = 0; i < arrayRank; i++)
            {
                var indexVariable = Expression.Variable(typeof(Int32));

                indices.Add(indexVariable);
            }

            return indices;
        }

        private static BinaryExpression ExpressionTreeArrayFieldToArrayFieldAssign(
            ParameterExpression inputParameter,
            ParameterExpression inputDictionary,
            ParameterExpression outputVariable,
            Type elementType,
            Type arrayType,
            List<ParameterExpression> indices)
        {
            ///// Intended code:
            /////
            ///// outputarray[i1, i2, ..., in] 
            /////     = (<elementType>)DeepCopyByExpressionTreeObj(
            /////            (Object)inputarray[i1, i2, ..., in]);

            var indexTo = Expression.ArrayAccess(outputVariable, indices);

            var indexFrom = Expression.ArrayIndex(Expression.Convert(inputParameter, arrayType), indices);

            var forceDeepCopy = elementType != ExpressionTreeObjectType;

            var rightSide =
                Expression.Convert(
                    Expression.Call(
                        DeepCopyByExpressionTreeObjMethod,
                        Expression.Convert(indexFrom, ExpressionTreeObjectType),
                        Expression.Constant(forceDeepCopy, typeof(Boolean)),
                        inputDictionary),
                    elementType);

            var assignExpression = Expression.Assign(indexTo, rightSide);

            return assignExpression;
        }

        private static BlockExpression ExpressionTreeLoopIntoLoop(
            ParameterExpression inputParameter,
            ParameterExpression indexVariable,
            Expression loopToEncapsulate,
            int dimension)
        {
            ///// Intended code:
            /////
            ///// int length = inputarray.GetLength(dimension); 
            ///// i = 0; 
            ///// while (true)
            ///// {
            /////     if (i >= length)
            /////     {
            /////         goto ENDLABELFORLOOP;
            /////     }
            /////     loopToEncapsulate;
            /////     i++; 
            ///// }
            ///// ENDLABELFORLOOP:

            var lengthVariable = Expression.Variable(typeof(Int32));

            var endLabelForThisLoop = Expression.Label();

            var newLoop =
                Expression.Loop(
                    Expression.Block(
                        new ParameterExpression[0],
                        Expression.IfThen(
                            Expression.GreaterThanOrEqual(indexVariable, lengthVariable),
                            Expression.Break(endLabelForThisLoop)),
                        loopToEncapsulate,
                        Expression.PostIncrementAssign(indexVariable)),
                    endLabelForThisLoop);

            var lengthAssignment = ExpressionTreeGetLengthForDimension(lengthVariable, inputParameter, dimension);

            var indexAssignment = Expression.Assign(indexVariable, Expression.Constant(0));

            return Expression.Block(
                new[] { lengthVariable },
                lengthAssignment,
                indexAssignment,
                newLoop);
        }

        private static BinaryExpression ExpressionTreeGetLengthForDimension(
            ParameterExpression lengthVariable,
            ParameterExpression inputParameter,
            int i)
        {
            ///// Intended code:
            /////
            ///// length = ((Array)input).GetLength(i); 

            var getLengthMethod = typeof(Array).GetMethod("GetLength", BindingFlags.Public | BindingFlags.Instance);

            var dimensionConstant = Expression.Constant(i);

            return Expression.Assign(
                lengthVariable,
                Expression.Call(
                    Expression.Convert(inputParameter, typeof(Array)),
                    getLengthMethod,
                    new[] { dimensionConstant }));
        }

        private static void ExpressionTreeFieldsCopy(Type type,
                                                  ParameterExpression inputParameter,
                                                  ParameterExpression inputDictionary,
                                                  ParameterExpression outputVariable,
                                                  ParameterExpression boxingVariable,
                                                  List<Expression> expressions)
        {
            var fields = ExpressionTreeGetAllRelevantFields(type);

            var readonlyFields = fields.Where(f => f.IsInitOnly).ToList();
            var writableFields = fields.Where(f => !f.IsInitOnly).ToList();

            ///// READONLY FIELDS COPY (with boxing)

            bool shouldUseBoxing = readonlyFields.Any();

            if (shouldUseBoxing)
            {
                var boxingExpression = Expression.Assign(boxingVariable, Expression.Convert(outputVariable, ExpressionTreeObjectType));

                expressions.Add(boxingExpression);
            }

            foreach (var field in readonlyFields)
            {
                if (ExpressionTreeIsDelegate(field.FieldType))
                {
                    ExpressionTreeReadonlyFieldToNull(field, boxingVariable, expressions);
                }
                else
                {
                    ExpressionTreeReadonlyFieldCopy(type,
                                                field,
                                                inputParameter,
                                                inputDictionary,
                                                boxingVariable,
                                                expressions);
                }
            }

            if (shouldUseBoxing)
            {
                var unboxingExpression = Expression.Assign(outputVariable, Expression.Convert(boxingVariable, type));

                expressions.Add(unboxingExpression);
            }

            ///// NOT-READONLY FIELDS COPY

            foreach (var field in writableFields)
            {
                if (ExpressionTreeIsDelegate(field.FieldType))
                {
                    ExpressionTreeWritableFieldToNull(field, outputVariable, expressions);
                }
                else
                {
                    ExpressionTreeWritableFieldCopy(type,
                                                field,
                                                inputParameter,
                                                inputDictionary,
                                                outputVariable,
                                                expressions);
                }
            }
        }

        private static FieldInfo[] ExpressionTreeGetAllRelevantFields(Type type, bool forceAllFields = false)
        {
            var fieldsList = new List<FieldInfo>();

            var typeCache = type;

            while (typeCache != null)
            {
                fieldsList.AddRange(
                    typeCache
                        .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
                        .Where(field => forceAllFields || ExpressionTreeIsTypeToDeepCopy(field.FieldType)));

                typeCache = typeCache.BaseType;
            }

            return fieldsList.ToArray();
        }

        private static void ExpressionTreeReadonlyFieldToNull(FieldInfo field, ParameterExpression boxingVariable, List<Expression> expressions)
        {
            // This option must be implemented by Reflection because of the following:
            // https://visualstudio.uservoice.com/forums/121579-visual-studio-2015/suggestions/2727812-allow-expression-assign-to-set-readonly-struct-f

            ///// Intended code:
            /////
            ///// fieldInfo.SetValue(boxing, <fieldtype>null);

            var fieldToNullExpression =
                    Expression.Call(
                        Expression.Constant(field),
                        SetValueMethod,
                        boxingVariable,
                        Expression.Constant(null, field.FieldType));

            expressions.Add(fieldToNullExpression);
        }

        private static void ExpressionTreeReadonlyFieldCopy(Type type,
                                                        FieldInfo field,
                                                        ParameterExpression inputParameter,
                                                        ParameterExpression inputDictionary,
                                                        ParameterExpression boxingVariable,
                                                        List<Expression> expressions)
        {
            // This option must be implemented by Reflection (SetValueMethod) because of the following:
            // https://visualstudio.uservoice.com/forums/121579-visual-studio-2015/suggestions/2727812-allow-expression-assign-to-set-readonly-struct-f

            ///// Intended code:
            /////
            ///// fieldInfo.SetValue(boxing, DeepCopyByExpressionTreeObj((Object)((<type>)input).<field>))

            var fieldFrom = Expression.Field(Expression.Convert(inputParameter, type), field);

            var forceDeepCopy = field.FieldType != ExpressionTreeObjectType;

            var fieldDeepCopyExpression =
                Expression.Call(
                    Expression.Constant(field, ExpressionTreeFieldInfoType),
                    SetValueMethod,
                    boxingVariable,
                    Expression.Call(
                        DeepCopyByExpressionTreeObjMethod,
                        Expression.Convert(fieldFrom, ExpressionTreeObjectType),
                        Expression.Constant(forceDeepCopy, typeof(Boolean)),
                        inputDictionary));

            expressions.Add(fieldDeepCopyExpression);
        }

        private static void ExpressionTreeWritableFieldToNull(FieldInfo field, ParameterExpression outputVariable, List<Expression> expressions)
        {
            ///// Intended code:
            /////
            ///// output.<field> = (<type>)null;

            var fieldTo = Expression.Field(outputVariable, field);

            var fieldToNullExpression =
                Expression.Assign(
                    fieldTo,
                    Expression.Constant(null, field.FieldType));

            expressions.Add(fieldToNullExpression);
        }

        private static void ExpressionTreeWritableFieldCopy(Type type,
                                                        FieldInfo field,
                                                        ParameterExpression inputParameter,
                                                        ParameterExpression inputDictionary,
                                                        ParameterExpression outputVariable,
                                                        List<Expression> expressions)
        {
            ///// Intended code:
            /////
            ///// output.<field> = (<fieldType>)DeepCopyByExpressionTreeObj((Object)((<type>)input).<field>);

            var fieldFrom = Expression.Field(Expression.Convert(inputParameter, type), field);

            var fieldType = field.FieldType;

            var fieldTo = Expression.Field(outputVariable, field);

            var forceDeepCopy = field.FieldType != ExpressionTreeObjectType;

            var fieldDeepCopyExpression =
                Expression.Assign(
                    fieldTo,
                    Expression.Convert(
                        Expression.Call(
                            DeepCopyByExpressionTreeObjMethod,
                            Expression.Convert(fieldFrom, ExpressionTreeObjectType),
                            Expression.Constant(forceDeepCopy, typeof(Boolean)),
                            inputDictionary),
                        fieldType));

            expressions.Add(fieldDeepCopyExpression);
        }

        private static bool ExpressionTreeIsDelegate(Type type)
        {
            return typeof(Delegate).IsAssignableFrom(type);
        }

        private static bool ExpressionTreeIsTypeToDeepCopy(Type type)
        {
            return ExpressionTreeIsClassOtherThanString(type) || ExpressionTreeIsStructWhichNeedsDeepCopy(type);
        }

        private static bool ExpressionTreeIsClassOtherThanString(Type type)
        {
            return !type.IsValueType && type != typeof(String);
        }

        private static bool ExpressionTreeIsStructWhichNeedsDeepCopy(Type type)
        {
            // The following structure ensures that multiple threads can use the dictionary
            // even while dictionary is locked and being updated by other thread.
            // That is why we do not modify the old dictionary instance but
            // we replace it with a new instance everytime.

            bool isStructTypeToDeepCopy;

            if (!ExpressionTreeStructTypeToDeepCopyDic.TryGetValue(type, out isStructTypeToDeepCopy))
            {
                lock (ExpressionTreeStructTypeDeepCopyLocker)
                {
                    if (!ExpressionTreeStructTypeToDeepCopyDic.TryGetValue(type, out isStructTypeToDeepCopy))
                    {
                        isStructTypeToDeepCopy = ExpressionTreeIsStructOtherThanBasicValueTypes(type) && ExpressionTreeHasInItsHierarchyFieldsWithClasses(type);

                        var newDictionary = ExpressionTreeStructTypeToDeepCopyDic.ToDictionary(pair => pair.Key, pair => pair.Value);

                        newDictionary[type] = isStructTypeToDeepCopy;

                        ExpressionTreeStructTypeToDeepCopyDic = newDictionary;
                    }
                }
            }

            return isStructTypeToDeepCopy;
        }

        private static bool ExpressionTreeIsStructOtherThanBasicValueTypes(Type type)
        {
            return type.IsValueType && !type.IsPrimitive && !type.IsEnum && type != typeof(Decimal);
        }

        private static bool ExpressionTreeHasInItsHierarchyFieldsWithClasses(Type type, HashSet<Type> alreadyCheckedTypes = null)
        {
            alreadyCheckedTypes = alreadyCheckedTypes ?? new HashSet<Type>();

            alreadyCheckedTypes.Add(type);

            var allFields = ExpressionTreeGetAllRelevantFields(type, forceAllFields: true);

            var allFieldTypes = allFields.Select(f => f.FieldType).Distinct().ToList();

            var hasFieldsWithClasses = allFieldTypes.Any(ExpressionTreeIsClassOtherThanString);

            if (hasFieldsWithClasses)
            {
                return true;
            }

            var notBasicStructsTypes = allFieldTypes.Where(ExpressionTreeIsStructOtherThanBasicValueTypes).ToList();

            var typesToCheck = notBasicStructsTypes.Where(t => !alreadyCheckedTypes.Contains(t)).ToList();

            foreach (var typeToCheck in typesToCheck)
            {
                if (ExpressionTreeHasInItsHierarchyFieldsWithClasses(typeToCheck, alreadyCheckedTypes))
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
        #region // 反射方法
        private static readonly MethodInfo CloneMethod = typeof(Object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);
        internal static bool IsPrimitive(Type type)
        {
            if (type == typeof(String)) return true;
            return (type.IsValueType & type.IsPrimitive);
        }

        internal static Object ReflectionInternalCopy(Object originalObject, IDictionary<Object, Object> visited)
        {
            if (originalObject == null) return null;
            var typeToReflect = originalObject.GetType();
            if (IsPrimitive(typeToReflect)) return originalObject;
            if (visited.ContainsKey(originalObject)) return visited[originalObject];
            if (typeof(Delegate).IsAssignableFrom(typeToReflect)) return null;
            var cloneObject = CloneMethod.Invoke(originalObject, null);
            if (typeToReflect.IsArray)
            {
                var arrayType = typeToReflect.GetElementType();
                if (IsPrimitive(arrayType) == false)
                {
                    Array clonedArray = (Array)cloneObject;
                    clonedArray.ForEach((array, indices) => array.SetValue(ReflectionInternalCopy(clonedArray.GetValue(indices), visited), indices));
                }

            }
            visited.Add(originalObject, cloneObject);
            ReflectionCopyFields(originalObject, visited, cloneObject, typeToReflect);
            ReflectionRecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);
            return cloneObject;
        }

        private static void ReflectionRecursiveCopyBaseTypePrivateFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect)
        {
            if (typeToReflect.BaseType != null)
            {
                ReflectionRecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.BaseType);
                ReflectionCopyFields(originalObject, visited, cloneObject, typeToReflect.BaseType, BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate);
            }
        }

        private static void ReflectionCopyFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy, Func<FieldInfo, bool> filter = null)
        {
            foreach (FieldInfo fieldInfo in typeToReflect.GetFields(bindingFlags))
            {
                if (typeToReflect.Name == "Entry" && fieldInfo.Name == "value")
                { }

                if (filter != null && filter(fieldInfo) == false) continue;
                if (IsPrimitive(fieldInfo.FieldType)) continue;
                var originalFieldValue = fieldInfo.GetValue(originalObject);
                var clonedFieldValue = ReflectionInternalCopy(originalFieldValue, visited);
                fieldInfo.SetValue(cloneObject, clonedFieldValue);
            }
        }
        #endregion
        #endregion
        #endregion

        #region // 类型内容
        /// <summary>
        /// 从程序集中搜索类型(搜到就返回)
        /// 完全匹配 不区分大小写
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public static Type SearchTypeFromAssembies(string fullName)
        {
            Type type = null;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var intype  = assembly.GetType(fullName, false, true);
                if(intype != null)
                {
                    return intype;
                }
            }
            return type;
        }
        #endregion
    }
}
