using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Extter
{
    /// <summary>
    /// 提示信息类型
    /// </summary>
    public enum AlertCodeType
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// 信息
        /// </summary>
        Info = 120010,
        /// <summary>
        /// 主要
        /// </summary>
        Primary = 120020,
        /// <summary>
        /// 警告
        /// </summary>
        Warn = 120030,
        /// <summary>
        /// 危险
        /// </summary>
        Danger = 120040,
        /// <summary>
        /// 成功
        /// </summary>
        Success = 120050,
    }
}
