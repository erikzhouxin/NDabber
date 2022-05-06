using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// 动态类型信息模型
    /// </summary>
    public class DyclassInfoModel
    {
        /// <summary>
        /// 引用
        /// </summary>
        public string[] Using { get; set; }
        /// <summary>
        /// 命名空间
        /// </summary>
        public String Namespace { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 特性数组
        /// </summary>
        public Attribute[] Attributes { get; set; }
    }
}
