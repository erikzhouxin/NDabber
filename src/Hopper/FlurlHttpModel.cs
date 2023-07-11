using System.Collections;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Data.Cobber;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Hopper
{
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
        public Flurl Url { get; set; }

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
            if (!Flurl.IsValid(request.Url))
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

            if (Flurl.IsValid(location))
                redir.Url = new Flurl(location);
            else if (location.OrdinalStartsWith("//"))
                redir.Url = new Flurl(call.Request.Url.Scheme + ":" + location);
            else if (location.OrdinalStartsWith("/"))
                redir.Url = Flurl.Combine(call.Request.Url.Root, location);
            else
                redir.Url = Flurl.Combine(call.Request.Url.Root, call.Request.Url.Path, location);

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
        public Flurl OriginUrl { get; }

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
        Flurl Url { get; set; }

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
        public FlurlRequest(Flurl url = null)
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
            if (!Flurl.IsValid(parts.FirstOrDefault()) && !string.IsNullOrEmpty(baseUrl))
                parts.Insert(0, baseUrl);

            if (parts.Any())
                Url = Flurl.Combine(parts.ToArray());
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
        public Flurl Url { get; set; }

        /// <inheritdoc />
        public HttpContent Content { get; set; }

        /// <inheritdoc />
        public FlurlCall RedirectedFrom { get; set; }

        /// <inheritdoc />
        public INameValueList<string> Headers { get; } = new NameValueList<string>(false); // header names are case-insensitive https://stackoverflow.com/a/5259004/62600

        /// <inheritdoc />
        public IEnumerable<(string Name, string Value)> Cookies =>
            FlurlExtensions.ParseRequestHeader(Headers.FirstOrDefault("Cookie"));

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
                headerValues.Select(hv => FlurlExtensions.ParseResponseHeader(url, hv)).ToList() :
                new List<FlurlCookie>());
#else
			return ResponseMessage.Headers.TryGetValues("Set-Cookie", out var headerValues) ?
				headerValues.Select(hv => FlurlExtensions.ParseResponseHeader(url, hv)).ToList() :
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
}
