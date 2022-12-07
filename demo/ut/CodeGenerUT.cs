using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Data.Dobber;
using System.Data.Extter;
using System.IO;
using System.Linq.Expressions;
using System.Text;

namespace System.Data.DabberUT
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
        [TestMethod]
        public void CreateContextEntities()
        {
            var jsonString = File.ReadAllText(Path.GetFullPath("CodeContextEntities.Json"));
            var values = jsonString.GetJsonObject();
            foreach (dynamic item in values)
            {
                var connString = (string)item.ConnString;
                var fileName = (string)item.FileName; // 相对路径或绝对路径
                var nameSpace = (string)item.NameSpace;
                var ignoreTables = ((string)item.IgnoreTables).Split(","); // 使用逗号(,)隔开表名
                var sb = ContextEntitiesBuilder.Create(StoreType.MySQL)
                    .SetNamespace(nameSpace)
                    .SetPreTable("")
                    .SetIgnoreTableOrColumn(ignoreTables)
                    .GetCodeSingle(new MySql.Data.MySqlClient.MySqlConnection(connString));
                File.WriteAllText(Path.GetFullPath(fileName), sb.ToString());
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