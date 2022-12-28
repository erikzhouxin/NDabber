using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Chineser;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

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
        [TestMethod]
        public void TestCarNo()
        {
            var pattern = @"^([京津沪渝冀豫云辽黑湘皖鲁新苏浙赣鄂桂甘晋蒙陕吉闽贵粤青藏川宁琼使领]{1}[a-zA-Z](([DF]((?![IO])[a-zA-Z0-9](?![IO]))[0-9]{4})|([0-9]{5}[DF]))|[京津沪渝冀豫云辽黑湘皖鲁新苏浙赣鄂桂甘晋蒙陕吉闽贵粤青藏川宁琼使领]{1}[A-Z]{1}[A-Z0-9]{4}[A-Z0-9挂学警港澳]{1})$";
            var regex = new Regex(pattern);
            var carnos = new List<string>
            {
                "宁A123456",
                "宁ADDDF6",
                "宁A12345",
                "宁112345",
                "宁A1234",
                "宁AD12345",
                "宁A1234挂",
                "宁A123挂",
                "宁61234挂",
                "宁A12345D"
            };
            foreach (var item in carnos)
            {
                Console.WriteLine($"{item.PadRight(10, ' ')} => {regex.IsMatch(item)}");
            }

        }
    }
}
