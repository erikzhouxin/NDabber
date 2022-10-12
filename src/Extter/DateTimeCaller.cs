using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Cobber
{
    public static partial class CobberCaller
    {
        /// <summary>
        /// 获取当前天是星期几
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static String GetDayOfWeek(this DateTime time)
        {
            switch (time.DayOfWeek)
            {
                case DayOfWeek.Sunday: return "星期日";
                case DayOfWeek.Monday: return "星期一";
                case DayOfWeek.Tuesday: return "星期二";
                case DayOfWeek.Wednesday: return "星期三";
                case DayOfWeek.Thursday: return "星期四";
                case DayOfWeek.Friday: return "星期五";
                case DayOfWeek.Saturday: return "星期六";
                default: return "周鑫";
            }
        }
        /// <summary>
        /// 日期格式
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static String GetDateString(this DateTime date) => date.ToString(Extter.ExtterCaller.DateFormatter);
        /// <summary>
        /// 时间格式
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static String GetTimeString(this DateTime time) => time.ToString(Extter.ExtterCaller.TimeFormatter);
        /// <summary>
        /// 日期时间格式
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static String GetDateTimeString(this DateTime dateTime) => dateTime.ToString(Extter.ExtterCaller.DateTimeFormatter);
    }
}
namespace System.Data.Extter
{
    /// <summary>
    /// 日期时间调用
    /// </summary>
    public static partial class ExtterCaller
    {
        /// <summary>
        /// Unix时间起始 1970-1-1
        /// </summary>
        public static DateTime UnixTimeStart { get; } = new DateTime(1970, 1, 1);
        /// <summary>
        /// 国庆节
        /// </summary>
        public static DateTime PRCNationalDay { get; } = new DateTime(1949, 10, 1);
        /// <summary>
        /// Unix时间起始 1970-1-1
        /// </summary>
        public static Int64 UnixTimestamp { get; } = UnixTimeStart.Ticks;
        /// <summary>
        /// 1970-1-1 00:00:00
        /// </summary>
        public static DateTime DateTime1970 { get => UnixTimeStart; }
        /// <summary>
        /// 从1970-1-1的秒数
        /// </summary>
        public static Int64 SecondsFrom1970 => 62135596800L;
        /// <summary>
        /// 从1970-1-1的秒数
        /// </summary>
        public static Int64 TicksFrom1970 => 621355968000000000L;
        /// <summary>
        /// 距离1970年的秒数
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static Int64 DistanceFrom1970Seconds(this DateTime dateTime) => (Int64)(dateTime - DateTime1970).TotalSeconds;
        /// <summary>
        /// 从距离1970的秒数还原时间
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static DateTime FromDistance1970Seconds(this Int64 seconds) => new DateTime((seconds + SecondsFrom1970) * TimeSpan.TicksPerSecond);
        /// <summary>
        /// 通用日期格式
        /// </summary>
        public static String DateFormatter { get; set; } = "yyyy-MM-dd";
        /// <summary>
        /// 通用时间格式
        /// </summary>
        public static String TimeFormatter { get; set; } = "HH:mm:ss";
        /// <summary>
        /// 通用日期时间格式
        /// </summary>
        public static String DateTimeFormatter { get; set; } = "yyyy-MM-dd HH:mm:ss";
        /// <summary>
        /// 日期格式
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static String ToDateString(this DateTime date) => date.ToString(DateFormatter);
        /// <summary>
        /// 时间格式
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static String ToTimeString(this DateTime time) => time.ToString(TimeFormatter);
        /// <summary>
        /// 日期时间格式
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static String ToDateTimeString(this DateTime dateTime) => dateTime.ToString(DateTimeFormatter);
        /// <summary>
        /// 返回距离当前时间的区间
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static TimeSpan GetNowTimeSpan(this DateTime dateTime) => GetTimeSpan(DateTime.Now, dateTime);
        /// <summary>
        /// 获取一个时间区间
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="dist"></param>
        /// <returns></returns>
        public static TimeSpan GetTimeSpan(this DateTime dateTime, DateTime dist) => dateTime - dist;
        /// <summary>
        /// 返回距离当前时间的区间
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static TimeSpan GetNowDuration(this DateTime dateTime) => GetDuration(DateTime.Now, dateTime);
        /// <summary>
        /// 获取一个时间区间
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="dist"></param>
        /// <returns></returns>
        public static TimeSpan GetDuration(this DateTime dateTime, DateTime dist) => (dateTime - dist).Duration();
    }
}
