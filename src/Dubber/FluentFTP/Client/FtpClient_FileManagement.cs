﻿using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Globalization;
using System.Security.Authentication;
using System.Net;
using System.Data.Dubber.Proxy;
using System.Data.Dubber.Servers;
using System.Data.Dubber.Helpers;
using System.Web;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Dubber
{
    public partial class FtpClient : IFtpClient, IDisposable
    {

        #region Delete File

        /// <summary>
        /// Deletes a file on the server
        /// </summary>
        /// <param name="path">The full or relative path to the file</param>
        public void DeleteFile(string path)
        {
            FtpReply reply;

            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }

#if !CORE14
            lock (m_lock)
            {
#endif
                path = path.GetFtpPath();

                LogFunc(nameof(DeleteFile), new object[] { path });

                if (!(reply = Execute("DELE " + path)).Success)
                {
                    throw new FtpCommandException(reply);
                }

#if !CORE14
            }

#endif
        }

#if ASYNC
		/// <summary>
		/// Deletes a file from the server asynchronously
		/// </summary>
		/// <param name="path">The full or relative path to the file</param>
		/// <param name="token">The token that can be used to cancel the entire process</param>
		public async Task DeleteFileAsync(string path, CancellationToken token = default(CancellationToken)) {
			FtpReply reply;

			// verify args
			if (path.IsBlank()) {
				throw new ArgumentException("Required parameter is null or blank.", "path");
			}

			path = path.GetFtpPath();

			LogFunc(nameof(DeleteFileAsync), new object[] { path });

			if (!(reply = await ExecuteAsync("DELE " + path, token)).Success) {
				throw new FtpCommandException(reply);
			}
		}
#endif

        #endregion

        #region File Exists

        /// <summary>
        /// Checks if a file exists on the server.
        /// </summary>
        /// <param name="path">The full or relative path to the file</param>
        /// <returns>True if the file exists</returns>
        public bool FileExists(string path)
        {
            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }

#if !CORE14
            lock (m_lock)
            {
#endif
                path = path.GetFtpPath();

                LogFunc(nameof(FileExists), new object[] { path });

                // A check for path.StartsWith("/") tells us, even if it is z/OS, we can use the normal unix logic

                // If z/OS: Do not GetAbsolutePath(), unless we have a leading slash
                if (ServerType != FtpServer.IBMzOSFTP || path.StartsWith("/"))
                {
                    // calc the absolute filepath
                    path = GetAbsolutePath(path);
                }

                // since FTP does not include a specific command to check if a file exists
                // here we check if file exists by attempting to get its filesize (SIZE)
                // If z/OS: Do not do SIZE, unless we have a leading slash
                if (HasFeature(FtpCapability.SIZE) && (ServerType != FtpServer.IBMzOSFTP || path.StartsWith("/")))
                {

                    // Fix #328: get filesize in ASCII or Binary mode as required by server
                    var sizeReply = new FtpSizeReply();
                    GetFileSizeInternal(path, sizeReply, -1);

                    // handle known errors to the SIZE command
                    var sizeKnownError = CheckFileExistsBySize(sizeReply);
                    if (sizeKnownError.HasValue)
                    {
                        return sizeKnownError.Value;
                    }
                }

                // check if file exists by attempting to get its date modified (MDTM)
                // If z/OS: Do not do MDTM, unless we have a leading slash
                if (HasFeature(FtpCapability.MDTM) && (ServerType != FtpServer.IBMzOSFTP || path.StartsWith("/")))
                {
                    var reply = Execute("MDTM " + path);
                    var ch = reply.Code[0];
                    if (ch == '2')
                    {
                        return true;
                    }
                    if (ch == '5' && reply.Message.IsKnownError(FtpServerStrings.fileNotFound))
                    {
                        return false;
                    }
                }

                // If z/OS: different handling, unless we have a leading slash
                if (ServerType == FtpServer.IBMzOSFTP && !path.StartsWith("/"))
                {
                    var fileList = GetNameListing(path);
                    return fileList.Count() > 0;
                }
                else
                // check if file exists by getting a name listing (NLST)
                {
                    var fileList = GetNameListing(path.GetFtpDirectoryName());
                    return FileListings.FileExistsInNameListing(fileList, path);
                }

#if !CORE14
            }

#endif
        }

        private bool? CheckFileExistsBySize(FtpSizeReply sizeReply)
        {

            // file surely exists
            if (sizeReply.Reply.Code[0] == '2')
            {
                return true;
            }

            // file surely does not exist
            if (sizeReply.Reply.Code[0] == '5' && sizeReply.Reply.Message.IsKnownError(FtpServerStrings.fileNotFound))
            {
                return false;
            }

            // Fix #518: This check is too broad and must be disabled, need to fallback to MDTM or NLST instead.
            // Fix #179: Add a generic check to since server returns 550 if file not found or no access to file.
            /*if (sizeReply.Reply.Code.Substring(0, 3) == "550") {
				return false;
			}*/

            // fallback to MDTM or NLST
            return null;
        }

