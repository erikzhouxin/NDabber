using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Linq;
using System.Text;

namespace System.Data.Extter
{
    /// <summary>
    /// 设置类型
    /// </summary>
    [Flags]
    [EDisplay("设置类型")]
    public enum SettingType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [EDisplay("未知")]
        Unknown = 0,
        /// <summary>
        /// 通用设置
        /// </summary>
        [EDisplay("通用设置")]
        App = 1,
        /// <summary>
        /// 应用设置
        /// </summary>
        [EDisplay("应用设置")]
        Settings = 2,
        /// <summary>
        /// 资源设置
        /// </summary>
        [EDisplay("资源设置")]
        Resources = 4,
        /// <summary>
        /// 内部设置
        /// </summary>
        [EDisplay("内部设置")]
        Inner = 8,
        /// <summary>
        /// 外部设置
        /// </summary>
        [EDisplay("外部设置")]
        Outer = 16,
        /// <summary>
        /// 硬件设置
        /// </summary>
        [EDisplay("硬件设置")]
        Hardware = 32,
        /// <summary>
        /// 数据库设置
        /// </summary>
        [EDisplay("数据库设置")]
        Database = 64,
        /// <summary>
        /// 消息设置
        /// </summary>
        [EDisplay("消息设置")]
        Message = 128,
        /// <summary>
        /// 跟踪设置
        /// </summary>
        [EDisplay("跟踪设置")]
        Trace = 256,
        /// <summary>
        /// 信息设置
        /// </summary>
        [EDisplay("信息设置")]
        Info = 512,
        /// <summary>
        /// 调试设置
        /// </summary>
        [EDisplay("调试设置")]
        Debug = 1024,
        /// <summary>
        /// 警告设置
        /// </summary>
        [EDisplay("警告设置")]
        Warn = 2048,
        /// <summary>
        /// 错误设置
        /// </summary>
        [EDisplay("错误设置")]
        Error = 4096,
        /// <summary>
        /// 崩溃设置
        /// </summary>
        [EDisplay("崩溃设置")]
        Crash = 8192,
        /// <summary>
        /// 发布设置
        /// </summary>
        [EDisplay("发布设置")]
        Publish = 16384,
        /// <summary>
        /// 发行设置
        /// </summary>
        [EDisplay("发行设置")]
        Release = 16384,
        /// <summary>
        /// 隐藏设置
        /// </summary>
        [EDisplay("隐藏设置")]
        Hide = 32768,
        /// <summary>
        /// 优先设置
        /// </summary>
        [EDisplay("优先设置")]
        Advance = 65536,
        /// <summary>
        /// 开发设置
        /// </summary>
        [EDisplay("开发设置")]
        Develop = 131072,
        /// <summary>
        /// 测试设置
        /// </summary>
        [EDisplay("测试设置")]
        Test = 262144,
        /// <summary>
        /// 用户设置
        /// </summary>
        [EDisplay("用户设置")]
        User = 524288,
        /// <summary>
        /// 自动设置
        /// </summary>
        [EDisplay("自动设置")]
        Auto = 1048576,
        /// <summary>
        /// 临时设置
        /// </summary>
        [EDisplay("临时设置")]
        Temp = 2097152,
        /// <summary>
        /// 设计设置
        /// </summary>
        [EDisplay("设计设置")]
        Design = 4194304,
        /// <summary>
        /// 模板设置
        /// </summary>
        [EDisplay("模板设置")]
        Template = 8388608,
        /// <summary>
        /// 代码设置
        /// </summary>
        [EDisplay("代码设置")]
        Coder = 16777216,
        /// <summary>
        /// 服务设置
        /// </summary>
        [EDisplay("服务设置")]
        Service = 33554432,
        /// <summary>
        /// 代理设置
        /// </summary>
        [EDisplay("代理设置")]
        Proxy = 67108864,
        /// <summary>
        /// 访问设置
        /// </summary>
        [EDisplay("访问设置")]
        Access = 134217728,
        /// <summary>
        /// 请求设置
        /// </summary>
        [EDisplay("请求设置")]
        Request = 268435456,
        /// <summary>
        /// 网络设置
        /// </summary>
        [EDisplay("网络设置")]
        Web = 268435456,
        /// <summary>
        /// 环境设置
        /// </summary>
        [EDisplay("环境设置")]
        Context = 536870912,
        /// <summary>
        /// 附加设置
        /// </summary>
        [EDisplay("附加设置")]
        Extra = 1073741824,
    }
}
