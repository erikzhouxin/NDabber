using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Dabber;
using System.Data.SQLiteCipher;
using System.Linq;
using System.Text;

namespace System.Data.DabberUT
{
    public class DbConnUT
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var conn = @"DataSource=C:\Users\Admin\Documents\CenIdea\Qualimetry\CenIdea.Qualimetry.NEamsUI.v4.5.sqlite;Password=Eams2020!@#;";
            var conner = SqliteConnectionPool.GetConnection(conn);
            conner.Open();
            var res = conner.Query("select * from sqlite_master");
            var count = res.Count();
            Console.WriteLine(count);
        }
    }
}
