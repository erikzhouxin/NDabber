using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// 字符串调用
    /// </summary>
    public static partial class CobberCaller
    {
        /// <summary>
        /// 转换成字符串唯一值
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="fmt"></param>
        /// <returns></returns>
        public static string GetString(this Guid guid, string fmt = "N") => guid.ToString(fmt);
        /// <summary>
        /// 转换成标题的大小写
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string GetTitleCase(this string val)
        {
            return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(val);
        }
        /// <summary>
        /// 获取空的默认值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public static String GetEmptyDefault(this string value, string defVal)
        {
            return string.IsNullOrWhiteSpace(value) ? defVal : value;
        }
        /// <summary>
        /// 连接字符串
        /// </summary>
        /// <param name="list"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public static String JoinString<T>(this IEnumerable<T> list, string split = ",")
        {
            return string.Join(split, list);
        }
        /// <summary>
        /// 连接字符串
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static String JoinLine<T>(this IEnumerable<T> list)
        {
            if (list == null || list.Count() == 0) { return string.Empty; }
            var sb = new StringBuilder(string.Format("{0}", list.First()));
            foreach (var item in list.Skip(1))
            {
                sb.AppendLine().AppendFormat("{0}", item);
            }
            return sb.ToString();
        }
    }
}
namespace System.Data.Extter
{
    /// <summary>
    /// 字符串调用扩展
    /// </summary>
    public static class StringCaller
    {
        #region // Base64
        /// <summary>
        /// 获取Base64编码
        /// </summary>
        /// <returns></returns>
        public static String GetBase64(this string json, Encoding encoding)
        {
            return Convert.ToBase64String(encoding.GetBytes(json));
        }
        /// <summary>
        /// 获取Base64编码
        /// </summary>
        /// <see cref="SerialCaller.GetBinBytes{T}(T)"/>
        /// <returns></returns>
        public static String GetBase64<T>(this T model)
        {
            return Convert.ToBase64String(model.GetBinBytes());
        }
        /// <summary>
        /// 获取Base64解码
        /// </summary>
        /// <see cref="SerialCaller.GetBinModel{T}(byte[])"/>
        /// <returns></returns>
        public static T GetDebase64<T>(this string base64)
        {
            return Convert.FromBase64String(base64).GetBinModel<T>();
        }
        /// <summary>
        /// 获取Base64编码
        /// </summary>
        /// <returns></returns>
        public static String GetBase64(this string json) => GetBase64(json, Encoding.UTF8);
        /// <summary>
        /// 获取Base64解码
        /// </summary>
        /// <returns></returns>
        public static String GetDebase64(this string base64, Encoding encoding)
        {
            var bytes = Convert.FromBase64String(base64);
            return encoding.GetString(bytes);
        }
        /// <summary>
        /// 获取Base64解码
        /// </summary>
        /// <returns></returns>
        public static String GetDebase64(this string base64) => GetDebase64(base64, Encoding.UTF8);
        #endregion
        #region // 压缩
        /// <summary>
        /// 压缩字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Compress(this string str) => Compress(str, Encoding.UTF8);
        /// <summary>
        /// 根据编码压缩字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Compress(this string str, Encoding encoding)
        {
            var bytes = encoding.GetBytes(str);

            var ms = new MemoryStream();
            var zip = new GZipStream(ms, CompressionMode.Compress, true);
            zip.Write(bytes, 0, bytes.Length);
            zip.Close();
            var buffer = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(buffer, 0, buffer.Length);
            ms.Close();

            return Convert.ToBase64String(buffer);
        }
        /// <summary>
        /// 解压字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Decompress(this string str) => Decompress(str, Encoding.UTF8);
        /// <summary>
        /// 根据编码解压字符串
        /// </summary>
        /// <returns></returns>
        public static string Decompress(this string str, Encoding encoding)
        {
            var bytes = Convert.FromBase64String(str);

            var ms = new MemoryStream(bytes);
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

            return encoding.GetString(buffer);
        }
        #endregion
    }
}
