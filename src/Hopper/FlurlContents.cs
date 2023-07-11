using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

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
            fileName = fileName ?? FlurlExtensions.GetFileName(path);
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
            using (var source = await FlurlExtensions.OpenReadAsync(Path, _bufferSize).ConfigureAwait(false))
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
}
