using System.Collections;
using System.Text.RegularExpressions;

namespace System.Data.Hopper
{
    /// <summary>
    /// A mutable object for fluently building and parsing URLs.
    /// </summary>
    public class Flurl
    {
        private string _originalString;
        private bool _parsed;

        private string _scheme;
        private string _userInfo;
        private string _host;
        private List<string> _pathSegments;
        private QueryParamCollection _queryParams;
        private string _fragment;
        private int? _port;
        private bool _leadingSlash;
        private bool _trailingSlash;
        private bool _trailingQmark;
        private bool _trailingHash;

        #region public properties
        /// <summary>
        /// The scheme of the URL, i.e. "http". Does not include ":" delimiter. Empty string if the URL is relative.
        /// </summary>
        public string Scheme
        {
            get => EnsureParsed()._scheme;
            set => EnsureParsed()._scheme = value;
        }

        /// <summary>
        /// i.e. "user:pass" in "https://user:pass@www.site.com". Empty string if not present.
        /// </summary>
        public string UserInfo
        {
            get => EnsureParsed()._userInfo;
            set => EnsureParsed()._userInfo = value;
        }

        /// <summary>
        /// i.e. "www.site.com" in "https://www.site.com:8080/path". Does not include user info or port.
        /// </summary>
        public string Host
        {
            get => EnsureParsed()._host;
            set => EnsureParsed()._host = value;
        }

        /// <summary>
        /// Port number of the URL. Null if not explicitly specified.
        /// </summary>
        public int? Port
        {
            get => EnsureParsed()._port;
            set => EnsureParsed()._port = value;
        }

        /// <summary>
        /// i.e. "www.site.com:8080" in "https://www.site.com:8080/path". Includes both user info and port, if included.
        /// </summary>
        public string Authority => string.Concat(
            UserInfo,
            UserInfo?.Length > 0 ? "@" : "",
            Host,
            Port.HasValue ? ":" : "",
            Port);

        /// <summary>
        /// i.e. "https://www.site.com:8080" in "https://www.site.com:8080/path" (everything before the path).
        /// </summary>
        public string Root => string.Concat(
            Scheme,
            Scheme?.Length > 0 ? ":" : "",
            Authority?.Length > 0 ? "//" : "",
            Authority);

        /// <summary>
        /// i.e. "/path" in "https://www.site.com/path". Empty string if not present. Leading and trailing "/" retained exactly as specified by user.
        /// </summary>
        public string Path
        {
            get
            {
                EnsureParsed();
                return string.Concat(
                    _leadingSlash ? "/" : "",
                    string.Join("/", PathSegments),
                    _trailingSlash && PathSegments.Any() ? "/" : "");
            }
            set
            {
                PathSegments.Clear();
                _trailingSlash = false;
                if (string.IsNullOrEmpty(value))
                    _leadingSlash = false;
                else if (value == "/")
                    _leadingSlash = true;
                else
                    AppendPathSegment(value ?? "");
            }
        }

        /// <summary>
        /// The "/"-delimited segments of the path, not including leading or trailing "/" characters.
        /// </summary>
        public IList<string> PathSegments => EnsureParsed()._pathSegments;

        /// <summary>
        /// i.e. "x=1&amp;y=2" in "https://www.site.com/path?x=1&amp;y=2". Does not include "?".
        /// </summary>
        public string Query
        {
            get => QueryParams.ToString();
            set => EnsureParsed()._queryParams = new QueryParamCollection(value);
        }

        /// <summary>
        /// Query parsed to name/value pairs.
        /// </summary>
        public QueryParamCollection QueryParams => EnsureParsed()._queryParams;

        /// <summary>
        /// i.e. "frag" in "https://www.site.com/path?x=y#frag". Does not include "#".
        /// </summary>
        public string Fragment
        {
            get => EnsureParsed()._fragment;
            set => EnsureParsed()._fragment = value;
        }

        /// <summary>
        /// True if URL does not start with a non-empty scheme. i.e. false for "https://www.site.com", true for "//www.site.com".
        /// </summary>
        public bool IsRelative => string.IsNullOrEmpty(Scheme);

        /// <summary>
        /// True if FlurlModel is absolute and scheme is https or wss.
        /// </summary>
        public bool IsSecureScheme => !IsRelative && (Scheme.OrdinalEquals("https", true) || Scheme.OrdinalEquals("wss", true));
        #endregion

        #region ctors and parsing methods
        /// <summary>
        /// Constructs a FlurlModel object from a string.
        /// </summary>
        /// <param name="baseUrl">The URL to use as a starting point.</param>
        public Flurl(string baseUrl = null)
        {
            _originalString = baseUrl?.Trim();
        }

