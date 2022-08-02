using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Data.Extter;
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
        public void TestEncoding()
        {
            var len = 27;
            var nor = 20;
            Console.WriteLine("{0}   {1}   {2}   {3}", "EncodingName".PadRight(len, ' '), "WebName".PadRight(nor, ' '), "HeaderName".PadRight(nor, ' '), "BodyName".PadRight(nor, ' '));
            foreach (EncodingInfo encodeInfo in Encoding.GetEncodings())
            {
                var encode = encodeInfo.GetEncoding();
                Console.WriteLine("{0} | {1} | {2} | {3}", encode.EncodingName.PadRight(len, ' '), encode.WebName.PadRight(nor, ' '), encode.HeaderName.PadRight(nor, ' '), encode.BodyName.PadRight(nor, ' '));
            }
        }
        [TestMethod]
        public void TestCrypto()
        {
            var jsonString = System.IO.File.ReadAllText(System.IO.Path.GetFullPath("CryptoValues.Json"));
            var values = jsonString.GetJsonObject();
            foreach (String item in values)
            {
                Console.WriteLine($"原密码  => {item}");
                Console.WriteLine($"MD5     => {UserPassword.GetMd5Hash(item)}");
                Console.WriteLine($"SHA1    => {UserPassword.GetSha1Hash(item)}");
                Console.WriteLine($"SHA256  => {UserPassword.GetSha256Hash(item)}");
                Console.WriteLine($"SHA384  => {UserPassword.GetSha384Hash(item)}");
                Console.WriteLine($"SHA512  => {UserPassword.GetSha512Hash(item)}");
                Console.WriteLine($"AES     => {UserCrypto.GetAesEncrypt(item, item)}");
                Console.WriteLine($"AES Hex => {UserCrypto.GetAesEncryptHex(item, item)}");
                Console.WriteLine($"===================================================================================================");
            }
        }
        [TestMethod]
        public void TestCryptoProvider()
        {
            var passSalt = Encoding.UTF8.GetBytes("Abc123!@#");

            var md5Hash1 = MD5.Create().ComputeHash(passSalt);
            var md5Hash2 = new MD5CryptoServiceProvider().ComputeHash(passSalt);
            Assert.AreEqual(UserCrypto.GetHexString(md5Hash1), UserCrypto.GetHexString(md5Hash2));

            var sha512Hash1 = SHA512.Create().ComputeHash(passSalt);
            var sha512Hash2 = new SHA512CryptoServiceProvider().ComputeHash(passSalt);
            Assert.AreEqual(UserCrypto.GetHexString(sha512Hash1), UserCrypto.GetHexString(sha512Hash2));
        }
        [TestMethod]
        public void TestRsa()
        {
            var enBytes = "你好啊RSA".GetBytes();
            byte[] enResBytes;
            var pubKey = "<RSAKeyValue><Modulus>uouaLXDbnS72BdX2kdYLTvWVe92+8lcXjxrbnMG0KhU2IMHl0PEspDmtnZBymazK8n6s0H9A/MedlFaQmyCG0GGqA8FBAkx6uLDbHqInyz4jivdPD2qJO7gpOT5BNxmO+q2Ue8A4Y3ldP3mcoDNLa0/MKVgjSq43nTXpxYT8K1E=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            var priKey = "<RSAKeyValue><Modulus>uouaLXDbnS72BdX2kdYLTvWVe92+8lcXjxrbnMG0KhU2IMHl0PEspDmtnZBymazK8n6s0H9A/MedlFaQmyCG0GGqA8FBAkx6uLDbHqInyz4jivdPD2qJO7gpOT5BNxmO+q2Ue8A4Y3ldP3mcoDNLa0/MKVgjSq43nTXpxYT8K1E=</Modulus><Exponent>AQAB</Exponent><P>3fkIgciHU0XLoYm3oMRtYOuBpec6QWg4x8fgBCAieEx3xwGNH1PyMPGP7BczSXt0NpbyHjLVXUtwYvd9NHky6w==</P><Q>1yRGXZeKu/VYMUe99S4ego/5aqOFqaXVFy1BQR84MEpvU5ecBHM+sGwB1tizQCdCrkZJszC34Hrz3YDyA5pzsw==</Q><DP>TAY1Ia46mwy2l5cBa3CbPayrdNTjIO+/Mr2EPiV7aNRX2bLwUKCBvL2fW27+w9YikWfVeP5UEWX7EgpNuaEhDQ==</DP><DQ>oezNgTfD1X/tZvnmQRj7Ia2XPenhejQ0VANdr9P9iGsdqz7a0Iak0kgYgkoLb+ecympgohuy6aPg1ABvJsMi/Q==</DQ><InverseQ>NwrRbMy9TdZTgdhmxJ9IkYQlizhOqjgli2FLzj5OcNz6kXDrn9BIMb0Qlehn+zPPmMvBSjQPHP12qNJe448SCQ==</InverseQ><D>HmfGt4Vfpl0TKJxZVifnK/WHaesYxnM/mcms5f4EmZ9fdTNlfArzVck47SewJjAt3BydmlZDh3AZ1SXO6BoDHwis3oa6nS2+gUs3k+AzhaIHJqBcewCyWSTAAvBJR176cGZSpIJvSOnReMT/njtzDgUn8Hs4IW6EZd5LnN9oPsk=</D></RSAKeyValue>";
            Console.WriteLine("------------------公钥加密私钥解密(可行)--------------------------------");
            Console.WriteLine((enResBytes = UserCrypto.GetRsaEncryptBytes(enBytes, pubKey)).GetHexString());
            Console.WriteLine(UserCrypto.GetRsaDecryptBytes(enResBytes, priKey).GetString());
            Console.WriteLine("------------------公钥加密公钥解密(不行)--------------------------------");
            Console.WriteLine((enResBytes = UserCrypto.GetRsaEncryptBytes(enBytes, pubKey)).GetHexString());
            try
            {
                Console.WriteLine(UserCrypto.GetRsaDecryptBytes(enResBytes, pubKey).GetString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"公钥加密公钥无法解密[{ex.Message}]");
            }
            Console.WriteLine("------------------私钥加密公钥解密(不行)--------------------------------");
            Console.WriteLine((enResBytes = UserCrypto.GetRsaEncryptBytes(enBytes, priKey)).GetHexString());
            try
            {
                Console.WriteLine(UserCrypto.GetRsaDecryptBytes(enResBytes, pubKey).GetString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"私钥加密公钥无法解密[{ex.Message}]");
            }
            Console.WriteLine("------------------私钥加密私钥解密(可行)--------------------------------");
            Console.WriteLine((enResBytes = UserCrypto.GetRsaEncryptBytes(enBytes, priKey)).GetHexString());
            Console.WriteLine(UserCrypto.GetRsaDecryptBytes(enResBytes, priKey).GetString());
            Console.WriteLine("------------------未知加密公私解密--------------------------------");
            Console.WriteLine(UserCrypto.GetRsaDecryptBytes("0DF6FA6FBBBBFF9DCA2679E2986E100A6BCEAA50559B23791E6C7E09AF3358244A7D570494B3C5E658FA7AECA31F4B5A748F5E6FDB2ABC382FB3DBD98BC33AE9DBD5185BA1F9041690F48C95023177285510130832FC900C3B3F397583CED40D282B805F1665D9C49887F3E3F608202AB76F6D596250D769360931BCDD068EC5".GetHexBytes(), priKey).GetString());
            try
            {
                Console.WriteLine(UserCrypto.GetRsaDecryptBytes("0DF6FA6FBBBBFF9DCA2679E2986E100A6BCEAA50559B23791E6C7E09AF3358244A7D570494B3C5E658FA7AECA31F4B5A748F5E6FDB2ABC382FB3DBD98BC33AE9DBD5185BA1F9041690F48C95023177285510130832FC900C3B3F397583CED40D282B805F1665D9C49887F3E3F608202AB76F6D596250D769360931BCDD068EC5".GetHexBytes(), pubKey).GetString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"公钥加密公钥无法解密[{ex.Message}]");
            }
            Console.WriteLine(UserCrypto.GetRsaDecryptBytes("5B8AD591558D1B7DDFAF49839E71A4E6696E4F242C2D1FB6D984FC8CA78F9FD85C648E977560735D028614E72BCDCE368967E824C9B081CAD89B92798A1C606005C7563B2DE853FD796942D7104CA6CED6CD79B0B567C20D7016FDEAB16838A733AB1E96973FFE7B3933802A27B52A311605FD083BE2A2DB7919EB1C2EEBB788".GetHexBytes(), priKey).GetString());
            try
            {
                Console.WriteLine(UserCrypto.GetRsaDecryptBytes("5B8AD591558D1B7DDFAF49839E71A4E6696E4F242C2D1FB6D984FC8CA78F9FD85C648E977560735D028614E72BCDCE368967E824C9B081CAD89B92798A1C606005C7563B2DE853FD796942D7104CA6CED6CD79B0B567C20D7016FDEAB16838A733AB1E96973FFE7B3933802A27B52A311605FD083BE2A2DB7919EB1C2EEBB788".GetHexBytes(), pubKey).GetString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"私钥加密公钥无法解密[{ex.Message}]");
            }
            Console.WriteLine("------------------模拟应用场景--------------------------------");
            Console.WriteLine("1.每个客户端授权时得到一个服务器发布的公钥和客户端自己的私钥,并将公钥回传给服务器");
            var clientKey = UserCrypto.GetRsaKeys();
            Console.WriteLine($"私钥(客户端):{clientKey.Key}");
            Console.WriteLine($"公钥(服务器):{clientKey.Value}");
            Console.WriteLine("2.服务器分发内容时给对应客户端发布的公钥进行数据加密传输,只有客户端自己的私钥可以解密");
            var clientName = Guid.NewGuid().GetString();
            Console.WriteLine($"发送给客户端:{clientName}你好");
            Console.WriteLine((enResBytes = UserCrypto.GetRsaEncryptBytes($"{clientName}你好".GetBytes(), clientKey.Value)).GetHexString());
            Console.WriteLine($"客户端接收到:{UserCrypto.GetRsaDecryptBytes(enResBytes, clientKey.Key).GetString()}");
            Console.WriteLine("3.客户端发送给服务器信息时,使用服务器发布的公钥进行加密传输,只有服务器才可以解密");
            Console.WriteLine("发送给服务器:服务器你好");
            Console.WriteLine((enResBytes = UserCrypto.GetRsaEncryptBytes("服务器你好".GetBytes(), pubKey)).GetHexString());
            Console.WriteLine($"服务器接收到:{UserCrypto.GetRsaDecryptBytes(enResBytes, priKey).GetString()}");
            Console.WriteLine("4.服务器响应使用客户端对应的公钥进行加密,客户端请求使用服务器发布的公钥进行加密");
            Console.WriteLine("...............................................................................");
            Console.WriteLine("----------------------以下是个超长的字符串-------------------------------");
            var sb = new StringBuilder();
            for (int i = 0; i < 10240; i++)
            {
                sb.Append(i);
            }
            var data = UserCrypto.GetRsaEncryptBytes(sb.ToString().GetBytes(), clientKey.Value);
            var str = UserCrypto.GetRsaDecryptBytes(data, clientKey.Key);
            Assert.AreEqual(str.GetString(), sb.ToString());
            Console.WriteLine("----------------------以下是个超长字符验签-------------------------------");
            var sign = UserCrypto.GetRsaSignBytes(data, clientKey.Key);
            Assert.IsTrue(UserCrypto.GetRsaSignCheck(data, sign, clientKey.Value));
            Assert.IsFalse(UserCrypto.GetRsaSignCheck(data, sign, pubKey));
        }
        [TestMethod]
        public void CreatePasswordCode()
        {
            var jsonString = System.IO.File.ReadAllText(System.IO.Path.GetFullPath("CryptoClient.Json"));
            var values = jsonString.GetJsonObject();
            foreach (var item in values)
            {
                String key = (String)item.Key + UserPassword.DefaultPasswordB;
                String password = item.Password;
                String value = item.Value;
                Console.WriteLine($"原密码  => 客户端:{(string)item.Key} {password}");
                Console.WriteLine($"密文本  => {UserCrypto.GetAesEncrypt(password, key)}");
                if (!string.IsNullOrEmpty(value))
                {
                    Console.WriteLine($"解密文  => {UserCrypto.GetAesDecrypt(value, key)}");
                }
                Console.WriteLine($"===================================================================================================");
            }
        }
        /// <summary>
        /// 得到加密配置
        /// </summary>
        [TestMethod]
        public void GetEncryptConfig()
        {
            var jsonString = System.IO.File.ReadAllText(System.IO.Path.GetFullPath("QMSValues.Json"));
            var values = jsonString.GetJsonObject();
            foreach (var item in values)
            {
                string appCode = (string)item.AppCode;
                Console.WriteLine($"AppCode: {appCode}");
                Console.WriteLine("SecurityKey : {0}", UserCrypto.GetAesEncrypt(appCode, appCode));
                string issuer = (string)item.Issuer;
                string connString = (string)item.ConnString;
                var storeApiKey = UserCrypto.GetAesEncrypt(new StoreModel(StoreType.MySQL, connString).GetJsonString(), issuer + appCode);
                Console.WriteLine("Story : StoreApi : {0}", storeApiKey);
            }
        }
    }
}
