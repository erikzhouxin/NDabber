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
    public class CyptoUT
    {
        [TestMethod]
        public void TestMD5()
        {
            var passSalt = Encoding.UTF8.GetBytes("Abc123!@#");
            var md5Hash1 = MD5.Create().ComputeHash(passSalt);
            var md5Hash2 = new MD5CryptoServiceProvider().ComputeHash(passSalt);
            var md5String1 = UserPassword.GetHexString(md5Hash1);
            var md5String2 = UserPassword.GetHexString(md5Hash2);
            Assert.AreEqual(md5String1, md5String2);
        }
        [TestMethod]
        public void TestSha512()
        {
            var passSalt = Encoding.UTF8.GetBytes("Abc123!@#");
            var md5Hash1 = SHA512.Create().ComputeHash(passSalt);
            var md5Hash2 = new SHA512CryptoServiceProvider().ComputeHash(passSalt);
            var md5String1 = UserPassword.GetHexString(md5Hash1);
            var md5String2 = UserPassword.GetHexString(md5Hash2);
            Assert.AreEqual(md5String1, md5String2);
            Console.WriteLine(UserPassword.GetHexString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes("EZhouXin")), false));
        }
    }
}
