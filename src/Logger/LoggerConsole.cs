using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Logger
{
    /// <summary>
    /// 日志输出
    /// </summary>
    public static class LoggerConsole
    {
        /// <summary>
        /// 级别
        /// </summary>
        public static LoggerLevel Level { get; set; } = LoggerLevel.Information;
        /// <summary>
        /// 写日志委托
        /// </summary>
        public static Action<LoggerLevel, string, Exception> LogAction { get; set; } = (level, message, ex) =>
        {
            if (level >= Level) { Console.WriteLine("{0} [{1}] {2} {3}", DateTime.Now, level, message, ex); }
        };
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public static void Warn(string message, Exception ex = null)
        {
            LogAction(LoggerLevel.Warning, message, ex);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public static void Error(string message, Exception ex = null)
        {
            LogAction(LoggerLevel.Error, message, ex);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public static void Debug(string message, Exception ex = null)
        {
            LogAction(LoggerLevel.Debug, message, ex);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public static void Info(string message, Exception ex = null)
        {
            LogAction(LoggerLevel.Information, message, ex);
        }
    }
}