#if ASYNC
		/// <summary>
		/// Checks if a file exists on the server asynchronously.
		/// </summary>
		/// <param name="path">The full or relative path to the file</param>
		/// <param name="token">The token that can be used to cancel the entire process</param>
		/// <returns>True if the file exists, false otherwise</returns>
		public async Task<bool> FileExistsAsync(string path, CancellationToken token = default(CancellationToken)) {
			// verify args
			if (path.IsBlank()) {
				throw new ArgumentException("Required parameter is null or blank.", "path");
			}

			path = path.GetFtpPath();

			LogFunc(nameof(FileExistsAsync), new object[] { path });

			// A check for path.StartsWith("/") tells us, even if it is z/OS, we can use the normal unix logic

			// Do not need GetAbsolutePath(path) if z/OS
			if (ServerType != FtpServer.IBMzOSFTP || path.StartsWith("/")) {
				// calc the absolute filepath
				path = await GetAbsolutePathAsync(path, token);
			}

			// since FTP does not include a specific command to check if a file exists
			// here we check if file exists by attempting to get its filesize (SIZE)
			// If z/OS: Do not do SIZE, unless we have a leading slash
			if (HasFeature(FtpCapability.SIZE) && (ServerType != FtpServer.IBMzOSFTP || path.StartsWith("/"))) {

				// Fix #328: get filesize in ASCII or Binary mode as required by server
				FtpSizeReply sizeReply = new FtpSizeReply();
				await GetFileSizeInternalAsync(path, -1, token, sizeReply);

				// handle known errors to the SIZE command
				var sizeKnownError = CheckFileExistsBySize(sizeReply);
				if (sizeKnownError.HasValue) {
					return sizeKnownError.Value;
				}
			}

			// check if file exists by attempting to get its date modified (MDTM)
			// If z/OS: Do not do MDTM, unless we have a leading slash
			if (HasFeature(FtpCapability.MDTM) && (ServerType != FtpServer.IBMzOSFTP || path.StartsWith("/"))) {
				FtpReply reply = await ExecuteAsync("MDTM " + path, token);
				var ch = reply.Code[0];
				if (ch == '2') {
					return true;
				}

				if (ch == '5' && reply.Message.IsKnownError(FtpServerStrings.fileNotFound)) {
					return false;
				}
			}

			// If z/OS: different handling, unless we have a leading slash
			if (ServerType == FtpServer.IBMzOSFTP && !path.StartsWith("/")) {
				var fileList = await GetNameListingAsync(path, token);
				return fileList.Count() > 0;
			}
			else
			// check if file exists by getting a name listing (NLST)
			{
				var fileList = await GetNameListingAsync(path.GetFtpDirectoryName(), token);
				return FileListings.FileExistsInNameListing(fileList, path);
			}
		}
#endif

        #endregion

        #region Rename File/Directory

        /// <summary>
        /// Renames an object on the remote file system.
        /// Low level method that should NOT be used in most cases. Prefer MoveFile() and MoveDirectory().
        /// Throws exceptions if the file does not exist, or if the destination file already exists.
        /// </summary>
        /// <param name="path">The full or relative path to the object</param>
        /// <param name="dest">The new full or relative path including the new name of the object</param>
        public void Rename(string path, string dest)
        {
            FtpReply reply;

            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }

            if (dest.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "dest");
            }

#if !CORE14
            lock (m_lock)
            {
#endif
                path = path.GetFtpPath();
                dest = dest.GetFtpPath();

                LogFunc(nameof(Rename), new object[] { path, dest });

                // calc the absolute filepaths
                path = GetAbsolutePath(path);
                dest = GetAbsolutePath(dest);

                if (!(reply = Execute("RNFR " + path)).Success)
                {
                    throw new FtpCommandException(reply);
                }

                if (!(reply = Execute("RNTO " + dest)).Success)
                {
                    throw new FtpCommandException(reply);
                }

#if !CORE14
            }

#endif
        }

