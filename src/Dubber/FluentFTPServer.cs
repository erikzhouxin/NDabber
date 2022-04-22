using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Dubber
{
	/// <summary>
	/// The base class used for all FTP server specific support.
	/// You may extend this class to implement support for custom FTP servers.
	/// </summary>
	public abstract class FtpBaseServer
	{

		/// <summary>
		/// Return the FtpServer enum value corresponding to your server, or Unknown if its a custom implementation.
		/// </summary>
		public virtual FtpServer ToEnum()
		{
			return FtpServer.Unknown;
		}

		/// <summary>
		/// Return true if your server is detected by the given FTP server welcome message.
		/// </summary>
		public virtual bool DetectByWelcome(string message)
		{
			return false;
		}

		/// <summary>
		/// Return true if your server is detected by the given SYST response message.
		/// Its a fallback method if the server did not send an identifying welcome message.
		/// </summary>
		public virtual bool DetectBySyst(string message)
		{
			return false;
		}

		/// <summary>
		/// Detect if your FTP server supports the recursive LIST command (LIST -R).
		/// If you know for sure that this is supported, return true here.
		/// </summary>
		public virtual bool RecursiveList()
		{
			return false;
		}

		/// <summary>
		/// Return your FTP server's default capabilities.
		/// Used if your server does not broadcast its capabilities using the FEAT command.
		/// </summary>
		public virtual string[] DefaultCapabilities()
		{
			return null;
		}

		/// <summary>
		/// Return true if the path is an absolute path according to your server's convention.
		/// </summary>
		public virtual bool IsAbsolutePath(string path)
		{
			return false;
		}

		/// <summary>
		/// Return the default file listing parser to be used with your FTP server.
		/// </summary>
		public virtual FtpParser GetParser()
		{
			return FtpParser.Unix;
		}

		/// <summary>
		/// Perform server-specific delete directory commands here.
		/// Return true if you executed a server-specific command.
		/// </summary>
		public virtual bool DeleteDirectory(FtpClient client, string path, string ftppath, bool deleteContents, FtpListOption options)
		{
			return false;
		}

#if ASYNC
		/// <summary>
		/// Perform async server-specific delete directory commands here.
		/// Return true if you executed a server-specific command.
		/// </summary>
		public virtual Task<bool> DeleteDirectoryAsync(FtpClient client, string path, string ftppath, bool deleteContents, FtpListOption options, CancellationToken token) {
			return Task.FromResult(false);
		}
#endif

		/// <summary>
		/// Perform server-specific create directory commands here.
		/// Return true if you executed a server-specific command.
		/// </summary>
		public virtual bool CreateDirectory(FtpClient client, string path, string ftppath, bool force)
		{
			return false;
		}

#if ASYNC
		/// <summary>
		/// Perform async server-specific create directory commands here.
		/// Return true if you executed a server-specific command.
		/// </summary>
		public virtual Task<bool> CreateDirectoryAsync(FtpClient client, string path, string ftppath, bool force, CancellationToken token) {
			return Task.FromResult(false);
		}
#endif

		/// <summary>
		/// Perform server-specific post-connection commands here.
		/// Return true if you executed a server-specific command.
		/// </summary>
		public virtual void AfterConnected(FtpClient client)
		{

		}

#if ASYNC
		/// <summary>
		/// Perform server-specific post-connection commands here.
		/// Return true if you executed a server-specific command.
		/// </summary>
		public virtual Task AfterConnectedAsync(FtpClient client, CancellationToken token) {
#if NET45
			return Task.FromResult(true);
#else
			return Task.CompletedTask;
#endif
		}
#endif
		/// <summary>
		/// Return true if your server requires custom handling of file size.
		/// </summary>
		public virtual bool IsCustomFileSize()
		{
			return false;
		}
		/// <summary>
		/// Perform server-specific file size fetching commands here.
		/// Return the file size in bytes.
		/// </summary>
		public virtual long GetFileSize(FtpClient client, string path)
		{
			return 0;
		}

#if ASYNC
		/// <summary>
		/// Perform server-specific file size fetching commands here.
		/// Return the file size in bytes.
		/// </summary>
		public virtual Task<long> GetFileSizeAsync(FtpClient client, string path, CancellationToken token) {
			return Task.FromResult(0L);
		}
#endif
		/// <summary>
		/// Check if the given path is a root directory on your FTP server.
		/// If you are unsure, return false.
		/// </summary>
		public virtual bool IsRoot(FtpClient client, string path)
		{
			return false;
		}

    }
    /// <summary>
    /// All servers with server-specific handling and support are listed here.
    /// Its possible you can connect to other FTP servers too.
    /// 
    /// To add support for another standard FTP server:
    ///		1) Modify the FtpServer enum
    ///		2) Add a new class extending FtpBaseServer
    ///		3) Create a new instance of your class in AllServers (below)
    ///		
    /// To support a custom FTP server you only need to extend FtpBaseServer
    /// and set it on your client.ServerHandler before calling Connect.
    /// </summary>
    internal static class FtpServerSpecificHandler
    {

        internal static List<FtpBaseServer> AllServers = new List<FtpBaseServer> {
            new BFtpdServer(),
            new CerberusServer(),
            new CrushFtpServer(),
            new FileZillaServer(),
            new FritzBoxServer(),
            new Ftp2S3GatewayServer(),
            new GlFtpdServer(),
            new GlobalScapeEftServer(),
            new HomegateFtpServer(),
            new IBMOS400FtpServer(),
            new IBMzOSFtpServer(),
            new NonStopTandemServer(),
            new OpenVmsServer(),
            new ProFtpdServer(),
            new PureFtpdServer(),
            new PyFtpdLibServer(),
            new ServUServer(),
            new SolarisFtpServer(),
            new VsFtpdServer(),
            new WindowsCeServer(),
            new WindowsIISServer(),
            new WSFTPServer(),
            new WuFtpdServer(),
            new XLightServer(),
            new TitanFtpServer()
        };

        #region Working Connection Profiles

        /// <summary>
        /// Return a known working connection profile from the host/port combination.
        /// </summary>
        public static FtpProfile GetWorkingProfileFromHost(string Host, int Port)
        {

            // Azure App Services / Azure Websites
            if (Host.IndexOf("ftp.azurewebsites.windows.net", StringComparison.OrdinalIgnoreCase) > -1)
            {

                return new FtpProfile
                {
                    Protocols = SslProtocols.Tls,
                    DataConnection = FtpDataConnectionType.PASV,
                    RetryAttempts = 5,
                    SocketPollInterval = 1000,
                    Timeout = 2000,
                };

            }

            return null;
        }

        #endregion

        #region Detect Server

        /// <summary>
        /// Detect the FTP Server based on the welcome message sent by the server after getting the 220 connection command.
        /// Its the primary method.
        /// </summary>
        public static FtpServer DetectFtpServer(FtpClient client, FtpReply HandshakeReply)
        {
            var serverType = client.ServerType;

            if (HandshakeReply.Success && (HandshakeReply.Message != null || HandshakeReply.InfoMessages != null))
            {
                var message = (HandshakeReply.Message ?? "") + (HandshakeReply.InfoMessages ?? "");

                // try to detect any of the servers
                foreach (var server in AllServers)
                {
                    if (server.DetectByWelcome(message))
                    {
                        serverType = server.ToEnum();
                        break;
                    }
                }

                // trace it
                if (serverType != FtpServer.Unknown)
                {
                    client.LogLine(FtpTraceLevel.Info, "Status:   Detected FTP server: " + serverType.ToString());
                }
            }

            return serverType;
        }

        /// <summary>
        /// Get a default FTP Server handler based on the enum value.
        /// </summary>
        public static FtpBaseServer GetServerHandler(FtpServer value)
        {
            if (value != FtpServer.Unknown)
            {
                foreach (var server in AllServers)
                {
                    if (server.ToEnum() == value)
                    {
                        return server;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Detect the FTP Server based on the response to the SYST connection command.
        /// Its a fallback method if the server did not send an identifying welcome message.
        /// </summary>
        public static FtpOperatingSystem DetectFtpOSBySyst(FtpClient client)
        {
            var serverOS = client.ServerOS;

            // detect OS type
            var system = client.SystemType.ToUpper();

            if (system.StartsWith("WINDOWS"))
            {
                // Windows OS
                serverOS = FtpOperatingSystem.Windows;
            }
            else if (system.Contains("Z/OS"))
            {
                // IBM z/OS
                // Syst message: "215 MVS is the operating system of this server. FTP Server is running on z/OS."
                // Syst message: "215 UNIX is the operating system of this server. FTP Server is running on z/OS."
                serverOS = FtpOperatingSystem.IBMzOS;
            }
            else if (system.Contains("UNIX") || system.Contains("AIX"))
            {
                // Unix OS
                serverOS = FtpOperatingSystem.Unix;
            }
            else if (system.Contains("VMS"))
            {
                // VMS or OpenVMS
                serverOS = FtpOperatingSystem.VMS;
            }
            else if (system.Contains("OS/400"))
            {
                // IBM OS/400
                serverOS = FtpOperatingSystem.IBMOS400;
            }
            else if (system.Contains("SUNOS"))
            {
                // SUN OS
                serverOS = FtpOperatingSystem.SunOS;
            }
            else
            {
                // assume Unix OS
                serverOS = FtpOperatingSystem.Unknown;
            }

            return serverOS;
        }

        /// <summary>
        /// Detect the FTP Server based on the response to the SYST connection command.
        /// Its a fallback method if the server did not send an identifying welcome message.
        /// </summary>
        public static FtpServer DetectFtpServerBySyst(FtpClient client)
        {
            var serverType = client.ServerType;

            // detect server type
            if (serverType == FtpServer.Unknown)
            {

                // try to detect any of the servers
                foreach (var server in AllServers)
                {
                    if (server.DetectBySyst(client.SystemType))
                    {
                        serverType = server.ToEnum();
                        break;
                    }
                }

                // trace it
                if (serverType != FtpServer.Unknown)
                {
                    client.LogStatus(FtpTraceLevel.Info, "Detected FTP server: " + serverType.ToString());
                }

            }

            return serverType;
        }

        #endregion

        #region Detect Capabilities

        /// <summary>
        /// Populates the capabilities flags based on capabilities given in the list of strings.
        /// </summary>
        public static void GetFeatures(FtpClient client, List<FtpCapability> m_capabilities, ref FtpHashAlgorithm m_hashAlgorithms, string[] features)
        {
            foreach (var feat in features)
            {
                var featName = feat.Trim().ToUpper();

                // Handle possible multiline FEAT reply format
                if (featName.StartsWith("211-"))
                {
                    featName = featName.Substring(4);
                }

                if (featName.StartsWith("MLST") || featName.StartsWith("MLSD"))
                {
                    m_capabilities.AddOnce(FtpCapability.MLSD);
                }
                else if (featName.StartsWith("MDTM"))
                {
                    m_capabilities.AddOnce(FtpCapability.MDTM);
                }
                else if (featName.StartsWith("REST STREAM"))
                {
                    m_capabilities.AddOnce(FtpCapability.REST);
                }
                else if (featName.StartsWith("SIZE"))
                {
                    m_capabilities.AddOnce(FtpCapability.SIZE);
                }
                else if (featName.StartsWith("UTF8"))
                {
                    m_capabilities.AddOnce(FtpCapability.UTF8);
                }
                else if (featName.StartsWith("PRET"))
                {
                    m_capabilities.AddOnce(FtpCapability.PRET);
                }
                else if (featName.StartsWith("MFMT"))
                {
                    m_capabilities.AddOnce(FtpCapability.MFMT);
                }
                else if (featName.StartsWith("MFCT"))
                {
                    m_capabilities.AddOnce(FtpCapability.MFCT);
                }
                else if (featName.StartsWith("MFF"))
                {
                    m_capabilities.AddOnce(FtpCapability.MFF);
                }
                else if (featName.StartsWith("MMD5"))
                {
                    m_capabilities.AddOnce(FtpCapability.MMD5);
                }
                else if (featName.StartsWith("XMD5"))
                {
                    m_capabilities.AddOnce(FtpCapability.XMD5);
                }
                else if (featName.StartsWith("XCRC"))
                {
                    m_capabilities.AddOnce(FtpCapability.XCRC);
                }
                else if (featName.StartsWith("XSHA1"))
                {
                    m_capabilities.AddOnce(FtpCapability.XSHA1);
                }
                else if (featName.StartsWith("XSHA256"))
                {
                    m_capabilities.AddOnce(FtpCapability.XSHA256);
                }
                else if (featName.StartsWith("XSHA512"))
                {
                    m_capabilities.AddOnce(FtpCapability.XSHA512);
                }
                else if (featName.StartsWith("EPSV"))
                {
                    m_capabilities.AddOnce(FtpCapability.EPSV);
                }
                else if (featName.StartsWith("CPSV"))
                {
                    m_capabilities.AddOnce(FtpCapability.CPSV);
                }
                else if (featName.StartsWith("NOOP"))
                {
                    m_capabilities.AddOnce(FtpCapability.NOOP);
                }
                else if (featName.StartsWith("CLNT"))
                {
                    m_capabilities.AddOnce(FtpCapability.CLNT);
                }
                else if (featName.StartsWith("SSCN"))
                {
                    m_capabilities.AddOnce(FtpCapability.SSCN);
                }
                else if (featName.StartsWith("SITE MKDIR"))
                {
                    m_capabilities.AddOnce(FtpCapability.SITE_MKDIR);
                }
                else if (featName.StartsWith("SITE RMDIR"))
                {
                    m_capabilities.AddOnce(FtpCapability.SITE_RMDIR);
                }
                else if (featName.StartsWith("SITE UTIME"))
                {
                    m_capabilities.AddOnce(FtpCapability.SITE_UTIME);
                }
                else if (featName.StartsWith("SITE SYMLINK"))
                {
                    m_capabilities.AddOnce(FtpCapability.SITE_SYMLINK);
                }
                else if (featName.StartsWith("AVBL"))
                {
                    m_capabilities.AddOnce(FtpCapability.AVBL);
                }
                else if (featName.StartsWith("THMB"))
                {
                    m_capabilities.AddOnce(FtpCapability.THMB);
                }
                else if (featName.StartsWith("RMDA"))
                {
                    m_capabilities.AddOnce(FtpCapability.RMDA);
                }
                else if (featName.StartsWith("DSIZ"))
                {
                    m_capabilities.AddOnce(FtpCapability.DSIZ);
                }
                else if (featName.StartsWith("HOST"))
                {
                    m_capabilities.AddOnce(FtpCapability.HOST);
                }
                else if (featName.StartsWith("CCC"))
                {
                    m_capabilities.AddOnce(FtpCapability.CCC);
                }
                else if (featName.StartsWith("MODE Z"))
                {
                    m_capabilities.AddOnce(FtpCapability.MODE_Z);
                }
                else if (featName.StartsWith("LANG"))
                {
                    m_capabilities.AddOnce(FtpCapability.LANG);
                }
                else if (featName.StartsWith("HASH"))
                {
                    Match m;

                    m_capabilities.AddOnce(FtpCapability.HASH);

                    if ((m = Regex.Match(featName, @"^HASH\s+(?<types>.*)$")).Success)
                    {
                        foreach (var type in m.Groups["types"].Value.Split(';'))
                        {
                            switch (type.ToUpper().Trim())
                            {
                                case "SHA-1":
                                case "SHA-1*":
                                    m_hashAlgorithms |= FtpHashAlgorithm.SHA1;
                                    break;

                                case "SHA-256":
                                case "SHA-256*":
                                    m_hashAlgorithms |= FtpHashAlgorithm.SHA256;
                                    break;

                                case "SHA-512":
                                case "SHA-512*":
                                    m_hashAlgorithms |= FtpHashAlgorithm.SHA512;
                                    break;

                                case "MD5":
                                case "MD5*":
                                    m_hashAlgorithms |= FtpHashAlgorithm.MD5;
                                    break;

                                case "CRC":
                                case "CRC*":
                                    m_hashAlgorithms |= FtpHashAlgorithm.CRC;
                                    break;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Assume Capabilities

        /// <summary>
        /// Assume the FTP Server's capabilities if it does not support the FEAT command.
        /// </summary>
        public static void AssumeCapabilities(FtpClient client, FtpBaseServer handler, List<FtpCapability> m_capabilities, ref FtpHashAlgorithm m_hashAlgorithms)
        {

            // ask the server handler to assume its capabilities
            if (handler != null)
            {
                var caps = handler.DefaultCapabilities();
                if (caps != null)
                {

                    // add the assumed capabilities to our set
                    GetFeatures(client, m_capabilities, ref m_hashAlgorithms, caps);
                }
            }

        }

        #endregion

    }
    internal static class FtpServerStrings
    {

        #region File Exists

        /// <summary>
        /// Error messages returned by various servers when a file does not exist.
        /// Instead of throwing an error, we use these to detect and handle the file detection properly.
        /// MUST BE LOWER CASE!
        /// </summary>
        public static string[] fileNotFound = new[] {
            "can't find file",
            "can't check for file existence",
            "does not exist",
            "failed to open file",
            "not found",
            "no such file",
            "cannot find the file",
            "cannot find",
            "can't get file",
            "could not get file",
            "could not get file size",
            "cannot get file",
            "not a regular file",
            "file unavailable",
            "file is unavailable",
            "file not unavailable",
            "file is not available",
            "no files found",
            "no file found",
            "datei oder verzeichnis nicht gefunden",
            "can't find the path",
            "cannot find the path",
            "could not find the path",
            "file doesnot exist"
        };

        #endregion

        #region File Size

        /// <summary>
        /// Error messages returned by various servers when a file size is not supported in ASCII mode.
        /// MUST BE LOWER CASE!
        /// </summary>
        public static string[] fileSizeNotInASCII = new[] {
            "not allowed in ascii",
            "size not allowed in ascii",
            "n'est pas autorisé en mode ascii"
        };

        #endregion

        #region File Transfer

        /// <summary>
        /// Error messages returned by various servers when a file transfer temporarily failed.
        /// MUST BE LOWER CASE!
        /// </summary>
        public static string[] unexpectedEOF = new[] {
            "unexpected eof for remote file",
            "received an unexpected eof",
            "unexpected eof"
        };

        #endregion

        #region Create Directory

        /// <summary>
        /// Error messages returned by various servers when a folder already exists.
        /// Instead of throwing an error, we use these to detect and handle the folder creation properly.
        /// MUST BE LOWER CASE!
        /// </summary>
        public static string[] folderExists = new[] {
            "exist on server",
            "exists on server",
            "file exist",
            "directory exist",
            "folder exist",
            "file already exist",
            "directory already exist",
            "folder already exist",
        };

        #endregion

    }

    #region // Handlers
    /// <summary>
    /// Server-specific handling for BFTPd FTP servers
    /// </summary>
    public class BFtpdServer : FtpBaseServer
    {

        /// <summary>
        /// Return the FtpServer enum value corresponding to your server, or Unknown if its a custom implementation.
        /// </summary>
        public override FtpServer ToEnum()
        {
            return FtpServer.BFTPd;
        }

        /// <summary>
        /// Return true if your server is detected by the given FTP server welcome message.
        /// </summary>
        public override bool DetectByWelcome(string message)
        {

            // Detect BFTPd server
            // Welcome message: "220 bftpd 2.2.1 at 192.168.1.1 ready"
            if (message.Contains("bftpd "))
            {
                return true;
            }

            return false;
        }

    }
    /// <summary>
    /// Server-specific handling for Cerberus FTP servers
    /// </summary>
    public class CerberusServer : FtpBaseServer
    {

        /// <summary>
        /// Return the FtpServer enum value corresponding to your server, or Unknown if its a custom implementation.
        /// </summary>
        public override FtpServer ToEnum()
        {
            return FtpServer.Cerberus;
        }

        /// <summary>
        /// Return true if your server is detected by the given FTP server welcome message.
        /// </summary>
        public override bool DetectByWelcome(string message)
        {

            // Detect Cerberus server
            // Welcome message: "220-Cerberus FTP Server Personal Edition"
            if (message.Contains("Cerberus FTP"))
            {
                return true;
            }

            return false;
        }

    }
    /// <summary>
    /// Server-specific handling for CrushFTP servers
    /// </summary>
    public class CrushFtpServer : FtpBaseServer
    {

        /// <summary>
        /// Return the FtpServer enum value corresponding to your server, or Unknown if its a custom implementation.
        /// </summary>
        public override FtpServer ToEnum()
        {
            return FtpServer.CrushFTP;
        }

        /// <summary>
        /// Return true if your server is detected by the given FTP server welcome message.
        /// </summary>
        public override bool DetectByWelcome(string message)
        {

            // Detect CrushFTP server
            // Welcome message: "220 CrushFTP Server Ready!"
            if (message.Contains("CrushFTP Server"))
            {
                return true;
            }

            return false;
        }

    }
    /// <summary>
    /// Server-specific handling for FileZilla FTP servers
    /// </summary>
    public class FileZillaServer : FtpBaseServer
    {

        /// <summary>
        /// Return the FtpServer enum value corresponding to your server, or Unknown if its a custom implementation.
        /// </summary>
        public override FtpServer ToEnum()
        {
            return FtpServer.FileZilla;
        }

        /// <summary>
        /// Return true if your server is detected by the given FTP server welcome message.
        /// </summary>
        public override bool DetectByWelcome(string message)
        {

            // Detect FileZilla server
            // Welcome message: "FileZilla Server 0.9.60 beta"
            if (message.Contains("FileZilla Server"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Detect if your FTP server supports the recursive LIST command (LIST -R).
        /// If you know for sure that this is supported, return true here.
        /// </summary>
        public override bool RecursiveList()
        {

            // No support, per: https://trac.filezilla-project.org/ticket/1848
            return false;

        }

    }

    /// <summary>
    /// Server-specific handling for FritzBox FTP servers
    /// </summary>
    public class FritzBoxServer : FtpBaseServer
    {

        /// <summary>
        /// Return the FtpServer enum value corresponding to your server, or Unknown if its a custom implementation.
        /// </summary>
        public override FtpServer ToEnum()
        {
            return FtpServer.FritzBox;
        }

        /// <summary>
        /// Return true if your server is detected by the given FTP server welcome message.
        /// </summary>
        public override bool DetectByWelcome(string message)
        {

            // Detect FTP2S3 server
            // Welcome message: "220 FRITZ!Box7490 FTP server ready"
            if (message.Contains("FRITZ!Box"))
            {
                return true;
            }
            return false;
        }

    }
    /// <summary>
    /// Server-specific handling for FTP2S3Gateway FTP servers
    /// </summary>
    public class Ftp2S3GatewayServer : FtpBaseServer
    {

        /// <summary>
        /// Return the FtpServer enum value corresponding to your server, or Unknown if its a custom implementation.
        /// </summary>
        public override FtpServer ToEnum()
        {
            return FtpServer.FTP2S3Gateway;
        }

        /// <summary>
        /// Return true if your server is detected by the given FTP server welcome message.
        /// </summary>
        public override bool DetectByWelcome(string message)
        {

            // Detect FTP2S3 server
            // Welcome message: "220 Aruba FTP2S3 gateway 1.0.1 ready"
            if (message.Contains("FTP2S3 gateway"))
            {
                return true;
            }
            return false;
        }

    }
    /// <summary>
    /// Server-specific handling for glFTPd FTP servers
    /// </summary>
    public class GlFtpdServer : FtpBaseServer
    {

        /// <summary>
        /// Return the FtpServer enum value corresponding to your server, or Unknown if its a custom implementation.
        /// </summary>
        public override FtpServer ToEnum()
        {
            return FtpServer.glFTPd;
        }

        /// <summary>
        /// Return true if your server is detected by the given FTP server welcome message.
        /// </summary>
        public override bool DetectByWelcome(string message)
        {

            // Detect glFTPd server
            // Welcome message: "220 W 00 T (glFTPd 2.01 Linux+TLS) ready."
            // Welcome message: "220 <hostname> (glFTPd 2.01 Linux+TLS) ready."
            if (message.Contains("glFTPd "))
            {
                return true;
            }

            return false;
        }

    }
    /// <summary>
    /// Server-specific handling for GlobalScapeEFT FTP servers
    /// </summary>
    public class GlobalScapeEftServer : FtpBaseServer
    {

        /// <summary>
        /// Return the FtpServer enum value corresponding to your server, or Unknown if its a custom implementation.
        /// </summary>
        public override FtpServer ToEnum()
        {
            return FtpServer.GlobalScapeEFT;
        }

        /// <summary>
        /// Return true if your server is detected by the given FTP server welcome message.
        /// </summary>
        public override bool DetectByWelcome(string message)
        {

            // Detect GlobalScape EFT server
            // Welcome message: "EFT Server Enterprise 7.4.5.6"
            if (message.Contains("EFT Server"))
            {
                return true;
            }

            return false;
        }

    }
    /// <summary>
    /// Server-specific handling for HomegateFTP servers
    /// </summary>
    public class HomegateFtpServer : FtpBaseServer
    {

        /// <summary>
        /// Return the FtpServer enum value corresponding to your server, or Unknown if its a custom implementation.
        /// </summary>
        public override FtpServer ToEnum()
        {
            return FtpServer.HomegateFTP;
        }

        /// <summary>
        /// Return true if your server is detected by the given FTP server welcome message.
        /// </summary>
        public override bool DetectByWelcome(string message)
        {

            // Detect Homegate FTP server
            // Welcome message: "220 Homegate FTP Server ready"
            if (message.Contains("Homegate FTP Server"))
            {
                return true;
            }

            return false;
        }

    }
    /// <summary>
    /// Server-specific handling for IBMOS400FTP servers
    /// </summary>
    public class IBMOS400FtpServer : FtpBaseServer
    {

        /// <summary>
        /// Return the FtpServer enum value corresponding to your server, or Unknown if its a custom implementation.
        /// </summary>
        public override FtpServer ToEnum()
        {
            return FtpServer.IBMOS400FTP;
        }

        /// <summary>
        /// Return true if your server is detected by the given FTP server welcome message.
        /// </summary>
        public override bool DetectByWelcome(string message)
        {

            // Detect IBM OS/400 server
            // Welcome message: "220-QTCP at xxxxxx.xxxxx.xxx.com."
            if (message.Contains("QTCP at"))
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
            return FtpParser.IBMOS400;
        }

    }
    /// <summary>
    /// Server-specific handling for IBMzOSFTP servers
    /// </summary>
    public class IBMzOSFtpServer : FtpBaseServer
    {

        /// <summary>
        /// Return the FtpServer enum value corresponding to your server, or Unknown if its a custom implementation.
        /// </summary>
        public override FtpServer ToEnum()
        {
            return FtpServer.IBMzOSFTP;
        }

        /// <summary>
        /// Return true if your server is detected by the given FTP server welcome message.
        /// </summary>
        public override bool DetectByWelcome(string message)
        {

            // Detect IBM z/OS server
            // Welcome message: "220-FTPD1 IBM FTP CS V2R3 at mysite.gov, 16:51:54 on 2019-12-12."
            if (message.Contains("IBM FTP CS"))
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
            return FtpParser.IBMzOS;
        }

        /// <summary>
        /// Perform server-specific post-connection commands here.
        /// Return true if you executed a server-specific command.
        /// </summary>
        public override void AfterConnected(FtpClient client)
        {
            FtpReply reply;
            if (!(reply = client.Execute("SITE DATASETMODE")).Success)
            {
                throw new FtpCommandException(reply);
            }
            if (!(reply = client.Execute("SITE QUOTESOVERRIDE")).Success)
            {
                throw new FtpCommandException(reply);
            }
        }

#if ASYNC
		/// <summary>
		/// Perform server-specific post-connection commands here.
		/// Return true if you executed a server-specific command.
		/// </summary>
		public override async Task AfterConnectedAsync(FtpClient client, CancellationToken token) {
			FtpReply reply;
			if (!(reply = await client.ExecuteAsync("SITE DATASETMODE", token)).Success) {
				throw new FtpCommandException(reply);
			}
			if (!(reply = await client.ExecuteAsync("SITE QUOTESOVERRIDE", token)).Success) {
				throw new FtpCommandException(reply);
			}
		}
#endif

        #region Get z/OS File Size

        public override bool IsCustomFileSize()
        {
            return true;
        }

        /// <summary>
        /// Get z/OS file size
        /// </summary>
        /// <param name="path">The full path of the file whose size you want to retrieve</param>
        /// <remarks>
        /// Make sure you are in the right realm (z/OS or HFS) before doing this
        /// </remarks>
        /// <returns>The size of the file</returns>
        public override long GetFileSize(FtpClient client, string path)
        {

            // prevent automatic parser detection switching to unix on HFS paths
            client.ListingParser = FtpParser.IBMzOS;

            // get metadata of the file
            FtpListItem[] entries = client.GetListing(path);

            // no entries or more than one: path is NOT for a single dataset or file
            if (entries.Length != 1) { return -1; }

            // if the path is for a SINGLE dataset or file, there will be only one entry
            FtpListItem entry = entries[0];

            // z/OS list parser will have determined that size
            return entry.Size;
        }

#if ASYNC
		/// <summary>
		/// Get z/OS file size
		/// </summary>
		/// <param name="path">The full path of the file whose size you want to retrieve</param>
		/// <remarks>
		/// Make sure you are in the right realm (z/OS or HFS) before doing this
		/// </remarks>
		/// <returns>The size of the file</returns>
		public override async Task<long> GetFileSizeAsync(FtpClient client, string path, CancellationToken token) {

			// prevent automatic parser detection switching to unix on HFS paths
			client.ListingParser = FtpParser.IBMzOS;

			// get metadata of the file
			FtpListItem[] entries = await client.GetListingAsync(path, token);

			// no entries or more than one: path is NOT for a single dataset or file
			if (entries.Length != 1) return -1;

			// if the path is for a SINGLE dataset or file, there will be only one entry
			FtpListItem entry = entries[0];

			// z/OS list parser will have determined that size
			return entry.Size;
		}
#endif
        #endregion

        /// <summary>
        /// Check if the given path is a root directory on your FTP server.
        /// If you are unsure, return false.
        /// </summary>
        public override bool IsRoot(FtpClient client, string path)
        {

            // Note: If on z/OS you have somehow managed to CWD "over" the top, i.e.
            // PWD returns "''", it is also root - you would need to CWD to some HLQ
            // that only you can imagine. There is no way to list the available top
            // level HLQs.

            // z/OS HFS root
            if (path == "/")
            {
                return true;
            }

            // z/OS HFS some path
            if (path.StartsWith("/"))
            {
                return false;
            }

            // z/OS HLQ (like 'SYS1.' or '')
            if (path.Trim('\'').TrimEnd('.').Split('.').Length <= 1)
            {
                return true;
            }

            // all others
            return false;
        }
    }
    /// <summary>
    /// Server-specific handling for NonStop/Tandem FTP servers
    /// </summary>
    public class NonStopTandemServer : FtpBaseServer
    {

        /// <summary>
        /// Return the FtpServer enum value corresponding to your server, or Unknown if its a custom implementation.
        /// </summary>
        public override FtpServer ToEnum()
        {
            return FtpServer.NonStopTandem;
        }

        /// <summary>
        /// Return true if your server is detected by the given FTP server welcome message.
        /// </summary>
        public override bool DetectByWelcome(string message)
        {

            // Detect Tandem/NonStop server
            // Welcome message: "220 mysite.com FTP SERVER T9552H02 (Version H02 TANDEM 11SEP2008) ready."
            // Welcome message: "220 FTP SERVER T9552G08 (Version G08 TANDEM 15JAN2008) ready."
            if (message.Contains("FTP SERVER ") && message.Contains(" TANDEM "))
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
            return FtpParser.NonStop;
        }

    }
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
    /// <summary>
    /// Server-specific handling for ProFTPD FTP servers
    /// </summary>
    public class ProFtpdServer : FtpBaseServer
    {

        /// <summary>
        /// Return the FtpServer enum value corresponding to your server, or Unknown if its a custom implementation.
        /// </summary>
        public override FtpServer ToEnum()
        {
            return FtpServer.ProFTPD;
        }

        /// <summary>
        /// Return true if your server is detected by the given FTP server welcome message.
        /// </summary>
        public override bool DetectByWelcome(string message)
        {

            // Detect ProFTPd server
            // Welcome message: "ProFTPD 1.3.5rc3 Server (***) [::ffff:***]"
            if (message.Contains("ProFTPD"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Detect if your FTP server supports the recursive LIST command (LIST -R).
        /// If you know for sure that this is supported, return true here.
        /// </summary>
        public override bool RecursiveList()
        {

            // Has support, per: http://www.proftpd.org/docs/howto/ListOptions.html
            return true;
        }

        /// <summary>
        /// Perform server-specific delete directory commands here.
        /// Return true if you executed a server-specific command.
        /// </summary>
        public override bool DeleteDirectory(FtpClient client, string path, string ftppath, bool deleteContents, FtpListOption options)
        {

            // Support #378 - Support RMDIR command for ProFTPd
            if (deleteContents && client.HasFeature(FtpCapability.SITE_RMDIR))
            {
                if ((client.Execute("SITE RMDIR " + ftppath)).Success)
                {
                    client.LogStatus(FtpTraceLevel.Verbose, "Used the server-specific SITE RMDIR command to quickly delete directory: " + ftppath);
                    return true;
                }
                else
                {
                    client.LogStatus(FtpTraceLevel.Verbose, "Failed to use the server-specific SITE RMDIR command to quickly delete directory: " + ftppath);
                }
            }

            return false;
        }

#if ASYNC
		/// <summary>
		/// Perform async server-specific delete directory commands here.
		/// Return true if you executed a server-specific command.
		/// </summary>
		public override async Task<bool> DeleteDirectoryAsync(FtpClient client, string path, string ftppath, bool deleteContents, FtpListOption options, CancellationToken token) {
			
			// Support #378 - Support RMDIR command for ProFTPd
			if (deleteContents && client.HasFeature(FtpCapability.SITE_RMDIR)) {
				if ((await client.ExecuteAsync("SITE RMDIR " + ftppath, token)).Success) {
					client.LogStatus(FtpTraceLevel.Verbose, "Used the server-specific SITE RMDIR command to quickly delete: " + ftppath);
					return true;
				}
				else {
					client.LogStatus(FtpTraceLevel.Verbose, "Failed to use the server-specific SITE RMDIR command to quickly delete: " + ftppath);
				}
			}

			return false;
		}
#endif

        /// <summary>
        /// Perform server-specific create directory commands here.
        /// Return true if you executed a server-specific command.
        /// </summary>
        public override bool CreateDirectory(FtpClient client, string path, string ftppath, bool force)
        {

            // Support #378 - Support MKDIR command for ProFTPd
            if (client.HasFeature(FtpCapability.SITE_MKDIR))
            {
                if ((client.Execute("SITE MKDIR " + ftppath)).Success)
                {
                    client.LogStatus(FtpTraceLevel.Verbose, "Used the server-specific SITE MKDIR command to quickly create: " + ftppath);
                    return true;
                }
                else
                {
                    client.LogStatus(FtpTraceLevel.Verbose, "Failed to use the server-specific SITE MKDIR command to quickly create: " + ftppath);
                }
            }

            return false;
        }

#if ASYNC
		/// <summary>
		/// Perform async server-specific create directory commands here.
		/// Return true if you executed a server-specific command.
		/// </summary>
		public override async Task<bool> CreateDirectoryAsync(FtpClient client, string path, string ftppath, bool force, CancellationToken token) {

			// Support #378 - Support MKDIR command for ProFTPd
			if (client.HasFeature(FtpCapability.SITE_MKDIR)) {
				if ((await client.ExecuteAsync("SITE MKDIR " + ftppath, token)).Success) {
					client.LogStatus(FtpTraceLevel.Verbose, "Used the server-specific SITE MKDIR command to quickly create: " + ftppath);
					return true;
				}
				else {
					client.LogStatus(FtpTraceLevel.Verbose, "Failed to use the server-specific SITE MKDIR command to quickly create: " + ftppath);
				}
			}

			return false;
		}
#endif

    }

    /// <summary>
    /// Server-specific handling for PureFTPd FTP servers
    /// </summary>
    public class PureFtpdServer : FtpBaseServer
    {

        /// <summary>
        /// Return the FtpServer enum value corresponding to your server, or Unknown if its a custom implementation.
        /// </summary>
        public override FtpServer ToEnum()
        {
            return FtpServer.PureFTPd;
        }

        /// <summary>
        /// Return true if your server is detected by the given FTP server welcome message.
        /// </summary>
        public override bool DetectByWelcome(string message)
        {

            // Detect Pure-FTPd server
            // Welcome message: "---------- Welcome to Pure-FTPd [privsep] [TLS] ----------"
            if (message.Contains("Pure-FTPd"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Detect if your FTP server supports the recursive LIST command (LIST -R).
        /// If you know for sure that this is supported, return true here.
        /// </summary>
        public override bool RecursiveList()
        {

            // Has support, per https://download.pureftpd.org/pub/pure-ftpd/doc/README
            return true;
        }

    }
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
    /// <summary>
    /// Server-specific handling for ServU FTP servers
    /// </summary>
    public class ServUServer : FtpBaseServer
    {

        /// <summary>
        /// Return the FtpServer enum value corresponding to your server, or Unknown if its a custom implementation.
        /// </summary>
        public override FtpServer ToEnum()
        {
            return FtpServer.ServU;
        }

        /// <summary>
        /// Return true if your server is detected by the given FTP server welcome message.
        /// </summary>
        public override bool DetectByWelcome(string message)
        {

            // Detect Serv-U server
            // Welcome message: "220 Serv-U FTP Server v5.0 for WinSock ready."
            if (message.Contains("Serv-U FTP"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Perform server-specific delete directory commands here.
        /// Return true if you executed a server-specific command.
        /// </summary>
        public override bool DeleteDirectory(FtpClient client, string path, string ftppath, bool deleteContents, FtpListOption options)
        {

            // Support #88 - Support RMDA command for Serv-U
            if (deleteContents && client.HasFeature(FtpCapability.RMDA))
            {
                if ((client.Execute("RMDA " + ftppath)).Success)
                {
                    client.LogStatus(FtpTraceLevel.Verbose, "Used the server-specific RMDA command to quickly delete directory: " + ftppath);
                    return true;
                }
                else
                {
                    client.LogStatus(FtpTraceLevel.Verbose, "Failed to use the server-specific RMDA command to quickly delete directory: " + ftppath);
                }
            }

            return false;
        }

#if ASYNC
		/// <summary>
		/// Perform async server-specific delete directory commands here.
		/// Return true if you executed a server-specific command.
		/// </summary>
		public override async Task<bool> DeleteDirectoryAsync(FtpClient client, string path, string ftppath, bool deleteContents, FtpListOption options, CancellationToken token) {

			// Support #88 - Support RMDA command for Serv-U
			if (deleteContents && client.HasFeature(FtpCapability.RMDA)) {
				if ((await client.ExecuteAsync("RMDA " + ftppath, token)).Success) {
					client.LogStatus(FtpTraceLevel.Verbose, "Used the server-specific RMDA command to quickly delete directory: " + ftppath);
					return true;
				}
				else {
					client.LogStatus(FtpTraceLevel.Verbose, "Failed to use the server-specific RMDA command to quickly delete directory: " + ftppath);
				}
			}

			return false;
		}
#endif

    }
    /// <summary>
    /// Server-specific handling for SolarisFTP servers
    /// </summary>
    public class SolarisFtpServer : FtpBaseServer
    {

        /// <summary>
        /// Return the FtpServer enum value corresponding to your server, or Unknown if its a custom implementation.
        /// </summary>
        public override FtpServer ToEnum()
        {
            return FtpServer.SolarisFTP;
        }

        /// <summary>
        /// Return true if your server is detected by the given SYST response message.
        /// Its a fallback method if the server did not send an identifying welcome message.
        /// </summary>
        public override bool DetectBySyst(string message)
        {

            // Detect SolarisFTP server
            // SYST response: "215 UNIX Type: L8 Version: SUNOS"
            if (message.Contains("SUNOS"))
            {
                return true;
            }

            return false;
        }

    }
    /// <summary>
    /// Server-specific handling for Titan FTP servers
    /// </summary>
    public class TitanFtpServer : FtpBaseServer
    {

        /// <summary>
        /// Return the FtpServer enum value corresponding to your server, or Unknown if its a custom implementation.
        /// </summary>
        public override FtpServer ToEnum()
        {
            return FtpServer.TitanFTP;
        }

        /// <summary>
        /// Return true if your server is detected by the given FTP server welcome message.
        /// </summary>
        public override bool DetectByWelcome(string message)
        {

            // Detect Pure-FTPd server
            // Welcome message: "220 Titan FTP Server 10.01.1740 Ready"
            if (message.Contains("Titan FTP"))
            {
                return true;
            }

            return false;
        }

    }
    /// <summary>
    /// Server-specific handling for VsFTPd FTP servers
    /// </summary>
    public class VsFtpdServer : FtpBaseServer
    {

        /// <summary>
        /// Return the FtpServer enum value corresponding to your server, or Unknown if its a custom implementation.
        /// </summary>
        public override FtpServer ToEnum()
        {
            return FtpServer.VsFTPd;
        }

        /// <summary>
        /// Return true if your server is detected by the given FTP server welcome message.
        /// </summary>
        public override bool DetectByWelcome(string message)
        {

            // Detect vsFTPd server
            // Welcome message: "(vsFTPd 3.0.3)"
            if (message.Contains("(vsFTPd"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Detect if your FTP server supports the recursive LIST command (LIST -R).
        /// If you know for sure that this is supported, return true here.
        /// </summary>
        public override bool RecursiveList()
        {

            // Has support, but OFF by default, per: https://linux.die.net/man/5/vsftpd.conf
            return false; // impossible to detect on a server-by-server basis
        }

    }
    /// <summary>
    /// Server-specific handling for WindowsCE FTP servers
    /// </summary>
    public class WindowsCeServer : FtpBaseServer
    {

        /// <summary>
        /// Return the FtpServer enum value corresponding to your server, or Unknown if its a custom implementation.
        /// </summary>
        public override FtpServer ToEnum()
        {
            return FtpServer.WindowsCE;
        }

        /// <summary>
        /// Return true if your server is detected by the given SYST response message.
        /// Its a fallback method if the server did not send an identifying welcome message.
        /// </summary>
        public override bool DetectBySyst(string message)
        {

            // Detect WindowsCE server
            // SYST type: "Windows_CE version 7.0"
            if (message.Contains("Windows_CE"))
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
    /// <summary>
    /// Server-specific handling for WS_FTP servers
    /// </summary>
    public class WSFTPServer : FtpBaseServer
    {

        /// <summary>
        /// Return the FtpServer enum value corresponding to your server, or Unknown if its a custom implementation.
        /// </summary>
        public override FtpServer ToEnum()
        {
            return FtpServer.WSFTPServer;
        }

        /// <summary>
        /// Return true if your server is detected by the given FTP server welcome message.
        /// </summary>
        public override bool DetectByWelcome(string message)
        {

            // Detect FTP2S3 server
            // Welcome message: "220 ***.com X2 WS_FTP Server 8.5.0(24135676)"
            if (message.Contains("WS_FTP Server"))
            {
                return true;
            }
            return false;
        }

    }
    /// <summary>
    /// Server-specific handling for WuFTPd FTP servers
    /// </summary>
    public class WuFtpdServer : FtpBaseServer
    {

        /// <summary>
        /// Return the FtpServer enum value corresponding to your server, or Unknown if its a custom implementation.
        /// </summary>
        public override FtpServer ToEnum()
        {
            return FtpServer.WuFTPd;
        }

        /// <summary>
        /// Return true if your server is detected by the given FTP server welcome message.
        /// </summary>
        public override bool DetectByWelcome(string message)
        {

            // Detect WuFTPd server
            // Welcome message: "FTP server (Revision 9.0 Version wuftpd-2.6.1 Mon Jun 30 09:28:28 GMT 2014) ready"
            // Welcome message: "220 DH FTP server (Version wu-2.6.2-5) ready"
            if (message.Contains("Version wuftpd") || message.Contains("Version wu-"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Detect if your FTP server supports the recursive LIST command (LIST -R).
        /// If you know for sure that this is supported, return true here.
        /// </summary>
        public override bool RecursiveList()
        {

            // No support, per: http://wu-ftpd.therockgarden.ca/man/ftpd.html
            return false;
        }

        /// <summary>
        /// Return your FTP server's default capabilities.
        /// Used if your server does not broadcast its capabilities using the FEAT command.
        /// </summary>
        public override string[] DefaultCapabilities()
        {

            // HP-UX version of wu-ftpd 2.6.1
            // http://nixdoc.net/man-pages/HP-UX/ftpd.1m.html

            // assume the basic features supported
            return new[] { "ABOR", "ACCT", "ALLO", "APPE", "CDUP", "CWD", "DELE", "EPSV", "EPRT", "HELP", "LIST", "LPRT", "LPSV", "MKD", "MDTM", "MODE", "NLST", "NOOP", "PASS", "PASV", "PORT", "PWD", "QUIT", "REST", "RETR", "RMD", "RNFR", "RNTO", "SITE", "SIZE", "STAT", "STOR", "STOU", "STRU", "SYST", "TYPE" };

        }

    }
    /// <summary>
    /// Server-specific handling for XLight FTP servers
    /// </summary>
    public class XLightServer : FtpBaseServer
    {

        /// <summary>
        /// Return the FtpServer enum value corresponding to your server, or Unknown if its a custom implementation.
        /// </summary>
        public override FtpServer ToEnum()
        {
            return FtpServer.XLight;
        }

        /// <summary>
        /// Return true if your server is detected by the given FTP server welcome message.
        /// </summary>
        public override bool DetectByWelcome(string message)
        {

            // Detect XLight server
            // Welcome message: "220 Xlight FTP Server 3.9 ready"
            if (message.Contains("Xlight FTP Server"))
            {
                return true;
            }

            return false;
        }


    }
    #endregion // Handlers
}