        /// <summary>
        /// Constructs a FlurlModel object from a System.Uri.
        /// </summary>
        /// <param name="uri">The System.Uri (required)</param>
        /// <exception cref="ArgumentNullException"><paramref name="uri"/> is <see langword="null" />.</exception>
        public Flurl(Uri uri)
        {
            _originalString = (uri ?? throw new ArgumentNullException(nameof(uri))).OriginalString;
            ParseInternal(uri); // parse eagerly, taking advantage of the fact that we already have a parsed Uri
        }

        /// <summary>
        /// Parses a URL string into a Flurl.FlurlModel object.
        /// </summary>
        public static Flurl Parse(string url) => new Flurl(url).ParseInternal();

        private Flurl EnsureParsed() => _parsed ? this : ParseInternal();

        private Flurl ParseInternal(Uri uri = null)
        {
            _parsed = true;

            uri = uri ?? new Uri(_originalString ?? "", UriKind.RelativeOrAbsolute);

            if (uri.OriginalString.OrdinalStartsWith("//"))
            {
                ParseInternal(new Uri("http:" + uri.OriginalString));
                _scheme = "";
            }
            else if (uri.OriginalString.OrdinalStartsWith("/"))
            {
                ParseInternal(new Uri("http://temp.com" + uri.OriginalString));
                _scheme = "";
                _host = "";
                _leadingSlash = true;
            }
            else if (uri.IsAbsoluteUri)
            {
                _scheme = uri.Scheme;
                _userInfo = uri.UserInfo;
                _host = uri.Host;
                _port = _originalString?.OrdinalStartsWith($"{Root}:{uri.Port}", ignoreCase: true) == true ? uri.Port : (int?)null; // don't default Port if not included explicitly
                _pathSegments = new List<string>();
                if (uri.AbsolutePath.Length > 0 && uri.AbsolutePath != "/")
                    AppendPathSegment(uri.AbsolutePath);
                _queryParams = new QueryParamCollection(uri.Query);
                _fragment = uri.Fragment.TrimStart('#'); // quirk - formal def of fragment does not include the #

                _leadingSlash = uri.OriginalString.OrdinalStartsWith(Root + "/", ignoreCase: true);
                _trailingSlash = _pathSegments.Any() && uri.AbsolutePath.OrdinalEndsWith("/");
                _trailingQmark = uri.Query == "?";
                _trailingHash = uri.Fragment == "#";

                // more quirk fixes
                var hasAuthority = uri.OriginalString.OrdinalStartsWith($"{Scheme}://", ignoreCase: true);
                if (hasAuthority && Authority.Length == 0 && PathSegments.Any())
                {
                    // Uri didn't parse Authority when it should have
                    _host = _pathSegments[0];
                    _pathSegments.RemoveAt(0);
                }
                else if (!hasAuthority && Authority.Length > 0)
                {
                    // Uri parsed Authority when it should not have
                    _pathSegments.Insert(0, Authority);
                    _userInfo = "";
                    _host = "";
                    _port = null;
                }
            }
            // if it's relative, System.Uri refuses to parse any of it. these hacks will force the matter
            else
            {
                ParseInternal(new Uri("http://temp.com/" + uri.OriginalString));
                _scheme = "";
                _host = "";
                _leadingSlash = false;
            }

            return this;
        }

        /// <summary>
        /// Parses a URL query to a QueryParamCollection.
        /// </summary>
        /// <param name="query">The URL query to parse.</param>
        public static QueryParamCollection ParseQueryParams(string query) => new QueryParamCollection(query);

        /// <summary>
        /// Splits the given path into segments, encoding illegal characters, "?", and "#".
        /// </summary>
        /// <param name="path">The path to split.</param>
        /// <returns></returns>
        public static IEnumerable<string> ParsePathSegments(string path)
        {
            var segments = EncodeIllegalCharacters(path)
                .Replace("?", "%3F")
                .Replace("#", "%23")
                .Split('/');

            if (!segments.Any())
                yield break;

            // skip first and/or last segment if either empty, but not any in between. "///" should return 2 empty segments for example. 
            var start = segments.First().Length > 0 ? 0 : 1;
            var count = segments.Length - (segments.Last().Length > 0 ? 0 : 1);

            for (var i = start; i < count; i++)
                yield return segments[i];
        }
        #endregion

