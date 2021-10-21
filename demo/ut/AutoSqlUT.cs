using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Data.Extter;
using System.Text;

namespace System.Data.DabberUT
{
    [TestClass]
    public class AutoSqlUT
    {
        [TestMethod]
        public void MyTestMethod()
        {
            Console.WriteLine(AutoSQLiteBuilder.Builder<TSysSettings>().Create);
            Console.WriteLine(AutoSQLiteBuilder.Builder<TSysSettings>().Update);
            Console.WriteLine(AutoSQLiteBuilder.Builder<TSysSettings>().Select);
        }
    }
}
