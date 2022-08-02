using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Chineser;
using System.Globalization;
using System.Text;

namespace System.Data.DabberUT.PinYinUT
{
    /// <summary>
    /// 拼音转换测试
    /// </summary>
    [TestClass]
    public class ChineserUT
    {
        [TestMethod]
        public void MyTestMethod()
        {
            Assert.IsTrue(ChineseChar.IsValidPinyin("ni"));
            Assert.IsTrue(ChineseChar.IsValidChar('啊'));
        }
        [TestMethod]
        public void TestCulture()
        {
            var cn = new CultureInfo("zh-CN");
            var cn1 = new CultureInfo("zh-cn");
            var cn2 = new CultureInfo("ZH-CN");
            Assert.AreEqual(cn.Name, cn1.Name);
            Assert.AreEqual(cn.Name, cn2.Name);
            Console.WriteLine($"{cn.Name} {cn1.Name} {cn2.Name}");
        }
    }
}
