﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Data.Dabber;
using System.Data.Extter;
using System.Data.Odbc;
using System.Data.SQLiteCipher;
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
            ITestNew newTest = CobberBuilder.CreateSampleInstance<ITestNew>();
            ITestClass model = CobberBuilder.CreateSampleInstance<ITestClass>();
            var access = PropertyAccess.GetAccess(model);
            access.FuncSetValue(model, nameof(ITestClass.ID), 1);
            access.FuncSetValue(model, nameof(ITestClass.Name), "周鑫");
            access.FuncSetValue(model, nameof(ITestClass.DateTime), DateTime.Now);
            Console.WriteLine(model.GetJsonString());
        }
        public interface ITestNew
        {
            string Name { get; }
            int ID { get; }
        }
        public interface ITestClass : ITestNew
        {
            int Age { get; }
            DateTime DateTime { get; }

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
        #region // 测试连接

        [TestMethod]
        public void TestOdbcMySql()
        {
            string MyConString = "DRIVER={MySQL ODBC 8.0 Unicode Driver};SERVER=localhost;DATABASE=c_qmsr;UID=root;PASSWORD=root;OPTION=3";

            var MyConnection = new OdbcConnection(MyConString);
            MyConnection.Open();
            Console.WriteLine(" success, connected successfully ! ");
        }

        #endregion 测试连接
    }
    /// <summary>
    /// 修改密码
    /// </summary>
    [TestClass]
    public class ChangePasswordUT
    {
        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void MyTestMethod()
        {
            var sqliteConnString = $"DataSource={System.IO.Path.GetFullPath(DateTime.Now.Date.Ticks + @".sqlite;")};password=456";
            using (var sqlite = new SqliteConnection(sqliteConnString))
            {
                sqlite.Open();
                Tester(sqlite);
                sqlite.ChangePassword();
            }
        }

        private static void Tester(SqliteConnection sqlite)
        {
            try
            {
                sqlite.Execute($"CREATE TABLE IF NOT EXISTS [TestDate{DateTime.Now.Date:yyyyMMdd}]([ID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,[Name] TEXT NOT NULL,[ETime] DATETIME NOT NULL DEFAULT (datetime(CURRENT_TIMESTAMP,'localtime')))", null);
                sqlite.Execute($"INSERT INTO [TestDate{DateTime.Now.Date:yyyyMMdd}](Name) VALUES(@Name)", new List<Object> { new { Name = "二蛋" }, new { Name = "三蛋" }, });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
