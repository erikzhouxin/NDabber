using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Data.Cobber;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace System.Data.Hopper
{
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
        IFlurlClient Get(Flurl url);

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
        public virtual IFlurlClient Get(Flurl url)
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
        protected abstract string GetCacheKey(Flurl url);

        /// <summary>
        /// Creates a new FlurlClient
        /// </summary>
        /// <param name="url">The URL (not used)</param>
        /// <returns></returns>
        protected virtual IFlurlClient Create(Flurl url) => new FlurlClient();

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
        protected override string GetCacheKey(Flurl url) => $"{url.Scheme}|{url.Host}|{url.Port}";
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
        protected override string GetCacheKey(Flurl url) => url.ToString();

        /// <summary>
        /// Returns a new new FlurlClient with BaseUrl set to the URL passed.
        /// </summary>
        /// <param name="url">The URL</param>
        /// <returns></returns>
        protected override IFlurlClient Create(Flurl url) => new FlurlClient(url);
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
}
