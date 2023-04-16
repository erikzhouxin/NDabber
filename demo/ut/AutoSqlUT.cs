using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Data.Extter;
using System.Data.Sqller;
using System.IO;
using System.Linq.Expressions;
using System.Text;

namespace System.Data.DabberUT
{
    [TestClass]
    public class AutoSqlUT
    {
        #region // SqlBuilder
        /// <summary>
        /// 标识
        /// </summary>
        public static Expression<Func<TSysParams, Int32>> ExpID => m => m.ID;
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
                    .Column(ExpID)
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
        #region // 测试自动生成SQL
        [TestMethod]
        public void TestSQLite()
        {
            Console.WriteLine(AutoSQLiteBuilder.Builder<TSysSettings>().Create);
            Console.WriteLine(AutoSQLiteBuilder.Builder<TSysSettings>().Update);
            Console.WriteLine(AutoSQLiteBuilder.Builder<TSysSettings>().Select);
        }
        [TestMethod]
        public void TestMySQL()
        {
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine(AutoMySqlBuilder.Builder<TSysSettings>().Create);
                Console.WriteLine(AutoMySqlBuilder.Builder<TSysSettings>().Update);
                Console.WriteLine(AutoMySqlBuilder.Builder<TSysSettings>().Select);
            }
        }
        [TestMethod]
        public void TestCreateSql()
        {
            var parent = AutoSQLiteBuilder.Builder<TestAttrParent>();
            var child = AutoSQLiteBuilder.Builder<TestAttrChild>();
            Console.WriteLine(parent.Select);
            Console.WriteLine(child.Select);
            Console.WriteLine(parent.Replace);

            /* SELECT[id] AS[Id],[name] AS[name],[age] AS[age] FROM[TestAttrParent]
     SELECT[Id] AS[Id],[Tester] AS[Tester],[age] AS[age] FROM [TestAttrChild]
    SELECT [id] AS [Id],[name] AS [name],[age] AS [age] FROM [TestAttrParent]
    SELECT [Id] AS [Id],[Tester] AS [Tester],[age] AS [age] FROM [TestAttrChild]

            */
        }
        #endregion // 测试自动生成SQL
        #region // 测试类
        [DbCol("Test", Name = "TestAttrParent")]
        internal class TestAttrParent
        {
            [DbCol("标识", Name = "id", Key = DbIxType.APK)]
            public virtual int Id { get; set; }

            [DbCol("标识", Name = "name")]
            public virtual string Name { get; set; }
            [DbCol("标识", Name = "age")]
            public virtual int Age { get; set; }
            public virtual DateTime Birthday { get; set; }
        }
        [DbCol("Test", Name = "TestAttrChild")]
        internal class TestAttrChild : TestAttrParent
        {
            [DbCol("id2", Name = "id2")]
            public override int Id { get; set; }
            public override string Name { get; set; }
            [DbCol("tester")]
            public virtual string Tester { get; set; }
        }
        #endregion 测试类
    }
}
