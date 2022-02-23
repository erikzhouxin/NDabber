using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Data.Dabber;
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
            var times = 10000;
            var now = DateTime.Now;
            var sql = string.Empty;
            var typeSql = string.Empty;
            var updateSql = string.Empty;

            now = DateTime.Now;
            for (int i = 0; i < times; i++)
            {
                sql = SqlScriptBuilder.CreateSimpleSelect(StoreType.SQLite)
                    .From<TSysParams>()
                    .Select().Add(nameof(TSysParams.Key), nameof(TSysParams.Key))
                    .Column(ExpressionTSysParams.ID)
                    .Column<TSysParams>(m => m.ID)
                    .Where().AndEqualAParam(nameof(TSysParams.ID), 123).SqlScript;
                typeSql = SqlScriptBuilder.CreateSimpleSelect(StoreType.SQLite).FromWhere<TSysParams>().SqlScript;
                updateSql = SqlScriptBuilder.CreateUpdate(StoreType.SQLite)
                    .From<TSysParams>()
                    .SetBParam(nameof(TSysParams.Key),nameof(TSysParams.Key))
                    .Where().AndEqualAParam(nameof(TSysParams.ID), 123)
                    .SqlScript;
            }
            Console.WriteLine(DateTime.Now - now);
            Console.WriteLine(sql);
            Console.WriteLine(typeSql);
            Console.WriteLine(updateSql);
        }

        [TestMethod]
        public void TestExpressionCode()
        {
            var path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "CodeGeners"));
            var typeBuilder = ExpressionPropertyBuilder.BuilderContent(typeof(TSysParams));
            File.WriteAllText(Path.Combine(path, "ExpressionLambda.cs"), typeBuilder.ToString());
        }
        [TestMethod]
        public void TestExpressionToSql()
        {
            ITestClass model = SampleClassBuilder<ITestClass>.CreateInstance();
            model.ID = 1;
            model.Name = "周鑫";
            Console.WriteLine(model.GetJsonString());
        }

        public interface ITestClass 
        {
            string Name { get; set; }
            int ID { get; set; }
            int Age { get; set; }
            DateTime DateTime { get; set; }

        }
    }
}
