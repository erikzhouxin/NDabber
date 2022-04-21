using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Data.Extter;
using System.Linq.Expressions;
using System.Text;

namespace NEamsUT.CodeGener
{
    /// <summary>
    /// GUID
    /// </summary>
    [TestClass]
    public class CodeGenerUT
    {
        /// <summary>
        /// 创建
        /// </summary>
        [TestMethod]
        public void Create()
        {
            var times = 10;
            for (int i = 0; i < times; i++)
            {
                Console.WriteLine(Guid.NewGuid().GetString());
            }
        }
    }
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
