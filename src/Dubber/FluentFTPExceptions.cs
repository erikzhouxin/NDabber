using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;

namespace System.Data.Dubber
{
    /// <summary>
    /// Exception triggered on FTP authentication failures
    /// </summary>
#if !CORE
    [Serializable]
#endif
    public class FtpAuthenticationException : FtpCommandException
    {
        /// <summary>
        /// Initializes a new instance of a FtpAuthenticationException
        /// </summary>
        /// <param name="code">Status code</param>
        /// <param name="message">Associated message</param>
        public FtpAuthenticationException(string code, string message) : base(code, message)
        {
        }

        /// <summary>
        /// Initializes a new instance of a FtpAuthenticationException
        /// </summary>
        /// <param name="reply">The FtpReply to build the exception from</param>
        public FtpAuthenticationException(FtpReply reply) : base(reply)
        {
        }

#if !CORE
        /// <summary>
        /// Must be implemented so every Serializer can Deserialize the Exception
        /// </summary>
        protected FtpAuthenticationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

#endif
    }
    /// <summary>
    /// Exception triggered on FTP command failures
    /// </summary>
#if !CORE
    [Serializable]
#endif
    public class FtpCommandException : FtpException
    {
        private string _code = null;

        /// <summary>
        /// Gets the completion code associated with the response
        /// </summary>
        public string CompletionCode
        {
            get => _code;
            private set => _code = value;
        }

        /// <summary>
        /// The type of response received from the last command executed
        /// </summary>
        public FtpResponseType ResponseType
        {
            get
            {
                if (_code != null)
                {
                    // we only care about error types, if an exception
                    // is being thrown for a successful response there
                    // is a problem.
                    switch (_code[0])
                    {
                        case '4':
                            return FtpResponseType.TransientNegativeCompletion;

                        case '5':
                            return FtpResponseType.PermanentNegativeCompletion;
                    }
                }

                return FtpResponseType.None;
            }
        }

        /// <summary>
        /// Initializes a new instance of a FtpResponseException
        /// </summary>
        /// <param name="code">Status code</param>
        /// <param name="message">Associated message</param>
        public FtpCommandException(string code, string message)
            : base(message)
        {
            CompletionCode = code;
        }

        /// <summary>
        /// Initializes a new instance of a FtpResponseException
        /// </summary>
        /// <param name="reply">The FtpReply to build the exception from</param>
        public FtpCommandException(FtpReply reply)
            : this(reply.Code, reply.ErrorMessage)
        {
        }

#if !CORE
        /// <summary>
        /// Must be implemented so every Serializer can Deserialize the Exception
        /// </summary>
        protected FtpCommandException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

#endif
    }
    /// <summary>
    /// FTP related error
    /// </summary>
#if !CORE
    [Serializable]
#endif
    public class FtpException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FtpException"/> class.
        /// </summary>
        /// <param name="message">The error message</param>
        public FtpException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpException"/> class with an inner exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public FtpException(string message, Exception innerException) : base(message, innerException)
        {
        }

#if !CORE
        /// <summary>
        /// Must be implemented so every Serializer can Deserialize the Exception
        /// </summary>
        protected FtpException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

#endif
    }
    /// <summary>
    /// Exception is thrown when the required hash algorithm is unsupported by the server.
    /// </summary>
#if !CORE
    [Serializable]