        #region fluent builder methods
        /// <summary>
        /// Appends a segment to the URL path, ensuring there is one and only one '/' character as a separator.
        /// </summary>
        /// <param name="segment">The segment to append</param>
        /// <param name="fullyEncode">If true, URL-encodes reserved characters such as '/', '+', and '%'. Otherwise, only encodes strictly illegal characters (including '%' but only when not followed by 2 hex characters).</param>
        /// <returns>the FlurlModel object with the segment appended</returns>
        /// <exception cref="ArgumentNullException"><paramref name="segment"/> is <see langword="null" />.</exception>
        public Flurl AppendPathSegment(object segment, bool fullyEncode = false)
        {
            if (segment == null)
                throw new ArgumentNullException(nameof(segment));

            EnsureParsed();

            if (fullyEncode)
            {
                PathSegments.Add(Uri.EscapeDataString(segment.ToInvariantString()));
                _trailingSlash = false;
            }
            else
            {
                var subpath = segment.ToInvariantString();
                foreach (var s in ParsePathSegments(subpath))
                    PathSegments.Add(s);
                _trailingSlash = subpath.OrdinalEndsWith("/");
            }

            _leadingSlash |= !IsRelative;
            return this;
        }

        /// <summary>
        /// Appends multiple segments to the URL path, ensuring there is one and only one '/' character as a seperator.
        /// </summary>
        /// <param name="segments">The segments to append</param>
        /// <returns>the FlurlModel object with the segments appended</returns>
        public Flurl AppendPathSegments(params object[] segments)
        {
            foreach (var segment in segments)
                AppendPathSegment(segment);

            return this;
        }

        /// <summary>
        /// Appends multiple segments to the URL path, ensuring there is one and only one '/' character as a seperator.
        /// </summary>
        /// <param name="segments">The segments to append</param>
        /// <returns>the FlurlModel object with the segments appended</returns>
        public Flurl AppendPathSegments(IEnumerable<object> segments)
        {
            foreach (var s in segments)
                AppendPathSegment(s);

            return this;
        }

        /// <summary>
        /// Removes the last path segment from the URL.
        /// </summary>
        /// <returns>The FlurlModel object.</returns>
        public Flurl RemovePathSegment()
        {
            if (PathSegments.Any())
                PathSegments.RemoveAt(PathSegments.Count - 1);
            return this;
        }

        /// <summary>
        /// Removes the entire path component of the URL, including the leading slash.
        /// </summary>
        /// <returns>The FlurlModel object.</returns>
        public Flurl RemovePath()
        {
            PathSegments.Clear();
            _leadingSlash = _trailingSlash = false;
            return this;
        }

        /// <summary>
        /// Adds a parameter to the query, overwriting the value if name exists.
        /// </summary>
        /// <param name="name">Name of query parameter</param>
        /// <param name="value">Value of query parameter</param>
        /// <param name="nullValueHandling">Indicates how to handle null values. Defaults to Remove (any existing)</param>
        /// <returns>The FlurlModel object with the query parameter added</returns>
        public Flurl SetQueryParam(string name, object value, NullValueHandling nullValueHandling = NullValueHandling.Remove)
        {
            QueryParams.AddOrReplace(name, value, false, nullValueHandling);
            return this;
        }

        /// <summary>
        /// Adds a parameter to the query, overwriting the value if name exists.
        /// </summary>
        /// <param name="name">Name of query parameter</param>
        /// <param name="value">Value of query parameter</param>
        /// <param name="isEncoded">Set to true to indicate the value is already URL-encoded</param>
        /// <param name="nullValueHandling">Indicates how to handle null values. Defaults to Remove (any existing)</param>
        /// <returns>The FlurlModel object with the query parameter added</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null" />.</exception>
        public Flurl SetQueryParam(string name, string value, bool isEncoded = false, NullValueHandling nullValueHandling = NullValueHandling.Remove)
        {
            QueryParams.AddOrReplace(name, value, isEncoded, nullValueHandling);
            return this;
        }

        /// <summary>
        /// Adds a parameter without a value to the query, removing any existing value.
        /// </summary>
        /// <param name="name">Name of query parameter</param>
        /// <returns>The FlurlModel object with the query parameter added</returns>
        public Flurl SetQueryParam(string name)
        {
            QueryParams.AddOrReplace(name, null, false, NullValueHandling.NameOnly);
            return this;
        }

        /// <summary>
        /// Parses values (usually an anonymous object or dictionary) into name/value pairs and adds them to the query, overwriting any that already exist.
        /// </summary>
        /// <param name="values">Typically an anonymous object, ie: new { x = 1, y = 2 }</param>
        /// <param name="nullValueHandling">Indicates how to handle null values. Defaults to Remove (any existing)</param>
        /// <returns>The FlurlModel object with the query parameters added</returns>
        public Flurl SetQueryParams(object values, NullValueHandling nullValueHandling = NullValueHandling.Remove)
        {
            if (values == null)
                return this;

            if (values is string s)
                return SetQueryParam(s);

            foreach (var kv in values.ToKeyValuePairs())
                SetQueryParam(kv.Key, kv.Value, nullValueHandling);

            return this;
        }

