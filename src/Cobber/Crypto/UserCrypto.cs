using System;
using System.Collections.Generic;
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
        #region // MD5
        #endregion
        #region // SHA
        #endregion
        #region // RAS
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
        /// <param name="content">明文字节（待加密）</param>
        /// <param name="key">密钥(32位)</param>
        /// <returns></returns>
        public static byte[] GetAesEncrypt(byte[] content, byte[] key)
        {
            key = GetFillBytes(key, 32);
            var rm = Aes.Create();
            rm.Key = key;
            rm.Mode = CipherMode.ECB;
            rm.Padding = PaddingMode.PKCS7;
            return rm.CreateEncryptor().TransformFinalBlock(content, 0, content.Length);
        }

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
        /// <param name="content">明文（待解密）</param>
        /// <param name="key">密钥(32位)</param>
        /// <returns></returns>
        public static byte[] GetAesDecrypt(byte[] content, byte[] key)
        {
            key = GetFillBytes(key, 32);
            var rm = Aes.Create();
            rm.Key = key;
            rm.Mode = CipherMode.ECB;
            rm.Padding = PaddingMode.PKCS7;
            return rm.CreateDecryptor().TransformFinalBlock(content, 0, content.Length);
        }
        #endregion
        #region // DES
        #endregion

    }
}
