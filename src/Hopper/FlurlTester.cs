using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Hopper
{
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
                With(call => urlPatterns.Any(p => FlurlExtensions.MatchesUrlPattern(call.Request.Url, p)));
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
            return With(call => FlurlExtensions.MatchesPattern(call.RequestBody, pattern));
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
            return With(c => FlurlExtensions.MatchesUrlPattern(c.Request.Url, urlPattern), $"URL pattern {urlPattern}");
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
            return With(c => FlurlExtensions.MatchesPattern(c.RequestBody, bodyPattern), $"body {bodyPattern}");
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
                        FlurlExtensions.MatchesPattern(creds[0], username) &&
                        FlurlExtensions.MatchesPattern(creds[1], password);
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
    }    /// <summary>
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
}
