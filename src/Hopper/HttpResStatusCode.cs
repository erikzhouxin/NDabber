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
        //
        // 摘要:
        //     Equivalent to HTTP status 300. System.Net.HttpStatusCode.Ambiguous indicates
        //     that the requested information has multiple representations. The default action
        //     is to treat this status as a redirect and follow the contents of the Location
        //     header associated with this response. Ambiguous is a synonym for MultipleChoices.
        Ambiguous = 300,
        //
        // 摘要:
        //     Equivalent to HTTP status 300. System.Net.HttpStatusCode.MultipleChoices indicates
        //     that the requested information has multiple representations. The default action
        //     is to treat this status as a redirect and follow the contents of the Location
        //     header associated with this response. MultipleChoices is a synonym for Ambiguous.
        MultipleChoices = 300,
        //
        // 摘要:
        //     Equivalent to HTTP status 301. System.Net.HttpStatusCode.Moved indicates that
        //     the requested information has been moved to the URI specified in the Location
        //     header. The default action when this status is received is to follow the Location
        //     header associated with the response. When the original request method was POST,
        //     the redirected request will use the GET method. Moved is a synonym for MovedPermanently.
        Moved = 301,
        //
        // 摘要:
        //     Equivalent to HTTP status 301. System.Net.HttpStatusCode.MovedPermanently indicates
        //     that the requested information has been moved to the URI specified in the Location
        //     header. The default action when this status is received is to follow the Location
        //     header associated with the response. MovedPermanently is a synonym for Moved.
        MovedPermanently = 301,
        //
        // 摘要:
        //     Equivalent to HTTP status 302. System.Net.HttpStatusCode.Found indicates that
        //     the requested information is located at the URI specified in the Location header.
        //     The default action when this status is received is to follow the Location header
        //     associated with the response. When the original request method was POST, the
        //     redirected request will use the GET method. Found is a synonym for Redirect.
        Found = 302,
        //
        // 摘要:
        //     Equivalent to HTTP status 302. System.Net.HttpStatusCode.Redirect indicates that
        //     the requested information is located at the URI specified in the Location header.
        //     The default action when this status is received is to follow the Location header
        //     associated with the response. When the original request method was POST, the
        //     redirected request will use the GET method. Redirect is a synonym for Found.
        Redirect = 302,
        //
        // 摘要:
        //     Equivalent to HTTP status 303. System.Net.HttpStatusCode.RedirectMethod automatically
        //     redirects the client to the URI specified in the Location header as the result
        //     of a POST. The request to the resource specified by the Location header will
        //     be made with a GET. RedirectMethod is a synonym for SeeOther.
        RedirectMethod = 303,
        //
        // 摘要:
        //     Equivalent to HTTP status 303. System.Net.HttpStatusCode.SeeOther automatically
        //     redirects the client to the URI specified in the Location header as the result
        //     of a POST. The request to the resource specified by the Location header will
        //     be made with a GET. SeeOther is a synonym for RedirectMethod
        SeeOther = 303,
        //
        // 摘要:
        //     Equivalent to HTTP status 304. System.Net.HttpStatusCode.NotModified indicates
        //     that the client's cached copy is up to date. The contents of the resource are
        //     not transferred.
        NotModified = 304,
        //
        // 摘要:
        //     Equivalent to HTTP status 305. System.Net.HttpStatusCode.UseProxy indicates that
        //     the request should use the proxy server at the URI specified in the Location
        //     header.
        UseProxy = 305,
        //
        // 摘要:
        //     Equivalent to HTTP status 306. System.Net.HttpStatusCode.Unused is a proposed
        //     extension to the HTTP/1.1 specification that is not fully specified.
        Unused = 306,
        //
        // 摘要:
        //     Equivalent to HTTP status 307. System.Net.HttpStatusCode.RedirectKeepVerb indicates
        //     that the request information is located at the URI specified in the Location
        //     header. The default action when this status is received is to follow the Location
        //     header associated with the response. When the original request method was POST,
        //     the redirected request will also use the POST method. RedirectKeepVerb is a synonym
        //     for TemporaryRedirect.
        RedirectKeepVerb = 307,
        //
        // 摘要:
        //     Equivalent to HTTP status 307. System.Net.HttpStatusCode.TemporaryRedirect indicates
        //     that the request information is located at the URI specified in the Location
        //     header. The default action when this status is received is to follow the Location
        //     header associated with the response. When the original request method was POST,
        //     the redirected request will also use the POST method. TemporaryRedirect is a
        //     synonym for RedirectKeepVerb.
        TemporaryRedirect = 307,
        //
        // 摘要:
        //     Equivalent to HTTP status 308. System.Net.HttpStatusCode.PermanentRedirect indicates
        //     that the request information is located at the URI specified in the Location
        //     header. The default action when this status is received is to follow the Location
        //     header associated with the response. When the original request method was POST,
        //     the redirected request will also use the POST method.
        PermanentRedirect = 308,
        //
        // 摘要:
        //     Equivalent to HTTP status 400. System.Net.HttpStatusCode.BadRequest indicates
        //     that the request could not be understood by the server. System.Net.HttpStatusCode.BadRequest
        //     is sent when no other error is applicable, or if the exact error is unknown or
        //     does not have its own error code.
        BadRequest = 400,
        //
        // 摘要:
        //     Equivalent to HTTP status 401. System.Net.HttpStatusCode.Unauthorized indicates
        //     that the requested resource requires authentication. The WWW-Authenticate header
        //     contains the details of how to perform the authentication.
        Unauthorized = 401,
        //
        // 摘要:
        //     Equivalent to HTTP status 402. System.Net.HttpStatusCode.PaymentRequired is reserved
        //     for future use.
        PaymentRequired = 402,
        //
        // 摘要:
        //     Equivalent to HTTP status 403. System.Net.HttpStatusCode.Forbidden indicates
        //     that the server refuses to fulfill the request.
        Forbidden = 403,
        //
        // 摘要:
        //     Equivalent to HTTP status 404. System.Net.HttpStatusCode.NotFound indicates that
        //     the requested resource does not exist on the server.
        NotFound = 404,
        //
        // 摘要:
        //     Equivalent to HTTP status 405. System.Net.HttpStatusCode.MethodNotAllowed indicates
        //     that the request method (POST or GET) is not allowed on the requested resource.
        MethodNotAllowed = 405,
        //
        // 摘要:
        //     Equivalent to HTTP status 406. System.Net.HttpStatusCode.NotAcceptable indicates
        //     that the client has indicated with Accept headers that it will not accept any
        //     of the available representations of the resource.
        NotAcceptable = 406,
        //
        // 摘要:
        //     Equivalent to HTTP status 407. System.Net.HttpStatusCode.ProxyAuthenticationRequired
        //     indicates that the requested proxy requires authentication. The Proxy-authenticate
        //     header contains the details of how to perform the authentication.
        ProxyAuthenticationRequired = 407,
        //
        // 摘要:
        //     Equivalent to HTTP status 408. System.Net.HttpStatusCode.RequestTimeout indicates
        //     that the client did not send a request within the time the server was expecting
        //     the request.
        RequestTimeout = 408,
        //
        // 摘要:
        //     Equivalent to HTTP status 409. System.Net.HttpStatusCode.Conflict indicates that
        //     the request could not be carried out because of a conflict on the server.
        Conflict = 409,
        //
        // 摘要:
        //     Equivalent to HTTP status 410. System.Net.HttpStatusCode.Gone indicates that
        //     the requested resource is no longer available.
        Gone = 410,
        //
        // 摘要:
        //     Equivalent to HTTP status 411. System.Net.HttpStatusCode.LengthRequired indicates
        //     that the required Content-length header is missing.
        LengthRequired = 411,
        //
        // 摘要:
        //     Equivalent to HTTP status 412. System.Net.HttpStatusCode.PreconditionFailed indicates
        //     that a condition set for this request failed, and the request cannot be carried
        //     out. Conditions are set with conditional request headers like If-Match, If-None-Match,
        //     or If-Unmodified-Since.
        PreconditionFailed = 412,
        //
        // 摘要:
        //     Equivalent to HTTP status 413. System.Net.HttpStatusCode.RequestEntityTooLarge
        //     indicates that the request is too large for the server to process.
        RequestEntityTooLarge = 413,
        //
        // 摘要:
        //     Equivalent to HTTP status 414. System.Net.HttpStatusCode.RequestUriTooLong indicates
        //     that the URI is too long.
        RequestUriTooLong = 414,
        //
        // 摘要:
        //     Equivalent to HTTP status 415. System.Net.HttpStatusCode.UnsupportedMediaType
        //     indicates that the request is an unsupported type.
        UnsupportedMediaType = 415,
        //
        // 摘要:
        //     Equivalent to HTTP status 416. System.Net.HttpStatusCode.RequestedRangeNotSatisfiable
        //     indicates that the range of data requested from the resource cannot be returned,
        //     either because the beginning of the range is before the beginning of the resource,
        //     or the end of the range is after the end of the resource.
        RequestedRangeNotSatisfiable = 416,
        //
        // 摘要:
        //     Equivalent to HTTP status 417. System.Net.HttpStatusCode.ExpectationFailed indicates
        //     that an expectation given in an Expect header could not be met by the server.
        ExpectationFailed = 417,
        //
        // 摘要:
        //     Equivalent to HTTP status 421. System.Net.HttpStatusCode.MisdirectedRequest indicates
        //     that the request was directed at a server that is not able to produce a response.
        MisdirectedRequest = 421,
        //
        // 摘要:
        //     Equivalent to HTTP status 422. System.Net.HttpStatusCode.UnprocessableEntity
        //     indicates that the request was well-formed but was unable to be followed due
        //     to semantic errors.
        UnprocessableEntity = 422,
        //
        // 摘要:
        //     Equivalent to HTTP status 423. System.Net.HttpStatusCode.Locked indicates that
        //     the source or destination resource is locked.
        Locked = 423,
        //
        // 摘要:
        //     Equivalent to HTTP status 424. System.Net.HttpStatusCode.FailedDependency indicates
        //     that the method couldn't be performed on the resource because the requested action
        //     depended on another action and that action failed.
        FailedDependency = 424,
        //
        // 摘要:
        //     Equivalent to HTTP status 426. System.Net.HttpStatusCode.UpgradeRequired indicates
        //     that the client should switch to a different protocol such as TLS/1.0.
        UpgradeRequired = 426,
        //
        // 摘要:
        //     Equivalent to HTTP status 428. System.Net.HttpStatusCode.PreconditionRequired
        //     indicates that the server requires the request to be conditional.
        PreconditionRequired = 428,
        //
        // 摘要:
        //     Equivalent to HTTP status 429. System.Net.HttpStatusCode.TooManyRequests indicates
        //     that the user has sent too many requests in a given amount of time.
        TooManyRequests = 429,
        //
        // 摘要:
        //     Equivalent to HTTP status 431. System.Net.HttpStatusCode.RequestHeaderFieldsTooLarge
        //     indicates that the server is unwilling to process the request because its header
        //     fields (either an individual header field or all the header fields collectively)
        //     are too large.
        RequestHeaderFieldsTooLarge = 431,
        //
        // 摘要:
        //     Equivalent to HTTP status 451. System.Net.HttpStatusCode.UnavailableForLegalReasons
        //     indicates that the server is denying access to the resource as a consequence
        //     of a legal demand.
        UnavailableForLegalReasons = 451,
        //
        // 摘要:
        //     Equivalent to HTTP status 500. System.Net.HttpStatusCode.InternalServerError
        //     indicates that a generic error has occurred on the server.
        InternalServerError = 500,
        //
        // 摘要:
        //     Equivalent to HTTP status 501. System.Net.HttpStatusCode.NotImplemented indicates
        //     that the server does not support the requested function.
        NotImplemented = 501,
        //
        // 摘要:
        //     Equivalent to HTTP status 502. System.Net.HttpStatusCode.BadGateway indicates
        //     that an intermediate proxy server received a bad response from another proxy
        //     or the origin server.
        BadGateway = 502,
        //
        // 摘要:
        //     Equivalent to HTTP status 503. System.Net.HttpStatusCode.ServiceUnavailable indicates
        //     that the server is temporarily unavailable, usually due to high load or maintenance.
        ServiceUnavailable = 503,
        //
        // 摘要:
        //     Equivalent to HTTP status 504. System.Net.HttpStatusCode.GatewayTimeout indicates
        //     that an intermediate proxy server timed out while waiting for a response from
        //     another proxy or the origin server.
        GatewayTimeout = 504,
        //
        // 摘要:
        //     Equivalent to HTTP status 505. System.Net.HttpStatusCode.HttpVersionNotSupported
        //     indicates that the requested HTTP version is not supported by the server.
        HttpVersionNotSupported = 505,
        //
        // 摘要:
        //     Equivalent to HTTP status 506. System.Net.HttpStatusCode.VariantAlsoNegotiates
        //     indicates that the chosen variant resource is configured to engage in transparent
        //     content negotiation itself and, therefore, isn't a proper endpoint in the negotiation
        //     process.
        VariantAlsoNegotiates = 506,
        //
        // 摘要:
        //     Equivalent to HTTP status 507. System.Net.HttpStatusCode.InsufficientStorage
        //     indicates that the server is unable to store the representation needed to complete
        //     the request.
        InsufficientStorage = 507,
        //
        // 摘要:
        //     Equivalent to HTTP status 508. System.Net.HttpStatusCode.LoopDetected indicates
        //     that the server terminated an operation because it encountered an infinite loop
        //     while processing a WebDAV request with "Depth: infinity". This status code is
        //     meant for backward compatibility with clients not aware of the 208 status code
        //     System.Net.HttpStatusCode.AlreadyReported appearing in multistatus response bodies.
        LoopDetected = 508,
        //
        // 摘要:
        //     Equivalent to HTTP status 510. System.Net.HttpStatusCode.NotExtended indicates
        //     that further extensions to the request are required for the server to fulfill
        //     it.
        NotExtended = 510,
        //
        // 摘要:
        //     Equivalent to HTTP status 511. System.Net.HttpStatusCode.NetworkAuthenticationRequired
        //     indicates that the client needs to authenticate to gain network access; it's
        //     intended for use by intercepting proxies used to control access to the network.
        NetworkAuthenticationRequired = 511
    }
}
