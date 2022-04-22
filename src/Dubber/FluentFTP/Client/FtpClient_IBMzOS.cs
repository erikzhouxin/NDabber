﻿using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Globalization;
using System.Security.Authentication;
using System.Net;
using System.Data.Dubber.Helpers;
using System.Data.Dubber.Proxy;
using System.Web;
using System.Threading;
using System.Data.Dubber.Helpers.Hashing;
using HashAlgos = System.Data.Dubber.Helpers.Hashing.HashAlgorithms;
using System.Threading.Tasks;

namespace System.Data.Dubber
{
    public partial class FtpClient : IDisposable
    {

        #region Get z/OS Realm

        /// <summary>
        /// If an FTP Server has "different realms", in which realm is the
        /// current working directory. 
        /// </summary>
        /// <returns>The realm</returns>
        public FtpZOSListRealm GetZOSListRealm()
        {

            LogFunc(nameof(GetZOSListRealm));

            // this case occurs immediately after connection and after the working dir has changed
            if (_LastWorkingDir == null)
            {
                ReadCurrentWorkingDirectory();
            }

            if (ServerType != FtpServer.IBMzOSFTP)
            {
                return FtpZOSListRealm.Invalid;
            }

            // It is a unix like path (starts with /)
            if (_LastWorkingDir[0] != '\'')
            {
                return FtpZOSListRealm.Unix;
            }

            // Ok, the CWD starts with a single quote. Classic z/OS dataset realm
            FtpReply reply;

#if !CORE14
            lock (m_lock)
            {
#endif
                // Fetch the current working directory. The reply will tell us what it is we are...
                if (!(reply = Execute("CWD " + _LastWorkingDir)).Success)
                {
                    throw new FtpCommandException(reply);
                }
#if !CORE14
            }
#endif
            // 250-The working directory may be a load library                          
            // 250 The working directory "GEEK.PRODUCT.LOADLIB" is a partitioned data set

            if (reply.InfoMessages != null &&
                reply.InfoMessages.Contains("may be a load library"))
            {
                return FtpZOSListRealm.MemberU;
            }

            if (reply.Message.Contains("is a partitioned data set"))
            {
                return FtpZOSListRealm.Member;
            }

            return FtpZOSListRealm.Dataset;
        }

#if ASYNC
		/// <summary>
		/// If an FTP Server has "different realms", in which realm is the
		/// current working directory. 
		/// </summary>
		/// <returns>The realm</returns>
		public async Task<FtpZOSListRealm> GetZOSListRealmAsync(CancellationToken token = default(CancellationToken)) {
			LogFunc(nameof(GetZOSListRealmAsync));

			// this case occurs immediately after connection and after the working dir has changed
			if (_LastWorkingDir == null) {
				await ReadCurrentWorkingDirectoryAsync(token);
			}

			if (ServerType != FtpServer.IBMzOSFTP) {
				return FtpZOSListRealm.Invalid;
			}

			// It is a unix like path (starts with /)
			if (_LastWorkingDir[0] != '\'') {
				return FtpZOSListRealm.Unix;
			}

			// Ok, the CWD starts with a single quote. Classic z/OS dataset realm
			FtpReply reply;

			// Fetch the current working directory. The reply will tell us what it is we are...
			if (!(reply = await ExecuteAsync("CWD " + _LastWorkingDir, token)).Success) {
				throw new FtpCommandException(reply);
			}

			// 250-The working directory may be a load library                          
			// 250 The working directory "GEEK.PRODUCTS.LOADLIB" is a partitioned data set

			if (reply.InfoMessages != null &&
				reply.InfoMessages.Contains("may be a load library")) {
				return FtpZOSListRealm.MemberU;
			}

			if (reply.Message.Contains("is a partitioned data set")) {
				return FtpZOSListRealm.Member;
			}

			return FtpZOSListRealm.Dataset;
		}
#endif
        #endregion

    }
}