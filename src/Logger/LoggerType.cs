using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Linq;
using System.Text;

namespace System.Data.Logger
{
    /// <summary>
    /// 日志类型
    /// </summary>
    [EDisplay("日志类型")]
    public enum LoggerType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [EDisplay("未知")]
        Unknown = 0,
        /// <summary>
        /// 系统日志
        /// </summary>
        [EDisplay("系统日志")]
        System = 1,
        /// <summary>
        /// 操作日志
        /// </summary>
        [EDisplay("操作日志")]
        Operator = 2,
        /// <summary>
        /// 异常日志
        /// </summary>
        [EDisplay("异常日志")]
        Exception = 3,
        /// <summary>
        /// 备份日志
        /// </summary>
        [EDisplay("备份日志")]
        Backup = 4,
        /// <summary>
        /// 升级日志
        /// </summary>
        [EDisplay("升级日志")]
        Upgrade = 5,
        /// <summary>
        /// 清理日志
        /// </summary>
        [EDisplay("清理日志")]
        Clear = 6,
        /// <summary>
        /// 修复日志
        /// </summary>
        [EDisplay("修复日志")]
        Repair = 7,
        /// <summary>
        /// 上传日志
        /// </summary>
        [EDisplay("上传日志")]
        Upload = 8,
        /// <summary>
        /// 请求日志
        /// </summary>
        [EDisplay("请求日志")]
        Request = 9,

        /// <summary>
        /// 调试日志
        /// </summary>
        [EDisplay("调试日志")]
        Debug = 101,
        /// <summary>
        /// 跟踪日志
        /// </summary>
        [EDisplay("跟踪日志")]
        Trace = 102,
        /// <summary>
        /// 信息日志
        /// </summary>
        [EDisplay("信息日志")]
        Info = 103,
        /// <summary>
        /// 警告日志
        /// </summary>
        [EDisplay("警告日志")]
        Warn = 104,
        /// <summary>
        /// 错误日志
        /// </summary>
        [EDisplay("错误日志")]
        Error = 105,
        /// <summary>
        /// 崩溃日志
        /// </summary>
        [EDisplay("崩溃日志")]
        Crash = 106,

        /// <summary>
        /// 设置日志
        /// </summary>
        [EDisplay("设置日志")]
        Setting = 201,

        /// <summary>
        /// 资源日志
        /// </summary>
        [EDisplay("资源日志")]
        Resources = 301,

        /// <summary>
        /// 内部日志
        /// </summary>
        [EDisplay("内部日志")]
        Inner = 401,

        /// <summary>
        /// 外部日志
        /// </summary>
        [EDisplay("外部日志")]
        Outer = 501,

        /// <summary>
        /// 硬件日志
        /// </summary>
        [EDisplay("硬件日志")]
        Hardware = 601,

        /// <summary>
        /// 数据库日志
        /// </summary>
        [EDisplay("数据库日志")]
        Database = 701,

        /// <summary>
        /// 消息日志
        /// </summary>
        [EDisplay("消息日志")]
        Message = 801,

        /// <summary>
        /// 发布日志
        /// </summary>
        [EDisplay("发布日志")]
        Publish = 901,

        /// <summary>
        /// 发行日志
        /// </summary>
        [EDisplay("发行日志")]
        Release = 1001,

        /// <summary>
        /// 隐藏日志
        /// </summary>
        [EDisplay("隐藏日志")]
        Hide = 1101,

        /// <summary>
        /// 优先日志
        /// </summary>
        [EDisplay("优先日志")]
        Advance = 1201,

        /// <summary>
        /// 开发日志
        /// </summary>
        [EDisplay("开发日志")]
        Develop = 1301,

        /// <summary>
        /// 测试日志
        /// </summary>
        [EDisplay("测试日志")]
        Test = 1401,

        /// <summary>
        /// 用户日志
        /// </summary>
        [EDisplay("用户日志")]
        User = 1501,

        /// <summary>
        /// 自动日志
        /// </summary>
        [EDisplay("自动日志")]
        Auto = 1601,

        /// <summary>
        /// 临时日志
        /// </summary>
        [EDisplay("临时日志")]
        Temp = 1701,

        /// <summary>
        /// 设计日志
        /// </summary>
        [EDisplay("设计日志")]
        Design = 1801,

        /// <summary>
        /// 模板日志
        /// </summary>
        [EDisplay("模板日志")]
        Template = 1901,

        /// <summary>
        /// 代码日志
        /// </summary>
        [EDisplay("代码日志")]
        Coder = 2001,

        /// <summary>
        /// 服务日志
        /// </summary>
        [EDisplay("服务日志")]
        Service = 2101,

        /// <summary>
        /// 代理日志
        /// </summary>
        [EDisplay("代理日志")]
        Proxy = 2201,

        /// <summary>
        /// 访问日志
        /// </summary>
        [EDisplay("访问日志")]
        Access = 2301,

        /// <summary>
        /// 网络日志
        /// </summary>
        [EDisplay("网络日志")]
        Web = 2401,

        /// <summary>
        /// 环境日志
        /// </summary>
        [EDisplay("环境日志")]
        Context = 2501,

        /// <summary>
        /// 附加日志
        /// </summary>
        [EDisplay("附加日志")]
        Extra = 2601,
    }
}
