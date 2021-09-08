using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Cobber
{
    public static partial class CobberCaller
    {
        /// <summary>
        /// 转换成秒时间字符串
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static String ToSecondString(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm:ss");
        }
        /// <summary>
        /// 转换成日期字符串
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static String ToDateString(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd");
        }
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
    }
}
namespace System.Data.Extter
{
    /// <summary>
    /// 日期时间调用
    /// </summary>
    public static class DateTimeCaller
    {
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
        public static String ToDateString(this DateTime date)
        {
            return date.ToString(DateFormatter);
        }
        /// <summary>
        /// 时间格式
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static String ToTimeString(this DateTime time)
        {
            return time.ToString(TimeFormatter);
        }
        /// <summary>
        /// 日期时间格式
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static String ToDateTimeString(this DateTime dateTime)
        {
            return dateTime.ToString(DateTimeFormatter);
        }
    }
}
