using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Cobber;
using System.Linq;
using System.Text;

namespace System.Data.Extter
{
    /// <summary>
    /// 本地参数表
    /// </summary>
    [DbCol("本地参数表", Name = nameof(TSysParams))]
    public partial class TSysParams
    {
        /// <summary>
        /// 标识
        /// </summary>
        [Key]
        [DbCol("标识", Type = DbColType.Int32, Key = DbIxType.APK)]
        public virtual Int32 ID { get; set; }
        /// <summary>
        /// 键名
        /// </summary>
        [DbCol("键名", Len = 255)]
        public virtual String Key { get; set; }
        /// <summary>
        /// 键值
        /// </summary>
        [DbCol("键值", Type = DbColType.StringMax)]
        public virtual String Value { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [DbCol("备注", Len = 255)]
        public virtual String Memo { get; set; }
    }
}
