using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// 用户密码
    /// </summary>
    public class UserPassword
    {
        /// <summary>
        /// 默认密码
        /// </summary>
        public static string DefaultPassword { get; set; } = "Ezx2020!@#";

        /// <summary>
        /// 默认密码字符初始化
        /// </summary>
        public UserPassword() : this(DefaultPassword, Guid.Empty) { }
        /// <summary>
        /// 设定密码字符初始化,保存密码为设定密码
        /// </summary>
        /// <param name="origin">密码字符串</param>
        public UserPassword(string origin) : this(origin, Guid.NewGuid()) { }

        /// <summary>
        /// 完整密码初始化,保存密码为设定密码,盐值为设定值
        /// </summary>
        /// <param name="origin">新密码</param>
        /// <param name="salt">盐值</param>
        public UserPassword(string origin, string salt)
        {
            Salt = salt;
            OPass = origin;
            HPass = GetSha1Hash(OPass, Salt);

        }
        /// <summary>
        /// 完整密码初始化,保存密码为设定密码,盐值为设定值
        /// </summary>
        /// <param name="origin">新密码</param>
        /// <param name="salt">盐值</param>
        public UserPassword(string origin, Guid salt) : this(origin, salt.ToString("N")) { }
        /// <summary>
        /// 盐值字符
        /// </summary>
        public string Salt { get; private set; }
        /// <summary>
        /// 原码字符
        /// </summary>
        public string OPass { get; private set; }
        /// <summary>
        /// 加密字符
        /// </summary>
        public string HPass { get; private set; }

        /// <summary>
        /// 获得散列密码(SHA-512)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="salt">盐值字符</param>
        /// <returns>加密字符</returns>
        public static string GetSha512Hash(string pass, string salt = null)
        {
            var sha2 = new SHA512Managed();
            var passSalt = Encoding.UTF8.GetBytes(pass + salt);
            var shaHash = sha2.ComputeHash(passSalt);
            return GetByte16String(shaHash);
        }

        /// <summary>
        /// 获得散列密码(SHA-256)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="salt">盐值字符</param>
        /// <returns>加密字符</returns>
        public static string GetSha256Hash(string pass, string salt = null)
        {
            var sha2 = new SHA256Managed();
            var passSalt = Encoding.UTF8.GetBytes(pass + salt);
            var shaHash = sha2.ComputeHash(passSalt);
            return GetByte16String(shaHash);
        }

        /// <summary>
        /// 获得散列密码(SHA-1)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="salt">盐值字符</param>
        /// <returns>加密字符</returns>
        public static string GetSha1Hash(string pass, string salt = null)
        {
            var sha1 = new SHA1Managed();
            var passSalt = Encoding.UTF8.GetBytes(pass + salt);
            var hashData = sha1.ComputeHash(passSalt);
            return GetByte16String(hashData);
        }

        /// <summary>
        /// 获取MD5加密(32位)
        /// 默认UTF-8转换
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <returns>加密字符</returns>
        public static string GetMd5Hash(string pass)
        {
            return GetMd5Hash(pass, Encoding.UTF8);
        }

        /// <summary>
        /// 获取MD5加密(32位)
        /// 默认UTF-8转换
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="salt">盐值字符</param>
        /// <returns>加密字符</returns>
        public static string GetMd5Hash(string pass, string salt)
        {
            return GetMd5Hash(pass, salt, Encoding.UTF8);
        }

        /// <summary>
        /// 获取MD5加密(32位)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="encode">编码</param>
        /// <returns>加密字符</returns>
        public static string GetMd5Hash(string pass, Encoding encode)
        {
            var passSalt = encode.GetBytes(pass);
            var md5 = MD5.Create();
            var hashData = md5.ComputeHash(passSalt);
            return GetByte16String(hashData);
        }

        /// <summary>
        /// 获取MD5加密(32位)
        /// </summary>
        /// <param name="pass">原码字符</param>
        /// <param name="salt">盐值字符</param>
        /// <param name="encode">编码</param>
        /// <returns>加密字符</returns>
        public static string GetMd5Hash(string pass, string salt, Encoding encode)
        {
            return GetMd5Hash(string.Format("{0}{1}", pass, salt), encode);
        }

        /// <summary>
        /// 将字节数组转换成16进制字符串
        /// </summary>
        /// <param name="hashData">字节数组</param>
        /// <returns>16进制字符串(大写字母)</returns>
        public static string GetByte16String(byte[] hashData)
        {
            StringBuilder sBuilder = new StringBuilder();
            foreach (var hash in hashData)
            {
                sBuilder.AppendFormat("{0:X2}", hash);
            }
            return sBuilder.ToString();
        }
    }
}
