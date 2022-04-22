﻿using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Data.Dubber;
using System.Data.Dubber.Servers;
#if (CORE || NETFX)
using System.Threading;
#endif
#if ASYNC
using System.Threading.Tasks;
#endif

namespace System.Data.Dubber.Servers.Handlers
{

    /// <summary>
    /// Server-specific handling for PyFtpdLib FTP servers
    /// </summary>
    public class PyFtpdLibServer : FtpBaseServer
    {

        /// <summary>
        /// Return the FtpServer enum value corresponding to your server, or Unknown if its a custom implementation.
        /// </summary>
        public override FtpServer ToEnum()
        {
            return FtpServer.PyFtpdLib;
        }

        /// <summary>
        /// Return true if your server is detected by the given FTP server welcome message.
        /// </summary>
        public override bool DetectByWelcome(string message)
        {

            // Detect PyFtpdLib server
            // Welcome message: "220 pyftpdlib 1.5.6 ready"
            if (message.Contains("pyftpdlib "))
            {
                return true;
            }

            return false;
        }

    }
}