using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.IO;
using System.Linq;
using System.Text;

namespace System.Data.Logger
{
    /// <summary>
    /// 日志调用者
    /// </summary>
    public static class LoggerCaller
    {
        /// <summary>
        /// 使用文件
        /// </summary>
        /// <returns></returns>
        public static TextWriter UseFile()
        {
            return new TextLogWriter();
        }
        /// <summary>
        /// 使用SQLite数据库存储内容
        /// </summary>
        /// <param name="GetConnection"></param>
        /// <returns></returns>
        public static TextWriter UseSQLite(Func<String,IDbConnection> GetConnection)
        {
            SQLiteConsoleWriter.GetConnection = GetConnection;
            return new SQLiteConsoleWriter();
        }
    }
}
