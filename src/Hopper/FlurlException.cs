using System.Threading.Tasks;

namespace System.Data.Hopper
{
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
}
