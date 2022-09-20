using System;
using System.Collections.Generic;
using System.Data.Extter;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// 用户密钥
    /// </summary>
    public static class UserCrypto
    {
        #region // 16进制
        /// <summary>
        /// 将字节数组转换成16进制字符串
        /// </summary>
        /// <param name="hashData">字节数组</param>
        /// <param name="isLower">是小写</param>
        /// <returns>16进制字符串(大写字母)</returns>
        public static string GetHexString(byte[] hashData, bool isLower = false)
        {
            var sb = new StringBuilder();
            var fmt = isLower ? "x2" : "X2";
            for (int i = 0; i < hashData.Length; i++)
            {
                sb.Append(hashData[i].ToString(fmt));
            }
            return sb.ToString();
        }
        #endregion
        #region // MD5
        /// <summary>
        /// 获取MD5加密(32位)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="isLower">是小写</param>
        /// <param name="encoding">编码</param>
        /// <returns>加密字符</returns>
        public static string GetMd5HexString(this string pass, bool isLower, Encoding encoding) => GetHexString(GetMd5Bytes(encoding.GetBytes(pass)), isLower);
        /// <summary>
        /// 获取MD5加密(32位)
        /// </summary>
        /// <param name="isLower"></param>
        /// <param name="passSalt"></param>
        /// <returns></returns>
        public static string GetMd5HexString(this byte[] passSalt, bool isLower = false) => GetHexString(GetMd5Bytes(passSalt), isLower);
        /// <summary>
        /// 获取MD5加密(32位)
        /// </summary>
        /// <param name="passSalt"></param>
        /// <returns></returns>
        public static byte[] GetMd5Bytes(this byte[] passSalt) => MD5.Create().ComputeHash(passSalt);
        #endregion
        #region // SHA
        /// <summary>
        /// 获得散列密码(SHA-1)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="isLower">是小写</param>
        /// <param name="encoding">编码</param>
        /// <returns>加密字符</returns>
        public static string GetSha1HexString(this string pass, bool isLower, Encoding encoding) => GetSha1HexString(encoding.GetBytes(pass), isLower);
        /// <summary>
        /// 获得散列密码(SHA-1)
        /// </summary>
        /// <param name="passSalt"></param>
        /// <param name="isLower">是小写</param>
        /// <returns>加密字符</returns>
        public static string GetSha1HexString(this byte[] passSalt, bool isLower) => GetHexString(GetSha1Bytes(passSalt), isLower);
        /// <summary>
        /// 获得散列密码(SHA-1)
        /// </summary>
        /// <param name="passSalt"></param>
        /// <returns></returns>
        public static byte[] GetSha1Bytes(this byte[] passSalt) => SHA1.Create().ComputeHash(passSalt);
        /// <summary>
        /// 获得散列密码(SHA-256)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="isLower">是小写</param>
        /// <param name="encoding">编码</param>
        /// <returns>加密字符</returns>
        public static string GetSha256HexString(this string pass, bool isLower, Encoding encoding) => GetSha256HexString(encoding.GetBytes(pass), isLower);
        /// <summary>
        /// 获得散列密码(SHA-256)
        /// </summary>
        /// <param name="passSalt"></param>
        /// <param name="isLower">是小写</param>
        /// <returns>加密字符</returns>
        public static string GetSha256HexString(this byte[] passSalt, bool isLower) => GetHexString(GetSha256Bytes(passSalt), isLower);
        /// <summary>
        /// 获得散列密码(SHA-256)
        /// </summary>
        /// <param name="passSalt"></param>
        /// <returns></returns>
        public static byte[] GetSha256Bytes(this byte[] passSalt) => SHA256.Create().ComputeHash(passSalt);
        /// <summary>
        /// 获得散列密码(SHA-384)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="isLower">是小写</param>
        /// <param name="encoding">编码</param>
        /// <returns>加密字符</returns>
        public static string GetSha384HexString(this string pass, bool isLower, Encoding encoding) => GetSha384HexString(encoding.GetBytes(pass), isLower);
        /// <summary>
        /// 获得散列密码(SHA-384)
        /// </summary>
        /// <param name="passSalt"></param>
        /// <param name="isLower">是小写</param>
        /// <returns>加密字符</returns>
        public static string GetSha384HexString(this byte[] passSalt, bool isLower) => GetHexString(GetSha384Bytes(passSalt), isLower);
        /// <summary>
        /// 获得散列密码(SHA-384)
        /// </summary>
        /// <param name="passSalt"></param>
        /// <returns></returns>
        public static byte[] GetSha384Bytes(this byte[] passSalt) => SHA384.Create().ComputeHash(passSalt);
        /// <summary>
        /// 获得散列密码(SHA-512)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="isLower">是小写</param>
        /// <param name="encoding">编码</param>
        /// <returns>加密字符</returns>
        public static string GetSha512HexString(this string pass, bool isLower, Encoding encoding) => GetSha512HexString(encoding.GetBytes(pass), isLower);
        /// <summary>
        /// 获得散列密码(SHA-512)
        /// </summary>
        /// <param name="passSalt"></param>
        /// <param name="isLower">是小写</param>
        /// <returns>加密字符</returns>
        public static string GetSha512HexString(this byte[] passSalt, bool isLower) => GetHexString(GetSha512Bytes(passSalt), isLower);
        /// <summary>
        /// 获得散列密码(SHA-512)
        /// </summary>
        /// <param name="passSalt"></param>
        /// <returns></returns>
        public static byte[] GetSha512Bytes(this byte[] passSalt) => SHA512.Create().ComputeHash(passSalt);
        #endregion
        #region // RSA
        /// <summary>
        /// 获取Rsa密钥(Key:私钥,Value:公钥)
        /// </summary>
        /// <returns></returns>
        public static KeyValuePair<string, String> GetRsaKeys(int keySize = 1024)
        {
            var rsa = RSA.Create();
            rsa.KeySize = keySize > 768 ? (int)(Math.Ceiling(keySize / 16.0) * 16) : 1024;
            return new KeyValuePair<string, string>(rsa.ToXmlString(true), rsa.ToXmlString(false));
        }
        /// <summary>
        /// 获取Rsa私钥,后续可根据私钥获取公钥
        /// </summary>
        /// <returns></returns>
        public static string GetRsaKey(int keySize = 1024)
        {
            var rsa = RSA.Create();
            rsa.KeySize = keySize > 768 ? (int)(Math.Ceiling(keySize / 16.0) * 16) : 1024;
            return rsa.ToXmlString(true);
        }
        /// <summary>
        /// 通过私钥获取Rsa的公钥
        /// </summary>
        /// <param name="priKey"></param>
        /// <returns></returns>
        public static String GetRsaPublicKey(string priKey)
        {
#if NETFx
            using (var rsa = RSA.Create())
#else
            using (var rsa = new RSACryptoServiceProvider())
#endif
            {
                rsa.FromXmlString(priKey);
                return rsa.ToXmlString(false);
            }
        }
        /// <summary>
        /// 获得公钥加密算法密码(RSA)
        /// </summary>
        /// <param name="passSalt"></param>
        /// <param name="pubKey"></param>
        /// <returns></returns>
        public static byte[] GetRsaEncryptBytes(this byte[] passSalt, string pubKey)
        {
#if NETFx
            using (var rsa = RSA.Create())
#else
            using (var rsa = new RSACryptoServiceProvider())
#endif
            {
                rsa.FromXmlString(pubKey);
                int bufSize = rsa.KeySize / 16;
                var buffer = new byte[bufSize];
                using (MemoryStream input = new MemoryStream(passSalt), output = new MemoryStream())
                {
                    while (true)
                    {
                        int readSize = input.Read(buffer, 0, bufSize);
                        if (readSize == 0)
                        {
                            break;
                        }
                        var temp = new byte[readSize];
                        Array.Copy(buffer, 0, temp, 0, readSize);
#if NETFx
                        var enBytes = rsa.Encrypt(temp, RSAEncryptionPadding.OaepSHA1);
#else
                        var enBytes = rsa.Encrypt(temp, true);
#endif
                        output.Write(enBytes, 0, enBytes.Length);
                    }

                    return output.ToArray();
                }
            }
        }
        /// <summary>
        /// 获得私钥解密算法密码(RSA)
        /// </summary>
        /// <param name="passData"></param>
        /// <param name="priKey"></param>
        /// <returns></returns>
        public static byte[] GetRsaDecryptBytes(this byte[] passData, string priKey)
        {
#if NETFx
            using (var rsa = RSA.Create())
#else
            using( var rsa = new RSACryptoServiceProvider())
#endif
            {
                rsa.FromXmlString(priKey);
                int bufSize = rsa.KeySize / 8;
                var buffer = new byte[bufSize];
                using (MemoryStream input = new MemoryStream(passData), output = new MemoryStream())
                {
                    while (true)
                    {
                        int readSize = input.Read(buffer, 0, bufSize);
                        if (readSize == 0)
                        {
                            break;
                        }
                        var temp = new byte[readSize];
                        Array.Copy(buffer, 0, temp, 0, readSize);
#if NETFx
                        var enBytes = rsa.Decrypt(temp, RSAEncryptionPadding.OaepSHA1);
#else
                var enBytes = rsa.Decrypt(temp, true);
#endif
                        output.Write(enBytes, 0, enBytes.Length);
                    }
                    return output.ToArray();
                }
            }
        }
        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="passData">需签名的数据</param>
        /// <param name="priKey"></param>
        /// <returns>签名后的值</returns>
        public static byte[] GetRsaSignBytes(byte[] passData, string priKey)
        {
            //根据需要加签时的哈希算法转化成对应的hash字符节
            byte[] rgbHash = SHA1.Create().ComputeHash(passData);
#if NETFx
            using (var rsa = RSA.Create())
#else
            using (var rsa = new RSACryptoServiceProvider())
#endif
            {
                rsa.FromXmlString(priKey);
                var formatter = new RSAPKCS1SignatureFormatter(rsa);
                formatter.SetHashAlgorithm(nameof(SHA1)); //此处是你需要加签的hash算法，需要和上边你计算的hash值的算法一致，不然会报错。
                return formatter.CreateSignature(rgbHash);
            }
        }
        /// <summary>
        /// 签名验证
        /// </summary>
        /// <param name="passData">待验证的字符串</param>
        /// <param name="sign">加签之后的字符串</param>
        /// <param name="pubKey"></param>
        /// <returns>签名是否符合</returns>
        public static bool GetRsaSignCheck(byte[] passData, byte[] sign, string pubKey)
        {
            try
            {
                byte[] rgbHash = SHA1.Create().ComputeHash(passData);
#if NETFx
                using (var rsa = RSA.Create())
#else
                using (var rsa = new RSACryptoServiceProvider())
#endif
                {
                    rsa.FromXmlString(pubKey);
                    var deformatter = new RSAPKCS1SignatureDeformatter(rsa);
                    deformatter.SetHashAlgorithm(nameof(SHA1));
                    return deformatter.VerifySignature(rgbHash, sign);
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion
        #region // AES 比 DES 更快更安全
        /// <summary>
        ///  AES 加密
        /// </summary>
        /// <param name="str">明文（待加密）</param>
        /// <param name="key">密钥(32位)</param>
        /// <returns></returns>
        public static string GetAesEncrypt(string str, string key)
        {
            if (string.IsNullOrEmpty(str)) { return string.Empty; }
            return Convert.ToBase64String(GetAesEncrypt(Encoding.UTF8.GetBytes(str), Encoding.UTF8.GetBytes(key)));
        }
        /// <summary>
        ///  AES 加密
        /// </summary>
        /// <param name="str">明文（待加密）</param>
        /// <param name="key">密钥(32位)</param>
        /// <returns></returns>
        public static string GetAesEncryptHex(string str, string key)
        {
            if (string.IsNullOrEmpty(str)) { return string.Empty; }
            return GetHexString(GetAesEncrypt(Encoding.UTF8.GetBytes(str), Encoding.UTF8.GetBytes(key)));
        }
        /// <summary>
        ///  AES 加密
        /// </summary>
        /// <param name="content">明文字节（待加密）</param>
        /// <param name="key">密钥(32位)</param>
        /// <returns></returns>
        public static byte[] GetAesEncrypt(byte[] content, byte[] key)
            => GetAesEncrypt(content, GetFillBytes(key, 32), CipherMode.ECB, PaddingMode.PKCS7);
        /// <summary>
        /// 获取Aes加密
        /// </summary>
        /// <param name="content">原文内容</param>
        /// <param name="key">密钥,必须是32位/16位/8位</param>
        /// <param name="mode">加密模式</param>
        /// <param name="padding">填充类型</param>
        /// <returns></returns>
        public static byte[] GetAesEncrypt(byte[] content, byte[] key, CipherMode mode, PaddingMode padding)
        {
            var rm = Aes.Create();
            rm.Key = key;
            rm.Mode = mode;
            rm.Padding = padding;
            return rm.CreateEncryptor().TransformFinalBlock(content, 0, content.Length);
        }
        /// <summary>
        /// 获取填充字节
        /// </summary>
        /// <param name="key"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        private static byte[] GetFillBytes(byte[] key, int len)
        {
            if (key == null || key.Length == 0)
            {
                return new byte[len];
            }
            else if (key.Length < len)
            {
                var newKey = new byte[len];
                for (int i = 1; i < key.Length; i++)
                {
                    newKey[len - i] = key[i];
                }
                return newKey;
            }
            else
            {
                return key.Take(len).ToArray();
            }
        }
        /// <summary>
        ///  AES 解密
        /// </summary>
        /// <param name="str">明文（待解密）</param>
        /// <param name="key">密文(32位)</param>
        /// <returns></returns>
        public static string GetAesDecrypt(string str, string key)
        {
            if (string.IsNullOrEmpty(str)) { return string.Empty; }
            return Encoding.UTF8.GetString(GetAesDecrypt(Convert.FromBase64String(str), Encoding.UTF8.GetBytes(key)));
        }
        /// <summary>
        ///  AES 解密
        /// </summary>
        /// <param name="str">明文（待解密）</param>
        /// <param name="key">密文(32位)</param>
        /// <returns></returns>
        public static string GetAesDecryptHex(string str, string key)
        {
            if (string.IsNullOrEmpty(str)) { return string.Empty; }
            return Encoding.UTF8.GetString(GetAesDecrypt(str.GetHexBytes(), Encoding.UTF8.GetBytes(key)));
        }
        /// <summary>
        ///  AES 解密
        /// </summary>
        /// <param name="content">明文（待解密）</param>
        /// <param name="key">密钥(32位)</param>
        /// <returns></returns>
        public static byte[] GetAesDecrypt(byte[] content, byte[] key)
            => GetAesDecrypt(content, GetFillBytes(key, 32), CipherMode.ECB, PaddingMode.PKCS7);
        /// <summary>
        /// 获取Aes解密
        /// </summary>
        /// <param name="content">原文内容</param>
        /// <param name="key">密钥,必须是32位/16位/8位</param>
        /// <param name="mode">加密模式</param>
        /// <param name="padding">填充类型</param>
        /// <returns></returns>
        public static byte[] GetAesDecrypt(byte[] content, byte[] key, CipherMode mode, PaddingMode padding)
        {
            var rm = Aes.Create();
            rm.Key = key;
            rm.Mode = mode;
            rm.Padding = padding;
            return rm.CreateDecryptor().TransformFinalBlock(content, 0, content.Length);
        }
        /// <summary>
        /// 获取右边填充/截断字节
        /// <see cref="ExtterCaller.PadInterceptRight"/>
        /// </summary>
        /// <param name="content"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] GetRightBytes(this string content, int length = 32)
        {
            if (string.IsNullOrEmpty(content)) { return new byte[length]; }
            return content.GetBytes().PadInterceptRight(length); ;
        }
        /// <summary>
        /// 获取左边填充/截断字节
        /// <see cref="ExtterCaller.PadInterceptLeft"/>
        /// </summary>
        /// <param name="content"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] GetLeftBytes(this string content, int length = 32)
        {
            if (string.IsNullOrEmpty(content)) { return new byte[length]; }
            return content.GetBytes().PadInterceptLeft(length); ;
        }
        #endregion
        #region // DES
        #endregion

    }
}
