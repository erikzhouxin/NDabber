using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.DabberUT
{
    /// <summary>
    /// 反射信息测试
    /// </summary>
    [TestClass]
    public class ReflectionInfoUT
    {
        public object Name { get; private set; }

        [TestMethod]
        public void MyTestMethod()
        {
            var test = TestTry.GetMemberFullName(() => Name);
            Console.WriteLine(test);
        }

        public void TestClass(string name)
        {

        }
    }
}
