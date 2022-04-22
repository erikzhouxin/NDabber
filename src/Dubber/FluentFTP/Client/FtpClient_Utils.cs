﻿using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Data.Dubber.Proxy;
using System.Data.Dubber.Servers;
using System.Data.Dubber.Rules;
using System.Data.Dubber.Helpers;
using SysSslProtocols = System.Security.Authentication.SslProtocols;
using System.Web;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Dubber
{
    public partial class FtpClient : IDisposable
    {

        /// <summary>
        /// Performs a bitwise and to check if the specified
        /// flag is set on the <see cref="Capabilities"/>  property.
        /// </summary>
        /// <param name="cap">The <see cref="FtpCapability"/> to check for</param>
        /// <returns>True if the feature was found, false otherwise</returns>
        public bool HasFeature(FtpCapability cap)
        {
            if (cap == FtpCapability.NONE && Capabilities.Count == 0)
            {
                return true;
            }

            return Capabilities.Contains(cap);
        }

        /// <summary>
        /// Retrieves the delegate for the specified IAsyncResult and removes
        /// it from the m_asyncmethods collection if the operation is successful
        /// </summary>
        /// <typeparam name="T">Type of delegate to retrieve</typeparam>
        /// <param name="ar">The IAsyncResult to retrieve the delegate for</param>
        /// <returns>The delegate that generated the specified IAsyncResult</returns>
        protected T GetAsyncDelegate<T>(IAsyncResult ar)
        {
            T func;

            lock (m_asyncmethods)
            {
                if (m_isDisposed)
                {
                    throw new ObjectDisposedException("This connection object has already been disposed.");
                }

                if (!m_asyncmethods.ContainsKey(ar))
                {
                    throw new InvalidOperationException("The specified IAsyncResult could not be located.");
                }

                if (!(m_asyncmethods[ar] is T))
                {
#if CORE
					throw new InvalidCastException("The AsyncResult cannot be matched to the specified delegate. ");
#else
                    var st = new StackTrace(1);

                    throw new InvalidCastException("The AsyncResult cannot be matched to the specified delegate. " + "Are you sure you meant to call " + st.GetFrame(0).GetMethod().Name + " and not another method?"
                    );
#endif
                }

                func = (T)m_asyncmethods[ar];
                m_asyncmethods.Remove(ar);
            }

            return func;
        }

        /// <summary>
        /// Ensure a relative path is absolute by appending the working dir
        /// </summary>
        private string GetAbsolutePath(string path)
        {

            if (path == null || path.Trim().Length == 0)
            {
                // if path not given, then use working dir
                var pwd = GetWorkingDirectory();
                if (pwd != null && pwd.Trim().Length > 0)
                {
                    path = pwd;
                }
                else
                {
                    path = "/";
                }

            }

            // FIX : #153 ensure this check works with unix & windows
            // FIX : #454 OpenVMS paths can be a single character
            else if (!path.StartsWith("/") && !(path.Length > 1 && path[1] == ':'))
            {

                // if its a server-specific absolute path then don't add base dir
                if (ServerHandler != null && ServerHandler.IsAbsolutePath(path))
                {
                    return path;
                }

                // if relative path given then add working dir to calc full path
                var pwd = GetWorkingDirectory();
                if (pwd != null && pwd.Trim().Length > 0 && path != pwd)
                {
                    // Check if PDS (MVS Dataset) file system
                    if (pwd.StartsWith("'") && ServerType == FtpServer.IBMzOSFTP)
                    {
                        // PDS that has single quotes is already fully qualified
                        return pwd;
                    }

                    if (path.StartsWith("./"))
                    {
                        path = path.Remove(0, 2);
                    }

                    path = (pwd + "/" + path).GetFtpPath();
                }
            }

            return path;
        }

#if ASYNC
		/// <summary>
		/// Ensure a relative path is absolute by appending the working dir
		/// </summary>
		private async Task<string> GetAbsolutePathAsync(string path, CancellationToken token) {

			if (path == null || path.Trim().Length == 0) {
				// if path not given, then use working dir
				string pwd = await GetWorkingDirectoryAsync(token);
				if (pwd != null && pwd.Trim().Length > 0) {
					path = pwd;
				}
				else {
					path = "/";
				}
			}

			// FIX : #153 ensure this check works with unix & windows
			// FIX : #454 OpenVMS paths can be a single character
			else if (!path.StartsWith("/") && !(path.Length > 1 && path[1] == ':')) {

				// if its a server-specific absolute path then don't add base dir
				if (ServerHandler != null && ServerHandler.IsAbsolutePath(path)) {
					return path;
				}

				// if relative path given then add working dir to calc full path
				string pwd = await GetWorkingDirectoryAsync(token);
				if (pwd != null && pwd.Trim().Length > 0) {
					if (path.StartsWith("./")) {
						path = path.Remove(0, 2);
					}

					path = (pwd + "/" + path).GetFtpPath();
				}
			}

			return path;
		}
#endif

        private static string DecodeUrl(string url)
        {
#if CORE
			return WebUtility.UrlDecode(url);
#else
            return HttpUtility.UrlDecode(url);
#endif
        }

        /// <summary>
        /// Disables UTF8 support and changes the Encoding property
        /// back to ASCII. If the server returns an error when trying
        /// to turn UTF8 off a FtpCommandException will be thrown.
        /// </summary>
        public void DisableUTF8()
        {
            FtpReply reply;

#if !CORE14
            lock (m_lock)
            {
#endif
                if (!(reply = Execute("OPTS UTF8 OFF")).Success)
                {
                    throw new FtpCommandException(reply);
                }

                m_textEncoding = Encoding.ASCII;
                m_textEncodingAutoUTF = false;
#if !CORE14
            }

#endif
        }

        /// <summary>
        /// Data shouldn't be on the socket, if it is it probably means we've been disconnected.
        /// Read and discard whatever is there and optionally close the connection.
        /// Returns the stale data as text, if any, or null if none was found.
        /// </summary>
        /// <param name="closeStream">close the connection?</param>
        /// <param name="evenEncrypted">even read encrypted data?</param>
        /// <param name="traceData">trace data to logs?</param>
        private string ReadStaleData(bool closeStream, bool evenEncrypted, bool traceData)
        {
            string staleData = null;
            if (m_stream != null && m_stream.SocketDataAvailable > 0)
            {
                if (traceData)
                {
                    LogStatus(FtpTraceLevel.Info, "There is stale data on the socket, maybe our connection timed out or you did not call GetReply(). Re-connecting...");
                }

                if (m_stream.IsConnected && (!m_stream.IsEncrypted || evenEncrypted))
                {
                    var buf = new byte[m_stream.SocketDataAvailable];
                    m_stream.RawSocketRead(buf);
                    staleData = Encoding.GetString(buf).TrimEnd('\r', '\n');
                    if (traceData)
                    {
                        LogStatus(FtpTraceLevel.Verbose, "The stale data was: " + staleData);
                    }
                    if (string.IsNullOrEmpty(staleData))
                    {
                        closeStream = false;
                    }
                }

                if (closeStream)
                {
                    m_stream.Close();
                }
            }
            return staleData;
        }

#if ASYNC
		/// <summary>
		/// Data shouldn't be on the socket, if it is it probably means we've been disconnected.
		/// Read and discard whatever is there and optionally close the connection.
		/// Returns the stale data as text, if any, or null if none was found.
		/// </summary>
		/// <param name="closeStream">close the connection?</param>
		/// <param name="evenEncrypted">even read encrypted data?</param>
		/// <param name="traceData">trace data to logs?</param>
		/// <param name="token">The token that can be used to cancel the entire process</param>
		private async Task<string> ReadStaleDataAsync(bool closeStream, bool evenEncrypted, bool traceData, CancellationToken token) {
			string staleData = null;
			if (m_stream != null && m_stream.SocketDataAvailable > 0) {
				if (traceData) {
					LogStatus(FtpTraceLevel.Info, "There is stale data on the socket, maybe our connection timed out or you did not call GetReply(). Re-connecting...");
				}

				if (m_stream.IsConnected && (!m_stream.IsEncrypted || evenEncrypted)) {
					var buf = new byte[m_stream.SocketDataAvailable];
					await m_stream.RawSocketReadAsync(buf, token);
					staleData = Encoding.GetString(buf).TrimEnd('\r', '\n');
					if (traceData) {
						LogStatus(FtpTraceLevel.Verbose, "The stale data was: " + staleData);
					}
				}

				if (closeStream) {
					m_stream.Close();
				}
			}
			return staleData;
		}
#endif

        /// <summary>
        /// Checks if this FTP/FTPS connection is made through a proxy.
        /// </summary>
        public bool IsProxy()
        {
            return this is FtpClientProxy;
        }

        /// <summary>
        /// Returns true if the file passes all the rules
        /// </summary>
        private bool FilePassesRules(FtpResult result, List<FtpRule> rules, bool useLocalPath, FtpListItem item = null)
        {
            if (rules != null && rules.Count > 0)
            {
                var passes = FtpRule.IsAllAllowed(rules, item ?? result.ToListItem(useLocalPath));
                if (!passes)
                {

                    LogStatus(FtpTraceLevel.Info, "Skipped file due to rule: " + (useLocalPath ? result.LocalPath : result.RemotePath));

                    // mark that the file was skipped due to a rule
                    result.IsSkipped = true;
                    result.IsSkippedByRule = true;

                    // skip uploading the file
                    return false;
                }
            }
            return true;
        }

    }
}
