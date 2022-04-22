﻿using System.IO;
using System.Net.Sockets;
using System.Data.Dubber.Helpers;
using System.Data.Dubber.Servers;

namespace System.Data.Dubber.Exceptions
{
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
}