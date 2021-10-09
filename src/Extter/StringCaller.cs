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
        /// <summary>
        /// 转换成大写
        /// </summary>
        /// <param name="cha"></param>
        /// <returns></returns>
        public static Char ToUpper(this char cha)
        {
            return Char.ToUpper(cha);
        }
        /// <summary>
        /// 转换成小写
        /// </summary>
        /// <param name="cha"></param>
        /// <returns></returns>
        public static Char ToLower(this char cha)
        {
            return Char.ToLower(cha);
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
        /// <summary>
        /// 转换成蛇形
        /// ATest=>a_test
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string ToSnakeCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) { return str; }
            List<char> list = new List<char>(str.Length * 2);
            list.Add(char.ToLower(str[0]));
            char curr;
            for (int i = 1; i < str.Length; i++)
            {
                curr = str[i];
                if (Char.IsUpper(curr))
                {
                    list.Add('_');
                    list.Add(Char.ToLower(curr));
                }
                else
                {
                    list.Add(curr);
                }
            }
            return new String(list.ToArray());
        }
        /// <summary>
        /// 驼峰转换成蛇形
        /// aTest=>a_test
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string CamelToSnakeCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) { return str; }
            List<char> list = new List<char>(str.Length * 2);
            char curr;
            for (int i = 0; i < str.Length; i++)
            {
                curr = str[i];
                if (Char.IsUpper(curr))
                {
                    list.Add('_');
                    list.Add(Char.ToLower(curr));
                }
                else
                {
                    list.Add(curr);
                }
            }
            return new String(list.ToArray());
        }
        /// <summary>
        /// 帕斯卡(大驼峰)转换成蛇形
        /// ATest=>a_test
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string PascalToSnakeCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) { return str; }
            List<char> list = new List<char>(str.Length * 2);
            list.Add(char.ToLower(str[0]));
            char curr;
            for (int i = 1; i < str.Length; i++)
            {
                curr = str[i];
                if (char.IsUpper(curr))
                {
                    list.Add('_');
                    list.Add(char.ToLower(curr));
                }
                else
                {
                    list.Add(curr);
                }
            }
            return new String(list.ToArray());
        }
        /// <summary>
        /// 帕斯卡(大驼峰)转换成驼峰
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string PascalToCamelCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) { return str; }
            char curr = Char.ToLower(str[0]);
            StringBuilder sb = new StringBuilder().Append(curr);
            for (int i = 0; i < str.Length; i++)
            {
                sb.Append(str[i]);
            }
            return sb.ToString();
        }
        /// <summary>
        /// 驼峰转换成帕斯卡(大驼峰)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string CamelToPascalCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) { return str; }
            char curr = Char.ToUpper(str[0]);
            StringBuilder sb = new StringBuilder().Append(curr);
            for (int i = 0; i < str.Length; i++)
            {
                sb.Append(str[i]);
            }
            return sb.ToString();
        }
        /// <summary>
        /// 蛇形转换成驼峰
        /// a_test=>aTest
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string SnakeToCamelCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) { return str; }
            List<char> list = new List<char>(str.Length);
            char curr;
            for (int i = 0; i < str.Length; i++)
            {
                curr = str[i];
                if (curr == '_')
                {
                    i++;
                    if (i >= str.Length) { break; }
                    curr = Char.ToUpper(str[i]);
                }
                list.Add(curr);
            }
            return new String(list.ToArray());
        }
        /// <summary>
        /// 蛇形转换成帕斯卡(大驼峰)
        /// a_test=>ATest
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string SnakeToPascalCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) { return str; }
            List<char> list = new List<char>(str.Length);
            char curr = str[0];
            int start;
            if (curr == '_')
            {
                if (str.Length == 1) { return str; }
                start = 2;
                list.Add(Char.ToUpper(curr));
            }
            else
            {
                start = 1;
                list.Add(char.ToUpper(curr));
            }
            for (int i = start; i < str.Length; i++)
            {
                curr = str[i];
                if (curr == '_')
                {
                    i++;
                    if (i >= str.Length) { break; }
                    curr = Char.ToUpper(str[i]);
                }
                list.Add(curr);
            }
            return new String(list.ToArray());
        }
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
        /// <see cref="SerialCaller.GetModelBytes{T}(T)"/>
        /// <returns></returns>
        public static String GetBase64<T>(this T model)
        {
            return Convert.ToBase64String(model.GetModelBytes());
        }
        /// <summary>
        /// 获取Base64解码
        /// </summary>
        /// <see cref="SerialCaller.GetBytesModel{T}(byte[])"/>
        /// <returns></returns>
        public static T GetDebase64<T>(this string base64)
        {
            return Convert.FromBase64String(base64).GetBytesModel<T>();
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