#endif
    public class FtpHashUnsupportedException : FtpException
    {

        private FtpHashAlgorithm _algo = FtpHashAlgorithm.NONE;

        /// <summary>
        /// Gets the unsupported hash algorithm
        /// </summary>
        public FtpHashAlgorithm Algorithm
        {
            get => _algo;
            private set => _algo = value;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public FtpHashUnsupportedException()
            : base("Your FTP server does not support the HASH command or any of the algorithm-specific commands. Use a better FTP server software or install a hashing/checksum module onto your server.")
        {

        }

        /// <summary>
        /// Algorithm-specific constructor
        /// </summary>
        public FtpHashUnsupportedException(FtpHashAlgorithm algo, string specialCommands)
            : base("Hash algorithm " + algo.PrintToString() + " is unsupported by your server using the HASH command or the " +
                  specialCommands + " command(s). " +
                  "Use another algorithm or use FtpHashAlgorithm.NONE to select the first available algorithm.")
        {

            Algorithm = algo;
        }

#if !CORE
        /// <summary>
        /// Must be implemented so every Serializer can Deserialize the Exception
        /// </summary>
        protected FtpHashUnsupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

#endif
    }
    /// <summary>
    /// Exception is thrown when TLS/SSL encryption could not be negotiated by the FTP server.
    /// </summary>
#if !CORE
    [Serializable]
#endif
    public class FtpInvalidCertificateException : FtpException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public FtpInvalidCertificateException()
            : base("FTPS security could not be established on the server. The certificate was not accepted.")
        {

        }

        /// <summary>
        /// Custom error message
        /// </summary>
        /// <param name="message">Error message</param>
        public FtpInvalidCertificateException(string message)
            : base(message)
        {
        }

#if !CORE
        /// <summary>
        /// Must be implemented so every Serializer can Deserialize the Exception
        /// </summary>
        protected FtpInvalidCertificateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

#endif
    }
    /// <summary>
    /// Exception thrown by FtpListParser when parsing of FTP directory listing fails.
    /// </summary>
#if !CORE
    [Serializable]
#endif
    public class FtpListParseException : FtpException
    {
        /// <summary>
        /// Creates a new FtpListParseException.
        /// </summary>
        public FtpListParseException()
            : base("Cannot parse file listing!")
        {
        }

#if !CORE
        /// <summary>
        /// Must be implemented so every Serializer can Deserialize the Exception
        /// </summary>
        protected FtpListParseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

#endif
    }
    /// <summary>
    /// Exception is thrown by FtpSocketStream when there is no FTP server socket to connect to.
    /// </summary>
#if !CORE
    [Serializable]
#endif
    public class FtpMissingSocketException : FtpException
    {
        /// <summary>
        /// Creates a new FtpMissingSocketException.
        /// </summary>
        /// <param name="innerException">The original exception.</param>
        public FtpMissingSocketException(Exception innerException)
            : base("Socket is missing", innerException)
        {
        }

#if !CORE
        /// <summary>
        /// Must be implemented so every Serializer can Deserialize the Exception
        /// </summary>
        protected FtpMissingSocketException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

#endif
    }
    /// <summary>
    /// Exception is thrown when TLS/SSL encryption could not be negotiated by the FTP server.
    /// </summary>
#if !CORE
    [Serializable]
#endif
    public class FtpSecurityNotAvailableException : FtpException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public FtpSecurityNotAvailableException()
            : base("FTPS security is not available on the server. To disable FTPS, set the EncryptionMode property to None.")
        {

        }

        /// <summary>
        /// Custom error message
        /// </summary>
        /// <param name="message">Error message</param>
        public FtpSecurityNotAvailableException(string message)
            : base(message)
        {
        }

#if !CORE
        /// <summary>
        /// Must be implemented so every Serializer can Deserialize the Exception
        /// </summary>
        protected FtpSecurityNotAvailableException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

#endif
    }
    /// <summary>
    /// Extension methods related to FTP tasks
    /// </summary>
    public static class IOExceptions
    {

        /// <summary>
        /// Check if operation can resume after <see cref="IOException"/>.
        /// </summary>
        /// <param name="exception">Received exception.</param>
        /// <returns>Result of checking.</returns>
        public static bool IsResumeAllowed(this IOException exception)
        {
            // resume if server disconnects midway (fixes #39 and #410)
            if (exception.InnerException != null || exception.Message.IsKnownError(FtpServerStrings.unexpectedEOF))
            {
                if (exception.InnerException is SocketException socketException)
                {
#if CORE
					return (int)socketException.SocketErrorCode == 10054;
#else
                    return socketException.ErrorCode == 10054;
#endif
                }

                return true;
            }

            return false;
        }


    }
    public class SocksProxyException : Exception
    {
        public SocksProxyException()
        {
        }

        public SocksProxyException(string message)
            : base(message)
        {
        }

        public SocksProxyException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
