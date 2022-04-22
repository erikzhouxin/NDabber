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
    /// Server-specific handling for WindowsServer/IIS FTP servers
    /// </summary>
    public class WindowsIISServer : FtpBaseServer
    {

        /// <summary>
        /// Return the FtpServer enum value corresponding to your server, or Unknown if its a custom implementation.
        /// </summary>
        public override FtpServer ToEnum()
        {
            return FtpServer.WindowsServerIIS;
        }

        /// <summary>
        /// Return true if your server is detected by the given FTP server welcome message.
        /// </summary>
        public override bool DetectByWelcome(string message)
        {

            // Detect Windows Server/IIS FTP server
            // Welcome message: "220-Microsoft FTP Service."
            if (message.Contains("Microsoft FTP Service"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Return the default file listing parser to be used with your FTP server.
        /// </summary>
        public override FtpParser GetParser()
        {
            return FtpParser.Windows;
        }

    }
}
