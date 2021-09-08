using System;
using System.Collections.Generic;
using System.Data.Cobber;
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
        /// 设置日志存储
        /// </summary>
        /// <param name="storeModel"></param>
        /// <param name="logType"></param>
        public static void SetConsole(StoreModel storeModel, LogType logType)
        {
            switch (storeModel.DbType)
            {
                case StoreType.SQLite:
                    SQLiteLogWriter.Initial(logType);
                    Console.SetOut(new SQLiteLogWriter()); // 未完成
                    break;
                case StoreType.SqlServer:
                case StoreType.MySQL:
                case StoreType.Oracle:
                case StoreType.PostgreSQL:
                case StoreType.Redis:
                case StoreType.Access:
                case StoreType.Excel:
                case StoreType.Xml:
                case StoreType.Memory:
                default:
                    Console.SetOut(new TextLogWriter());
                    break;
            }
        }
        /// <summary>
        /// 日志类型
        /// </summary>
        public enum LogType
        {
            /// <summary>
            /// 单库单表
            /// </summary>
            SingleByLogger,
            /// <summary>
            /// 单库年表
            /// </summary>
            SingleByYear,
            /// <summary>
            /// 单库季表
            /// </summary>
            SingleByQuarter,
            /// <summary>
            /// 年库单表
            /// </summary>
            YearByLogger,
            /// <summary>
            /// 年库月表
            /// </summary>
            YearByMonth,
            /// <summary>
            /// 年库季表
            /// </summary>
            YearByQuarter,
            /// <summary>
            /// 月库单表
            /// </summary>
            MonthByLogger,
            /// <summary>
            /// 月库日表
            /// </summary>
            MonthByDay,
        }
    }
}
