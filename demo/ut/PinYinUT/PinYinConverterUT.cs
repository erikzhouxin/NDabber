using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Chineser;
using System.Text;

namespace System.Data.DabberUT.PinYinUT
{
    /// <summary>
    /// 拼音转换测试
    /// </summary>
    [TestClass]
    public class PinYinConverterUT
    {
        [TestMethod]
        public void MyTestMethod()
        {
            Assert.IsTrue(ChineseChar.IsValidPinyin("ni"));
            Assert.IsTrue(ChineseChar.IsValidChar('啊'));
        }
    }
}
