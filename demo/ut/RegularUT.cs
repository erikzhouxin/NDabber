using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace System.Data.DabberUT
{
    /// <summary>
    /// 通用测试类
    /// </summary>
    [TestClass]
    public class RegularUT
    {
        [TestMethod]
        public void DoubleAdd()
        {
            double n = 171.6;
            double m = 28.17;
            double k = n + m;
            Console.WriteLine(k);
        }
        [TestMethod]
        public void DoubleNAZero()
        {
            double n = 0;
            double m = -0;
            Console.WriteLine(n);
            Console.WriteLine(-m);
            Console.WriteLine(-n == 0);
            Console.WriteLine(-m == 0);
        }
    }
}
