﻿using System;
#if !CORE
using System.Runtime.Serialization;
#endif

namespace System.Data.Dubber {

	/// <summary>
	/// Exception is thrown when TLS/SSL encryption could not be negotiated by the FTP server.
	/// </summary>
#if !CORE
	[Serializable]
#endif
	public class FtpInvalidCertificateException : FtpException {
		/// <summary>
		/// Default constructor
		/// </summary>
		public FtpInvalidCertificateException()
			: base("FTPS security could not be established on the server. The certificate was not accepted.") {
			
		}

		/// <summary>
		/// Custom error message
		/// </summary>
		/// <param name="message">Error message</param>
		public FtpInvalidCertificateException(string message)
			: base(message) {
		}

#if !CORE
		/// <summary>
		/// Must be implemented so every Serializer can Deserialize the Exception
		/// </summary>
		protected FtpInvalidCertificateException(SerializationInfo info, StreamingContext context) : base(info, context) {
		}

#endif
	}
}