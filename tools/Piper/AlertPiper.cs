using System;
using System.Collections.Generic;
using System.Data.Dabber;
using System.Linq;
using System.Text;

namespace System.Data.Piper
{
    /// <summary>
    /// 字符串数据
    /// </summary>
    public class AlertPipeString : AlertPipeModel<String> { }
    /// <summary>
    /// 泛型数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AlertPipeModel<T>
    {
        /// <summary>
        /// 命令
        /// </summary>
        public string C { get; set; }
        /// <summary>
        /// 模型内容
        /// </summary>
        public T M { get; set; }
    }
    /// <summary>
    /// 管道提示结果
    /// </summary>
    public class AlertPipeResult : AlertPipeModel<String>
    {
        /// <summary>
        /// 代码
        /// </summary>
        public int K { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool I { get; set; }
        /// <summary>
        /// 文本提示
        /// </summary>
        public dynamic D { get; set; }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="alert"></param>
        public AlertPipeResult(IAlertMsg alert)
        {
            C = alert.Code.ToString();
            I = alert.IsSuccess;
            M = alert.Message;
            D = alert.Data;
            K = alert.Code;
        }
        /// <summary>
        /// 构造
        /// </summary>
        public AlertPipeResult() { }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="message"></param>
        public AlertPipeResult(bool isSuccess, string message)
        {
            this.I = isSuccess;
            this.M = message;
        }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="message"></param>
        /// <param name="code"></param>
        /// <param name="data"></param>
        public AlertPipeResult(bool isSuccess, string message, int code, object data)
        {
            this.I = isSuccess;
            this.M = message;
            this.C = code.ToString();
            this.D = data;
            this.K = code;
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="alert"></param>
        public static implicit operator AlertPipeResult(AlertMsg alert)
        {
            return new AlertPipeResult(alert);
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="alert"></param>
        public static implicit operator AlertMsg(AlertPipeResult alert)
        {
            return new AlertMsg(alert.I, alert.M) { Code = alert.K, Data = alert.D };
        }
        /// <summary>
        /// 获取未知命令结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static AlertPipeResult GetUnknown<T>(AlertPipeModel<T> arg)
        {
            return new AlertPipeResult(false, "未知命令", 404, arg);
        }
    }
}
