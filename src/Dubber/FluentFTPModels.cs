using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Data.Dubber
{
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
#if CORE
						hashAlg = SHA1.Create();
#else
                        hashAlg = new SHA1CryptoServiceProvider();
#endif
                        break;

#if !NET20
                    case FtpHashAlgorithm.SHA256:
#if CORE
						hashAlg = SHA256.Create();
#else
                        hashAlg = new SHA256CryptoServiceProvider();
#endif
                        break;

                    case FtpHashAlgorithm.SHA512:
#if CORE
						hashAlg = SHA512.Create();
#else
                        hashAlg = new SHA512CryptoServiceProvider();
#endif
                        break;

#endif
                    case FtpHashAlgorithm.MD5:
#if CORE
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
#if !CORE
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
}
