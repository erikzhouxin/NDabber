using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Linq;
using System.Text;

namespace System.Data.Extter
{
    /// <summary>
    /// 性别
    /// </summary>
    [EDisplay("性别完整类型")]
    public enum SexType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [EDisplay("未知")]
        Unknown = 0,
        /// <summary>
        /// 男性
        /// </summary>
        [EDisplay("男性")]
        Male = 1,
        /// <summary>
        /// 女性
        /// </summary>
        [EDisplay("女性")]
        Female = 2,
        /// <summary>
        /// 女性改（变）为男性
        /// </summary>
        [EDisplay("女性改（变）为男性")]
        F2M = 5,
        /// <summary>
        /// 男性改（变）为女性
        /// </summary>
        [EDisplay("男性改（变）为女性")]
        M2F = 6,
        /// <summary>
        /// 未说明
        /// </summary>
        [EDisplay("未说明")]
        Other = 9
    }
    /// <summary>
    /// 性别
    /// </summary>
    [EDisplay("性别")]
    public enum SexSType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [EDisplay("未知")]
        Unknown = 0,
        /// <summary>
        /// 女性
        /// </summary>
        [EDisplay("女性")]
        Female = 1,
        /// <summary>
        /// 
        /// </summary>
        [EDisplay("男性")]
        Male = 2,
    }
}
