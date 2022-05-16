using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Extter
{
    /// <summary>
    /// 扩展静态调用内容
    /// </summary>
    public static partial class ExtterCaller
    {
        /// <summary>
        /// 获取泛型实例提示信息
        /// </summary>
        public static AlertMsg<T> GetAlert<T>(this T data) => data;
        /// <summary>
        /// 获取泛型实例提示信息
        /// </summary>
        public static AlertMsg<T> GetAlert<T>(this T data, string msg) => new AlertMsg<T>(data != null, msg) { Data = data };
        /// <summary>
        /// 获取动态实例提示信息
        /// </summary>
        public static AlertMessage GetAlertMessage<T>(this T data, string msg = null) => new AlertMessage(data != null, msg) { Data = data };
#if NETFrame
        /// <summary>
        /// 获取动态接口提示信息
        /// </summary>
        public static AlertMsg GetAlert(this Tuple<bool, string> res) => new AlertMsg(res.Item1, res.Item2);
        /// <summary>
        /// 获取动态实例提示信息
        /// </summary>
        public static AlertMessage GetAlertMessage(this Tuple<bool, string> res) => new AlertMessage(res.Item1, res.Item2);
#endif
#if NETFx
        /// <summary>
        /// 获取动态接口提示信息
        /// </summary>
        public static AlertMsg GetAlert(this (bool IsSuccess, String Message) res) => new AlertMsg(res.IsSuccess, res.Message);
        /// <summary>
        /// 获取动态实例提示信息
        /// </summary>
        public static AlertMessage GetAlertMessage(this (bool IsSuccess, String Message) res) => new AlertMessage(res.IsSuccess, res.Message);
#endif
    }
}
