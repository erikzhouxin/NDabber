using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace System.Data.Extter
{
    public static partial class ExtterCaller
    {
        /// <summary>
        /// 创建一个Web请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
#if NET50 || NET60
        [Obsolete("WebRequest、HttpWebRequest、ServicePoint 与 WebClient 已过时，请改用 HttpClient", DiagnosticId = "SYSLIB0014", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
        public static T CreateWebRequest<T>(string url) where T : WebRequest
        {
            // 官方极不负责任的说对于 Ftp，由于 HttpClient 不支持它，因此建议使用第三方库。
            // https://docs.microsoft.com/zh-cn/dotnet/core/compatibility/networking/6.0/webrequest-deprecated
            return (T)WebRequest.Create(url);
        }
    }
}
