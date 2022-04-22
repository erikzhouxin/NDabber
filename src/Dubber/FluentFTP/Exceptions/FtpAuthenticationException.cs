﻿using System;
#if !CORE
using System.Runtime.Serialization;
#endif

namespace System.Data.Dubber {

	/// <summary>
	/// Exception triggered on FTP authentication failures
	/// </summary>
#if !CORE
	[Serializable]
#endif
	public class FtpAuthenticationException : FtpCommandException {
		/// <summary>
		/// Initializes a new instance of a FtpAuthenticationException
		/// </summary>
		/// <param name="code">Status code</param>
		/// <param name="message">Associated message</param>
		public FtpAuthenticationException(string code, string message) : base(code, message) {
		}

		/// <summary>
		/// Initializes a new instance of a FtpAuthenticationException
		/// </summary>
		/// <param name="reply">The FtpReply to build the exception from</param>
		public FtpAuthenticationException(FtpReply reply) : base(reply) {
		}

#if !CORE
		/// <summary>
		/// Must be implemented so every Serializer can Deserialize the Exception
		/// </summary>
		protected FtpAuthenticationException(SerializationInfo info, StreamingContext context) : base(info, context) {
		}

#endif
	}

}
