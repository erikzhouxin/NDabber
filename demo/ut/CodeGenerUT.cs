using Microsoft.VisualStudio.TestTools.UnitTesting;
using NPOI.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
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
                    .SetPreTable("T", "V")
                    .SetIgnoreTableOrColumn(ignoreTables)
                    .GetCodeSingle(new MySql.Data.MySqlClient.MySqlConnection(connString));
                File.WriteAllText(Path.GetFullPath(fileName), sb.ToString());
            }
        }
        [TestMethod]
        public void CreateEnumFlags()
        {
            // ConsoleEnumFlags(0, 32, (v) => $"DO_{v + 1}", (v) => $"DO-{v + 1}", (v) => $"{(ulong)Math.Pow(2, v)}U");
            // ConsoleEnumFlags(32, 64, (v) => $"DI_{v - 31}", (v) => $"DI-{v - 31}", (v) => $"{(ulong)Math.Pow(2, v)}U");

            var ticks1 = DateTime.Now.Ticks;
            var nos = GetNoType(18446744073709551615U);
            var ticks2 = DateTime.Now.Ticks;
            var nos2 = GetNoType2(18446744073709551615U);
            var ticks3 = DateTime.Now.Ticks;
            var nos3 = GetNoType3(18446744073709551615U);
            var ticks4 = DateTime.Now.Ticks;
            for (int i = 0; i < 100; i++) { nos = GetNoType(18246744073709551625U); }
            var ticks5 = DateTime.Now.Ticks;
            for (int i = 0; i < 100; i++) { nos2 = GetNoType2(18246744073709551625U); }
            var ticks6 = DateTime.Now.Ticks;
            for (int i = 0; i < 100; i++) { nos3 = GetNoType(18246744073709551625U); }
            var ticks7 = DateTime.Now.Ticks;
            Console.WriteLine($"第一次:{ticks2 - ticks1}");
            Console.WriteLine($"第二次:{ticks3 - ticks2}");
            Console.WriteLine($"第三次:{ticks4 - ticks3}");
            Console.WriteLine($"第一次:{ticks5 - ticks4}");
            Console.WriteLine($"第二次:{ticks6 - ticks5}");
            Console.WriteLine($"第三次:{ticks7 - ticks6}");
            Console.WriteLine(nos.GetJsonString());
            Assert.AreEqual(nos.GetJsonString(), nos2.GetJsonString());
            Assert.AreEqual(nos.GetJsonString(), nos3.GetJsonString());
        }

        private static void ConsoleEnumFlags(int start, int end, Func<int, string> enumName, Func<int, string> enumDesc, Func<int, string> enumValue)
        {
            var sb = new StringBuilder();
            for (int i = start; i < end; i++)
            {
                sb.AppendLine("/// <summary>")
                     .AppendLine($"/// {enumDesc(i)}")
                     .AppendLine("/// </summary>")
                     .AppendLine($"[EDisplay(\"{enumDesc(i)}\")]")
                     .AppendLine($"{enumName(i)} = {enumValue(i)},");
            }
            Console.WriteLine(sb);
        }
        /// <summary>
        /// 获取序号值
        /// </summary>
        /// <returns></returns>
        public static Tuble<int[], int[]> GetNoType2(ulong typeVal)
        {
            var outList = new List<int>(7);
            for (int i = 0; i < 32; i++)
            {
                if (((typeVal >> i) & 1) == 1) { outList.Add(i); }
            }
            var inList = new List<int>(7);
            for (int i = 32; i < 64; i++)
            {
                if (((typeVal >> i) & 1) == 1) { inList.Add(i - 32); }
            }
            return new Tuble<int[], int[]>(outList.ToArray(), inList.ToArray());
        }
        /// <summary>
        /// 获取序号值
        /// </summary>
        /// <returns></returns>
        public static Tuble<int[], int[]> GetNoType(ulong typeVal)
        {
            return new Tuble<int[], int[]>(InnerGetNoType((uint)typeVal), InnerGetNoType((uint)(typeVal >> 32)));
            static int[] InnerGetNoType(uint outVal)
            {
                var outList = new List<int>(7);
                for (int i = 0; i < 32; i++)
                {
                    if (((outVal >> i) & 1) == 1) { outList.Add(i); }
                }
                return outList.ToArray();
            }
        }
        /// <summary>
        /// 获取序号值 推荐使用
        /// </summary>
        /// <returns></returns>
        public static Tuble<int[], int[]> GetNoType3(ulong typeVal)
        {
            var bitArray = new BitArray(typeVal.GetBytes());
            var outList = new List<int>(7);
            for (int i = 0; i < 32; i++)
            {
                if (bitArray[i]) { outList.Add(i); }
            }
            var inList = new List<int>(7);
            for (int i = 32; i < 64; i++)
            {
                if (bitArray[i]) { inList.Add(i - 32); }
            }
            return new Tuble<int[], int[]>(outList.ToArray(), inList.ToArray());
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