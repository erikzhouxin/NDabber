using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

/// <summary>
/// 尝试测试
/// </summary>
public static class TestTry
{
    /// <summary>
    /// 调用
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public static Exception Call(this Action action)
    {
        try
        {
            action();
            return null;
        }
        catch (Exception ex) { return ex; }
    }
    /// <summary>
    /// 调用
    /// </summary>
    /// <returns></returns>
    public static Exception Call<T1>(this Action<T1> action, T1 t1 = default)
    {
        try
        {
            action(t1);
            return null;
        }
        catch (Exception ex) { return ex; }
    }
    /// <summary>
    /// 调用
    /// </summary>
    /// <returns></returns>
    public static Exception Call<T1, T2>(this Action<T1, T2> action, T1 t1 = default, T2 t2 = default)
    {
        try
        {
            action(t1, t2);
            return null;
        }
        catch (Exception ex) { return ex; }
    }
    /// <summary>
    /// 调用
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public static T Call<T>(this Func<T> action)
    {
        try
        {
            return action();
        }
        catch { }
        return default;
    }
    /// <summary>
    /// 调用
    /// </summary>
    /// <returns></returns>
    public static Exception Call(this Delegate action, params object[] args)
    {
        try
        {
            action.DynamicInvoke(args);
            return null;
        }
        catch (Exception ex) { return ex; }
    }
    /// <summary>
    /// 创建数组
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="models"></param>
    /// <returns></returns>
    public static T[] CreateArray<T>(params T[] models) => models;
    /// <summary>
    /// 方法信息
    /// </summary>
    /// <returns></returns>
    public static string GetMethodFullName(this Delegate expression)
    {
        var member = expression.Method;
        return $"{member.DeclaringType?.FullName}.{member.Name}";
    }
    /// <summary>
    /// 成员全称
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    public static String GetMemberFullName(this MemberInfo member)
    {
        return $"{member.DeclaringType?.FullName}.{member.Name}";
    }
}
