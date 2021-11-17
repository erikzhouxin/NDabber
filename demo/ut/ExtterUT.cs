using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Extter;
using System.Text;

namespace System.Data.DabberUT
{
    /// <summary>
    /// 扩展测试
    /// </summary>
    [TestClass]
    public class ExtterUT
    {
        #region // AssemblyTypeCaller
        [TestMethod]
        public void TestAssemblyType()
        {
            foreach (var item in typeof(AssemblyTypeCaller).GetTypes())
            {
                Console.WriteLine(item.Name);
            }
            Console.WriteLine("-------------------------------------");
            foreach (var item in typeof(AssemblyTypeCaller).GetTypes(typeof(AssemblyTypeCaller).Namespace))
            {
                Console.WriteLine(item.Name);
            }
        }
        #endregion
    }
}
