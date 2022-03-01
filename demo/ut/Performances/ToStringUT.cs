using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace System.Data.DabberUT.Performances
{
    /// <summary>
    /// 转换字符性能
    /// </summary>
    [TestClass]
    public class ToStringUT
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
}