        /// <summary>
        /// Adds multiple parameters without values to the query.
        /// </summary>
        /// <param name="names">Names of query parameters.</param>
        /// <returns>The FlurlModel object with the query parameter added</returns>
        public Flurl SetQueryParams(IEnumerable<string> names)
        {
            if (names == null)
                return this;

            foreach (var name in names.Where(n => !string.IsNullOrEmpty(n)))
                SetQueryParam(name);

            return this;
        }

        /// <summary>
        /// Adds multiple parameters without values to the query.
        /// </summary>
        /// <param name="names">Names of query parameters</param>
        /// <returns>The FlurlModel object with the query parameter added.</returns>
        public Flurl SetQueryParams(params string[] names) => SetQueryParams(names as IEnumerable<string>);

        /// <summary>
        /// Removes a name/value pair from the query by name.
        /// </summary>
        /// <param name="name">Query string parameter name to remove</param>
        /// <returns>The FlurlModel object with the query parameter removed</returns>
        public Flurl RemoveQueryParam(string name)
        {
            QueryParams.Remove(name);
            return this;
        }

        /// <summary>
        /// Removes multiple name/value pairs from the query by name.
        /// </summary>
        /// <param name="names">Query string parameter names to remove</param>
        /// <returns>The FlurlModel object.</returns>
        public Flurl RemoveQueryParams(params string[] names)
        {
            foreach (var name in names)
                QueryParams.Remove(name);
            return this;
        }

        /// <summary>
        /// Removes multiple name/value pairs from the query by name.
        /// </summary>
        /// <param name="names">Query string parameter names to remove</param>
        /// <returns>The FlurlModel object with the query parameters removed</returns>
        public Flurl RemoveQueryParams(IEnumerable<string> names)
        {
            foreach (var name in names)
                QueryParams.Remove(name);
            return this;
        }

        /// <summary>
        /// Removes the entire query component of the URL.
        /// </summary>
        /// <returns>The FlurlModel object.</returns>
        public Flurl RemoveQuery()
        {
            QueryParams.Clear();
            return this;
        }

        /// <summary>
        /// Set the URL fragment fluently.
        /// </summary>
        /// <param name="fragment">The part of the URL after #</param>
        /// <returns>The FlurlModel object with the new fragment set</returns>
        public Flurl SetFragment(string fragment)
        {
            Fragment = fragment ?? "";
            return this;
        }

        /// <summary>
        /// Removes the URL fragment including the #.
        /// </summary>
        /// <returns>The FlurlModel object with the fragment removed</returns>
        public Flurl RemoveFragment() => SetFragment("");

        /// <summary>
        /// Resets the URL to its root, including the scheme, any user info, host, and port (if specified).
        /// </summary>
        /// <returns>The FlurlModel object trimmed to its root.</returns>
        public Flurl ResetToRoot()
        {
            PathSegments.Clear();
            QueryParams.Clear();
            Fragment = "";
            _leadingSlash = false;
            _trailingSlash = false;
            return this;
        }

        /// <summary>
        /// Resets the URL to its original state as set in the constructor.
        /// </summary>
        public Flurl Reset()
        {
            if (_parsed)
            {
                _scheme = null;
                _userInfo = null;
                _host = null;
                _port = null;
                _pathSegments = null;
                _queryParams = null;
                _fragment = null;
                _leadingSlash = false;
                _trailingSlash = false;
                _parsed = false;
            }
            return this;
        }

        /// <summary>
        /// Creates a copy of this FlurlModel.
        /// </summary>
        public Flurl Clone() => new Flurl(this);
        #endregion

        #region conversion, equality, etc.
        /// <summary>
        /// Converts this FlurlModel object to its string representation.
        /// </summary>
        /// <param name="encodeSpaceAsPlus">Indicates whether to encode spaces with the "+" character instead of "%20"</param>
        /// <returns></returns>
        public string ToString(bool encodeSpaceAsPlus)
        {
            if (!_parsed)
                return _originalString ?? "";

            return string.Concat(
                Root,
                encodeSpaceAsPlus ? Path.Replace("%20", "+") : Path,
                _trailingQmark || QueryParams.Any() ? "?" : "",
                QueryParams.ToString(encodeSpaceAsPlus),
                _trailingHash || Fragment?.Length > 0 ? "#" : "",
                Fragment).Trim();
        }

