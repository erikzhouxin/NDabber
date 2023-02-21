using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using static System.Net.WebRequestMethods;

namespace System.Data.Hopper
{
    /// <summary>
    /// HTTP 请求方式类型 System.Net.Http中HttpMethod
    /// HTTP 是超文本传输协议，用来定义客户端与服务器数据传输的规范。
    /// HTTP 服务端默认端口为 80，HTTPS 默认端口为 443，客户端的端口是动态分配的。
    /// HTTP 请求方式一共有 9 种，分别为 POST 、GET 、HEAD、PUT 、PATCH 、 OPTIONS 、DELETE 、CONNECT 、 TRACE 。
    /// 其中前三种 POST 、GET 、HEAD 是 HTTP 1.0 定义的，
    /// 后六种 PUT 、PATCH 、 OPTIONS 、DELETE 、CONNECT 、 TRACE 是 HTTP 1.1 定义的。
    /// </summary>
    public enum HttpReqMethodType
    {
        /// <summary>
        /// 表示向指定资源提交数据，数据包含在请求头中。
        /// 有可能导致新的资源建立或原有资源修改。 
        /// POST 请求是 HTTP 请求中使用最多的一种请求方式。
        /// </summary>
        POST,
        /// <summary>
        /// 表示请求指定的页面信息，并返回实体内容。
        /// </summary>
        GET,
        /// <summary>
        /// 类似于 GET，只不过返回的响应体中没有具体内容，只有报文头，用于获取报文头。
        /// </summary>
        HEAD,
        /// <summary>
        /// 从客户端向服务器传送的数据取代指定的内容，即向指定的位置上传最新的内容。
        /// </summary>
        PUT,
        /// <summary>
        /// 对 PUT 方法的补充，用来对已知资源进行局部更新。
        /// </summary>
        PATCH,
        /// <summary>
        /// 返回服务器针对特殊资源所支持的 HTML 请求方式 或 允许客户端查看服务器的性能。
        /// </summary>
        OPTIONS,
        /// <summary>
        /// 请求服务器删除 Request-URL 所标识的资源。
        /// </summary>
        DELETE,
        /// <summary>
        /// HTTP 1.1 中预留给能够将连接改为管道方式的代理服务器。
        /// </summary>
        CONNECT,
        /// <summary>
        /// 回显服务器收到的请求，主要用于测试和诊断。
        /// </summary>
        TRACE
    }
}
