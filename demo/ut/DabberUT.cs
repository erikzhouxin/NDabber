using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Dabber;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.DabberUT
{
    [TestClass]
    public class DabberUT
    {
        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void TestPureMySQL()
        {
            var mysqlConnString = "server=192.168.1.110;database=qms;uid=root;pwd=root;port=3306;sslmode=none;";
            var times = 1000;
            DateTime dt;

            dt = DateTime.Now;
            DoSomething(CreateConnection(mysqlConnString));
            Console.WriteLine(DateTime.Now - dt);
            dt = DateTime.Now;
            DoSomething(CreateConnection(mysqlConnString));
            Console.WriteLine(DateTime.Now - dt);
            dt = DateTime.Now;
            for (int i = 0; i < times; i++)
            {
                DoSomething(CreateConnection(mysqlConnString));
            }
            Console.WriteLine(DateTime.Now - dt);
            dt = DateTime.Now;
            var pa = Parallel.For(0, times, i =>
            {
                DoSomething(CreateConnection(mysqlConnString));
            });
            while (true)
            {
                if (pa.IsCompleted) { break; }
            }
            Console.WriteLine(DateTime.Now - dt);
        }

        private static void DoSomething(DbConnection conn)
        {
            using (conn)
            {
                conn.Open();
                var test = conn.QueryFirstOrDefault<int>("SELECT COUNT(*) FROM `t_contract`");
                Assert.AreEqual(test, 10);
            }
        }

        [TestMethod]
        public void TestPoolMySQL()
        {
            var mysqlConnString = "server=192.168.1.110;database=qms;uid=root;pwd=root;port=3306;sslmode=none;ConnectionTimeout=6000;";
            var times = 100;
            DateTime dt;

            dt = DateTime.Now;
            DoSomething(DbConnPool<MySqlConnection>.GetConnection(mysqlConnString, CreateConnection));
            Console.WriteLine(DateTime.Now - dt);
            dt = DateTime.Now;
            DoSomething(DbConnPool<MySqlConnection>.GetConnection(mysqlConnString, CreateConnection));
            Console.WriteLine(DateTime.Now - dt);
            dt = DateTime.Now;
            for (int i = 0; i < times; i++)
            {
                DoSomething(DbConnPool<MySqlConnection>.GetConnection(mysqlConnString, CreateConnection));
            }
            Console.WriteLine(DateTime.Now - dt);
            dt = DateTime.Now;
            var pa = Parallel.For(0, times, i =>
            {
                DoSomething(DbConnPool<MySqlConnection>.GetConnection(mysqlConnString, CreateConnection));
            });
            while (true)
            {
                if (pa.IsCompleted) { break; }
            }
            Console.WriteLine(DateTime.Now - dt);
        }

        private MySqlConnection CreateConnection(string arg)
        {
            return new MySqlConnection(arg);
        }
    }
}
