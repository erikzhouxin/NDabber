using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SysSslProtocols = System.Security.Authentication.SslProtocols;

namespace System.Data.Dubber
{
    #region // Rules
    /// <summary>
    /// Only accept files that have the given extension, or exclude files of a given extension.
    /// </summary>
    public class FtpFileExtensionRule : FtpRule
    {

        /// <summary>
        /// If true, only files of the given extension are uploaded or downloaded. If false, files of the given extension are excluded.
        /// </summary>
        public bool Whitelist;

        /// <summary>
        /// The extensions to match
        /// </summary>
        public IList<string> Exts;

        /// <summary>
        /// Only accept files that have the given extension, or exclude files of a given extension.
        /// </summary>
        /// <param name="whitelist">If true, only files of the given extension are uploaded or downloaded. If false, files of the given extension are excluded.</param>
        /// <param name="exts">The extensions to match</param>
        public FtpFileExtensionRule(bool whitelist, IList<string> exts)
        {
            this.Whitelist = whitelist;
            this.Exts = exts;
        }

        /// <summary>
        /// Checks if the files has the given extension, or exclude files of the given extension.
        /// </summary>
        public override bool IsAllowed(FtpListItem item)
        {
            if (item.Type == FtpFileSystemObjectType.File)
            {
                var ext = Path.GetExtension(item.Name).Replace(".", "").ToLower();
                if (Whitelist)
                {

                    // whitelist
                    if (ext.IsBlank())
                    {
                        return false;
                    }
                    else
                    {
                        return Exts.Contains(ext);
                    }
                }
                else
                {

                    // blacklist
                    if (ext.IsBlank())
                    {
                        return true;
                    }
                    else
                    {
                        return !Exts.Contains(ext);
                    }
                }
            }
            else
            {
                return true;
            }
        }

    }
    /// <summary>
    /// Only accept files whose names match the given regular expression(s), or exclude files that match.
    /// </summary>
    public class FtpFileNameRegexRule : FtpRule
    {

        /// <summary>
        /// If true, only items where one of the supplied regex pattern matches are uploaded or downloaded.
        /// If false, items where one of the supplied regex pattern matches are excluded.
        /// </summary>
        public bool Whitelist;

        /// <summary>
        /// The files names to match
        /// </summary>
        public List<string> RegexPatterns;

        /// <summary>
        /// Only accept items that match one of the supplied regex patterns.
        /// </summary>
        /// <param name="whitelist">If true, only items where one of the supplied regex pattern matches are uploaded or downloaded. If false, items where one of the supplied regex pattern matches are excluded.</param>
        /// <param name="regexPatterns">The list of regex patterns to match. Only valid patterns are accepted and stored. If none of the patterns are valid, this rule is disabled and passes all objects.</param>
        public FtpFileNameRegexRule(bool whitelist, IList<string> regexPatterns)
        {
            this.Whitelist = whitelist;
            this.RegexPatterns = regexPatterns.Where(x => x.IsValidRegEx()).ToList();
        }

        /// <summary>
        /// Checks if the FtpListItem Name does match any RegexPattern
        /// </summary>
        public override bool IsAllowed(FtpListItem item)
        {

            // if no valid regex patterns, accept all objects
            if (RegexPatterns.Count == 0)
            {
                return true;
            }

            // only check files
            if (item.Type == FtpFileSystemObjectType.File)
            {
                var fileName = item.Name;

                if (Whitelist)
                {
                    return RegexPatterns.Any(x => Regex.IsMatch(fileName, x));
                }
                else
                {
                    return !RegexPatterns.Any(x => Regex.IsMatch(fileName, x));
                }
            }
            else
            {
                return true;
            }
        }

    }
    /// <summary>
    /// Only accept files that have the given name, or exclude files of a given name.
    /// </summary>
    public class FtpFileNameRule : FtpRule
    {

        /// <summary>
        /// If true, only files of the given name are uploaded or downloaded. If false, files of the given name are excluded.
        /// </summary>
        public bool Whitelist;

        /// <summary>
        /// The files names to match
        /// </summary>
        public IList<string> Names;

        /// <summary>
        /// Only accept files that have the given name, or exclude files of a given name.
        /// </summary>
        /// <param name="whitelist">If true, only files of the given name are downloaded. If false, files of the given name are excluded.</param>
        /// <param name="names">The files names to match</param>
        public FtpFileNameRule(bool whitelist, IList<string> names)
        {
            this.Whitelist = whitelist;
            this.Names = names;
        }

        /// <summary>
        /// Checks if the files has the given name, or exclude files of the given name.
        /// </summary>
        public override bool IsAllowed(FtpListItem item)
        {
            if (item.Type == FtpFileSystemObjectType.File)
            {
                var fileName = item.Name;
                if (Whitelist)
                {
                    return Names.Contains(fileName);
                }
                else
                {
                    return !Names.Contains(fileName);
                }
            }
            else
            {
                return true;
            }
        }

    }
    /// <summary>
    /// Only accept folders whose names match the given regular expression(s), or exclude folders that match.
    /// </summary>
    public class FtpFolderRegexRule : FtpRule
    {

        /// <summary>
        /// If true, only folders where one of the supplied regex pattern matches are uploaded or downloaded.
        /// If false, folders where one of the supplied regex pattern matches are excluded.
        /// </summary>
        public bool Whitelist;

        /// <summary>
        /// The files names to match
        /// </summary>
        public List<string> RegexPatterns;

        /// <summary>
        /// Which path segment to start checking from
        /// </summary>
        public int StartSegment;

        /// <summary>
        /// Only accept items that one of the supplied regex pattern.
        /// </summary>
        /// <param name="whitelist">If true, only folders where one of the supplied regex pattern matches are uploaded or downloaded. If false, folders where one of the supplied regex pattern matches are excluded.</param>
        /// <param name="regexPatterns">The list of regex patterns to match. Only valid patterns are accepted and stored. If none of the patterns are valid, this rule is disabled and passes all objects.</param>
        /// <param name="startSegment">Which path segment to start checking from. 0 checks root folder onwards. 1 skips root folder.</param>
        public FtpFolderRegexRule(bool whitelist, IList<string> regexPatterns, int startSegment = 0)
        {
            this.Whitelist = whitelist;
            this.RegexPatterns = regexPatterns.Where(x => x.IsValidRegEx()).ToList();
            this.StartSegment = startSegment;
        }

