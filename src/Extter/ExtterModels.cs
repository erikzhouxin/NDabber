using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Cobber;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace System.Data.Extter
{
    /// <summary>
    /// 系统设置
    /// </summary>
    [DbCol("系统设置", Name = nameof(TSysSettings))]
    public class TSysSettings
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        [Key]
        [DbCol("唯一标识", Len = 64, Key = DbIxType.PK)]
        public virtual String ID { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        [DbCol("分类", Type = DbColType.Int32, Key = DbIxType.UIX, Index = nameof(Cate) + "|" + nameof(Key))]
        public virtual Int32 Cate { get; set; }
        /// <summary>
        /// 键名
        /// </summary>
        [DbCol("键名", Len = 255)]
        public virtual String Key { get; set; }
        /// <summary>
        /// 字符键值
        /// </summary>
        [DbCol("字符键值", Type = DbColType.StringMax)]
        public virtual String Value { get; set; } = "{}";
        /// <summary>
        /// 类型
        /// </summary>
        [DbCol("类型", Type = DbColType.StringNormal)]
        public virtual String Type { get; set; } = string.Empty;
        /// <summary>
        /// 备注
        /// </summary>
        [DbCol("备注", Len = 255)]
        public virtual String Memo { get; set; } = string.Empty;
    }
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
