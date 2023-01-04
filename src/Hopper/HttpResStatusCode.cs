using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Hopper
{
    /// <summary>
    /// 为HTTP定义的状态码值
    /// <see cref="System.Net.HttpStatusCode"/>
    /// </summary>
    public enum HttpResStatusCode
    {
        /// <summary>
        /// 等价于HTTP状态100。System.Net.HttpStatusCode.Continue表示
        /// 客户端可以继续它的请求
        /// </summary>
        Continue = 100,
        /// <summary>
        /// 等价于HTTP状态101。System.Net.HttpStatusCode.SwitchingProtocols表示
        /// 协议版本或协议正在被更改。
        /// </summary>
        SwitchingProtocols = 101,
        /// <summary>
        /// 等价于HTTP状态102。System.Net.HttpStatusCode.Processing表示
        /// 服务器已经接受了完整的请求，但还没有完成。
        /// </summary>
        Processing = 102,
        /// <summary>
        /// 等价于HTTP状态103。System.Net.HttpStatusCode.EarlyHints表示
        /// 服务器可能会向客户端发送带有首部的最终响应
        /// 信息反应中包含的字段。
        /// </summary>
        EarlyHints = 103,
        /// <summary>
        /// 等价于HTTP状态200。System.Net.HttpStatusCode.OK表示
        /// 请求成功，请求的信息在响应中。这
        /// 是最常见的状态码。
        /// </summary>
        OK = 200,
        /// <summary>
        /// 等价于HTTP状态201。System.Net.HttpStatusCode.Created表示
        /// 请求导致在发送响应之前创建了一个新资源。
        /// </summary>
        Created = 201,
        /// <summary>
        /// 等价于HTTP状态202。System.Net.HttpStatusCode.Accepted表示
        /// 请求已被接受作进一步处理。
        /// </summary>
        Accepted = 202,
        /// <summary>
        /// 等价于HTTP状态203。System.Net.HttpStatusCode.NonAuthoritativeInformation
        /// 表示返回的元信息来自缓存的副本，而不是
        /// 原始服务器，因此可能不正确。
        /// </summary>
        NonAuthoritativeInformation = 203,
        /// <summary>
        /// 等价于HTTP状态204。System.Net.HttpStatusCode.NoContent表示
        /// 请求已经成功处理，响应是有意的空白。
        /// </summary>
        NoContent = 204,
        /// <summary>
        /// 等同于HTTP状态205。System.Net.HttpStatusCode.ResetContent表示
        /// 客户端应该重置(而不是重新加载)当前资源。
        /// </summary>
        ResetContent = 205,
        /// <summary>
        /// 等价于HTTP状态206。System.Net.HttpStatusCode.PartialContent表示
        /// 响应是GET请求的部分响应，包含字节范围。
        /// </summary>
        PartialContent = 206,
        /// <summary>
        /// 等价于HTTP状态207。System.Net.HttpStatusCode.MultiStatus表示
        /// 在Web分布式编写期间，单个响应的多个状态码
        /// 以及版本控制(WebDAV)操作。响应主体包含描述的XML
        /// 状态码。
        /// </summary>
        MultiStatus = 207,
        /// <summary>
        /// 等价于HTTP状态208。System.Net.HttpStatusCode.AlreadyReported表示
        /// 前面已经列举了WebDAV绑定的成员
        /// 这是多状态响应的一部分，不再包括在内。
        /// </summary>
        AlreadyReported = 208,
        /// <summary>
        /// 等同于HTTP状态226。System.Net.HttpStatusCode.IMUsed表示
        /// 服务器完成了对资源的请求，响应是一种表示
        /// 应用于当前实例的一个或多个实例操作的结果。
        /// </summary>
        IMUsed = 226,
        /// <summary>
        /// 等价于HTTP状态300。System.Net.HttpStatusCode.Ambiguous表示
        /// 所请求的信息具有多种表示。默认操作
        /// 是将此状态视为重定向并跟随内容的位置吗
        /// 与此响应关联的头信息。歧义是多项选择的同义词。
        /// </summary>
        Ambiguous = 300,
        /// <summary>
        /// 等价于HTTP状态300。System.Net.HttpStatusCode.MultipleChoices表示
        /// 所请求的信息具有多种表示。默认操作
        /// 是将此状态视为重定向并跟随内容的位置吗
        /// 与此响应关联的头信息。多项选择是歧义的同义词。
        /// </summary>
        MultipleChoices = 300,
        /// <summary>
        /// 等价于HTTP状态301。System.Net.HttpStatusCode.Moved表示
        /// 请求的信息已经移动到位置中指定的URI
        /// 头。接收到此状态时的默认操作是跟踪位置
        /// 与响应相关联的首部。当原始的请求方法是POST时，
        /// 重定向的请求将使用GET方法。Moved是Moved permanently的同义词。
        /// </summary>
        Moved = 301,
        /// <summary>
        /// 等价于HTTP状态301。System.Net.HttpStatusCode.MovedPermanently表示
        /// 请求的信息已经移动到位置中指定的URI
        /// 头。接收到此状态时的默认操作是跟踪位置
        /// 与响应相关联的首部。MovedPermanently是Moved的同义词。
        /// </summary>
        MovedPermanently = 301,
        /// <summary>
        /// 等价于HTTP状态302。System.Net.HttpStatusCode.Found指出
        /// 请求的信息位于Location头中指定的URI中。
        /// 接收到此状态时的默认操作是跟随Location头
        /// 与响应相关联。当原始的请求方法是POST时
        /// 重定向的请求将使用GET方法。Found是重定向的同义词。
        /// </summary>
        Found = 302,
        /// <summary>
        /// 等价于HTTP状态302。System.Net.HttpStatusCode.Redirect表示
        /// 请求的信息位于Location头中指定的URI中。
        /// 接收到此状态时的默认操作是跟随Location头
        /// 与响应相关联。当原始的请求方法是POST时
        /// 重定向的请求将使用GET方法。Redirect是Found的同义词。
        /// </summary>
        Redirect = 302,
        /// <summary>
        /// 等价于HTTP状态303。System.Net.HttpStatusCode.RedirectMethod自动
        /// 结果是将客户端重定向到Location首部中指定的URI
        /// 指一个岗位。对Location首部指定的资源的请求将
        /// 用一种方式获得。RedirectMethod是SeeOther的同义词。
        /// </summary>
        RedirectMethod = 303,
        /// <summary>
        /// 等价于HTTP状态303。System.Net.HttpStatusCode.SeeOther自动
        /// 结果是将客户端重定向到Location首部中指定的URI
        /// 指一个岗位。对Location首部指定的资源的请求将
        /// 用一种方式获得。SeeOther是RedirectMethod的同义词
        /// </summary>
        SeeOther = 303,
        /// <summary>
        /// 等价于HTTP状态304。System.Net.HttpStatusCode.NotModified表示
        /// 客户端的缓存副本是最新的。资源的内容是
        /// 不转移。
        /// </summary>
        NotModified = 304,
        /// <summary>
        /// 等价于HTTP状态305。System.Net.HttpStatusCode.UseProxy表示
        /// 请求应该使用位置中指定的URI的代理服务器
        /// 头。
        /// </summary>
        UseProxy = 305,
        /// <summary>
        /// 等价于HTTP状态306。建议使用System.Net.HttpStatusCode.Unused
        /// 未完全指定的HTTP/1.1规范的扩展。
        /// </summary>
        Unused = 306,
        /// <summary>
        /// 等价于HTTP状态307。System.Net.HttpStatusCode.RedirectKeepVerb表示
        /// 请求信息位于该位置中指定的URI中
        /// 头。接收到此状态时的默认操作是跟踪位置
        /// 与响应相关联的首部。当原始的请求方法是POST时，
        /// 重定向的请求也将使用POST方法。RedirectKeepVerb是一个同义词
        /// TemporaryRedirect。
        /// </summary>
        RedirectKeepVerb = 307,
        /// <summary>
        /// 等价于HTTP状态307。System.Net.HttpStatusCode.TemporaryRedirect表示
        /// 请求信息位于该位置中指定的URI中
        /// 头。接收到此状态时的默认操作是跟踪位置
        /// 与响应相关联的首部。当原始的请求方法是POST时，
        /// 重定向的请求也将使用POST方法。TemporaryRedirect是一个
        /// RedirectKeepVerb的同义词。
        /// </summary>
        TemporaryRedirect = 307,
        /// <summary>
        /// 等价于HTTP状态308。System.Net.HttpStatusCode.PermanentRedirect表示
        /// 请求信息位于该位置中指定的URI中
        /// 头。接收到此状态时的默认操作是跟踪位置
        /// 与响应相关联的首部。当原始的请求方法是POST时，
        /// 重定向的请求也将使用POST方法。
        /// </summary>
        PermanentRedirect = 308,
        /// <summary>
        /// 等价于HTTP状态400。System.Net.HttpStatusCode.BadRequest表示
        /// 服务器无法理解该请求。System.Net.HttpStatusCode.BadRequest
        /// 在没有其他错误时，或者确切的错误未知时，或者
        /// 没有自己的错误码。
        /// </summary>
        BadRequest = 400,
        /// <summary>
        /// 等价于HTTP状态401。System.Net.HttpStatusCode.Unauthorized表示
        /// 请求的资源需要身份验证。WWW-Authenticate报头
        /// 包含如何执行身份验证的详细信息。
        /// </summary>
        Unauthorized = 401,
        /// <summary>
        /// 等价于HTTP状态402。System.Net.HttpStatusCode.PaymentRequired是保留的
        /// 供将来使用。
        /// </summary>
        PaymentRequired = 402,
        /// <summary>
        /// 等价于HTTP status 403。System.Net.HttpStatusCode.Forbidden表示
        /// 服务器拒绝完成请求。
        /// </summary>
        Forbidden = 403,
        /// <summary>
        /// 等价于HTTP状态404。System.Net.HttpStatusCode.NotFound指出
        /// 请求的资源在服务器上不存在。
        /// </summary>
        NotFound = 404,
        /// <summary>
        /// 等价于HTTP状态405。System.Net.HttpStatusCode.MethodNotAllowed表示
        /// 请求的资源不允许使用请求方法(POST或GET)。
        /// </summary>
        MethodNotAllowed = 405,
        /// <summary>
        /// 等价于HTTP状态406。System.Net.HttpStatusCode.NotAcceptable表示
        /// 客户端已经用Accept标头表示它不会接受任何标头
        /// 资源的可用表示。
        /// </summary>
        NotAcceptable = 406,
        /// <summary>
        /// 等价于HTTP状态407。System.Net.HttpStatusCode.ProxyAuthenticationRequired
        /// 表示请求的代理需要验证。的Proxy-authenticate
        /// Header包含如何执行身份验证的详细信息。
        /// </summary>
        ProxyAuthenticationRequired = 407,
        /// <summary>
        /// 等价于HTTP状态408。System.Net.HttpStatusCode.RequestTimeout表示
        /// 客户端没有在服务器期望的时间内发送请求
        /// 请求。
        /// </summary>
        RequestTimeout = 408,
        /// <summary>
        /// 等价于HTTP状态409。System.Net.HttpStatusCode.Conflict表示
        /// 由于服务器上的冲突，请求无法执行。
        /// </summary>
        Conflict = 409,
        /// <summary>
        /// 等同于HTTP状态410。System.Net.HttpStatusCode.Gone表示
        /// 请求的资源不再可用。
        /// </summary>
        Gone = 410,
        /// <summary>
        /// 等价于HTTP状态411。System.Net.HttpStatusCode.LengthRequired表示
        /// 缺少所需的Content-length首部。
        /// </summary>
        LengthRequired = 411,
        /// <summary>
        /// 等价于HTTP状态412。System.Net.HttpStatusCode.PreconditionFailed表示
        /// 为此请求设置的条件失败，请求无法进行
        /// 出去了。条件是用条件请求头设置的，如If-Match, If-None-Match，
        /// 或If-Unmodified-Since。
        /// </summary>
        PreconditionFailed = 412,
        /// <summary>
        /// 等价于HTTP状态413System.Net.HttpStatusCode.RequestEntityTooLarge
        /// 表示请求太大，服务器无法处理。
        /// </summary>
        RequestEntityTooLarge = 413,
        /// <summary>
        /// 等同于HTTP状态414。System.Net.HttpStatusCode.RequestUriTooLong表示
        /// URI太长。
        /// </summary>
        RequestUriTooLong = 414,
        /// <summary>
        /// 等价于HTTP状态415System.Net.HttpStatusCode.UnsupportedMediaType
        /// 表示请求是不支持的类型
        /// </summary>
        UnsupportedMediaType = 415,
        /// <summary>
        /// 等价于HTTP状态416System.Net.HttpStatusCode.RequestedRangeNotSatisfiable
        /// 表示无法返回请求资源的数据范围，
        /// 要么是因为范围的起始位置在资源的起始位置之前，
        /// 或者范围的终点在资源的终点之后
        /// </summary>
        RequestedRangeNotSatisfiable = 416,
        /// <summary>
        /// 等价于HTTP状态417System.Net.HttpStatusCode.ExpectationFailed表示
        /// 服务器无法满足Expect头信息中的期望
        /// </summary>
        ExpectationFailed = 417,
        /// <summary>
        /// 等价于HTTP状态421System.Net.HttpStatusCode.MisdirectedRequest表示
        /// 请求指向的服务器无法产生响应
        /// </summary>
        MisdirectedRequest = 421,
        /// <summary>
        /// 等价于HTTP状态422System.Net.HttpStatusCode.UnprocessableEntity
        /// 表示请求格式良好，但无法执行
        /// 指向语义错误。
        /// </summary>
        UnprocessableEntity = 422,
        /// <summary>
        /// 等价于HTTP状态423System.Net.HttpStatusCode.Locked表示
        /// 源或目的资源被锁定。
        /// </summary>
        Locked = 423,
        /// <summary>
        /// 等同于HTTP状态424。System.Net.HttpStatusCode.FailedDependency表示
        /// 由于请求的操作，该方法无法在资源上执行
        /// 依赖于另一个操作，但该操作失败。
        /// </summary>
        FailedDependency = 424,
        /// <summary>
        /// 等价于HTTP状态426System.Net.HttpStatusCode.UpgradeRequired表示
        /// 客户端应该切换到其他协议，例如TLS/1.0。
        /// </summary>
        UpgradeRequired = 426,
        /// <summary>
        /// 等价于HTTP状态428。System.Net.HttpStatusCode.PreconditionRequired
        /// 表示，服务器要求该请求为条件请求。
        /// </summary>
        PreconditionRequired = 428,
        /// <summary>
        /// 等价于HTTP状态429。System.Net.HttpStatusCode.TooManyRequests表示
        /// 用户在给定时间内发送了太多请求。
        /// </summary>
        TooManyRequests = 429,
        /// <summary>
        /// 等价于HTTP状态431。System.Net.HttpStatusCode.RequestHeaderFieldsTooLarge
        /// 表示服务器不愿意处理请求，因为它有首部
        /// 字段(单个字段或所有字段的集合)
        /// 都太大了
        /// </summary>
        RequestHeaderFieldsTooLarge = 431,
        /// <summary>
        /// 等价于HTTP状态451。System.Net.HttpStatusCode.UnavailableForLegalReasons
        /// 表示服务器拒绝访问该资源
        /// 合法的请求
        /// </summary>
        UnavailableForLegalReasons = 451,
        /// <summary>
        /// 等价于HTTP状态500。System.Net.HttpStatusCode.InternalServerError
        /// 表示服务器上发生了一般性错误。
        /// </summary>
        InternalServerError = 500,
        /// <summary>
        /// 等同于HTTP状态501。System.Net.HttpStatusCode.NotImplemented表示
        /// 服务器不支持请求的函数
        /// </summary>
        NotImplemented = 501,
        /// <summary>
        /// 等价于HTTP状态502。System.Net.HttpStatusCode.BadGateway表示
        /// 中间代理服务器从另一个代理收到了错误的响应
        /// 或者源服务器。
        /// </summary>
        BadGateway = 502,
        /// <summary>
        /// 等价于HTTP状态503。System.Net.HttpStatusCode.ServiceUnavailable表示
        /// 服务器暂时不可用，通常是由于高负载或维护。
        /// </summary>
        ServiceUnavailable = 503,
        /// <summary>
        /// 等价于HTTP状态504。System.Net.HttpStatusCode.GatewayTimeout表示
        /// 中间代理服务器在等待响应时超时
        /// 另一个代理服务器或原始服务器。
        /// </summary>
        GatewayTimeout = 504,
        /// <summary>
        /// 等价于HTTP状态505。System.Net.HttpStatusCode.HttpVersionNotSupported
        /// 表示服务器不支持请求的HTTP版本。
        /// </summary>
        HttpVersionNotSupported = 505,
        /// <summary>
        /// 等价于HTTP状态506。System.Net.HttpStatusCode.VariantAlsoNegotiates
        /// 指示将选定的变体资源配置为启用透明
        /// 因此，内容协商本身并不是协商的适当终点
        /// 的过程。
        /// </summary>
        VariantAlsoNegotiates = 506,
        /// <summary>
        /// 等价于HTTP状态507。System.Net.HttpStatusCode.InsufficientStorage
        /// 指示服务器无法存储完成所需的表示
        /// 请求。
        /// </summary>
        InsufficientStorage = 507,
        /// <summary>
        /// 等价于HTTP状态508。System.Net.HttpStatusCode.LoopDetected表示
        /// 服务器因为遇到无限循环而终止操作
        /// 处理一个“Depth: infinity”的WebDAV请求。此状态码为:
        /// 旨在向后兼容不知道208状态码的客户端
        /// 已经报告在多状态响应体中出现。
        /// </summary>
        LoopDetected = 508,
        /// <summary>
        /// 相当于HTTP状态510。System.Net.HttpStatusCode.NotExtended表示
        /// 请求的进一步扩展需要服务器来完成
        /// 它。
        /// </summary>
        NotExtended = 510,
        /// <summary>
        /// 等价于HTTP状态511。System.Net.HttpStatusCode.NetworkAuthenticationRequired
        /// 表示客户端需要认证才能接入网络;这是
        /// 用于拦截用于控制访问网络的代理。
        /// </summary>
        NetworkAuthenticationRequired = 511
    }
}
