using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Data.Dubber
{
    #region // Events
    /// <summary>
    /// Event fired if a bad SSL certificate is encountered. This even is used internally; if you
    /// don't have a specific reason for using it you are probably looking for FtpSslValidation.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="e"></param>
    public delegate void FtpSocketStreamSslValidation(FtpSocketStream stream, FtpSslValidationEventArgs e);
    /// <summary>
    /// Event is fired when a SSL certificate needs to be validated
    /// </summary>
    /// <param name="control">The control connection that triggered the event</param>
    /// <param name="e">Event args</param>
    public delegate void FtpSslValidation(FtpClient control, FtpSslValidationEventArgs e);

    /// <summary>
    /// Event args for the FtpSslValidationError delegate
    /// </summary>
    public class FtpSslValidationEventArgs : EventArgs
    {
        private X509Certificate m_certificate = null;

        /// <summary>
        /// The certificate to be validated
        /// </summary>
        public X509Certificate Certificate
        {
            get => m_certificate;
            set => m_certificate = value;
        }

        private X509Chain m_chain = null;

        /// <summary>
        /// The certificate chain
        /// </summary>
        public X509Chain Chain
        {
            get => m_chain;
            set => m_chain = value;
        }

        private SslPolicyErrors m_policyErrors = SslPolicyErrors.None;

        /// <summary>
        /// Validation errors, if any.
        /// </summary>
        public SslPolicyErrors PolicyErrors
        {
            get => m_policyErrors;
            set => m_policyErrors = value;
        }

        private bool m_accept = false;

        /// <summary>
        /// Gets or sets a value indicating if this certificate should be accepted. The default
        /// value is false. If the certificate is not accepted, an AuthenticationException will
        /// be thrown.
        /// </summary>
        public bool Accept
        {
            get => m_accept;
            set => m_accept = value;
        }
    }
    #endregion // Events
    /// <summary>
    /// Object that keeps track of an active FXP Connection between 2 FTP servers.
    /// </summary>
    public class FtpFxpSession : IDisposable
    {
        /// <summary>
        /// A connection to the FTP server where the file or folder is currently stored
        /// </summary>
        public FtpClient SourceServer;

        /// <summary>
        /// A connection to the destination FTP server where you want to create the file or folder
        /// </summary>
        public FtpClient TargetServer;

        /// <summary>
        /// A connection to the destination FTP server used to track progress while transfer is going on.
        /// </summary>
        public FtpClient ProgressServer;

        /// <summary>
        /// Gets a value indicating if this object has already been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Closes an FXP connection by disconnecting and disposing off the FTP clients that are
        /// cloned for this FXP connection. Manually created FTP clients are untouched.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            if (SourceServer != null)
            {
                SourceServer.AutoDispose();
                SourceServer = null;
            }
            if (TargetServer != null)
            {
                TargetServer.AutoDispose();
                TargetServer = null;
            }
            if (ProgressServer != null)
            {
                ProgressServer.AutoDispose();
                ProgressServer = null;
            }

            IsDisposed = true;
            GC.SuppressFinalize(this);

        }
    }
    /// <summary>
    /// Represents a computed hash of an object
    /// on the FTP server. See the following link
    /// for more information:
    /// http://tools.ietf.org/html/draft-bryan-ftpext-hash-02
    /// </summary>
    public class FtpHash
    {
        private FtpHashAlgorithm m_algorithm = FtpHashAlgorithm.NONE;

        /// <summary>
        /// Gets the algorithm that was used to compute the hash
        /// </summary>
        public FtpHashAlgorithm Algorithm
        {
            get => m_algorithm;
            internal set => m_algorithm = value;
        }

        private string m_value = null;

        /// <summary>
        /// Gets the computed hash returned by the server
        /// </summary>
        public string Value
        {
            get => m_value;
            internal set => m_value = value;
        }

        /// <summary>
        /// Gets a value indicating if this object represents a
        /// valid hash response from the server.
        /// </summary>
        public bool IsValid => m_algorithm != FtpHashAlgorithm.NONE && !string.IsNullOrEmpty(m_value);

        /// <summary>
        /// Computes the hash for the specified file and compares
        /// it to the value in this object. CRC hashes are not supported 
        /// because there is no built-in support in the .net framework and
        /// a CRC implementation exceeds the scope of this project. If you
        /// attempt to call this on a CRC hash a <see cref="NotImplementedException"/> will
        /// be thrown.
        /// </summary>
        /// <param name="file">The file to compute the hash for</param>
        /// <returns>True if the computed hash matches what's stored in this object.</returns>
        /// <exception cref="NotImplementedException">Thrown if called on a CRC Hash</exception>
        public bool Verify(string file)
        {

            // read the file using a FileStream or by reading it entirely into memory if it fits within 1 MB
            using (var istream = FtpFileStream.GetFileReadStream(null, file, false, 1024 * 1024))
            {

                // verify the file data against the hash reported by the FTP server
                return Verify(istream);
            }
        }

        /// <summary>
        /// Computes the hash for the specified stream and compares
        /// it to the value in this object. CRC hashes are not supported 
        /// because there is no built-in support in the .net framework and
        /// a CRC implementation exceeds the scope of this project. If you
        /// attempt to call this on a CRC hash a <see cref="NotImplementedException"/> will
        /// be thrown.
        /// </summary>
        /// <param name="istream">The stream to compute the hash for</param>
        /// <returns>True if the computed hash matches what's stored in this object.</returns>
        /// <exception cref="NotImplementedException">Thrown if called on a CRC Hash</exception>
        public bool Verify(Stream istream)
        {
            if (IsValid)
            {
                HashAlgorithm hashAlg = null;

                switch (m_algorithm)
                {
                    case FtpHashAlgorithm.SHA1:
#if NETFx
                        hashAlg = SHA1.Create();
#else
                        hashAlg = new SHA1CryptoServiceProvider();
#endif
                        break;

#if !NET20
                    case FtpHashAlgorithm.SHA256:
#if NETFx
                        hashAlg = SHA256.Create();
#else
                        hashAlg = new SHA256CryptoServiceProvider();
#endif
                        break;

                    case FtpHashAlgorithm.SHA512:
#if NETFx
                        hashAlg = SHA512.Create();
#else
                        hashAlg = new SHA512CryptoServiceProvider();
#endif
                        break;

#endif
                    case FtpHashAlgorithm.MD5:
#if NETFx
                        hashAlg = MD5.Create();
#else
                        hashAlg = new MD5CryptoServiceProvider();
#endif
                        break;

                    case FtpHashAlgorithm.CRC:

                        hashAlg = new CRC32();

                        break;

                    default:
                        throw new NotImplementedException("Unknown hash algorithm: " + m_algorithm.ToString());
                }

                try
                {
                    byte[] data = null;
                    var hash = new StringBuilder();

                    data = hashAlg.ComputeHash(istream);
                    if (data != null)
                    {

                        // convert hash to hex string
                        foreach (var b in data)
                        {
                            hash.Append(b.ToString("x2"));
                        }
                        var hashStr = hash.ToString();

                        // check if hash exactly matches
                        if (hashStr.Equals(m_value, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                        // check if hash matches without the "0" prefix that .NET CRC sometimes generates
                        // to fix #820: Validation of short CRC checksum fails due to mismatch with hex format
                        if (Strings.RemovePrefix(hashStr, "0").Equals(Strings.RemovePrefix(m_value, "0"), StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                        return false;
                    }
                }
                finally
                {

                    // .NET 2.0 doesn't provide access to Dispose() for HashAlgorithm
#if !NET20 && !NET35
                    if (hashAlg != null)
                    {
                        hashAlg.Dispose();
                    }

#endif
                }
            }

            return false;
        }

        /// <summary>
        /// Creates an empty instance.
        /// </summary>
        internal FtpHash()
        {
        }
    }
    /// <summary>
    /// Represents a file system object on the server
    /// </summary>
    public class FtpListItem
    {
        /// <summary>
        /// Blank constructor, you will need to fill arguments manually.
        /// 
        /// NOTE TO USER : You should not need to construct this class manually except in advanced cases. Typically constructed by GetListing().
        /// </summary>
        public FtpListItem()
        {
        }

        /// <summary>
        /// Constructor with mandatory arguments filled.
        /// 
        /// NOTE TO USER : You should not need to construct this class manually except in advanced cases. Typically constructed by GetListing().
        /// </summary>
        public FtpListItem(string record, string name, long size, bool isDir, ref DateTime lastModifiedTime)
        {
            this.Input = record;
            this.Name = name;
            this.Size = size;
            this.Type = isDir ? FtpFileSystemObjectType.Directory : FtpFileSystemObjectType.File;
            this.Modified = lastModifiedTime;
        }

        /// <summary>
        /// Gets the type of file system object.
        /// </summary>
        public FtpFileSystemObjectType Type;

        /// <summary>
        /// Gets the sub type of file system object.
        /// </summary>
        public FtpFileSystemObjectSubType SubType;

        /// <summary>
        /// Gets the full path name to the file or folder.
        /// </summary>
        public string FullName;

        private string m_name = null;

        /// <summary>
        /// Gets the name of the file or folder. Does not include the full path.
        /// </summary>
        public string Name
        {
            get
            {
                if (m_name == null && FullName != null)
                {
                    return FullName.GetFtpFileName();
                }
                return m_name;
            }
            set => m_name = value;
        }

        /// <summary>
        /// Gets the target a symbolic link points to.
        /// </summary>
        public string LinkTarget;

        /// <summary>
        /// Gets the number of links pointing to this file. Only supplied by Unix servers.
        /// </summary>
        public int LinkCount;

        /// <summary>
        /// Gets the object that the LinkTarget points to. This property is null unless you pass the
        /// <see cref="FtpListOption.DerefLinks"/> flag in which case GetListing() will try to resolve
        /// the target itself.
        /// </summary>
        public FtpListItem LinkObject;

        /// <summary>
        /// Gets the last write time of the object after timezone conversion (if enabled).
        /// </summary>
        public DateTime Modified = DateTime.MinValue;

        /// <summary>
        /// Gets the created date of the object after timezone conversion (if enabled).
        /// </summary>
        public DateTime Created = DateTime.MinValue;

        /// <summary>
        /// Gets the last write time of the object before any timezone conversion.
        /// </summary>
        public DateTime RawModified = DateTime.MinValue;

        /// <summary>
        /// Gets the created date of the object before any timezone conversion.
        /// </summary>
        public DateTime RawCreated = DateTime.MinValue;

        /// <summary>
        /// Gets the size of the object.
        /// </summary>
        public long Size = -1;

        /// <summary>
        /// Gets special UNIX permissions such as Sticky, SUID and SGID.
        /// </summary>
        public FtpSpecialPermissions SpecialPermissions = FtpSpecialPermissions.None;

        /// <summary>
        /// Gets the owner permissions.
        /// </summary>
        public FtpPermission OwnerPermissions = FtpPermission.None;

        /// <summary>
        /// Gets the group permissions.
        /// </summary>
        public FtpPermission GroupPermissions = FtpPermission.None;

        /// <summary>
        /// Gets the others permissions.
        /// </summary>
        public FtpPermission OthersPermissions = FtpPermission.None;

        /// <summary>
        /// Gets the raw string received for the file permissions.
        /// Use this if the other properties are blank/invalid.
        /// </summary>
        public string RawPermissions;

        /// <summary>
        /// Gets the file permissions in the CHMOD format.
        /// </summary>
        public int Chmod;

        /// <summary>
        /// Gets the raw string received for the file's GROUP permissions.
        /// Use this if the other properties are blank/invalid.
        /// </summary>
        public string RawGroup = null;

        /// <summary>
        /// Gets the raw string received for the file's OWNER permissions.
        /// Use this if the other properties are blank/invalid.
        /// </summary>
        public string RawOwner = null;


        /// <summary>
        /// Gets the input string that was parsed to generate the
        /// values in this object.
        /// </summary>
        public string Input;

        /// <summary>
        /// Returns a string representation of this object and its properties
        /// </summary>
        /// <returns>A string representing this object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            if (Type == FtpFileSystemObjectType.File)
            {
                sb.Append("FILE");
            }
            else if (Type == FtpFileSystemObjectType.Directory)
            {
                sb.Append("DIR ");
            }
            else if (Type == FtpFileSystemObjectType.Link)
            {
                sb.Append("LINK");
            }

            sb.Append("   ");
            sb.Append(Name);
            if (Type == FtpFileSystemObjectType.File)
            {
                sb.Append("      ");
                sb.Append("(");
                sb.Append(Size.FileSizeToString());
                sb.Append(")");
            }

            if (Created != DateTime.MinValue)
            {
                sb.Append("      ");
                sb.Append("Created : ");
                sb.Append(Created.ToString());
            }

            if (Modified != DateTime.MinValue)
            {
                sb.Append("      ");
                sb.Append("Modified : ");
                sb.Append(Modified.ToString());
            }

            return sb.ToString();
        }
    }
    /// <summary>
    /// Ftp档案
    /// </summary>
#if !NETFx
    [Serializable]
#endif
    public class FtpProfile
    {
        /// <summary>
        /// The host IP address or URL of the FTP server
        /// </summary>
        public string Host;

        /// <summary>
        /// The FTP username and password used to login
        /// </summary>
        public NetworkCredential Credentials;

        /// <summary>
        /// A working Encryption Mode found for this profile
        /// </summary>
        public FtpEncryptionMode Encryption = FtpEncryptionMode.None;

        /// <summary>
        /// A working Ssl Protocol setting found for this profile
        /// </summary>
        public SslProtocols Protocols = SslProtocols.None;

        /// <summary>
        /// A working Data Connection Type found for this profile
        /// </summary>
        public FtpDataConnectionType DataConnection = FtpDataConnectionType.PASV;

        /// <summary>
        /// A working Encoding setting found for this profile
        /// </summary>
        public Encoding Encoding;

        /// <summary>
        /// A working Timeout setting found for this profile, or 0 if default value should be used
        /// </summary>
        public int Timeout = 0;

        /// <summary>
        /// A working SocketPollInterval setting found for this profile, or 0 if default value should be used
        /// </summary>
        public int SocketPollInterval = 0;

        /// <summary>
        /// A working RetryAttempts setting found for this profile, or 0 if default value should be used
        /// </summary>
        public int RetryAttempts = 0;

        /// <summary>
        /// If the server surely supports the given encoding.
        /// </summary>
        public bool EncodingVerified = true;


        /// <summary>
        /// Generates valid C# code for this connection profile.
        /// </summary>
        /// <returns></returns>
        public string ToCode()
        {
            var sb = new StringBuilder();

            sb.AppendLine("// add this above your namespace declaration");
            sb.AppendLine("using System.Data.Dubber;");
            sb.AppendLine("using System.Text;");
            sb.AppendLine("using System.Net;");
            sb.AppendLine("using System.Security.Authentication;");
            sb.AppendLine();

            sb.AppendLine("// add this to create and configure the FTP client");
            sb.AppendLine("var client = new FtpClient();");


            // use LoadProfile rather than setting each property manually
            // this also allows us to use the high level properties like Timeout without
            // setting each Timeout individually
            sb.AppendLine("client.LoadProfile(new FtpProfile {");

            sb.AppendLine("	Host = " + Host.EscapeStringLiteral() + ",");
            sb.AppendLine("	Credentials = new NetworkCredential(" + Credentials.UserName.EscapeStringLiteral() + ", " + Credentials.Password.EscapeStringLiteral() + "),");
            sb.AppendLine("	Encryption = FtpEncryptionMode." + Encryption.ToString() + ",");
            sb.AppendLine("	Protocols = SslProtocols." + Protocols.ToString().Replace(", ", " | SslProtocols.") + ", ");
            sb.AppendLine("	DataConnection = FtpDataConnectionType." + DataConnection.ToString() + ",");


            // warn users if encoding is unverified
            if (!EncodingVerified)
            {
                sb.AppendLine("	// WARNING: This encoding cannot be verified because your server does not explicitly advertise it, but it is the best option.");
                sb.AppendLine("	// If you have issues with international characters in listings or file transfer, disable the following line.");
            }

            // Fix #468 - Invalid code generated: Encoding = System.Text.UTF8Encoding+UTF8EncodingSealed
            var encoding = Encoding.ToString();
            if (encoding.Contains("+"))
            {
                sb.AppendLine("	Encoding = " + encoding.Substring(0, encoding.IndexOf('+')) + ",");
            }
            else
            {
                sb.AppendLine("	Encoding = " + encoding + ",");
            }

            // Required for #533 - Auto detect Azure servers and use working settings
            if (Timeout != 0)
            {
                sb.AppendLine("	Timeout = " + Timeout + ",");
            }
            if (SocketPollInterval != 0)
            {
                sb.AppendLine("	SocketPollInterval = " + SocketPollInterval + ",");
            }
            if (RetryAttempts != 0)
            {
                sb.AppendLine("	RetryAttempts = " + RetryAttempts + ",");
            }

            sb.AppendLine("});");

            if (Encryption != FtpEncryptionMode.None)
            {
                sb.AppendLine("// if you want to accept any certificate then set ValidateAnyCertificate=true and delete the following event handler");
                sb.AppendLine("client.ValidateCertificate += new FtpSslValidation(delegate (FtpClient control, FtpSslValidationEventArgs e) {");
                sb.AppendLine("	// add your logic to test if the SSL certificate is valid (see the FAQ for examples)");
                sb.AppendLine("	e.Accept = true;");
                sb.AppendLine("});");
            }

            sb.AppendLine("client.Connect();");

            return sb.ToString();
        }
    }
    /// <summary>
    /// Class to report FTP file transfer progress during upload or download of files
    /// </summary>
    public class FtpProgress
    {
        /// <summary>
        /// A value between 0-100 indicating percentage complete, or -1 for indeterminate.
        /// Used to track the progress of an individual file transfer.
        /// </summary>
        public double Progress { get; set; }

        /// <summary>
        /// A value indicating how many bytes have been transferred.
        /// When unable to calculate percentage, having the partial byte count may help in providing some feedback.
        /// </summary>
        public long TransferredBytes { get; set; }

        /// <summary>
        /// A value representing the current Transfer Speed in Bytes per seconds.
        /// Used to track the progress of an individual file transfer.
        /// </summary>
        public double TransferSpeed { get; set; }

        /// <summary>
        /// A value representing the calculated 'Estimated time of arrival'.
        /// Used to track the progress of an individual file transfer.
        /// </summary>
        public TimeSpan ETA { get; set; }

        /// <summary>
        /// Stores the absolute remote path of the current file being transfered.
        /// </summary>
        public string RemotePath { get; set; }

        /// <summary>
        /// Stores the absolute local path of the current file being transfered.
        /// </summary>
        public string LocalPath { get; set; }

        /// <summary>
        /// Stores the index of the file in the listing.
        /// Only used when transferring multiple files or an entire directory.
        /// </summary>
        public int FileIndex { get; set; }

        /// <summary>
        /// Stores the total count of the files to be transfered.
        /// Only used when transferring multiple files or an entire directory.
        /// </summary>
        public int FileCount { get; set; }

        /// <summary>
        /// Create a new FtpProgress object for meta progress info.
        /// </summary>
        public FtpProgress(int fileCount, int fileIndex)
        {
            FileCount = fileCount;
            FileIndex = fileIndex;
        }

        /// <summary>
        /// Create a new FtpProgress object for individual file transfer progress.
        /// </summary>
        public FtpProgress(double progress, long bytesTransferred, double transferspeed, TimeSpan remainingtime, string localPath, string remotePath, FtpProgress metaProgress)
        {

            // progress of individual file transfer
            Progress = progress;
            TransferSpeed = transferspeed;
            ETA = remainingtime;
            LocalPath = localPath;
            RemotePath = remotePath;
            TransferredBytes = bytesTransferred;

            // progress of the entire task
            if (metaProgress != null)
            {
                FileCount = metaProgress.FileCount;
                FileIndex = metaProgress.FileIndex;
            }
        }

        /// <summary>
        /// Convert Transfer Speed (bytes per second) in human readable format
        /// </summary>
        public string TransferSpeedToString()
        {
            var value = TransferSpeed > 0 ? TransferSpeed / 1024 : 0; //get KB/s

            if (value < 1024)
            {
                return Math.Round(value, 2).ToString() + " KB/s";
            }
            else
            {
                value = value / 1024;
                return Math.Round(value, 2).ToString() + " MB/s";
            }
        }


        /// <summary>
        /// Create a new FtpProgress object for a file transfer and calculate the ETA, Percentage and Transfer Speed.
        /// </summary>
        public static FtpProgress Generate(long fileSize, long position, long bytesProcessed, TimeSpan elapsedtime, string localPath, string remotePath, FtpProgress metaProgress)
        {

            // default values to send
            double progressValue = -1;
            double transferSpeed = 0;
            var estimatedRemaingTime = TimeSpan.Zero;

            // catch any divide-by-zero errors
            try
            {

                // calculate raw transferSpeed (bytes per second)
                transferSpeed = bytesProcessed / elapsedtime.TotalSeconds;

                // If fileSize < 0 the below computations make no sense 
                if (fileSize > 0)
                {

                    // calculate % based on file length vs file offset
                    // send a value between 0-100 indicating percentage complete
                    progressValue = (double)position / (double)fileSize * 100;

                    //calculate remaining time			
                    estimatedRemaingTime = TimeSpan.FromSeconds((fileSize - position) / transferSpeed);
                }
            }
            catch (Exception)
            {
            }

            // suppress invalid values and send -1 instead
            if (double.IsNaN(progressValue) || double.IsInfinity(progressValue))
            {
                progressValue = -1;
            }
            if (double.IsNaN(transferSpeed) || double.IsInfinity(transferSpeed))
            {
                transferSpeed = 0;
            }

            var p = new FtpProgress(progressValue, position, transferSpeed, estimatedRemaingTime, localPath, remotePath, metaProgress);
            return p;
        }

    }
    /// <summary>
    /// Represents a reply to an event on the server
    /// </summary>
    public struct FtpReply
    {
        /// <summary>
        /// The type of response received from the last command executed
        /// </summary>
        public FtpResponseType Type
        {
            get
            {
                int code;

                if (Code != null && Code.Length > 0 &&
                    int.TryParse(Code[0].ToString(), out code))
                {
                    return (FtpResponseType)code;
                }

                return FtpResponseType.None;
            }
        }

        private string m_respCode;

        /// <summary>
        /// The status code of the response
        /// </summary>
        public string Code
        {
            get => m_respCode;
            set => m_respCode = value;
        }

        private string m_respMessage;

        /// <summary>
        /// The message, if any, that the server sent with the response
        /// </summary>
        public string Message
        {
            get => m_respMessage;
            set => m_respMessage = value;
        }

        private string m_infoMessages;

        /// <summary>
        /// Informational messages sent from the server
        /// </summary>
        public string InfoMessages
        {
            get => m_infoMessages;
            set => m_infoMessages = value;
        }

        /// <summary>
        /// General success or failure of the last command executed, by checking the FTP status code.
        /// 1xx, 2xx, 3xx indicate success and 4xx, 5xx are failures.
        /// </summary>
        public bool Success
        {
            get
            {
                if (Code != null && Code.Length > 0)
                {

                    // 1xx, 2xx, 3xx indicate success
                    // 4xx, 5xx are failures
                    if (Code[0] == '1' || Code[0] == '2' || Code[0] == '3')
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the error message including any informational output
        /// that was sent by the server. Sometimes the final response
        /// line doesn't contain anything informative as to what was going
        /// on with the server. Instead it may send information messages so
        /// in an effort to give as meaningful as a response as possible
        /// the informational messages will be included in the error.
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                var message = "";

                if (Success)
                {
                    return message;
                }

                if (InfoMessages != null && InfoMessages.Length > 0)
                {
                    foreach (var s in InfoMessages.Split('\n'))
                    {
                        var m = Regex.Replace(s, "^[0-9]{3}-", "");
                        message += m.Trim() + "; ";
                    }
                }

                message += Message;

                return message;
            }
        }
    }
    /// <summary>
    /// Stores the result of a file transfer when UploadDirectory or DownloadDirectory is used.
    /// </summary>
    public class FtpResult
    {

        /// <summary>
        /// Returns true if the file was downloaded, false if it was uploaded.
        /// </summary>
        public bool IsDownload;

        /// <summary>
        /// Gets the type of file system object.
        /// </summary>
        public FtpFileSystemObjectType Type;

        /// <summary>
        /// Gets the size of the file.
        /// </summary>
        public long Size;

        /// <summary>
        /// Gets the name and extension of the file.
        /// </summary>
        public string Name;

        /// <summary>
        /// Stores the absolute remote path of the current file being transfered.
        /// </summary>
        public string RemotePath { get; set; }

        /// <summary>
        /// Stores the absolute local path of the current file being transfered.
        /// </summary>
        public string LocalPath { get; set; }

        /// <summary>
        /// Gets the error that occurring during transferring this file, if any.
        /// </summary>
        public Exception Exception;

        /// <summary>
        /// Returns true if the file was downloaded/uploaded, or the file was already existing with the same file size.
        /// </summary>
        public bool IsSuccess;

        /// <summary>
        /// Was the file skipped?
        /// </summary>
        public bool IsSkipped;

        /// <summary>
        /// Was the file skipped due to failing the rule condition?
        /// </summary>
        public bool IsSkippedByRule;

        /// <summary>
        /// Was there an error during transfer? You can read the Exception property for more details.
        /// </summary>
        public bool IsFailed;

        /// <summary>
        /// Convert this result to a FTP list item.
        /// </summary>
        public FtpListItem ToListItem(bool useLocalPath)
        {
            return new FtpListItem
            {
                Type = Type,
                Size = Size,
                Name = Name,
                FullName = useLocalPath ? LocalPath : RemotePath,
            };
        }
        /// <summary>
        /// 转换成文本
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            // add type
            if (IsSkipped)
            {
                sb.Append("Skipped:     ");
            }
            else if (IsFailed)
            {
                sb.Append("Failed:      ");
            }
            else
            {
                if (IsDownload)
                {
                    sb.Append("Downloaded:  ");
                }
                else
                {
                    sb.Append("Uploaded:    ");
                }
            }

            // add path
            if (IsDownload)
            {
                sb.Append(RemotePath);
                sb.Append("  -->  ");
                sb.Append(LocalPath);
            }
            else
            {
                sb.Append(LocalPath);
                sb.Append("  -->  ");
                sb.Append(RemotePath);
            }

            // add error
            if (IsFailed && Exception != null && Exception.Message != null)
            {
                sb.Append("  [!]  ");
                sb.Append(Exception.Message);
            }

            return sb.ToString();
        }

    }
    internal class FtpSizeReply
    {

        public long FileSize { get; set; }

        public FtpReply Reply;

    }
    internal class IntRef
    {
        public int Value;
    }
    #region // Enums
    /// <summary>
    /// Server features
    /// </summary>
    [Flags]
    public enum FtpCapability : int
    {
        /// <summary>
        /// This server said it doesn't support anything!
        /// </summary>
        NONE = 1,

        /// <summary>
        /// Supports the MLST command
        /// </summary>
        MLSD = 2,

        /// <summary>
        /// Supports the SIZE command
        /// </summary>
        SIZE = 3,

        /// <summary>
        /// Supports the MDTM command
        /// </summary>
        MDTM = 4,

        /// <summary>
        /// Supports download/upload stream resumes
        /// </summary>
        REST = 5,

        /// <summary>
        /// Supports UTF8
        /// </summary>
        UTF8 = 6,

        /// <summary>
        /// PRET Command used by DrFTPD
        /// </summary>
        PRET = 7,

        /// <summary>
        /// Server supports the MFMT command for setting the
        /// modified date of an object on the server
        /// </summary>
        MFMT = 8,

        /// <summary>
        /// Server supports the MFCT command for setting the
        /// created date of an object on the server
        /// </summary>
        MFCT = 9,

        /// <summary>
        /// Server supports the MFF command for setting certain facts
        /// about file system objects. It typically allows you to modify
        /// the last modification time, creation time, UNIX group/owner/mode of a file.
        /// </summary>
        MFF = 10,

        /// <summary>
        /// Server supports the STAT command
        /// </summary>
        STAT = 11,

        /// <summary>
        /// Support for the HASH command
        /// </summary>
        HASH = 12,

        /// <summary>
        /// Support for the MD5 command
        /// </summary>
        MD5 = 13,

        /// <summary>
        /// Support for the XMD5 command
        /// </summary>
        XMD5 = 14,

        /// <summary>
        /// Support for the XCRC command
        /// </summary>
        XCRC = 15,

        /// <summary>
        /// Support for the XSHA1 command
        /// </summary>
        XSHA1 = 16,

        /// <summary>
        /// Support for the XSHA256 command
        /// </summary>
        XSHA256 = 17,

        /// <summary>
        /// Support for the XSHA512 command
        /// </summary>
        XSHA512 = 18,

        /// <summary>
        /// Support for the EPSV file-transfer command
        /// </summary>
        EPSV = 19,

        /// <summary>
        /// Support for the CPSV command
        /// </summary>
        CPSV = 20,

        /// <summary>
        /// Support for the NOOP command
        /// </summary>
        NOOP = 21,

        /// <summary>
        /// Support for the CLNT command
        /// </summary>
        CLNT = 22,

        /// <summary>
        /// Support for the SSCN command
        /// </summary>
        SSCN = 23,

        /// <summary>
        /// Support for the SITE MKDIR (make directory) server-specific command for ProFTPd
        /// </summary>
        SITE_MKDIR = 24,

        /// <summary>
        /// Support for the SITE RMDIR (remove directory) server-specific command for ProFTPd
        /// </summary>
        SITE_RMDIR = 25,

        /// <summary>
        /// Support for the SITE UTIME server-specific command for ProFTPd
        /// </summary>
        SITE_UTIME = 26,

        /// <summary>
        /// Support for the SITE SYMLINK server-specific command for ProFTPd
        /// </summary>
        SITE_SYMLINK = 27,

        /// <summary>
        /// Support for the AVBL (get available space) server-specific command for Serv-U
        /// </summary>
        AVBL = 28,

        /// <summary>
        /// Support for the THMB (get image thumbnail) server-specific command for Serv-U
        /// </summary>
        THMB = 29,

        /// <summary>
        /// Support for the RMDA (remove directory) server-specific command for Serv-U
        /// </summary>
        RMDA = 30,

        /// <summary>
        /// Support for the DSIZ (get directory size) server-specific command for Serv-U
        /// </summary>
        DSIZ = 31,

        /// <summary>
        /// Support for the HOST (get host) server-specific command for Serv-U
        /// </summary>
        HOST = 32,

        /// <summary>
        /// Support for the CCC (Clear Command Channel) command, which makes a secure FTP channel revert back to plain text.
        /// </summary>
        CCC = 33,

        /// <summary>
        /// Support for the MODE Z (compression enabled) command, which says that the server supports ZLIB compression for all transfers
        /// </summary>
        MODE_Z = 34,

        /// <summary>
        /// Support for the LANG (language negotiation) command.
        /// </summary>
        LANG = 35,

        /// <summary>
        /// Support for the MMD5 (multiple MD5 hash) command.
        /// </summary>
        MMD5 = 36,

    }
    /// <summary>
    /// Flags that control how file comparison is performed. If you are unsure what to use, set it to Auto.
    /// </summary>
    [Flags]
    public enum FtpCompareOption
    {

        /// <summary>
        /// Compares the file size and the checksum of the file (using the first supported hash algorithm).
        /// The local and remote file sizes and checksums should exactly match for the file to be considered equal.
        /// </summary>
        Auto = 0,

        /// <summary>
        /// Compares the file size.
        /// Both file sizes should exactly match for the file to be considered equal.
        /// </summary>
        Size = 1,

        /// <summary>
        /// Compares the date modified of the file.
        /// Both dates should exactly match for the file to be considered equal.
        /// </summary>
        DateModified = 2,

        /// <summary>
        /// Compares the checksum or hash of the file using the first supported hash algorithm.
        /// Both checksums should exactly match for the file to be considered equal.
        /// </summary>
        Checksum = 4,

    }
    /// <summary>
    /// The result of a file comparison operation.
    /// </summary>
    public enum FtpCompareResult
    {

        /// <summary>
        /// Success. Local and remote files are exactly equal.
        /// </summary>
        Equal = 1,

        /// <summary>
        /// Failure. Local and remote files do not match.
        /// </summary>
        NotEqual = 2,

        /// <summary>
        /// Failure. Either the local or remote file does not exist.
        /// </summary>
        FileNotExisting = 3,

        /// <summary>
        /// Failure. Checksum verification is enabled and your server does not support any hash algorithm.
        /// </summary>
        ChecksumNotSupported = 4,

    }
    /// <summary>
    /// Data connection type
    /// </summary>
    public enum FtpDataConnectionType
    {
        /// <summary>
        /// This type of data connection attempts to use the EPSV command
        /// and if the server does not support EPSV it falls back to the
        /// PASV command before giving up unless you are connected via IPv6
        /// in which case the PASV command is not supported.
        /// </summary>
        AutoPassive,

        /// <summary>
        /// Passive data connection. EPSV is a better
        /// option if it's supported. Passive connections
        /// connect to the IP address dictated by the server
        /// which may or may not be accessible by the client
        /// for example a server behind a NAT device may
        /// give an IP address on its local network that
        /// is inaccessible to the client. Please note that IPv6
        /// does not support this type data connection. If you
        /// ask for PASV and are connected via IPv6 EPSV will
        /// automatically be used in its place.
        /// </summary>
        PASV,

        /// <summary>
        /// Same as PASV except the host supplied by the server is ignored
        /// and the data connection is made to the same address that the control
        /// connection is connected to. This is useful in scenarios where the
        /// server supplies a private/non-routable network address in the
        /// PASV response. It's functionally identical to EPSV except some
        /// servers may not implement the EPSV command. Please note that IPv6
        /// does not support this type data connection. If you
        /// ask for PASV and are connected via IPv6 EPSV will
        /// automatically be used in its place.
        /// </summary>
        PASVEX,

        /// <summary>
        /// Extended passive data connection, recommended. Works
        /// the same as a PASV connection except the server
        /// does not dictate an IP address to connect to, instead
        /// the passive connection goes to the same address used
        /// in the control connection. This type of data connection
        /// supports IPv4 and IPv6.
        /// </summary>
        EPSV,

        /// <summary>
        /// This type of data connection attempts to use the EPRT command
        /// and if the server does not support EPRT it falls back to the
        /// PORT command before giving up unless you are connected via IPv6
        /// in which case the PORT command is not supported.
        /// </summary>
        AutoActive,

        /// <summary>
        /// Active data connection, not recommended unless
        /// you have a specific reason for using this type.
        /// Creates a listening socket on the client which
        /// requires firewall exceptions on the client system
        /// as well as client network when connecting to a
        /// server outside of the client's network. In addition
        /// the IP address of the interface used to connect to the
        /// server is the address the server is told to connect to
        /// which, if behind a NAT device, may be inaccessible to
        /// the server. This type of data connection is not supported
        /// by IPv6. If you specify PORT and are connected via IPv6
        /// EPRT will automatically be used instead.
        /// </summary>
        PORT,

        /// <summary>
        /// Extended active data connection, not recommended
        /// unless you have a specific reason for using this
        /// type. Creates a listening socket on the client
        /// which requires firewall exceptions on the client
        /// as well as client network when connecting to a 
        /// server outside of the client's network. The server
        /// connects to the IP address it sees the client coming
        /// from. This type of data connection supports IPv4 and IPv6.
        /// </summary>
        EPRT
    }
    /// <summary>
    /// Type of data transfer to do
    /// </summary>
    public enum FtpDataType
    {
        /// <summary>
        /// ASCII transfer
        /// </summary>
        ASCII,

        /// <summary>
        /// Binary transfer
        /// </summary>
        Binary
    }
    /// <summary>
    /// Controls how timestamps returned by the server are converted.
    /// </summary>
    public enum FtpDate
    {

        /// <summary>
        /// Returns the server timestamps in Server Time. No timezone conversion is performed.
        /// </summary>
        ServerTime = 0,

        /// <summary>
        /// Returns the server timestamps in Local Time.
        /// Ensure that the TimeZone property is correctly set to the server's timezone.
        /// If you are on .NET Core/.NET Standard, you need to set the LocalTimeZone property for this to work.
        /// </summary>
        LocalTime = 1,

        /// <summary>
        /// Returns the server timestamps in UTC (Coordinated Universal Time).
        /// Ensure that the TimeZone property is correctly set to the server's timezone.
        /// </summary>
        UTC = 2,

    }
    /// <summary>
    /// Defines the type of encryption to use
    /// </summary>
    public enum FtpEncryptionMode
    {
        /// <summary>
        /// Plain text.
        /// </summary>
        None,

        /// <summary>
        /// FTPS encryption is used from the start of the connection, port 990.
        /// </summary>
        Implicit,

        /// <summary>
        /// Connection starts in plain text and FTPS encryption is enabled
        /// with the AUTH command immediately after the server greeting.
        /// </summary>
        Explicit,

        /// <summary>
        /// FTPS encryption is used if supported by the server, otherwise it falls back to plaintext FTP communication.
        /// </summary>
        Auto
    }
    /// <summary>
    /// Defines how multi-file processes should handle a processing error.
    /// </summary>
    /// <remarks><see cref="FtpError.Stop"/> &amp; <see cref="FtpError.Throw"/> Cannot Be Combined</remarks>
    [Flags]
    public enum FtpError
    {
        /// <summary>
        /// No action is taken upon errors.  The method absorbs the error and continues.
        /// </summary>
        None = 0,

        /// <summary>
        /// If any files have completed successfully (or failed after a partial download/upload) then should be deleted.  
        /// This will simulate an all-or-nothing transaction downloading or uploading multiple files.  If this option is not
        /// combined with <see cref="FtpError.Stop"/> or <see cref="FtpError.Throw"/> then the method will
        /// continue to process all items whether if they are successful or not and then delete everything if a failure was
        /// encountered at any point.
        /// </summary>
        DeleteProcessed = 1,

        /// <summary>
        /// The method should stop processing any additional files and immediately return upon encountering an error.
        /// Cannot be combined with <see cref="FtpError.Throw"/>
        /// </summary>
        Stop = 2,

        /// <summary>
        /// The method should stop processing any additional files and immediately throw the current error.
        /// Cannot be combined with <see cref="FtpError.Stop"/>
        /// </summary>
        Throw = 4,
    }
    /// <summary>
    /// Type of file system of object
    /// </summary>
    public enum FtpFileSystemObjectSubType
    {

        /// <summary>
        /// The default subtype.
        /// </summary>
        Unknown,

        /// <summary>
        /// A sub directory within the listed directory.
        /// (Only set when machine listing is available and type is 'dir')
        /// </summary>
        SubDirectory,

        /// <summary>
        /// The self directory.
        /// (Only set when machine listing is available and type is 'cdir')
        /// </summary>
        SelfDirectory,

        /// <summary>
        /// The parent directory.
        /// (Only set when machine listing is available and type is 'pdir')
        /// </summary>
        ParentDirectory,

    }
    /// <summary>
    /// Type of file system of object
    /// </summary>
    public enum FtpFileSystemObjectType
    {
        /// <summary>
        /// A file
        /// </summary>
        File,

        /// <summary>
        /// A directory
        /// </summary>
        Directory,

        /// <summary>
        /// A symbolic link
        /// </summary>
        Link
    }
    /// <summary>
    /// Determines how we handle downloading and uploading folders
    /// </summary>
    public enum FtpFolderSyncMode
    {

        /// <summary>
        /// Dangerous but useful method!
        /// Uploads/downloads all the missing files to update the server/local filesystem.
        /// Deletes the extra files to ensure that the target is an exact mirror of the source.
        /// </summary>
        Mirror,

        /// <summary>
        /// Safe method!
        /// Uploads/downloads all the missing files to update the server/local filesystem.
        /// </summary>
        Update,

    }
    /// <summary>
    /// Different types of hashing algorithms for computing checksums.
    /// </summary>
    [Flags]
    public enum FtpHashAlgorithm : int
    {

        /// <summary>
        /// Automatic algorithm, or hashing not supported.
        /// </summary>
        NONE = 0,

        /// <summary>
        /// SHA-1 algorithm
        /// </summary>
        SHA1 = 1,

        /// <summary>
        /// SHA-256 algorithm
        /// </summary>
        SHA256 = 2,

        /// <summary>
        /// SHA-512 algorithm
        /// </summary>
        SHA512 = 4,

        /// <summary>
        /// MD5 algorithm
        /// </summary>
        MD5 = 8,

        /// <summary>
        /// CRC algorithm
        /// </summary>
        CRC = 16
    }
    /// <summary>
    /// IP Versions to allow when connecting
    /// to a server.
    /// </summary>
    [Flags]
    public enum FtpIpVersion : int
    {
        /// <summary>
        /// Internet Protocol Version 4
        /// </summary>
        IPv4 = 1,

        /// <summary>
        /// Internet Protocol Version 6
        /// </summary>
        IPv6 = 2,

        /// <summary>
        /// Allow any supported version
        /// </summary>
        ANY = IPv4 | IPv6
    }
    /// <summary>
    /// Flags that can control how a file listing is performed. If you are unsure what to use, set it to Auto.
    /// </summary>
    [Flags]
    public enum FtpListOption
    {
        /// <summary>
        /// Tries machine listings (MDTM command) if supported,
        /// and if not then falls back to OS-specific listings (LIST command)
        /// </summary>
        Auto = 0,

        /// <summary>
        /// Load the modify date using MDTM when it could not
        /// be parsed from the server listing. This only pertains
        /// to servers that do not implement the MLSD command.
        /// </summary>
        Modify = 1,

        /// <summary>
        /// Load the file size using the SIZE command when it
        /// could not be parsed from the server listing. This
        /// only pertains to servers that do not support the
        /// MLSD command.
        /// </summary>
        Size = 2,

        /// <summary>
        /// Combines the Modify and Size flags
        /// </summary>
        SizeModify = Modify | Size,

        /// <summary>
        /// Show hidden/dot files. This only pertains to servers
        /// that do not support the MLSD command. This option
        /// makes use the non standard -a parameter to LIST to
        /// tell the server to show hidden files. Since it's a
        /// non-standard option it may not always work. MLSD listings
        /// have no such option and whether or not a hidden file is
        /// shown is at the discretion of the server.
        /// </summary>
        AllFiles = 4,

        /// <summary>
        /// Force the use of OS-specific listings (LIST command) even if
        /// machine listings (MLSD command) are supported by the server
        /// </summary>
        ForceList = 8,

        /// <summary>
        /// Use the NLST command instead of LIST for a reliable file listing
        /// </summary>
        NameList = 16,

        /// <summary>
        /// Force the use of the NLST command (the slowest mode) even if machine listings
        /// and OS-specific listings are supported by the server
        /// </summary>
        ForceNameList = ForceList | NameList,

        /// <summary>
        /// Try to dereference symbolic links, and stored the linked file/directory in FtpListItem.LinkObject
        /// </summary>
        DerefLinks = 32,

        /// <summary>
        /// Sets the ForceList flag and uses `LS' instead of `LIST' as the
        /// command for getting a directory listing. This option overrides
        /// ForceNameList and ignores the AllFiles flag.
        /// </summary>
        UseLS = 64 | ForceList,

        /// <summary>
        /// Gets files within subdirectories as well. Adds the -r option to the LIST command.
        /// Some servers may not support this feature.
        /// </summary>
        Recursive = 128,

        /// <summary>
        /// Do not retrieve path when no path is supplied to GetListing(),
        /// instead just execute LIST with no path argument.
        /// </summary>
        NoPath = 256,

        /// <summary>
        /// Include two extra items into the listing, for the current directory (".")
        /// and the parent directory (".."). Meaningless unless you want these two
        /// items for some reason.
        /// </summary>
        IncludeSelfAndParent = 512,

        /// <summary>
        /// Force the use of STAT command for getting file listings
        /// </summary>
        UseStat = 1024
    }
    /// <summary>
    /// Determines how we handle partially downloaded files
    /// </summary>
    public enum FtpLocalExists
    {
        /// <summary>
        /// Restart the download of a file if it is partially downloaded.
        /// Overwrites the file if it exists on disk.
        /// </summary>
        Overwrite,

        /// <summary>
        /// Resume the download of a file if it is partially downloaded.
        /// Appends to the file if it exists, by checking the length and adding the missing data.
        /// If the file doesn't exist on disk, a new file is created.
        /// </summary>
        Resume,

        /// <summary>
        /// Blindly skip downloading the file if it exists on disk, without any more checks.
        /// This is only included to be compatible with legacy behaviour.
        /// </summary>
        Skip,

        /// <summary>
        /// Append is now renamed to Resume.
        /// </summary>
        [ObsoleteAttribute("Append is now renamed to Resume to better reflect its behaviour.", true)]
        Append,
    }
    /// <summary>
    /// Defines the operating system of the FTP server.
    /// </summary>
    public enum FtpOperatingSystem
    {
        /// <summary>
        /// Unknown operating system
        /// </summary>
        Unknown,

        /// <summary>
        /// Definitely Windows or Windows Server
        /// </summary>
        Windows,

        /// <summary>
        /// Definitely Unix or AIX-based server
        /// </summary>
        Unix,

        /// <summary>
        /// Definitely VMS or OpenVMS server
        /// </summary>
        VMS,

        /// <summary>
        /// Definitely IBM OS/400 server
        /// </summary>
        IBMOS400,

        /// <summary>
        /// Definitely IBM z/OS server
        /// </summary>
        IBMzOS,

        /// <summary>
        /// Definitely SUN OS/Solaris server
        /// </summary>
        SunOS,
    }
    /// <summary>
    /// 操作比较符
    /// </summary>
    public enum FtpOperator
    {

        /// <summary>
        /// If the value is exactly equal to X
        /// </summary>
        Equals,
        /// <summary>
        /// If the value is anything except for X
        /// </summary>
        NotEquals,
        /// <summary>
        /// If the value is less than X
        /// </summary>
        LessThan,
        /// <summary>
        /// If the value is less than or equal to X
        /// </summary>
        LessThanOrEquals,
        /// <summary>
        /// If the value is more than X
        /// </summary>
        MoreThan,
        /// <summary>
        /// If the value is more than or equal to X
        /// </summary>
        MoreThanOrEquals,
        /// <summary>
        /// If the value is between the range of X and Y
        /// </summary>
        BetweenRange,
        /// <summary>
        /// If the value is outside the range of X and Y
        /// </summary>
        OutsideRange,

    }
    /// <summary>
    /// The type of response the server responded with
    /// </summary>
    public enum FtpParser : int
    {
        /// <summary>
        /// Use the custom parser that you have set on the FtpClient object (ListingCustomParser property)
        /// </summary>
        Custom = -1,

        /// <summary>
        /// Automatically detect the file listing parser to use based on the FTP server (SYST command).
        /// </summary>
        Auto = 0,

        /// <summary>
        /// Machine listing parser, works on any FTP server supporting the MLST/MLSD commands.
        /// </summary>
        Machine = 1,

        /// <summary>
        /// File listing parser for Windows/IIS.
        /// </summary>
        Windows = 2,

        /// <summary>
        /// File listing parser for Unix.
        /// </summary>
        Unix = 3,

        /// <summary>
        /// Alternate parser for Unix. Use this if the default one does not work.
        /// </summary>
        UnixAlt = 4,

        /// <summary>
        /// File listing parser for Vax/VMS/OpenVMS.
        /// </summary>
        VMS = 5,

        /// <summary>
        /// File listing parser for IBM z/OS
        /// </summary>
        IBMzOS = 6,

        /// <summary>
        /// File listing parser for IBM OS/400.
        /// </summary>
        IBMOS400 = 7,

        /// <summary>
        /// File listing parser for Tandem/Nonstop Guardian OS.
        /// </summary>
        NonStop = 8
    }
    /// <summary>
    /// Types of file permissions
    /// </summary>
    [Flags]
    public enum FtpPermission : uint
    {
        /// <summary>
        /// No access
        /// </summary>
        None = 0,

        /// <summary>
        /// Executable
        /// </summary>
        Execute = 1,

        /// <summary>
        /// Writable
        /// </summary>
        Write = 2,

        /// <summary>
        /// Readable
        /// </summary>
        Read = 4
    }
    /// <summary>
    /// Defines the behavior for uploading/downloading files that already exist
    /// </summary>
    public enum FtpRemoteExists
    {

        /// <summary>
        /// Do not check if the file exists. A bit faster than the other options.
        /// Only use this if you are SURE that the file does not exist on the server.
        /// Otherwise it can cause the UploadFile method to hang due to filesize mismatch.
        /// </summary>
        NoCheck,

        /// <summary>
        /// Resume uploading by appending to the remote file, but don't check if it exists and add missing data.
        /// This might be required if you don't have permissions on the server to list files in the folder.
        /// Only use this if you are SURE that the file does not exist on the server otherwise it can cause the UploadFile method to hang due to filesize mismatch.
        /// </summary>
        ResumeNoCheck,

        /// <summary>
        /// Append the local file to the end of the remote file, but don't check if it exists and add missing data.
        /// This might be required if you don't have permissions on the server to list files in the folder.
        /// Only use this if you are SURE that the file does not exist on the server otherwise it can cause the UploadFile method to hang due to filesize mismatch.
        /// </summary>
        AddToEndNoCheck,

        /// <summary>
        /// Skip the file if it exists, without any more checks.
        /// </summary>
        Skip,

        /// <summary>
        /// Overwrite the file if it exists.
        /// </summary>
        Overwrite,

        /// <summary>
        /// Resume uploading by appending to the remote file if it exists.
        /// It works by checking the remote file length and adding the missing data.
        /// </summary>
        Resume,

        /// <summary>
        /// Append the local file to the end of the remote file.
        /// </summary>
        AddToEnd,

        /// <summary>
        /// Append is now renamed to Resume. Alternatively you can use AddToEnd.
        /// </summary>
        [ObsoleteAttribute("Append is now renamed to Resume to better reflect its behaviour. If you instead want to append the local file to the END of the remote file without resuming, then use AddToEnd.", true)]
        Append,
    }
    /// <summary>
    /// The type of response the server responded with
    /// </summary>
    public enum FtpResponseType : int
    {
        /// <summary>
        /// No response
        /// </summary>
        None = 0,

        /// <summary>
        /// Success
        /// </summary>
        PositivePreliminary = 1,

        /// <summary>
        /// Success
        /// </summary>
        PositiveCompletion = 2,

        /// <summary>
        /// Success
        /// </summary>
        PositiveIntermediate = 3,

        /// <summary>
        /// Temporary failure
        /// </summary>
        TransientNegativeCompletion = 4,

        /// <summary>
        /// Permanent failure
        /// </summary>
        PermanentNegativeCompletion = 5
    }
    /// <summary>
    /// Determines how SSL Buffering is handled
    /// </summary>
    public enum FtpsBuffering
    {
        /// <summary>
        /// Enables buffering in all cases except when using FTP proxies.
        /// </summary>
        Auto,

        /// <summary>
        /// Always disables SSL Buffering to reduce FTPS connectivity issues.
        /// </summary>
        Off,

        /// <summary>
        /// Always enables SSL Buffering to massively speed up FTPS operations.
        /// </summary>
        On
    }
    /// <summary>
    /// Defines the type of the FTP server software.
    /// Add constants here as you add detection scripts for individual server types.
    /// </summary>
    public enum FtpServer
    {
        /// <summary>
        /// Unknown FTP server software
        /// </summary>
        Unknown,

        /// <summary>
        /// Definitely PureFTPd server
        /// </summary>
        PureFTPd,

        /// <summary>
        /// Definitely VsFTPd server
        /// </summary>
        VsFTPd,

        /// <summary>
        /// Definitely ProFTPD server
        /// </summary>
        ProFTPD,

        /// <summary>
        /// Definitely FileZilla server
        /// </summary>
        FileZilla,

        /// <summary>
        /// Definitely OpenVMS server
        /// </summary>
        OpenVMS,

        /// <summary>
        /// Definitely Windows CE FTP server
        /// </summary>
        WindowsCE,

        /// <summary>
        /// Definitely WuFTPd server
        /// </summary>
        WuFTPd,

        /// <summary>
        /// Definitely GlobalScape EFT server
        /// </summary>
        GlobalScapeEFT,

        /// <summary>
        /// Definitely HP NonStop/Tandem server
        /// </summary>
        NonStopTandem,

        /// <summary>
        /// Definitely Serv-U server
        /// </summary>
        ServU,

        /// <summary>
        /// Definitely Cerberus FTP server
        /// </summary>
        Cerberus,

        /// <summary>
        /// Definitely Windows Server/IIS FTP server
        /// </summary>
        WindowsServerIIS,

        /// <summary>
        /// Definitely CrushFTP server
        /// </summary>
        CrushFTP,

        /// <summary>
        /// Definitely glFTPd server
        /// </summary>
        glFTPd,

        /// <summary>
        /// Definitely Homegate FTP server
        /// </summary>
        HomegateFTP,

        /// <summary>
        /// Definitely BFTPd server
        /// </summary>
        BFTPd,

        /// <summary>
        /// Definitely FTP2S3 gateway server
        /// </summary>
        FTP2S3Gateway,

        /// <summary>
        /// Definitely XLight FTP server
        /// </summary>
        XLight,

        /// <summary>
        /// Definitely Sun OS Solaris FTP server
        /// </summary>
        SolarisFTP,

        /// <summary>
        /// Definitely IBM z/OS FTP server
        /// </summary>
        IBMzOSFTP,

        /// <summary>
        /// Definitely IBM OS/400 FTP server
        /// </summary>
        IBMOS400FTP,

        /// <summary>
        /// Definitely FritzBox FTP server
        /// </summary>
        FritzBox,

        /// <summary>
        /// Definitely WS_FTP server
        /// </summary>
        WSFTPServer,

        /// <summary>
        /// Definitely PyFtpdLib server
        /// </summary>
        PyFtpdLib,

        /// <summary>
        /// Definitely Titan FTP server
        /// </summary>
        TitanFTP,
    }
    /// <summary>
    /// Types of special UNIX permissions
    /// </summary>
    [Flags]
    public enum FtpSpecialPermissions : int
    {
        /// <summary>
        /// No special permissions are set
        /// </summary>
        None = 0,

        /// <summary>
        /// Sticky bit is set
        /// </summary>
        Sticky = 1,

        /// <summary>
        /// SGID bit is set
        /// </summary>
        SetGroupID = 2,

        /// <summary>
        /// SUID bit is set
        /// </summary>
        SetUserID = 4
    }
    /// <summary>
    /// The result of an upload or download operation
    /// </summary>
    public enum FtpStatus
    {

        /// <summary>
        /// The upload or download failed with an error transferring, or the source file did not exist
        /// </summary>
        Failed = 0,

        /// <summary>
        /// The upload or download completed successfully
        /// </summary>
        Success = 1,

        /// <summary>
        /// The upload or download was skipped because the file already existed on the target
        /// </summary>
        Skipped = 2

    }
    /// <summary>
    /// Defines the level of the tracing message.  Depending on the framework version this is translated
    /// to an equivalent logging level in System.Diagnostices (if available)
    /// </summary>
    public enum FtpTraceLevel
    {
        /// <summary>
        /// Used for logging Debug or Verbose level messages
        /// </summary>
        Verbose,

        /// <summary>
        /// Used for logging Informational messages
        /// </summary>
        Info,

        /// <summary>
        /// Used for logging non-fatal or ignorable error messages
        /// </summary>
        Warn,

        /// <summary>
        /// Used for logging Error messages that may need investigation 
        /// </summary>
        Error
    }
    /// <summary>
    /// Defines if additional verification and actions upon failure that 
    /// should be performed when uploading/downloading files using the high-level APIs.  Ignored if the 
    /// FTP server does not support any hashing algorithms.
    /// </summary>
    [Flags]
    public enum FtpVerify
    {
        /// <summary>
        /// No verification of the file is performed
        /// </summary>
        None = 0,

        /// <summary>
        /// The checksum of the file is verified, if supported by the server.
        /// If the checksum comparison fails then we retry the download/upload
        /// a specified amount of times before giving up. (See <see cref="FtpClient.RetryAttempts"/>)
        /// </summary>
        Retry = 1,

        /// <summary>
        /// The checksum of the file is verified, if supported by the server.
        /// If the checksum comparison fails then the failed file will be deleted.
        /// If combined with <see cref="FtpVerify.Retry"/>, then
        /// the deletion will occur if it fails upon the final retry.
        /// </summary>
        Delete = 2,

        /// <summary>
        /// The checksum of the file is verified, if supported by the server.
        /// If the checksum comparison fails then an exception will be thrown.
        /// If combined with <see cref="FtpVerify.Retry"/>, then the throw will
        /// occur upon the failure of the final retry, and/or if combined with <see cref="FtpVerify.Delete"/>
        /// the method will throw after the deletion is processed.
        /// </summary>
        Throw = 4,

        /// <summary>
        /// The checksum of the file is verified, if supported by the server.
        /// If the checksum comparison fails then the method returns false and no other action is taken.
        /// </summary>
        OnlyChecksum = 8,
    }
    /// <summary>
    /// Flags that can control how a file listing is performed. If you are unsure what to use, set it to Auto.
    /// </summary>
    [Flags]
    public enum FtpZOSListRealm
    {
        /// <summary>
        /// Not z/OS Server
        /// </summary>
        Invalid = -1,

        /// <summary>
        /// HFS / USS 
        /// </summary>
        Unix = 0,

        /// <summary>
        /// z/OS classic dataset
        /// </summary>
        Dataset = 1,

        /// <summary>
        /// Partitioned dataset member, RECFM != U
        /// </summary>
        Member = 2,

        /// <summary>
        /// Partitioned dataset member, RECFM = U
        /// </summary>
        MemberU = 3
    }
    #region // Socks
    /// <summary>
    /// 认证类型
    /// </summary>
    public enum SocksAuthType
    {
        /// <summary>
        /// 无
        /// </summary>
        NoAuthRequired = 0x00,
        /// <summary>
        /// GSSAPI
        /// </summary>
        GSSAPI = 0x01,
        /// <summary>
        /// 用户名密码
        /// </summary>
        UsernamePassword = 0x02,
        /// <summary>
        /// 不可访问的方法
        /// </summary>
        NoAcceptableMethods = 0xFF
    }
    /// <summary>
    /// 握手回复
    /// </summary>
    public enum SocksReply
    {
        /// <summary>
        /// 成功
        /// </summary>
        Succeeded = 0x00,
        /// <summary>
        /// 普通错误
        /// </summary>
        GeneralSOCKSServerFailure = 0x01,
        /// <summary>
        /// 不允许的权限
        /// </summary>
        NotAllowedByRuleset = 0x02,
        /// <summary>
        /// 网络不可达
        /// </summary>
        NetworkUnreachable = 0x03,
        /// <summary>
        /// 网站无法访问
        /// </summary>
        HostUnreachable = 0x04,
        /// <summary>
        /// 连接拒绝
        /// </summary>
        ConnectionRefused = 0x05,
        /// <summary>
        /// TTL超时
        /// </summary>
        TTLExpired = 0x06,
        /// <summary>
        /// 命令不支持
        /// </summary>
        CommandNotSupported = 0x07,
        /// <summary>
        /// 地址类型不支持
        /// </summary>
        AddressTypeNotSupported = 0x08
    }

    internal enum SocksRequestAddressType
    {
        Unknown = 0x00,
        IPv4 = 0x01,
        FQDN = 0x03,
        IPv6 = 0x04
    }

    internal enum SocksRequestCommand : byte
    {
        Connect = 0x01,
        Bind = 0x02,
        UdpAssociate = 0x03
    }

    internal enum SocksVersion
    {
        Four = 0x04,
        Five = 0x05
    }
    #endregion
    #endregion // Enums
    #region // Exceptions

    /// <summary>
    /// Exception triggered on FTP authentication failures
    /// </summary>
#if !NETFx
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

#if !NETFx
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
#if !NETFx
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

#if !NETFx
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
#if !NETFx
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

#if !NETFx
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
#if !NETFx
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

#if !NETFx
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
#if !NETFx
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

#if !NETFx
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
#if !NETFx
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

#if !NETFx
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
#if !NETFx
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

#if !NETFx
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
#if !NETFx
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

#if !NETFx
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
#if NETFx
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
    /// <summary>
    /// 握手代理异常
    /// </summary>
    public class SocksProxyException : Exception
    {
        /// <summary>
        /// 构造
        /// </summary>
        public SocksProxyException()
        {
        }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="message"></param>
        public SocksProxyException(string message)
            : base(message)
        {
        }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public SocksProxyException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
    #endregion // Exceptions
}
