using System;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Linq;
using System.Text;

namespace System.Data.Hopper
{
    /// <summary>
    /// 网络请求回调内容
    /// </summary>
    public static partial class HopperCaller
    {
        /// <summary>
        /// 获取网络请求方式
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static HttpReqMethodType GetHttpReqMethodType(string method)
        {
            return NEnumerable<HttpReqMethodType>.GetFromName(method).Enum;
        }
    }
}
