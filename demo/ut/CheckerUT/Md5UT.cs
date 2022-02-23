using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Security.Cryptography;
using System.Text;

namespace System.Data.DabberUT.CheckerUT
{
    /// <summary>
    /// MD5测试
    /// </summary>
    [TestClass]
    public class Md5UT
    {
        [TestMethod]
        public void MyTestMethod()
        {
            var passSalt = Encoding.UTF8.GetBytes("Abc123!@#");
            var md5Hash1 = MD5.Create().ComputeHash(passSalt);
            var md5Hash2 = new MD5CryptoServiceProvider().ComputeHash(passSalt);
            var md5String1 = UserPassword.GetByte16String(md5Hash1);
            var md5String2 = UserPassword.GetByte16String(md5Hash2);
            Assert.AreEqual(md5String1, md5String2);
        }
    }
}
