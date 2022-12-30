﻿using System;

namespace System
{
    /// <summary>
    /// 交换数据接口
    /// </summary>
    public interface IDataSwapModel
    {
        /// <summary>
        /// 标识
        /// </summary>
        string I { get; set; }
        /// <summary>
        /// 命令
        /// </summary>
        string C { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        string M { get; set; }
        /// <summary>
        /// 参数值(Json模型)
        /// </summary>
        string P { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        DateTime T { get; set; }
        /// <summary>
        /// 参数键
        /// </summary>
        string K { get; set; }
        /// <summary>
        /// 是否返回
        /// </summary>
        bool F { get; set; }
        /// <summary>
        /// 返回数据
        /// </summary>
        string R { get; set; }
    }
    /// <summary>
    /// 交换模型
    /// </summary>
    public class DataSwapModel : IDataSwapModel
    {
        /// <summary>
        /// 未找到代码
        /// </summary>
        public const string NotFoundCode = "404";
        /// <summary>
        /// 未找到命令
        /// </summary>
        public const string NotFoundCmd = "notfound";
        /// <summary>
        /// 响应代号
        /// </summary>
        public const string ResponseCode = "200";
        /// <summary>
        /// 响应命令
        /// </summary>
        public const string ResponseCmd = "response";
        /// <summary>
        /// 错误代号
        /// </summary>
        public const string ErrorCode = "500";
        /// <summary>
        /// 错误命令
        /// </summary>
        public const string ErrorCmd = "error";
        /// <summary>
        /// 标识
        /// </summary>
        public virtual string I { get; set; }
        /// <summary>
        /// 命令
        /// </summary>
        public virtual string C { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public virtual string M { get; set; }
        /// <summary>
        /// 参数值(Json模型)
        /// </summary>
        public virtual string P { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public virtual DateTime T { get; set; }
        /// <summary>
        /// 参数键
        /// </summary>
        public virtual string K { get; set; }
        /// <summary>
        /// 是否返回
        /// </summary>
        public virtual bool F { get; set; }
        /// <summary>
        /// 返回数据
        /// </summary>
        public virtual string R { get; set; }
    }
    /// <summary>
    /// 命名管道转换模型
    /// </summary>
    public class PiperSwapModel : DataSwapModel { }
    /// <summary>
    /// Socket转换模型
    /// </summary>
    public class SocketSwapModel : DataSwapModel { }
    /// <summary>
    /// Mqtt转换模型
    /// </summary>
    public class MqttSwapModel : DataSwapModel { }
}