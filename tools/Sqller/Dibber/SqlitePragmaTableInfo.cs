using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Dibber
{
    /// <summary>
    /// SQLite表信息内容
    /// </summary>
    public class SqlitePragmaTableInfo
    {
        /// <summary>
        /// 列标识
        /// </summary>
        public Int32 CID { get; set; }
        /// <summary>
        /// 列名称
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// 列类型
        /// </summary>
        public String Type { get; set; }
        /// <summary>
        /// 不为空
        /// </summary>
        public Boolean NotNull { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public String Deflt_Value { get; set; }
        /// <summary>
        /// 主键
        /// </summary>
        public Boolean PK { get; set; }
        /// <summary>
        /// 隐藏
        /// </summary>
        public Boolean Hidden { get; set; }
    }
}