#if ASYNC
		/// <summary>
		/// Renames an object on the remote file system asynchronously.
		/// Low level method that should NOT be used in most cases. Prefer MoveFile() and MoveDirectory().
		/// Throws exceptions if the file does not exist, or if the destination file already exists.
		/// </summary>
		/// <param name="path">The full or relative path to the object</param>
		/// <param name="dest">The new full or relative path including the new name of the object</param>
		/// <param name="token">The token that can be used to cancel the entire process</param>
		public async Task RenameAsync(string path, string dest, CancellationToken token = default(CancellationToken)) {
			FtpReply reply;

			// verify args
			if (path.IsBlank()) {
				throw new ArgumentException("Required parameter is null or blank.", "path");
			}

			if (dest.IsBlank()) {
				throw new ArgumentException("Required parameter is null or blank.", "dest");
			}

			path = path.GetFtpPath();
			dest = dest.GetFtpPath();

			LogFunc(nameof(RenameAsync), new object[] { path, dest });

			// calc the absolute filepaths
			path = await GetAbsolutePathAsync(path, token);
			dest = await GetAbsolutePathAsync(dest, token);

			if (!(reply = await ExecuteAsync("RNFR " + path, token)).Success) {
				throw new FtpCommandException(reply);
			}

			if (!(reply = await ExecuteAsync("RNTO " + dest, token)).Success) {
				throw new FtpCommandException(reply);
			}
		}
#endif

        #endregion

        #region Move File

        /// <summary>
        /// Moves a file on the remote file system from one directory to another.
        /// Always checks if the source file exists. Checks if the dest file exists based on the `existsMode` parameter.
        /// Only throws exceptions for critical errors.
        /// </summary>
        /// <param name="path">The full or relative path to the object</param>
        /// <param name="dest">The new full or relative path including the new name of the object</param>
        /// <param name="existsMode">Should we check if the dest file exists? And if it does should we overwrite/skip the operation?</param>
        /// <returns>Whether the file was moved</returns>
        public bool MoveFile(string path, string dest, FtpRemoteExists existsMode = FtpRemoteExists.Overwrite)
        {
            // verify args
            if (path.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "path");
            }

            if (dest.IsBlank())
            {
                throw new ArgumentException("Required parameter is null or blank.", "dest");
            }

            path = path.GetFtpPath();
            dest = dest.GetFtpPath();

            LogFunc(nameof(MoveFile), new object[] { path, dest, existsMode });

            if (FileExists(path))
            {
                // check if dest file exists and act accordingly
                if (existsMode != FtpRemoteExists.NoCheck)
                {
                    var destExists = FileExists(dest);
                    if (destExists)
                    {
                        switch (existsMode)
                        {
                            case FtpRemoteExists.Overwrite:
                                DeleteFile(dest);
                                break;

                            case FtpRemoteExists.Skip:
                                return false;
                        }
                    }
                }

                // move the file
                Rename(path, dest);

                return true;
            }

            return false;
        }

#if ASYNC
		/// <summary>
		/// Moves a file asynchronously on the remote file system from one directory to another.
		/// Always checks if the source file exists. Checks if the dest file exists based on the `existsMode` parameter.
		/// Only throws exceptions for critical errors.
		/// </summary>
		/// <param name="path">The full or relative path to the object</param>
		/// <param name="dest">The new full or relative path including the new name of the object</param>
		/// <param name="existsMode">Should we check if the dest file exists? And if it does should we overwrite/skip the operation?</param>
		/// <param name="token">The token that can be used to cancel the entire process</param>
		/// <returns>Whether the file was moved</returns>
		public async Task<bool> MoveFileAsync(string path, string dest, FtpRemoteExists existsMode = FtpRemoteExists.Overwrite, CancellationToken token = default(CancellationToken)) {
			// verify args
			if (path.IsBlank()) {
				throw new ArgumentException("Required parameter is null or blank.", "path");
			}

			if (dest.IsBlank()) {
				throw new ArgumentException("Required parameter is null or blank.", "dest");
			}

			path = path.GetFtpPath();
			dest = dest.GetFtpPath();

			LogFunc(nameof(MoveFileAsync), new object[] { path, dest, existsMode });

			if (await FileExistsAsync(path, token)) {
				// check if dest file exists and act accordingly
				if (existsMode != FtpRemoteExists.NoCheck) {
					bool destExists = await FileExistsAsync(dest, token);
					if (destExists) {
						switch (existsMode) {
							case FtpRemoteExists.Overwrite:
								await DeleteFileAsync(dest, token);
								break;

							case FtpRemoteExists.Skip:
								return false;
						}
					}
				}

				// move the file
				await RenameAsync(path, dest, token);

				return true;
			}

			return false;
		}
#endif

        #endregion

    }
}