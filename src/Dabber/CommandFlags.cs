using System;
using System.Data.Cobber;

namespace System.Data.Dabber
{
    /// <summary>
    /// 控制命令行为的附加状态标志
    /// </summary>
    [Flags]
    [EDisplay("命令特征")]
    public enum CommandFlags
    {
        /// <summary>
        /// 无
        /// </summary>
        [EDisplay("无")]
        None = 0,
        /// <summary>
        /// 数据是否应该在返回之前进行缓冲?
        /// </summary>
        [EDisplay("缓冲")]
        Buffered = 1,
        /// <summary>
        /// 异步查询可以流水线化吗?
        /// </summary>
        [EDisplay("流水线")]
        Pipelined = 2,
        /// <summary>
        /// 计划缓存应该被绕过吗?
        /// </summary>
        [EDisplay("无缓存")]
        NoCache = 4,
    }

}
