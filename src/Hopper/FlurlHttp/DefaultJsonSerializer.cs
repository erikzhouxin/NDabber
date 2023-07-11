using Newtonsoft.Json;
using System.Data.Cobber;
using System.Data.Extter;
using System.IO;

namespace Flurl.Http.Configuration
{
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
		public DefaultJsonSerializer(JsonSerializerSettings options = null) {
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
		public T Deserialize<T>(Stream stream) {
			if (stream.Length == 0) { return default; }
			using var jsonW = new StreamReader(stream);
            return JsonConvert.DeserializeObject<T>(jsonW.ReadToEnd(), _options);
		}
	}
}