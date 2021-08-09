using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
