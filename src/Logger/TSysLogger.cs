using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Cobber;
using System.Linq;
using System.Text;

namespace System.Data.Logger
{
    /// <summary>
    /// 本地日志
    /// </summary>
    [DbCol("日志记录表", Name = nameof(TSysLoggers))]
    public partial class TSysLoggers
    {
        /// <summary>
        /// 标识
        /// </summary>
        [Key]
        [DbCol("唯一标识", Len = 64, Key = DbIxType.PK)]
        public virtual String ID { get; set; }
        /// <summary>
        /// 日志类型
        /// </summary>
        [DbCol("日志类型", Type = DbColType.Int32)]
        public virtual Int32 Type { get; set; }
        /// <summary>
        /// 日志内容
        /// </summary>
        [DbCol("日志内容", Type = DbColType.StringNormal)]
        public virtual String Content { get; set; }
        /// <summary>
        /// 操作的数据集
        /// </summary>
        [DbCol("操作的数据集", Type = DbColType.StringNormal, IsReq = false)]
        public virtual String Data { get; set; }
        /// <summary>
        /// 操作的数据集
        /// </summary>
        [DbCol("操作的数据集", Type = DbColType.StringMax, IsReq = false)]
        public virtual String Extra { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [DbCol("创建时间", Type = DbColType.DateTime)]
        public virtual DateTime CTime { get; set; }
        /// <summary>
        /// 终端Mac/客户端IP
        /// </summary>
        [DbCol("终端Mac/客户端IP", Len = 255)]
        public virtual String DeviceID { get; set; }
        /// <summary>
        /// 客户端类型
        /// </summary>
        [DbCol("客户端类型", Len = 255)]
        public virtual String CType { get; set; }
        /// <summary>
        /// 标记
        /// </summary>
        [DbCol("标记", Type = DbColType.Int32)]
        public virtual Int32 Status { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [DbCol("备注", Len = 255, Default = "操作日志")]
        public virtual String Memo { get; set; } = string.Empty;
    }
}
