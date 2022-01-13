using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Dabber
{
    /// <summary>
    /// sqlite_master
    /// </summary>
    public sealed class SqlitePragmaMasterInfo
    {
        /// <summary>
        /// 表类型
        /// </summary>
        public const String TypeTable = "table";
        /// <summary>
        /// 索引类型
        /// </summary>
        public const String TypeIndex = "index";
        /// <summary>
        /// 视图类型
        /// </summary>
        public const String TypeView = "view";
        /// <summary>
        /// 触发器类型
        /// </summary>
        public const string TypeTrigger = "trigger";
        /// <summary>
        /// 类型
        /// table
        /// index
        /// view
        /// trigger
        /// </summary>
        public String Type { get; set; }
        /// <summary>
        /// 表名/索引名/视图名/触发器名(无引号)
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// 所在表名(无引号)
        /// </summary>
        public String TblName { get => Tbl_Name; set => Tbl_Name = value; }
        /// <summary>
        /// 所在表名(无引号)(源列)
        /// </summary>
        public String Tbl_Name { get; set; }
        /// <summary>
        /// 起始页
        /// </summary>
        public Int32 Rootpage { get; set; }
        /// <summary>
        /// 创建SQL
        /// </summary>
        public String Sql { get; set; }
    }
}
