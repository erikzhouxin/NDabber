using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Logger
{
    /// <summary>
    /// 日志级别
    /// [Microsoft.Extensions.Logging.LogLevel]
    /// </summary>
    public enum LoggerLevel
    {
        /// <summary>
        /// 跟踪日志
        /// </summary>
        Trace,
        /// <summary>
        /// 调试日志
        /// </summary>
        Debug,
        /// <summary>
        /// 信息日志
        /// </summary>
        Information,
        /// <summary>
        /// 警告日志
        /// </summary>
        Warning,
        /// <summary>
        /// 错误日志
        /// </summary>
        Error,
        /// <summary>
        /// 严重日志
        /// </summary>
        Critical,
        /// <summary>
        /// 不写日志
        /// </summary>
        None
    }
}
