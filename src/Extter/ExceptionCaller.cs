using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Extter
{
    /// <summary>
    /// 异常调用
    /// </summary>
    public static class ExceptionCaller
    {
        /// <summary>
        /// 获取异常的信息及跟踪信息文本
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string GetMessageStackTraceString(this Exception exception)
        {
            var sb = new StringBuilder();
            sb.Append(exception.Message);
            if(exception.StackTrace != null)
            {
                sb.AppendLine().Append(exception.StackTrace);
            }
            return sb.ToString();
        }
    }
}
