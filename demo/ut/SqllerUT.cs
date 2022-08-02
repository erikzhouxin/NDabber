using Microsoft.VisualStudio.TestTools.UnitTesting;
using NEamsUT.CodeGener;
using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Data.Extter;
using System.Data.Sqller;
using System.IO;
using System.Text;

namespace System.Data.DabberUT
{
    /// <summary>
    /// Sqlite脚本测试
    /// </summary>
    [TestClass]
    public class SqllerUT
    {
        #region // SqlBuilder
        [TestMethod]
        public void SqlBuilderUT()
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
                    .SetBParam(nameof(TSysParams.Key), nameof(TSysParams.Key))
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
            var path = Path.GetFullPath(Directory.GetCurrentDirectory());
            var typeBuilder = ExtterBuilder.BuilderContent(typeof(TSysParams));
            File.WriteAllText(Path.Combine(path, "ExpressionLambda.cs"), typeBuilder.ToString());
        }
        [TestMethod]
        public void TestExpressionToSql()
        {
            ITestClass model = CobberBuilder.CreateSampleInstance<ITestClass>();
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
        #endregion
        #region // SqlScript
        [TestMethod]
        [DataRow(StoreType.SQLite)]
        [DataRow(StoreType.MySQL)]
        [DataRow(StoreType.SqlServer)]
        public void InsertTest(StoreType storeType)
        {
            var insertSqls = GetInsertSqls(storeType);
            Console.WriteLine(insertSqls);

            var updateSqls = GetUpdateSqls(storeType);
            Console.Write(updateSqls);
        }
        public StringBuilder GetInsertSqls(StoreType storeType)
        {
            return new StringBuilder()
                .AppendLine(SqlScriptBuilder.CreateInsert(storeType).From<TSysParams>().SqlScript)
                .AppendLine(SqlScriptBuilder.CreateInsert(storeType).From(typeof(TSysSettings)).SqlScript)
                .AppendLine(SqlScriptBuilder.CreateInsert(storeType).UseSelect<TSysParams>().Select(SqlScriptBuilder.CreateSimpleSelect(storeType).FromWhere<TSysParams>()).SqlScript)
                .AppendLine(SqlScriptBuilder.CreateInsert(storeType).UseSelect(typeof(TSysSettings)).Select(SqlScriptBuilder.CreateSimpleSelect(storeType).FromWhere(typeof(TSysSettings))).SqlScript);
        }
        public StringBuilder GetUpdateSqls(StoreType storeType)
        {
            return new StringBuilder()
                .AppendLine(SqlScriptBuilder.CreateUpdate(storeType).From<TSysParams>().SetAParam(nameof(TSysParams.Key)).SetBParam(nameof(TSysParams.Value), "Value").Where().AndEqualAParam("ID", 123).SqlScript)
                .AppendLine(SqlScriptBuilder.CreateUpdate(storeType).From(typeof(TSysParams)).SetClause("Value = IFNULL(Value,'',Value)").Where().SqlScript)
                .AppendLine(SqlScriptBuilder.CreateUpdate(storeType).FromWhere<TSysParams>().SqlScript)
                .AppendLine(SqlScriptBuilder.CreateUpdate(storeType).FromWhere(typeof(TSysSettings)).SqlScript);
        }
        #endregion
    }
}
