using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// 文件帮助类
    /// </summary>
    public static class UserFileable
    {
        #region // 文件MD5 SHA 散列值
        /// <summary>
        /// 获取文件的Md5值
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string GetFileMd5(this FileInfo file) => GetStreamMd5(file.OpenRead());

        /// <summary>
        /// 获取文件的Md5值
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string GetStreamMd5(this Stream file)
        {
            try
            {
                using (MD5 md5 = MD5.Create())
                {
                    byte[] retVal = md5.ComputeHash(file);
                    return GetByte16String(retVal);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(nameof(GetFileMd5) + "失败, 错误:" + ex.Message);
            }
        }

        /// <summary>
        /// 获取文件的Hash值
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string GetFileHash(this FileInfo file)
        {
            try
            {
                using (HashAlgorithm hash = HashAlgorithm.Create("SHA"))
                {
                    byte[] retVal = hash.ComputeHash(file.OpenRead());
                    return GetByte16String(retVal);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(nameof(GetFileHash) + "失败, 错误:" + ex.Message);
            }
        }
        /// <summary>
        /// 获取文件的Hash值
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string GetFileSha1(this FileInfo file) => GetStreamSha1(file.OpenRead());
        /// <summary>
        /// 获取文件的Hash值
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string GetStreamSha1(this Stream file)
        {
            try
            {
                using (SHA1 hash = SHA1.Create())
                {
                    byte[] retVal = hash.ComputeHash(file);
                    return GetByte16String(retVal);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(nameof(GetFileSha1) + "失败, 错误:" + ex.Message);
            }
        }
        /// <summary>
        /// 将字节数组转换成16进制字符串
        /// </summary>
        /// <param name="hashData">字节数组</param>
        /// <returns>16进制字符串(大写字母)</returns>
        public static string GetByte16String(byte[] hashData)
        {
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < hashData.Length; i++)
            {
                sBuilder.AppendFormat("{0:X2}", hashData[i]);
            }
            return sBuilder.ToString();
        }
        #endregion
    }
}
