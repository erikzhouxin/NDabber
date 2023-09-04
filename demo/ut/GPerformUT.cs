using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLitePCL.Raw.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Dabber;
using System.Data.SQLiteCipher;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.DabberUT.Performances
{
    /// <summary>
    /// 转换字符性能
    /// </summary>
    [TestClass]
    public class PerformUT
    {
        [TestMethod]
        public void NumberToHexString()
        {
            int times = 100000;
            // 开始
            StartNumberToHex(times, DateTime.Now);
            Console.WriteLine($"----------------------------");
            StartNumberToHex(times, DateTime.Now);
        }

        private static void StartNumberToHex(int times, DateTime now)
        {
            now = DateTime.Now;
            for (int i = 0; i < times; i++)
            {
                i.ToString("X2");
            }
            Console.WriteLine($"ToString:{DateTime.Now - now}");
            now = DateTime.Now;
            for (int i = 0; i < times; i++)
            {
                string.Format("X2", i);
            }
            Console.WriteLine($"Format:{DateTime.Now - now}");
        }
    }
    /// <summary>
    /// PerformanceUT 的摘要说明
    /// </summary>
    [TestClass]
    public class PerformanceUT
    {
        /// <summary>
        /// sqlite连接字符串
        /// </summary>
        /// <returns></returns>
        public static string SQLiteConnectionString { get; } = $"DataSource={System.IO.Path.GetFullPath("System.Data.sqlite3")}";
        /// <summary>
        /// sqlite连接字符串
        /// </summary>
        /// <returns></returns>
        public static string SqliteConnectionString { get; } = $"DataSource={System.IO.Path.GetFullPath("Microsoft.Data.sqlite3")}";
        /// <summary>
        /// 构造
        /// </summary>
        public PerformanceUT() { }
        /// <summary>
        /// 测试方法一
        /// </summary>
        [TestMethod]
        public void TestMethod1()
        {
            for (int i = 0; i < 3; i++)
            {
                TestMethod2();
            }
        }
        /// <summary>
        /// 测试方法二
        /// </summary>
        public void TestMethod2()
        {
            var dt = DateTime.Now;
            using (var conn = new System.Data.SQLiteCipher.SqliteConnection(SqliteConnectionString + ".2;Password=Test"))
            {
                TestTimes(conn);
            }
            Console.WriteLine(DateTime.Now - dt);
            dt = DateTime.Now;
            using (var conn = new System.Data.SQLite.SQLiteConnection(SQLiteConnectionString))
            {
                TestTimes(conn);
            }
            Console.WriteLine(DateTime.Now - dt);
            dt = DateTime.Now;
            using (var conn = new System.Data.SQLiteCipher.SqliteConnection(SqliteConnectionString))
            {
                TestTimes(conn);
            }
            Console.WriteLine(DateTime.Now - dt);
            System.IO.File.Delete(System.IO.Path.GetFullPath("System.Data.sqlite3"));
            System.IO.File.Delete(System.IO.Path.GetFullPath("Microsoft.Data.sqlite3"));
            System.IO.File.Delete(System.IO.Path.GetFullPath("Microsoft.Data.sqlite3.2"));
            Console.WriteLine();
            Console.WriteLine();
        }

        private static void TestTimes(DbConnection conn)
        {
            var dt = DateTime.Now;
            conn.Open();
            Console.Write($"打开:{(DateTime.Now - dt).Ticks:00000000}    ");
            dt = DateTime.Now;
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS [TEST_PERFORMANCE]([ID] TEXT NOT NULL,[NAME] TEXT NOT NULL,[TIME] INTEGER NOT NULL)";
                cmd.ExecuteNonQuery();
                Console.Write($"创建表:{(DateTime.Now - dt).Ticks:00000000}    ");
                dt = DateTime.Now;
                cmd.CommandText = "INSERT INTO [TEST_PERFORMANCE]([ID],[NAME],[TIME]) VALUES(@ID,@NAME,@TIME)";
                var objectList = new List<Object>();
                for (int i = 0; i < 10; i++)
                {
                    cmd.Parameters.Clear();
                    var model = new
                    {
                        ID = Guid.NewGuid(),
                        NAME = $"Test-{i}",
                        TIME = DateTime.Now.Ticks,
                    };
                    objectList.Add(model);
                    AddCmdParameter(cmd, "@ID", model.ID);
                    AddCmdParameter(cmd, "@NAME", model.NAME);
                    AddCmdParameter(cmd, "@TIME", model.TIME);
                    cmd.ExecuteNonQuery();
                }
                Console.Write($"插入10条:{(DateTime.Now - dt).Ticks:00000000}    ");
                dt = DateTime.Now;
                var objList = conn.Query("SELECT * FROM [TEST_PERFORMANCE]");
                Assert.IsTrue(objectList.Count() == objList.Count());
                Console.Write($"ORM查询10条:{(DateTime.Now - dt).Ticks:00000000}    ");
                dt = DateTime.Now;
                cmd.CommandText = "DELETE FROM [TEST_PERFORMANCE]";
                cmd.ExecuteNonQuery();
                Console.Write($"删除10条:{(DateTime.Now - dt).Ticks:00000000}    ");
                dt = DateTime.Now;
                conn.Execute("INSERT INTO [TEST_PERFORMANCE]([ID],[NAME],[TIME]) VALUES(@ID,@NAME,@TIME)", objList);
                Console.Write($"ORM插入10条:{(DateTime.Now - dt).Ticks:00000000}    ");
                dt = DateTime.Now;
                objList = conn.Query("SELECT * FROM [TEST_PERFORMANCE]");
                Assert.IsTrue(objectList.Count() == objList.Count());
                Console.Write($"ORM查询10条:{(DateTime.Now - dt).Ticks:00000000}    ");
                dt = DateTime.Now;
                cmd.CommandText = "DROP TABLE [TEST_PERFORMANCE]";
                cmd.ExecuteNonQuery();
                Console.Write($"删除表:{(DateTime.Now - dt).Ticks:00000000}    ");
                dt = DateTime.Now;
                Console.WriteLine();
            }
        }

        private static void AddCmdParameter(DbCommand cmd, string name, object value)
        {
            var param = cmd.CreateParameter();
            param.ParameterName = name;
            param.Value = value.ToString();
            cmd.Parameters.Add(param);
        }
        [TestMethod]
        public void TestDirectoryCreate()
        {
            string path;
            var dt = DateTime.Now;
            int times = 1000000;

            path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, $"{DateTime.Now.Ticks}"));
            dt = DateTime.Now;
            for (int i = 0; i < times; i++)
            {
                Directory.CreateDirectory(path);
            }
            Console.WriteLine(DateTime.Now - dt);

            path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, $"{DateTime.Now.Ticks}"));
            dt = DateTime.Now;
            for (int i = 0; i < times; i++)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            Console.WriteLine(DateTime.Now - dt);


        }
        /// <summary>
        /// 测试方法
        /// </summary>
        [TestMethod]
        public void MyTestMethod()
        {
            int times = 100;
            var sqliteConnString = $"DataSource={System.IO.Path.GetFullPath($"{GetType().Name}.sqlite;")};password=cenidea2020;";
            using (var connCreate = SqliteConnectionPool.GetConnection(sqliteConnString))
            {
                connCreate.Open();
                connCreate.Execute($"CREATE TABLE IF NOT EXISTS [{DateTime.Now.Date.Ticks}](ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,NAME TEXT NOT NULL)", null);
            }
            var now = DateTime.Now;
            Task[] tasks = new Task[5];
            for (int i = 0; i < 5; i++)
            {
                var task = new Task(() =>
                {
                    for (int i = 0; i < times; i++)
                    {
                        using (var conn = SqliteConnectionPool.GetConnection(sqliteConnString))
                        {
                            DbTransaction trans = null;
                            try
                            {
                                conn.Open();
                                trans = conn.BeginTransaction();
                                conn.Execute($"INSERT INTO [{DateTime.Now.Date.Ticks}](NAME) VALUES('{DateTime.Now.Ticks}')", null, trans);
                                trans.Commit();
                            }
                            catch (Exception ex)
                            {
                                if (trans != null) { trans.Rollback(); }
                                Console.WriteLine(ex.Message);
                            }
                        }
                    }
                });
                tasks[i] = task;
                task.Start();
            }
            Task.WaitAll(tasks);
            Console.WriteLine("次数:{0} X 5 => 时间:{1}", times, DateTime.Now - now);
        }
        /// <summary>
        /// 测试COUNT和ANY性能
        /// </summary>
        [TestMethod]
        public void TestCountAny()
        {
            var times = 10000000;
            var list = new List<int>();
            var listObj = new List<TestListObject>();
            var listString = new List<string>();
            for (int i = 0; i < times; i++)
            {
                list.Add(i);
                var day = DateTime.Now.AddMinutes(i);
                listString.Add(day.ToString("yyyy-MM-dd"));
                listObj.Add(new TestListObject
                {
                    Id = i,
                    Name = $"test{i:######}",
                    Time = day,
                });
            }
            var objCount = 0;
            var time = DateTime.Now;
            for (int i = 0; i < times; i++)
            {
                objCount = listObj.Count();
            }
            Console.Write("对象计数:{0}", DateTime.Now - time);
            var strCount = 0;
            time = DateTime.Now;
            for (int i = 0; i < times; i++)
            {
                strCount = listString.Count();
            }
            Console.Write("字符计数:{0}", DateTime.Now - time);
            var count = 0;
            time = DateTime.Now;
            for (int i = 0; i < times; i++)
            {
                count = list.Count();
            }
            Console.WriteLine("数字计数:{0}", DateTime.Now - time);
            var hasObj = false;
            time = DateTime.Now;
            for (int i = 0; i < times; i++)
            {
                hasObj = listObj.Count() > 0;
            }
            Console.Write("对象有无:{0}", DateTime.Now - time);
            var hasStr = false;
            time = DateTime.Now;
            for (int i = 0; i < times; i++)
            {
                hasStr = listString.Count() > 0;
            }
            Console.Write("字符有无:{0}", DateTime.Now - time);
            var has = false;
            time = DateTime.Now;
            for (int i = 0; i < times; i++)
            {
                has = list.Count() > 0;
            }
            Console.WriteLine("数字有无:{0}", DateTime.Now - time);
            time = DateTime.Now;
            for (int i = 0; i < times; i++)
            {
                hasObj = listObj.Any();
            }
            Console.Write("对象存在:{0}", DateTime.Now - time);
            time = DateTime.Now;
            for (int i = 0; i < times; i++)
            {
                hasStr = listString.Any();
            }
            Console.Write("字符存在:{0}", DateTime.Now - time);
            time = DateTime.Now;
            for (int i = 0; i < times; i++)
            {
                has = list.Any();
            }
            Console.WriteLine("数字存在:{0}", DateTime.Now - time);

            Assert.IsTrue(objCount == strCount && strCount == count);
            Assert.IsTrue(hasObj && hasStr & has);
        }
        private class TestListObject
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime Time { get; set; }
        }
        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void TestGetRandom()
        {
            var random = new Random(1000000);
            Console.WriteLine(random.Next());
            Console.WriteLine(random.Next());
            Console.WriteLine(random.Next());
            Console.WriteLine(random.Next());
            Console.WriteLine(random.Next());
            Func<int> getRandom = () => 0;
            getRandom = () =>
            {
                var list = new List<int>();
                for (int i = 0; i < 100; i++)
                {
                    list.Add(i);
                }
                for (int i = 0; i < 10000; i++)
                {
                    var ra = random.Next(0, 1000000);
                    if (!list.Contains(ra))
                    {
                        return ra;
                    }
                }
                return getRandom();
            };
            var datetime = DateTime.Now;
            getRandom();
            Console.WriteLine(DateTime.Now - datetime);
        }
        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void TestGetRound()
        {
            for (int i = 0; i < 9; i++)
            {
                System.Console.WriteLine("{0}=>{1:0.00}", 6.605m + i * 0.01m, Math.Round(6.605m + i * 0.01m, 2, MidpointRounding.ToEven));
            }
            for (int i = 0; i < 9; i++)
            {
                System.Console.WriteLine("{0}=>{1:0.00}", 6.605 + i * 0.01, Math.Round(6.605 + i * 0.01, 2, MidpointRounding.ToEven));
            }
        }
        /// <summary>
        /// 测试
        /// </summary>
        [TestMethod]
        public void MyTestMethod22()
        {
            var times = 10;
            var sqliteConnString = $"DataSource={System.IO.Path.GetFullPath(@"SQLitePoolUT.sqlite;")};password=cenidea2020;";
            for (int i = 0; i < times; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    using (var conn = SqliteConnectionPool.GetConnection(sqliteConnString))
                    {
                        DbTransaction trans = null;
                        try
                        {
                            conn.Open();
                            trans = conn.BeginTransaction();
                            var table = DateTime.Now.Date.Ticks;
                            conn.Execute($"CREATE TABLE IF NOT EXISTS [{table}](ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,NAME TEXT NOT NULL)", null, trans);
                            conn.Execute($"INSERT INTO [{table}](NAME) VALUES('{DateTime.Now.Ticks}')", null, trans);
                            conn.Query<String>($"SELECT NAME FROM [{table}]", null, trans);
                            //conn.Execute($"DROP TABLE [{table}]", null, trans);
                            Thread.Sleep(1000);
                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            if (trans != null) { trans.Rollback(); }
                            Console.WriteLine(ex.Message);
                        }
                    }
                });
            }
            Thread.Sleep(30000);
        }
    }
}
