using System.Collections;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Hopper
{
    /// <summary>
    /// Flurl统一扩展内容
    /// </summary>
    public static partial class FlurlExtensions
    {
        #region // CommonExtensions for Objects
        /// <summary>
        /// Returns a key-value-pairs representation of the object.
        /// For strings, URL query string format assumed and pairs are parsed from that.
        /// For objects that already implement IEnumerable&lt;KeyValuePair&gt;, the object itself is simply returned.
        /// For all other objects, all publicly readable properties are extracted and returned as pairs.
        /// </summary>
        /// <param name="obj">The object to parse into key-value pairs</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <see langword="null" />.</exception>
        public static IEnumerable<(string Key, object Value)> ToKeyValuePairs(this object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            return
                obj is string s ? StringToKV(s) :
                obj is IEnumerable e ? CollectionToKV(e) :
                ObjectToKV(obj);
        }

        /// <summary>
        /// Returns a string that represents the current object, using CultureInfo.InvariantCulture where possible.
        /// Dates are represented in IS0 8601.
        /// </summary>
        public static string ToInvariantString(this object obj)
        {
            // inspired by: http://stackoverflow.com/a/19570016/62600
            return
                obj == null ? null :
                obj is DateTime dt ? dt.ToString("o", CultureInfo.InvariantCulture) :
                obj is DateTimeOffset dto ? dto.ToString("o", CultureInfo.InvariantCulture) :
                obj is IConvertible c ? c.ToString(CultureInfo.InvariantCulture) :
                obj is IFormattable f ? f.ToString(null, CultureInfo.InvariantCulture) :
                obj.ToString();
        }

        internal static bool OrdinalEquals(this string s, string value, bool ignoreCase = false) =>
            s != null && s.Equals(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

        internal static bool OrdinalContains(this string s, string value, bool ignoreCase = false) =>
            s != null && s.IndexOf(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) >= 0;

        internal static bool OrdinalStartsWith(this string s, string value, bool ignoreCase = false) =>
            s != null && s.StartsWith(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

        internal static bool OrdinalEndsWith(this string s, string value, bool ignoreCase = false) =>
            s != null && s.EndsWith(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

        /// <summary>
        /// Splits at the first occurrence of the given separator.
        /// </summary>
        /// <param name="s">The string to split.</param>
        /// <param name="separator">The separator to split on.</param>
        /// <returns>Array of at most 2 strings. (1 if separator is not found.)</returns>
        public static string[] SplitOnFirstOccurence(this string s, string separator)
        {
            // Needed because full PCL profile doesn't support Split(char[], int) (#119)
            if (string.IsNullOrEmpty(s))
                return new[] { s };

            var i = s.IndexOf(separator);
            return (i == -1) ?
                new[] { s } :
                new[] { s.Substring(0, i), s.Substring(i + separator.Length) };
        }

        private static IEnumerable<(string Key, object Value)> StringToKV(string s)
        {
            if (string.IsNullOrEmpty(s))
                return Enumerable.Empty<(string, object)>();

            return
                from p in s.Split('&')
                let pair = p.SplitOnFirstOccurence("=")
                let name = pair[0]
                let value = (pair.Length == 1) ? null : pair[1]
                select (name, (object)value);
        }

        private static IEnumerable<(string Name, object Value)> ObjectToKV(object obj) =>
            from prop in obj.GetType().GetProperties()
            let getter = prop.GetGetMethod(false)
            where getter != null
            let val = getter.Invoke(obj, null)
            select (prop.Name, GetDeclaredTypeValue(val, prop.PropertyType));

        internal static object GetDeclaredTypeValue(object value, Type declaredType)
        {
            if (value == null || value.GetType() == declaredType)
                return value;

            // without this we had https://github.com/tmenier/Flurl/issues/669
            // related: https://stackoverflow.com/q/3531318/62600
            declaredType = Nullable.GetUnderlyingType(declaredType) ?? declaredType;

            // added to deal with https://github.com/tmenier/Flurl/issues/632
            // thx @j2jensen!
            if (value is IEnumerable col
                && declaredType.IsGenericType
                && declaredType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                && !col.GetType().GetInterfaces().Contains(declaredType)
                && declaredType.IsInstanceOfType(col))
            {
                var elementType = declaredType.GetGenericArguments()[0];
                return col.Cast<object>().Select(element => Convert.ChangeType(element, elementType));
            }

            return value;
        }

        private static IEnumerable<(string Key, object Value)> CollectionToKV(IEnumerable col)
        {
            bool TryGetProp(object obj, string name, out object value)
            {
                var prop = obj.GetType().GetProperty(name);
                var field = obj.GetType().GetField(name);

                if (prop != null)
                {
                    value = prop.GetValue(obj, null);
                    return true;
                }
                if (field != null)
                {
                    value = field.GetValue(obj);
                    return true;
                }
                value = null;
                return false;
            }

            bool IsTuple2(object item, out object name, out object val)
            {
                name = null;
                val = null;
                return
                    item.GetType().Name.OrdinalContains("Tuple") &&
                    TryGetProp(item, "Item1", out name) &&
                    TryGetProp(item, "Item2", out val) &&
                    !TryGetProp(item, "Item3", out _);
            }

            bool LooksLikeKV(object item, out object name, out object val)
            {
                name = null;
                val = null;
                return
                    (TryGetProp(item, "Key", out name) || TryGetProp(item, "key", out name) || TryGetProp(item, "Name", out name) || TryGetProp(item, "name", out name)) &&
                    (TryGetProp(item, "Value", out val) || TryGetProp(item, "value", out val));
            }

            foreach (var item in col)
            {
                if (item == null)
                    continue;
                if (!IsTuple2(item, out var name, out var val) && !LooksLikeKV(item, out name, out val))
                    yield return (item.ToInvariantString(), null);
                else if (name != null)
                    yield return (name.ToInvariantString(), val);
            }
        }

        /// <summary>
        /// Merges the key/value pairs from d2 into d1, without overwriting those already set in d1.
        /// </summary>
        public static void Merge<TKey, TValue>(this IDictionary<TKey, TValue> d1, IDictionary<TKey, TValue> d2)
        {
            foreach (var kv in d2.Where(x => !d1.ContainsKey(x.Key)).ToList())
            {
                d1[kv.Key] = kv.Value;
            }
        }

        /// <summary>
        /// Strips any single quotes or double quotes from the beginning and end of a string.
        /// </summary>
        public static string StripQuotes(this string s) => Regex.Replace(s, "^\\s*['\"]+|['\"]+\\s*$", "");

        /// <summary>
        /// True if the given string is a valid IPv4 address.
        /// </summary>
        public static bool IsIP(this string s)
        {
            // based on https://stackoverflow.com/a/29942932/62600
            if (string.IsNullOrEmpty(s))
                return false;

            var parts = s.Split('.');
            return parts.Length == 4 && parts.All(x => byte.TryParse(x, out _));
        }
        #endregion CommonExtensions
        #region // GeneratedExtensions Fluent URL-building extension methods on String and Uri.
        /// <summary>
        /// Creates a new Url object from the string and appends a segment to the URL path, ensuring there is one and only one '/' character as a separator.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="segment">The segment to append</param>
        /// <param name="fullyEncode">If true, URL-encodes reserved characters such as '/', '+', and '%'. Otherwise, only encodes strictly illegal characters (including '%' but only when not followed by 2 hex characters).</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl AppendPathSegment(this string url, object segment, bool fullyEncode = false)
        {
            return new Flurl(url).AppendPathSegment(segment, fullyEncode);
        }

        /// <summary>
        /// Appends multiple segments to the URL path, ensuring there is one and only one '/' character as a separator.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="segments">The segments to append</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl AppendPathSegments(this string url, params object[] segments)
        {
            return new Flurl(url).AppendPathSegments(segments);
        }

        /// <summary>
        /// Appends multiple segments to the URL path, ensuring there is one and only one '/' character as a separator.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="segments">The segments to append</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl AppendPathSegments(this string url, IEnumerable<object> segments)
        {
            return new Flurl(url).AppendPathSegments(segments);
        }

        /// <summary>
        /// Removes the last path segment from the URL.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl RemovePathSegment(this string url)
        {
            return new Flurl(url).RemovePathSegment();
        }

        /// <summary>
        /// Removes the entire path component of the URL.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl RemovePath(this string url)
        {
            return new Flurl(url).RemovePath();
        }

        /// <summary>
        /// Creates a new Url object from the string and adds a parameter to the query, overwriting the value if name exists.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="name">Name of query parameter</param>
        /// <param name="value">Value of query parameter</param>
        /// <param name="nullValueHandling">Indicates how to handle null values. Defaults to Remove (any existing)</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl SetQueryParam(this string url, string name, object value, NullValueHandling nullValueHandling = NullValueHandling.Remove)
        {
            return new Flurl(url).SetQueryParam(name, value, nullValueHandling);
        }

        /// <summary>
        /// Creates a new Url object from the string and adds a parameter to the query, overwriting the value if name exists.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="name">Name of query parameter</param>
        /// <param name="value">Value of query parameter</param>
        /// <param name="isEncoded">Set to true to indicate the value is already URL-encoded. Defaults to false.</param>
        /// <param name="nullValueHandling">Indicates how to handle null values. Defaults to Remove (any existing).</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl SetQueryParam(this string url, string name, string value, bool isEncoded = false, NullValueHandling nullValueHandling = NullValueHandling.Remove)
        {
            return new Flurl(url).SetQueryParam(name, value, isEncoded, nullValueHandling);
        }

        /// <summary>
        /// Creates a new Url object from the string and adds a parameter without a value to the query, removing any existing value.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="name">Name of query parameter</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl SetQueryParam(this string url, string name)
        {
            return new Flurl(url).SetQueryParam(name);
        }

        /// <summary>
        /// Creates a new Url object from the string, parses values object into name/value pairs, and adds them to the query, overwriting any that already exist.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="values">Typically an anonymous object, ie: new { x = 1, y = 2 }</param>
        /// <param name="nullValueHandling">Indicates how to handle null values. Defaults to Remove (any existing)</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl SetQueryParams(this string url, object values, NullValueHandling nullValueHandling = NullValueHandling.Remove)
        {
            return new Flurl(url).SetQueryParams(values, nullValueHandling);
        }

        /// <summary>
        /// Creates a new Url object from the string and adds multiple parameters without values to the query.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="names">Names of query parameters.</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl SetQueryParams(this string url, IEnumerable<string> names)
        {
            return new Flurl(url).SetQueryParams(names);
        }

        /// <summary>
        /// Creates a new Url object from the string and adds multiple parameters without values to the query.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="names">Names of query parameters</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl SetQueryParams(this string url, params string[] names)
        {
            return new Flurl(url).SetQueryParams(names);
        }

        /// <summary>
        /// Creates a new Url object from the string and removes a name/value pair from the query by name.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="name">Query string parameter name to remove</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl RemoveQueryParam(this string url, string name)
        {
            return new Flurl(url).RemoveQueryParam(name);
        }

        /// <summary>
        /// Creates a new Url object from the string and removes multiple name/value pairs from the query by name.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="names">Query string parameter names to remove</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl RemoveQueryParams(this string url, params string[] names)
        {
            return new Flurl(url).RemoveQueryParams(names);
        }

        /// <summary>
        /// Creates a new Url object from the string and removes multiple name/value pairs from the query by name.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="names">Query string parameter names to remove</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl RemoveQueryParams(this string url, IEnumerable<string> names)
        {
            return new Flurl(url).RemoveQueryParams(names);
        }

        /// <summary>
        /// Removes the entire query component of the URL.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl RemoveQuery(this string url)
        {
            return new Flurl(url).RemoveQuery();
        }

        /// <summary>
        /// Set the URL fragment fluently.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <param name="fragment">The part of the URL after #</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl SetFragment(this string url, string fragment)
        {
            return new Flurl(url).SetFragment(fragment);
        }

        /// <summary>
        /// Removes the URL fragment including the #.
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl RemoveFragment(this string url)
        {
            return new Flurl(url).RemoveFragment();
        }

        /// <summary>
        /// Trims the URL to its root, including the scheme, any user info, host, and port (if specified).
        /// </summary>
        /// <param name="url">This URL.</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl ResetToRoot(this string url)
        {
            return new Flurl(url).ResetToRoot();
        }

        /// <summary>
        /// Creates a new Url object from the string and appends a segment to the URL path, ensuring there is one and only one '/' character as a separator.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="segment">The segment to append</param>
        /// <param name="fullyEncode">If true, URL-encodes reserved characters such as '/', '+', and '%'. Otherwise, only encodes strictly illegal characters (including '%' but only when not followed by 2 hex characters).</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl AppendPathSegment(this Uri uri, object segment, bool fullyEncode = false)
        {
            return new Flurl(uri).AppendPathSegment(segment, fullyEncode);
        }

        /// <summary>
        /// Appends multiple segments to the URL path, ensuring there is one and only one '/' character as a separator.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="segments">The segments to append</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl AppendPathSegments(this Uri uri, params object[] segments)
        {
            return new Flurl(uri).AppendPathSegments(segments);
        }

        /// <summary>
        /// Appends multiple segments to the URL path, ensuring there is one and only one '/' character as a separator.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="segments">The segments to append</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl AppendPathSegments(this Uri uri, IEnumerable<object> segments)
        {
            return new Flurl(uri).AppendPathSegments(segments);
        }

        /// <summary>
        /// Removes the last path segment from the URL.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl RemovePathSegment(this Uri uri)
        {
            return new Flurl(uri).RemovePathSegment();
        }

        /// <summary>
        /// Removes the entire path component of the URL.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl RemovePath(this Uri uri)
        {
            return new Flurl(uri).RemovePath();
        }

        /// <summary>
        /// Creates a new Url object from the string and adds a parameter to the query, overwriting the value if name exists.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="name">Name of query parameter</param>
        /// <param name="value">Value of query parameter</param>
        /// <param name="nullValueHandling">Indicates how to handle null values. Defaults to Remove (any existing)</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl SetQueryParam(this Uri uri, string name, object value, NullValueHandling nullValueHandling = NullValueHandling.Remove)
        {
            return new Flurl(uri).SetQueryParam(name, value, nullValueHandling);
        }

        /// <summary>
        /// Creates a new Url object from the string and adds a parameter to the query, overwriting the value if name exists.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="name">Name of query parameter</param>
        /// <param name="value">Value of query parameter</param>
        /// <param name="isEncoded">Set to true to indicate the value is already URL-encoded. Defaults to false.</param>
        /// <param name="nullValueHandling">Indicates how to handle null values. Defaults to Remove (any existing).</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl SetQueryParam(this Uri uri, string name, string value, bool isEncoded = false, NullValueHandling nullValueHandling = NullValueHandling.Remove)
        {
            return new Flurl(uri).SetQueryParam(name, value, isEncoded, nullValueHandling);
        }

        /// <summary>
        /// Creates a new Url object from the string and adds a parameter without a value to the query, removing any existing value.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="name">Name of query parameter</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl SetQueryParam(this Uri uri, string name)
        {
            return new Flurl(uri).SetQueryParam(name);
        }

        /// <summary>
        /// Creates a new Url object from the string, parses values object into name/value pairs, and adds them to the query, overwriting any that already exist.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="values">Typically an anonymous object, ie: new { x = 1, y = 2 }</param>
        /// <param name="nullValueHandling">Indicates how to handle null values. Defaults to Remove (any existing)</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl SetQueryParams(this Uri uri, object values, NullValueHandling nullValueHandling = NullValueHandling.Remove)
        {
            return new Flurl(uri).SetQueryParams(values, nullValueHandling);
        }

        /// <summary>
        /// Creates a new Url object from the string and adds multiple parameters without values to the query.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="names">Names of query parameters.</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl SetQueryParams(this Uri uri, IEnumerable<string> names)
        {
            return new Flurl(uri).SetQueryParams(names);
        }

        /// <summary>
        /// Creates a new Url object from the string and adds multiple parameters without values to the query.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="names">Names of query parameters</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl SetQueryParams(this Uri uri, params string[] names)
        {
            return new Flurl(uri).SetQueryParams(names);
        }

        /// <summary>
        /// Creates a new Url object from the string and removes a name/value pair from the query by name.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="name">Query string parameter name to remove</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl RemoveQueryParam(this Uri uri, string name)
        {
            return new Flurl(uri).RemoveQueryParam(name);
        }

        /// <summary>
        /// Creates a new Url object from the string and removes multiple name/value pairs from the query by name.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="names">Query string parameter names to remove</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl RemoveQueryParams(this Uri uri, params string[] names)
        {
            return new Flurl(uri).RemoveQueryParams(names);
        }

        /// <summary>
        /// Creates a new Url object from the string and removes multiple name/value pairs from the query by name.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="names">Query string parameter names to remove</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl RemoveQueryParams(this Uri uri, IEnumerable<string> names)
        {
            return new Flurl(uri).RemoveQueryParams(names);
        }

        /// <summary>
        /// Removes the entire query component of the URL.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl RemoveQuery(this Uri uri)
        {
            return new Flurl(uri).RemoveQuery();
        }

        /// <summary>
        /// Set the URL fragment fluently.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <param name="fragment">The part of the URL after #</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl SetFragment(this Uri uri, string fragment)
        {
            return new Flurl(uri).SetFragment(fragment);
        }

        /// <summary>
        /// Removes the URL fragment including the #.
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl RemoveFragment(this Uri uri)
        {
            return new Flurl(uri).RemoveFragment();
        }

        /// <summary>
        /// Trims the URL to its root, including the scheme, any user info, host, and port (if specified).
        /// </summary>
        /// <param name="uri">This System.Uri.</param>
        /// <returns>A new Flurl.Url object.</returns>
        public static Flurl ResetToRoot(this Uri uri)
        {
            return new Flurl(uri).ResetToRoot();
        }
        #endregion GeneratedExtensions
        #region // GeneratedExtensions Fluent extension methods on String, FlurlModel, Uri, and IFlurlRequest.
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
        public static Task<IFlurlResponse> SendAsync(this Flurl url, HttpMethod verb, HttpContent content = null, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
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
        public static Task<IFlurlResponse> SendJsonAsync(this Flurl url, HttpMethod verb, object body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
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
        public static Task<IFlurlResponse> SendStringAsync(this Flurl url, HttpMethod verb, string body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
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
        public static Task<IFlurlResponse> SendUrlEncodedAsync(this Flurl url, HttpMethod verb, object body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
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
        public static Task<IFlurlResponse> GetAsync(this Flurl url, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
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
        public static Task<T> GetJsonAsync<T>(this Flurl url, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
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
        public static Task<string> GetStringAsync(this Flurl url, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
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
        public static Task<Stream> GetStreamAsync(this Flurl url, HttpCompletionOption completionOption = HttpCompletionOption.ResponseHeadersRead, CancellationToken cancellationToken = default)
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
        public static Task<byte[]> GetBytesAsync(this Flurl url, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
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
        public static Task<IFlurlResponse> PostAsync(this Flurl url, HttpContent content = null, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
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
        public static Task<IFlurlResponse> PostJsonAsync(this Flurl url, object body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
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
        public static Task<IFlurlResponse> PostStringAsync(this Flurl url, string body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
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
        public static Task<IFlurlResponse> PostUrlEncodedAsync(this Flurl url, object body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
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
        public static Task<IFlurlResponse> HeadAsync(this Flurl url, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
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
        public static Task<IFlurlResponse> PutAsync(this Flurl url, HttpContent content = null, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
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
        public static Task<IFlurlResponse> PutJsonAsync(this Flurl url, object body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
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
        public static Task<IFlurlResponse> PutStringAsync(this Flurl url, string body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
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
        public static Task<IFlurlResponse> DeleteAsync(this Flurl url, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
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
        public static Task<IFlurlResponse> PatchAsync(this Flurl url, HttpContent content = null, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
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
        public static Task<IFlurlResponse> PatchJsonAsync(this Flurl url, object body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
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
        public static Task<IFlurlResponse> PatchStringAsync(this Flurl url, string body, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
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
        public static Task<IFlurlResponse> OptionsAsync(this Flurl url, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
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
        public static Task<string> DownloadFileAsync(this Flurl url, string localFolderPath, string localFileName = null, int bufferSize = 4096, HttpCompletionOption completionOption = HttpCompletionOption.ResponseHeadersRead, CancellationToken cancellationToken = default)
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
        public static Task<IFlurlResponse> PostMultipartAsync(this Flurl url, Action<CapturedMultipartContent> buildContent, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default)
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
        public static IFlurlRequest WithHeader(this Flurl url, string name, object value)
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
        public static IFlurlRequest WithHeaders(this Flurl url, object headers, bool replaceUnderscoreWithHyphen = true)
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
        public static IFlurlRequest WithBasicAuth(this Flurl url, string username, string password)
        {
            return new FlurlRequest(url).WithBasicAuth(username, password);
        }

        /// <summary>
        /// Creates a new FlurlRequest and sets the Authorization header with a bearer token according to OAuth 2.0 specification.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="token">The acquired oAuth bearer token.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithOAuthBearerToken(this Flurl url, string token)
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
        public static IFlurlRequest WithCookie(this Flurl url, string name, object value)
        {
            return new FlurlRequest(url).WithCookie(name, value);
        }

        /// <summary>
        /// Creates a new FlurlRequest and adds name-value pairs to its Cookie header based on property names/values of the provided object, or keys/values if object is a dictionary. To automatically maintain a cookie "session", consider using a CookieJar or CookieSession instead.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="values">Names/values of HTTP cookies to set. Typically an anonymous object or IDictionary.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithCookies(this Flurl url, object values)
        {
            return new FlurlRequest(url).WithCookies(values);
        }

        /// <summary>
        /// Creates a new FlurlRequest and sets the CookieJar associated with this request, which will be updated with any Set-Cookie headers present in the response and is suitable for reuse in subsequent requests.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="cookieJar">The CookieJar.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithCookies(this Flurl url, CookieJar cookieJar)
        {
            return new FlurlRequest(url).WithCookies(cookieJar);
        }

        /// <summary>
        /// Creates a new FlurlRequest and associates it with a new CookieJar, which will be updated with any Set-Cookie headers present in the response and is suitable for reuse in subsequent requests.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="cookieJar">The created CookieJar, which can be reused in subsequent requests.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithCookies(this Flurl url, out CookieJar cookieJar)
        {
            return new FlurlRequest(url).WithCookies(out cookieJar);
        }

        /// <summary>
        /// Creates a new FlurlRequest and allows changing its Settings inline.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="action">A delegate defining the Settings changes.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest ConfigureRequest(this Flurl url, Action<FlurlHttpSettings> action)
        {
            return new FlurlRequest(url).ConfigureRequest(action);
        }

        /// <summary>
        /// Creates a new FlurlRequest and sets the request timeout.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="timespan">Time to wait before the request times out.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithTimeout(this Flurl url, TimeSpan timespan)
        {
            return new FlurlRequest(url).WithTimeout(timespan);
        }

        /// <summary>
        /// Creates a new FlurlRequest and sets the request timeout.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="seconds">Seconds to wait before the request times out.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithTimeout(this Flurl url, int seconds)
        {
            return new FlurlRequest(url).WithTimeout(seconds);
        }

        /// <summary>
        /// Creates a new FlurlRequest and adds a pattern representing an HTTP status code or range of codes which (in addition to 2xx) will NOT result in a FlurlHttpException being thrown.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="pattern">Examples: "3xx", "100,300,600", "100-299,6xx"</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest AllowHttpStatus(this Flurl url, string pattern)
        {
            return new FlurlRequest(url).AllowHttpStatus(pattern);
        }

        /// <summary>
        /// Creates a new FlurlRequest and adds an HttpStatusCode which (in addition to 2xx) will NOT result in a FlurlHttpException being thrown.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="statusCodes">The HttpStatusCode(s) to allow.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest AllowHttpStatus(this Flurl url, params HttpStatusCode[] statusCodes)
        {
            return new FlurlRequest(url).AllowHttpStatus(statusCodes);
        }

        /// <summary>
        /// Creates a new FlurlRequest and configures it to allow any returned HTTP status without throwing a FlurlHttpException.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest AllowAnyHttpStatus(this Flurl url)
        {
            return new FlurlRequest(url).AllowAnyHttpStatus();
        }

        /// <summary>
        /// Creates a new FlurlRequest and configures whether redirects are automatically followed.
        /// </summary>
        /// <param name="url">This Flurl.FlurlModel.</param>
        /// <param name="enabled">true if Flurl should automatically send a new request to the redirect URL, false if it should not.</param>
        /// <returns>A new IFlurlRequest.</returns>
        public static IFlurlRequest WithAutoRedirect(this Flurl url, bool enabled)
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
        #endregion GeneratedExtensions
        #region // UrlBuilderExtensions URL builder extension methods on FlurlRequest
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
        #endregion UrlBuilderExtensions
        #region // Util 内部调用 Utility methods used by both HttpTestSetup and HttpTestAssertion
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
        #endregion Util 内部调用
        #region // ResponseExtensions ReceiveXXX ext methods off Task<IFlurlResponse> that allow chaining off methods like SendAsync without the need for nested awaits.
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
        #endregion
        #region // SettingsExtensions Fluent extension methods for tweaking FlurlHttpSettings
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
        #endregion SettingsExtensions
        #region // MultipartExtensions Fluent extension methods for sending multipart/form-data requests.
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
        #endregion
        #region // CookieExtensions Fluent extension methods for working with HTTP cookies.
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
            return request.WithHeader("Cookie", FlurlExtensions.ToRequestHeader(cookies));
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
            return request.WithHeader("Cookie", FlurlExtensions.ToRequestHeader(cookies));
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
        #endregion CookieExtensions
        #region // DownloadExtensions Fluent extension methods for downloading a file.
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
                using (var fileStream = await FlurlExtensions.OpenWriteAsync(localFolderPath, localFileName, bufferSize).ConfigureAwait(false))
                {
                    await httpStream.CopyToAsync(fileStream, bufferSize, cancellationToken).ConfigureAwait(false);
                }
            }

            return FlurlExtensions.CombinePath(localFolderPath, localFileName);
        }

        private static string GetFileNameFromHeaders(HttpResponseMessage resp)
        {
            var header = resp.Content?.Headers.ContentDisposition;
            if (header == null) return null;
            // prefer filename* per https://tools.ietf.org/html/rfc6266#section-4.3
            var val = (header.FileNameStar ?? header.FileName)?.StripQuotes();
            if (val == null) return null;
            return FlurlExtensions.MakeValidName(val);
        }

        private static string GetFileNameFromPath(IFlurlRequest req)
        {
            return FlurlExtensions.MakeValidName(Flurl.Decode(req.Url.Path.Split('/').Last(), false));
        }
        #endregion DownloadExtensions
        #region // HeaderExtensions Fluent extension methods for working with HTTP request headers.
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
        #endregion HeaderExtensions
        #region // HttpMessageExtensions Extension methods off HttpRequestMessage and HttpResponseMessage.
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
        #endregion HttpMessageExtensions
        #region // FlurlClientFactoryExtensions Extension methods on IFlurlClientFactory
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
        #endregion FlurlClientFactoryExtensions
        #region // FileUtil
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
        #endregion FileUtil
        #region // CookieCutter Utility and extension methods for parsing and validating cookies.
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
            if (!Flurl.IsValid(cookie.OriginUrl))
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
                var fakeUrl = new Flurl("https://" + host);
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
        public static bool ShouldSendTo(this FlurlCookie cookie, Flurl requestUrl, out string reason)
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

        private static bool IsDomainMatch(this FlurlCookie cookie, Flurl requestUrl, out string reason)
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

        private static bool IsPathMatch(this FlurlCookie cookie, Flurl requestUrl, out string reason)
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
        #endregion CookieCutter
    }
}
