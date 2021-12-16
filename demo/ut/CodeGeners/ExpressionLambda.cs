using System;
using System.Linq.Expressions;

#pragma warning disable
namespace System.Data.Extter
{
    /// <summary>
    /// 表达式类:本地参数表
    /// <see cref="TSysParams"/>
    /// </summary>
    public partial class ExpressionTSysParams
    {
        /// <summary>
        /// 标识
        /// </summary>
        public static Expression<Func<TSysParams, Int32>> ID => m => m.ID;
        /// <summary>
        /// 键名
        /// </summary>
        public static Expression<Func<TSysParams, String>> Key => m => m.Key;
        /// <summary>
        /// 键值
        /// </summary>
        public static Expression<Func<TSysParams, String>> Value => m => m.Value;
        /// <summary>
        /// 备注
        /// </summary>
        public static Expression<Func<TSysParams, String>> Memo => m => m.Memo;
    }
}
