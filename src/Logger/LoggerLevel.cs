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
#if NETFrame
namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Defines logging severity levels
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Logs that contain the most detailed messages. These messages may contain sensitive
        /// application data. These messages are disabled by default and should never be
        /// enabled in a production environment.
        /// </summary>
        Trace,
        /// <summary>
        /// Logs that are used for interactive investigation during development. These logs
        /// should primarily contain information useful for debugging and have no long-term
        /// value.
        /// </summary>
        Debug,
        /// <summary>
        /// Logs that track the general flow of the application. These logs should have long-term
        /// value.
        /// </summary>
        Information,
        /// <summary>
        /// Logs that highlight an abnormal or unexpected event in the application flow,
        /// but do not otherwise cause the application execution to stop.
        /// </summary>
        Warning,
        /// <summary>
        /// Logs that highlight when the current flow of execution is stopped due to a failure.
        /// These should indicate a failure in the current activity, not an application-wide
        /// failure.
        /// </summary>
        Error,
        /// <summary>
        /// Logs that describe an unrecoverable application or system crash, or a catastrophic
        /// failure that requires immediate attention.
        /// </summary>
        Critical,
        /// <summary>
        /// Not used for writing log messages. Specifies that a logging category should not
        /// write any messages.
        /// </summary>
        None
    }
}
#endif
