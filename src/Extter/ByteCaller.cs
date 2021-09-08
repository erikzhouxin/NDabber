using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
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
    }
}
namespace System.Data.Extter
{
    /// <summary>
    /// 字节调用
    /// </summary>
    public static class ByteCaller
    {
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
