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
    /// Server-specific handling for OpenVMS FTP servers
    /// </summary>
    public class OpenVmsServer : FtpBaseServer
    {

        /// <summary>
        /// Return the FtpServer enum value corresponding to your server, or Unknown if its a custom implementation.
        /// </summary>
        public override FtpServer ToEnum()
        {
            return FtpServer.OpenVMS;
        }

        /// <summary>
        /// Return true if your server is detected by the given FTP server welcome message.
        /// </summary>
        public override bool DetectByWelcome(string message)
        {

            // Detect OpenVMS server
            // Welcome message: "220 ftp.bedrock.net FTP-OpenVMS FTPD V5.5-3 (c) 2001 Process Software"
            if (message.Contains("OpenVMS FTPD"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Return true if your server is detected by the given SYST response message.
        /// Its a fallback method if the server did not send an identifying welcome message.
        /// </summary>
        public override bool DetectBySyst(string message)
        {

            // Detect OpenVMS server
            // SYST type: "VMS OpenVMS V8.4"
            if (message.Contains("OpenVMS"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Return your FTP server's default capabilities.
        /// Used if your server does not broadcast its capabilities using the FEAT command.
        /// </summary>
        public override string[] DefaultCapabilities()
        {

            // OpenVMS HGFTP
            // https://gist.github.com/robinrodricks/9631f9fad3c0fc4c667adfd09bd98762

            // assume the basic features supported
            return new[] { "CWD", "DELE", "LIST", "NLST", "MKD", "MDTM", "PASV", "PORT", "PWD", "QUIT", "RNFR", "RNTO", "SITE", "STOR", "STRU", "TYPE" };

        }

        /// <summary>
        /// Return true if the path is an absolute path according to your server's convention.
        /// </summary>
        public override bool IsAbsolutePath(string path)
        {

            // FIX : #380 for OpenVMS absolute paths are "SYS$SYSDEVICE:[USERS.mylogin]"
            // FIX : #402 for OpenVMS absolute paths are "SYSDEVICE:[USERS.mylogin]"
            // FIX : #424 for OpenVMS absolute paths are "FTP_DEFAULT:[WAGN_IN]"
            // FIX : #454 for OpenVMS absolute paths are "TOPAS$ROOT:[000000.TUIL.YR_20.SUBLIS]"
            if (new Regex("[A-Za-z$._]*:\\[[A-Za-z0-9$_.]*\\]").Match(path).Success)
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
            return FtpParser.VMS;
        }


    }
}