        /// <summary>
        /// Converts this FlurlModel object to its string representation.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ToString(false);

        /// <summary>
        /// Converts this FlurlModel object to System.Uri
        /// </summary>
        /// <returns>The System.Uri object</returns>
        public Uri ToUri() => new Uri(this, UriKind.RelativeOrAbsolute);

        /// <summary>
        /// Implicit conversion from FlurlModel to String.
        /// </summary>
        /// <param name="url">The FlurlModel object</param>
        /// <returns>The string</returns>
        public static implicit operator string(Flurl url) => url?.ToString();

        /// <summary>
        /// Implicit conversion from String to FlurlModel.
        /// </summary>
        /// <param name="url">The String representation of the URL</param>
        /// <returns>The string</returns>
        public static implicit operator Flurl(string url) => new Flurl(url);

        /// <summary>
        /// Implicit conversion from System.Uri to Flurl.FlurlModel.
        /// </summary>
        /// <returns>The string</returns>
        public static implicit operator Flurl(Uri uri) => new Flurl(uri.ToString());

        /// <summary>
        /// True if obj is an instance of FlurlModel and its string representation is equal to this instance's string representation.
        /// </summary>
        /// <param name="obj">The object to compare to this instance.</param>
        /// <returns></returns>
        public override bool Equals(object obj) => obj is Flurl url && this.ToString().OrdinalEquals(url.ToString());

        /// <summary>
        /// Returns the hashcode for this FlurlModel.
        /// </summary>
        public override int GetHashCode() => this.ToString().GetHashCode();
        #endregion

        #region static utility methods
        /// <summary>
        /// Basically a Path.Combine for URLs. Ensures exactly one '/' separates each segment,
        /// and exactly on '&amp;' separates each query parameter.
        /// URL-encodes illegal characters but not reserved characters.
        /// </summary>
        /// <param name="parts">URL parts to combine.</param>
        public static string Combine(params string[] parts)
        {
            if (parts == null)
                throw new ArgumentNullException(nameof(parts));

            string result = "";
            bool inQuery = false, inFragment = false;

            string CombineEnsureSingleSeparator(string a, string b, char separator)
            {
                if (string.IsNullOrEmpty(a)) return b;
                if (string.IsNullOrEmpty(b)) return a;
                return a.TrimEnd(separator) + separator + b.TrimStart(separator);
            }

            foreach (var part in parts)
            {
                if (string.IsNullOrEmpty(part))
                    continue;

                if (result.OrdinalEndsWith("?") || part.OrdinalStartsWith("?"))
                    result = CombineEnsureSingleSeparator(result, part, '?');
                else if (result.OrdinalEndsWith("#") || part.OrdinalStartsWith("#"))
                    result = CombineEnsureSingleSeparator(result, part, '#');
                else if (inFragment)
                    result += part;
                else if (inQuery)
                    result = CombineEnsureSingleSeparator(result, part, '&');
                else
                    result = CombineEnsureSingleSeparator(result, part, '/');

                if (part.OrdinalContains("#"))
                {
                    inQuery = false;
                    inFragment = true;
                }
                else if (!inFragment && part.OrdinalContains("?"))
                {
                    inQuery = true;
                }
            }
            return EncodeIllegalCharacters(result);
        }

        /// <summary>
        /// Decodes a URL-encoded string.
        /// </summary>
        /// <param name="s">The URL-encoded string.</param>
        /// <param name="interpretPlusAsSpace">If true, any '+' character will be decoded to a space.</param>
        /// <returns></returns>
        public static string Decode(string s, bool interpretPlusAsSpace)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            return Uri.UnescapeDataString(interpretPlusAsSpace ? s.Replace("+", " ") : s);
        }

        private const int MAX_URL_LENGTH = 65519;

