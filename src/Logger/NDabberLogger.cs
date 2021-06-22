using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Dabber
{
    /// <summary>
    /// 日志类
    /// </summary>
    public interface IDabberLogger
    {
        /// <summary>
        /// 写入异常
        /// </summary>
        /// <param name="ex"></param>
        void Write(Exception ex);
        /// <summary>
        /// 写入信息
        /// </summary>
        /// <param name="message"></param>
        void Write(string message);
    }
    /// <summary>
    /// 系统日志
    /// </summary>
    public static class NDabberLogger
    {
        /// <summary>
        /// 日志组件
        /// </summary>
        internal static IDabberLogger Logger { get; private set; }
        /// <summary>
        /// 注册日志
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static IDabberLogger Regist<T>(T model) where T : IDabberLogger
        {
            Logger = model;
            return model;
        }
        /// <summary>
        /// 写入异常
        /// </summary>
        /// <param name="ex">异常类</param>
        internal static void Write(Exception ex)
        {
            Task.Factory.StartNew(() => Logger?.Write(ex));
        }
        /// <summary>
        /// 写入异常
        /// </summary>
        /// <param name="ex">异常类</param>
        internal static void WriteLogger(this Exception ex)
        {
            Task.Factory.StartNew(() => Logger?.Write(ex));
        }
        /// <summary>
        /// 写入异常
        /// </summary>
        /// <param name="message">异常信息</param>
        internal static void Write(string message)
        {
            Task.Factory.StartNew(() => Logger?.Write(message));
        }
    }
}