        /// <summary>
        /// Checks if the FtpListItem Name does match any RegexPattern
        /// </summary>
        public override bool IsAllowed(FtpListItem item)
        {

            // if no valid regex patterns, accept all objects
            if (RegexPatterns.Count == 0)
            {
                return true;
            }

            // get the folder name of this item
            string[] dirNameParts = null;
            if (item.Type == FtpFileSystemObjectType.File)
            {
                dirNameParts = item.FullName.GetFtpDirectoryName().GetPathSegments();
            }
            else if (item.Type == FtpFileSystemObjectType.Directory)
            {
                dirNameParts = item.FullName.GetPathSegments();
            }
            else
            {
                return true;
            }

            // check against whitelist or blacklist
            if (Whitelist)
            {

                // loop thru path segments starting at given index
                for (int d = StartSegment; d < dirNameParts.Length; d++)
                {
                    var dirName = dirNameParts[d];

                    // whitelist
                    foreach (var pattern in RegexPatterns)
                    {
                        if (Regex.IsMatch(dirName.Trim(), pattern))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            else
            {

                // loop thru path segments starting at given index
                for (int d = StartSegment; d < dirNameParts.Length; d++)
                {
                    var dirName = dirNameParts[d];

                    // blacklist
                    foreach (var pattern in RegexPatterns)
                    {
                        if (Regex.IsMatch(dirName.Trim(), pattern))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

        }

    }
    /// <summary>
    /// Only accept folders that have the given name, or exclude folders of a given name.
    /// </summary>
    public class FtpFolderNameRule : FtpRule
    {

        public static List<string> CommonBlacklistedFolders = new List<string> {
            ".git",
            ".svn",
            ".DS_Store",
            "node_modules",
        };

        /// <summary>
        /// If true, only folders of the given name are uploaded or downloaded.
        /// If false, folders of the given name are excluded.
        /// </summary>
        public bool Whitelist;

        /// <summary>
        /// The folder names to match
        /// </summary>
        public IList<string> Names;

        /// <summary>
        /// Which path segment to start checking from
        /// </summary>
        public int StartSegment;

        /// <summary>
        /// Only accept folders that have the given name, or exclude folders of a given name.
        /// </summary>
        /// <param name="whitelist">If true, only folders of the given name are downloaded. If false, folders of the given name are excluded.</param>
        /// <param name="names">The folder names to match</param>
        /// <param name="startSegment">Which path segment to start checking from. 0 checks root folder onwards. 1 skips root folder.</param>
        public FtpFolderNameRule(bool whitelist, IList<string> names, int startSegment = 0)
        {
            this.Whitelist = whitelist;
            this.Names = names;
            this.StartSegment = startSegment;
        }

        /// <summary>
        /// Checks if the folders has the given name, or exclude folders of the given name.
        /// </summary>
        public override bool IsAllowed(FtpListItem item)
        {

            // get the folder name of this item
            string[] dirNameParts = null;
            if (item.Type == FtpFileSystemObjectType.File)
            {
                dirNameParts = item.FullName.GetFtpDirectoryName().GetPathSegments();
            }
            else if (item.Type == FtpFileSystemObjectType.Directory)
            {
                dirNameParts = item.FullName.GetPathSegments();
            }
            else
            {
                return true;
            }

            // check against whitelist or blacklist
            if (Whitelist)
            {

                // loop thru path segments starting at given index
                for (int d = StartSegment; d < dirNameParts.Length; d++)
                {
                    var dirName = dirNameParts[d];

                    // whitelist
                    if (Names.Contains(dirName.Trim()))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {

                // loop thru path segments starting at given index
                for (int d = StartSegment; d < dirNameParts.Length; d++)
                {
                    var dirName = dirNameParts[d];

                    // blacklist
                    if (Names.Contains(dirName.Trim()))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

    }
    /// <summary>
    /// Base class used for all FTP Rules. Extend this class to create custom rules.
    /// You only need to provide an implementation for IsAllowed, and add any custom arguments that you require.
    /// </summary>
    public class FtpRule
    {

        public FtpRule()
        {
        }

        /// <summary>
        /// Returns true if the object has passed this rules.
        /// </summary>
        public virtual bool IsAllowed(FtpListItem result)
        {
            return true;
        }

        /// <summary>
        /// Returns true if the object has passed all the rules.
        /// </summary>
        public static bool IsAllAllowed(List<FtpRule> rules, FtpListItem result)
        {
            foreach (var rule in rules)
            {
                if (!rule.IsAllowed(result))
                {
                    return false;
                }
            }
            return true;
        }

    }
    /// <summary>
    /// Only accept files that are of the given size, or within the given range of sizes.
    /// </summary>
    public class FtpSizeRule : FtpRule
    {

        /// <summary>
        /// Which operator to use
        /// </summary>
        public FtpOperator Operator;

        /// <summary>
        /// The first value, required for all operators
        /// </summary>
        public long X;

        /// <summary>
        /// The second value, only required for BetweenRange and OutsideRange operators
        /// </summary>
        public long Y;

        /// <summary>
        /// Only accept files that are of the given size, or within the given range of sizes.
        /// </summary>
        /// <param name="ruleOperator">Which operator to use</param>
        /// <param name="x">The first value, required for all operators</param>
        /// <param name="y">The second value, only required for BetweenRange and OutsideRange operators.</param>
        public FtpSizeRule(FtpOperator ruleOperator, long x, long y = 0)
        {
            this.Operator = ruleOperator;
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Checks if the file is of the given size, or within the given range of sizes.
        /// </summary>
        public override bool IsAllowed(FtpListItem result)
        {
            if (result.Type == FtpFileSystemObjectType.File)
            {
                return Operators.Validate(Operator, result.Size, X, Y);
            }
            else
            {
                return true;
            }
        }

    }
    #endregion // Rules
    #region // Stream
    /// <summary>
    /// Base class for data stream connections
    /// </summary>
    public class FtpDataStream : FtpSocketStream
    {
        private FtpReply m_commandStatus;

        /// <summary>
        /// Gets the status of the command that was used to open
        /// this data channel
        /// </summary>
        public FtpReply CommandStatus
        {
            get => m_commandStatus;
            set => m_commandStatus = value;
        }

        private FtpClient m_control = null;

        /// <summary>
        /// Gets or sets the control connection for this data stream. Setting
        /// the control connection causes the object to be cloned and a new
        /// connection is made to the server to carry out the task. This ensures
        /// that multiple streams can be opened simultaneously.
        /// </summary>
        public FtpClient ControlConnection
        {
            get => m_control;
            set => m_control = value;
        }

        private long m_length = 0;

        /// <summary>
        /// Gets or sets the length of the stream. Only valid for file transfers
        /// and only valid on servers that support the Size command.
        /// </summary>
        public override long Length => m_length;

        private long m_position = 0;

        /// <summary>
        /// Gets or sets the position of the stream
        /// </summary>
        public override long Position
        {
            get => m_position;
            set => throw new InvalidOperationException("You cannot modify the position of a FtpDataStream. This property is updated as data is read or written to the stream.");
        }

        /// <summary>
        /// Reads data off the stream
        /// </summary>
        /// <param name="buffer">The buffer to read into</param>
        /// <param name="offset">Where to start in the buffer</param>
        /// <param name="count">Number of bytes to read</param>
        /// <returns>The number of bytes read</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            var read = base.Read(buffer, offset, count);
            m_position += read;
            return read;
        }

#if !NET40
        /// <summary>
        /// Reads data off the stream asynchronously
        /// </summary>
        /// <param name="buffer">The buffer to read into</param>
        /// <param name="offset">Where to start in the buffer</param>
        /// <param name="count">Number of bytes to read</param>
        /// <param name="token">The cancellation token for this task</param>
        /// <returns>The number of bytes read</returns>
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken token)
        {
            int read = await base.ReadAsync(buffer, offset, count, token);
            m_position += read;
            return read;
        }
#endif

        /// <summary>
        /// Writes data to the stream
        /// </summary>
        /// <param name="buffer">The buffer to write to the stream</param>
        /// <param name="offset">Where to start in the buffer</param>
        /// <param name="count">The number of bytes to write to the buffer</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            base.Write(buffer, offset, count);
            m_position += count;
        }

#if !NET40
        /// <summary>
        /// Writes data to the stream asynchronously
        /// </summary>
        /// <param name="buffer">The buffer to write to the stream</param>
        /// <param name="offset">Where to start in the buffer</param>
        /// <param name="count">The number of bytes to write to the buffer</param>
        /// <param name="token">The <see cref="CancellationToken"/> for this task</param>
        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken token)
        {
            await base.WriteAsync(buffer, offset, count, token);
            m_position += count;
        }
#endif

        /// <summary>
        /// Sets the length of this stream
        /// </summary>
        /// <param name="value">Value to apply to the Length property</param>
        public override void SetLength(long value)
        {
            m_length = value;
        }

        /// <summary>
        /// Sets the position of the stream. Intended to be used
        /// internally by FtpControlConnection.
        /// </summary>
        /// <param name="pos">The position</param>
        public void SetPosition(long pos)
        {
            m_position = pos;
        }

        /// <summary>
        /// Closes the connection and reads the server's reply
        /// </summary>
        public new FtpReply Close()
        {
            base.Close();

            try
            {
                if (ControlConnection != null)
                {
                    return ControlConnection.CloseDataStream(this);
                }
            }
            finally
            {
                m_commandStatus = new FtpReply();
                m_control = null;
            }

            return new FtpReply();
        }

        /// <summary>
        /// Creates a new data stream object
        /// </summary>
        /// <param name="conn">The control connection to be used for carrying out this operation</param>
        public FtpDataStream(FtpClient conn) : base(conn)
        {
            if (conn == null)
            {
                throw new ArgumentException("The control connection cannot be null.");
            }

            ControlConnection = conn;

            // always accept certificate no matter what because if code execution ever
            // gets here it means the certificate on the control connection object being
            // cloned was already accepted.
            ValidateCertificate += new FtpSocketStreamSslValidation(delegate (FtpSocketStream obj, FtpSslValidationEventArgs e) { e.Accept = true; });

            m_position = 0;
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~FtpDataStream()
        {
            // Fix: Hard catch and suppress all exceptions during disposing as there are constant issues with this method
            try
            {
                Dispose(false);
            }
            catch (Exception ex)
            {
            }
        }
    }

    public static class FtpFileStream
    {

        /// <summary>
        /// Returns the file size using synchronous file I/O.
        /// </summary>
        public static long GetFileSize(string localPath, bool checkExists)
        {
            if (checkExists)
            {
                if (!File.Exists(localPath))
                {
                    return 0;
                }
            }
            return new FileInfo(localPath).Length;
        }

#if !NET40
        /// <summary>
        /// Returns the file size using async file I/O.
        /// </summary>
        public static async Task<long> GetFileSizeAsync(string localPath, bool checkExists, CancellationToken token)
        {
            if (checkExists)
            {
                if (!(await Task.Run(() => File.Exists(localPath), token)))
                {
                    return 0;
                }
            }
            return (await Task.Run(() => new FileInfo(localPath), token)).Length;
        }
#endif

        /// <summary>
        /// Returns the file size using synchronous file I/O.
        /// </summary>
        public static DateTime GetFileDateModifiedUtc(string localPath)
        {
            return new FileInfo(localPath).LastWriteTimeUtc;
        }

#if !NET40
        /// <summary>
        /// Returns the file size using synchronous file I/O.
        /// </summary>
        public static async Task<DateTime> GetFileDateModifiedUtcAsync(string localPath, CancellationToken token)
        {
            return (await Task.Run(() => new FileInfo(localPath), token)).LastWriteTimeUtc;
        }
#endif

        /// <summary>
        /// Returns a new stream to upload a file from disk.
        /// If the file fits within the fileSizeLimit, then it is read in a single disk call and stored in memory, and a MemoryStream is returned.
        /// If it is larger than that, then a regular read-only FileStream is returned.
        /// </summary>
        public static Stream GetFileReadStream(FtpClient client, string localPath, bool isAsync, long fileSizeLimit, long knownLocalFileSize = 0)
        {

            // if quick transfer is enabled
            /*if (fileSizeLimit > 0) {

				// ensure we have the size of the local file
				if (knownLocalFileSize == 0) {
					knownLocalFileSize = GetFileSize(localPath, false);
				}

				// check if quick transfer mode is possible
				if (knownLocalFileSize > 0 && knownLocalFileSize < fileSizeLimit) {

					// trace
					if (client != null) {
						client.LogStatus(FtpTraceLevel.Verbose, "Using quick transfer for " + knownLocalFileSize.FileSizeToString() + " file, within " + fileSizeLimit.FileSizeToString());
					}

					// read the entire file into memory
					var bytes = File.ReadAllBytes(localPath);

					// create a new memory stream wrapping the bytes and return that
					return new MemoryStream(bytes);
				}

			}

			// trace
			if (client != null) {
				client.LogStatus(FtpTraceLevel.Verbose, "Using file stream for " + knownLocalFileSize.FileSizeToString() + " file, outside " + fileSizeLimit.FileSizeToString());
			}*/

            // normal slow mode, return a FileStream
            var bufferSize = client != null ? client.LocalFileBufferSize : 4096;
            return new FileStream(localPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, isAsync);
        }

        /// <summary>
        /// Returns a new stream to download a file to disk.
        /// If the file fits within the fileSizeLimit, then a new MemoryStream is returned.
        /// If it is larger than that, then a regular writable FileStream is returned.
        /// </summary>
        public static Stream GetFileWriteStream(FtpClient client, string localPath, bool isAsync, long fileSizeLimit, long knownRemoteFileSize = 0, bool isAppend = false, long restartPos = 0)
        {

            // if quick transfer is enabled
            /*if (fileSizeLimit > 0) {

				// check if quick transfer mode is possible
				if (!isAppend && restartPos == 0 && knownRemoteFileSize > 0 && knownRemoteFileSize < fileSizeLimit) {

					// trace
					if (client != null) {
						client.LogStatus(FtpTraceLevel.Info, "Using quick transfer for " + knownRemoteFileSize.FileSizeToString() + " file, within " + fileSizeLimit.FileSizeToString());
					}

					// create a new memory stream and return that
					return new MemoryStream();
				}
			}

			// trace
			if (client != null) {
				client.LogStatus(FtpTraceLevel.Verbose, "Using file stream for " + knownRemoteFileSize.FileSizeToString() + " file, outside " + fileSizeLimit.FileSizeToString());
			}*/

            // normal slow mode, return a FileStream
            var bufferSize = client != null ? client.LocalFileBufferSize : 4096;
            if (isAppend)
            {
                return new FileStream(localPath, FileMode.Append, FileAccess.Write, FileShare.None, bufferSize, isAsync);
            }
            else
            {
                return new FileStream(localPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, isAsync);
            }
        }

        /// <summary>
        /// If the stream is a MemoryStream, completes the quick download by writing the file to disk.
        /// </summary>
        public static void CompleteQuickFileWrite(Stream fileStream, string localPath)
        {

            // if quick transfer is enabled
            /*if (fileStream is MemoryStream) {

				// write the file to disk using a single disk call
				using (var file = new FileStream(localPath, FileMode.Create, FileAccess.Write, FileShare.None, client.LocalFileBufferSize, false)) {
					fileStream.Position = 0;
					((MemoryStream)fileStream).WriteTo(file);
				}
			}*/
        }

#if !NET40
        /// <summary>
        /// If the stream is a MemoryStream, completes the quick download by writing the file to disk.
        /// </summary>
        public static Task CompleteQuickFileWriteAsync(Stream fileStream, string localPath, CancellationToken token)
        {
#if NET45
			return Task.FromResult(true);
#else
            return Task.CompletedTask;
#endif
            // if quick transfer is enabled
            /*if (fileStream is MemoryStream) {

				// write the file to disk using a single disk call
				await Task.Run(() => {
					using (var file = new FileStream(localPath, FileMode.Create, FileAccess.Write, FileShare.None, client.LocalFileBufferSize, false)) {
						fileStream.Position = 0;
						((MemoryStream)fileStream).WriteTo(file);
					}
				}, token);
			}*/

        }
#endif

    }
    /// <summary>
    /// Stream class used for talking. Used by FtpClient, extended by FtpDataStream
    /// </summary>
    public class FtpSocketStream : Stream, IDisposable
    {
        public readonly FtpClient Client;

        public FtpSocketStream(FtpClient conn)
        {
            Client = conn;
        }

        /// <summary>
        /// Used for tacking read/write activity on the socket
        /// to determine if Poll() should be used to test for
        /// socket connectivity. The socket in this class will
        /// not know it has been disconnected if the remote host
        /// closes the connection first. Using Poll() avoids 
        /// the exception that would be thrown when trying to
        /// read or write to the disconnected socket.
        /// </summary>
        private DateTime m_lastActivity = DateTime.Now;

        private Socket m_socket = null;

        /// <summary>
        /// The socket used for talking
        /// </summary>
        protected Socket Socket
        {
            get => m_socket;
            private set => m_socket = value;
        }

        private int m_socketPollInterval = 15000;

        /// <summary>
        /// Gets or sets the length of time in milliseconds
        /// that must pass since the last socket activity
        /// before calling Poll() on the socket to test for
        /// connectivity. Setting this interval too low will
        /// have a negative impact on performance. Setting this
        /// interval to 0 disables Poll()'ing all together.
        /// The default value is 15 seconds.
        /// </summary>
        public int SocketPollInterval
        {
            get => m_socketPollInterval;
            set => m_socketPollInterval = value;
        }

        /// <summary>
        /// Gets the number of available bytes on the socket, 0 if the
        /// socket has not been initialized. This property is used internally
        /// by FtpClient in an effort to detect disconnections and gracefully
        /// reconnect the control connection.
        /// </summary>
        internal int SocketDataAvailable
        {
            get
            {
                if (m_socket != null)
                {
                    return m_socket.Available;
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets a value indicating if this socket stream is connected
        /// </summary>
        public bool IsConnected
        {
            get
            {
                try
                {
                    if (m_socket == null)
                    {
                        return false;
                    }

                    if (!m_socket.Connected)
                    {
                        Close();
                        return false;
                    }

                    if (!CanRead || !CanWrite)
                    {
                        Close();
                        return false;
                    }

                    if (m_socketPollInterval > 0 && DateTime.Now.Subtract(m_lastActivity).TotalMilliseconds > m_socketPollInterval)
                    {
                        Client.LogStatus(FtpTraceLevel.Verbose, "Testing connectivity using Socket.Poll()...");

                        // FIX : #273 update m_lastActivity to the current time
                        m_lastActivity = DateTime.Now;

                        if (m_socket.Poll(500000, SelectMode.SelectRead) && m_socket.Available == 0)
                        {
                            Close();
                            return false;
                        }
                    }
                }
                catch (SocketException sockex)
                {
                    Close();
                    Client.LogStatus(FtpTraceLevel.Warn, "FtpSocketStream.IsConnected: Caught and discarded SocketException while testing for connectivity: " + sockex.ToString());
                    return false;
                }
                catch (IOException ioex)
                {
                    Close();
                    Client.LogStatus(FtpTraceLevel.Warn, "FtpSocketStream.IsConnected: Caught and discarded IOException while testing for connectivity: " + ioex.ToString());
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating if encryption is being used
        /// </summary>
        public bool IsEncrypted
        {
            get
            {
                return m_sslStream != null;
            }
        }

        private NetworkStream m_netStream = null;

        /// <summary>
        /// The non-encrypted stream
        /// </summary>
        private NetworkStream NetworkStream
        {
            get => m_netStream;
            set => m_netStream = value;
        }

        private BufferedStream m_bufStream = null;

        private SslStream m_sslStream = null;

        /// <summary>
        /// The encrypted stream
        /// </summary>
        private SslStream SslStream
        {
            get => m_sslStream;
            set => m_sslStream = value;
        }
        /// <summary>
        /// Gets the underlying stream, could be a NetworkStream or SslStream
        /// </summary>
        protected Stream BaseStream
        {
            get
            {
                if (m_sslStream != null)
                {
                    return m_sslStream;
                }
                else if (m_netStream != null)
                {
                    return m_netStream;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets a value indicating if this stream can be read
        /// </summary>
        public override bool CanRead
        {
            get
            {
                if (m_netStream != null)
                {
                    return m_netStream.CanRead;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating if this stream if seekable
        /// </summary>
        public override bool CanSeek => false;

        /// <summary>
        /// Gets a value indicating if this stream can be written to
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                if (m_netStream != null)
                {
                    return m_netStream.CanWrite;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the length of the stream
        /// </summary>
        public override long Length => 0;

        /// <summary>
        /// Gets the current position of the stream. Trying to
        /// set this property throws an InvalidOperationException()
        /// </summary>
        public override long Position
        {
            get
            {
                if (BaseStream != null)
                {
                    return BaseStream.Position;
                }

                return 0;
            }
            set => throw new InvalidOperationException();
        }

        private event FtpSocketStreamSslValidation m_sslvalidate = null;

        /// <summary>
        /// Event is fired when a SSL certificate needs to be validated
        /// </summary>
        public event FtpSocketStreamSslValidation ValidateCertificate
        {
            add => m_sslvalidate += value;
            remove => m_sslvalidate -= value;
        }

        private int m_readTimeout = Timeout.Infinite;

        /// <summary>
        /// Gets or sets the amount of time to wait for a read operation to complete. Default
        /// value is Timeout.Infinite.
        /// </summary>
        public override int ReadTimeout
        {
            get => m_readTimeout;
            set
            {
                m_readTimeout = value;
                if (m_netStream != null)
                {
                    m_netStream.ReadTimeout = m_readTimeout;
                }
            }
        }

        private int m_connectTimeout = 30000;

        /// <summary>
        /// Gets or sets the length of time milliseconds to wait
        /// for a connection succeed before giving up. The default
        /// is 30000 (30 seconds).
        /// </summary>
        public int ConnectTimeout
        {
            get => m_connectTimeout;
            set => m_connectTimeout = value;
        }

        /// <summary>
        /// Gets the local end point of the socket
        /// </summary>
        public IPEndPoint LocalEndPoint
        {
            get
            {
                if (m_socket == null)
                {
                    return null;
                }

                return (IPEndPoint)m_socket.LocalEndPoint;
            }
        }

        /// <summary>
        /// Gets the remote end point of the socket
        /// </summary>
        public IPEndPoint RemoteEndPoint
        {
            get
            {
                if (m_socket == null)
                {
                    return null;
                }

                return (IPEndPoint)m_socket.RemoteEndPoint;
            }
        }

        /// <summary>
        /// Fires the SSL certificate validation event
        /// </summary>
        /// <param name="certificate">Certificate being validated</param>
        /// <param name="chain">Certificate chain</param>
        /// <param name="errors">Policy errors if any</param>
        /// <returns>True if it was accepted, false otherwise</returns>
        protected bool OnValidateCertificate(X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            var evt = m_sslvalidate;

            if (evt != null)
            {
                var e = new FtpSslValidationEventArgs()
                {
                    Certificate = certificate,
                    Chain = chain,
                    PolicyErrors = errors,
                    Accept = errors == SslPolicyErrors.None
                };

                evt(this, e);
                return e.Accept;
            }

            // if the event was not handled then only accept
            // the certificate if there were no validation errors
            return errors == SslPolicyErrors.None;
        }

        /// <summary>
        /// Throws an InvalidOperationException
        /// </summary>
        /// <param name="offset">Ignored</param>
        /// <param name="origin">Ignored</param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Throws an InvalidOperationException
        /// </summary>
        /// <param name="value">Ignored</param>
        public override void SetLength(long value)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Flushes the stream
        /// </summary>
        public override void Flush()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("The FtpSocketStream object is not connected.");
            }

            if (BaseStream == null)
            {
                throw new InvalidOperationException("The base stream of the FtpSocketStream object is null.");
            }

            BaseStream.Flush();
        }

#if !NET40

        /// <summary>
        /// Flushes the stream asynchronously
        /// </summary>
        /// <param name="token">The <see cref="CancellationToken"/> for this task</param>
        public override async Task FlushAsync(CancellationToken token)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("The FtpSocketStream object is not connected.");
            }

            if (BaseStream == null)
            {
                throw new InvalidOperationException("The base stream of the FtpSocketStream object is null.");
            }

            await BaseStream.FlushAsync(token);
        }

#endif

        /// <summary>
        /// Bypass the stream and read directly off the socket.
        /// </summary>
        /// <param name="buffer">The buffer to read into</param>
        /// <returns>The number of bytes read</returns>
        internal int RawSocketRead(byte[] buffer)
        {
            var read = 0;

            if (m_socket != null && m_socket.Connected)
            {
                read = m_socket.Receive(buffer, buffer.Length, 0);
            }

            return read;
        }

        internal async Task EnableCancellation(Task task, CancellationToken token, Action action)
        {
            var registration = token.Register(action);
            _ = task.ContinueWith(x => registration.Dispose(), CancellationToken.None);
            await task;
        }

        internal async Task<T> EnableCancellation<T>(Task<T> task, CancellationToken token, Action action)
        {
            var registration = token.Register(action);
            _ = task.ContinueWith(x => registration.Dispose(), CancellationToken.None);
            return await task;
        }


#if NET45
		/// <summary>
		/// Bypass the stream and read directly off the socket.
		/// </summary>
		/// <param name="buffer">The buffer to read into</param>
		/// <param name="token">The token that can be used to cancel the entire process</param>
		/// <returns>The number of bytes read</returns>
		internal async Task<int> RawSocketReadAsync(byte[] buffer, CancellationToken token) {
			var read = 0;

			if (m_socket != null && m_socket.Connected) {
				var asyncResult = m_socket.BeginReceive(buffer, 0, buffer.Length, 0, null, null);
				read = await EnableCancellation(
					Task.Factory.FromAsync(asyncResult, m_socket.EndReceive),
					token, 
					() => CloseSocket()
				);
			}

			return read;
		}

#endif

#if NETFx
        /// <summary>
        /// Bypass the stream and read directly off the socket.
        /// </summary>
        /// <param name="buffer">The buffer to read into</param>
        /// <returns>The number of bytes read</returns>
        internal async Task<int> RawSocketReadAsync(byte[] buffer, CancellationToken token)
        {
            var read = 0;

            if (m_socket != null && m_socket.Connected && !token.IsCancellationRequested)
            {
                read = await m_socket.ReceiveAsync(new ArraySegment<byte>(buffer), 0);
            }

            return read;
        }
#endif

        /// <summary>
        /// Reads data from the stream
        /// </summary>
        /// <param name="buffer">Buffer to read into</param>
        /// <param name="offset">Where in the buffer to start</param>
        /// <param name="count">Number of bytes to be read</param>
        /// <returns>The amount of bytes read from the stream</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
#if !NETFx
            IAsyncResult ar = null;
#endif

            if (BaseStream == null)
            {
                return 0;
            }

            m_lastActivity = DateTime.Now;
#if NETFx
            return BaseStream.Read(buffer, offset, count);
#else
            ar = BaseStream.BeginRead(buffer, offset, count, null, null);
            bool success = ar.AsyncWaitHandle.WaitOne(m_readTimeout, true);
            ar.AsyncWaitHandle.Close();
            if (!success)
            {
                Close();
                throw new TimeoutException("Timed out trying to read data from the socket stream!");
            }

            return BaseStream.EndRead(ar);
#endif
        }

#if !NET40

        /// <summary>
        /// Reads data from the stream
        /// </summary>
        /// <param name="buffer">Buffer to read into</param>
        /// <param name="offset">Where in the buffer to start</param>
        /// <param name="count">Number of bytes to be read</param>
        /// <param name="token">The <see cref="CancellationToken"/> for this task</param>
        /// <returns>The amount of bytes read from the stream</returns>
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken token)
        {
            if (BaseStream == null)
            {
                return 0;
            }

            m_lastActivity = DateTime.Now;
            using (var cts = CancellationTokenSource.CreateLinkedTokenSource(token))
            {
                cts.CancelAfter(ReadTimeout);
                cts.Token.Register(() => Close());
                try
                {
                    var res = await BaseStream.ReadAsync(buffer, offset, count, cts.Token);
                    return res;
                }
                catch
                {
                    // CTS for Cancellation triggered and caused the exception
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException("Cancelled read from socket stream");
                    }

                    // CTS for Timeout triggered and caused the exception
                    if (cts.IsCancellationRequested)
                    {
                        throw new TimeoutException("Timed out trying to read data from the socket stream!");
                    }

                    // Nothing of the above. So we rethrow the exception.
                    throw;
                }
            }
        }

#endif

        /// <summary>
        /// Reads a line from the socket
        /// </summary>
        /// <param name="encoding">The type of encoding used to convert from byte[] to string</param>
        /// <returns>A line from the stream, null if there is nothing to read</returns>
        public string ReadLine(System.Text.Encoding encoding)
        {
            var data = new List<byte>();
            var buf = new byte[1];
            string line = null;

            while (Read(buf, 0, buf.Length) > 0)
            {
                data.Add(buf[0]);
                if ((char)buf[0] == '\n')
                {
                    line = encoding.GetString(data.ToArray()).Trim('\r', '\n');
                    break;
                }
            }

            return line;
        }

        /// <summary>
        /// Reads all line from the socket
        /// </summary>
        /// <param name="encoding">The type of encoding used to convert from byte[] to string</param>
        /// <param name="bufferSize">The size of the buffer</param>
        /// <returns>A list of lines from the stream</returns>
        public IEnumerable<string> ReadAllLines(System.Text.Encoding encoding, int bufferSize)
        {
            int charRead;
            var data = new List<byte>();
            var buf = new byte[bufferSize];

            while ((charRead = Read(buf, 0, buf.Length)) > 0)
            {
                var firstByteToReadIdx = 0;

                var separatorIdx = Array.IndexOf(buf, (byte)'\n', firstByteToReadIdx, charRead - firstByteToReadIdx); //search in full byte array readed

                while (separatorIdx >= 0) // at least one '\n' returned
                {
                    while (firstByteToReadIdx <= separatorIdx)
                    {
                        data.Add(buf[firstByteToReadIdx++]);
                    }

                    var line = encoding.GetString(data.ToArray()).Trim('\r', '\n'); // convert data to string
                    yield return line;
                    data.Clear();

                    separatorIdx = Array.IndexOf(buf, (byte)'\n', firstByteToReadIdx, charRead - firstByteToReadIdx); //search in full byte array readed
                }

                while (firstByteToReadIdx < charRead) // add all remaining characters to data
                {
                    data.Add(buf[firstByteToReadIdx++]);
                }
            }
        }

#if !NET40
        /// <summary>
        /// Reads a line from the socket asynchronously
        /// </summary>
        /// <param name="encoding">The type of encoding used to convert from byte[] to string</param>
        /// <param name="token">The <see cref="CancellationToken"/> for this task</param>
        /// <returns>A line from the stream, null if there is nothing to read</returns>
        public async Task<string> ReadLineAsync(System.Text.Encoding encoding, CancellationToken token)
        {
            var data = new List<byte>();
            var buf = new byte[1];
            string line = null;

            while (await ReadAsync(buf, 0, buf.Length, token) > 0)
            {
                data.Add(buf[0]);
                if ((char)buf[0] == '\n')
                {
                    line = encoding.GetString(data.ToArray()).Trim('\r', '\n');
                    break;
                }
            }

            return line;
        }

        /// <summary>
        /// Reads all line from the socket
        /// </summary>
        /// <param name="encoding">The type of encoding used to convert from byte[] to string</param>
        /// <param name="bufferSize">The size of the buffer</param>
        /// <returns>A list of lines from the stream</returns>
        public async Task<IEnumerable<string>> ReadAllLinesAsync(System.Text.Encoding encoding, int bufferSize, CancellationToken token)
        {
            int charRead;
            var data = new List<byte>();
            var lines = new List<string>();
            var buf = new byte[bufferSize];

            while ((charRead = await ReadAsync(buf, 0, buf.Length, token)) > 0)
            {
                var firstByteToReadIdx = 0;

                var separatorIdx = Array.IndexOf(buf, (byte)'\n', firstByteToReadIdx, charRead - firstByteToReadIdx); //search in full byte array read

                while (separatorIdx >= 0) // at least one '\n' returned
                {
                    while (firstByteToReadIdx <= separatorIdx)
                    {
                        data.Add(buf[firstByteToReadIdx++]);
                    }

                    var line = encoding.GetString(data.ToArray()).Trim('\r', '\n'); // convert data to string
                    lines.Add(line);
                    data.Clear();

                    separatorIdx = Array.IndexOf(buf, (byte)'\n', firstByteToReadIdx, charRead - firstByteToReadIdx); //search in full byte array read
                }

                while (firstByteToReadIdx < charRead) // add all remaining characters to data
                {
                    data.Add(buf[firstByteToReadIdx++]);
                }
            }

            return lines;
        }
#endif

        /// <summary>
        /// Writes data to the stream
        /// </summary>
        /// <param name="buffer">Buffer to write to stream</param>
        /// <param name="offset">Where in the buffer to start</param>
        /// <param name="count">Number of bytes to be read</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (BaseStream == null)
            {
                return;
            }

            BaseStream.Write(buffer, offset, count);
            m_lastActivity = DateTime.Now;
        }

#if !NET40
        /// <summary>
        /// Writes data to the stream asynchronously
        /// </summary>
        /// <param name="buffer">Buffer to write to stream</param>
        /// <param name="offset">Where in the buffer to start</param>
        /// <param name="count">Number of bytes to be read</param>
        /// <param name="token">The <see cref="CancellationToken"/> for this task</param>
        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken token)
        {
            if (BaseStream == null)
            {
                return;
            }

            await BaseStream.WriteAsync(buffer, offset, count, token);
            m_lastActivity = DateTime.Now;
        }
#endif

        /// <summary>
        /// Writes a line to the stream using the specified encoding
        /// </summary>
        /// <param name="encoding">Encoding used for writing the line</param>
        /// <param name="buf">The data to write</param>
        public void WriteLine(System.Text.Encoding encoding, string buf)
        {
            byte[] data;
            data = encoding.GetBytes(buf + "\r\n");
            Write(data, 0, data.Length);
        }

#if !NET40
        /// <summary>
        /// Writes a line to the stream using the specified encoding asynchronously
        /// </summary>
        /// <param name="encoding">Encoding used for writing the line</param>
        /// <param name="buf">The data to write</param>
        /// <param name="token">The <see cref="CancellationToken"/> for this task</param>
        public async Task WriteLineAsync(System.Text.Encoding encoding, string buf, CancellationToken token)
        {
            var data = encoding.GetBytes(buf + "\r\n");
            await WriteAsync(data, 0, data.Length, token);
        }
#endif

#if NETFx
        /// <summary>
        /// Disconnects from server
        /// </summary>
        public virtual void Close()
        {
            Dispose(true);
        }
#endif

        /// <summary>
        /// Disconnects from server
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            // Fix: Hard catch and suppress all exceptions during disposing as there are constant issues with this method
            try
            {
                // ensure null exceptions don't occur here
                if (Client != null)
                {
                    Client.LogStatus(FtpTraceLevel.Verbose, "Disposing FtpSocketStream...");
                }
            }
            catch (Exception)
            {
            }

            if (m_sslStream != null)
            {
                try
                {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                    m_sslStream.ShutdownAsync().RunSynchronously();
#endif
                    m_sslStream.Dispose();
                }
                catch (Exception ex)
                {
                }

                m_sslStream = null;
            }
            if (m_bufStream != null)
            {
                try
                {
                    // ensure the last of the buffered bytes are flushed
                    // before we close the socket and network stream
                    m_bufStream.Flush();
                    m_bufStream.Dispose();
                }
                catch (Exception ex)
                {
                }

                m_bufStream = null;
            }
            if (m_netStream != null)
            {
                try
                {
                    m_netStream.Dispose();
                }
                catch (Exception ex)
                {
                }

                m_netStream = null;
            }

            CloseSocket();
        }

        /// <summary>
        /// Safely close the socket if its open
        /// </summary>
        internal void CloseSocket()
        {
            if (m_socket != null)
            {
                try
                {
                    if (m_socket.Connected)
                    {
#if NETFx
                        m_socket.Shutdown(SocketShutdown.Send);
#endif
                    }
                    m_socket.Close();
                    m_socket.Dispose();
                }
                catch { }
                m_socket = null;
            }
        }

        /// <summary>
        /// Sets socket options on the underlying socket
        /// </summary>
        /// <param name="level">SocketOptionLevel</param>
        /// <param name="name">SocketOptionName</param>
        /// <param name="value">SocketOptionValue</param>
        public void SetSocketOption(SocketOptionLevel level, SocketOptionName name, bool value)
        {
            if (m_socket == null)
            {
                throw new InvalidOperationException("The underlying socket is null. Have you established a connection?");
            }

            m_socket.SetSocketOption(level, name, value);
        }

        /// <summary>
        /// Connect to the specified host
        /// </summary>
        /// <param name="host">The host to connect to</param>
        /// <param name="port">The port to connect to</param>
        /// <param name="ipVersions">Internet Protocol versions to support during the connection phase</param>
        public void Connect(string host, int port, FtpIpVersion ipVersions)
        {
#if NETFx
            IPAddress[] addresses = Dns.GetHostAddressesAsync(host).Result;
#else
            IAsyncResult ar = null;
            var addresses = Dns.GetHostAddresses(host);
#endif

            if (ipVersions == 0)
            {
                throw new ArgumentException("The ipVersions parameter must contain at least 1 flag.");
            }

            for (var i = 0; i < addresses.Length; i++)
            {
                // we don't need to do this check unless
                // a particular version of IP has been
                // omitted so we won't.
                if (ipVersions != FtpIpVersion.ANY)
                {
                    switch (addresses[i].AddressFamily)
                    {
                        case AddressFamily.InterNetwork:
                            if ((ipVersions & FtpIpVersion.IPv4) != FtpIpVersion.IPv4)
                            {
#if DEBUG
                                Client.LogStatus(FtpTraceLevel.Verbose, "Skipped IPV4 address : " + addresses[i].ToString());
#endif
                                continue;
                            }

                            break;

                        case AddressFamily.InterNetworkV6:
                            if ((ipVersions & FtpIpVersion.IPv6) != FtpIpVersion.IPv6)
                            {
#if DEBUG
                                Client.LogStatus(FtpTraceLevel.Verbose, "Skipped IPV6 address : " + addresses[i].ToString());
#endif
                                continue;
                            }

                            break;
                    }
                }

                if (FtpTrace.LogIP)
                {
                    Client.LogStatus(FtpTraceLevel.Info, "Connecting to " + addresses[i].ToString() + ":" + port);
                }
                else
                {
                    Client.LogStatus(FtpTraceLevel.Info, "Connecting to ***:" + port);
                }

                m_socket = new Socket(addresses[i].AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                BindSocketToLocalIp();

#if NETFx


                var args = new SocketAsyncEventArgs
                {
                    RemoteEndPoint = new IPEndPoint(addresses[i], port)
                };
                var connectEvent = new ManualResetEvent(false);
                args.Completed += (s, e) => { connectEvent.Set(); };

                if (m_socket.ConnectAsync(args))
                {
                    if (!connectEvent.WaitOne(m_connectTimeout))
                    {
                        Close();
                        if (i + 1 == addresses.Length)
                        {
                            throw new TimeoutException("Timed out trying to connect!");
                        }
                    }
                }

                if (args.SocketError != SocketError.Success)
                {
                    throw new SocketException((int)args.SocketError);
                }

                break;
#else
                ar = m_socket.BeginConnect(addresses[i], port, null, null);
                bool success = ar.AsyncWaitHandle.WaitOne(m_connectTimeout, true);
                ar.AsyncWaitHandle.Close();
                if (!success)
                {
                    Close();

                    // check to see if we're out of addresses, and throw a TimeoutException
                    if (i + 1 == addresses.Length)
                    {
                        throw new TimeoutException("Timed out trying to connect!");
                    }
                }
                else
                {
                    m_socket.EndConnect(ar);

                    // we got a connection, break out
                    // of the loop.
                    break;
                }

#endif
            }

            // make sure that we actually connected to
            // one of the addresses returned from GetHostAddresses()
            if (m_socket == null || !m_socket.Connected)
            {
                Close();
                throw new IOException("Failed to connect to host.");
            }

            m_netStream = new NetworkStream(m_socket);
            m_netStream.ReadTimeout = m_readTimeout;
            m_lastActivity = DateTime.Now;
        }

#if !NET40
        /// <summary>
        /// Connect to the specified host
        /// </summary>
        /// <param name="host">The host to connect to</param>
        /// <param name="port">The port to connect to</param>
        /// <param name="ipVersions">Internet Protocol versions to support during the connection phase</param>
        /// <param name="token">The token that can be used to cancel the entire process</param>
        public async Task ConnectAsync(string host, int port, FtpIpVersion ipVersions, CancellationToken token)
        {
            IPAddress[] addresses = await Dns.GetHostAddressesAsync(host);

            if (ipVersions == 0)
            {
                throw new ArgumentException("The ipVersions parameter must contain at least 1 flag.");
            }

            for (var i = 0; i < addresses.Length; i++)
            {
                // we don't need to do this check unless
                // a particular version of IP has been
                // omitted so we won't.
                if (ipVersions != FtpIpVersion.ANY)
                {
                    switch (addresses[i].AddressFamily)
                    {
                        case AddressFamily.InterNetwork:
                            if ((ipVersions & FtpIpVersion.IPv4) != FtpIpVersion.IPv4)
                            {
#if DEBUG
                                Client.LogStatus(FtpTraceLevel.Verbose, "Skipped IPV4 address : " + addresses[i].ToString());
#endif
                                continue;
                            }

                            break;

                        case AddressFamily.InterNetworkV6:
                            if ((ipVersions & FtpIpVersion.IPv6) != FtpIpVersion.IPv6)
                            {
#if DEBUG
                                Client.LogStatus(FtpTraceLevel.Verbose, "Skipped IPV6 address : " + addresses[i].ToString());
#endif
                                continue;
                            }

                            break;
                    }
                }

                if (FtpTrace.LogIP)
                {
                    Client.LogStatus(FtpTraceLevel.Info, "Connecting to " + addresses[i].ToString() + ":" + port);
                }
                else
                {
                    Client.LogStatus(FtpTraceLevel.Info, "Connecting to ***:" + port);
                }

                m_socket = new Socket(addresses[i].AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                BindSocketToLocalIp();
#if NETFx
                if (this.ConnectTimeout > 0)
                {
                    using (var timeoutSrc = CancellationTokenSource.CreateLinkedTokenSource(token))
                    {
                        timeoutSrc.CancelAfter(this.ConnectTimeout);
                        await EnableCancellation(m_socket.ConnectAsync(addresses[i], port), timeoutSrc.Token, () => CloseSocket());
                        break;
                    }
                }
                else
                {
                    await EnableCancellation(m_socket.ConnectAsync(addresses[i], port), token, () => CloseSocket());
                    break;
                }
#else
                var connectResult = m_socket.BeginConnect(addresses[i], port, null, null);
                await EnableCancellation(Task.Factory.FromAsync(connectResult, m_socket.EndConnect), token, () => CloseSocket());
                break;
#endif
            }

            // make sure that we actually connected to
            // one of the addresses returned from GetHostAddresses()
            if (m_socket == null || !m_socket.Connected)
            {
                Close();
                throw new IOException("Failed to connect to host.");
            }

            m_netStream = new NetworkStream(m_socket);
            m_netStream.ReadTimeout = m_readTimeout;
            m_lastActivity = DateTime.Now;
        }
#endif

        /// <summary>
        /// Activates SSL on this stream using default protocols. Fires the ValidateCertificate event. 
        /// If this event is not handled and there are SslPolicyErrors present, the certificate will 
        /// not be accepted.
        /// </summary>
        /// <param name="targethost">The host to authenticate the certificate against</param>
        public void ActivateEncryption(string targethost)
        {
            ActivateEncryption(targethost, null, Client.SslProtocols);
        }

#if !NET40
        /// <summary>
        /// Activates SSL on this stream using default protocols. Fires the ValidateCertificate event. 
        /// If this event is not handled and there are SslPolicyErrors present, the certificate will 
        /// not be accepted.
        /// </summary>
        /// <param name="targethost">The host to authenticate the certificate against</param>
        public async Task ActivateEncryptionAsync(string targethost)
        {
            await ActivateEncryptionAsync(targethost, null, Client.SslProtocols);
        }
#endif

        /// <summary>
        /// Activates SSL on this stream using default protocols. Fires the ValidateCertificate event.
        /// If this event is not handled and there are SslPolicyErrors present, the certificate will 
        /// not be accepted.
        /// </summary>
        /// <param name="targethost">The host to authenticate the certificate against</param>
        /// <param name="clientCerts">A collection of client certificates to use when authenticating the SSL stream</param>
        public void ActivateEncryption(string targethost, X509CertificateCollection clientCerts)
        {
            ActivateEncryption(targethost, clientCerts, Client.SslProtocols);
        }

#if !NET40
        /// <summary>
        /// Activates SSL on this stream using default protocols. Fires the ValidateCertificate event.
        /// If this event is not handled and there are SslPolicyErrors present, the certificate will 
        /// not be accepted.
        /// </summary>
        /// <param name="targethost">The host to authenticate the certificate against</param>
        /// <param name="clientCerts">A collection of client certificates to use when authenticating the SSL stream</param>
        public async Task ActivateEncryptionAsync(string targethost, X509CertificateCollection clientCerts)
        {
            await ActivateEncryptionAsync(targethost, clientCerts, Client.SslProtocols);
        }
#endif

        /// <summary>
        /// Activates SSL on this stream using the specified protocols. Fires the ValidateCertificate event.
        /// If this event is not handled and there are SslPolicyErrors present, the certificate will 
        /// not be accepted.
        /// </summary>
        /// <param name="targethost">The host to authenticate the certificate against</param>
        /// <param name="clientCerts">A collection of client certificates to use when authenticating the SSL stream</param>
        /// <param name="sslProtocols">A bitwise parameter for supported encryption protocols.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails</exception>
        public void ActivateEncryption(string targethost, X509CertificateCollection clientCerts, SslProtocols sslProtocols)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("The FtpSocketStream object is not connected.");
            }

            if (m_netStream == null)
            {
                throw new InvalidOperationException("The base network stream is null.");
            }

            if (m_sslStream != null)
            {
                throw new InvalidOperationException("SSL Encryption has already been enabled on this stream.");
            }

            try
            {
                DateTime auth_start;
                TimeSpan auth_time_total;

                CreateBufferStream();

#if NETFx
                m_sslStream = new SslStream(GetBufferStream(), true, new RemoteCertificateValidationCallback(
                    delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return OnValidateCertificate(certificate, chain, sslPolicyErrors); }
                ));
#else
                m_sslStream = new FtpSslStream(GetBufferStream(), true, new RemoteCertificateValidationCallback(
                    delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return OnValidateCertificate(certificate, chain, sslPolicyErrors); }
                ));
#endif

                auth_start = DateTime.Now;
                try
                {
#if NETFx
                    m_sslStream.AuthenticateAsClientAsync(targethost, clientCerts, sslProtocols, Client.ValidateCertificateRevocation).Wait();
#else
                    m_sslStream.AuthenticateAsClient(targethost, clientCerts, sslProtocols, Client.ValidateCertificateRevocation);
#endif
                }
                catch (IOException ex)
                {
                    if (ex.InnerException is Win32Exception)
                    {
                        var win32Exception = (Win32Exception)ex.InnerException;
                        if (win32Exception.NativeErrorCode == 10053)
                        {
                            throw new FtpMissingSocketException(ex);
                        }
                    }

                    throw;
                }

                auth_time_total = DateTime.Now.Subtract(auth_start);
                Client.LogStatus(FtpTraceLevel.Info, "FTPS Authentication Successful");
                Client.LogStatus(FtpTraceLevel.Verbose, "Time to activate encryption: " + auth_time_total.Hours + "h " + auth_time_total.Minutes + "m " + auth_time_total.Seconds + "s.  Total Seconds: " + auth_time_total.TotalSeconds + ".");
            }
            catch (AuthenticationException)
            {
                // authentication failed and in addition it left our 
                // ssl stream in an unusable state so cleanup needs
                // to be done and the exception can be re-thrown for
                // handling down the chain. (Add logging?)
                Close();
                Client.LogStatus(FtpTraceLevel.Error, "FTPS Authentication Failed");
                throw;
            }
        }

        /// <summary>
        /// Conditionally create a SSL BufferStream based on the configuration in FtpClient.SslBuffering.
        /// </summary>
        private void CreateBufferStream()
        {
            // Fix: SSL BufferStream is automatically disabled when using FTP proxies, and enabled in all other cases
            // Fix: SSL Buffering is disabled on .NET 5.0 due to issues in .NET framework - See #682
#if NET50
				m_bufStream = null;
#else
            if (Client.SslBuffering == FtpsBuffering.On ||
                Client.SslBuffering == FtpsBuffering.Auto && !Client.IsProxy())
            {
                m_bufStream = new BufferedStream(NetworkStream, 81920);
            }
            else
            {
                m_bufStream = null;
            }
#endif
        }

        /// <summary>
        /// If SSL Buffering is enabled it returns the BufferStream, else returns the internal NetworkStream.
        /// </summary>
        /// <returns></returns>
        private Stream GetBufferStream()
        {
            return m_bufStream != null ? (Stream)m_bufStream : (Stream)NetworkStream;
        }

#if !NET40
        /// <summary>
        /// Activates SSL on this stream using the specified protocols. Fires the ValidateCertificate event.
        /// If this event is not handled and there are SslPolicyErrors present, the certificate will 
        /// not be accepted.
        /// </summary>
        /// <param name="targethost">The host to authenticate the certificate against</param>
        /// <param name="clientCerts">A collection of client certificates to use when authenticating the SSL stream</param>
        /// <param name="sslProtocols">A bitwise parameter for supported encryption protocols.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails</exception>
        public async Task ActivateEncryptionAsync(string targethost, X509CertificateCollection clientCerts, SslProtocols sslProtocols)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("The FtpSocketStream object is not connected.");
            }

            if (m_netStream == null)
            {
                throw new InvalidOperationException("The base network stream is null.");
            }

            if (m_sslStream != null)
            {
                throw new InvalidOperationException("SSL Encryption has already been enabled on this stream.");
            }

            try
            {
                DateTime auth_start;
                TimeSpan auth_time_total;

                CreateBufferStream();

#if NETFx
                m_sslStream = new SslStream(GetBufferStream(), true, new RemoteCertificateValidationCallback(
                    delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return OnValidateCertificate(certificate, chain, sslPolicyErrors); }
                ));
#else
                m_sslStream = new FtpSslStream(GetBufferStream(), true, new RemoteCertificateValidationCallback(
                    delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return OnValidateCertificate(certificate, chain, sslPolicyErrors); }
                ));
#endif

                auth_start = DateTime.Now;
                try
                {
                    await m_sslStream.AuthenticateAsClientAsync(targethost, clientCerts, sslProtocols, Client.ValidateCertificateRevocation);
                }
                catch (IOException ex)
                {
                    if (ex.InnerException is Win32Exception)
                    {
                        var win32Exception = (Win32Exception)ex.InnerException;
                        if (win32Exception.NativeErrorCode == 10053)
                        {
                            throw new FtpMissingSocketException(ex);
                        }
                    }

                    throw;
                }

                auth_time_total = DateTime.Now.Subtract(auth_start);
                Client.LogStatus(FtpTraceLevel.Info, "FTPS Authentication Successful");
                Client.LogStatus(FtpTraceLevel.Verbose, "Time to activate encryption: " + auth_time_total.Hours + "h " + auth_time_total.Minutes + "m " + auth_time_total.Seconds + "s.  Total Seconds: " + auth_time_total.TotalSeconds + ".");
            }
            catch (AuthenticationException)
            {
                // authentication failed and in addition it left our 
                // ssl stream in an unusable state so cleanup needs
                // to be done and the exception can be re-thrown for
                // handling down the chain. (Add logging?)
                Close();
                Client.LogStatus(FtpTraceLevel.Error, "FTPS Authentication Failed");
                throw;
            }
        }
#endif

#if !CORE
        /// <summary>
        /// Deactivates SSL on this stream using the specified protocols and reverts back to plain-text FTP.
        /// </summary>
        public void DeactivateEncryption()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("The FtpSocketStream object is not connected.");
            }

            if (m_sslStream == null)
            {
                throw new InvalidOperationException("SSL Encryption has not been enabled on this stream.");
            }

            m_sslStream.Close();
            m_sslStream = null;
        }
#endif

        /// <summary>
        /// Instructs this stream to listen for connections on the specified address and port
        /// </summary>
        /// <param name="address">The address to listen on</param>
        /// <param name="port">The port to listen on</param>
        public void Listen(IPAddress address, int port)
        {
            if (!IsConnected)
            {
                if (m_socket == null)
                {
                    m_socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                }

                m_socket.Bind(new IPEndPoint(address, port));
                m_socket.Listen(1);
            }
        }

        /// <summary>
        /// Accepts a connection from a listening socket
        /// </summary>
        public void Accept()
        {
            if (m_socket != null)
            {
                m_socket = m_socket.Accept();
            }
        }

#if NET45
		/// <summary>
		/// Accepts a connection from a listening socket
		/// </summary>
		public async Task AcceptAsync() {
			if (m_socket != null) {
				var iar = m_socket.BeginAccept(null, null);
				await Task.Factory.FromAsync(iar, m_socket.EndAccept);
			}
		}
#endif

#if NETFx
        /// <summary>
        /// Accepts a connection from a listening socket
        /// </summary>
        public async Task AcceptAsync()
        {
            if (m_socket != null)
            {
                m_socket = await m_socket.AcceptAsync();
                m_netStream = new NetworkStream(m_socket);
                m_netStream.ReadTimeout = m_readTimeout;
            }
        }
#else
        /// <summary>
        /// Asynchronously accepts a connection from a listening socket
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public IAsyncResult BeginAccept(AsyncCallback callback, object state)
        {
            if (m_socket != null)
            {
                return m_socket.BeginAccept(callback, state);
            }

            return null;
        }

        /// <summary>
        /// Completes a BeginAccept() operation
        /// </summary>
        /// <param name="ar">IAsyncResult returned from BeginAccept</param>
        public void EndAccept(IAsyncResult ar)
        {
            if (m_socket != null)
            {
                m_socket = m_socket.EndAccept(ar);
                m_netStream = new NetworkStream(m_socket);
                m_netStream.ReadTimeout = m_readTimeout;
            }
        }
#endif
        private void BindSocketToLocalIp()
        {
#if !NET40
            if (Client.SocketLocalIp != null)
            {

                var localPort = LocalPorts.GetRandomAvailable(Client.SocketLocalIp);
                var localEndpoint = new IPEndPoint(Client.SocketLocalIp, localPort);

#if DEBUG
                Client.LogStatus(FtpTraceLevel.Verbose, $"Will now bind to {localEndpoint}");
#endif

                this.m_socket.Bind(localEndpoint);
            }
#endif
        }

#if NETFx
        internal SocketAsyncEventArgs BeginAccept()
        {
            var args = new SocketAsyncEventArgs();
            var connectEvent = new ManualResetEvent(false);
            args.UserToken = connectEvent;
            args.Completed += (s, e) => { connectEvent.Set(); };
            if (!m_socket.AcceptAsync(args))
            {
                CheckResult(args);
                return null;
            }

            return args;
        }

        internal void EndAccept(SocketAsyncEventArgs args, int timeout)
        {
            if (args == null)
            {
                return;
            }

            var connectEvent = (ManualResetEvent)args.UserToken;
            if (!connectEvent.WaitOne(timeout))
            {
                Close();
                throw new TimeoutException("Timed out waiting for the server to connect to the active data socket.");
            }

            CheckResult(args);
        }

        private void CheckResult(SocketAsyncEventArgs args)
        {
            if (args.SocketError != SocketError.Success)
            {
                throw new SocketException((int)args.SocketError);
            }

            m_socket = args.AcceptSocket;
            m_netStream = new NetworkStream(args.AcceptSocket);
            m_netStream.ReadTimeout = m_readTimeout;
        }

#endif
    }
#if !NETFx
    /// <summary>
    /// .NET SslStream doesn't close TLS connection properly.
    /// It does not send the close_notify alert before closing the connection.
    /// FtpSslStream uses unsafe code to do that.
    /// This is required when we want to downgrade the connection to plaintext using CCC command.
    /// Thanks to Neco @ https://stackoverflow.com/questions/237807/net-sslstream-doesnt-close-tls-connection-properly/22626756#22626756
    /// </summary>
    internal class FtpSslStream : SslStream
    {
        private bool sentCloseNotify = false;

        public FtpSslStream(Stream innerStream)
            : base(innerStream)
        {
        }

        public FtpSslStream(Stream innerStream, bool leaveInnerStreamOpen)
            : base(innerStream, leaveInnerStreamOpen)
        {
        }

        public FtpSslStream(Stream innerStream, bool leaveInnerStreamOpen, RemoteCertificateValidationCallback userCertificateValidationCallback)
            : base(innerStream, leaveInnerStreamOpen, userCertificateValidationCallback)
        {
        }

        public FtpSslStream(Stream innerStream, bool leaveInnerStreamOpen, RemoteCertificateValidationCallback userCertificateValidationCallback, LocalCertificateSelectionCallback userCertificateSelectionCallback)
            : base(innerStream, leaveInnerStreamOpen, userCertificateValidationCallback, userCertificateSelectionCallback)
        {
        }

#if !NET20 && !NET35
        public FtpSslStream(Stream innerStream, bool leaveInnerStreamOpen, RemoteCertificateValidationCallback userCertificateValidationCallback, LocalCertificateSelectionCallback userCertificateSelectionCallback, EncryptionPolicy encryptionPolicy)
            : base(innerStream, leaveInnerStreamOpen, userCertificateValidationCallback, userCertificateSelectionCallback, encryptionPolicy)
        {
        }

#endif
        public override void Close()
        {
            try
            {
                if (!sentCloseNotify)
                {
                    SslDirectCall.CloseNotify(this);
                    sentCloseNotify = true;
                }
            }
            finally
            {
                base.Close();
            }
        }
    }

    internal static unsafe class SslDirectCall
    {
        /// <summary>
        /// Send an SSL close_notify alert.
        /// </summary>
        /// <param name="sslStream"></param>
        public static void CloseNotify(SslStream sslStream)
        {
            if (sslStream.IsAuthenticated && sslStream.CanWrite)
            {
                var isServer = sslStream.IsServer;

                byte[] result;
                int resultSz;
                var asmbSystem = typeof(System.Net.Authorization).Assembly;

                var SCHANNEL_SHUTDOWN = 1;
                var workArray = BitConverter.GetBytes(SCHANNEL_SHUTDOWN);

                var sslstate = FtpReflection.GetField(sslStream, "_SslState");
                var context = FtpReflection.GetProperty(sslstate, "Context");

                var securityContext = FtpReflection.GetField(context, "m_SecurityContext");
                var securityContextHandleOriginal = FtpReflection.GetField(securityContext, "_handle");
                var securityContextHandle = default(SslNativeApi.SSPIHandle);
                securityContextHandle.HandleHi = (IntPtr)FtpReflection.GetField(securityContextHandleOriginal, "HandleHi");
                securityContextHandle.HandleLo = (IntPtr)FtpReflection.GetField(securityContextHandleOriginal, "HandleLo");

                var credentialsHandle = FtpReflection.GetField(context, "m_CredentialsHandle");
                var credentialsHandleHandleOriginal = FtpReflection.GetField(credentialsHandle, "_handle");
                var credentialsHandleHandle = default(SslNativeApi.SSPIHandle);
                credentialsHandleHandle.HandleHi = (IntPtr)FtpReflection.GetField(credentialsHandleHandleOriginal, "HandleHi");
                credentialsHandleHandle.HandleLo = (IntPtr)FtpReflection.GetField(credentialsHandleHandleOriginal, "HandleLo");

                var bufferSize = 1;
                var securityBufferDescriptor = new SslNativeApi.SecurityBufferDescriptor(bufferSize);
                var unmanagedBuffer = new SslNativeApi.SecurityBufferStruct[bufferSize];

                fixed (SslNativeApi.SecurityBufferStruct* ptr = unmanagedBuffer)
                fixed (void* workArrayPtr = workArray)
                {
                    securityBufferDescriptor.UnmanagedPointer = (void*)ptr;

                    unmanagedBuffer[0].token = (IntPtr)workArrayPtr;
                    unmanagedBuffer[0].count = workArray.Length;
                    unmanagedBuffer[0].type = SslNativeApi.BufferType.Token;

                    SslNativeApi.SecurityStatus status;
                    status = (SslNativeApi.SecurityStatus)SslNativeApi.ApplyControlToken(ref securityContextHandle, securityBufferDescriptor);
                    if (status == SslNativeApi.SecurityStatus.OK)
                    {
                        unmanagedBuffer[0].token = IntPtr.Zero;
                        unmanagedBuffer[0].count = 0;
                        unmanagedBuffer[0].type = SslNativeApi.BufferType.Token;

                        var contextHandleOut = default(SslNativeApi.SSPIHandle);
                        var outflags = SslNativeApi.ContextFlags.Zero;
                        long ts = 0;

                        var inflags = SslNativeApi.ContextFlags.SequenceDetect |
                                      SslNativeApi.ContextFlags.ReplayDetect |
                                      SslNativeApi.ContextFlags.Confidentiality |
                                      SslNativeApi.ContextFlags.AcceptExtendedError |
                                      SslNativeApi.ContextFlags.AllocateMemory |
                                      SslNativeApi.ContextFlags.InitStream;

                        if (isServer)
                        {
                            status = (SslNativeApi.SecurityStatus)SslNativeApi.AcceptSecurityContext(ref credentialsHandleHandle, ref securityContextHandle, null,
                                inflags, SslNativeApi.Endianness.Native, ref contextHandleOut, securityBufferDescriptor, ref outflags, out ts);
                        }
                        else
                        {
                            status = (SslNativeApi.SecurityStatus)SslNativeApi.InitializeSecurityContextW(ref credentialsHandleHandle, ref securityContextHandle, null,
                                inflags, 0, SslNativeApi.Endianness.Native, null, 0, ref contextHandleOut, securityBufferDescriptor, ref outflags, out ts);
                        }

                        if (status == SslNativeApi.SecurityStatus.OK)
                        {
                            var resultArr = new byte[unmanagedBuffer[0].count];
                            Marshal.Copy(unmanagedBuffer[0].token, resultArr, 0, resultArr.Length);
                            Marshal.FreeCoTaskMem(unmanagedBuffer[0].token);
                            result = resultArr;
                            resultSz = resultArr.Length;
                        }
                        else
                        {
                            throw new InvalidOperationException(string.Format("AcceptSecurityContext/InitializeSecurityContextW returned [{0}] during CloseNotify.", status));
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException(string.Format("ApplyControlToken returned [{0}] during CloseNotify.", status));
                    }
                }

                var innerStream = (Stream)FtpReflection.GetProperty(sslstate, "InnerStream");
                innerStream.Write(result, 0, resultSz);
            }
        }
    }

    internal static unsafe class SslNativeApi
    {
        internal enum BufferType
        {
            Empty,
            Data,
            Token,
            Parameters,
            Missing,
            Extra,
            Trailer,
            Header,
            Padding = 9,
            Stream,
            ChannelBindings = 14,
            TargetHost = 16,
            ReadOnlyFlag = -2147483648,
            ReadOnlyWithChecksum = 268435456
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct SSPIHandle
        {
            public IntPtr HandleHi;
            public IntPtr HandleLo;
            public bool IsZero => HandleHi == IntPtr.Zero && HandleLo == IntPtr.Zero;

            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            internal void SetToInvalid()
            {
                HandleHi = IntPtr.Zero;
                HandleLo = IntPtr.Zero;
            }

            public override string ToString()
            {
                return HandleHi.ToString("x") + ":" + HandleLo.ToString("x");
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal class SecurityBufferDescriptor
        {
            public readonly int Version;
            public readonly int Count;
            public unsafe void* UnmanagedPointer;

            public SecurityBufferDescriptor(int count)
            {
                Version = 0;
                Count = count;
                UnmanagedPointer = null;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SecurityBufferStruct
        {
            public int count;
            public BufferType type;
            public IntPtr token;
            public static readonly int Size = sizeof(SecurityBufferStruct);
        }

        internal enum SecurityStatus
        {
            OK,
            ContinueNeeded = 590610,
            CompleteNeeded,
            CompAndContinue,
            ContextExpired = 590615,
            CredentialsNeeded = 590624,
            Renegotiate,
            OutOfMemory = -2146893056,
            InvalidHandle,
            Unsupported,
            TargetUnknown,
            InternalError,
            PackageNotFound,
            NotOwner,
            CannotInstall,
            InvalidToken,
            CannotPack,
            QopNotSupported,
            NoImpersonation,
            LogonDenied,
            UnknownCredentials,
            NoCredentials,
            MessageAltered,
            OutOfSequence,
            NoAuthenticatingAuthority,
            IncompleteMessage = -2146893032,
            IncompleteCredentials = -2146893024,
            BufferNotEnough,
            WrongPrincipal,
            TimeSkew = -2146893020,
            UntrustedRoot,
            IllegalMessage,
            CertUnknown,
            CertExpired,
            AlgorithmMismatch = -2146893007,
            SecurityQosFailed,
            SmartcardLogonRequired = -2146892994,
            UnsupportedPreauth = -2146892989,
            BadBinding = -2146892986
        }

        [Flags]
        internal enum ContextFlags
        {
            Zero = 0,
            Delegate = 1,
            MutualAuth = 2,
            ReplayDetect = 4,
            SequenceDetect = 8,
            Confidentiality = 16,
            UseSessionKey = 32,
            AllocateMemory = 256,
            Connection = 2048,
            InitExtendedError = 16384,
            AcceptExtendedError = 32768,
            InitStream = 32768,
            AcceptStream = 65536,
            InitIntegrity = 65536,
            AcceptIntegrity = 131072,
            InitManualCredValidation = 524288,
            InitUseSuppliedCreds = 128,
            InitIdentify = 131072,
            AcceptIdentify = 524288,
            ProxyBindings = 67108864,
            AllowMissingBindings = 268435456,
            UnverifiedTargetName = 536870912
        }

        internal enum Endianness
        {
            Network,
            Native = 16
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        [DllImport("secur32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern int ApplyControlToken(ref SSPIHandle contextHandle, [In][Out] SecurityBufferDescriptor outputBuffer);

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        [DllImport("secur32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern unsafe int AcceptSecurityContext(ref SSPIHandle credentialHandle, ref SSPIHandle contextHandle, [In] SecurityBufferDescriptor inputBuffer, [In] ContextFlags inFlags, [In] Endianness endianness, ref SSPIHandle outContextPtr, [In][Out] SecurityBufferDescriptor outputBuffer, [In][Out] ref ContextFlags attributes, out long timeStamp);

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        [DllImport("secur32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern unsafe int InitializeSecurityContextW(ref SSPIHandle credentialHandle, ref SSPIHandle contextHandle, [In] byte* targetName, [In] ContextFlags inFlags, [In] int reservedI, [In] Endianness endianness, [In] SecurityBufferDescriptor inputBuffer, [In] int reservedII, ref SSPIHandle outContextPtr, [In][Out] SecurityBufferDescriptor outputBuffer, [In][Out] ref ContextFlags attributes, out long timeStamp);
    }

#endif
    #endregion // Stream
}
