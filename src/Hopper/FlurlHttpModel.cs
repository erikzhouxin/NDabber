using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Collections.Concurrent;
using System.Collections;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Runtime.CompilerServices;
using Flurl;
using System.Text.RegularExpressions;
using System.Data.Cobber;

namespace System.Data.Hopper
{
    /// <summary>
    /// Provides HTTP content based on a serialized JSON object, with the JSON string captured to a property
    /// so it can be read without affecting the read-once content stream.
    /// </summary>
    public class CapturedJsonContent : CapturedStringContent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CapturedJsonContent"/> class.
        /// </summary>
        /// <param name="json">The json.</param>
        public CapturedJsonContent(string json) : base(json, "application/json; charset=UTF-8") { }
    }
    /// <summary>
    /// Provides HTTP content for a multipart/form-data request.
    /// </summary>
    public class CapturedMultipartContent : MultipartContent
    {
        private readonly FlurlHttpSettings _settings;
        private readonly List<HttpContent> _capturedParts = new List<HttpContent>();

        /// <summary>
        /// Gets an array of HttpContent objects that make up the parts of the multipart request.
        /// </summary>
#if NET40
        public IReadOnlyList<HttpContent> Parts => new EReadOnlyCollection<HttpContent>(_capturedParts);
#else
		public IReadOnlyList<HttpContent> Parts => _capturedParts;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="CapturedMultipartContent"/> class.
        /// </summary>
        /// <param name="settings">The FlurlHttpSettings used to serialize each content part. (Defaults to FlurlHttp.GlobalSettings.)</param>
        public CapturedMultipartContent(FlurlHttpSettings settings = null) : base("form-data")
        {
            _settings = settings ?? FlurlHttp.GlobalSettings;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CapturedMultipartContent"/> class.
        /// </summary>
        /// <param name="subtype">The subtype of the multipart content.</param>
        /// <param name="settings">The FlurlHttpSettings used to serialize each content part. (Defaults to FlurlHttp.GlobalSettings.)</param>
        public CapturedMultipartContent(string subtype, FlurlHttpSettings settings = null) : base(subtype)
        {
            _settings = settings ?? FlurlHttp.GlobalSettings;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CapturedMultipartContent"/> class.
        /// </summary>
        /// <param name="subtype">The subtype of the multipart content.</param>
        /// <param name="boundary">The boundary string for the multipart content.</param>
        /// <param name="settings">The FlurlHttpSettings used to serialize each content part. (Defaults to FlurlHttp.GlobalSettings.)</param>
        public CapturedMultipartContent(string subtype, string boundary, FlurlHttpSettings settings = null) : base(subtype, boundary)
        {
            _settings = settings ?? FlurlHttp.GlobalSettings;
        }

        /// <summary>
        /// Add a content part to the multipart request.
        /// </summary>
        /// <param name="name">The control name of the part.</param>
        /// <param name="content">The HttpContent of the part.</param>
        /// <returns>This CapturedMultipartContent instance (supports method chaining).</returns>
        public CapturedMultipartContent Add(string name, HttpContent content) => AddInternal(name, content, null);

        /// <summary>
        /// Add a simple string part to the multipart request.
        /// </summary>
        /// <param name="name">The name of the part.</param>
        /// <param name="value">The string value of the part.</param>
        /// <param name="contentType">The value of the Content-Type header for this part. If null (the default), header will be excluded, which complies with the HTML 5 standard.</param>
        /// <returns>This CapturedMultipartContent instance (supports method chaining).</returns>
        public CapturedMultipartContent AddString(string name, string value, string contentType = null) =>
            AddInternal(name, new CapturedStringContent(value, contentType), null);

        /// <summary>
        /// Add multiple string parts to the multipart request by parsing an object's properties into control name/content pairs.
        /// </summary>
        /// <param name="data">The object (typically anonymous) whose properties are parsed into control name/content pairs.</param>
        /// <param name="contentType">The value of the Content-Type header for this part. If null, header will be excluded, which complies with the HTML 5 standard.</param>
        /// <returns>This CapturedMultipartContent instance (supports method chaining).</returns>
        public CapturedMultipartContent AddStringParts(object data, string contentType = null)
        {
            foreach (var kv in data.ToKeyValuePairs())
            {
                if (kv.Value == null)
                    continue;
                AddString(kv.Key, kv.Value.ToInvariantString(), contentType);
            }
            return this;
        }

        /// <summary>
        /// Add a JSON-serialized part to the multipart request.
        /// </summary>
        /// <param name="name">The control name of the part.</param>
        /// <param name="data">The content of the part, which will be serialized to JSON.</param>
        /// <returns>This CapturedMultipartContent instance (supports method chaining).</returns>
        public CapturedMultipartContent AddJson(string name, object data) =>
            AddInternal(name, new CapturedJsonContent(_settings.JsonSerializer.Serialize(data)), null);

        /// <summary>
        /// Add a URL-encoded part to the multipart request.
        /// </summary>
        /// <param name="name">The control name of the part.</param>
        /// <param name="data">The content of the part, whose properties will be parsed and serialized to URL-encoded format.</param>
        /// <returns>This CapturedMultipartContent instance (supports method chaining).</returns>
        public CapturedMultipartContent AddUrlEncoded(string name, object data) =>
            AddInternal(name, new CapturedUrlEncodedContent(_settings.UrlEncodedSerializer.Serialize(data)), null);

        /// <summary>
        /// Adds a file to the multipart request from a stream.
        /// </summary>
        /// <param name="name">The control name of the part.</param>
        /// <param name="stream">The file stream to send.</param>
        /// <param name="fileName">The filename, added to the Content-Disposition header of the part.</param>
        /// <param name="contentType">The content type of the file.</param>
        /// <param name="bufferSize">The buffer size of the stream upload in bytes. Defaults to 4096.</param>
        /// <returns>This CapturedMultipartContent instance (supports method chaining).</returns>
        public CapturedMultipartContent AddFile(string name, Stream stream, string fileName, string contentType = null, int bufferSize = 4096)
        {
            var content = new StreamContent(stream, bufferSize);
            if (contentType != null)
                content.Headers.TryAddWithoutValidation("Content-Type", contentType);
            return AddInternal(name, content, fileName);
        }

        /// <summary>
        /// Adds a file to the multipart request from a local path.
        /// </summary>
        /// <param name="name">The control name of the part.</param>
        /// <param name="path">The local path to the file.</param>
        /// <param name="contentType">The content type of the file.</param>
        /// <param name="bufferSize">The buffer size of the stream upload in bytes. Defaults to 4096.</param>
        /// <param name="fileName">The filename, added to the Content-Disposition header of the part. Defaults to local file name.</param>
        /// <returns>This CapturedMultipartContent instance (supports method chaining).</returns>
        public CapturedMultipartContent AddFile(string name, string path, string contentType = null, int bufferSize = 4096, string fileName = null)
        {
            fileName = fileName ?? FileUtil.GetFileName(path);
            var content = new FileContent(path, bufferSize);
            if (contentType != null)
                content.Headers.TryAddWithoutValidation("Content-Type", contentType);
            return AddInternal(name, content, fileName);
        }

        private CapturedMultipartContent AddInternal(string name, HttpContent content, string fileName)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name must not be empty", nameof(name));

            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = name,
                FileName = fileName,
                FileNameStar = fileName
            };
            // The base class's collection of parts is cleared on Dispose, which isn't exactly in the spirit of "Captured",
            // which is why we need to add it to this other collection. (#580)
            _capturedParts.Add(content);
            base.Add(content);
            return this;
        }
    }
    /// <summary>
    /// Provides HTTP content based on a string, with the string itself captured to a property
    /// so it can be read without affecting the read-once content stream.
    /// </summary>
    public class CapturedStringContent : StringContent
    {
        /// <summary>
        /// The content body captured as a string. Can be read multiple times (unlike the content stream).
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="CapturedStringContent"/> with a Content-Type header of text/plain; charset=UTF-8
        /// </summary>
        /// <param name="content">The content.</param>
        public CapturedStringContent(string content) : base(content ?? "")
        {
            Content = content ?? "";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CapturedStringContent"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="contentType">Value of the Content-Type header. To exclude the header, set to null explicitly.</param>
        public CapturedStringContent(string content, string contentType) : base(content)
        {
            Content = content;
            Headers.Remove("Content-Type");
            if (contentType != null)
                Headers.TryAddWithoutValidation("Content-Type", contentType);
        }
    }

    /// <summary>
    /// Provides HTTP content based on an object serialized to URL-encoded name-value pairs.
    /// Useful in simulating an HTML form POST. Serialized content is captured to Content property
    /// so it can be read without affecting the read-once content stream.
    /// </summary>
    public class CapturedUrlEncodedContent : CapturedStringContent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CapturedUrlEncodedContent"/> class.
        /// </summary>
        /// <param name="data">Content represented as a (typically anonymous) object, which will be parsed into name/value pairs.</param>
        public CapturedUrlEncodedContent(string data) : base(data, "application/x-www-form-urlencoded") { }
    }
    /// <summary>
    /// Utility and extension methods for parsing and validating cookies.
    /// </summary>
    public static class CookieCutter
    {
        /// <summary>
        /// Parses a Cookie request header to name-value pairs.
        /// </summary>
        /// <param name="headerValue">Value of the Cookie request header.</param>
        /// <returns></returns>
        public static IEnumerable<(string Name, string Value)> ParseRequestHeader(string headerValue)
        {
            if (string.IsNullOrEmpty(headerValue)) yield break;

            foreach (var pair in GetPairs(headerValue))
                yield return (pair.Name, pair.Value);
        }

        /// <summary>
        /// Parses a Set-Cookie response header to a FlurlCookie.
        /// </summary>
        /// <param name="url">The URL that sent the response.</param>
        /// <param name="headerValue">Value of the Set-Cookie header.</param>
        /// <returns></returns>
        public static FlurlCookie ParseResponseHeader(string url, string headerValue)
        {
            if (string.IsNullOrEmpty(headerValue)) return null;

            FlurlCookie cookie = null;
            foreach (var pair in GetPairs(headerValue))
            {
                if (cookie == null)
                    cookie = new FlurlCookie(pair.Name, pair.Value.Trim('"'), url, DateTimeOffset.UtcNow);

                // ordinal string compare is both safest and fastest
                // https://docs.microsoft.com/en-us/dotnet/standard/base-types/best-practices-strings#recommendations-for-string-usage
                else if (pair.Name.OrdinalEquals("Expires", true))
                    cookie.Expires = DateTimeOffset.TryParse(pair.Value, out var d) ? d : (DateTimeOffset?)null;
                else if (pair.Name.OrdinalEquals("Max-Age", true))
                    cookie.MaxAge = int.TryParse(pair.Value, out var i) ? i : (int?)null;
                else if (pair.Name.OrdinalEquals("Domain", true))
                    cookie.Domain = pair.Value;
                else if (pair.Name.OrdinalEquals("Path", true))
                    cookie.Path = pair.Value;
                else if (pair.Name.OrdinalEquals("HttpOnly", true))
                    cookie.HttpOnly = true;
                else if (pair.Name.OrdinalEquals("Secure", true))
                    cookie.Secure = true;
                else if (pair.Name.OrdinalEquals("SameSite", true))
                    cookie.SameSite = Enum.TryParse<SameSite>(pair.Value, true, out var val) ? val : (SameSite?)null;
            }
            return cookie;
        }

        /// <summary>
        /// Parses list of semicolon-delimited "name=value" pairs.
        /// </summary>
        private static IEnumerable<(string Name, string Value)> GetPairs(string list) =>
            from part in list.Split(';')
            let pair = part.SplitOnFirstOccurence("=")
            select (pair[0].Trim(), pair.Last().Trim());

        /// <summary>
        /// Creates a Cookie request header value from a list of cookie name-value pairs.
        /// </summary>
        /// <returns>A header value if cookie values are present, otherwise null.</returns>
        public static string ToRequestHeader(IEnumerable<(string Name, string Value)> cookies)
        {
            if (cookies?.Any() != true) return null;

            return string.Join("; ", cookies.Select(c =>
                $"{c.Name}={c.Value}"));
        }

        /// <summary>
        /// True if this cookie passes well-accepted rules for the Set-Cookie header. If false, provides a descriptive reason.
        /// </summary>
        public static bool IsValid(this FlurlCookie cookie, out string reason)
        {
            // TODO: validate name and value? https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Set-Cookie

            if (cookie.OriginUrl == null)
            {
                reason = "OriginUrl (URL that returned the original Set-Cookie header) is required in order to validate this cookie.";
                return false;
            }
            if (!FlurlModel.IsValid(cookie.OriginUrl))
            {
                reason = $"OriginUrl {cookie.OriginUrl} is not a valid absolute URL.";
                return false;
            }
            if (cookie.Secure && !cookie.OriginUrl.IsSecureScheme)
            {
                reason = $"Secure cannot be true unless OriginUrl ({cookie.OriginUrl}) has a secure scheme (https).";
                return false;
            }

            if (!string.IsNullOrEmpty(cookie.Domain))
            {
                if (cookie.Domain.IsIP())
                {
                    reason = "Domain cannot be an IP address.";
                    return false;
                }
                if (cookie.OriginUrl.Host.IsIP())
                {
                    reason = "Domain cannot be set when origin URL is an IP address.";
                    return false;
                }
                if (!cookie.Domain.Trim('.').OrdinalContains("."))
                {
                    reason = $"{cookie.Domain} is not a valid value for Domain.";
                    return false;
                }
                var host = cookie.Domain.OrdinalStartsWith(".") ? cookie.Domain.Substring(1) : cookie.Domain;
                var fakeUrl = new FlurlModel("https://" + host);
                if (fakeUrl.IsRelative || fakeUrl.Host != host)
                {
                    reason = $"{cookie.Domain} is not a valid Domain. A non-empty Domain must be a valid URI host (no scheme, path, port, etc).";
                    return false;
                }
                if (!cookie.IsDomainMatch(cookie.OriginUrl, out reason))
                {
                    return false;
                }
            }

            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Cookies#Cookie_prefixes

            if (cookie.Name.OrdinalStartsWith("__Host-"))
            {
                if (!cookie.OriginUrl.IsSecureScheme)
                {
                    reason = "Cookie named with __Host- prefix must originate from a secure (https) domain.";
                    return false;
                }
                if (!cookie.Secure)
                {
                    reason = "Cookie named with __Host- prefix must contain the Secure attribute.";
                    return false;
                }
                if (!string.IsNullOrEmpty(cookie.Domain))
                {
                    reason = "Cookie named with __Host- prefix must not contain the Domain attribute.";
                    return false;
                }
                if (cookie.Path != "/")
                {
                    reason = "Cookie named with __Host- prefix must contain the Path attribute with a value of '/'.";
                    return false;
                }
            }
            if (cookie.Name.OrdinalStartsWith("__Secure-"))
            {
                if (!cookie.OriginUrl.IsSecureScheme)
                {
                    reason = "Cookie named with __Secure- prefix must originate from a secure (https) domain.";
                    return false;
                }
                if (!cookie.Secure)
                {
                    reason = "Cookie named with __Secure- prefix must contain the Secure attribute.";
                    return false;
                }
            }

            // it seems intuitive tht a non-empty path should start with /, but I can't find this in any spec
            //if (!string.IsNullOrEmpty(Path) && !Path.OrdinalStartsWith("/")) {
            //	reason = $"{Path} is not a valid Path. A non-empty Path must start with a / character.";
            //	return false;
            //}

            reason = "ok";
            return true;
        }

        /// <summary>
        /// True if this cookie is expired. If true, provides a descriptive reason (Expires or Max-Age).
        /// </summary>
        public static bool IsExpired(this FlurlCookie cookie, out string reason)
        {
            // Max-Age takes precedence over Expires
            if (cookie.MaxAge.HasValue)
            {
                if (cookie.MaxAge.Value <= 0 || cookie.DateReceived.AddSeconds(cookie.MaxAge.Value) < DateTimeOffset.UtcNow)
                {
                    reason = $"Cookie's Max-Age={cookie.MaxAge} (seconds) has expired.";
                    return true;
                }
            }
            else if (cookie.Expires.HasValue && cookie.Expires < DateTimeOffset.UtcNow)
            {
                reason = $"Cookie with Expires={cookie.Expires} has expired.";
                return true;
            }
            reason = "ok";
            return false;
        }

        /// <summary>
        /// True if this cookie should be sent in a request to the given URL. If false, provides a descriptive reason.
        /// </summary>
        public static bool ShouldSendTo(this FlurlCookie cookie, FlurlModel requestUrl, out string reason)
        {
            if (cookie.Secure && !requestUrl.IsSecureScheme)
            {
                reason = $"Cookie is marked Secure and request URL is insecure ({requestUrl.Scheme}).";
                return false;
            }

            return
                cookie.IsValid(out reason) &&
                !cookie.IsExpired(out reason) &&
                IsDomainMatch(cookie, requestUrl, out reason) &&
                IsPathMatch(cookie, requestUrl, out reason);
        }

        private static bool IsDomainMatch(this FlurlCookie cookie, FlurlModel requestUrl, out string reason)
        {
            reason = "ok";

            if (!string.IsNullOrEmpty(cookie.Domain))
            {
                var domain = cookie.Domain.OrdinalStartsWith(".") ? cookie.Domain.Substring(1) : cookie.Domain;
                if (requestUrl.Host.OrdinalEquals(domain, true))
                    return true;

                if (requestUrl.Host.OrdinalEndsWith("." + domain, true))
                    return true;

                reason = $"Cookie with Domain={cookie.Domain} should not be sent to {requestUrl.Host}.";
                return false;
            }
            else
            {
                if (requestUrl.Host.OrdinalEquals(cookie.OriginUrl.Host, true))
                    return true;

                reason = $"Cookie set from {cookie.OriginUrl.Host} without Domain specified should only be sent to that specific host, not {requestUrl.Host}.";
                return false;
            }
        }

        private static bool IsPathMatch(this FlurlCookie cookie, FlurlModel requestUrl, out string reason)
        {
            reason = "ok";

            // implementation of default-path algorithm https://tools.ietf.org/html/rfc6265#section-5.1.4
            string GetDefaultPath()
            {
                var origPath = cookie.OriginUrl.Path;
                if (origPath == "" || origPath[0] != '/') return "/";
                if (origPath.Count(c => c == '/') <= 1) return "/";
                return origPath.Substring(0, origPath.LastIndexOf('/'));
            }

            // https://tools.ietf.org/html/rfc6265#section-5.2.4
            var cookiePath = (cookie.Path?.OrdinalStartsWith("/") == true) ? cookie.Path : GetDefaultPath();

            if (cookiePath.Length > 1 && cookiePath.OrdinalEndsWith("/"))
                cookiePath = cookiePath.TrimEnd('/');

            if (cookiePath == "/")
                return true;

            var requestPath = (requestUrl.Path.Length > 0) ? requestUrl.Path : "/";

            if (requestPath.OrdinalEquals(cookiePath)) // Path is case-sensitive, unlike Domain
                return true;

            if (requestPath.OrdinalStartsWith(cookiePath) && requestPath[cookiePath.Length] == '/')
                return true;

            reason = string.IsNullOrEmpty(cookie.Path) ?
                $"Cookie from path {cookiePath} should not be sent to path {requestUrl.Path}." :
                $"Cookie with Path={cookie.Path} should not be sent to path {requestUrl.Path}.";

            return false;
        }

        // Possible future enhancement: https://github.com/tmenier/Flurl/issues/538
        // This method works, but the feature still needs caching of some kind and an opt-in config setting.
        //private static async Task<bool> IsPublicSuffixesAsync(string domain) {
        //	using (var stream = await "https://publicsuffix.org/list/public_suffix_list.dat".GetStreamAsync())
        //	using (var reader = new StreamReader(stream)) {
        //		while (true) {
        //			var line = await reader.ReadLineAsync();
        //			if (line == null) break;
        //			if (line.Trim() == "") continue;
        //			if (line.OrdinalStartsWith("//")) continue;
        //			if (line == domain) return true;
        //		}
        //	}
        //	return false;
        //}
    }
    /// <summary>
    /// Fluent extension methods for working with HTTP cookies.
    /// </summary>
    public static class CookieExtensions
    {
        /// <summary>
        /// Adds or updates a name-value pair in this request's Cookie header.
        /// To automatically maintain a cookie "session", consider using a CookieJar or CookieSession instead.
        /// </summary>
        /// <param name="request">The IFlurlRequest.</param>
        /// <param name="name">The cookie name.</param>
        /// <param name="value">The cookie value.</param>
        /// <returns>This IFlurlClient instance.</returns>
        public static IFlurlRequest WithCookie(this IFlurlRequest request, string name, object value)
        {
            var cookies = new NameValueList<string>(request.Cookies, true); // cookie names are case-sensitive https://stackoverflow.com/a/11312272/62600
            cookies.AddOrReplace(name, value.ToInvariantString());
            return request.WithHeader("Cookie", CookieCutter.ToRequestHeader(cookies));
        }

        /// <summary>
        /// Adds or updates name-value pairs in this request's Cookie header, based on property names/values
        /// of the provided object, or keys/values if object is a dictionary.
        /// To automatically maintain a cookie "session", consider using a CookieJar or CookieSession instead.
        /// </summary>
        /// <param name="request">The IFlurlRequest.</param>
        /// <param name="values">Names/values of HTTP cookies to set. Typically an anonymous object or IDictionary.</param>
        /// <returns>This IFlurlClient.</returns>
        public static IFlurlRequest WithCookies(this IFlurlRequest request, object values)
        {
            var cookies = new NameValueList<string>(request.Cookies, true); // cookie names are case-sensitive https://stackoverflow.com/a/11312272/62600
                                                                            // although rare, we need to accommodate the possibility of multiple cookies with the same name
            foreach (var group in values.ToKeyValuePairs().GroupBy(x => x.Key))
            {
                // add or replace the first one (by name)
                cookies.AddOrReplace(group.Key, group.First().Value.ToInvariantString());
                // append the rest
                foreach (var kv in group.Skip(1))
                    cookies.Add(kv.Key, kv.Value.ToInvariantString());
            }
            return request.WithHeader("Cookie", CookieCutter.ToRequestHeader(cookies));
        }

        /// <summary>
        /// Sets the CookieJar associated with this request, which will be updated with any Set-Cookie headers present
        /// in the response and is suitable for reuse in subsequent requests.
        /// </summary>
        /// <param name="request">The IFlurlRequest.</param>
        /// <param name="cookieJar">The CookieJar.</param>
        /// <returns>This IFlurlClient instance.</returns>
        public static IFlurlRequest WithCookies(this IFlurlRequest request, CookieJar cookieJar)
        {
            request.CookieJar = cookieJar;
            return request;
        }

        /// <summary>
        /// Creates a new CookieJar and associates it with this request, which will be updated with any Set-Cookie
        /// headers present in the response and is suitable for reuse in subsequent requests.
        /// </summary>
        /// <param name="request">The IFlurlRequest.</param>
        /// <param name="cookieJar">The created CookieJar, which can be reused in subsequent requests.</param>
        /// <returns>This IFlurlClient instance.</returns>
        public static IFlurlRequest WithCookies(this IFlurlRequest request, out CookieJar cookieJar)
        {
            cookieJar = new CookieJar();
            return request.WithCookies(cookieJar);
        }
    }
    /// <summary>
    /// A collection of FlurlCookies that can be attached to one or more FlurlRequests, either explicitly via WithCookies
    /// or implicitly via a CookieSession. Stores cookies received via Set-Cookie response headers.
    /// </summary>
    public class CookieJar : IReadOnlyCollection<FlurlCookie>
    {
        private readonly ConcurrentDictionary<string, FlurlCookie> _dict = new ConcurrentDictionary<string, FlurlCookie>();

        /// <summary>
        /// Adds a cookie to the jar or replaces one with the same Name/Domain/Path.
        /// Throws InvalidCookieException if cookie is invalid.
        /// </summary>
        /// <param name="name">Name of the cookie.</param>
        /// <param name="value">Value of the cookie.</param>
        /// <param name="originUrl">URL of request that sent the original Set-Cookie header.</param>
        /// <param name="dateReceived">Date/time that original Set-Cookie header was received. Defaults to current date/time. Important for Max-Age to be enforced correctly.</param>
        public CookieJar AddOrReplace(string name, object value, string originUrl, DateTimeOffset? dateReceived = null) =>
            AddOrReplace(new FlurlCookie(name, value.ToInvariantString(), originUrl, dateReceived));

        /// <summary>
        /// Adds a cookie to the jar or replaces one with the same Name/Domain/Path.
        /// Throws InvalidCookieException if cookie is invalid.
        /// </summary>
        public CookieJar AddOrReplace(FlurlCookie cookie)
        {
            if (!TryAddOrReplace(cookie, out var reason))
                throw new InvalidCookieException(reason);

            return this;
        }

        /// <summary>
        /// Adds a cookie to the jar or updates if one with the same Name/Domain/Path already exists,
        /// but only if it is valid and not expired.
        /// </summary>
        /// <returns>true if cookie is valid and was added or updated. If false, provides descriptive reason.</returns>
        public bool TryAddOrReplace(FlurlCookie cookie, out string reason)
        {
            if (!cookie.IsValid(out reason))
                return false;

            if (cookie.IsExpired(out reason))
            {
                // when server sends an expired cookie, it's effectively an instruction for client to delete it.
                // https://stackoverflow.com/a/53573622/62600
                _dict.TryRemove(cookie.GetKey(), out _);
                return false;
            }

            cookie.Lock(); // makes immutable
            _dict[cookie.GetKey()] = cookie;

            return true;
        }

        /// <summary>
        /// Removes all cookies matching the given predicate.
        /// </summary>
        public CookieJar Remove(Func<FlurlCookie, bool> predicate)
        {
            var keys = _dict.Where(kv => predicate(kv.Value)).Select(kv => kv.Key).ToList();
            foreach (var key in keys)
                _dict.TryRemove(key, out _);
            return this;
        }

        /// <summary>
        /// Removes all cookies from this CookieJar
        /// </summary>
        public CookieJar Clear()
        {
            _dict.Clear();
            return this;
        }

        /// <inheritdoc/>
        public IEnumerator<FlurlCookie> GetEnumerator() => _dict.Values.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => _dict.GetEnumerator();

        /// <inheritdoc/>
        public int Count => _dict.Count;
    }

    /// <summary>
    /// Exception thrown when attempting to add or update an invalid FlurlCookie to a CookieJar.
    /// </summary>
    public class InvalidCookieException : Exception
    {
        /// <summary>
        /// Creates a new InvalidCookieException.
        /// </summary>
        public InvalidCookieException(string reason) : base(reason) { }
    }
    /// <summary>
    /// A context where multiple requests use a common CookieJar.
    /// </summary>
    public class CookieSession : IDisposable
    {
        private readonly string _baseUrl;
        private readonly IFlurlClient _client;

        /// <summary>
        /// Creates a new CookieSession where all requests are made off the same base URL.
        /// </summary>
        public CookieSession(string baseUrl = null)
        {
            _baseUrl = baseUrl;
        }

        /// <summary>
        /// Creates a new CookieSession where all requests are made using the provided IFlurlClient
        /// </summary>
        public CookieSession(IFlurlClient client)
        {
            _client = client;
        }

        /// <summary>
        /// The CookieJar used by all requests sent with this CookieSession.
        /// </summary>
        public CookieJar Cookies { get; } = new CookieJar();

        /// <summary>
        /// Creates a new IFlurlRequest with this session's CookieJar that can be further built and sent fluently.
        /// </summary>
        /// <param name="urlSegments">The URL or URL segments for the request.</param>
        public IFlurlRequest Request(params object[] urlSegments) => (_client == null) ?
            new FlurlRequest(_baseUrl, urlSegments).WithCookies(Cookies) :
            new FlurlRequest(_client, urlSegments).WithCookies(Cookies);

        /// <summary>
        /// Not necessary to call. IDisposable is implemented mainly for the syntactic sugar of using statements.
        /// </summary>
        public void Dispose() { }
    }
    /// <summary>
    /// An IFlurlClientFactory implementation that caches and reuses the same one instance of
    /// FlurlClient per combination of scheme, host, and port. This is the default
    /// implementation used when calls are made fluently off Urls/strings.
    /// </summary>
    public class DefaultFlurlClientFactory : FlurlClientFactoryBase
    {
        /// <summary>
        /// Returns a unique cache key based on scheme, host, and port of the given URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>The cache key</returns>
        protected override string GetCacheKey(FlurlModel url) => $"{url.Scheme}|{url.Host}|{url.Port}";
    }
    /// <summary>
    /// ISerializer implementation based on System.Text.Json.
    /// Default serializer used in calls to GetJsonAsync, PostJsonAsync, etc.
    /// </summary>
    public class DefaultJsonSerializer : ISerializer
    {
        private readonly JsonSerializerSettings _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultJsonSerializer"/> class.
        /// </summary>
        /// <param name="options">Options to control (de)serialization behavior.</param>
        public DefaultJsonSerializer(JsonSerializerSettings options = null)
        {
            _options = options ?? CobberCaller.CurrentWebsiteNewtonsoftSetting;
        }

        /// <summary>
        /// Serializes the specified object to a JSON string.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        public string Serialize(object obj) => JsonConvert.SerializeObject(obj, _options);

        /// <summary>
        /// Deserializes the specified JSON string to an object of type T.
        /// </summary>
        /// <param name="s">The JSON string to deserialize.</param>
        public T Deserialize<T>(string s) => string.IsNullOrWhiteSpace(s) ? default : JsonConvert.DeserializeObject<T>(s, _options);

        /// <summary>
        /// Deserializes the specified stream to an object of type T.
        /// </summary>
        /// <param name="stream">The stream to deserialize.</param>
        public T Deserialize<T>(Stream stream)
        {
            if (stream.Length == 0) { return default; }
            using var jsonW = new StreamReader(stream);
            return JsonConvert.DeserializeObject<T>(jsonW.ReadToEnd(), _options);
        }
    }
    /// <summary>
    /// ISerializer implementation that converts an object representing name/value pairs to a URL-encoded string.
    /// Default serializer used in calls to PostUrlEncodedAsync, etc. 
    /// </summary>
    public class DefaultUrlEncodedSerializer : ISerializer
    {
        /// <summary>
        /// Serializes the specified object to a URL-encoded string.
        /// </summary>
        public string Serialize(object obj)
        {
            if (obj == null)
                return null;

            var qp = new QueryParamCollection();
            foreach (var kv in obj.ToKeyValuePairs())
                qp.AddOrReplace(kv.Key, kv.Value, false, NullValueHandling.Ignore);
            return qp.ToString(true);
        }

        /// <summary>
        /// Deserializing a URL-encoded string is not supported.
        /// </summary>
        public T Deserialize<T>(string s) => throw new NotImplementedException("Deserializing a URL-encoded string is not supported.");

        /// <summary>
        /// Deserializing a URL-encoded string is not supported.
        /// </summary>
        public T Deserialize<T>(Stream stream) => throw new NotImplementedException("Deserializing a URL-encoded string is not supported.");
    }
    /// <summary>
    /// Fluent extension methods for downloading a file.
    /// </summary>
    public static class DownloadExtensions
    {
        /// <summary>
        /// Asynchronously downloads a file at the specified URL.
        /// </summary>
        /// <param name="request">The flurl request.</param>
        /// <param name="localFolderPath">Path of local folder where file is to be downloaded.</param>
        /// <param name="localFileName">Name of local file. If not specified, the source filename (from Content-Dispostion header, or last segment of the URL) is used.</param>
        /// <param name="bufferSize">Buffer size in bytes. Default is 4096.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the local path of the downloaded file.</returns>
        public static async Task<string> DownloadFileAsync(this IFlurlRequest request, string localFolderPath, string localFileName = null, int bufferSize = 4096, HttpCompletionOption completionOption = HttpCompletionOption.ResponseHeadersRead, CancellationToken cancellationToken = default)
        {
            using (var resp = await request.SendAsync(HttpMethod.Get, null, completionOption, cancellationToken).ConfigureAwait(false))
            {
                localFileName ??=
                    GetFileNameFromHeaders(resp.ResponseMessage) ??
                    GetFileNameFromPath(request);

                // http://codereview.stackexchange.com/a/18679
                using (var httpStream = await resp.GetStreamAsync().ConfigureAwait(false))
                using (var fileStream = await FileUtil.OpenWriteAsync(localFolderPath, localFileName, bufferSize).ConfigureAwait(false))
                {
                    await httpStream.CopyToAsync(fileStream, bufferSize, cancellationToken).ConfigureAwait(false);
                }
            }

            return FileUtil.CombinePath(localFolderPath, localFileName);
        }

        private static string GetFileNameFromHeaders(HttpResponseMessage resp)
        {
            var header = resp.Content?.Headers.ContentDisposition;
            if (header == null) return null;
            // prefer filename* per https://tools.ietf.org/html/rfc6266#section-4.3
            var val = (header.FileNameStar ?? header.FileName)?.StripQuotes();
            if (val == null) return null;
            return FileUtil.MakeValidName(val);
        }

        private static string GetFileNameFromPath(IFlurlRequest req)
        {
            return FileUtil.MakeValidName(FlurlModel.Decode(req.Url.Path.Split('/').Last(), false));
        }
    }
    /// <summary>
    /// Represents HTTP content based on a local file. Typically used with PostMultipartAsync for uploading files.
    /// </summary>
    public class FileContent : HttpContent
    {
        /// <summary>
        /// The local file path.
        /// </summary>
        public string Path { get; }

        private readonly int _bufferSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileContent"/> class.
        /// </summary>
        /// <param name="path">The local file path.</param>
        /// <param name="bufferSize">The buffer size of the stream upload in bytes. Defaults to 4096.</param>
        public FileContent(string path, int bufferSize = 4096)
        {
            Path = path;
            _bufferSize = bufferSize;
        }

        /// <summary>
        /// Serializes to stream asynchronous.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            using (var source = await FileUtil.OpenReadAsync(Path, _bufferSize).ConfigureAwait(false))
            {
                await source.CopyToAsync(stream, _bufferSize).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Tries the length of the compute.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }
    }
    internal static class FileUtil
    {
        internal static string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        internal static string CombinePath(params string[] paths)
        {
            return Path.Combine(paths);
        }

        internal static Task<Stream> OpenReadAsync(string path, int bufferSize)
        {
            return TestTry.TaskFromResult<Stream>(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, useAsync: true));
        }

        internal static Task<Stream> OpenWriteAsync(string folderPath, string fileName, int bufferSize)
        {
            Directory.CreateDirectory(folderPath); // checks existence
            var filePath = Path.Combine(folderPath, fileName);
            return TestTry.TaskFromResult<Stream>(new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, useAsync: true));
        }

        /// <summary>
        /// Replaces invalid path characters with underscores.
        /// </summary>
        internal static string MakeValidName(string s)
        {
            return string.Join("_", s.Split(Path.GetInvalidFileNameChars()));
        }
    }
    /// <summary>
    /// Represents a set of request conditions and fake responses for faking HTTP calls in tests.
    /// Usually created fluently via HttpTest.ForCallsTo, rather than instantiated directly.
    /// </summary>
    public class FilteredHttpTestSetup : HttpTestSetup
    {
        private readonly List<Func<FlurlCall, bool>> _filters = new List<Func<FlurlCall, bool>>();

        /// <summary>
        /// Constructs a new instance of FilteredHttpTestSetup.
        /// </summary>
        /// <param name="settings">FlurlHttpSettings used in fake calls.</param>
        /// <param name="urlPatterns">URL(s) or URL pattern(s) that this HttpTestSetup applies to. Can contain * wildcard.</param>
        public FilteredHttpTestSetup(FlurlHttpSettings settings, params string[] urlPatterns) : base(settings)
        {
            if (urlPatterns.Any())
                With(call => urlPatterns.Any(p => Util.MatchesUrlPattern(call.Request.Url, p)));
        }

        /// <summary>
        /// Returns true if the given FlurlCall matches one of the URL patterns and all other criteria defined for this HttpTestSetup.
        /// </summary>
        internal bool IsMatch(FlurlCall call) => _filters.All(f => f(call));

        /// <summary>
        /// Defines a condition for which this HttpTestSetup applies.
        /// </summary>
        public FilteredHttpTestSetup With(Func<FlurlCall, bool> condition)
        {
            _filters.Add(condition);
            return this;
        }

        /// <summary>
        /// Defines a condition for which this HttpTestSetup does NOT apply.
        /// </summary>
        public FilteredHttpTestSetup Without(Func<FlurlCall, bool> condition)
        {
            return With(c => !condition(c));
        }

        /// <summary>
        /// Defines one or more HTTP verbs, any of which a call must match in order for this HttpTestSetup to apply.
        /// </summary>
        public FilteredHttpTestSetup WithVerb(params HttpMethod[] verbs)
        {
            return With(call => call.HasAnyVerb(verbs));
        }

        /// <summary>
        /// Defines one or more HTTP verbs, any of which a call must match in order for this HttpTestSetup to apply.
        /// </summary>
        public FilteredHttpTestSetup WithVerb(params string[] verbs)
        {
            return With(call => call.HasAnyVerb(verbs));
        }

        /// <summary>
        /// Defines a query parameter and (optionally) its value that a call must contain in order for this HttpTestSetup to apply.
        /// </summary>
        public FilteredHttpTestSetup WithQueryParam(string name, object value = null)
        {
            return With(c => c.HasQueryParam(name, value));
        }

        /// <summary>
        /// Defines a query parameter and (optionally) its value that a call must NOT contain in order for this HttpTestSetup to apply.
        /// </summary>
        public FilteredHttpTestSetup WithoutQueryParam(string name, object value = null)
        {
            return Without(c => c.HasQueryParam(name, value));
        }

        /// <summary>
        /// Defines query parameter names, ALL of which a call must contain in order for this HttpTestSetup to apply.
        /// </summary>
        public FilteredHttpTestSetup WithQueryParams(params string[] names)
        {
            return With(c => c.HasAllQueryParams(names));
        }

        /// <summary>
        /// Defines query parameter names, NONE of which a call must contain in order for this HttpTestSetup to apply.
        /// If no names are provided, call must not contain any query parameters.
        /// </summary>
        public FilteredHttpTestSetup WithoutQueryParams(params string[] names)
        {
            return Without(c => c.HasAnyQueryParam(names));
        }

        /// <summary>
        /// Defines query parameters, ALL of which a call must contain in order for this HttpTestSetup to apply.
        /// </summary>
        /// <param name="values">Object (usually anonymous) or dictionary that is parsed to name/value query parameters to check for. Values may contain * wildcard.</param>
        public FilteredHttpTestSetup WithQueryParams(object values)
        {
            return With(c => c.HasQueryParams(values));
        }

        /// <summary>
        /// Defines query parameters, NONE of which a call must contain in order for this HttpTestSetup to apply.
        /// </summary>
        /// <param name="values">Object (usually anonymous) or dictionary that is parsed to name/value query parameters to check for. Values may contain * wildcard.</param>
        public FilteredHttpTestSetup WithoutQueryParams(object values)
        {
            return Without(c => c.HasQueryParams(values));
        }

        /// <summary>
        /// Defines query parameter names, ANY of which a call must contain in order for this HttpTestSetup to apply.
        /// If no names are provided, call must contain at least one query parameter with any name.
        /// </summary>
        public FilteredHttpTestSetup WithAnyQueryParam(params string[] names)
        {
            return With(c => c.HasAnyQueryParam(names));
        }

        /// <summary>
        /// Defines a request header and (optionally) its value that a call must contain in order for this HttpTestSetup to apply.
        /// </summary>
        public FilteredHttpTestSetup WithHeader(string name, string valuePattern = null)
        {
            return With(c => c.HasHeader(name, valuePattern));
        }

        /// <summary>
        /// Defines a request header and (optionally) its value that a call must NOT contain in order for this HttpTestSetup to apply.
        /// </summary>
        public FilteredHttpTestSetup WithoutHeader(string name, string valuePattern = null)
        {
            return Without(c => c.HasHeader(name, valuePattern));
        }

        /// <summary>
        /// Defines a request body that must exist in order for this HttpTestSetup to apply.
        /// The * wildcard can be used.
        /// </summary>
        public FilteredHttpTestSetup WithRequestBody(string pattern)
        {
            return With(call => Util.MatchesPattern(call.RequestBody, pattern));
        }

        /// <summary>
        /// Defines an object that, when serialized to JSON, must match the request body in order for this HttpTestSetup to apply.
        /// </summary>
        public FilteredHttpTestSetup WithRequestJson(object body)
        {
            return WithRequestBody(Settings.JsonSerializer.Serialize(body));
        }
    }
    /// <summary>
    /// Encapsulates request, response, and other details associated with an HTTP call. Useful for diagnostics and available in
    /// global event handlers and FlurlHttpException.Call.
    /// </summary>
    public class FlurlCall
    {
        /// <summary>
        /// The IFlurlRequest associated with this call.
        /// </summary>
        public IFlurlRequest Request { get; set; }

        /// <summary>
        /// The raw HttpRequestMessage associated with this call.
        /// </summary>
        public HttpRequestMessage HttpRequestMessage { get; set; }

        /// <summary>
        /// Captured request body. Available ONLY if HttpRequestMessage.Content is a Flurl.Http.Content.CapturedStringContent.
        /// </summary>
        public string RequestBody => (HttpRequestMessage.Content as CapturedStringContent)?.Content;

        /// <summary>
        /// The IFlurlResponse associated with this call if the call completed, otherwise null.
        /// </summary>
        public IFlurlResponse Response { get; set; }

        /// <summary>
        /// If this call has a 3xx response and Location header, contains information about how to handle the redirect.
        /// Otherwise null.
        /// </summary>
        public FlurlRedirect Redirect { get; set; }

        /// <summary>
        /// The raw HttpResponseMessage associated with the call if the call completed, otherwise null.
        /// </summary>
        public HttpResponseMessage HttpResponseMessage { get; set; }

        /// <summary>
        /// Exception that occurred while sending the HttpRequestMessage.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// User code should set this to true inside global event handlers (OnError, etc) to indicate
        /// that the exception was handled and should not be propagated further.
        /// </summary>
        public bool ExceptionHandled { get; set; }

        /// <summary>
        /// DateTime the moment the request was sent.
        /// </summary>
        public DateTime StartedUtc { get; set; }

        /// <summary>
        /// DateTime the moment a response was received.
        /// </summary>
        public DateTime? EndedUtc { get; set; }

        /// <summary>
        /// Total duration of the call if it completed, otherwise null.
        /// </summary>
        public TimeSpan? Duration => EndedUtc - StartedUtc;

        /// <summary>
        /// True if a response was received, regardless of whether it is an error status.
        /// </summary>
        public bool Completed => HttpResponseMessage != null;

        /// <summary>
        /// True if response was received with any success status or a match with AllowedHttpStatusRange setting.
        /// </summary>
        public bool Succeeded =>
            HttpResponseMessage == null ? false :
            (int)HttpResponseMessage.StatusCode < 400 ? true :
            string.IsNullOrEmpty(Request?.Settings?.AllowedHttpStatusRange) ? false :
            HttpStatusRangeParser.IsMatch(Request.Settings.AllowedHttpStatusRange, HttpResponseMessage.StatusCode);

        /// <summary>
        /// Returns the verb and absolute URI associated with this call.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{HttpRequestMessage.Method:U} {Request.Url}";
        }
    }

    /// <summary>
    /// An object containing information about if/how an automatic redirect request will be created and sent.
    /// </summary>
    public class FlurlRedirect
    {
        /// <summary>
        /// The URL to redirect to, from the response's Location header.
        /// </summary>
        public FlurlModel Url { get; set; }

        /// <summary>
        /// The number of auto-redirects that have already occurred since the original call, plus 1 for this one.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// If true, Flurl will automatically send a redirect request. Set to false to prevent auto-redirect.
        /// </summary>
        public bool Follow { get; set; }

        /// <summary>
        /// If true, the redirect request will use the GET verb and will not forward the original request body.
        /// Otherwise, the original verb and body will be preserved in the redirect.
        /// </summary>
        public bool ChangeVerbToGet { get; set; }
    }
    /// <summary>
    /// Interface defining FlurlClient's contract (useful for mocking and DI)
    /// </summary>
    public interface IFlurlClient : IHttpSettingsContainer, IDisposable
    {
        /// <summary>
        /// Gets the HttpClient to be used in subsequent HTTP calls. Creation (when necessary) is delegated
        /// to FlurlHttp.FlurlClientFactory. Reused for the life of the FlurlClient.
        /// </summary>
        HttpClient HttpClient { get; }

        /// <summary>
        /// Gets or sets the base URL used for all calls made with this client.
        /// </summary>
        string BaseUrl { get; set; }

        /// <summary>
        /// Creates a new IFlurlRequest that can be further built and sent fluently.
        /// </summary>
        /// <param name="urlSegments">The URL or URL segments for the request. If BaseUrl is defined, it is assumed that these are path segments off that base.</param>
        /// <returns>A new IFlurlRequest</returns>
        IFlurlRequest Request(params object[] urlSegments);

        /// <summary>
        /// Gets a value indicating whether this instance (and its underlying HttpClient) has been disposed.
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// Asynchronously sends an HTTP request.
        /// </summary>
        /// <param name="request">The IFlurlRequest to send.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        Task<IFlurlResponse> SendAsync(IFlurlRequest request, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// A reusable object for making HTTP calls.
    /// </summary>
    public class FlurlClient : IFlurlClient
    {
        /// <summary>
        /// Initializes a new instance of <see cref="FlurlClient"/>.
        /// </summary>
        /// <param name="baseUrl">The base URL associated with this client.</param>
        public FlurlClient(string baseUrl = null) :
            this(FlurlHttp.GlobalSettings.FlurlClientFactory.CreateHttpClient(), baseUrl)
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="FlurlClient"/>, wrapping an existing HttpClient.
        /// Generally, you should let Flurl create and manage HttpClient instances for you, but you might, for
        /// example, have an HttpClient instance that was created by a 3rd-party library and you want to use
        /// Flurl to build and send calls with it. Be aware that if the HttpClient has an underlying
        /// HttpMessageHandler that processes cookies and automatic redirects (as is the case by default),
        /// Flurl's re-implementation of those features may not work properly.
        /// </summary>
        /// <param name="httpClient">The instantiated HttpClient instance.</param>
        /// <param name="baseUrl">The base URL associated with this client.</param>
        public FlurlClient(HttpClient httpClient, string baseUrl = null)
        {
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            BaseUrl = baseUrl ?? HttpClient.BaseAddress?.ToString();
        }

        /// <inheritdoc />
        public string BaseUrl { get; set; }

        /// <inheritdoc />
        public FlurlHttpSettings Settings { get; } = new FlurlHttpSettings();

        /// <inheritdoc />
        public INameValueList<string> Headers { get; } = new NameValueList<string>(false); // header names are case-insensitive https://stackoverflow.com/a/5259004/62600

        /// <inheritdoc />
        public HttpClient HttpClient { get; }

        /// <inheritdoc />
        public IFlurlRequest Request(params object[] urlSegments) => new FlurlRequest(this, urlSegments);

        /// <inheritdoc />
        public async Task<IFlurlResponse> SendAsync(IFlurlRequest request, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (request.Url == null)
                throw new ArgumentException("Cannot send Request. Url property was not set.");
            if (!FlurlModel.IsValid(request.Url))
                throw new ArgumentException($"Cannot send Request. {request.Url} is a not a valid URL.");

            var settings = request.Settings;
            var reqMsg = new HttpRequestMessage(request.Verb, request.Url) { Content = request.Content };

            SyncHeaders(request, reqMsg);
            var call = new FlurlCall
            {
                Request = request,
                HttpRequestMessage = reqMsg
            };

            await RaiseEventAsync(settings.BeforeCall, settings.BeforeCallAsync, call).ConfigureAwait(false);

            // in case URL or headers were modified in the handler above
            reqMsg.RequestUri = request.Url.ToUri();
            SyncHeaders(request, reqMsg);

            call.StartedUtc = DateTime.UtcNow;
            var ct = GetCancellationTokenWithTimeout(cancellationToken, settings.Timeout, out var cts);

            HttpTest.Current?.LogCall(call);

            try
            {
                call.HttpResponseMessage =
                    HttpTest.Current?.FindSetup(call)?.GetNextResponse() ??
                    await HttpClient.SendAsync(reqMsg, completionOption, ct).ConfigureAwait(false);

                call.HttpResponseMessage.RequestMessage = reqMsg;
                call.Response = new FlurlResponse(call, request.CookieJar);

                if (call.Succeeded)
                {
                    var redirResponse = await ProcessRedirectAsync(call, completionOption, cancellationToken).ConfigureAwait(false);
                    return redirResponse ?? call.Response;
                }
                else
                    throw new FlurlHttpException(call, null);
            }
            catch (Exception ex)
            {
                return await HandleExceptionAsync(call, ex, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                reqMsg.Dispose();
                cts?.Dispose();
                call.EndedUtc = DateTime.UtcNow;
                await RaiseEventAsync(settings.AfterCall, settings.AfterCallAsync, call).ConfigureAwait(false);
            }
        }

        private void SyncHeaders(IFlurlRequest req, HttpRequestMessage reqMsg)
        {
            // copy any client-level (default) headers to FlurlRequest
            foreach (var header in this.Headers.Where(h => !req.Headers.Contains(h.Name)).ToList())
                req.Headers.Add(header.Name, header.Value);

            // copy headers from FlurlRequest to HttpRequestMessage
            foreach (var header in req.Headers)
                reqMsg.SetHeader(header.Name, header.Value.Trim(), false);

            // copy headers from HttpContent to FlurlRequest
            if (reqMsg.Content != null)
            {
                foreach (var header in reqMsg.Content.Headers)
                    req.Headers.AddOrReplace(header.Key, string.Join(",", header.Value));
            }
        }

        private async Task<IFlurlResponse> ProcessRedirectAsync(FlurlCall call, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            var settings = call.Request.Settings;
            if (settings.Redirects.Enabled)
                call.Redirect = GetRedirect(call);

            if (call.Redirect == null)
                return null;

            await RaiseEventAsync(settings.OnRedirect, settings.OnRedirectAsync, call).ConfigureAwait(false);

            if (call.Redirect.Follow != true)
                return null;

            var changeToGet = call.Redirect.ChangeVerbToGet;

            var redir = new FlurlRequest(this)
            {
                Url = call.Redirect.Url,
                Verb = changeToGet ? HttpMethod.Get : call.HttpRequestMessage.Method,
                Content = changeToGet ? null : call.Request.Content,
                RedirectedFrom = call,
                Settings = { Defaults = settings }
            };

            if (call.Request.CookieJar != null)
                redir.CookieJar = call.Request.CookieJar;

            redir.WithHeaders(call.Request.Headers.Where(h =>
                h.Name.OrdinalEquals("Cookie", true) ? false : // never blindly forward Cookie header; CookieJar should be used to ensure rules are enforced
                h.Name.OrdinalEquals("Authorization", true) ? settings.Redirects.ForwardAuthorizationHeader :
                h.Name.OrdinalEquals("Transfer-Encoding", true) ? settings.Redirects.ForwardHeaders && !changeToGet :
                settings.Redirects.ForwardHeaders));

            var ct = GetCancellationTokenWithTimeout(cancellationToken, settings.Timeout, out var cts);
            try
            {
                return await SendAsync(redir, completionOption, ct).ConfigureAwait(false);
            }
            finally
            {
                cts?.Dispose();
            }
        }

        // partially lifted from https://github.com/dotnet/runtime/blob/master/src/libraries/System.Net.Http/src/System/Net/Http/SocketsHttpHandler/RedirectHandler.cs
        private static FlurlRedirect GetRedirect(FlurlCall call)
        {
            if (call.Response.StatusCode < 300 || call.Response.StatusCode > 399)
                return null;

            if (!call.Response.Headers.TryGetFirst("Location", out var location))
                return null;

            var redir = new FlurlRedirect();

            if (FlurlModel.IsValid(location))
                redir.Url = new FlurlModel(location);
            else if (location.OrdinalStartsWith("//"))
                redir.Url = new FlurlModel(call.Request.Url.Scheme + ":" + location);
            else if (location.OrdinalStartsWith("/"))
                redir.Url = FlurlModel.Combine(call.Request.Url.Root, location);
            else
                redir.Url = FlurlModel.Combine(call.Request.Url.Root, call.Request.Url.Path, location);

            // Per https://tools.ietf.org/html/rfc7231#section-7.1.2, a redirect location without a
            // fragment should inherit the fragment from the original URI.
            if (string.IsNullOrEmpty(redir.Url.Fragment))
                redir.Url.Fragment = call.Request.Url.Fragment;

            redir.Count = 1 + (call.Request.RedirectedFrom?.Redirect?.Count ?? 0);

            var isSecureToInsecure = (call.Request.Url.IsSecureScheme && !redir.Url.IsSecureScheme);

            redir.Follow =
                new[] { 301, 302, 303, 307, 308 }.Contains(call.Response.StatusCode) &&
                redir.Count <= call.Request.Settings.Redirects.MaxAutoRedirects &&
                (call.Request.Settings.Redirects.AllowSecureToInsecure || !isSecureToInsecure);

            bool ChangeVerbToGetOn(int statusCode, HttpMethod verb)
            {
                switch (statusCode)
                {
                    // 301 and 302 are a bit ambiguous. The spec says to preserve the verb
                    // but most browsers rewrite it to GET. HttpClient stack changes it if
                    // only it's a POST, presumably since that's a browser-friendly verb.
                    // Seems odd, but sticking with that is probably the safest bet.
                    // https://github.com/dotnet/runtime/blob/master/src/libraries/System.Net.Http/src/System/Net/Http/SocketsHttpHandler/RedirectHandler.cs#L140
                    case 301:
                    case 302:
                        return verb == HttpMethod.Post;
                    case 303:
                        return true;
                    default: // 307 & 308 mainly
                        return false;
                }
            }

            redir.ChangeVerbToGet =
                redir.Follow &&
                ChangeVerbToGetOn(call.Response.StatusCode, call.Request.Verb);

            return redir;
        }

        private static Task RaiseEventAsync(Action<FlurlCall> syncHandler, Func<FlurlCall, Task> asyncHandler, FlurlCall call)
        {
            syncHandler?.Invoke(call);
            if (asyncHandler != null)
                return asyncHandler(call);
            return TestTry.TaskFromResult(0);
        }

        internal static async Task<IFlurlResponse> HandleExceptionAsync(FlurlCall call, Exception ex, CancellationToken token)
        {
            call.Exception = ex;
            await RaiseEventAsync(call.Request.Settings.OnError, call.Request.Settings.OnErrorAsync, call).ConfigureAwait(false);

            if (call.ExceptionHandled)
                return call.Response;

            if (ex is OperationCanceledException && !token.IsCancellationRequested)
                throw new FlurlHttpTimeoutException(call, ex);

            if (ex is FlurlHttpException)
                throw ex;

            throw new FlurlHttpException(call, ex);
        }

        private static CancellationToken GetCancellationTokenWithTimeout(CancellationToken original, TimeSpan? timeout, out CancellationTokenSource timeoutTokenSource)
        {
            timeoutTokenSource = null;
            if (!timeout.HasValue)
                return original;

            timeoutTokenSource = CancellationTokenSource.CreateLinkedTokenSource(original);
            timeoutTokenSource.CancelAfter(timeout.Value);
            return timeoutTokenSource.Token;
        }

        /// <inheritdoc />
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Disposes the underlying HttpClient and HttpMessageHandler.
        /// </summary>
        public virtual void Dispose()
        {
            if (IsDisposed)
                return;

            HttpClient.Dispose();
            IsDisposed = true;
        }
    }
    /// <summary>
    /// Encapsulates a creation/caching strategy for IFlurlClient instances. Custom factories looking to extend
    /// Flurl's behavior should inherit from this class, rather than implementing IFlurlClientFactory directly.
    /// </summary>
    public abstract class FlurlClientFactoryBase : IFlurlClientFactory
    {
        private readonly ConcurrentDictionary<string, IFlurlClient> _clients = new ConcurrentDictionary<string, IFlurlClient>();

        /// <summary>
        /// By default, uses a caching strategy of one FlurlClient per host. This maximizes reuse of
        /// underlying HttpClient/Handler while allowing things like cookies to be host-specific.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>The FlurlClient instance.</returns>
        public virtual IFlurlClient Get(FlurlModel url)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            return _clients.AddOrUpdate(
                GetCacheKey(url),
                u => Create(u),
                (u, client) => client.IsDisposed ? Create(u) : client);
        }

        /// <summary>
        /// Defines a strategy for getting a cache key based on a Url. Default implementation
        /// returns the host part (i.e www.api.com) so that all calls to the same host use the
        /// same FlurlClient (and HttpClient/HttpMessageHandler) instance.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>The cache key</returns>
        protected abstract string GetCacheKey(FlurlModel url);

        /// <summary>
        /// Creates a new FlurlClient
        /// </summary>
        /// <param name="url">The URL (not used)</param>
        /// <returns></returns>
        protected virtual IFlurlClient Create(FlurlModel url) => new FlurlClient();

        /// <summary>
        /// Disposes all cached IFlurlClient instances and clears the cache.
        /// </summary>
        public void Dispose()
        {
            foreach (var kv in _clients)
            {
                if (!kv.Value.IsDisposed)
                    kv.Value.Dispose();
            }
            _clients.Clear();
        }

        /// <summary>
        /// Override in custom factory to customize the creation of HttpClient used in all Flurl HTTP calls
        /// (except when one is passed explicitly to the FlurlClient constructor). In order not to lose
        /// Flurl.Http functionality, it is recommended to call base.CreateClient and customize the result.
        /// </summary>
        public virtual HttpClient CreateHttpClient(HttpMessageHandler handler)
        {
            return new HttpClient(handler)
            {
                // Timeouts handled per request via FlurlHttpSettings.Timeout
                Timeout = HopperCompat.InfiniteTimeSpan //System.Threading.Timeout.InfiniteTimeSpan
            };
        }

        /// <summary>
        /// Override in custom factory to customize the creation of the top-level HttpMessageHandler used in all
        /// Flurl HTTP calls (except when an HttpClient is passed explicitly to the FlurlClient constructor).
        /// In order not to lose Flurl.Http functionality, it is recommended to call base.CreateMessageHandler, and
        /// either customize the returned HttpClientHandler, or set it as the InnerHandler of a DelegatingHandler.
        /// </summary>
        public virtual HttpMessageHandler CreateMessageHandler()
        {
            var httpClientHandler = new HttpClientHandler();

            // flurl has its own mechanisms for managing cookies and redirects

            try { httpClientHandler.UseCookies = false; }
            catch (PlatformNotSupportedException) { } // look out for WASM platforms (#543)

            if (httpClientHandler.SupportsRedirectConfiguration)
                httpClientHandler.AllowAutoRedirect = false;

            if (httpClientHandler.SupportsAutomaticDecompression)
            {
                // #266
                // deflate not working? see #474
                httpClientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }
            return httpClientHandler;
        }
    }
    /// <summary>
    /// Corresponds to the possible values of the SameSite attribute of the Set-Cookie header.
    /// </summary>
    public enum SameSite
    {
        /// <summary>
        /// Indicates a browser should only send cookie for same-site requests.
        /// </summary>
        Strict,
        /// <summary>
        /// Indicates a browser should send cookie for cross-site requests only with top-level navigation. 
        /// </summary>
        Lax,
        /// <summary>
        /// Indicates a browser should send cookie for same-site and cross-site requests.
        /// </summary>
        None
    }

    /// <summary>
    /// Represents an HTTP cookie. Closely matches Set-Cookie response header.
    /// </summary>
    public class FlurlCookie
    {
        private string _value;
        private DateTimeOffset? _expires;
        private int? _maxAge;
        private string _domain;
        private string _path;
        private bool _secure;
        private bool _httpOnly;
        private SameSite? _sameSite;

        private bool _locked;

        /// <summary>
        /// Creates a new FlurlCookie.
        /// </summary>
        /// <param name="name">Name of the cookie.</param>
        /// <param name="value">Value of the cookie.</param>
        /// <param name="originUrl">URL of request that sent the original Set-Cookie header.</param>
        /// <param name="dateReceived">Date/time that original Set-Cookie header was received. Defaults to current date/time. Important for Max-Age to be enforced correctly.</param>
        public FlurlCookie(string name, string value, string originUrl = null, DateTimeOffset? dateReceived = null)
        {
            Name = name;
            Value = value;
            OriginUrl = originUrl;
            DateReceived = dateReceived ?? DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// The URL that originally sent the Set-Cookie response header. If adding to a CookieJar, this is required unless
        /// both Domain AND Path are specified.
        /// </summary>
        public FlurlModel OriginUrl { get; }

        /// <summary>
        /// Date and time the cookie was received. Defaults to date/time this FlurlCookie was created.
        /// Important for Max-Age to be enforced correctly.
        /// </summary>
        public DateTimeOffset DateReceived { get; }

        /// <summary>
        /// The cookie name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The cookie value.
        /// </summary>
        public string Value
        {
            get => _value;
            set => Update(ref _value, value);
        }

        /// <summary>
        /// Corresponds to the Expires attribute of the Set-Cookie header.
        /// </summary>
        public DateTimeOffset? Expires
        {
            get => _expires;
            set => Update(ref _expires, value);
        }

        /// <summary>
        /// Corresponds to the Max-Age attribute of the Set-Cookie header.
        /// </summary>
        public int? MaxAge
        {
            get => _maxAge;
            set => Update(ref _maxAge, value);
        }

        /// <summary>
        /// Corresponds to the Domain attribute of the Set-Cookie header.
        /// </summary>
        public string Domain
        {
            get => _domain;
            set => Update(ref _domain, value);
        }

        /// <summary>
        /// Corresponds to the Path attribute of the Set-Cookie header.
        /// </summary>
        public string Path
        {
            get => _path;
            set => Update(ref _path, value);
        }

        /// <summary>
        /// Corresponds to the Secure attribute of the Set-Cookie header.
        /// </summary>
        public bool Secure
        {
            get => _secure;
            set => Update(ref _secure, value);
        }

        /// <summary>
        /// Corresponds to the HttpOnly attribute of the Set-Cookie header.
        /// </summary>
        public bool HttpOnly
        {
            get => _httpOnly;
            set => Update(ref _httpOnly, value);
        }

        /// <summary>
        /// Corresponds to the SameSite attribute of the Set-Cookie header.
        /// </summary>
        public SameSite? SameSite
        {
            get => _sameSite;
            set => Update(ref _sameSite, value);
        }

        /// <summary>
        /// Generates a key based on cookie Name, Domain, and Path (using OriginalUrl in the absence of Domain/Path).
        /// Used by CookieJar to determine whether to add a cookie or update an existing one.
        /// </summary>
        public string GetKey()
        {
            var domain = string.IsNullOrEmpty(Domain) ? "*" + OriginUrl.Host : Domain;
            var path = string.IsNullOrEmpty(Path) ? OriginUrl.Path : Path;
            if (path.Length == 0) path = "/";
            return $"{domain}{path}:{Name}";
        }

        /// <summary>
        /// Makes this cookie immutable. Call when added to a jar.
        /// </summary>
        internal void Lock()
        {
            _locked = true;
        }

        private void Update<T>(ref T field, T newVal, [CallerMemberName] string propName = null)
        {
            // == throws with generics (strangely), and .Equals needs a null check. Jon Skeet to the rescue.
            // https://stackoverflow.com/a/390974/62600
            if (EqualityComparer<T>.Default.Equals(field, newVal))
                return;

            if (_locked)
                throw new Exception("After a cookie has been added to a CookieJar, it becomes immutable and cannot be changed.");

            field = newVal;
        }
    }
    /// <summary>
    /// A static container for global configuration settings affecting Flurl.Http behavior.
    /// </summary>
    public static class FlurlHttp
    {
        private static readonly object _configLock = new object();

        private static Lazy<GlobalFlurlHttpSettings> _settings =
            new Lazy<GlobalFlurlHttpSettings>(() => new GlobalFlurlHttpSettings());

        /// <summary>
        /// Globally configured Flurl.Http settings. Should normally be written to by calling FlurlHttp.Configure once application at startup.
        /// </summary>
        public static GlobalFlurlHttpSettings GlobalSettings => _settings.Value;

        /// <summary>
        /// Provides thread-safe access to Flurl.Http's global configuration settings. Should only be called once at application startup.
        /// </summary>
        /// <param name="configAction">the action to perform against the GlobalSettings.</param>
        public static void Configure(Action<GlobalFlurlHttpSettings> configAction)
        {
            lock (_configLock)
            {
                configAction(GlobalSettings);
            }
        }

        /// <summary>
        /// Provides thread-safe access to a specific IFlurlClient, typically to configure settings and default headers.
        /// The URL is used to find the client, but keep in mind that the same client will be used in all calls to the same host by default.
        /// </summary>
        /// <param name="url">the URL used to find the IFlurlClient.</param>
        /// <param name="configAction">the action to perform against the IFlurlClient.</param>
        public static void ConfigureClient(string url, Action<IFlurlClient> configAction) =>
            GlobalSettings.FlurlClientFactory.ConfigureClient(url, configAction);
    }
    /// <summary>
    /// An exception that is thrown when an HTTP call made by Flurl.Http fails, including when the response
    /// indicates an unsuccessful HTTP status code.
    /// </summary>
    public class FlurlHttpException : Exception
    {
        /// <summary>
        /// An object containing details about the failed HTTP call
        /// </summary>
        public FlurlCall Call { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlurlHttpException"/> class.
        /// </summary>
        /// <param name="call">The call.</param>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public FlurlHttpException(FlurlCall call, string message, Exception inner) : base(message, inner)
        {
            Call = call;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlurlHttpException"/> class.
        /// </summary>
        /// <param name="call">The call.</param>
        /// <param name="inner">The inner.</param>
        public FlurlHttpException(FlurlCall call, Exception inner) : this(call, BuildMessage(call, inner), inner) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlurlHttpException"/> class.
        /// </summary>
        /// <param name="call">The call.</param>
        public FlurlHttpException(FlurlCall call) : this(call, BuildMessage(call, null), null) { }

        private static string BuildMessage(FlurlCall call, Exception inner)
        {
            if (call?.Response != null && !call.Succeeded)
                return $"Call failed with status code {call.Response.StatusCode} ({call.HttpResponseMessage.ReasonPhrase}): {call}";

            var msg = "Call failed";
            if (inner != null) msg += ". " + inner.Message.TrimEnd('.');
            return msg + ((call == null) ? "." : $": {call}");
        }

        /// <summary>
        /// Gets the HTTP status code of the response if a response was received, otherwise null.
        /// </summary>
        public int? StatusCode => Call?.Response?.StatusCode;

        /// <summary>
        /// Gets the response body of the failed call.
        /// </summary>
        /// <returns>A task whose result is the string contents of the response body.</returns>
        public Task<string> GetResponseStringAsync() => Call?.Response?.GetStringAsync() ?? TestTry.TaskFromResult((string)null);

        /// <summary>
        /// Deserializes the JSON response body to an object of the given type.
        /// </summary>
        /// <typeparam name="T">A type whose structure matches the expected JSON response.</typeparam>
        /// <returns>A task whose result is an object containing data in the response body.</returns>
        public Task<T> GetResponseJsonAsync<T>() => Call?.Response?.GetJsonAsync<T>() ?? TestTry.TaskFromResult(default(T));
    }

    /// <summary>
    /// An exception that is thrown when an HTTP call made by Flurl.Http times out.
    /// </summary>
    public class FlurlHttpTimeoutException : FlurlHttpException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlurlHttpTimeoutException"/> class.
        /// </summary>
        /// <param name="call">Details of the HTTP call that caused the exception.</param>
        /// <param name="inner">The inner exception.</param>
        public FlurlHttpTimeoutException(FlurlCall call, Exception inner) : base(call, BuildMessage(call), inner) { }

        private static string BuildMessage(FlurlCall call) =>
            (call == null) ? "Call timed out." : $"Call timed out: {call}";
    }

    /// <summary>
    /// An exception that is thrown when an HTTP response could not be parsed to a particular format.
    /// </summary>
    public class FlurlParsingException : FlurlHttpException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlurlParsingException"/> class.
        /// </summary>
        /// <param name="call">Details of the HTTP call that caused the exception.</param>
        /// <param name="expectedFormat">The format that could not be parsed to, i.e. JSON.</param>
        /// <param name="inner">The inner exception.</param>
        public FlurlParsingException(FlurlCall call, string expectedFormat, Exception inner) : base(call, BuildMessage(call, expectedFormat), inner)
        {
            ExpectedFormat = expectedFormat;
        }

        /// <summary>
        /// The format that could not be parsed to, i.e. JSON.
        /// </summary>
        public string ExpectedFormat { get; }

        private static string BuildMessage(FlurlCall call, string expectedFormat)
        {
            var msg = $"Response could not be deserialized to {expectedFormat}";
            return msg + ((call == null) ? "." : $": {call}");
        }
    }
    /// <summary>
    /// A set of properties that affect Flurl.Http behavior
    /// </summary>
    public class FlurlHttpSettings
    {
        // Values are dictionary-backed so we can check for key existence. Can't do null-coalescing
        // because if a setting is set to null at the request level, that should stick.
        private readonly IDictionary<string, object> _vals = new Dictionary<string, object>();

        private FlurlHttpSettings _defaults;

        /// <summary>
        /// Creates a new FlurlHttpSettings object.
        /// </summary>
        public FlurlHttpSettings()
        {
            Redirects = new RedirectSettings(this);
            ResetDefaults();
        }
        /// <summary>
        /// Gets or sets the default values to fall back on when values are not explicitly set on this instance.
        /// </summary>
        public virtual FlurlHttpSettings Defaults
        {
            get => _defaults ?? FlurlHttp.GlobalSettings;
            set => _defaults = value;
        }

        /// <summary>
        /// Gets or sets the HTTP request timeout.
        /// </summary>
        public TimeSpan? Timeout
        {
            get => Get<TimeSpan?>();
            set => Set(value);
        }

        /// <summary>
        /// Gets or sets a pattern representing a range of HTTP status codes which (in addtion to 2xx) will NOT result in Flurl.Http throwing an Exception.
        /// Examples: "3xx", "100,300,600", "100-299,6xx", "*" (allow everything)
        /// 2xx will never throw regardless of this setting.
        /// </summary>
        public string AllowedHttpStatusRange
        {
            get => Get<string>();
            set => Set(value);
        }

        /// <summary>
        /// Gets or sets object used to serialize and deserialize JSON. Default implementation uses Newtonsoft Json.NET.
        /// </summary>
        public ISerializer JsonSerializer
        {
            get => Get<ISerializer>();
            set => Set(value);
        }

        /// <summary>
        /// Gets or sets object used to serialize URL-encoded data. (Deserialization not supported in default implementation.)
        /// </summary>
        public ISerializer UrlEncodedSerializer
        {
            get => Get<ISerializer>();
            set => Set(value);
        }

        /// <summary>
        /// Gets object whose properties describe how Flurl.Http should handle redirect (3xx) responses.
        /// </summary>
        public RedirectSettings Redirects { get; }

        /// <summary>
        /// Gets or sets a callback that is invoked immediately before every HTTP request is sent.
        /// </summary>
        public Action<FlurlCall> BeforeCall
        {
            get => Get<Action<FlurlCall>>();
            set => Set(value);
        }

        /// <summary>
        /// Gets or sets a callback that is invoked asynchronously immediately before every HTTP request is sent.
        /// </summary>
        public Func<FlurlCall, Task> BeforeCallAsync
        {
            get => Get<Func<FlurlCall, Task>>();
            set => Set(value);
        }

        /// <summary>
        /// Gets or sets a callback that is invoked immediately after every HTTP response is received.
        /// </summary>
        public Action<FlurlCall> AfterCall
        {
            get => Get<Action<FlurlCall>>();
            set => Set(value);
        }

        /// <summary>
        /// Gets or sets a callback that is invoked asynchronously immediately after every HTTP response is received.
        /// </summary>
        public Func<FlurlCall, Task> AfterCallAsync
        {
            get => Get<Func<FlurlCall, Task>>();
            set => Set(value);
        }

        /// <summary>
        /// Gets or sets a callback that is invoked when an error occurs during any HTTP call, including when any non-success
        /// HTTP status code is returned in the response. Response should be null-checked if used in the event handler.
        /// </summary>
        public Action<FlurlCall> OnError
        {
            get => Get<Action<FlurlCall>>();
            set => Set(value);
        }

        /// <summary>
        /// Gets or sets a callback that is invoked asynchronously when an error occurs during any HTTP call, including when any non-success
        /// HTTP status code is returned in the response. Response should be null-checked if used in the event handler.
        /// </summary>
        public Func<FlurlCall, Task> OnErrorAsync
        {
            get => Get<Func<FlurlCall, Task>>();
            set => Set(value);
        }

        /// <summary>
        /// Gets or sets a callback that is invoked when any 3xx response with a Location header is received.
        /// You can inspect/manipulate the call.Redirect object to determine what will happen next.
        /// An auto-redirect will only happen if call.Redirect.Follow is true upon exiting the callback.
        /// </summary>
        public Action<FlurlCall> OnRedirect
        {
            get => Get<Action<FlurlCall>>();
            set => Set(value);
        }

        /// <summary>
        /// Gets or sets a callback that is invoked asynchronously when any 3xx response with a Location header is received.
        /// You can inspect/manipulate the call.Redirect object to determine what will happen next.
        /// An auto-redirect will only happen if call.Redirect.Follow is true upon exiting the callback.
        /// </summary>
        public Func<FlurlCall, Task> OnRedirectAsync
        {
            get => Get<Func<FlurlCall, Task>>();
            set => Set(value);
        }

        /// <summary>
        /// Resets all overridden settings to their default values. For example, on a FlurlRequest,
        /// all settings are reset to FlurlClient-level settings.
        /// </summary>
        public virtual void ResetDefaults()
        {
            _vals.Clear();
        }

        /// <summary>
        /// Gets a settings value from this instance if explicitly set, otherwise from the default settings that back this instance.
        /// </summary>
        internal T Get<T>([CallerMemberName] string propName = null)
        {
            var testVals = HttpTest.Current?.Settings._vals;
            return
                testVals?.ContainsKey(propName) == true ? (T)testVals[propName] :
                _vals.ContainsKey(propName) ? (T)_vals[propName] :
                Defaults != null ? (T)Defaults.Get<T>(propName) :
                default;
        }

        /// <summary>
        /// Sets a settings value for this instance.
        /// </summary>
        internal void Set<T>(T value, [CallerMemberName] string propName = null)
        {
            _vals[propName] = value;
        }
    }

    /// <summary>
    /// Global default settings for Flurl.Http
    /// </summary>
    public class GlobalFlurlHttpSettings : FlurlHttpSettings
    {
        internal GlobalFlurlHttpSettings()
        {
            ResetDefaults();
        }

        /// <summary>
        /// Defaults at the global level do not make sense and will always be null.
        /// </summary>
        public override FlurlHttpSettings Defaults
        {
            get => null;
            set => throw new Exception("Global settings cannot be backed by any higher-level defauts.");
        }

        /// <summary>
        /// Gets or sets the factory that defines creating, caching, and reusing FlurlClient instances and,
        /// by proxy, HttpClient instances.
        /// </summary>
        public IFlurlClientFactory FlurlClientFactory
        {
            get => Get<IFlurlClientFactory>();
            set => Set(value);
        }

        /// <summary>
        /// Resets all global settings to their default values.
        /// </summary>
        public override void ResetDefaults()
        {
            base.ResetDefaults();
            Timeout = TimeSpan.FromSeconds(100); // same as HttpClient
            JsonSerializer = new DefaultJsonSerializer();
            UrlEncodedSerializer = new DefaultUrlEncodedSerializer();
            FlurlClientFactory = new DefaultFlurlClientFactory();
            Redirects.Enabled = true;
            Redirects.AllowSecureToInsecure = false;
            Redirects.ForwardHeaders = false;
            Redirects.ForwardAuthorizationHeader = false;
            Redirects.MaxAutoRedirects = 10;
        }
    }
    /// <summary>
    /// Represents an HTTP request. Can be created explicitly via new FlurlRequest(), fluently via FlurlModel.Request(),
    /// or implicitly when a call is made via methods like FlurlModel.GetAsync().
    /// </summary>
    public interface IFlurlRequest : IHttpSettingsContainer
    {
        /// <summary>
        /// Gets or sets the IFlurlClient to use when sending the request.
        /// </summary>
        IFlurlClient Client { get; set; }

        /// <summary>
        /// Gets or sets the HTTP method of the request. Normally you don't need to set this explicitly; it will be set
        /// when you call the sending method, such as GetAsync, PostAsync, etc.
        /// </summary>
        HttpMethod Verb { get; set; }

        /// <summary>
        /// Gets or sets the URL to be called.
        /// </summary>
        FlurlModel Url { get; set; }

        /// <summary>
        /// The body content of this request.
        /// </summary>
        HttpContent Content { get; set; }

        /// <summary>
        /// Gets Name/Value pairs parsed from the Cookie request header.
        /// </summary>
        IEnumerable<(string Name, string Value)> Cookies { get; }

        /// <summary>
        /// Gets or sets the collection of HTTP cookies that can be shared between multiple requests. When set, values that
        /// should be sent with this request (based on Domain, Path, and other rules) are immediately copied to the Cookie
        /// request header, and any Set-Cookie headers received in the response will be written to the CookieJar.
        /// </summary>
        CookieJar CookieJar { get; set; }

        /// <summary>
        /// The FlurlCall that received a 3xx response and automatically triggered this request.
        /// </summary>
        FlurlCall RedirectedFrom { get; set; }

        /// <summary>
        /// Asynchronously sends the HTTP request. Mainly used to implement higher-level extension methods (GetJsonAsync, etc).
        /// </summary>
        /// <param name="verb">The HTTP method used to make the request.</param>
        /// <param name="content">Contents of the request body.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        Task<IFlurlResponse> SendAsync(HttpMethod verb, HttpContent content = null, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default);
    }

    /// <inheritdoc />
    public class FlurlRequest : IFlurlRequest
    {
        private IFlurlClient _client;
        private CookieJar _jar;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlurlRequest"/> class.
        /// </summary>
        /// <param name="url">The URL to call with this FlurlRequest instance.</param>
        public FlurlRequest(FlurlModel url = null)
        {
            Url = url;
        }

        /// <summary>
        /// Used internally by FlurlClient.Request
        /// </summary>
        internal FlurlRequest(IFlurlClient client, params object[] urlSegments) : this(client?.BaseUrl, urlSegments)
        {
            Client = client;
        }

        /// <summary>
        /// Used internally by FlurlClient.Request and CookieSession.Request
        /// </summary>
        internal FlurlRequest(string baseUrl, params object[] urlSegments)
        {
            var parts = new List<string>(urlSegments.Select(s => s.ToInvariantString()));
            if (!FlurlModel.IsValid(parts.FirstOrDefault()) && !string.IsNullOrEmpty(baseUrl))
                parts.Insert(0, baseUrl);

            if (parts.Any())
                Url = FlurlModel.Combine(parts.ToArray());
        }

        /// <inheritdoc />
        public FlurlHttpSettings Settings { get; } = new FlurlHttpSettings();

        /// <inheritdoc />
        public IFlurlClient Client
        {
            get => _client;
            set
            {
                _client = value;
                Settings.Defaults = _client?.Settings;
            }
        }

        /// <inheritdoc />
        public HttpMethod Verb { get; set; }

        /// <inheritdoc />
        public FlurlModel Url { get; set; }

        /// <inheritdoc />
        public HttpContent Content { get; set; }

        /// <inheritdoc />
        public FlurlCall RedirectedFrom { get; set; }

        /// <inheritdoc />
        public INameValueList<string> Headers { get; } = new NameValueList<string>(false); // header names are case-insensitive https://stackoverflow.com/a/5259004/62600

        /// <inheritdoc />
        public IEnumerable<(string Name, string Value)> Cookies =>
            CookieCutter.ParseRequestHeader(Headers.FirstOrDefault("Cookie"));

        /// <inheritdoc />
        public CookieJar CookieJar
        {
            get => _jar;
            set => ApplyCookieJar(value);
        }

        /// <inheritdoc />
        public Task<IFlurlResponse> SendAsync(HttpMethod verb, HttpContent content = null, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            Verb = verb;
            Content = content;
            Client ??= FlurlHttp.GlobalSettings.FlurlClientFactory.Get(Url);
            return Client.SendAsync(this, completionOption, cancellationToken);
        }

        private void ApplyCookieJar(CookieJar jar)
        {
            _jar = jar;
            if (jar == null)
                return;

            this.WithCookies(
                from c in CookieJar
                where c.ShouldSendTo(this.Url, out _)
                // sort by longest path, then earliest creation time, per #2: https://tools.ietf.org/html/rfc6265#section-5.4
                orderby (c.Path ?? c.OriginUrl.Path).Length descending, c.DateReceived
                select (c.Name, c.Value));
        }
    }
    /// <summary>
    /// Represents an HTTP response.
    /// </summary>
    public interface IFlurlResponse : IDisposable
    {
        /// <summary>
        /// Gets the collection of response headers received.
        /// </summary>
        IReadOnlyNameValueList<string> Headers { get; }

        /// <summary>
        /// Gets the collection of HTTP cookies received in this response via Set-Cookie headers.
        /// </summary>
        IReadOnlyList<FlurlCookie> Cookies { get; }

        /// <summary>
        /// Gets the raw HttpResponseMessage that this IFlurlResponse wraps.
        /// </summary>
        HttpResponseMessage ResponseMessage { get; }

        /// <summary>
        /// Gets the HTTP status code of the response.
        /// </summary>
        int StatusCode { get; }

        /// <summary>
        /// Deserializes JSON-formatted HTTP response body to object of type T.
        /// </summary>
        /// <typeparam name="T">A type whose structure matches the expected JSON response.</typeparam>
        /// <returns>A Task whose result is an object containing data in the response body.</returns>
        /// <example>x = await url.PostAsync(data).GetJson&lt;T&gt;()</example>
        /// <exception cref="FlurlHttpException">Condition.</exception>
        Task<T> GetJsonAsync<T>();

        /// <summary>
        /// Returns HTTP response body as a string.
        /// </summary>
        /// <returns>A Task whose result is the response body as a string.</returns>
        /// <example>s = await url.PostAsync(data).GetString()</example>
        Task<string> GetStringAsync();

        /// <summary>
        /// Returns HTTP response body as a stream.
        /// </summary>
        /// <returns>A Task whose result is the response body as a stream.</returns>
        /// <example>stream = await url.PostAsync(data).GetStream()</example>
        Task<Stream> GetStreamAsync();

        /// <summary>
        /// Returns HTTP response body as a byte array.
        /// </summary>
        /// <returns>A Task whose result is the response body as a byte array.</returns>
        /// <example>bytes = await url.PostAsync(data).GetBytes()</example>
        Task<byte[]> GetBytesAsync();
    }

    /// <inheritdoc />
    public class FlurlResponse : IFlurlResponse
    {
        private readonly FlurlCall _call;
        private readonly Lazy<IReadOnlyNameValueList<string>> _headers;
        private readonly Lazy<IReadOnlyList<FlurlCookie>> _cookies;
        private object _capturedBody = null;
        private bool _streamRead = false;
        private ISerializer _serializer = null;

        /// <inheritdoc />
        public IReadOnlyNameValueList<string> Headers => _headers.Value;

        /// <inheritdoc />
        public IReadOnlyList<FlurlCookie> Cookies => _cookies.Value;

        /// <inheritdoc />
        public HttpResponseMessage ResponseMessage => _call.HttpResponseMessage;

        /// <inheritdoc />
        public int StatusCode => (int)ResponseMessage.StatusCode;

        /// <summary>
        /// Creates a new FlurlResponse that wraps the give HttpResponseMessage.
        /// </summary>
        public FlurlResponse(FlurlCall call, CookieJar cookieJar = null)
        {
            _call = call;
            _headers = new Lazy<IReadOnlyNameValueList<string>>(LoadHeaders);
            _cookies = new Lazy<IReadOnlyList<FlurlCookie>>(LoadCookies);
            LoadCookieJar(cookieJar);
        }

        private IReadOnlyNameValueList<string> LoadHeaders()
        {
            var result = new NameValueList<string>(false);

            foreach (var h in ResponseMessage.Headers)
                foreach (var v in h.Value)
                    result.Add(h.Key, v);

            if (ResponseMessage.Content?.Headers == null)
                return result;

            foreach (var h in ResponseMessage.Content.Headers)
                foreach (var v in h.Value)
                    result.Add(h.Key, v);

            return result;
        }

        private IReadOnlyList<FlurlCookie> LoadCookies()
        {
            var url = ResponseMessage.RequestMessage.RequestUri.AbsoluteUri;
#if NET40
            return new EReadOnlyCollection<FlurlCookie>(ResponseMessage.Headers.TryGetValues("Set-Cookie", out var headerValues) ?
                headerValues.Select(hv => CookieCutter.ParseResponseHeader(url, hv)).ToList() :
                new List<FlurlCookie>());
#else
			return ResponseMessage.Headers.TryGetValues("Set-Cookie", out var headerValues) ?
				headerValues.Select(hv => CookieCutter.ParseResponseHeader(url, hv)).ToList() :
				new List<FlurlCookie>();
#endif
        }

        private void LoadCookieJar(CookieJar jar)
        {
            if (jar == null) return;
            foreach (var cookie in Cookies)
                jar.TryAddOrReplace(cookie, out _); // not added if cookie fails validation
        }

        /// <inheritdoc />
        public async Task<T> GetJsonAsync<T>()
        {
            if (_streamRead)
            {
                if (_capturedBody == null) return default;
                if (_capturedBody is T body) return body;
            }

            _serializer ??= _call.Request.Settings.JsonSerializer;

            try
            {
                if (_streamRead)
                {
                    // Stream was read but captured as a different type than T. If it was captured as a string,
                    // we should be in good shape. If it was deserialized to a different type, the best we can
                    // do is serialize it and then deserialize to T, and we could lose data. But that's a very
                    // uncommon scenario, hopefully. https://github.com/tmenier/Flurl/issues/571#issuecomment-881712479
                    var s = _capturedBody as string ?? _serializer.Serialize(_capturedBody);
                    _capturedBody = _serializer.Deserialize<T>(s);
                }
                else
                {
                    using var stream = await ResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
                    _capturedBody = _serializer.Deserialize<T>(stream);
                }
                return (T)_capturedBody;
            }
            catch (Exception ex)
            {
                _serializer = null;
                _capturedBody = await ResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                _streamRead = true;
                await FlurlClient.HandleExceptionAsync(_call, new FlurlParsingException(_call, "JSON", ex), CancellationToken.None).ConfigureAwait(false);
                return default;
            }
            finally
            {
                _streamRead = true;
            }
        }

        /// <inheritdoc />
        public async Task<string> GetStringAsync()
        {
            if (_streamRead)
            {
                return
                    (_capturedBody == null) ? null :
                    // if GetJsonAsync<T> was called, we streamed the response directly to a T (for memory efficiency)
                    // without first capturing a string. it's too late to get it, so the best we can do is serialize the T
                    (_serializer != null) ? _serializer.Serialize(_capturedBody) :
                    _capturedBody?.ToString();
            }

            // fixes #606. also verified that HttpClient.GetStringAsync returns empty string when Content is null.
            if (ResponseMessage.Content == null)
                return "";

#if NETSTANDARD2_0
			// https://stackoverflow.com/questions/46119872/encoding-issues-with-net-core-2 (#86)
			System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
#endif
            // strip quotes from charset so .NET doesn't choke on them
            // https://github.com/dotnet/corefx/issues/5014
            // https://github.com/tmenier/Flurl/pull/76
            var ct = ResponseMessage.Content.Headers?.ContentType;
            if (ct?.CharSet != null)
                ct.CharSet = ct.CharSet.StripQuotes();

            _capturedBody = await ResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            _streamRead = true;
            return (string)_capturedBody;
        }

        /// <inheritdoc />
        public Task<Stream> GetStreamAsync()
        {
            _streamRead = true;
            return ResponseMessage.Content.ReadAsStreamAsync();
        }

        /// <inheritdoc />
        public async Task<byte[]> GetBytesAsync()
        {
            if (_streamRead)
                return _capturedBody as byte[];

            _capturedBody = await ResponseMessage.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            _streamRead = true;
            return (byte[])_capturedBody;
        }

        /// <summary>
        /// Disposes the underlying HttpResponseMessage.
        /// </summary>
        public void Dispose() => ResponseMessage.Dispose();
    }
    /// <summary>
    /// Fluent extension methods on String, FlurlModel, Uri, and IFlurlRequest.
    /// </summary>
    public static partial class GeneratedExtensions
    {
        /// <summary>
        /// Sends an asynchronous request.
        /// </summary>
        /// <param name="request">This IFlurlRequest</param>
        /// <param name="verb">The HTTP verb used to make the request.</param>
        /// <param name="body">An object representing the request body, which will be serialized to JSON.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> SendJsonAsync(this IFlurlRequest request, HttpMethod verb, object body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            var content = new CapturedJsonContent(request.Settings.JsonSerializer.Serialize(body));
            return request.SendAsync(verb, content, completionOption, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous request.
        /// </summary>
        /// <param name="request">This IFlurlRequest</param>
        /// <param name="verb">The HTTP verb used to make the request.</param>
        /// <param name="body">The request body.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> SendStringAsync(this IFlurlRequest request, HttpMethod verb, string body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            var content = new CapturedStringContent(body);
            return request.SendAsync(verb, content, completionOption, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous request.
        /// </summary>
        /// <param name="request">This IFlurlRequest</param>
        /// <param name="verb">The HTTP verb used to make the request.</param>
        /// <param name="body">An object representing the request body, which will be serialized to a URL-encoded string.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> SendUrlEncodedAsync(this IFlurlRequest request, HttpMethod verb, object body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            var content = new CapturedUrlEncodedContent(request.Settings.UrlEncodedSerializer.Serialize(body));
            return request.SendAsync(verb, content, completionOption, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous GET request.
        /// </summary>
        /// <param name="request">This IFlurlRequest</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> GetAsync(this IFlurlRequest request, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return request.SendAsync(HttpMethod.Get, null, completionOption, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous GET request.
        /// </summary>
        /// <param name="request">This IFlurlRequest</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the JSON response body deserialized to an object of type T.</returns>
        public static Task<T> GetJsonAsync<T>(this IFlurlRequest request, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return request.SendAsync(HttpMethod.Get, null, completionOption, cancellationToken).ReceiveJson<T>();
        }

        /// <summary>
        /// Sends an asynchronous GET request.
        /// </summary>
        /// <param name="request">This IFlurlRequest</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the response body as a string.</returns>
        public static Task<string> GetStringAsync(this IFlurlRequest request, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return request.SendAsync(HttpMethod.Get, null, completionOption, cancellationToken).ReceiveString();
        }

        /// <summary>
        /// Sends an asynchronous GET request.
        /// </summary>
        /// <param name="request">This IFlurlRequest</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the response body as a Stream.</returns>
        public static Task<Stream> GetStreamAsync(this IFlurlRequest request, HttpCompletionOption completionOption = HttpCompletionOption.ResponseHeadersRead, CancellationToken cancellationToken = default)
        {
            return request.SendAsync(HttpMethod.Get, null, completionOption, cancellationToken).ReceiveStream();
        }

        /// <summary>
        /// Sends an asynchronous GET request.
        /// </summary>
        /// <param name="request">This IFlurlRequest</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the response body as a byte array.</returns>
        public static Task<byte[]> GetBytesAsync(this IFlurlRequest request, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return request.SendAsync(HttpMethod.Get, null, completionOption, cancellationToken).ReceiveBytes();
        }

        /// <summary>
        /// Sends an asynchronous POST request.
        /// </summary>
        /// <param name="request">This IFlurlRequest</param>
        /// <param name="content">The request body content.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PostAsync(this IFlurlRequest request, HttpContent content = null, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return request.SendAsync(HttpMethod.Post, content, completionOption, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous POST request.
        /// </summary>
        /// <param name="request">This IFlurlRequest</param>
        /// <param name="body">An object representing the request body, which will be serialized to JSON.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PostJsonAsync(this IFlurlRequest request, object body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            var content = new CapturedJsonContent(request.Settings.JsonSerializer.Serialize(body));
            return request.SendAsync(HttpMethod.Post, content, completionOption, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous POST request.
        /// </summary>
        /// <param name="request">This IFlurlRequest</param>
        /// <param name="body">The request body.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PostStringAsync(this IFlurlRequest request, string body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            var content = new CapturedStringContent(body);
            return request.SendAsync(HttpMethod.Post, content, completionOption, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous POST request.
        /// </summary>
        /// <param name="request">This IFlurlRequest</param>
        /// <param name="body">An object representing the request body, which will be serialized to a URL-encoded string.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PostUrlEncodedAsync(this IFlurlRequest request, object body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            var content = new CapturedUrlEncodedContent(request.Settings.UrlEncodedSerializer.Serialize(body));
            return request.SendAsync(HttpMethod.Post, content, completionOption, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous HEAD request.
        /// </summary>
        /// <param name="request">This IFlurlRequest</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> HeadAsync(this IFlurlRequest request, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return request.SendAsync(HttpMethod.Head, null, completionOption, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous PUT request.
        /// </summary>
        /// <param name="request">This IFlurlRequest</param>
        /// <param name="content">The request body content.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PutAsync(this IFlurlRequest request, HttpContent content = null, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return request.SendAsync(HttpMethod.Put, content, completionOption, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous PUT request.
        /// </summary>
        /// <param name="request">This IFlurlRequest</param>
        /// <param name="body">An object representing the request body, which will be serialized to JSON.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PutJsonAsync(this IFlurlRequest request, object body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            var content = new CapturedJsonContent(request.Settings.JsonSerializer.Serialize(body));
            return request.SendAsync(HttpMethod.Put, content, completionOption, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous PUT request.
        /// </summary>
        /// <param name="request">This IFlurlRequest</param>
        /// <param name="body">The request body.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PutStringAsync(this IFlurlRequest request, string body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            var content = new CapturedStringContent(body);
            return request.SendAsync(HttpMethod.Put, content, completionOption, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous DELETE request.
        /// </summary>
        /// <param name="request">This IFlurlRequest</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> DeleteAsync(this IFlurlRequest request, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return request.SendAsync(HttpMethod.Delete, null, completionOption, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous PATCH request.
        /// </summary>
        /// <param name="request">This IFlurlRequest</param>
        /// <param name="content">The request body content.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PatchAsync(this IFlurlRequest request, HttpContent content = null, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return request.SendAsync(new HttpMethod("PATCH"), content, completionOption, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous PATCH request.
        /// </summary>
        /// <param name="request">This IFlurlRequest</param>
        /// <param name="body">An object representing the request body, which will be serialized to JSON.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PatchJsonAsync(this IFlurlRequest request, object body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            var content = new CapturedJsonContent(request.Settings.JsonSerializer.Serialize(body));
            return request.SendAsync(new HttpMethod("PATCH"), content, completionOption, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous PATCH request.
        /// </summary>
        /// <param name="request">This IFlurlRequest</param>
        /// <param name="body">The request body.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PatchStringAsync(this IFlurlRequest request, string body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            var content = new CapturedStringContent(body);
            return request.SendAsync(new HttpMethod("PATCH"), content, completionOption, cancellationToken);
        }

        /// <summary>
        /// Sends an asynchronous OPTIONS request.
        /// </summary>
        /// <param name="request">This IFlurlRequest</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> OptionsAsync(this IFlurlRequest request, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return request.SendAsync(HttpMethod.Options, null, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous request.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="verb">The HTTP verb used to make the request.</param>
        /// <param name="content">The request body content.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> SendAsync(this FlurlModel url, HttpMethod verb, HttpContent content = null, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).SendAsync(verb, content, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous request.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="verb">The HTTP verb used to make the request.</param>
        /// <param name="body">An object representing the request body, which will be serialized to JSON.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> SendJsonAsync(this FlurlModel url, HttpMethod verb, object body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).SendJsonAsync(verb, body, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous request.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="verb">The HTTP verb used to make the request.</param>
        /// <param name="body">The request body.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> SendStringAsync(this FlurlModel url, HttpMethod verb, string body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).SendStringAsync(verb, body, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous request.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="verb">The HTTP verb used to make the request.</param>
        /// <param name="body">An object representing the request body, which will be serialized to a URL-encoded string.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> SendUrlEncodedAsync(this FlurlModel url, HttpMethod verb, object body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).SendUrlEncodedAsync(verb, body, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous GET request.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> GetAsync(this FlurlModel url, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).GetAsync(completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous GET request.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the JSON response body deserialized to an object of type T.</returns>
        public static Task<T> GetJsonAsync<T>(this FlurlModel url, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).GetJsonAsync<T>(completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous GET request.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the response body as a string.</returns>
        public static Task<string> GetStringAsync(this FlurlModel url, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).GetStringAsync(completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous GET request.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the response body as a Stream.</returns>
        public static Task<Stream> GetStreamAsync(this FlurlModel url, HttpCompletionOption completionOption = HttpCompletionOption.ResponseHeadersRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).GetStreamAsync(completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous GET request.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the response body as a byte array.</returns>
        public static Task<byte[]> GetBytesAsync(this FlurlModel url, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).GetBytesAsync(completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous POST request.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="content">The request body content.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PostAsync(this FlurlModel url, HttpContent content = null, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).PostAsync(content, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous POST request.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="body">An object representing the request body, which will be serialized to JSON.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PostJsonAsync(this FlurlModel url, object body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).PostJsonAsync(body, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous POST request.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="body">The request body.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PostStringAsync(this FlurlModel url, string body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).PostStringAsync(body, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous POST request.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="body">An object representing the request body, which will be serialized to a URL-encoded string.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PostUrlEncodedAsync(this FlurlModel url, object body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).PostUrlEncodedAsync(body, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous HEAD request.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> HeadAsync(this FlurlModel url, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).HeadAsync(completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous PUT request.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="content">The request body content.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PutAsync(this FlurlModel url, HttpContent content = null, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).PutAsync(content, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous PUT request.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="body">An object representing the request body, which will be serialized to JSON.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PutJsonAsync(this FlurlModel url, object body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).PutJsonAsync(body, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous PUT request.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="body">The request body.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PutStringAsync(this FlurlModel url, string body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).PutStringAsync(body, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous DELETE request.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> DeleteAsync(this FlurlModel url, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).DeleteAsync(completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous PATCH request.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="content">The request body content.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PatchAsync(this FlurlModel url, HttpContent content = null, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).PatchAsync(content, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous PATCH request.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="body">An object representing the request body, which will be serialized to JSON.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PatchJsonAsync(this FlurlModel url, object body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).PatchJsonAsync(body, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous PATCH request.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="body">The request body.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PatchStringAsync(this FlurlModel url, string body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).PatchStringAsync(body, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous OPTIONS request.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> OptionsAsync(this FlurlModel url, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).OptionsAsync(completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a new FlurlRequest and asynchronously downloads a file.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="localFolderPath">Path of local folder where file is to be downloaded.</param>
        /// <param name="localFileName">Name of local file. If not specified, the source filename (last segment of the URL) is used.</param>
        /// <param name="bufferSize">Buffer size in bytes. Default is 4096.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the local path of the downloaded file.</returns>
        public static Task<string> DownloadFileAsync(this FlurlModel url, string localFolderPath, string localFileName = null, int bufferSize = 4096, HttpCompletionOption completionOption = HttpCompletionOption.ResponseHeadersRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).DownloadFileAsync(localFolderPath, localFileName, bufferSize, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous multipart/form-data POST request.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="buildContent">A delegate for building the content parts.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PostMultipartAsync(this FlurlModel url, Action<CapturedMultipartContent> buildContent, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).PostMultipartAsync(buildContent, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a new FlurlRequest and sets a request header.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="name">The header name.</param>
        /// <param name="value">The header value.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithHeader(this FlurlModel url, string name, object value)
        {
            return new FlurlRequest(url).WithHeader(name, value);
        }

        /// <summary>
        /// Creates a new FlurlRequest and sets request headers based on property names/values of the provided object, or keys/values if object is a dictionary, to be sent.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="headers">Names/values of HTTP headers to set. Typically an anonymous object or IDictionary.</param>
        /// <param name="replaceUnderscoreWithHyphen">If true, underscores in property names will be replaced by hyphens. Default is true.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithHeaders(this FlurlModel url, object headers, bool replaceUnderscoreWithHyphen = true)
        {
            return new FlurlRequest(url).WithHeaders(headers, replaceUnderscoreWithHyphen);
        }

        /// <summary>
        /// Creates a new FlurlRequest and sets the Authorization header according to Basic Authentication protocol.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="username">Username of authenticating user.</param>
        /// <param name="password">Password of authenticating user.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithBasicAuth(this FlurlModel url, string username, string password)
        {
            return new FlurlRequest(url).WithBasicAuth(username, password);
        }

        /// <summary>
        /// Creates a new FlurlRequest and sets the Authorization header with a bearer token according to OAuth 2.0 specification.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="token">The acquired oAuth bearer token.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithOAuthBearerToken(this FlurlModel url, string token)
        {
            return new FlurlRequest(url).WithOAuthBearerToken(token);
        }

        /// <summary>
        /// Creates a new FlurlRequest and adds a name-value pair to its Cookie header. To automatically maintain a cookie "session", consider using a CookieJar or CookieSession instead.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="name">The cookie name.</param>
        /// <param name="value">The cookie value.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithCookie(this FlurlModel url, string name, object value)
        {
            return new FlurlRequest(url).WithCookie(name, value);
        }

        /// <summary>
        /// Creates a new FlurlRequest and adds name-value pairs to its Cookie header based on property names/values of the provided object, or keys/values if object is a dictionary. To automatically maintain a cookie "session", consider using a CookieJar or CookieSession instead.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="values">Names/values of HTTP cookies to set. Typically an anonymous object or IDictionary.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithCookies(this FlurlModel url, object values)
        {
            return new FlurlRequest(url).WithCookies(values);
        }

        /// <summary>
        /// Creates a new FlurlRequest and sets the CookieJar associated with this request, which will be updated with any Set-Cookie headers present in the response and is suitable for reuse in subsequent requests.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="cookieJar">The CookieJar.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithCookies(this FlurlModel url, CookieJar cookieJar)
        {
            return new FlurlRequest(url).WithCookies(cookieJar);
        }

        /// <summary>
        /// Creates a new FlurlRequest and associates it with a new CookieJar, which will be updated with any Set-Cookie headers present in the response and is suitable for reuse in subsequent requests.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="cookieJar">The created CookieJar, which can be reused in subsequent requests.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithCookies(this FlurlModel url, out CookieJar cookieJar)
        {
            return new FlurlRequest(url).WithCookies(out cookieJar);
        }

        /// <summary>
        /// Creates a new FlurlRequest and allows changing its Settings inline.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="action">A delegate defining the Settings changes.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest ConfigureRequest(this FlurlModel url, Action<FlurlHttpSettings> action)
        {
            return new FlurlRequest(url).ConfigureRequest(action);
        }

        /// <summary>
        /// Creates a new FlurlRequest and sets the request timeout.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="timespan">Time to wait before the request times out.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithTimeout(this FlurlModel url, TimeSpan timespan)
        {
            return new FlurlRequest(url).WithTimeout(timespan);
        }

        /// <summary>
        /// Creates a new FlurlRequest and sets the request timeout.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="seconds">Seconds to wait before the request times out.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithTimeout(this FlurlModel url, int seconds)
        {
            return new FlurlRequest(url).WithTimeout(seconds);
        }

        /// <summary>
        /// Creates a new FlurlRequest and adds a pattern representing an HTTP status code or range of codes which (in addition to 2xx) will NOT result in a FlurlHttpException being thrown.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="pattern">Examples: "3xx", "100,300,600", "100-299,6xx"</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest AllowHttpStatus(this FlurlModel url, string pattern)
        {
            return new FlurlRequest(url).AllowHttpStatus(pattern);
        }

        /// <summary>
        /// Creates a new FlurlRequest and adds an HttpStatusCode which (in addition to 2xx) will NOT result in a FlurlHttpException being thrown.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="statusCodes">The HttpStatusCode(s) to allow.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest AllowHttpStatus(this FlurlModel url, params HttpStatusCode[] statusCodes)
        {
            return new FlurlRequest(url).AllowHttpStatus(statusCodes);
        }

        /// <summary>
        /// Creates a new FlurlRequest and configures it to allow any returned HTTP status without throwing a FlurlHttpException.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest AllowAnyHttpStatus(this FlurlModel url)
        {
            return new FlurlRequest(url).AllowAnyHttpStatus();
        }

        /// <summary>
        /// Creates a new FlurlRequest and configures whether redirects are automatically followed.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="enabled">true if Flurl should automatically send a new request to the redirect URL, false if it should not.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithAutoRedirect(this FlurlModel url, bool enabled)
        {
            return new FlurlRequest(url).WithAutoRedirect(enabled);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous request.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="verb">The HTTP verb used to make the request.</param>
        /// <param name="content">The request body content.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> SendAsync(this string url, HttpMethod verb, HttpContent content = null, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).SendAsync(verb, content, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous request.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="verb">The HTTP verb used to make the request.</param>
        /// <param name="body">An object representing the request body, which will be serialized to JSON.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> SendJsonAsync(this string url, HttpMethod verb, object body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).SendJsonAsync(verb, body, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous request.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="verb">The HTTP verb used to make the request.</param>
        /// <param name="body">The request body.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> SendStringAsync(this string url, HttpMethod verb, string body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).SendStringAsync(verb, body, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous request.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="verb">The HTTP verb used to make the request.</param>
        /// <param name="body">An object representing the request body, which will be serialized to a URL-encoded string.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> SendUrlEncodedAsync(this string url, HttpMethod verb, object body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).SendUrlEncodedAsync(verb, body, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous GET request.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> GetAsync(this string url, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).GetAsync(completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous GET request.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the JSON response body deserialized to an object of type T.</returns>
        public static Task<T> GetJsonAsync<T>(this string url, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).GetJsonAsync<T>(completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous GET request.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the response body as a string.</returns>
        public static Task<string> GetStringAsync(this string url, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).GetStringAsync(completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous GET request.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the response body as a Stream.</returns>
        public static Task<Stream> GetStreamAsync(this string url, HttpCompletionOption completionOption = HttpCompletionOption.ResponseHeadersRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).GetStreamAsync(completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous GET request.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the response body as a byte array.</returns>
        public static Task<byte[]> GetBytesAsync(this string url, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).GetBytesAsync(completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous POST request.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="content">The request body content.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PostAsync(this string url, HttpContent content = null, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).PostAsync(content, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous POST request.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="body">An object representing the request body, which will be serialized to JSON.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PostJsonAsync(this string url, object body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).PostJsonAsync(body, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous POST request.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="body">The request body.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PostStringAsync(this string url, string body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).PostStringAsync(body, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous POST request.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="body">An object representing the request body, which will be serialized to a URL-encoded string.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PostUrlEncodedAsync(this string url, object body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).PostUrlEncodedAsync(body, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous HEAD request.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> HeadAsync(this string url, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).HeadAsync(completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous PUT request.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="content">The request body content.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PutAsync(this string url, HttpContent content = null, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).PutAsync(content, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous PUT request.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="body">An object representing the request body, which will be serialized to JSON.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PutJsonAsync(this string url, object body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).PutJsonAsync(body, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous PUT request.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="body">The request body.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PutStringAsync(this string url, string body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).PutStringAsync(body, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous DELETE request.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> DeleteAsync(this string url, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).DeleteAsync(completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous PATCH request.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="content">The request body content.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PatchAsync(this string url, HttpContent content = null, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).PatchAsync(content, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous PATCH request.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="body">An object representing the request body, which will be serialized to JSON.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PatchJsonAsync(this string url, object body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).PatchJsonAsync(body, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous PATCH request.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="body">The request body.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PatchStringAsync(this string url, string body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).PatchStringAsync(body, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous OPTIONS request.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> OptionsAsync(this string url, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).OptionsAsync(completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a new FlurlRequest and asynchronously downloads a file.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="localFolderPath">Path of local folder where file is to be downloaded.</param>
        /// <param name="localFileName">Name of local file. If not specified, the source filename (last segment of the URL) is used.</param>
        /// <param name="bufferSize">Buffer size in bytes. Default is 4096.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the local path of the downloaded file.</returns>
        public static Task<string> DownloadFileAsync(this string url, string localFolderPath, string localFileName = null, int bufferSize = 4096, HttpCompletionOption completionOption = HttpCompletionOption.ResponseHeadersRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).DownloadFileAsync(localFolderPath, localFileName, bufferSize, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous multipart/form-data POST request.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="buildContent">A delegate for building the content parts.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PostMultipartAsync(this string url, Action<CapturedMultipartContent> buildContent, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(url).PostMultipartAsync(buildContent, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a new FlurlRequest and sets a request header.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="name">The header name.</param>
        /// <param name="value">The header value.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithHeader(this string url, string name, object value)
        {
            return new FlurlRequest(url).WithHeader(name, value);
        }

        /// <summary>
        /// Creates a new FlurlRequest and sets request headers based on property names/values of the provided object, or keys/values if object is a dictionary, to be sent.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="headers">Names/values of HTTP headers to set. Typically an anonymous object or IDictionary.</param>
        /// <param name="replaceUnderscoreWithHyphen">If true, underscores in property names will be replaced by hyphens. Default is true.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithHeaders(this string url, object headers, bool replaceUnderscoreWithHyphen = true)
        {
            return new FlurlRequest(url).WithHeaders(headers, replaceUnderscoreWithHyphen);
        }

        /// <summary>
        /// Creates a new FlurlRequest and sets the Authorization header according to Basic Authentication protocol.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="username">Username of authenticating user.</param>
        /// <param name="password">Password of authenticating user.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithBasicAuth(this string url, string username, string password)
        {
            return new FlurlRequest(url).WithBasicAuth(username, password);
        }

        /// <summary>
        /// Creates a new FlurlRequest and sets the Authorization header with a bearer token according to OAuth 2.0 specification.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="token">The acquired oAuth bearer token.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithOAuthBearerToken(this string url, string token)
        {
            return new FlurlRequest(url).WithOAuthBearerToken(token);
        }

        /// <summary>
        /// Creates a new FlurlRequest and adds a name-value pair to its Cookie header. To automatically maintain a cookie "session", consider using a CookieJar or CookieSession instead.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="name">The cookie name.</param>
        /// <param name="value">The cookie value.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithCookie(this string url, string name, object value)
        {
            return new FlurlRequest(url).WithCookie(name, value);
        }

        /// <summary>
        /// Creates a new FlurlRequest and adds name-value pairs to its Cookie header based on property names/values of the provided object, or keys/values if object is a dictionary. To automatically maintain a cookie "session", consider using a CookieJar or CookieSession instead.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="values">Names/values of HTTP cookies to set. Typically an anonymous object or IDictionary.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithCookies(this string url, object values)
        {
            return new FlurlRequest(url).WithCookies(values);
        }

        /// <summary>
        /// Creates a new FlurlRequest and sets the CookieJar associated with this request, which will be updated with any Set-Cookie headers present in the response and is suitable for reuse in subsequent requests.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="cookieJar">The CookieJar.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithCookies(this string url, CookieJar cookieJar)
        {
            return new FlurlRequest(url).WithCookies(cookieJar);
        }

        /// <summary>
        /// Creates a new FlurlRequest and associates it with a new CookieJar, which will be updated with any Set-Cookie headers present in the response and is suitable for reuse in subsequent requests.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="cookieJar">The created CookieJar, which can be reused in subsequent requests.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithCookies(this string url, out CookieJar cookieJar)
        {
            return new FlurlRequest(url).WithCookies(out cookieJar);
        }

        /// <summary>
        /// Creates a new FlurlRequest and allows changing its Settings inline.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="action">A delegate defining the Settings changes.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest ConfigureRequest(this string url, Action<FlurlHttpSettings> action)
        {
            return new FlurlRequest(url).ConfigureRequest(action);
        }

        /// <summary>
        /// Creates a new FlurlRequest and sets the request timeout.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="timespan">Time to wait before the request times out.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithTimeout(this string url, TimeSpan timespan)
        {
            return new FlurlRequest(url).WithTimeout(timespan);
        }

        /// <summary>
        /// Creates a new FlurlRequest and sets the request timeout.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="seconds">Seconds to wait before the request times out.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithTimeout(this string url, int seconds)
        {
            return new FlurlRequest(url).WithTimeout(seconds);
        }

        /// <summary>
        /// Creates a new FlurlRequest and adds a pattern representing an HTTP status code or range of codes which (in addition to 2xx) will NOT result in a FlurlHttpException being thrown.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="pattern">Examples: "3xx", "100,300,600", "100-299,6xx"</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest AllowHttpStatus(this string url, string pattern)
        {
            return new FlurlRequest(url).AllowHttpStatus(pattern);
        }

        /// <summary>
        /// Creates a new FlurlRequest and adds an HttpStatusCode which (in addition to 2xx) will NOT result in a FlurlHttpException being thrown.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="statusCodes">The HttpStatusCode(s) to allow.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest AllowHttpStatus(this string url, params HttpStatusCode[] statusCodes)
        {
            return new FlurlRequest(url).AllowHttpStatus(statusCodes);
        }

        /// <summary>
        /// Creates a new FlurlRequest and configures it to allow any returned HTTP status without throwing a FlurlHttpException.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest AllowAnyHttpStatus(this string url)
        {
            return new FlurlRequest(url).AllowAnyHttpStatus();
        }

        /// <summary>
        /// Creates a new FlurlRequest and configures whether redirects are automatically followed.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="enabled">true if Flurl should automatically send a new request to the redirect URL, false if it should not.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithAutoRedirect(this string url, bool enabled)
        {
            return new FlurlRequest(url).WithAutoRedirect(enabled);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous request.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="verb">The HTTP verb used to make the request.</param>
        /// <param name="content">The request body content.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> SendAsync(this Uri uri, HttpMethod verb, HttpContent content = null, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(uri).SendAsync(verb, content, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous request.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="verb">The HTTP verb used to make the request.</param>
        /// <param name="body">An object representing the request body, which will be serialized to JSON.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> SendJsonAsync(this Uri uri, HttpMethod verb, object body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(uri).SendJsonAsync(verb, body, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous request.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="verb">The HTTP verb used to make the request.</param>
        /// <param name="body">The request body.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> SendStringAsync(this Uri uri, HttpMethod verb, string body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(uri).SendStringAsync(verb, body, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous request.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="verb">The HTTP verb used to make the request.</param>
        /// <param name="body">An object representing the request body, which will be serialized to a URL-encoded string.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> SendUrlEncodedAsync(this Uri uri, HttpMethod verb, object body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(uri).SendUrlEncodedAsync(verb, body, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous GET request.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> GetAsync(this Uri uri, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(uri).GetAsync(completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous GET request.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the JSON response body deserialized to an object of type T.</returns>
        public static Task<T> GetJsonAsync<T>(this Uri uri, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(uri).GetJsonAsync<T>(completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous GET request.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the response body as a string.</returns>
        public static Task<string> GetStringAsync(this Uri uri, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(uri).GetStringAsync(completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous GET request.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the response body as a Stream.</returns>
        public static Task<Stream> GetStreamAsync(this Uri uri, HttpCompletionOption completionOption = HttpCompletionOption.ResponseHeadersRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(uri).GetStreamAsync(completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous GET request.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the response body as a byte array.</returns>
        public static Task<byte[]> GetBytesAsync(this Uri uri, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(uri).GetBytesAsync(completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous POST request.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="content">The request body content.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PostAsync(this Uri uri, HttpContent content = null, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(uri).PostAsync(content, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous POST request.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="body">An object representing the request body, which will be serialized to JSON.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PostJsonAsync(this Uri uri, object body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(uri).PostJsonAsync(body, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous POST request.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="body">The request body.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PostStringAsync(this Uri uri, string body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(uri).PostStringAsync(body, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous POST request.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="body">An object representing the request body, which will be serialized to a URL-encoded string.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PostUrlEncodedAsync(this Uri uri, object body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(uri).PostUrlEncodedAsync(body, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous HEAD request.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> HeadAsync(this Uri uri, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(uri).HeadAsync(completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous PUT request.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="content">The request body content.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PutAsync(this Uri uri, HttpContent content = null, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(uri).PutAsync(content, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous PUT request.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="body">An object representing the request body, which will be serialized to JSON.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PutJsonAsync(this Uri uri, object body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(uri).PutJsonAsync(body, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous PUT request.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="body">The request body.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PutStringAsync(this Uri uri, string body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(uri).PutStringAsync(body, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous DELETE request.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> DeleteAsync(this Uri uri, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(uri).DeleteAsync(completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous PATCH request.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="content">The request body content.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PatchAsync(this Uri uri, HttpContent content = null, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(uri).PatchAsync(content, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous PATCH request.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="body">An object representing the request body, which will be serialized to JSON.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PatchJsonAsync(this Uri uri, object body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(uri).PatchJsonAsync(body, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous PATCH request.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="body">The request body.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PatchStringAsync(this Uri uri, string body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(uri).PatchStringAsync(body, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous OPTIONS request.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> OptionsAsync(this Uri uri, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(uri).OptionsAsync(completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a new FlurlRequest and asynchronously downloads a file.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="localFolderPath">Path of local folder where file is to be downloaded.</param>
        /// <param name="localFileName">Name of local file. If not specified, the source filename (last segment of the URL) is used.</param>
        /// <param name="bufferSize">Buffer size in bytes. Default is 4096.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the local path of the downloaded file.</returns>
        public static Task<string> DownloadFileAsync(this Uri uri, string localFolderPath, string localFileName = null, int bufferSize = 4096, HttpCompletionOption completionOption = HttpCompletionOption.ResponseHeadersRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(uri).DownloadFileAsync(localFolderPath, localFileName, bufferSize, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a FlurlRequest and sends an asynchronous multipart/form-data POST request.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="buildContent">A delegate for building the content parts.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PostMultipartAsync(this Uri uri, Action<CapturedMultipartContent> buildContent, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            return new FlurlRequest(uri).PostMultipartAsync(buildContent, completionOption, cancellationToken);
        }

        /// <summary>
        /// Creates a new FlurlRequest and sets a request header.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="name">The header name.</param>
        /// <param name="value">The header value.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithHeader(this Uri uri, string name, object value)
        {
            return new FlurlRequest(uri).WithHeader(name, value);
        }

        /// <summary>
        /// Creates a new FlurlRequest and sets request headers based on property names/values of the provided object, or keys/values if object is a dictionary, to be sent.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="headers">Names/values of HTTP headers to set. Typically an anonymous object or IDictionary.</param>
        /// <param name="replaceUnderscoreWithHyphen">If true, underscores in property names will be replaced by hyphens. Default is true.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithHeaders(this Uri uri, object headers, bool replaceUnderscoreWithHyphen = true)
        {
            return new FlurlRequest(uri).WithHeaders(headers, replaceUnderscoreWithHyphen);
        }

        /// <summary>
        /// Creates a new FlurlRequest and sets the Authorization header according to Basic Authentication protocol.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="username">Username of authenticating user.</param>
        /// <param name="password">Password of authenticating user.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithBasicAuth(this Uri uri, string username, string password)
        {
            return new FlurlRequest(uri).WithBasicAuth(username, password);
        }

        /// <summary>
        /// Creates a new FlurlRequest and sets the Authorization header with a bearer token according to OAuth 2.0 specification.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="token">The acquired oAuth bearer token.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithOAuthBearerToken(this Uri uri, string token)
        {
            return new FlurlRequest(uri).WithOAuthBearerToken(token);
        }

        /// <summary>
        /// Creates a new FlurlRequest and adds a name-value pair to its Cookie header. To automatically maintain a cookie "session", consider using a CookieJar or CookieSession instead.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="name">The cookie name.</param>
        /// <param name="value">The cookie value.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithCookie(this Uri uri, string name, object value)
        {
            return new FlurlRequest(uri).WithCookie(name, value);
        }

        /// <summary>
        /// Creates a new FlurlRequest and adds name-value pairs to its Cookie header based on property names/values of the provided object, or keys/values if object is a dictionary. To automatically maintain a cookie "session", consider using a CookieJar or CookieSession instead.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="values">Names/values of HTTP cookies to set. Typically an anonymous object or IDictionary.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithCookies(this Uri uri, object values)
        {
            return new FlurlRequest(uri).WithCookies(values);
        }

        /// <summary>
        /// Creates a new FlurlRequest and sets the CookieJar associated with this request, which will be updated with any Set-Cookie headers present in the response and is suitable for reuse in subsequent requests.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="cookieJar">The CookieJar.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithCookies(this Uri uri, CookieJar cookieJar)
        {
            return new FlurlRequest(uri).WithCookies(cookieJar);
        }

        /// <summary>
        /// Creates a new FlurlRequest and associates it with a new CookieJar, which will be updated with any Set-Cookie headers present in the response and is suitable for reuse in subsequent requests.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="cookieJar">The created CookieJar, which can be reused in subsequent requests.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithCookies(this Uri uri, out CookieJar cookieJar)
        {
            return new FlurlRequest(uri).WithCookies(out cookieJar);
        }

        /// <summary>
        /// Creates a new FlurlRequest and allows changing its Settings inline.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="action">A delegate defining the Settings changes.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest ConfigureRequest(this Uri uri, Action<FlurlHttpSettings> action)
        {
            return new FlurlRequest(uri).ConfigureRequest(action);
        }

        /// <summary>
        /// Creates a new FlurlRequest and sets the request timeout.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="timespan">Time to wait before the request times out.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithTimeout(this Uri uri, TimeSpan timespan)
        {
            return new FlurlRequest(uri).WithTimeout(timespan);
        }

        /// <summary>
        /// Creates a new FlurlRequest and sets the request timeout.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="seconds">Seconds to wait before the request times out.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithTimeout(this Uri uri, int seconds)
        {
            return new FlurlRequest(uri).WithTimeout(seconds);
        }

        /// <summary>
        /// Creates a new FlurlRequest and adds a pattern representing an HTTP status code or range of codes which (in addition to 2xx) will NOT result in a FlurlHttpException being thrown.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="pattern">Examples: "3xx", "100,300,600", "100-299,6xx"</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest AllowHttpStatus(this Uri uri, string pattern)
        {
            return new FlurlRequest(uri).AllowHttpStatus(pattern);
        }

        /// <summary>
        /// Creates a new FlurlRequest and adds an HttpStatusCode which (in addition to 2xx) will NOT result in a FlurlHttpException being thrown.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="statusCodes">The HttpStatusCode(s) to allow.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest AllowHttpStatus(this Uri uri, params HttpStatusCode[] statusCodes)
        {
            return new FlurlRequest(uri).AllowHttpStatus(statusCodes);
        }

        /// <summary>
        /// Creates a new FlurlRequest and configures it to allow any returned HTTP status without throwing a FlurlHttpException.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest AllowAnyHttpStatus(this Uri uri)
        {
            return new FlurlRequest(uri).AllowAnyHttpStatus();
        }

        /// <summary>
        /// Creates a new FlurlRequest and configures whether redirects are automatically followed.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="enabled">true if Flurl should automatically send a new request to the redirect URL, false if it should not.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithAutoRedirect(this Uri uri, bool enabled)
        {
            return new FlurlRequest(uri).WithAutoRedirect(enabled);
        }

    }
    /// <summary>
    /// Fluent extension methods for working with HTTP request headers.
    /// </summary>
    public static class HeaderExtensions
    {
        /// <summary>
        /// Sets an HTTP header to be sent with this IFlurlRequest or all requests made with this IFlurlClient.
        /// </summary>
        /// <param name="clientOrRequest">The IFlurlClient or IFlurlRequest.</param>
        /// <param name="name">HTTP header name.</param>
        /// <param name="value">HTTP header value.</param>
        /// <returns>This IFlurlClient or IFlurlRequest.</returns>
        public static T WithHeader<T>(this T clientOrRequest, string name, object value) where T : IHttpSettingsContainer
        {
            if (value == null)
                clientOrRequest.Headers.Remove(name);
            else
                clientOrRequest.Headers.AddOrReplace(name, value.ToInvariantString().Trim());
            return clientOrRequest;
        }

        /// <summary>
        /// Sets HTTP headers based on property names/values of the provided object, or keys/values if object is a dictionary, to be sent with this IFlurlRequest or all requests made with this IFlurlClient.
        /// </summary>
        /// <param name="clientOrRequest">The IFlurlClient or IFlurlRequest.</param>
        /// <param name="headers">Names/values of HTTP headers to set. Typically an anonymous object or IDictionary.</param>
        /// <param name="replaceUnderscoreWithHyphen">If true, underscores in property names will be replaced by hyphens. Default is true.</param>
        /// <returns>This IFlurlClient or IFlurlRequest.</returns>
        public static T WithHeaders<T>(this T clientOrRequest, object headers, bool replaceUnderscoreWithHyphen = true) where T : IHttpSettingsContainer
        {
            if (headers == null)
                return clientOrRequest;

            // underscore replacement only applies when object properties are parsed to kv pairs
            replaceUnderscoreWithHyphen = replaceUnderscoreWithHyphen && !(headers is string) && !(headers is IEnumerable);

            foreach (var kv in headers.ToKeyValuePairs())
            {
                var key = replaceUnderscoreWithHyphen ? kv.Key.Replace("_", "-") : kv.Key;
                clientOrRequest.WithHeader(key, kv.Value);
            }

            return clientOrRequest;
        }

        /// <summary>
        /// Sets HTTP authorization header according to Basic Authentication protocol to be sent with this IFlurlRequest or all requests made with this IFlurlClient.
        /// </summary>
        /// <param name="clientOrRequest">The IFlurlClient or IFlurlRequest.</param>
        /// <param name="username">Username of authenticating user.</param>
        /// <param name="password">Password of authenticating user.</param>
        /// <returns>This IFlurlClient or IFlurlRequest.</returns>
        public static T WithBasicAuth<T>(this T clientOrRequest, string username, string password) where T : IHttpSettingsContainer
        {
            // http://stackoverflow.com/questions/14627399/setting-authorization-header-of-httpclient
            var encodedCreds = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
            return clientOrRequest.WithHeader("Authorization", $"Basic {encodedCreds}");
        }

        /// <summary>
        /// Sets HTTP authorization header with acquired bearer token according to OAuth 2.0 specification to be sent with this IFlurlRequest or all requests made with this IFlurlClient.
        /// </summary>
        /// <param name="clientOrRequest">The IFlurlClient or IFlurlRequest.</param>
        /// <param name="token">The acquired bearer token to pass.</param>
        /// <returns>This IFlurlClient or IFlurlRequest.</returns>
        public static T WithOAuthBearerToken<T>(this T clientOrRequest, string token) where T : IHttpSettingsContainer
        {
            return clientOrRequest.WithHeader("Authorization", $"Bearer {token}");
        }
    }
    /// <summary>
    /// Provides fluent helpers for asserting against fake HTTP calls. Usually created fluently
    /// via HttpTest.ShouldHaveCalled or HttpTest.ShouldNotHaveCalled, rather than instantiated directly.
    /// </summary>
    public class HttpCallAssertion
    {
        private readonly bool _negate;
        private readonly IList<string> _expectedConditions = new List<string>();

        private IList<FlurlCall> _calls;

        /// <summary>
        /// Constructs a new instance of HttpCallAssertion.
        /// </summary>
        /// <param name="loggedCalls">Set of calls (usually from HttpTest.CallLog) to assert against.</param>
        /// <param name="negate">If true, assertions pass when calls matching criteria were NOT made.</param>
        public HttpCallAssertion(IEnumerable<FlurlCall> loggedCalls, bool negate = false)
        {
            _calls = loggedCalls.ToList();
            _negate = negate;
        }

        /// <summary>
        /// Assert whether calls matching specified criteria were made a specific number of times. (When not specified,
        /// assertions verify whether any calls matching criteria were made.)
        /// </summary>
        /// <param name="expectedCount">Exact number of expected calls</param>
        /// <exception cref="ArgumentException"><paramref name="expectedCount"/> must be greater than or equal to 0.</exception>
        public void Times(int expectedCount)
        {
            if (expectedCount < 0)
                throw new ArgumentException("expectedCount must be greater than or equal to 0.");

            Assert(expectedCount);
        }

        /// <summary>
        /// Asserts whether calls were made matching the given predicate function.
        /// </summary>
        /// <param name="match">Predicate (usually a lambda expression) that tests a FlurlCall and returns a bool.</param>
        /// <param name="descrip">A description of what is being asserted.</param>
        public HttpCallAssertion With(Func<FlurlCall, bool> match, string descrip = null)
        {
            if (!string.IsNullOrEmpty(descrip))
                _expectedConditions.Add(descrip);
            _calls = _calls.Where(match).ToList();
            Assert();
            return this;
        }

        /// <summary>
        /// Asserts whether calls were made that do NOT match the given predicate function.
        /// </summary>
        /// <param name="match">Predicate (usually a lambda expression) that tests a FlurlCall and returns a bool.</param>
        /// <param name="descrip">A description of what is being asserted.</param>
        public HttpCallAssertion Without(Func<FlurlCall, bool> match, string descrip = null)
        {
            return With(c => !match(c), descrip);
        }

        /// <summary>
        /// Asserts whether calls were made matching given URL or URL pattern.
        /// </summary>
        /// <param name="urlPattern">Can contain * wildcard.</param>
        public HttpCallAssertion WithUrlPattern(string urlPattern)
        {
            return With(c => Util.MatchesUrlPattern(c.Request.Url, urlPattern), $"URL pattern {urlPattern}");
        }

        /// <summary>
        /// Asserts whether calls were made with any of the given HTTP verbs.
        /// </summary>
        public HttpCallAssertion WithVerb(params HttpMethod[] verbs)
        {
            var list = string.Join(", ", verbs.Select(v => v.Method));
            return With(call => call.HasAnyVerb(verbs), $"verb {list}");
        }

        /// <summary>
        /// Asserts whether calls were made with any of the given HTTP verbs.
        /// </summary>
        public HttpCallAssertion WithVerb(params string[] verbs)
        {
            var list = string.Join(", ", verbs);
            return With(call => call.HasAnyVerb(verbs), $"verb {list}");
        }

        #region request body
        /// <summary>
        /// Asserts whether calls were made containing given request body. body may contain * wildcard.
        /// </summary>
        public HttpCallAssertion WithRequestBody(string bodyPattern)
        {
            return With(c => Util.MatchesPattern(c.RequestBody, bodyPattern), $"body {bodyPattern}");
        }

        /// <summary>
        /// Asserts whether calls were made containing given JSON-encoded request body. body may contain * wildcard.
        /// </summary>
        public HttpCallAssertion WithRequestJson(object body)
        {
            var serializedBody = FlurlHttp.GlobalSettings.JsonSerializer.Serialize(body);
            return WithRequestBody(serializedBody);
        }

        /// <summary>
        /// Asserts whether calls were made containing given URL-encoded request body. body may contain * wildcard.
        /// </summary>
        public HttpCallAssertion WithRequestUrlEncoded(object body)
        {
            var serializedBody = FlurlHttp.GlobalSettings.UrlEncodedSerializer.Serialize(body);
            return WithRequestBody(serializedBody);
        }
        #endregion

        #region query params
        /// <summary>
        /// Asserts whether calls were made containing the given query parameter name and (optionally) value. value may contain * wildcard.
        /// </summary>
        public HttpCallAssertion WithQueryParam(string name, object value = null)
        {
            return With(c => c.HasQueryParam(name, value), BuildDescrip("query param", name, value));
        }

        /// <summary>
        /// Asserts whether calls were made NOT containing the given query parameter and (optionally) value. value may contain * wildcard.
        /// </summary>
        public HttpCallAssertion WithoutQueryParam(string name, object value = null)
        {
            return Without(c => c.HasQueryParam(name, value), BuildDescrip("no query param", name, value));
        }

        /// <summary>
        /// Asserts whether calls were made containing ALL the given query parameters (regardless of their values).
        /// </summary>
        public HttpCallAssertion WithQueryParams(params string[] names)
        {
            return names.Select(n => WithQueryParam(n)).LastOrDefault() ?? this;
        }

        /// <summary>
        /// Asserts whether calls were made NOT containing any of the given query parameters.
        /// If no names are provided, asserts no calls were made with any query parameters.
        /// </summary>
        public HttpCallAssertion WithoutQueryParams(params string[] names)
        {
            if (!names.Any())
                return With(c => !c.Request.Url.QueryParams.Any(), "no query parameters");
            return names.Select(n => WithoutQueryParam(n)).LastOrDefault() ?? this;
        }

        /// <summary>
        /// Asserts whether calls were made containing ANY the given query parameters (regardless of their values).
        /// If no names are provided, asserts that calls were made containing at least one query parameter with any name.
        /// </summary>
        public HttpCallAssertion WithAnyQueryParam(params string[] names)
        {
            var descrip = $"any query param {string.Join(", ", names)}".Trim();
            return With(c => c.HasAnyQueryParam(names), descrip);
        }

        /// <summary>
        /// Asserts whether calls were made containing all of the given query parameter values.
        /// </summary>
        /// <param name="values">Object (usually anonymous) or dictionary that is parsed to name/value query parameters to check for. Values may contain * wildcard.</param>
        public HttpCallAssertion WithQueryParams(object values)
        {
            return values.ToKeyValuePairs().Select(kv => WithQueryParam(kv.Key, kv.Value)).LastOrDefault() ?? this;
        }

        /// <summary>
        /// Asserts whether calls were made NOT containing any of the given query parameter values.
        /// </summary>
        /// <param name="values">Object (usually anonymous) or dictionary that is parsed to name/value query parameters to check for. Values may contain * wildcard.</param>
        public HttpCallAssertion WithoutQueryParams(object values)
        {
            return values.ToKeyValuePairs().Select(kv => WithoutQueryParam(kv.Key, kv.Value)).LastOrDefault() ?? this;
        }
        #endregion

        #region headers
        /// <summary>
        /// Asserts whether calls were made containing the given header name and (optionally) value. value may contain * wildcard.
        /// </summary>
        public HttpCallAssertion WithHeader(string name, object value = null)
        {
            return With(c => c.HasHeader(name, value), BuildDescrip("header", name, value));
        }

        /// <summary>
        /// Asserts whether calls were made NOT containing the given header and (optionally) value. value may contain * wildcard.
        /// </summary>
        public HttpCallAssertion WithoutHeader(string name, object value = null)
        {
            return Without(c => c.HasHeader(name, value), BuildDescrip("no header", name, value));
        }

        /// <summary>
        /// Asserts whether calls were made containing ALL the given headers (regardless of their values).
        /// </summary>
        public HttpCallAssertion WithHeaders(params string[] names)
        {
            return names.Select(n => WithHeader(n)).LastOrDefault() ?? this;
        }

        /// <summary>
        /// Asserts whether calls were made NOT containing any of the given headers.
        /// If no names are provided, asserts no calls were made with any headers.
        /// </summary>
        public HttpCallAssertion WithoutHeaders(params string[] names)
        {
            if (!names.Any())
                return With(c => !c.Request.Headers.Any(), "no headers");
            return names.Select(n => WithoutHeader(n)).LastOrDefault() ?? this;
        }

        /// <summary>
        /// Asserts whether calls were made containing ANY the given headers (regardless of their values).
        /// If no names are provided, asserts that calls were made containing at least one header with any name.
        /// </summary>
        public HttpCallAssertion WithAnyHeader(params string[] names)
        {
            var descrip = $"any header {string.Join(", ", names)}".Trim();
            return With(call => {
                if (!names.Any()) return call.Request.Headers.Any();
                return call.Request.Headers.Select(h => h.Name).Intersect(names).Any();
            }, descrip);
        }

        /// <summary>
        /// Asserts whether calls were made containing all of the given header values.
        /// </summary>
        /// <param name="values">Object (usually anonymous) or dictionary that is parsed to name/value headers to check for. Values may contain * wildcard.</param>
        public HttpCallAssertion WithHeaders(object values)
        {
            return values.ToKeyValuePairs().Select(kv => WithHeader(kv.Key, kv.Value)).LastOrDefault() ?? this;
        }

        /// <summary>
        /// Asserts whether calls were made NOT containing any of the given header values.
        /// </summary>
        /// <param name="values">Object (usually anonymous) or dictionary that is parsed to name/value headers to check for. Values may contain * wildcard.</param>
        public HttpCallAssertion WithoutHeaders(object values)
        {
            return values.ToKeyValuePairs().Select(kv => WithoutHeader(kv.Key, kv.Value)).LastOrDefault() ?? this;
        }
        #endregion

        #region cookies
        /// <summary>
        /// Asserts whether calls were made containing the given cookie name and (optionally) value. value may contain * wildcard.
        /// </summary>
        public HttpCallAssertion WithCookie(string name, object value = null)
        {
            return With(c => c.HasCookie(name, value), BuildDescrip("cookie", name, value));
        }

        /// <summary>
        /// Asserts whether calls were made NOT containing the given cookie and (optionally) value. value may contain * wildcard.
        /// </summary>
        public HttpCallAssertion WithoutCookie(string name, object value = null)
        {
            return Without(c => c.HasCookie(name, value), BuildDescrip("no cookie", name, value));
        }

        /// <summary>
        /// Asserts whether calls were made containing ALL the given cookies (regardless of their values).
        /// </summary>
        public HttpCallAssertion WithCookies(params string[] names)
        {
            return names.Select(n => WithCookie(n)).LastOrDefault() ?? this;
        }

        /// <summary>
        /// Asserts whether calls were made NOT containing any of the given cookies.
        /// If no names are provided, asserts no calls were made with any cookies.
        /// </summary>
        public HttpCallAssertion WithoutCookies(params string[] names)
        {
            if (!names.Any())
                return With(c => !c.Request.Cookies.Any(), "no cookies");
            return names.Select(n => WithoutCookie(n)).LastOrDefault() ?? this;
        }

        /// <summary>
        /// Asserts whether calls were made containing ANY the given cookies (regardless of their values).
        /// If no names are provided, asserts that calls were made containing at least one cookie with any name.
        /// </summary>
        public HttpCallAssertion WithAnyCookie(params string[] names)
        {
            var descrip = $"any cookie {string.Join(", ", names)}".Trim();
            return With(call => {
                if (!names.Any()) return call.Request.Cookies.Any();
                return call.Request.Cookies.Select(c => c.Name).Intersect(names).Any();
            }, descrip);
        }

        /// <summary>
        /// Asserts whether calls were made containing all of the given cookie values.
        /// </summary>
        /// <param name="values">Object (usually anonymous) or dictionary that is parsed to name/value cookies to check for. Values may contain * wildcard.</param>
        public HttpCallAssertion WithCookies(object values)
        {
            return values.ToKeyValuePairs().Select(kv => WithCookie(kv.Key, kv.Value)).LastOrDefault() ?? this;
        }

        /// <summary>
        /// Asserts whether calls were made NOT containing any of the given cookie values.
        /// </summary>
        /// <param name="values">Object (usually anonymous) or dictionary that is parsed to name/value cookies to check for. Values may contain * wildcard.</param>
        public HttpCallAssertion WithoutCookies(object values)
        {
            return values.ToKeyValuePairs().Select(kv => WithoutCookie(kv.Key, kv.Value)).LastOrDefault() ?? this;
        }
        #endregion

        /// <summary>
        /// Asserts whether calls were made with a request body of the given content (MIME) type.
        /// </summary>
        public HttpCallAssertion WithContentType(string contentType)
        {
            // Content-Type header may include charset or boundary after a semicolon, i.e. application/json; charset=utf-8
            // Be lenient and allow assertion to pass if only the media type is checked.
            return With(c =>
                c.HasHeader("Content-Type", contentType) || c.HasHeader("Content-Type", contentType + ";*"),
                "content type " + contentType);
        }

        /// <summary>
        /// Asserts whether an Authorization header was set with the given Bearer token, or any Bearer token if excluded.
        /// Token can contain * wildcard.
        /// </summary>
        public HttpCallAssertion WithOAuthBearerToken(string token = "*")
        {
            return WithHeader("Authorization", $"Bearer {token}");
        }

        /// <summary>
        /// Asserts whether the Authorization header was set with Basic auth and (optionally) the given credentials.
        /// Username and password can contain * wildcard.
        /// </summary>
        public HttpCallAssertion WithBasicAuth(string username = "*", string password = "*")
        {
            return With(call => {
                var val = call.Request.Headers.FirstOrDefault("Authorization");
                if (val == null) return false;
                if (!val.OrdinalStartsWith("Basic ")) return false;
                if ((username ?? "*") == "*" && (password ?? "*") == "*") return true;
                var encodedCreds = val.Substring(6);
                try
                {
                    var bytes = Convert.FromBase64String(encodedCreds);
                    var creds = Encoding.UTF8.GetString(bytes, 0, bytes.Length).SplitOnFirstOccurence(":");
                    return
                        creds.Length == 2 &&
                        Util.MatchesPattern(creds[0], username) &&
                        Util.MatchesPattern(creds[1], password);
                }
                catch (FormatException)
                {
                    return false;
                }
            });
        }

        private void Assert(int? count = null)
        {
            var pass = count.HasValue ? (_calls.Count == count.Value) : _calls.Any();
            if (_negate) pass = !pass;

            if (!pass)
                throw new HttpTestException(_expectedConditions, count, _calls.Count);
        }

        private string BuildDescrip(string label, string name, object value)
        {
            var result = $"{label} {name}";
            if (value != null) result += $" = {value}";
            return result;
        }
    }
    /// <summary>
    /// Extension methods off HttpRequestMessage and HttpResponseMessage.
    /// </summary>
    public static class HttpMessageExtensions
    {
        /// <summary>
        /// Set a header on this HttpRequestMessage (default), or its Content property if it's a known content-level header.
        /// No validation. Overwrites any existing value(s) for the header. 
        /// </summary>
        /// <param name="request">The HttpRequestMessage.</param>
        /// <param name="name">The header name.</param>
        /// <param name="value">The header value.</param>
        /// <param name="createContentIfNecessary">If it's a content-level header and there is no content, this determines whether to create an empty HttpContent or just ignore the header.</param>
        public static void SetHeader(this HttpRequestMessage request, string name, object value, bool createContentIfNecessary = true)
        {
            new HttpMessage(request).SetHeader(name, value, createContentIfNecessary);
        }

        /// <summary>
        /// Set a header on this HttpResponseMessage (default), or its Content property if it's a known content-level header.
        /// No validation. Overwrites any existing value(s) for the header. 
        /// </summary>
        /// <param name="response">The HttpResponseMessage.</param>
        /// <param name="name">The header name.</param>
        /// <param name="value">The header value.</param>
        /// <param name="createContentIfNecessary">If it's a content-level header and there is no content, this determines whether to create an empty HttpContent or just ignore the header.</param>
        public static void SetHeader(this HttpResponseMessage response, string name, object value, bool createContentIfNecessary = true)
        {
            new HttpMessage(response).SetHeader(name, value, createContentIfNecessary);
        }

        private static void SetHeader(this HttpMessage msg, string name, object value, bool createContentIfNecessary)
        {
            switch (name.ToLower())
            {
                // https://docs.microsoft.com/en-us/dotnet/api/system.net.http.headers.httpcontentheaders
                case "allow":
                case "content-disposition":
                case "content-encoding":
                case "content-language":
                case "content-length":
                case "content-location":
                case "content-md5":
                case "content-range":
                case "content-type":
                case "expires":
                case "last-modified":
                    // it's a content-level header
                    if (msg.Content != null)
                    {
                        msg.Content.Headers.Remove(name);
                    }
                    else if (createContentIfNecessary && value != null)
                    {
                        msg.Content = new CapturedStringContent("");
                        msg.Content.Headers.Clear();
                    }
                    else
                    {
                        break;
                    }

                    if (value != null)
                        msg.Content.Headers.TryAddWithoutValidation(name, new[] { value.ToInvariantString() });
                    break;
                default:
                    // it's a request/response-level header
                    if (!name.OrdinalEquals("Set-Cookie", true)) // multiple set-cookie headers are allowed
                        msg.Headers.Remove(name);
                    if (value != null)
                        msg.Headers.TryAddWithoutValidation(name, new[] { value.ToInvariantString() });
                    break;
            }
        }

        /// <summary>
        /// Wrapper class for treating HttpRequestMessage and HttpResponseMessage uniformly. (Unfortunately they don't have a common interface.)
        /// </summary>
        private class HttpMessage
        {
            private readonly HttpRequestMessage _request;
            private readonly HttpResponseMessage _response;

            public HttpHeaders Headers => _request?.Headers as HttpHeaders ?? _response?.Headers;

            public HttpContent Content
            {
                get => _request?.Content ?? _response?.Content;
                set
                {
                    if (_request != null) _request.Content = value;
                    else _response.Content = value;
                }
            }

            public HttpMessage(HttpRequestMessage request)
            {
                _request = request;
            }

            public HttpMessage(HttpResponseMessage response)
            {
                _response = response;
            }
        }
    }
    /// <summary>
    /// The status range parser class.
    /// </summary>
    public static class HttpStatusRangeParser
    {
        /// <summary>
        /// Determines whether the specified pattern is match.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentException">pattern is invalid.</exception>
        public static bool IsMatch(string pattern, HttpStatusCode value)
        {
            return IsMatch(pattern, (int)value);
        }

        /// <summary>
        /// Determines whether the specified pattern is match.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentException"><paramref name="pattern"/> is invalid.</exception>
        public static bool IsMatch(string pattern, int value)
        {
            if (pattern == null)
                return false;

            foreach (var range in pattern.Split(',').Select(p => p.Trim()))
            {
                if (range == "")
                    continue;

                if (range == "*")
                    return true; // special case - allow everything

                var bounds = range.Split('-');
                int lower = 0, upper = 0;

                var valid =
                    bounds.Length <= 2 &&
                    int.TryParse(Regex.Replace(bounds.First().Trim(), "[*xX]", "0"), out lower) &&
                    int.TryParse(Regex.Replace(bounds.Last().Trim(), "[*xX]", "9"), out upper);

                if (!valid)
                {
                    throw new ArgumentException(
                        $"Invalid range pattern: \"{pattern}\". Examples of allowed patterns: \"400\", \"4xx\", \"300,400-403\", \"*\".");
                }

                if (value >= lower && value <= upper)
                    return true;
            }
            return false;
        }
    }
    /// <summary>
    /// An object whose existence puts Flurl.Http into test mode where actual HTTP calls are faked. Provides a response
    /// queue, call log, and assertion helpers for use in Arrange/Act/Assert style tests.
    /// </summary>
    [Serializable]
    public class HttpTest : HttpTestSetup, IDisposable
    {
        private readonly ConcurrentQueue<FlurlCall> _calls = new ConcurrentQueue<FlurlCall>();
        private readonly List<FilteredHttpTestSetup> _filteredSetups = new List<FilteredHttpTestSetup>();

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpTest"/> class.
        /// </summary>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public HttpTest() : base(new FlurlHttpSettings())
        {
            SetCurrentTest(this);
        }

        internal void LogCall(FlurlCall call) => _calls.Enqueue(call);

        /// <summary>
        /// Gets the current HttpTest from the logical (async) call context
        /// </summary>
        public static HttpTest Current => GetCurrentTest();

        /// <summary>
        /// List of all (fake) HTTP calls made since this HttpTest was created.
        /// </summary>
#if NET40
        public IReadOnlyList<FlurlCall> CallLog => new EReadOnlyCollection<FlurlCall>(_calls.ToList());
#else
        public IReadOnlyList<FlurlCall> CallLog => new ReadOnlyCollection<FlurlCall>(_calls.ToList());
#endif

        /// <summary>
        /// Change FlurlHttpSettings for the scope of this HttpTest.
        /// </summary>
        /// <param name="action">Action defining the settings changes.</param>
        /// <returns>This HttpTest</returns>
        public HttpTest Configure(Action<FlurlHttpSettings> action)
        {
            action(Settings);
            return this;
        }

        /// <summary>
        /// Fluently creates and returns a new request-specific test setup. 
        /// </summary>
        public FilteredHttpTestSetup ForCallsTo(params string[] urlPatterns)
        {
            var setup = new FilteredHttpTestSetup(Settings, urlPatterns);
            _filteredSetups.Add(setup);
            return setup;
        }

        internal HttpTestSetup FindSetup(FlurlCall call)
        {
            return _filteredSetups.FirstOrDefault(ts => ts.IsMatch(call)) ?? (HttpTestSetup)this;
        }

        /// <summary>
        /// Asserts whether matching URL was called, throwing HttpCallAssertException if it wasn't.
        /// </summary>
        /// <param name="urlPattern">URL that should have been called. Can include * wildcard character.</param>
        public HttpCallAssertion ShouldHaveCalled(string urlPattern)
        {
            return new HttpCallAssertion(CallLog).WithUrlPattern(urlPattern);
        }

        /// <summary>
        /// Asserts whether matching URL was NOT called, throwing HttpCallAssertException if it was.
        /// </summary>
        /// <param name="urlPattern">URL that should not have been called. Can include * wildcard character.</param>
        public void ShouldNotHaveCalled(string urlPattern)
        {
            new HttpCallAssertion(CallLog, true).WithUrlPattern(urlPattern);
        }

        /// <summary>
        /// Asserts whether any HTTP call was made, throwing HttpCallAssertException if none were.
        /// </summary>
        public HttpCallAssertion ShouldHaveMadeACall()
        {
            return new HttpCallAssertion(CallLog).WithUrlPattern("*");
        }

        /// <summary>
        /// Asserts whether no HTTP calls were made, throwing HttpCallAssertException if any were.
        /// </summary>
        public void ShouldNotHaveMadeACall()
        {
            new HttpCallAssertion(CallLog, true).WithUrlPattern("*");
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            SetCurrentTest(null);
        }

        private static readonly System.Threading.AsyncLocal<HttpTest> _test = new System.Threading.AsyncLocal<HttpTest>();
        private static void SetCurrentTest(HttpTest test) => _test.Value = test;
        private static HttpTest GetCurrentTest() => _test.Value;
    }
    /// <summary>
    /// An exception thrown by HttpTest's assertion methods to indicate that the assertion failed.
    /// </summary>
    public class HttpTestException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpTestException"/> class.
        /// </summary>
        /// <param name="conditions">The expected call conditions.</param>
        /// <param name="expectedCalls">The expected number of calls.</param>
        /// <param name="actualCalls">The actual number calls.</param>
        public HttpTestException(IList<string> conditions, int? expectedCalls, int actualCalls) : base(BuildMessage(conditions, expectedCalls, actualCalls)) { }

        private static string BuildMessage(IList<string> conditions, int? expectedCalls, int actualCalls)
        {
            var expected =
                (expectedCalls == null) ? "any calls to be made" :
                (expectedCalls == 0) ? "no calls to be made" :
                (expectedCalls == 1) ? "1 call to be made" :
                expectedCalls + " calls to be made";
            var actual =
                (actualCalls == 0) ? "no matching calls were made" :
                (actualCalls == 1) ? "1 matching call was made" :
                actualCalls + " matching calls were made";
            if (conditions.Any())
                expected += " with " + string.Join(" and ", conditions);
            else
                actual = actual.Replace(" matching", "");
            return $"Expected {expected}, but {actual}.";
        }
    }
    /// <summary>
    /// Abstract base class class for HttpTest and FilteredHttpTestSetup. Provides fluent methods for building queue of fake responses.
    /// </summary>
    public abstract class HttpTestSetup
    {
        private readonly List<Func<HttpResponseMessage>> _responses = new List<Func<HttpResponseMessage>>();

        private int _respIndex = 0;
        private bool _allowRealHttp = false;

        /// <summary>
        /// Constructs a new instance of HttpTestSetup.
        /// </summary>
        /// <param name="settings">FlurlHttpSettings used in fake calls.</param>
        protected HttpTestSetup(FlurlHttpSettings settings)
        {
            Settings = settings;
        }

        /// <summary>
        /// The FlurlHttpSettings used in fake calls.
        /// </summary>
        public FlurlHttpSettings Settings { get; }

        internal HttpResponseMessage GetNextResponse()
        {
            if (_allowRealHttp)
                return null;

            // atomically get the next response in the list, or the last one if we're past the end
            if (_responses.Any())
                return _responses[Math.Min(Interlocked.Increment(ref _respIndex), _responses.Count) - 1]();

            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("")
            };
        }

        /// <summary>
        /// Adds a fake HTTP response to the response queue.
        /// </summary>
        /// <param name="body">The simulated response body string.</param>
        /// <param name="status">The simulated HTTP status. Default is 200.</param>
        /// <param name="headers">The simulated response headers (optional).</param>
        /// <param name="cookies">The simulated response cookies (optional).</param>
        /// <param name="replaceUnderscoreWithHyphen">If true, underscores in property names of headers will be replaced by hyphens. Default is true.</param>
        /// <returns>The current HttpTest object (so more responses can be chained).</returns>
        public HttpTestSetup RespondWith(string body, int status = 200, object headers = null, object cookies = null, bool replaceUnderscoreWithHyphen = true)
        {
            return RespondWith(() => new CapturedStringContent(body), status, headers, cookies, replaceUnderscoreWithHyphen);
        }

        /// <summary>
        /// Adds a fake HTTP response to the response queue with the given data serialized to JSON as the content body.
        /// </summary>
        /// <param name="body">The object to be JSON-serialized and used as the simulated response body.</param>
        /// <param name="status">The simulated HTTP status. Default is 200.</param>
        /// <param name="headers">The simulated response headers (optional).</param>
        /// <param name="cookies">The simulated response cookies (optional).</param>
        /// <param name="replaceUnderscoreWithHyphen">If true, underscores in property names of headers will be replaced by hyphens. Default is true.</param>
        /// <returns>The current HttpTest object (so more responses can be chained).</returns>
        public HttpTestSetup RespondWithJson(object body, int status = 200, object headers = null, object cookies = null, bool replaceUnderscoreWithHyphen = true)
        {
            var s = Settings.JsonSerializer.Serialize(body);
            return RespondWith(() => new CapturedJsonContent(s), status, headers, cookies, replaceUnderscoreWithHyphen);
        }

        /// <summary>
        /// Adds a fake HTTP response to the response queue.
        /// </summary>
        /// <param name="buildContent">A function that builds the simulated response body content. Optional.</param>
        /// <param name="status">The simulated HTTP status. Optional. Default is 200.</param>
        /// <param name="headers">The simulated response headers. Optional.</param>
        /// <param name="cookies">The simulated response cookies. Optional.</param>
        /// <param name="replaceUnderscoreWithHyphen">If true, underscores in property names of headers will be replaced by hyphens. Default is true.</param>
        /// <returns>The current HttpTest object (so more responses can be chained).</returns>
        public HttpTestSetup RespondWith(Func<HttpContent> buildContent = null, int status = 200, object headers = null, object cookies = null, bool replaceUnderscoreWithHyphen = true)
        {
            _responses.Add(() => {
                var response = new HttpResponseMessage
                {
                    StatusCode = (HttpStatusCode)status,
                    Content = buildContent?.Invoke()
                };

                if (headers != null)
                {
                    foreach (var kv in headers.ToKeyValuePairs())
                    {
                        var key = replaceUnderscoreWithHyphen ? kv.Key.Replace("_", "-") : kv.Key;
                        response.SetHeader(key, kv.Value.ToInvariantString());
                    }
                }

                if (cookies != null)
                {
                    foreach (var kv in cookies.ToKeyValuePairs())
                        response.Headers.Add("Set-Cookie", $"{kv.Key}={kv.Value}");
                }
                return response;
            });
            return this;
        }

        /// <summary>
        /// Adds a simulated timeout response to the response queue.
        /// </summary>
        public HttpTestSetup SimulateTimeout() =>
            SimulateException(new TaskCanceledException(null, new TimeoutException())); // inner exception covers the .net5+ case https://stackoverflow.com/a/65989456/62600

        /// <summary>
        /// Adds the throwing of an exception to the response queue.
        /// </summary>
        /// <param name="exception">The exception to throw when the call is simulated.</param>
        public HttpTestSetup SimulateException(Exception exception)
        {
            _responses.Add(() => throw exception);
            return this;
        }

        /// <summary>
        /// Do NOT fake requests for this setup. Typically called on a filtered setup, i.e. HttpTest.ForCallsTo(urlPattern).AllowRealHttp();
        /// </summary>
        public void AllowRealHttp()
        {
            _responses.Clear();
            _allowRealHttp = true;
        }
    }
    /// <summary>
    /// Interface for defining a strategy for creating, caching, and reusing IFlurlClient instances and
    /// their underlying HttpClient instances. It is generally preferable to derive from FlurlClientFactoryBase
    /// and only override methods as needed, rather than implementing this interface from scratch.
    /// </summary>
    public interface IFlurlClientFactory : IDisposable
    {
        /// <summary>
        /// Strategy to create a FlurlClient or reuse an existing one, based on the URL being called.
        /// </summary>
        /// <param name="url">The URL being called.</param>
        /// <returns></returns>
        IFlurlClient Get(FlurlModel url);

        /// <summary>
        /// Defines how HttpClient should be instantiated and configured by default. Do NOT attempt
        /// to cache/reuse HttpClient instances here - that should be done at the FlurlClient level
        /// via a custom FlurlClientFactory that gets registered globally.
        /// </summary>
        /// <param name="handler">The HttpMessageHandler used to construct the HttpClient.</param>
        /// <returns></returns>
        HttpClient CreateHttpClient(HttpMessageHandler handler);

        /// <summary>
        /// Defines how the HttpMessageHandler used by HttpClients that are created by
        /// this factory should be instantiated and configured. 
        /// </summary>
        /// <returns></returns>
        HttpMessageHandler CreateMessageHandler();
    }

    /// <summary>
    /// Extension methods on IFlurlClientFactory
    /// </summary>
    public static class FlurlClientFactoryExtensions
    {
        // https://stackoverflow.com/questions/51563732/how-do-i-lock-when-the-ideal-scope-of-the-lock-object-is-known-only-at-runtime
        private static readonly ConditionalWeakTable<IFlurlClient, object> _clientLocks = new ConditionalWeakTable<IFlurlClient, object>();

        /// <summary>
        /// Provides thread-safe access to a specific IFlurlClient, typically to configure settings and default headers.
        /// The URL is used to find the client, but keep in mind that the same client will be used in all calls to the same host by default.
        /// </summary>
        /// <param name="factory">This IFlurlClientFactory.</param>
        /// <param name="url">the URL used to find the IFlurlClient.</param>
        /// <param name="configAction">the action to perform against the IFlurlClient.</param>
        public static IFlurlClientFactory ConfigureClient(this IFlurlClientFactory factory, string url, Action<IFlurlClient> configAction)
        {
            var client = factory.Get(url);
            lock (_clientLocks.GetOrCreateValue(client))
            {
                configAction(client);
            }
            return factory;
        }

        /// <summary>
        /// Creates an HttpClient with the HttpMessageHandler returned from this factory's CreateMessageHandler method.
        /// </summary>
        public static HttpClient CreateHttpClient(this IFlurlClientFactory fac) => fac.CreateHttpClient(fac.CreateMessageHandler());
    }
    /// <summary>
    /// Defines stateful aspects (headers, cookies, etc) common to both IFlurlClient and IFlurlRequest
    /// </summary>
    public interface IHttpSettingsContainer
    {
        /// <summary>
        /// Gets the FlurlHttpSettings object used by this client or request.
        /// </summary>
        FlurlHttpSettings Settings { get; }

        /// <summary>
        /// Collection of headers sent on this request or all requests using this client.
        /// </summary>
        INameValueList<string> Headers { get; }
    }
    /// <summary>
    /// Contract for serializing and deserializing objects.
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Serializes an object to a string representation.
        /// </summary>
        string Serialize(object obj);
        /// <summary>
        /// Deserializes an object from a string representation.
        /// </summary>
        T Deserialize<T>(string s);
        /// <summary>
        /// Deserializes an object from a stream representation.
        /// </summary>
        T Deserialize<T>(Stream stream);
    }
    /// <summary>
    /// Fluent extension methods for sending multipart/form-data requests.
    /// </summary>
    public static class MultipartExtensions
    {
        /// <summary>
        /// Sends an asynchronous multipart/form-data POST request.
        /// </summary>
        /// <param name="buildContent">A delegate for building the content parts.</param>
        /// <param name="request">The IFlurlRequest.</param>
        /// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task whose result is the received IFlurlResponse.</returns>
        public static Task<IFlurlResponse> PostMultipartAsync(this IFlurlRequest request, Action<CapturedMultipartContent> buildContent, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
        {
            var cmc = new CapturedMultipartContent(request.Settings);
            buildContent(cmc);
            return request.SendAsync(HttpMethod.Post, cmc, completionOption, cancellationToken);
        }
    }
    /// <summary>
    /// An IFlurlClientFactory implementation that caches and reuses the same IFlurlClient instance
    /// per URL requested, which it assumes is a "base" URL, and sets the IFlurlClient.BaseUrl property
    /// to that value. Ideal for use with IoC containers - register as a singleton, inject into a service
    /// that wraps some web service, and use to set a private IFlurlClient field in the constructor.
    /// </summary>
    public class PerBaseUrlFlurlClientFactory : FlurlClientFactoryBase
    {
        /// <summary>
        /// Returns the entire URL, which is assumed to be some "base" URL for a service.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>The cache key</returns>
        protected override string GetCacheKey(FlurlModel url) => url.ToString();

        /// <summary>
        /// Returns a new new FlurlClient with BaseUrl set to the URL passed.
        /// </summary>
        /// <param name="url">The URL</param>
        /// <returns></returns>
        protected override IFlurlClient Create(FlurlModel url) => new FlurlClient(url);
    }
    /// <summary>
    /// A set of properties that affect Flurl.Http behavior specific to auto-redirecting.
    /// </summary>
    public class RedirectSettings
    {
        private readonly FlurlHttpSettings _settings;

        /// <summary>
        /// Creates a new instance of RedirectSettings.
        /// </summary>
        public RedirectSettings(FlurlHttpSettings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// If false, all of Flurl's mechanisms for handling redirects, including raising the OnRedirect event,
        /// are disabled entirely. This could also impact cookie functionality. Default is true. If you don't
        /// need Flurl's redirect or cookie functionality, or you are providing an HttpClient whose HttpClientHandler
        /// is providing these services, then it is safe to set this to false.
        /// </summary>
        public bool Enabled
        {
            get => _settings.Get<bool>("Redirects_Enabled");
            set => _settings.Set(value, "Redirects_Enabled");
        }

        /// <summary>
        /// If true, redirecting from HTTPS to HTTP is allowed. Default is false, as this behavior is considered
        /// insecure.
        /// </summary>
        public bool AllowSecureToInsecure
        {
            get => _settings.Get<bool>("Redirects_AllowSecureToInsecure");
            set => _settings.Set(value, "Redirects_AllowSecureToInsecure");
        }

        /// <summary>
        /// If true, request-level headers sent in the original request are forwarded in the redirect, with the
        /// exception of Authorization (use ForwardAuthorizationHeader) and Cookie (use a CookieJar). Also, any
        /// headers set on FlurlClient are automatically sent with all requests, including redirects. Default is true.
        /// </summary>
        public bool ForwardHeaders
        {
            get => _settings.Get<bool>("Redirects_ForwardHeaders");
            set => _settings.Set(value, "Redirects_ForwardHeaders");
        }

        /// <summary>
        /// If true, any Authorization header sent in the original request is forwarded in the redirect.
        /// Default is false, as this behavior is considered insecure.
        /// </summary>
        public bool ForwardAuthorizationHeader
        {
            get => _settings.Get<bool>("Redirects_ForwardAuthorizationHeader");
            set => _settings.Set(value, "Redirects_ForwardAuthorizationHeader");
        }

        /// <summary>
        /// Maximum number of redirects that Flurl will automatically follow in a single request. Default is 10.
        /// </summary>
        public int MaxAutoRedirects
        {
            get => _settings.Get<int>("Redirects_MaxRedirects");
            set => _settings.Set(value, "Redirects_MaxRedirects");
        }
    }
    /// <summary>
    /// ReceiveXXX extension methods off Task&lt;IFlurlResponse&gt; that allow chaining off methods like SendAsync
    /// without the need for nested awaits.
    /// </summary>
    public static class ResponseExtensions
    {
        /// <summary>
        /// Deserializes JSON-formatted HTTP response body to object of type T. Intended to chain off an async HTTP.
        /// </summary>
        /// <typeparam name="T">A type whose structure matches the expected JSON response.</typeparam>
        /// <returns>A Task whose result is an object containing data in the response body.</returns>
        /// <example>x = await url.PostAsync(data).ReceiveJson&lt;T&gt;()</example>
        /// <exception cref="FlurlHttpException">Condition.</exception>
        public static async Task<T> ReceiveJson<T>(this Task<IFlurlResponse> response)
        {
            using var resp = await response.ConfigureAwait(false);
            if (resp == null) return default;
            return await resp.GetJsonAsync<T>().ConfigureAwait(false);
        }

        /// <summary>
        /// Returns HTTP response body as a string. Intended to chain off an async call.
        /// </summary>
        /// <returns>A Task whose result is the response body as a string.</returns>
        /// <example>s = await url.PostAsync(data).ReceiveString()</example>
        public static async Task<string> ReceiveString(this Task<IFlurlResponse> response)
        {
            using var resp = await response.ConfigureAwait(false);
            if (resp == null) return null;
            return await resp.GetStringAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Returns HTTP response body as a stream. Intended to chain off an async call.
        /// </summary>
        /// <returns>A Task whose result is the response body as a stream.</returns>
        /// <example>stream = await url.PostAsync(data).ReceiveStream()</example>
        public static async Task<Stream> ReceiveStream(this Task<IFlurlResponse> response)
        {
            // don't wrap in a using, otherwise we'll dispose the stream too early.
            // we can dispose it if there's an error, otherwise the user is on the hook for it.
            var resp = await response.ConfigureAwait(false);
            if (resp == null) return null;
            try
            {
                return await resp.GetStreamAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {
                resp.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Returns HTTP response body as a byte array. Intended to chain off an async call.
        /// </summary>
        /// <returns>A Task whose result is the response body as a byte array.</returns>
        /// <example>bytes = await url.PostAsync(data).ReceiveBytes()</example>
        public static async Task<byte[]> ReceiveBytes(this Task<IFlurlResponse> response)
        {
            using var resp = await response.ConfigureAwait(false);
            if (resp == null) return null;
            return await resp.GetBytesAsync().ConfigureAwait(false);
        }
    }
    /// <summary>
    /// Fluent extension methods for tweaking FlurlHttpSettings
    /// </summary>
    public static class SettingsExtensions
    {
        /// <summary>
        /// Change FlurlHttpSettings for this IFlurlClient.
        /// </summary>
        /// <param name="client">The IFlurlClient.</param>
        /// <param name="action">Action defining the settings changes.</param>
        /// <returns>The IFlurlClient with the modified Settings</returns>
        public static IFlurlClient Configure(this IFlurlClient client, Action<FlurlHttpSettings> action)
        {
            action(client.Settings);
            return client;
        }

        /// <summary>
        /// Change FlurlHttpSettings for this IFlurlRequest.
        /// </summary>
        /// <param name="request">The IFlurlRequest.</param>
        /// <param name="action">Action defining the settings changes.</param>
        /// <returns>The IFlurlRequest with the modified Settings</returns>
        public static IFlurlRequest ConfigureRequest(this IFlurlRequest request, Action<FlurlHttpSettings> action)
        {
            action(request.Settings);
            return request;
        }

        /// <summary>
        /// Sets the timeout for this IFlurlRequest or all requests made with this IFlurlClient.
        /// </summary>
        /// <param name="obj">The IFlurlClient or IFlurlRequest.</param>
        /// <param name="timespan">Time to wait before the request times out.</param>
        /// <returns>This IFlurlClient or IFlurlRequest.</returns>
        public static T WithTimeout<T>(this T obj, TimeSpan timespan) where T : IHttpSettingsContainer
        {
            obj.Settings.Timeout = timespan;
            return obj;
        }

        /// <summary>
        /// Sets the timeout for this IFlurlRequest or all requests made with this IFlurlClient.
        /// </summary>
        /// <param name="obj">The IFlurlClient or IFlurlRequest.</param>
        /// <param name="seconds">Seconds to wait before the request times out.</param>
        /// <returns>This IFlurlClient or IFlurlRequest.</returns>
        public static T WithTimeout<T>(this T obj, int seconds) where T : IHttpSettingsContainer
        {
            obj.Settings.Timeout = TimeSpan.FromSeconds(seconds);
            return obj;
        }

        /// <summary>
        /// Adds a pattern representing an HTTP status code or range of codes which (in addition to 2xx) will NOT result in a FlurlHttpException being thrown.
        /// </summary>
        /// <param name="obj">The IFlurlClient or IFlurlRequest.</param>
        /// <param name="pattern">Examples: "3xx", "100,300,600", "100-299,6xx"</param>
        /// <returns>This IFlurlClient or IFlurlRequest.</returns>
        public static T AllowHttpStatus<T>(this T obj, string pattern) where T : IHttpSettingsContainer
        {
            if (!string.IsNullOrWhiteSpace(pattern))
            {
                var current = obj.Settings.AllowedHttpStatusRange;
                if (string.IsNullOrWhiteSpace(current))
                    obj.Settings.AllowedHttpStatusRange = pattern;
                else
                    obj.Settings.AllowedHttpStatusRange += "," + pattern;
            }
            return obj;
        }

        /// <summary>
        /// Adds an <see cref="HttpStatusCode" /> which (in addition to 2xx) will NOT result in a FlurlHttpException being thrown.
        /// </summary>
        /// <param name="obj">The IFlurlClient or IFlurlRequest.</param>
        /// <param name="statusCodes">Examples: HttpStatusCode.NotFound</param>
        /// <returns>This IFlurlClient or IFlurlRequest.</returns>
        public static T AllowHttpStatus<T>(this T obj, params HttpStatusCode[] statusCodes) where T : IHttpSettingsContainer
        {
            var pattern = string.Join(",", statusCodes.Select(c => (int)c));
            return AllowHttpStatus(obj, pattern);
        }

        /// <summary>
        /// Prevents a FlurlHttpException from being thrown on any completed response, regardless of the HTTP status code.
        /// </summary>
        /// <param name="obj">The IFlurlClient or IFlurlRequest.</param>
        /// <returns>This IFlurlClient or IFlurlRequest.</returns>
        public static T AllowAnyHttpStatus<T>(this T obj) where T : IHttpSettingsContainer
        {
            obj.Settings.AllowedHttpStatusRange = "*";
            return obj;
        }

        /// <summary>
        /// Configures whether redirects are automatically followed.
        /// </summary>
        /// <param name="obj">The IFlurlClient or IFlurlRequest.</param>
        /// <param name="enabled">true if Flurl should automatically send a new request to the redirect URL, false if it should not.</param>
        /// <returns>This IFlurlClient or IFlurlRequest.</returns>
        public static T WithAutoRedirect<T>(this T obj, bool enabled) where T : IHttpSettingsContainer
        {
            obj.Settings.Redirects.Enabled = enabled;
            return obj;
        }

        /// <summary>
        /// Sets a callback that is invoked immediately before every HTTP request is sent.
        /// </summary>
        public static T BeforeCall<T>(this T obj, Action<FlurlCall> act) where T : IHttpSettingsContainer
        {
            obj.Settings.BeforeCall = act;
            return obj;
        }

        /// <summary>
        /// Sets a callback that is invoked asynchronously immediately before every HTTP request is sent.
        /// </summary>
        public static T BeforeCall<T>(this T obj, Func<FlurlCall, Task> act) where T : IHttpSettingsContainer
        {
            obj.Settings.BeforeCallAsync = act;
            return obj;
        }

        /// <summary>
        /// Sets a callback that is invoked immediately after every HTTP response is received.
        /// </summary>
        public static T AfterCall<T>(this T obj, Action<FlurlCall> act) where T : IHttpSettingsContainer
        {
            obj.Settings.AfterCall = act;
            return obj;
        }

        /// <summary>
        /// Sets a callback that is invoked asynchronously immediately after every HTTP response is received.
        /// </summary>
        public static T AfterCall<T>(this T obj, Func<FlurlCall, Task> act) where T : IHttpSettingsContainer
        {
            obj.Settings.AfterCallAsync = act;
            return obj;
        }

        /// <summary>
        /// Sets a callback that is invoked when an error occurs during any HTTP call, including when any non-success
        /// HTTP status code is returned in the response. Response should be null-checked if used in the event handler.
        /// </summary>
        public static T OnError<T>(this T obj, Action<FlurlCall> act) where T : IHttpSettingsContainer
        {
            obj.Settings.OnError = act;
            return obj;
        }

        /// <summary>
        /// Sets a callback that is invoked asynchronously when an error occurs during any HTTP call, including when any non-success
        /// HTTP status code is returned in the response. Response should be null-checked if used in the event handler.
        /// </summary>
        public static T OnError<T>(this T obj, Func<FlurlCall, Task> act) where T : IHttpSettingsContainer
        {
            obj.Settings.OnErrorAsync = act;
            return obj;
        }

        /// <summary>
        /// Sets a callback that is invoked when any 3xx response with a Location header is received.
        /// You can inspect/manipulate the call.Redirect object to determine what will happen next.
        /// An auto-redirect will only happen if call.Redirect.Follow is true upon exiting the callback.
        /// </summary>
        public static T OnRedirect<T>(this T obj, Action<FlurlCall> act) where T : IHttpSettingsContainer
        {
            obj.Settings.OnRedirect = act;
            return obj;
        }

        /// <summary>
        /// Sets a callback that is invoked asynchronously when any 3xx response with a Location header is received.
        /// You can inspect/manipulate the call.Redirect object to determine what will happen next.
        /// An auto-redirect will only happen if call.Redirect.Follow is true upon exiting the callback.
        /// </summary>
        public static T OnRedirect<T>(this T obj, Func<FlurlCall, Task> act) where T : IHttpSettingsContainer
        {
            obj.Settings.OnRedirectAsync = act;
            return obj;
        }
    }
    /// <summary>
    /// URL builder extension methods on FlurlRequest
    /// </summary>
    public static class UrlBuilderExtensions
    {
        /// <summary>
        /// Appends a segment to the URL path, ensuring there is one and only one '/' character as a seperator.
        /// </summary>
        /// <param name="request">The IFlurlRequest associated with the URL</param>
        /// <param name="segment">The segment to append</param>
        /// <param name="fullyEncode">If true, URL-encodes reserved characters such as '/', '+', and '%'. Otherwise, only encodes strictly illegal characters (including '%' but only when not followed by 2 hex characters).</param>
        /// <returns>This IFlurlRequest</returns>
        /// <exception cref="ArgumentNullException"><paramref name="segment"/> is <see langword="null" />.</exception>
        public static IFlurlRequest AppendPathSegment(this IFlurlRequest request, object segment, bool fullyEncode = false)
        {
            request.Url.AppendPathSegment(segment, fullyEncode);
            return request;
        }

        /// <summary>
        /// Appends multiple segments to the URL path, ensuring there is one and only one '/' character as a seperator.
        /// </summary>
        /// <param name="request">The IFlurlRequest associated with the URL</param>
        /// <param name="segments">The segments to append</param>
        /// <returns>This IFlurlRequest</returns>
        public static IFlurlRequest AppendPathSegments(this IFlurlRequest request, params object[] segments)
        {
            request.Url.AppendPathSegments(segments);
            return request;
        }

        /// <summary>
        /// Appends multiple segments to the URL path, ensuring there is one and only one '/' character as a seperator.
        /// </summary>
        /// <param name="request">The IFlurlRequest associated with the URL</param>
        /// <param name="segments">The segments to append</param>
        /// <returns>This IFlurlRequest</returns>
        public static IFlurlRequest AppendPathSegments(this IFlurlRequest request, IEnumerable<object> segments)
        {
            request.Url.AppendPathSegments(segments);
            return request;
        }

        /// <summary>
        /// Adds a parameter to the URL query, overwriting the value if name exists.
        /// </summary>
        /// <param name="request">The IFlurlRequest associated with the URL</param>
        /// <param name="name">Name of query parameter</param>
        /// <param name="value">Value of query parameter</param>
        /// <param name="nullValueHandling">Indicates how to handle null values. Defaults to Remove (any existing)</param>
        /// <returns>This IFlurlRequest</returns>
        public static IFlurlRequest SetQueryParam(this IFlurlRequest request, string name, object value, NullValueHandling nullValueHandling = NullValueHandling.Remove)
        {
            request.Url.SetQueryParam(name, value, nullValueHandling);
            return request;
        }

        /// <summary>
        /// Adds a parameter to the URL query, overwriting the value if name exists.
        /// </summary>
        /// <param name="request">The IFlurlRequest associated with the URL</param>
        /// <param name="name">Name of query parameter</param>
        /// <param name="value">Value of query parameter</param>
        /// <param name="isEncoded">Set to true to indicate the value is already URL-encoded</param>
        /// <param name="nullValueHandling">Indicates how to handle null values. Defaults to Remove (any existing)</param>
        /// <returns>This IFlurlRequest</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null" />.</exception>
        public static IFlurlRequest SetQueryParam(this IFlurlRequest request, string name, string value, bool isEncoded = false, NullValueHandling nullValueHandling = NullValueHandling.Remove)
        {
            request.Url.SetQueryParam(name, value, isEncoded, nullValueHandling);
            return request;
        }

        /// <summary>
        /// Adds a parameter without a value to the URL query, removing any existing value.
        /// </summary>
        /// <param name="request">The IFlurlRequest associated with the URL</param>
        /// <param name="name">Name of query parameter</param>
        /// <returns>This IFlurlRequest</returns>
        public static IFlurlRequest SetQueryParam(this IFlurlRequest request, string name)
        {
            request.Url.SetQueryParam(name);
            return request;
        }

        /// <summary>
        /// Parses values (usually an anonymous object or dictionary) into name/value pairs and adds them to the URL query, overwriting any that already exist.
        /// </summary>
        /// <param name="request">The IFlurlRequest associated with the URL</param>
        /// <param name="values">Typically an anonymous object, ie: new { x = 1, y = 2 }</param>
        /// <param name="nullValueHandling">Indicates how to handle null values. Defaults to Remove (any existing)</param>
        /// <returns>This IFlurlRequest</returns>
        public static IFlurlRequest SetQueryParams(this IFlurlRequest request, object values, NullValueHandling nullValueHandling = NullValueHandling.Remove)
        {
            request.Url.SetQueryParams(values, nullValueHandling);
            return request;
        }

        /// <summary>
        /// Adds multiple parameters without values to the URL query.
        /// </summary>
        /// <param name="request">The IFlurlRequest associated with the URL</param>
        /// <param name="names">Names of query parameters.</param>
        /// <returns>This IFlurlRequest</returns>
        public static IFlurlRequest SetQueryParams(this IFlurlRequest request, IEnumerable<string> names)
        {
            request.Url.SetQueryParams(names);
            return request;
        }

        /// <summary>
        /// Adds multiple parameters without values to the URL query.
        /// </summary>
        /// <param name="request">The IFlurlRequest associated with the URL</param>
        /// <param name="names">Names of query parameters</param>
        /// <returns>This IFlurlRequest</returns>
        public static IFlurlRequest SetQueryParams(this IFlurlRequest request, params string[] names)
        {
            request.Url.SetQueryParams(names as IEnumerable<string>);
            return request;
        }

        /// <summary>
        /// Removes a name/value pair from the URL query by name.
        /// </summary>
        /// <param name="request">The IFlurlRequest associated with the URL</param>
        /// <param name="name">Query string parameter name to remove</param>
        /// <returns>This IFlurlRequest</returns>
        public static IFlurlRequest RemoveQueryParam(this IFlurlRequest request, string name)
        {
            request.Url.RemoveQueryParam(name);
            return request;
        }

        /// <summary>
        /// Removes multiple name/value pairs from the URL query by name.
        /// </summary>
        /// <param name="request">The IFlurlRequest associated with the URL</param>
        /// <param name="names">Query string parameter names to remove</param>
        /// <returns>This IFlurlRequest</returns>
        public static IFlurlRequest RemoveQueryParams(this IFlurlRequest request, params string[] names)
        {
            request.Url.RemoveQueryParams(names);
            return request;
        }

        /// <summary>
        /// Removes multiple name/value pairs from the URL query by name.
        /// </summary>
        /// <param name="request">The IFlurlRequest associated with the URL</param>
        /// <param name="names">Query string parameter names to remove</param>
        /// <returns>This IFlurlRequest</returns>
        public static IFlurlRequest RemoveQueryParams(this IFlurlRequest request, IEnumerable<string> names)
        {
            request.Url.RemoveQueryParams(names);
            return request;
        }

        /// <summary>
        /// Set the URL fragment fluently.
        /// </summary>
        /// <param name="request">The IFlurlRequest associated with the URL</param>
        /// <param name="fragment">The part of the URL afer #</param>
        /// <returns>This IFlurlRequest</returns>
        public static IFlurlRequest SetFragment(this IFlurlRequest request, string fragment)
        {
            request.Url.SetFragment(fragment);
            return request;
        }

        /// <summary>
        /// Removes the URL fragment including the #.
        /// </summary>
        /// <param name="request">The IFlurlRequest associated with the URL</param>
        /// <returns>This IFlurlRequest</returns>
        public static IFlurlRequest RemoveFragment(this IFlurlRequest request)
        {
            request.Url.RemoveFragment();
            return request;
        }
    }
    /// <summary>
    /// Utility methods used by both HttpTestSetup and HttpTestAssertion
    /// </summary>
    internal static class Util
    {
        internal static bool HasAnyVerb(this FlurlCall call, HttpMethod[] verbs)
        {
            // for good measure, check both FlurlRequest.Verb and HttpRequestMessage.Method
            return verbs.Any(verb => call.Request.Verb == verb && call.HttpRequestMessage.Method == verb);
        }

        internal static bool HasAnyVerb(this FlurlCall call, string[] verbs)
        {
            // for good measure, check both FlurlRequest.Verb and HttpRequestMessage.Method
            return verbs.Any(verb =>
                call.Request.Verb.Method.OrdinalEquals(verb, true) &&
                call.HttpRequestMessage.Method.Method.OrdinalEquals(verb, true));
        }

        /// <summary>
        /// null value means just check for existence by name
        /// </summary>
        internal static bool HasQueryParam(this FlurlCall call, string name, object value = null)
        {
            if (value == null)
                return call.Request.Url.QueryParams.Contains(name);

            var paramVals = call.Request.Url.QueryParams
                .Where(p => p.Name == name)
                .Select(p => p.Value.ToInvariantString())
                .ToList();

            if (!paramVals.Any())
                return false;
            if (!(value is string) && value is IEnumerable en)
            {
                var values = en.Cast<object>().Select(o => o.ToInvariantString()).ToList();
                return values.Intersect(paramVals).Count() == values.Count;
            }
            return paramVals.Any(v => MatchesValueOrPattern(v, value));
        }

        internal static bool HasAllQueryParams(this FlurlCall call, string[] names)
        {
            return call.Request.Url.QueryParams
               .Select(p => p.Name)
               .Intersect(names)
               .Count() == names.Length;
        }

        internal static bool HasAnyQueryParam(this FlurlCall call, string[] names)
        {
            var qp = call.Request.Url.QueryParams;
            return names.Any() ? qp
               .Select(p => p.Name)
               .Intersect(names)
               .Any() : qp.Any();
        }

        internal static bool HasQueryParams(this FlurlCall call, object values)
        {
            return values.ToKeyValuePairs().All(kv => call.HasQueryParam(kv.Key, kv.Value));
        }

        /// <summary>
        /// null value means just check for existence by name
        /// </summary>
        internal static bool HasHeader(this FlurlCall call, string name, object value = null)
        {
            return (value == null) ?
                call.Request.Headers.Contains(name) :
                call.Request.Headers.TryGetFirst(name, out var val) && MatchesValueOrPattern(val, value);
        }

        /// <summary>
        /// null value means just check for existence by name
        /// </summary>
        internal static bool HasCookie(this FlurlCall call, string name, object value = null)
        {
            return (value == null) ?
                call.Request.Cookies.Any(c => c.Name == name) :
                MatchesValueOrPattern(call.Request.Cookies.FirstOrDefault(c => c.Name == name).Value, value);
        }

        private static bool MatchesValueOrPattern(object valueToMatch, object value)
        {
            if (valueToMatch is string pattern && value is string s)
                return MatchesPattern(pattern, s);
            // string match is good enough
            return valueToMatch?.ToInvariantString() == value?.ToInvariantString();
        }

        /// <summary>
        /// same as MatchesPattern, but doesn't require trailing * to ignore query string
        /// </summary>
        internal static bool MatchesUrlPattern(string url, string pattern)
        {
            if (MatchesPattern(url, pattern))
                return true;
            if (pattern.OrdinalEndsWith("*"))
                return false;
            if (pattern.OrdinalContains("?"))
                return MatchesPattern(url, pattern + "&*");
            else
                return MatchesPattern(url, pattern + "?*");
        }

        /// <summary>
        /// match simple patterns with * wildcard
        /// </summary>
        internal static bool MatchesPattern(string textToCheck, string pattern)
        {
            // avoid regex'ing in simple cases
            if (string.IsNullOrEmpty(pattern) || pattern == "*") return true;
            if (string.IsNullOrEmpty(textToCheck)) return false;
            if (!pattern.OrdinalContains("*")) return textToCheck == pattern;

            var regex = "^" + Regex.Escape(pattern).Replace("\\*", "(.*)") + "$";
            return Regex.IsMatch(textToCheck ?? "", regex);
        }
    }
}
