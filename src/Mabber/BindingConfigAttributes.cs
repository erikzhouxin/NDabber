using System;

namespace System.Data.Mabber
{
    /// <summary>
    /// 忽略特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class IgnoreAttribute : Attribute
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="targetType"></param>
        public IgnoreAttribute(Type targetType = null)
        {
            TargetType = targetType;
        }
        /// <summary>
        /// 标记类型
        /// </summary>
        public Type TargetType { get; }
    }

    /// <summary>
    /// 绑定特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class BindAttribute : Attribute
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="targetMemberName"></param>
        /// <param name="targetType"></param>
        public BindAttribute(string targetMemberName, Type targetType = null)
        {
            MemberName = targetMemberName;
            TargetType = targetType;
        }
        /// <summary>
        /// 标记类型
        /// </summary>
        public Type TargetType { get; }
        /// <summary>
        /// 属性名称
        /// </summary>
        public string MemberName { get; }
    }
}