        /// <summary>
        /// URL-encodes a string, including reserved characters such as '/' and '?'.
        /// </summary>
        /// <param name="s">The string to encode.</param>
        /// <param name="encodeSpaceAsPlus">If true, spaces will be encoded as + signs. Otherwise, they'll be encoded as %20.</param>
        /// <returns>The encoded URL.</returns>
        public static string Encode(string s, bool encodeSpaceAsPlus = false)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            if (s.Length > MAX_URL_LENGTH)
            {
                // Uri.EscapeDataString is going to throw because the string is "too long", so break it into pieces and concat them
                var parts = new string[(int)Math.Ceiling((double)s.Length / MAX_URL_LENGTH)];
                for (var i = 0; i < parts.Length; i++)
                {
                    var start = i * MAX_URL_LENGTH;
                    var len = Math.Min(MAX_URL_LENGTH, s.Length - start);
                    parts[i] = Uri.EscapeDataString(s.Substring(start, len));
                }
                s = string.Concat(parts);
            }
            else
            {
                s = Uri.EscapeDataString(s);
            }
            return encodeSpaceAsPlus ? s.Replace("%20", "+") : s;
        }

        /// <summary>
        /// URL-encodes characters in a string that are neither reserved nor unreserved. Avoids encoding reserved characters such as '/' and '?'. Avoids encoding '%' if it begins a %-hex-hex sequence (i.e. avoids double-encoding).
        /// </summary>
        /// <param name="s">The string to encode.</param>
        /// <param name="encodeSpaceAsPlus">If true, spaces will be encoded as + signs. Otherwise, they'll be encoded as %20.</param>
        /// <returns>The encoded URL.</returns>
        public static string EncodeIllegalCharacters(string s, bool encodeSpaceAsPlus = false)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            if (encodeSpaceAsPlus)
                s = s.Replace(" ", "+");

            // Uri.EscapeUriString mostly does what we want - encodes illegal characters only - but it has a quirk
            // in that % isn't illegal if it's the start of a %-encoded sequence https://stackoverflow.com/a/47636037/62600

            // no % characters, so avoid the regex overhead
            if (!s.OrdinalContains("%"))
                return Uri.EscapeUriString(s);

            // pick out all %-hex-hex matches and avoid double-encoding
            return Regex.Replace(s, "(.*?)((%[0-9A-Fa-f]{2})|$)", c =>
            {
                var a = c.Groups[1].Value; // group 1 is a sequence with no %-encoding - encode illegal characters
                var b = c.Groups[2].Value; // group 2 is a valid 3-character %-encoded sequence - leave it alone!
                return Uri.EscapeUriString(a) + b;
            });
        }

        /// <summary>
        /// Checks if a string is a well-formed absolute URL.
        /// </summary>
        /// <param name="url">The string to check</param>
        /// <returns>true if the string is a well-formed absolute URL</returns>
        public static bool IsValid(string url) => url != null && Uri.IsWellFormedUriString(url, UriKind.Absolute);
        #endregion
    }
    /// <summary>
    /// Defines common methods for INameValueList and IReadOnlyNameValueList.
    /// </summary>
    public interface INameValueListBase<TValue>
    {
        /// <summary>
        /// Returns the first Value of the given Name if one exists, otherwise null or default value.
        /// </summary>
        TValue FirstOrDefault(string name);

        /// <summary>
        /// Gets the first Value of the given Name, if one exists.
        /// </summary>
        /// <returns>true if any item of the given name is found, otherwise false.</returns>
        bool TryGetFirst(string name, out TValue value);

        /// <summary>
        /// Gets all Values of the given Name.
        /// </summary>
        IEnumerable<TValue> GetAll(string name);

        /// <summary>
        /// True if any items with the given Name exist.
        /// </summary>
        bool Contains(string name);

        /// <summary>
        /// True if any item with the given Name and Value exists.
        /// </summary>
        bool Contains(string name, TValue value);
    }
    /// <summary>
    /// Defines an ordered collection of Name/Value pairs where duplicate names are allowed but aren't typical.
    /// </summary>
    public interface INameValueList<TValue> : IList<(string Name, TValue Value)>, INameValueListBase<TValue>
    {
        /// <summary>
        /// Adds a new Name/Value pair.
        /// </summary>
        void Add(string name, TValue value);

        /// <summary>
        /// Replaces the first occurrence of the given Name with the given Value and removes any others,
        /// or adds a new Name/Value pair if none exist.
        /// </summary>
        void AddOrReplace(string name, TValue value);

        /// <summary>
        /// Removes all items of the given Name.
        /// </summary>
        /// <returns>true if any item of the given name is found, otherwise false.</returns>
        bool Remove(string name);
    }
    /// <summary>
    /// Defines a read-only ordered collection of Name/Value pairs where duplicate names are allowed but aren't typical.
    /// </summary>
    public interface IReadOnlyNameValueList<TValue> : IReadOnlyList<(string Name, TValue Value)>, INameValueListBase<TValue>
    {
    }
    /// <summary>
    /// An ordered collection of Name/Value pairs where duplicate names are allowed but aren't typical.
    /// Useful for things where a dictionary would work great if not for those pesky edge cases (headers, cookies, etc).
    /// </summary>
    public class NameValueList<TValue> : List<(string Name, TValue Value)>, INameValueList<TValue>, IReadOnlyNameValueList<TValue>
    {
        private bool _caseSensitiveNames;

        /// <summary>
        /// Instantiates a new empty NameValueList.
        /// </summary>
        public NameValueList(bool caseSensitiveNames)
        {
            _caseSensitiveNames = caseSensitiveNames;
        }

        /// <summary>
        /// Instantiates a new NameValueList with the Name/Value pairs provided.
        /// </summary>
        public NameValueList(IEnumerable<(string Name, TValue Value)> items, bool caseSensitiveNames)
        {
            _caseSensitiveNames = caseSensitiveNames;
            AddRange(items);
        }

        /// <inheritdoc />
        public void Add(string name, TValue value) => Add((name, value));

        /// <inheritdoc />
        public void AddOrReplace(string name, TValue value)
        {
            var i = 0;
            var replaced = false;
            while (i < this.Count)
            {
                if (!this[i].Name.OrdinalEquals(name, !_caseSensitiveNames))
                    i++;
                else if (replaced)
                    this.RemoveAt(i);
                else
                {
                    this[i] = (name, value);
                    replaced = true;
                    i++;
                }
            }

            if (!replaced)
                this.Add(name, value);
        }

        /// <inheritdoc />
        public bool Remove(string name) => RemoveAll(x => x.Name.OrdinalEquals(name, !_caseSensitiveNames)) > 0;

        /// <inheritdoc />
        public TValue FirstOrDefault(string name) => GetAll(name).FirstOrDefault();

        /// <inheritdoc />
        public bool TryGetFirst(string name, out TValue value)
        {
            foreach (var v in GetAll(name))
            {
                value = v;
                return true;
            }
            value = default;
            return false;
        }

        /// <inheritdoc />
        public IEnumerable<TValue> GetAll(string name) => this
            .Where(x => x.Name.OrdinalEquals(name, !_caseSensitiveNames))
            .Select(x => x.Value);

        /// <inheritdoc />
        public bool Contains(string name) => this.Any(x => x.Name.OrdinalEquals(name, !_caseSensitiveNames));

        /// <inheritdoc />
        public bool Contains(string name, TValue value) => Contains((name, value));
    }
    /// <summary>
    /// Describes how to handle null values in query parameters.
    /// </summary>
    public enum NullValueHandling
    {
        /// <summary>
        /// Set as name without value in query string.
        /// </summary>
        NameOnly,
        /// <summary>
        /// Don't add to query string, remove any existing value.
        /// </summary>
        Remove,
        /// <summary>
        /// Don't add to query string, but leave any existing value unchanged.
        /// </summary>
        Ignore
    }
    /// <summary>
    /// Represents a URL query as a collection of name/value pairs. Insertion order is preserved.
    /// </summary>
    public class QueryParamCollection : IReadOnlyNameValueList<object>
    {
        private readonly NameValueList<QueryParamValue> _values = new NameValueList<QueryParamValue>(true);

        /// <summary>
        /// Returns a new instance of QueryParamCollection
        /// </summary>
        /// <param name="query">Optional query string to parse.</param>
        public QueryParamCollection(string query = null)
        {
            if (query == null)
                return;

            _values.AddRange(
                from kv in query.TrimStart('?').ToKeyValuePairs()
                select (kv.Key, new QueryParamValue(kv.Value, true)));
        }

        /// <summary>
        /// Returns serialized, encoded query string. Insertion order is preserved.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ToString(false);

        /// <summary>
        /// Returns serialized, encoded query string. Insertion order is preserved.
        /// </summary>
        /// <returns></returns>
        public string ToString(bool encodeSpaceAsPlus) => string.Join("&",
            from p in _values
            let name = Flurl.EncodeIllegalCharacters(p.Name, encodeSpaceAsPlus)
            let value = p.Value.Encode(encodeSpaceAsPlus)
            select (value == null) ? name : $"{name}={value}");

        /// <summary>
        /// Appends a query parameter. If value is a collection type (array, IEnumerable, etc.), multiple parameters are added, i.e. x=1&amp;x=2.
        /// To overwrite existing parameters of the same name, use AddOrReplace instead.
        /// </summary>
        /// <param name="name">Name of the parameter.</param>
        /// <param name="value">Value of the parameter. If it's a collection, multiple parameters of the same name are added.</param>
        /// <param name="isEncoded">If true, assume value(s) already URL-encoded.</param>
        /// <param name="nullValueHandling">Describes how to handle null values.</param>
        public void Add(string name, object value, bool isEncoded = false, NullValueHandling nullValueHandling = NullValueHandling.Remove)
        {
            if (value == null && nullValueHandling == NullValueHandling.Remove)
            {
                _values.Remove(name);
                return;
            }

            foreach (var val in SplitCollection(value).ToList())
            {
                if (val == null && nullValueHandling != NullValueHandling.NameOnly)
                    continue;
                _values.Add(name, new QueryParamValue(val, isEncoded));
            }
        }

        /// <summary>
        /// Replaces existing query parameter(s) or appends to the end. If value is a collection type (array, IEnumerable, etc.),
        /// multiple parameters are added, i.e. x=1&amp;x=2. If any of the same name already exist, they are overwritten one by one
        /// (preserving order) and any remaining are appended to the end. If fewer values are specified than already exist,
        /// remaining existing values are removed.
        /// </summary>
        /// <param name="name">Name of the parameter.</param>
        /// <param name="value">Value of the parameter. If it's a collection, multiple parameters of the same name are added/replaced.</param>
        /// <param name="isEncoded">If true, assume value(s) already URL-encoded.</param>
        /// <param name="nullValueHandling">Describes how to handle null values.</param>
        public void AddOrReplace(string name, object value, bool isEncoded = false, NullValueHandling nullValueHandling = NullValueHandling.Remove)
        {
            if (!Contains(name))
                Add(name, value, isEncoded, nullValueHandling);

            // This covers some complex edge cases involving multiple values of the same name.
            // example: x has values at positions 2 and 4 in the query string, then we set x to
            // an array of 4 values. We want to replace the values at positions 2 and 4 with the
            // first 2 values of the new array, then append the remaining 2 values to the end.
            var values = new Queue<object>(SplitCollection(value));

            var old = _values.ToArray();
            _values.Clear();

            foreach (var item in old)
            {
                if (item.Name != name)
                {
                    _values.Add(item);
                    continue;
                }

                if (values.Count == 0)
                    continue; // remove, effectively

                var val = values.Dequeue();
                if (val == null && nullValueHandling == NullValueHandling.Ignore)
                    _values.Add(item);
                else if (val == null && nullValueHandling == NullValueHandling.Remove)
                    continue;
                else
                    Add(name, val, isEncoded, nullValueHandling);
            }

            // add the rest to the end
            while (values.Count > 0)
            {
                Add(name, values.Dequeue(), isEncoded, nullValueHandling);
            }
        }

        private IEnumerable<object> SplitCollection(object value)
        {
            if (value == null)
                yield return null;
            else if (value is string s)
                yield return s;
            else if (value is IEnumerable en)
            {
                foreach (var item in en.Cast<object>().SelectMany(SplitCollection).ToList())
                    yield return item;
            }
            else
                yield return value;
        }

        /// <summary>
        /// Removes all query parameters of the given name.
        /// </summary>
        public void Remove(string name) => _values.Remove(name);

        /// <summary>
        /// Clears all query parameters from this collection.
        /// </summary>
        public void Clear() => _values.Clear();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />>
        public IEnumerator<(string Name, object Value)> GetEnumerator() =>
            _values.Select(qp => (qp.Name, Original: qp.Value.Value)).GetEnumerator();

        /// <inheritdoc />>
        public int Count => _values.Count;

        /// <inheritdoc />>
        public (string Name, object Value) this[int index] => (_values[index].Name, _values[index].Value.Value);

        /// <inheritdoc />>
        public object FirstOrDefault(string name) => _values.FirstOrDefault(name).Value;

        /// <inheritdoc />>
        public bool TryGetFirst(string name, out object value)
        {
            var result = _values.TryGetFirst(name, out var qv);
            value = qv.Value;
            return result;
        }

        /// <inheritdoc />>
        public IEnumerable<object> GetAll(string name) => _values.GetAll(name).Select(qv => qv.Value);

        /// <inheritdoc />>
        public bool Contains(string name) => _values.Contains(name);

        /// <inheritdoc />>
        public bool Contains(string name, object value) => _values.Any(qv => qv.Name == name && qv.Value.Value.Equals(value));
    }
    /// <summary>
    /// Represents a query parameter value with the ability to track whether it was already encoded when created.
    /// </summary>
    internal readonly struct QueryParamValue
    {
        private readonly string _encodedValue;

        public QueryParamValue(object value, bool isEncoded)
        {
            if (isEncoded && value is string s)
            {
                _encodedValue = s;
                Value = Flurl.Decode(s, true);
            }
            else
            {
                Value = value;
                _encodedValue = null;
            }
        }

        public object Value { get; }

        public string Encode(bool encodeSpaceAsPlus) =>
            (Value == null) ? null :
            (_encodedValue != null) ? _encodedValue :
            (Value is string s) ? Flurl.Encode(s, encodeSpaceAsPlus) :
            Flurl.Encode(Value.ToInvariantString(), encodeSpaceAsPlus);
    }
}
