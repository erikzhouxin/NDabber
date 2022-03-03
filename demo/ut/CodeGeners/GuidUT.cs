using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Text;

namespace NEamsUT.CodeGener
{
    /// <summary>
    /// GUID
    /// </summary>
    [TestClass]
    public class GuidUT
    {
        /// <summary>
        /// 创建
        /// </summary>
        [TestMethod]
        public void Create()
        {
            var times = 10;
            for (int i = 0; i < times; i++)
            {
                Console.WriteLine(Guid.NewGuid().GetString());
            }
        }
    }
}
