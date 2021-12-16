using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Builbber;
using System.Data.Extter;
using System.Data.Sqller;
using System.IO;
using System.Text;

namespace System.Data.DabberUT
{
    /// <summary>
    /// Sql创建者测试
    /// </summary>
    [TestClass]
    public class SqlBuilderUT
    {
        [TestMethod]
        public void MyTestMethod()
        {
            var times = 100000;
            var now = DateTime.Now;
            var sql = string.Empty;
            var typeSql = string.Empty;

            now = DateTime.Now;
            for (int i = 0; i < times; i++)
            {
                sql = SqlScriptBuilder.CreateSimpleSelect(Cobber.StoreType.SQLite)
                    .From<TSysParams>()
                    .Select.Add(nameof(TSysParams.Key), nameof(TSysParams.Key))
                    .Column(ExpressionTSysParams.ID)
                    .Column<TSysParams>(m => m.ID)
                    .Where.AndEqual(nameof(TSysParams.ID), 123).SqlScript;
                typeSql = SqlScriptBuilder.CreateSimpleSelect(Cobber.StoreType.SQLite).FromWhere<TSysParams>().SqlScript;
            }
            Console.WriteLine(DateTime.Now - now);
            Console.WriteLine(sql);
            Console.WriteLine(typeSql);
        }

        [TestMethod]
        public void TestExpressionCode()
        {
            var path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "CodeGeners"));
            var typeBuilder = ExpressionPropertyBuilder.BuilderContent(typeof(TSysParams));
            File.WriteAllText(Path.Combine(path, "ExpressionLambda.cs"), typeBuilder.ToString());
        }
    }
}
