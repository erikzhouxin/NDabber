using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Cobber
{
    public static partial class CobberCaller
    {
        /// <summary>
        /// 获取对象的Json字符串(小写属性)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        [Obsolete("替代方案:GetJsonLowerString")]
        public static string GetLowerJsonString<T>(this T value) => JsonConvert.SerializeObject(value, LowerNewtonsoftSetting);
        /// <summary>
        /// 获取对象的Json字符串
        /// Newtonsoft.Json.JsonConvert
        /// </summary>
        [Obsolete("替代方案:GetJsonWebString")]
        public static string GetWebJsonString<T>(this T value) => JsonConvert.SerializeObject(value, WebNewtonsoftSetting);
        /// <summary>
        /// 转换成秒时间字符串
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        [Obsolete("替代方案:GetDateTimeString")]
        public static String ToSecondString(this DateTime time) => time.ToString("yyyy-MM-dd HH:mm:ss");
        /// <summary>
        /// 转换成日期字符串
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        [Obsolete("替代方案:GetDateString")]
        public static String ToDateString(this DateTime time) => time.ToString("yyyy-MM-dd");
    }
}
