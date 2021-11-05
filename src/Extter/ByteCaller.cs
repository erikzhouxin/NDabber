using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// 字节编码
    /// </summary>
    public static partial class CobberCaller
    {
        /// <summary>
        /// 转换成字节数组
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this string content) => GetBytes(content, Encoding.UTF8);
        /// <summary>
        /// 转换成字节数组
        /// </summary>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this String content, Encoding encoding)
        {
            return encoding.GetBytes(content);
        }
        /// <summary>
        /// 比较两个字节数组是否相同内容
        /// </summary>
        /// <param name="curr"></param>
        /// <param name="tag"></param>
        /// <param name="isNull">同时为NULL时是否相等,默认相等</param>
        /// <returns></returns>
        public static bool EqualBytes(this byte[] curr, byte[] tag, bool isNull = true)
        {
            if (curr == null && tag == null) { return isNull; }
            if (curr == null || tag == null) { return false; }
            if (curr.Length != tag.Length) { return false; }
            for (int i = 0; i < curr.Length; i++)
            {
                if (curr[i] != tag[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
namespace System.Data.Extter
{
    /// <summary>
    /// 字节调用
    /// </summary>
    public static class ByteCaller
    {
        /// <summary>
        /// 获取MD5加密值
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] GetMd5(this byte[] bytes)
        {
            return new MD5CryptoServiceProvider().ComputeHash(bytes);
        }
        /// <summary>
        /// 获取MD5加密值
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string GetMd5String(this byte[] bytes)
        {
            return GetHexString(GetMd5(bytes));
        }
        /// <summary>
        /// 将字节数组转换成16进制字符串
        /// </summary>
        /// <param name="hashData">字节数组</param>
        /// <returns>16进制字符串(大写字母)</returns>
        public static string GetHexString(this byte[] hashData)
        {
            StringBuilder sBuilder = new StringBuilder();
            foreach (var hash in hashData)
            {
                sBuilder.AppendFormat("{0:X2}", hash);
            }
            return sBuilder.ToString();
        }
        /// <summary>
        /// 将16进制字符串转换成字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] GetHexByte(this string hexString)
        {
            if ((hexString.Length % 2) != 0) { hexString += " "; }
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return returnBytes;
        }
        /// <summary>
        /// 将16进制字符串转换成字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <param name="fix"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public static byte[] GetHexByte(this string hexString, char fix, char replace = ' ')
        {
            hexString.Replace(replace.ToString(), "");
            if ((hexString.Length % 2) != 0) { hexString += fix; }
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return returnBytes;
        }
        /// <summary>
        /// 将16进制字符串转换成字节数组
        /// </summary>
        /// <returns></returns>
        public static byte[] GetHexByte(this string hexString, char fix = ' ', string replace = " ")
        {
            hexString.Replace(replace, "");
            if ((hexString.Length % 2) != 0) { hexString += fix; }
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return returnBytes;
        }
        /// <summary>
        /// 将16进制字符串转换成字节数组
        /// </summary>
        /// <returns></returns>
        public static byte[] GetHexByte(this string hexString, char fix, params char[] replaces)
        {
            foreach (var item in replaces)
            {
                hexString.Replace(item.ToString(), "");
            }
            if ((hexString.Length % 2) != 0) { hexString += fix; }
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return returnBytes;
        }
        #region // 压缩
        /// <summary>
        /// 压缩字节
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Compress(this byte[] data)
        {
            var ms = new MemoryStream();
            var zip = new GZipStream(ms, CompressionMode.Compress, true);
            zip.Write(data, 0, data.Length);
            zip.Close();
            var buffer = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(buffer, 0, buffer.Length);
            ms.Close();
            return buffer;
        }

        /// <summary>
        /// 解压字节
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Decompress(this byte[] data)
        {
            var ms = new MemoryStream(data);
            var zip = new GZipStream(ms, CompressionMode.Decompress, true);
            var msreader = new MemoryStream();
            var buffer = new byte[0x1000];
            while (true)
            {
                var reader = zip.Read(buffer, 0, buffer.Length);
                if (reader <= 0)
                {
                    break;
                }
                msreader.Write(buffer, 0, reader);
            }
            zip.Close();
            ms.Close();
            msreader.Position = 0;
            buffer = msreader.ToArray();
            msreader.Close();
            return buffer;
        }
        #endregion
    }
}
